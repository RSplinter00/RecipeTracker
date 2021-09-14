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
using RecipeTracker.Views.Settings;
using RecipeTracker.ViewModels.Settings;
using Xamarin.Essentials;
using Plugin.Connectivity;
using Plugin.GoogleClient;
using System.Threading.Tasks;
using System;

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
        /// hecks if the user has an internet connection.
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
            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            // Register services.
            containerRegistry.Register<IAuthenticationService, GoogleAuthenticationService>();
            containerRegistry.Register<IDatabaseService, DatabaseService>();
            containerRegistry.Register<IReportingService, ReportingService>();

            // Register the navigation page.
            containerRegistry.RegisterForNavigation<NavigationPage>();

            // Register the settings pages.
            containerRegistry.RegisterForNavigation<SettingsPage, SettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<AccountSettingsPage, AccountSettingsPageViewModel>();
            containerRegistry.RegisterForNavigation<ReportIssueSettingsPage, ReportIssueSettingsPageViewModel>();

            // Register the application pages.
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<DisplayRecipePage, DisplayRecipePageViewModel>();
            containerRegistry.RegisterForNavigation<EditRecipePage, EditRecipePageViewModel>();

        }
    }
}
