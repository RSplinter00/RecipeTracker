using Plugin.Connectivity;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using System;
using Xamarin.Essentials;

namespace RecipeTracker.ViewModels
{
    public class ViewModelBase : BindableBase, IInitialize, INavigationAware
    {
        internal INavigationService NavigationService { get; private set; }
        internal IPageDialogService DialogService { get; private set; }
        internal IAuthenticationService AuthService { get; private set; }
        internal IDatabaseService DatabaseService { get; private set; }

        public DelegateCommand<string> NavigateCommand { get; }

        public ViewModelBase(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
        {
            NavigationService = navigationService;
            DialogService = pageDialogService;
            AuthService = authService;
            DatabaseService = databaseService;

            NavigateCommand = new DelegateCommand<string>(NavigateToPageAsync);
        }

        protected bool IsConnected()
        {
            if (DeviceInfo.Platform == DevicePlatform.Unknown) return true;

            return CrossConnectivity.Current.IsConnected;
        }

        internal async void NavigateToMainPageAsync()
        {
            var result = await NavigationService.NavigateAsync("/NavigationPage/MainPage");

            if (result != null && !result.Success) Console.WriteLine("Failed to navigate to MainPage");
        }

        internal async void NavigateToPageAsync(string path)
        {
            var result = await NavigationService.NavigateAsync(path);

            if (result != null && !result.Success) Console.WriteLine($"Unable to navigate to page with path: { path }");
        }

        internal async void NavigateToPageAsync(string path, INavigationParameters navigationParams)
        {
            var result = await NavigationService.NavigateAsync(path, navigationParams);

            if (result != null && !result.Success) Console.WriteLine($"Unable to navigate to page with path: { path }");
        }

        internal async void GoBackAsync()
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
