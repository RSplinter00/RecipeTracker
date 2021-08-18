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
    public partial class App : PrismApplication
    {
        public App(IPlatformInitializer initializer = null) : base(initializer)
        {
        }

        protected override async void OnInitialized()
        {
            InitializeComponent();

            await NavigationService.NavigateAsync("NavigationPage/MainPage");
        }

        protected override void RegisterTypes(IContainerRegistry containerRegistry)
        {
            containerRegistry.RegisterSingleton<IAppInfo, AppInfoImplementation>();

            containerRegistry.Register<IAuthenticationService, GoogleAuthenticationService>();
            containerRegistry.Register<IDatabaseService, DatabaseService>();

            containerRegistry.RegisterForNavigation<NavigationPage>();
            containerRegistry.RegisterForNavigation<MainPage, MainPageViewModel>();
            containerRegistry.RegisterForNavigation<DisplayRecipePage, DisplayRecipePageViewModel>();
            containerRegistry.RegisterForNavigation<EditRecipePage, EditRecipePageViewModel>();
        }
    }
}
