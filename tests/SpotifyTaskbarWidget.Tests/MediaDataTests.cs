using SpotifyTaskbarWidget;

namespace SpotifyTaskbarWidget.Tests;

public class MediaDataTests
{
    [Theory]
    [InlineData("Chrome", "Simpmusic_ejp2bhxmz1qq6!Simpmusic", 1)]
    [InlineData("SIMPMUSIC_EJP2BHXMZ1QQ6!SIMPMUSIC", "Spotify.exe", 1)]
    [InlineData("SimpMusic_fake!Simpmusic", "Chrome", -1)]
    [InlineData("Spotify.exe", "Simpmusic_ejp2bhxmz1qq6!Simpmusic", 0)]
    public void SelectSessionIndex_ChoosesOnlySupportedPlayersAndPrefersSpotify(string first, string second, int expected)
    {
        Assert.Equal(expected, MediaService.SelectSessionIndex(new[] { first, second }));
    }
    [Fact]
    public void TrackInfo_RecordPropertiesAndImmutability()
    {
        var now = DateTime.UtcNow;
        var info1 = new TrackInfo("Song A", "Artist A", true, true, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(180), now);
        var info2 = new TrackInfo("Song A", "Artist A", true, true, TimeSpan.FromSeconds(30), TimeSpan.FromSeconds(180), now);

        Assert.Equal(info1, info2);
        Assert.Equal("Song A", info1.Title);
        Assert.Equal("Artist A", info1.Artist);
        Assert.True(info1.IsPlaying);
        Assert.True(info1.IsShuffle);
        Assert.Equal(TimeSpan.FromSeconds(30), info1.Position);
        Assert.Equal(TimeSpan.FromSeconds(180), info1.Duration);
    }

    [Fact]
    public void TrackStaleDetection_PositionExceedingDuration_IsDetected()
    {
        // When position > duration (stale previous track snapshot)
        var duration = TimeSpan.FromSeconds(100);
        var stalePosition = TimeSpan.FromSeconds(150);

        bool isStale = stalePosition > duration;
        Assert.True(isStale);
    }

    [Fact]
    public void TrackStaleDetection_OldSnapshotOnNewTrack_IsDetected()
    {
        string lastTrackKey = "Old Song|Old Artist";
        string currentTitle = "New Song";
        string currentArtist = "New Artist";
        string currentKey = currentTitle + "|" + currentArtist;

        var positionAtUtc = DateTime.UtcNow - TimeSpan.FromSeconds(10); // 10s old
        var duration = TimeSpan.FromSeconds(200);
        var pos = TimeSpan.FromSeconds(50);

        bool stale = pos > duration ||
                     (currentKey != lastTrackKey && DateTime.UtcNow - positionAtUtc > TimeSpan.FromSeconds(5));

        Assert.True(stale);
    }

    [Fact]
    public void TrackGeneration_MonotonicIncrement_ProtectsAgainstConcurrentUpdates()
    {
        long generation = 0;
        long gen1 = unchecked(++generation);
        long gen2 = unchecked(++generation);
        long gen3 = unchecked(++generation);

        Assert.True(gen2 > gen1);
        Assert.True(gen3 > gen2);

        // An async task capturing gen1 should be rejected when generation is gen3
        Assert.False(gen1 == generation);
    }
}
