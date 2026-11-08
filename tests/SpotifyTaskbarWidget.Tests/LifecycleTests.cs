using SpotifyTaskbarWidget;

namespace SpotifyTaskbarWidget.Tests;

public class LifecycleTests
{
    [Fact]
    public async Task MediaService_Shutdown_CancelsPendingInitializationAndMarksDisposed()
    {
        var media = new MediaService();
        Assert.False(media.IsDisposed);

        // Start initialization which may attempt SMTC or delay
        var initTask = media.InitializeAsync();

        // Shutdown immediately to test race condition (R2)
        media.Shutdown();

        Assert.True(media.IsDisposed);

        // Awaiting the task should complete promptly without hanging
        var completed = await Task.WhenAny(initTask, Task.Delay(TimeSpan.FromSeconds(3)));
        Assert.Same(initTask, completed);
    }

    [Fact]
    public void MediaService_Shutdown_IsIdempotentAndSafe()
    {
        var media = new MediaService();
        bool changedFired = false;
        media.Changed += () => changedFired = true;

        // Calling shutdown multiple times should not throw
        media.Shutdown();
        media.Shutdown();

        Assert.True(media.IsDisposed);
        Assert.False(changedFired);
    }

    [Fact]
    public async Task MediaService_RepeatedCycle_DoesNotLeakState()
    {
        for (int i = 0; i < 5; i++)
        {
            var media = new MediaService();
            var initTask = media.InitializeAsync();
            media.Shutdown();
            await Task.WhenAny(initTask, Task.Delay(TimeSpan.FromSeconds(1)));
            Assert.True(media.IsDisposed);
        }
    }
}
