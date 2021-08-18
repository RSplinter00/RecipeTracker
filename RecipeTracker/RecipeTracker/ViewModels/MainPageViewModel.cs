using Plugin.GoogleClient;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private bool hasSynced = false;
        private bool promptedForLogin = false;

        public DelegateCommand OnToggleLoginCommand { get; }
        public DelegateCommand OnRefreshCommand { get; }
        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Recipe> OnRecipeSelectedCommand { get; }

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
            OnRecipeSelectedCommand = new DelegateCommand<Recipe>(OnRecipeSelected);
        }

        private async void OnToggleLogin()
        {
            if (IsConnected())
            {
                await ToggleLogin();
            }
        }

        private async void OnRefresh()
        {
            await RefreshRecipes();
        }

        public async Task ToggleLogin()
        {
            if (AuthService.GetUser() == null)
            {
                await AuthService.LoginAsync();
                promptedForLogin = true;

                SetLoginToolbarText();
                await DatabaseService.SyncRecipesAsync();
                await RefreshRecipes();
            }
            else
            {
                if (await AuthService.LogoutAsync())
                {
                    await DialogService.DisplayAlertAsync("Uitgelogd!", "U bent succesvol uitgelogd.", "Ok");
                    promptedForLogin = true;
                    OnNavigatedTo(null);
                }
            }

        }

        internal void SetLoginToolbarText()
        {
            if (AuthService.GetUser() == null) LoginToolbarItemText = LoginText;
            else LoginToolbarItemText = LogoutText;
        }

        public async Task RefreshRecipes()
        {
            IsRefreshing = true;

            Recipes = await DatabaseService.GetRecipesAsync();

            IsRefreshing = false;
        }

        public async void OnRecipeSelected(Recipe selectedRecipe)
        {
            if (selectedRecipe == null) await DialogService.DisplayAlertAsync("Incorrecte recept!", "Dit recept bestaat niet.", "Ok");
            else
            {
                var parameters = new NavigationParameters
                {
                    { "SelectedRecipe", selectedRecipe.Id }
                };

                NavigateToPageAsync("DisplayRecipePage", parameters);
            }
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            var response = GoogleActionStatus.Error;
            
            if (IsConnected() && !promptedForLogin)
            {
                response = await AuthService.LoginAsync();
                promptedForLogin = true;
            }

            SetLoginToolbarText();

            if (response == GoogleActionStatus.Completed && !hasSynced)
            {
                await DatabaseService.SyncRecipesAsync();
                hasSynced = true;
            }

            Recipes = await DatabaseService.GetRecipesAsync();
        }
    }
}
