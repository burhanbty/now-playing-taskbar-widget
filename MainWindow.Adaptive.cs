using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;

namespace SpotifyTaskbarWidget;

public partial class MainWindow
{
    private int _expandedGeneration;
    private IntPtr _expandedHwnd;

    private void InitializeAdaptiveUi()
    {
        ExpandedPopup.Resources.MergedDictionaries.Add(Resources);
        VolumePopup.Opened += AdaptiveVolumeOpened;
        UpdateExpandedVisibility();
    }

    private void ApplyAdaptiveLanguage()
    {
        MoreButton.ToolTip = ArtworkDetailsButton.ToolTip = L.ShowControls;
        CloseDetailsButton.ToolTip = L.CloseControls;
        ExpandedPlayButton.ToolTip = L.TipPlayPause;
        ExpandedNextButton.ToolTip = L.TipNext;
        OpenPlayerButton.ToolTip = L.OpenSpotify;
        NowPlayingLabel.Text = L.NowPlaying;
    }

    internal void UpdateExpandedVisibility()
    {
        SetVisible(ExpandedPlayButton, _settings.ShowPlay && CanControl(MediaControls.PlayPause));
        SetVisible(ExpandedNextButton, _settings.ShowNext && CanControl(MediaControls.Next));
        SetVisible(PrevButton, _settings.ShowPrev && CanControl(MediaControls.Previous));
        SetVisible(LikeButton, _settings.ShowLike && !_isSimpMusic);
        SetVisible(ShuffleButton, _settings.ShowShuffle && CanControl(MediaControls.Shuffle));
        SetVisible(RepeatButton, _settings.ShowRepeat && CanControl(MediaControls.Repeat));
        SetVisible(VolumeButton, _settings.ShowVolume);
        ExpandedSurface.LayoutTransform = new ScaleTransform(_settings.Scale, _settings.Scale);
        ExpandedSurface.Opacity = _settings.Opacity;
    }

    internal void UpdatePlayerControls(bool isSimpMusic, MediaControls controls)
    {
        if (_isSimpMusic == isSimpMusic && _simpMusicControls == controls) return;
        _isSimpMusic = isSimpMusic;
        _simpMusicControls = controls;
        OpenPlayerLabel.Text = isSimpMusic ? "SimpMusic" : "Spotify";
        OpenPlayerButton.ToolTip = isSimpMusic ? L.OpenSimpMusic : L.OpenSpotify;
        VolumeButton.ToolTip = isSimpMusic ? L.TipSimpMusicVolume : L.TipVolume;
        ProgressTrack.Cursor = CanControl(MediaControls.Seek) ? Cursors.Hand : Cursors.Arrow;
        UpdateExpandedVisibility();
    }

    private void OpenSelectedPlayer_Click(object sender, RoutedEventArgs e) => OpenSelectedPlayer();

    private void OpenSelectedPlayer()
    {
        if (!IsDisplayedSessionCurrent) return;
        if (_isSimpMusic)
            SpotifyActions.OpenRunningPlayerWindow("SimpMusic");
        else
            SpotifyActions.OpenSpotifyWindow();
    }

    private static void SetVisible(UIElement element, bool visible) =>
        element.Visibility = visible ? Visibility.Visible : Visibility.Collapsed;

    private void Details_Click(object sender, RoutedEventArgs e)
    {
        e.Handled = true;
        if (_moveMode || _closed || _rideAnimating || !_playerPresent || Visibility != Visibility.Visible)
            return;
        if (ExpandedPopup.IsOpen) { CloseExpanded(); return; }
        CaptureForeground();
        UpdateExpandedVisibility();
        ExpandedPopup.IsOpen = true;
    }

    private void CloseDetails_Click(object sender, RoutedEventArgs e) => CloseExpanded();

    private void CloseExpanded()
    {
        _expandedGeneration++; // invalidate a pending volume read even before Popup.Closed is delivered
        ExpandedSurface.BeginAnimation(OpacityProperty, null);
        if (ExpandedSurface.RenderTransform is TranslateTransform translation)
            translation.BeginAnimation(TranslateTransform.YProperty, null);
        ExpandedPopup.IsOpen = false;
        VolumePopup.IsOpen = false;
    }

    private void ExpandedSurface_PreviewKeyDown(object sender, KeyEventArgs e)
    {
        if (e.Key != Key.Escape) return;
        e.Handled = true;
        CloseExpanded();
    }

    private void ExpandedPopup_Opened(object? sender, EventArgs e)
    {
        if (_closed || !ExpandedPopup.IsOpen) { CloseExpanded(); return; }
        _expandedGeneration++;
        if (PresentationSource.FromVisual(ExpandedSurface) is HwndSource source)
        {
            _expandedHwnd = source.Handle;
            int style = Interop.GetWindowLong(source.Handle, Interop.GWL_EXSTYLE);
            Interop.SetWindowLong(source.Handle, Interop.GWL_EXSTYLE,
                style | Interop.WS_EX_NOACTIVATE | Interop.WS_EX_TOOLWINDOW);
            Interop.EnsureTopmost(source.Handle);
        }
        UpdateProgressUi();
        AnimateDetailsEntry();
    }

    private void ExpandedPopup_Closed(object? sender, EventArgs e)
    {
        _expandedGeneration++;
        VolumePopup.IsOpen = false;
        ExpandedPopup.StaysOpen = false;
        // Light-dismiss may be a deliberate click into another application.
        IntPtr foreground = Interop.GetForegroundWindow();
        if (foreground == _expandedHwnd || foreground == _hwnd || foreground == _fgBeforeUi)
            RestoreForeground();
        else
            _fgBeforeUi = IntPtr.Zero;
        _expandedHwnd = IntPtr.Zero;
        if (!_closed && _hwnd != IntPtr.Zero) ReassertTopmost();
    }

    private void AdaptiveVolumeOpened(object? sender, EventArgs e)
    {
        // Keep the outer panel alive while its nested native popup owns mouse capture.
        if (ExpandedPopup.IsOpen) ExpandedPopup.StaysOpen = true;
    }

    private void AnimateDetailsEntry()
    {
        ExpandedSurface.BeginAnimation(OpacityProperty, null);
        ExpandedSurface.RenderTransform = new TranslateTransform();
        if (_motionEnabled == false) return;
        ExpandedSurface.BeginAnimation(OpacityProperty, new DoubleAnimation(0, _settings.Opacity,
            TimeSpan.FromMilliseconds(160)) { FillBehavior = FillBehavior.Stop });
        ((TranslateTransform)ExpandedSurface.RenderTransform).BeginAnimation(TranslateTransform.YProperty,
            new DoubleAnimation(5, 0, TimeSpan.FromMilliseconds(160))
            {
                EasingFunction = new CubicEase { EasingMode = EasingMode.EaseOut },
                FillBehavior = FillBehavior.Stop
            });
    }

    private void Root_MouseEnter(object sender, MouseEventArgs e) => SetGlassHighlight(true);
    private void Root_MouseLeave(object sender, MouseEventArgs e) => SetGlassHighlight(false);

    private void Root_PreviewMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
    {
        if (!_moveMode) return;
        Root_MouseLeftButtonDown(sender, e);
        e.Handled = true;
    }

    private void CloseVolumeAfterPointerExit()
    {
        VolumePopup.IsOpen = false;
        if (ExpandedPopup.IsOpen && !ExpandedSurface.IsMouseOver && !Root.IsMouseOver)
            CloseExpanded();
    }

    private void SetGlassHighlight(bool hovered)
    {
        double from = GlassHover.Opacity;
        GlassHover.BeginAnimation(OpacityProperty, null);
        double target = hovered ? 1 : 0;
        if (_motionEnabled == false) { GlassHover.Opacity = target; return; }
        var animation = new DoubleAnimation(from, target, TimeSpan.FromMilliseconds(140));
        GlassHover.Opacity = target;
        GlassHover.BeginAnimation(OpacityProperty, animation);
    }

    private static string FormatTimeline(TimeSpan time) => time.TotalHours >= 1
        ? $"{(int)time.TotalHours}:{time.Minutes:00}:{time.Seconds:00}"
        : $"{(int)time.TotalMinutes}:{time.Seconds:00}";
}
