using Microsoft.AppCenter.Crashes;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using System.Collections.Generic;

namespace RecipeTracker.ViewModels
{
    /// <summary>
    /// Class <c>ViewModelBase</c> is the base model for all viewmodels
    /// </summary>
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

        /// <summary>
        /// navigates the application to the mainpage, resetting the navigation stack.
        /// </summary>
        internal async void NavigateToMainPageAsync()
        {
            var path = "/NavigationPage/MainPage";
            var result = await NavigationService.NavigateAsync(path);
            if (result != null && !result.Success)
            {
                var properties = new Dictionary<string, string>
                {
                    { "Failed to navigate to page", path }
                };

                Crashes.TrackError(result.Exception, properties);
            }
        }

        /// <summary>
        /// nvigates the application to the page with the given path.
        /// </summary>
        /// <param name="path">The path of the page to navigate to.</param>
        internal async void NavigateToPageAsync(string path)
        {
            var result = await NavigationService.NavigateAsync(path);

            if (result != null && !result.Success)
            {
                var properties = new Dictionary<string, string>
                {
                    { "Failed to navigate to page", path }
                };

                Crashes.TrackError(result.Exception, properties);
            }
        }

        /// <summary>
        /// Navigates the application to the page with the given path and sets the given parameters.
        /// </summary>
        /// <param name="path">The path of the page to navigate to.</param>
        /// <param name="navigationParams">Parameters to pass to the next page.</param>
        internal async void NavigateToPageAsync(string path, INavigationParameters navigationParams)
        {
            var result = await NavigationService.NavigateAsync(path, navigationParams);

            if (result != null && !result.Success)
            {
                var properties = new Dictionary<string, string>
                {
                    { "Failed to navigate to page", path }
                };

                Crashes.TrackError(result.Exception, properties);
            }
        }

        /// <summary>
        /// Navigates the application back to the previous page in the navigation stack.
        /// </summary>
        internal async void GoBackAsync()
        {
            var result = await NavigationService.GoBackAsync();

            if (result != null && !result.Success)
            {
                var properties = new Dictionary<string, string>
                {
                    { "Failed to navigate to previous page", $"from: {NavigationService.GetNavigationUriPath()}" }
                };

                Crashes.TrackError(result.Exception, properties);
            }
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
