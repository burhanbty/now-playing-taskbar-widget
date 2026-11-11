using Windows.Media;
using Windows.Media.Control;
using Windows.Storage.Streams;

namespace SpotifyTaskbarWidget;

public sealed record TrackInfo(string Title, string Artist, bool IsPlaying, bool? IsShuffle,
    TimeSpan Position, TimeSpan Duration, DateTime PositionAtUtc)
{
    internal MediaControls Controls { get; init; }
    internal MediaPlaybackAutoRepeatMode? Repeat { get; init; }
}

[Flags]
internal enum MediaControls
{
    None = 0, PlayPause = 1, Previous = 2, Next = 4, Seek = 8, Shuffle = 16, Repeat = 32,
}

/// <summary>
/// Lê a música atual através da API de media do Windows (SMTC).
/// Prefere Spotify e aceita a sessão do SimpMusic instalado pela Store.
/// </summary>
public sealed class MediaService
{
    private GlobalSystemMediaTransportControlsSessionManager? _manager;
    private volatile GlobalSystemMediaTransportControlsSession? _session;
    private readonly CancellationTokenSource _initCts = new();
    private bool _disposed;
    private readonly object _pickLock = new();

    public bool IsDisposed => _disposed;
    internal GlobalSystemMediaTransportControlsSession? SelectedSession => _session;

    internal static bool IsSimpMusicSource(string? source) =>
        string.Equals(source, "Simpmusic_ejp2bhxmz1qq6!Simpmusic", StringComparison.OrdinalIgnoreCase);

    internal static int SelectSessionIndex(IReadOnlyList<string?> sources)
    {
        int simpMusic = -1;
        for (int i = 0; i < sources.Count; i++)
        {
            if (sources[i]?.Contains("spotify", StringComparison.OrdinalIgnoreCase) == true)
                return i;
            if (simpMusic < 0 && IsSimpMusicSource(sources[i]))
                simpMusic = i;
        }
        return simpMusic;
    }

    /// <summary>Disparado (em thread de background) quando a faixa ou o estado mudam.</summary>
    public event Action? Changed;

    /// <summary>Disparado só quando a posição/duração mudam — acontece a cada poucos
    /// segundos, por isso o tratamento tem de ser leve.</summary>
    public event Action? TimelineChanged;

    public async Task InitializeAsync()
    {
        var token = _initCts.Token;
        // No arranque com o Windows o WinRT pode ainda não estar pronto — uma
        // falha transitória aqui deixava o widget sem faixa a sessão INTEIRA.
        // Insistir com recuo (4s→64s, ~2 min) antes de desistir de vez.
        for (int attempt = 0; ; attempt++)
        {
            if (_disposed || token.IsCancellationRequested) return;
            try
            {
                _manager = await GlobalSystemMediaTransportControlsSessionManager.RequestAsync();
                break;
            }
            catch (Exception ex)
            {
                if (attempt >= 5)
                {
                    // Sem a API de sessões de media (edições N sem Media Feature
                    // Pack, WinRT avariado) — deixar rasto para o report
                    Diag.Once("smtc-init", "Media session API unavailable (this is why nothing shows as playing): " + ex.Message);
                    return;
                }
                try
                {
                    await Task.Delay(TimeSpan.FromSeconds(4 << attempt), token);
                }
                catch (OperationCanceledException)
                {
                    return;
                }
            }
        }

        if (_disposed || token.IsCancellationRequested) return;

        if (_manager != null)
        {
            _manager.SessionsChanged += OnSessionsChanged;
            PickSession();
        }
    }

    private void OnSessionsChanged(GlobalSystemMediaTransportControlsSessionManager sender, SessionsChangedEventArgs args)
    {
        if (_disposed) return;
        PickSession();
    }

    /// <summary>Dessubscreve tudo — sem isto, uma janela fechada ficava presa
    /// na memória pelos eventos WinRT e continuava a processar sessões.</summary>
    public void Shutdown()
    {
        lock (_pickLock)
        {
            if (_disposed) return;
            _disposed = true;

            try
            {
                _initCts.Cancel();
                _initCts.Dispose();
            }
            catch { }

            if (_manager != null)
            {
                try
                {
                    _manager.SessionsChanged -= OnSessionsChanged;
                }
                catch { }
            }

            var old = _session;
            _session = null;
            if (old != null)
            {
                try
                {
                    old.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                    old.PlaybackInfoChanged -= OnPlaybackInfoChanged;
                    old.TimelinePropertiesChanged -= OnTimelineChanged;
                }
                catch { }
            }

            Changed = null;
            TimelineChanged = null;
        }
    }

    private void PickSession()
    {
        if (_disposed || _manager == null) return;

        GlobalSystemMediaTransportControlsSession? chosen = null;
        try
        {
            var sessions = _manager.GetSessions();
            var sources = sessions.Select(s => s.SourceAppUserModelId).ToArray();
            int index = SelectSessionIndex(sources);
            if (index >= 0) chosen = sessions[index];
            // Há sessões mas nenhuma é do Spotify: só é anómalo se o Spotify
            // estiver mesmo a correr (Chrome/YouTube com o Spotify fechado é o
            // dia-a-dia normal — registá-lo enchia o log de falsos erros)
            if (chosen == null && sessions.Count > 0)
            {
                var procs = System.Diagnostics.Process.GetProcessesByName("Spotify");
                bool spotifyRunning = procs.Length > 0;
                foreach (var p in procs) p.Dispose();
                if (spotifyRunning)
                    Diag.Once("no-spotify-session",
                        "Media sessions present but none matches Spotify: " +
                        string.Join(", ", sessions.Select(x => x.SourceAppUserModelId ?? "(null)")));
            }
        }
        catch (Exception ex)
        {
            Diag.Once("get-sessions", "Reading media sessions failed: " + ex.Message);
        }

        // SessionsChanged chega em threads WinRT em rajadas (arranque/fecho do
        // Spotify): sem lock, duas trocas entrelaçadas duplicavam subscrições
        // ou deixavam handlers presos numa sessão morta
        lock (_pickLock)
        {
            if (_disposed) return;

            var old = _session;
            if (ReferenceEquals(old, chosen))
            {
                if (chosen == null) Changed?.Invoke();
                return;
            }
            if (old != null)
            {
                try
                {
                    old.MediaPropertiesChanged -= OnMediaPropertiesChanged;
                    old.PlaybackInfoChanged -= OnPlaybackInfoChanged;
                    old.TimelinePropertiesChanged -= OnTimelineChanged;
                }
                catch { }
            }

            _session = chosen;
            if (chosen != null)
            {
                chosen.MediaPropertiesChanged += OnMediaPropertiesChanged;
                chosen.PlaybackInfoChanged += OnPlaybackInfoChanged;
                chosen.TimelinePropertiesChanged += OnTimelineChanged;
            }
        }

        Changed?.Invoke();
    }

    private void OnMediaPropertiesChanged(GlobalSystemMediaTransportControlsSession sender, MediaPropertiesChangedEventArgs args)
    {
        if (!_disposed) Changed?.Invoke();
    }

    private void OnPlaybackInfoChanged(GlobalSystemMediaTransportControlsSession sender, PlaybackInfoChangedEventArgs args)
    {
        if (!_disposed) Changed?.Invoke();
    }

    private void OnTimelineChanged(GlobalSystemMediaTransportControlsSession sender, TimelinePropertiesChangedEventArgs args)
    {
        if (!_disposed) TimelineChanged?.Invoke();
    }

    /// <summary>Leitura rápida da timeline (sem propriedades da faixa nem capa).</summary>
    internal (TimeSpan Position, TimeSpan Duration, bool IsPlaying, DateTime PositionAtUtc)? GetTimeline(
        GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null) return null;
        try
        {
            var tl = s.GetTimelineProperties();
            var pi = s.GetPlaybackInfo();
            return (tl?.Position ?? TimeSpan.Zero,
                    tl != null ? tl.EndTime - tl.StartTime : TimeSpan.Zero,
                    pi?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing,
                    tl?.LastUpdatedTime.UtcDateTime ?? DateTime.UtcNow);
        }
        catch
        {
            return null;
        }
    }

    internal async Task<TrackInfo?> GetTrackAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null) return null;
        try
        {
            var props = await s.TryGetMediaPropertiesAsync();
            var pi = s.GetPlaybackInfo();
            bool playing = pi?.PlaybackStatus ==
                           GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing;

            var tl = s.GetTimelineProperties();
            TimeSpan position = tl?.Position ?? TimeSpan.Zero;
            TimeSpan duration = tl != null ? tl.EndTime - tl.StartTime : TimeSpan.Zero;
            DateTime positionAt = tl?.LastUpdatedTime.UtcDateTime ?? DateTime.UtcNow;

            var track = new TrackInfo(props?.Title ?? "", props?.Artist ?? "", playing, pi?.IsShuffleActive,
                position, duration, positionAt);
            if (!IsSimpMusicSource(s.SourceAppUserModelId)) return track;

            var controls = pi?.Controls;
            MediaControls available = MediaControls.None;
            if (controls != null)
            {
                if (controls.IsPlayPauseToggleEnabled || (playing ? controls.IsPauseEnabled : controls.IsPlayEnabled))
                    available |= MediaControls.PlayPause;
                if (controls.IsPreviousEnabled) available |= MediaControls.Previous;
                if (controls.IsNextEnabled) available |= MediaControls.Next;
                if (controls.IsPlaybackPositionEnabled) available |= MediaControls.Seek;
                if (controls.IsShuffleEnabled && pi?.IsShuffleActive != null) available |= MediaControls.Shuffle;
                if (controls.IsRepeatEnabled && pi?.AutoRepeatMode != null) available |= MediaControls.Repeat;
            }
            return track with { Controls = available, Repeat = pi?.AutoRepeatMode };
        }
        catch (Exception ex)
        {
            Diag.Once("get-track", "Reading track from the selected media session failed: " + ex.Message);
            return null;
        }
    }

    internal async Task<byte[]?> GetThumbnailAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null) return null;
        try
        {
            var props = await s.TryGetMediaPropertiesAsync();
            if (props?.Thumbnail == null) return null;

            using var stream = await props.Thumbnail.OpenReadAsync();
            if (stream.Size == 0) return null;

            var bytes = new byte[stream.Size];
            using var reader = new DataReader(stream.GetInputStreamAt(0));
            await reader.LoadAsync((uint)stream.Size);
            reader.ReadBytes(bytes);
            return bytes;
        }
        catch
        {
            return null;
        }
    }

    internal async Task TogglePlayPauseAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null || !ReferenceEquals(s, _session)) return;
        try
        {
            if (!IsSimpMusicSource(s.SourceAppUserModelId))
            {
                await s.TryTogglePlayPauseAsync();
                return;
            }

            var playback = s.GetPlaybackInfo();
            if (playback?.Controls?.IsPlayPauseToggleEnabled == true)
                await s.TryTogglePlayPauseAsync();
            else if (playback?.PlaybackStatus == GlobalSystemMediaTransportControlsSessionPlaybackStatus.Playing)
                await s.TryPauseAsync();
            else
                await s.TryPlayAsync();
        }
        catch { }
    }

    internal async Task NextAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null || !ReferenceEquals(s, _session)) return;
        try { await s.TrySkipNextAsync(); } catch { }
    }

    internal async Task PreviousAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null || !ReferenceEquals(s, _session)) return;
        try { await s.TrySkipPreviousAsync(); } catch { }
    }

    internal async Task SeekAsync(GlobalSystemMediaTransportControlsSession? s, TimeSpan position)
    {
        if (s == null || !ReferenceEquals(s, _session)) return;
        try { await s.TryChangePlaybackPositionAsync(position.Ticks); } catch { }
    }

    internal async Task CycleRepeatAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null || !ReferenceEquals(s, _session)) return;
        try
        {
            var current = s.GetPlaybackInfo()?.AutoRepeatMode ?? Windows.Media.MediaPlaybackAutoRepeatMode.None;
            var next = current switch
            {
                Windows.Media.MediaPlaybackAutoRepeatMode.None => Windows.Media.MediaPlaybackAutoRepeatMode.List,
                Windows.Media.MediaPlaybackAutoRepeatMode.List => Windows.Media.MediaPlaybackAutoRepeatMode.Track,
                _ => Windows.Media.MediaPlaybackAutoRepeatMode.None,
            };
            await s.TryChangeAutoRepeatModeAsync(next);
        }
        catch { }
    }

    internal async Task ToggleShuffleAsync(GlobalSystemMediaTransportControlsSession? s)
    {
        if (s == null || !ReferenceEquals(s, _session)) return;
        try
        {
            bool current = s.GetPlaybackInfo()?.IsShuffleActive ?? false;
            await s.TryChangeShuffleActiveAsync(!current);
        }
        catch { }
    }
}
