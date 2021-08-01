using Prism;
using Prism.Ioc;
using ReceptTracker.ViewModels;
using ReceptTracker.Views;
using Xamarin.Essentials.Interfaces;
using Xamarin.Essentials.Implementation;
using Xamarin.Forms;
using ReceptTracker.Controllers;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("ReceptTracker.Unit")]
namespace ReceptTracker
{
    public partial class App
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
