using Prism;
using Prism.Ioc;
using RecipeTracker.ViewModels;
using RecipeTracker.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using RecipeTracker.Services;
using System.Runtime.CompilerServices;
using Prism.DryIoc;
using Microsoft.AppCenter;
using Microsoft.AppCenter.Analytics;
using Microsoft.AppCenter.Crashes;
using RecipeTracker.Views.Settings;
using RecipeTracker.ViewModels.Settings;
using Xamarin.Essentials;
using Plugin.Connectivity;

[assembly: InternalsVisibleTo("RecipeTracker.Unit")]
namespace RecipeTracker
{
    /// <summary>
    /// Class <c>App</c> setsup everything needed to properly run he application.
    /// </summary>
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        /// <summary>
        /// Checks if the user has an internet connection.
        /// For testing purposes, if the device platform is unknown, it will always return true.
        /// </summary>
        /// <returns>Whether the user has an internet connection or not.</returns>
        public static bool IsConnected()
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown) return true;

            return CrossConnectivity.Current.IsConnected;
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            // Navigate to the main page.
            var response = await NavigationService.NavigateAsync("NavigationPage/MainPage");
            if (!response.Success) Crashes.TrackError(response.Exception);
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            // Register the navigation page.
            containerRegistry.RegisterForNavigation<NavigationPage>();

            // Register the application pages.
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<DisplayRecipePage, DisplayRecipePageViewModel>();
            containerRegistry.RegisterForNavigation<EditRecipePage, EditRecipePageViewModel>();

            // Register the settings pages.
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<AccountSettingsPage, AccountSettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ReportIssueSettingsPage, ReportIssueSettingsPageViewModel>();

            // Register services.
            containerRegistry.Register<IAuthenticationService, GoogleAuthenticationService>();
            containerRegistry.Register<IDatabaseService, DatabaseService>();
            containerRegistry.Register<IReportingService, ReportingService>();

            // Register singleton for Xamarin Essentials
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();
        }

        protected override void OnStart()
        {
            base.OnStart();
            AppCenter.Start($"android={AppSettingsManager.Settings["AndroidAppCenterKey"]}", typeof(Analytics), typeof(Crashes));
        }
    }
}
