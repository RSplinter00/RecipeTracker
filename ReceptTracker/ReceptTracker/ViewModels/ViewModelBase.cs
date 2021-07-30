using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using System;

namespace ReceptTracker.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware
    {
        protected INavigationService NavigationService { get; private set; }
        protected IPageDialogService DialogService { get; private set; }
        //protected IRecipeController RecipeController { get; private set; }
        protected IAuthenticationService AuthService { get; private set; }
        protected IFirebaseService FirebaseService { get; private set; }

        public DelegateCommand<string> NavigateCommand { get; }

        public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IFirebaseService firebaseService)
        {
            NavigationService = navigationService;
            DialogService = pageDialogService;
            //RecipeController = recipeController;
            AuthService = authService;
            FirebaseService = firebaseService;

            NavigateCommand = new DelegateCommand<string>(NavigateToPageAsync);
        }

        protected async void NavigateToMainPageAsync()
        {
            var result = await NavigationService.NavigateAsync("/NavigationPage/MainPage");

            if (result != null && !result.Success) Console.WriteLine("Failed to navigate to MainPage");
        }

        public async void NavigateToPageAsync(string path)
        {
            var result = await NavigationService.NavigateAsync(path);

            if (result != null && !result.Success) Console.WriteLine($"Unable to navigate to page with path: { path }");
        }

        protected async void NavigateToPageAsync(string path, INavigationParameters navigationParams)
        {
            var result = await NavigationService.NavigateAsync(path, navigationParams);

            if (result != null && !result.Success) Console.WriteLine($"Unable to navigate to page with path: { path }");
        }

        protected async void GoBackAsync()
        {
            var result = await NavigationService.GoBackAsync();

            if (result != null && !result.Success) Console.WriteLine("Failed to navigate to the previous page");
        }

        public virtual void Initialize(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedFrom(INavigationParameters parameters)
        {

        }

        public virtual void OnNavigatedTo(INavigationParameters parameters)
        {

        }
    }
}
