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

            // Register the navigation page.
            containerRegistry.RegisterForNavigation<NavigationPage>();
            
            // Register the application pages.
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<DisplayRecipePage, DisplayRecipePageViewModel>();
            containerRegistry.RegisterForNavigation<EditRecipePage, EditRecipePageViewModel>();
        }
    }
}
