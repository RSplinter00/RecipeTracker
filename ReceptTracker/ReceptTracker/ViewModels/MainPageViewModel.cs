using Plugin.Connectivity;
using Plugin.GoogleClient;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;

namespace ReceptTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private bool hasSynced = false;
        private bool promptedForLogin = false;

        public DelegateCommand OnToggleLoginCommand { get; }
        public DelegateCommand OnRefreshCommand { get; }
        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Recipe> RecipeSelectedCommand { get; }

        private readonly string LoginText = "Inloggen";
        private readonly string LogoutText = "Uitloggen";
        private string loginToolbarItemText;
        public string LoginToolbarItemText
        {
            get => loginToolbarItemText;
            private set => SetProperty(ref loginToolbarItemText, value);
        }

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

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            OnToggleLoginCommand = new DelegateCommand(OnToggleLogin);
            OnRefreshCommand = new DelegateCommand(OnRefresh);
            AddRecipeCommand = new DelegateCommand(() => NavigateToPageAsync("EditRecipePage"));
            RecipeSelectedCommand = new DelegateCommand<Recipe>(RecipeSelected);
        }

        public async void OnToggleLogin()
        {
            if (CrossConnectivity.Current.IsConnected)
            {
                if (AuthService.GetUser() == null)
                {
                    await AuthService.LoginAsync();
                    promptedForLogin = true;

                    OnRefresh();
                }
                else
                {
                    if (AuthService.Logout())
                    {
                        await DialogService.DisplayAlertAsync("Uitgelogd!", "U bent succesvol uitgelogd.", "Ok");
                        OnNavigatedTo(null);
                    }
                }

                SetLoginToolbarText();
            }
        }

        private void SetLoginToolbarText()
        {
            if (AuthService.GetUser() == null) LoginToolbarItemText = LoginText;
            else LoginToolbarItemText = LogoutText;
        }

        public async void OnRefresh()
        {
            IsRefreshing = true;

            Recipes = await DatabaseService.GetRecipesAsync();

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
            var response = GoogleActionStatus.Error;
            
            if (CrossConnectivity.Current.IsConnected && !promptedForLogin)
            {
                response = await AuthService.LoginAsync();
                promptedForLogin = true;
            }

            SetLoginToolbarText();

            if (response == GoogleActionStatus.Completed && !hasSynced)
            {
                await DatabaseService.SyncRecipes();
                hasSynced = true;
            }

            OnRefresh();
        }
    }
}
