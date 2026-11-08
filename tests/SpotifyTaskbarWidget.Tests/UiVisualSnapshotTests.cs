using System.IO;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using SpotifyTaskbarWidget;

namespace SpotifyTaskbarWidget.Tests;

public class UiVisualSnapshotTests
{
    [Fact]
    public void RenderWidgetVisualSnapshots_ProducesValidPreviews()
    {
        Exception? threadEx = null;
        var thread = new Thread(() =>
        {
            try
            {
                string outDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "screenshots"));
                Directory.CreateDirectory(outDir);

                // 1. Dark Preview
                RenderSnapshot(isLight: false, Path.Combine(outDir, "fluent_dark_preview.png"));

                // 2. Light Preview
                RenderSnapshot(isLight: true, Path.Combine(outDir, "fluent_light_preview.png"));
            }
            catch (Exception ex)
            {
                threadEx = ex;
            }
        });

        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        thread.Join(TimeSpan.FromSeconds(15));

        if (threadEx != null)
        {
            throw new InvalidOperationException("Failed to render visual snapshot: " + threadEx.Message, threadEx);
        }

        string outDir = Path.GetFullPath(Path.Combine(AppContext.BaseDirectory, "..", "..", "..", "..", "..", "docs", "screenshots"));
        string darkPath = Path.Combine(outDir, "fluent_dark_preview.png");
        string lightPath = Path.Combine(outDir, "fluent_light_preview.png");

        Assert.True(File.Exists(darkPath) && new FileInfo(darkPath).Length > 0);
        Assert.True(File.Exists(lightPath) && new FileInfo(lightPath).Length > 0);
    }

    private static void RenderSnapshot(bool isLight, string outPath)
    {
        var window = new MainWindow();

        var titleText = (TextBlock)window.FindName("TitleText");
        var artistText = (TextBlock)window.FindName("ArtistText");
        var contentPanel = (StackPanel)window.FindName("ContentPanel");
        var launcherPanel = (StackPanel)window.FindName("LauncherPanel");
        var progressTrack = (Border)window.FindName("ProgressTrack");
        var progressFill = (Border)window.FindName("ProgressFill");
        var playPauseBtn = (Button)window.FindName("PlayPauseButton");
        var playPauseIcon = (System.Windows.Shapes.Path)window.FindName("PlayPauseIcon");
        var root = (Border)window.FindName("Root");

        titleText.Text = "Blinding Lights";
        artistText.Text = "The Weeknd";
        contentPanel.Visibility = Visibility.Visible;
        launcherPanel.Visibility = Visibility.Collapsed;
        progressFill.Width = 85;

        if (isLight)
        {
            // Apply light theme colors
            var lightPrimary = new SolidColorBrush(Color.FromRgb(0x1A, 0x1A, 0x1A));
            var lightSubdued = new SolidColorBrush(Color.FromRgb(0x5C, 0x5C, 0x5C));
            titleText.Foreground = lightPrimary;
            artistText.Foreground = lightSubdued;
            progressTrack.Background = new SolidColorBrush(Color.FromArgb(0x18, 0x00, 0x00, 0x00));
            progressFill.Background = lightPrimary;
            playPauseBtn.Background = lightPrimary;
            playPauseIcon.Fill = Brushes.White;
        }

        // Layout
        root.Measure(new Size(400, 50));
        root.Arrange(new Rect(0, 0, Math.Max(320, root.DesiredSize.Width), 42));
        root.UpdateLayout();

        int width = (int)Math.Max(320, Math.Ceiling(root.ActualWidth));
        int height = (int)Math.Max(42, Math.Ceiling(root.ActualHeight));

        var rtb = new RenderTargetBitmap(width, height, 96, 96, PixelFormats.Pbgra32);
        rtb.Render(root);

        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(rtb));

        using (var stream = File.Create(outPath))
        {
            encoder.Save(stream);
        }

        window.Close();
    }
}
