using System.Text.Json;
using SpotifyTaskbarWidget;

namespace SpotifyTaskbarWidget.Tests;

public class SettingsTests
{
    [Fact]
    public void WidgetSettings_DefaultValues_AreSensible()
    {
        var s = new WidgetSettings();
        Assert.True(s.AutoPosition);
        Assert.Equal(0, s.MonitorIndex);
        Assert.Equal(1.0, s.Scale);
        Assert.Equal(1.0, s.Opacity);
        Assert.True(s.ShowPlay);
        Assert.True(s.ShowLike);
        Assert.True(s.ShowShuffle);
        Assert.True(s.ShowPrev);
        Assert.True(s.ShowNext);
        Assert.True(s.ShowRepeat);
        Assert.True(s.ShowVolume);
    }

    [Fact]
    public void WidgetSettings_LegacyJsonDeserialization_PreservesValues()
    {
        // Old JSON format with legacy fields
        string legacyJson = """
        {
            "AutoPosition": false,
            "X": 450.5,
            "MonitorIndex": 1,
            "Opacity": 0.05,
            "Scale": 1.1
        }
        """;

        var s = JsonSerializer.Deserialize<WidgetSettings>(legacyJson)!;
        s.Monitors ??= new List<int>();
        s.ManualX ??= new Dictionary<int, double>();

        if (s.Monitors.Count == 0)
            s.Monitors.Add(Math.Max(0, s.MonitorIndex));
        s.Monitors = s.Monitors.Where(i => i >= 0).Distinct().OrderBy(i => i).ToList();

        if (!s.AutoPosition && s.ManualX.Count == 0)
            s.ManualX[s.MonitorIndex] = s.X;

        s.Opacity = Math.Clamp(s.Opacity, 0.2, 1.0);

        Assert.Equal(1, s.Monitors.Single());
        Assert.Equal(450.5, s.ManualX[1]);
        // Opacity should be clamped to at least 0.20
        Assert.Equal(0.20, s.Opacity);
        Assert.Equal(1.1, s.Scale);
    }
}
