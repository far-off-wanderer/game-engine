using System;
using Windows.ApplicationModel;
using Windows.ApplicationModel.Activation;
using Windows.UI.ViewManagement;
using Windows.UI.Xaml;
using Windows.UI.Xaml.Controls;
using Windows.UI.Xaml.Navigation;

namespace uwp
{
    sealed partial class App : Application
{
    static string deviceFamily;

    public App()
    {
        this.InitializeComponent();
        this.Suspending += OnSuspending;

        if (Windows.Foundation.Metadata.ApiInformation.IsPropertyPresent("Windows.UI.Xaml.Application", "RequiresPointerMode"))
        {
            if (IsXbox())
            {
                Application.Current.RequiresPointerMode = ApplicationRequiresPointerMode.WhenRequested;
            }
        }
    }

    public static bool IsXbox()
    {
        if (deviceFamily == null)
            deviceFamily = Windows.System.Profile.AnalyticsInfo.VersionInfo.DeviceFamily;

        return deviceFamily == "Windows.Xbox";
    }

    protected override void OnLaunched(LaunchActivatedEventArgs e)
    {
        ApplicationView.GetForCurrentView().SetDesiredBoundsMode(ApplicationViewBoundsMode.UseCoreWindow);

#if DEBUG
            if (System.Diagnostics.Debugger.IsAttached)
            {
                this.DebugSettings.EnableFrameRateCounter = true;
            }
#endif

        Frame rootFrame = Window.Current.Content as Frame;

        if (rootFrame == null)
        {
            rootFrame = new Frame();

            rootFrame.NavigationFailed += OnNavigationFailed;

            if (e.PreviousExecutionState == ApplicationExecutionState.Terminated)
            {
            }

            Window.Current.Content = rootFrame;
        }

        if (rootFrame.Content == null)
        {
            rootFrame.Navigate(typeof(GamePage), e.Arguments);
        }
        Window.Current.Activate();
    }

    void OnNavigationFailed(object sender, NavigationFailedEventArgs e)
    {
        throw new Exception("Failed to load Page " + e.SourcePageType.FullName);
    }

    private void OnSuspending(object sender, SuspendingEventArgs e)
    {
        var deferral = e.SuspendingOperation.GetDeferral();
        deferral.Complete();
    }
}
}