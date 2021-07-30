using Plugin.GoogleClient;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace ReceptTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand OnLogoutCommand { get; }
        public DelegateCommand OnRefreshCommand { get; }
        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Recipe> RecipeSelectedCommand { get; }

        private bool isRefreshing;
        public bool IsRefreshing
        {
            get => isRefreshing;
            private set => SetProperty(ref isRefreshing, value);
        }

        private List<Recipe> recipes;
        public List<Recipe> Recipes
        {
            get => recipes;
            private set => SetProperty(ref recipes, value);
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IFirebaseService firebaseService) : base(navigationService, pageDialogService, authService, firebaseService)
        {
            OnLogoutCommand = new DelegateCommand(OnLogout);
            OnRefreshCommand = new DelegateCommand(OnRefresh);
            AddRecipeCommand = new DelegateCommand(() => NavigateToPageAsync("EditRecipePage"));
            RecipeSelectedCommand = new DelegateCommand<Recipe>(RecipeSelected);
        }

        public async void OnLogout()
        {
            var result = AuthService.Logout();

            if (result) await DialogService.DisplayAlertAsync("Uitgelogd!", "U bent succesvol uitgelogd.", "Ok");
            else await DialogService.DisplayAlertAsync("Error", "Niet mogelijk om uit te loggen", "Ok");

            OnNavigatedTo(null);
        }

        public async void OnRefresh()
        {
            IsRefreshing = true;

            Recipes = await FirebaseService.GetRecipesAsync();

            IsRefreshing = false;
        }

        public void RecipeSelected(Recipe selectedRecipe)
        {
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", selectedRecipe.Id }
            };

            NavigateToPageAsync("DisplayRecipePage", parameters);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var response = await AuthService.LoginAsync();
            if (response != GoogleActionStatus.Completed) Debugger.Break();

            OnRefresh();
        }
    }
}
