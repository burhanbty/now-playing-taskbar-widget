using System.IO;
using System.Reflection;
using System.Runtime.ExceptionServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Imaging;
using System.Windows.Threading;
using ShapePath = System.Windows.Shapes.Path;

namespace SpotifyTaskbarWidget.Tests;

// Never show MainWindow: Loaded would attach to taskbars, media and the user's profile.
public class UiVisualSnapshotTests
{
    [Fact]
    public void RenderLiquidGlassDesignStudies() => WithWindow(window =>
    {
        string path = Path.GetFullPath(Path.Combine(OutputDirectory, "..", "..", "..", "docs", "design", "phase25", "Variants.xaml"));
        using var file = File.OpenRead(path);
        var designs = (ResourceDictionary)System.Windows.Markup.XamlReader.Load(file);
        Directory.CreateDirectory(OutputDirectory);
        foreach (bool light in new[] { false, true })
        {
            window.ApplyTheme(light);
            foreach (string name in new[] { "Tidal", "Prism" })
            {
                var sample = new ContentControl { Template = (ControlTemplate)designs[name] };
                sample.Resources.MergedDictionaries.Add(window.Resources);
                Render(sample, light, 2, $"study_{name}_{(light ? "light" : "dark")}.png");
            }
        }
    });
    private static string OutputDirectory => Path.GetFullPath(Path.Combine(
        AppContext.BaseDirectory, "..", "..", "..", "..", "..", "obj", "phase25", "visuals"));

    [Fact]
    public void RenderWidgetVisualSnapshots_ProducesValidPreviews() => WithWindow(window =>
    {
        Directory.CreateDirectory(OutputDirectory);
        Get<TextBlock>(window, "TitleText").Text = "Midnight / Gece Yolculuğu";
        Get<TextBlock>(window, "ArtistText").Text = "Synthetic preview artist";
        Get<Border>(window, "ProgressFill").Width = 85;
        Get<TextBlock>(window, "PositionText").Text = "1:12";
        Get<TextBlock>(window, "DurationText").Text = "3:46";
        foreach (bool light in new[] { false, true })
        {
            window.ApplyTheme(light);
            foreach (double dpi in new[] { 1.0, 1.25, 1.5, 2.0 })
            {
                string prefix = $"{(light ? "light" : "dark")}_{dpi * 100:0}";
                Assert.True(Responsive(window, 420));
                Render(Get<Border>(window, "Root"), light, dpi, prefix + "_fallback.png");
                // Rectangular synthetic artwork exercises centered UniformToFill cropping.
                var drawing = new DrawingVisual();
                using (var dc = drawing.RenderOpen())
                {
                    dc.DrawRectangle(new LinearGradientBrush(Colors.Teal, Colors.MidnightBlue, 45), null, new Rect(0, 0, 96, 48));
                    dc.DrawEllipse(Brushes.PaleTurquoise, null, new Point(48, 24), 15, 15);
                }
                var bitmap = new RenderTargetBitmap(96, 48, 96, 96, PixelFormats.Pbgra32);
                bitmap.Render(drawing);
                Get<ImageBrush>(window, "ArtBrush").ImageSource = bitmap;
                Get<Border>(window, "ArtImage").Visibility = Visibility.Visible;
                Get<ShapePath>(window, "ArtPlaceholder").Visibility = Visibility.Collapsed;
                Render(Get<Border>(window, "Root"), light, dpi, prefix + "_art.png");
                Render(Get<Border>(window, "ExpandedSurface"), light, dpi, prefix + "_expanded.png");
                Assert.Equal("Midnight / Gece Yolculuğu", Get<TextBlock>(window, "ExpandedTitle").Text);
                Assert.True(Responsive(window, 188));
                Render(Get<Border>(window, "Root"), light, dpi, prefix + "_compact.png");
                Get<Border>(window, "ArtImage").Visibility = Visibility.Collapsed;
                Get<ShapePath>(window, "ArtPlaceholder").Visibility = Visibility.Visible;
            }
            // Detached flyouts: no native popup, focus change or volume command.
            var root = Get<Border>(window, "Root");
            var menu = root.ContextMenu;
            menu.PlacementTarget = root;
            Render(menu, light, 1, $"{(light ? "light" : "dark")}_menu.png");
            Render(Get<Border>(window, "VolumePopupBorder"), light, 1, $"{(light ? "light" : "dark")}_volume.png");
        }
    });

    [Fact]
    public void ThemeSwitch_RetintsFlyoutsAndUnknownStates_PreservesActiveState() => WithWindow(window =>
    {
        foreach (bool light in new[] { false, true, false })
        {
            window.ApplyTheme(light);
            var primary = (SolidColorBrush)window.FindResource("PrimaryBrush");
            Assert.Equal(light ? Color.FromRgb(26, 26, 26) : Color.FromRgb(245, 245, 245), primary.Color);
            Assert.Same(primary, Get<TextBlock>(window, "TitleText").Foreground);
            Assert.Same(primary, Get<TextBlock>(window, "OpacityValueText").Foreground);
            Assert.Same(window.FindResource("SurfaceBrush"), Get<Border>(window, "VolumePopupBorder").Background);
            Assert.Same(window.FindResource("GlassPlayInkBrush"), Get<ShapePath>(window, "PlayPauseIcon").Fill);
            var menu = Get<Border>(window, "Root").ContextMenu;
            menu.PlacementTarget = Get<Border>(window, "Root");
            menu.ApplyTemplate();
            Assert.Same(window.FindResource(typeof(ContextMenu)), menu.Style);
            Assert.Same(primary, menu.Foreground);
            Assert.Same(window.FindResource("SurfaceBrush"), ((Border)VisualTreeHelper.GetChild(menu, 0)).Background);
            var sizeMenu = Get<MenuItem>(window, "SizeMenuItem");
            sizeMenu.ApplyTemplate();
            Assert.Same(window.FindResource(typeof(MenuItem)), sizeMenu.Style);
            var submenu = (Popup)sizeMenu.Template.FindName("PART_Popup", sizeMenu);
            Assert.Same(window.FindResource("SurfaceBrush"), ((Border)submenu.Child).Background);
        }
        Invoke(window, "ApplyShuffleVisual", ShuffleMode.Smart);
        var active = Get<ShapePath>(window, "ShuffleIcon").Fill;
        var tooltip = Get<Button>(window, "ShuffleButton").ToolTip;
        Invoke(window, "ApplyRepeatVisual", RepeatMode.Unknown);
        window.ApplyTheme(true);
        Assert.Same(active, Get<ShapePath>(window, "ShuffleIcon").Fill);
        Assert.Equal(tooltip, Get<Button>(window, "ShuffleButton").ToolTip);
        Assert.Equal(Visibility.Visible, Get<ShapePath>(window, "ShuffleSmartStar").Visibility);
        Assert.Same(window.FindResource("MutedBrush"), Get<ShapePath>(window, "RepeatIcon").Fill);
        foreach (string name in new[] { "ArtworkDetailsButton", "MoreButton", "ExpandedPlayButton", "ExpandedNextButton",
                     "PrevButton", "LikeButton", "ShuffleButton", "RepeatButton", "VolumeButton", "CloseDetailsButton" })
        {
            var button = Get<Button>(window, name);
            var peer = new System.Windows.Automation.Peers.ButtonAutomationPeer(button);
            Assert.Equal(button.ToolTip?.ToString(), peer.GetName());
            Assert.NotNull(peer.GetPattern(System.Windows.Automation.Peers.PatternInterface.Invoke));
        }
    });

    [Fact]
    public void ResponsiveLayout_PreservesPriorityAndFitsAtEveryUserScale() => WithWindow(window =>
    {
        var settings = (WidgetSettings)typeof(MainWindow).GetField("_settings", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        var root = Get<Border>(window, "Root");
        foreach (double scale in new[] { 0.8, 1.0, 1.1 })
        {
            settings.Scale = scale;
            root.LayoutTransform = new ScaleTransform(scale, scale);
            foreach (int available in new[] { 151, 152, 179, 180, 207, 208, 244, 272, 300, 420 })
            {
                bool fits = Responsive(window, available * scale);
                Assert.Equal(available >= 152, fits);
                if (!fits) continue;
                root.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
                Assert.True(root.DesiredSize.Width <= available * scale + 1, $"{root.DesiredSize.Width} > {available * scale} at {scale}");
                Assert.True(root.DesiredSize.Height <= 48);
                Assert.Equal(available >= 180, Get<Button>(window, "NextButton").Visibility == Visibility.Visible);
                Assert.Equal(available >= 208, Get<Button>(window, "MoreButton").Visibility == Visibility.Visible);
                Assert.Equal(Visibility.Visible, Get<Button>(window, "ArtworkDetailsButton").Visibility);
                foreach (string name in new[] { "PrevButton", "LikeButton", "ShuffleButton", "RepeatButton", "VolumeButton" })
                    Assert.Equal(Visibility.Visible, Get<Button>(window, name).Visibility);
            }
        }
        settings.ShowPlay = false;
        settings.ShowPrev = false;
        settings.ShowVolume = false;
        window.UpdateExpandedVisibility();
        Assert.True(Responsive(window, 420));
        foreach (string name in new[] { "PlayPauseButton", "PrevButton", "VolumeButton" })
            Assert.Equal(Visibility.Collapsed, Get<Button>(window, name).Visibility);
    });

    [Fact]
    public void ReducedMotion_DisablesTransitions_WhileKeepingPressFeedback() => WithWindow(window =>
    {
        foreach (bool enabled in new[] { true, false, true })
        {
            window.ApplyMotionPreference(enabled);
            foreach (string name in new[] { "PrevButton", "PlayPauseButton", "ExpandedPlayButton", "MoreButton" })
            {
                var button = Get<Button>(window, name);
                button.ApplyTemplate();
                var templateRoot = (FrameworkElement)VisualTreeHelper.GetChild(button, 0);
                var group = Assert.IsType<VisualStateGroup>(VisualStateManager.GetVisualStateGroups(templateRoot)[0]);
                var transition = Assert.IsType<VisualTransition>(group.Transitions[0]);
                Assert.Equal(TimeSpan.FromMilliseconds(enabled ? 140 : 0), transition.GeneratedDuration.TimeSpan);
                Assert.True(VisualStateManager.GoToState(button, "Pressed", false));
                // Evaluate the VSM clock deterministically; detached visuals have no display clock.
                group.CurrentState.Storyboard.SeekAlignedToLastTick(templateRoot, TimeSpan.Zero, TimeSeekOrigin.BeginTime);
                Assert.Equal(1, ((Border)button.Template.FindName("feedback", button)).Opacity);
                Assert.False(button.Focusable);
                Assert.Null(button.Effect);
                Assert.True(VisualStateManager.GoToState(button, "Normal", false));
            }
            Assert.Equal(enabled ? PopupAnimation.Fade : PopupAnimation.None, Get<Popup>(window, "VolumePopup").PopupAnimation);
        }
        Assert.False(window.ShowActivated);
        Assert.False(window.ShowInTaskbar);
    });

    [Fact]
    public void ExpandedControls_RespectEveryVisibilitySetting_AndStopEntryMotion() => WithWindow(window =>
    {
        var settings = (WidgetSettings)typeof(MainWindow).GetField("_settings", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        settings.ShowPlay = settings.ShowNext = settings.ShowPrev = settings.ShowLike =
            settings.ShowShuffle = settings.ShowRepeat = settings.ShowVolume = false;
        settings.Scale = 1.1;
        settings.Opacity = 0.2;
        window.UpdateExpandedVisibility();
        Assert.True(Responsive(window, 118 * settings.Scale));
        foreach (string name in new[] { "ExpandedPlayButton", "ExpandedNextButton", "PrevButton", "LikeButton", "ShuffleButton", "RepeatButton", "VolumeButton" })
            Assert.Equal(Visibility.Collapsed, Get<Button>(window, name).Visibility);
        Assert.Equal(Visibility.Visible, Get<Button>(window, "ArtworkDetailsButton").Visibility);
        var panel = Get<Border>(window, "ExpandedSurface");
        Assert.Equal(1.1, ((ScaleTransform)panel.LayoutTransform).ScaleX);
        window.ApplyMotionPreference(false);
        Invoke(window, "AnimateDetailsEntry");
        Assert.Equal(0.2, panel.Opacity);
        Assert.False(panel.HasAnimatedProperties);
        window.ApplyMotionPreference(true);
        Invoke(window, "AnimateDetailsEntry");
        Assert.True(panel.HasAnimatedProperties);
        Invoke(window, "CloseExpanded");
        Assert.False(panel.HasAnimatedProperties);
        Assert.False(Get<Popup>(window, "ExpandedPopup").IsOpen);
    });

    [Fact]
    public void ExpandedTimeline_UsesExistingPositionAndProgressPreference() => WithWindow(window =>
    {
        SetField(window, "_duration", TimeSpan.FromSeconds(226));
        SetField(window, "_basePosition", TimeSpan.FromSeconds(72));
        var panel = Get<Border>(window, "ExpandedSurface");
        panel.Measure(new Size(316, double.PositiveInfinity));
        panel.Arrange(new Rect(new Point(), panel.DesiredSize));
        Invoke(window, "UpdateProgressUi");
        Assert.Equal("1:12", Get<TextBlock>(window, "PositionText").Text);
        Assert.Equal("3:46", Get<TextBlock>(window, "DurationText").Text);
        Assert.Equal(72.0 / 226 * Get<Border>(window, "ProgressTrack").ActualWidth,
            Get<Border>(window, "ProgressFill").Width, 4);
        var settings = (WidgetSettings)typeof(MainWindow).GetField("_settings", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!;
        settings.ShowProgress = false;
        Invoke(window, "UpdateProgressUi");
        Assert.Equal(Visibility.Collapsed, Get<Border>(window, "ProgressTrack").Visibility);
        Assert.Equal(Visibility.Collapsed, Get<Grid>(window, "TimelineLabels").Visibility);
    });

    [Fact]
    public void SimpMusicControls_FollowSessionCapabilitiesAndRestoreSpotifyControls() => WithWindow(window =>
    {
        window.UpdatePlayerControls(true, MediaControls.PlayPause | MediaControls.Next);
        Assert.True(Responsive(window, 420));
        Assert.Equal(Visibility.Visible, Get<Button>(window, "PlayPauseButton").Visibility);
        Assert.Equal(Visibility.Visible, Get<Button>(window, "ExpandedNextButton").Visibility);
        foreach (string name in new[] { "PrevButton", "LikeButton", "ShuffleButton", "RepeatButton" })
            Assert.Equal(Visibility.Collapsed, Get<Button>(window, name).Visibility);
        Assert.Equal(Visibility.Visible, Get<Button>(window, "VolumeButton").Visibility);
        Assert.Equal("SimpMusic", Get<TextBlock>(window, "OpenPlayerLabel").Text);

        window.UpdatePlayerControls(false, MediaControls.None);
        Assert.Equal(Visibility.Visible, Get<Button>(window, "LikeButton").Visibility);
        Assert.Equal(Visibility.Visible, Get<Button>(window, "ShuffleButton").Visibility);
        Assert.Equal("Spotify", Get<TextBlock>(window, "OpenPlayerLabel").Text);
    });

    [Fact]
    public void NativePopup_IsolatedHost_PreservesFocusAndClosesNestedSurface() => WithWindow(window =>
    {
        // Host only the real visual tree. Never Show MainWindow or run its Loaded integration.
        var root = Get<Border>(window, "Root");
        var expanded = Get<Popup>(window, "ExpandedPopup");
        var volume = Get<Popup>(window, "VolumePopup");
        window.Content = null;
        NameScope.SetNameScope(root, NameScope.GetNameScope(window));
        var host = new Window
        {
            Content = root, WindowStyle = WindowStyle.None, AllowsTransparency = true,
            Background = Brushes.Transparent, ShowInTaskbar = false, ShowActivated = false,
            SizeToContent = SizeToContent.WidthAndHeight, Left = 100, Top = 400
        };
        host.Resources.MergedDictionaries.Add(window.Resources);
        host.SourceInitialized += (_, _) =>
        {
            IntPtr handle = new WindowInteropHelper(host).Handle;
            Interop.SetWindowLong(handle, Interop.GWL_EXSTYLE,
                Interop.GetWindowLong(handle, Interop.GWL_EXSTYLE) | Interop.WS_EX_NOACTIVATE | Interop.WS_EX_TOOLWINDOW);
        };
        IntPtr foreground = Interop.GetForegroundWindow();
        try
        {
            window.ApplyTheme(false);
            window.ApplyMotionPreference(false);
            host.Show();
            PumpAnimationFrame();
            Invoke(window, "CaptureForeground");
            expanded.IsOpen = true;
            PumpAnimationFrame();
            Assert.True(expanded.IsOpen);
            var source = Assert.IsType<HwndSource>(PresentationSource.FromVisual(Get<Border>(window, "ExpandedSurface")));
            Assert.NotEqual(0, Interop.GetWindowLong(source.Handle, Interop.GWL_EXSTYLE) & Interop.WS_EX_NOACTIVATE);
            Assert.Equal(foreground, Interop.GetForegroundWindow());
            Assert.False((bool)typeof(MainWindow).GetField("_uiReady", BindingFlags.Instance | BindingFlags.NonPublic)!.GetValue(window)!);
            volume.IsOpen = true; // No Volume_Click / slider changes / media commands.
            PumpAnimationFrame();
            Assert.True(expanded.StaysOpen);
            volume.IsOpen = false;
            PumpAnimationFrame();
            Assert.False(expanded.StaysOpen);
            Invoke(window, "HideWidget");
            PumpAnimationFrame();
            Assert.False(expanded.IsOpen);
            Assert.False(volume.IsOpen);
            Assert.Equal(foreground, Interop.GetForegroundWindow());
        }
        finally
        {
            expanded.IsOpen = volume.IsOpen = false;
            host.Close();
        }
    });

    private static T Get<T>(MainWindow window, string name) => (T)window.FindName(name);
    private static void SetField(MainWindow window, string name, object value) =>
        typeof(MainWindow).GetField(name, BindingFlags.Instance | BindingFlags.NonPublic)!.SetValue(window, value);
    private static object? Invoke(MainWindow window, string method, params object[] args) =>
        typeof(MainWindow).GetMethod(method, BindingFlags.Instance | BindingFlags.NonPublic)!.Invoke(window, args);
    private static bool Responsive(MainWindow window, double available) => (bool)Invoke(window, "ApplyResponsiveLayout", available)!;

    private static void PumpAnimationFrame()
    {
        var frame = new DispatcherFrame();
        var timer = new DispatcherTimer(DispatcherPriority.Background) { Interval = TimeSpan.FromMilliseconds(30) };
        timer.Tick += (_, _) => { timer.Stop(); frame.Continue = false; };
        timer.Start();
        Dispatcher.PushFrame(frame);
    }

    private static void WithWindow(Action<MainWindow> action)
    {
        Exception? error = null;
        var thread = new Thread(() =>
        {
            MainWindow? window = null;
            try
            {
                window = new MainWindow(new WidgetSettings()) { ClosedByApp = true };
                action(window);
            }
            catch (Exception ex) { error = ex; }
            finally
            {
                window?.Close();
                Dispatcher.CurrentDispatcher.InvokeShutdown();
            }
        }) { IsBackground = true };
        thread.SetApartmentState(ApartmentState.STA);
        thread.Start();
        Assert.True(thread.Join(TimeSpan.FromSeconds(30)), "Off-screen UI verification timed out.");
        if (error != null) ExceptionDispatchInfo.Capture(error).Throw();
    }

    private static void Render(FrameworkElement element, bool light, double scale, string name)
    {
        element.Measure(new Size(double.PositiveInfinity, double.PositiveInfinity));
        element.Arrange(new Rect(new Point(), element.DesiredSize));
        element.UpdateLayout();
        PumpAnimationFrame();
        double logicalWidth = Math.Max(element.ActualWidth, element.DesiredSize.Width);
        double logicalHeight = Math.Max(element.ActualHeight, element.DesiredSize.Height);
        int width = (int)Math.Ceiling(logicalWidth * scale);
        int height = (int)Math.Ceiling(logicalHeight * scale);
        Assert.True(width > 0 && height > 0);
        var bitmap = new RenderTargetBitmap(width, height, 96 * scale, 96 * scale, PixelFormats.Pbgra32);
        var backdrop = new DrawingVisual();
        using (var dc = backdrop.RenderOpen())
            dc.DrawRectangle(new SolidColorBrush(light ? Color.FromRgb(243, 243, 243) : Color.FromRgb(32, 32, 32)), null,
                new Rect(0, 0, logicalWidth, logicalHeight));
        bitmap.Render(backdrop);
        bitmap.Render(element);
        var encoder = new PngBitmapEncoder();
        encoder.Frames.Add(BitmapFrame.Create(bitmap));
        using var stream = File.Create(Path.Combine(OutputDirectory, name));
        encoder.Save(stream);
    }
}
