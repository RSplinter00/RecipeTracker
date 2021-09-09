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
    /// <summary>
    /// Class <c>MainPageViewModel</c> is the viewmodel for the main page.
    /// This page manages the authentication and tracks the recipes of the user.
    /// </summary>
    public class MainPageViewModel : ViewModelBase
    {
        private bool hasSynced = false;
        private bool promptedForLogin = false;

        public DelegateCommand OnToggleLoginCommand { get; }
        public DelegateCommand OnSettingsPressedCommand { get; }
        public DelegateCommand OnRefreshCommand { get; }
        public DelegateCommand AddRecipeCommand { get; }
        public DelegateCommand<Recipe> OnRecipeSelectedCommand { get; }

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
            OnSettingsPressedCommand = new DelegateCommand(() => NavigateToPageAsync("SettingsPage"));
            OnRefreshCommand = new DelegateCommand(OnRefresh);
            AddRecipeCommand = new DelegateCommand(() => NavigateToPageAsync("EditRecipePage"));
            OnRecipeSelectedCommand = new DelegateCommand<Recipe>(OnRecipeSelected);
        }

        /// <summary>
        ///  If the user has an internet connection, task <see cref="ToggleLogin"/> is executed.
        /// </summary>
        private async void OnToggleLogin()
        {
            if (App.IsConnected()) await ToggleLogin();
        }

        /// <summary>
        /// Executes task <see cref="RefreshRecipes"/>.
        /// </summary>
        private async void OnRefresh()
        {
            await RefreshRecipes();
        }

        /// <summary>
        /// Logs the user in, if he isn't already. If the user is logged in, he will be logged out.
        /// </summary>
        public async Task ToggleLogin()
        {
            if (AuthService.GetUser() == null)
            {
                // If the user is not logged in, authenticate him and setup the page accordingly.
                await AuthService.LoginAsync();
                promptedForLogin = true;

                await DatabaseService.SyncRecipesAsync();
                await RefreshRecipes();
            }
            else
            {
                // If the user is logged in, log him out and if successful, reset the current page.
                if (await AuthService.LogoutAsync())
                {
                    await DialogService.DisplayAlertAsync("Uitgelogd!", "U bent succesvol uitgelogd.", "Ok");
                    promptedForLogin = true;
                    OnNavigatedTo(null);
                }
            }

        }

        /// <summary>
        /// Refreshes the page and updates the list of recipes.
        /// </summary>
        public async Task RefreshRecipes()
        {
            IsRefreshing = true;

            Recipes = await DatabaseService.GetRecipesAsync();

            IsRefreshing = false;
        }

        /// <summary>
        /// Navigates to <c>DisplayRecipePage</c> with as parameter the id of the selected recipe.
        /// </summary>
        /// <param name="selectedRecipe">The recipe to be displayed.</param>
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

        /// <summary>
        /// Setups the main page by authentication the user and synchronizing recipes, if needed. And populates the page with recipes.
        /// </summary>
        internal async Task SetupMainPage()
        {

            if (App.IsConnected() && !promptedForLogin && AuthService.GetUser() != null)
            {
                // If the user has an internet connection, is still logged in and hasn't asked to login before, call the login function.
                await AuthService.LoginAsync();
                promptedForLogin = true;
            }

            if (AuthService.GetUser() != null && !hasSynced)
            {
                // If the user is logged in and hasn't synced before, save cached recipes to the cloud.
                await DatabaseService.SyncRecipesAsync();
                hasSynced = true;
            }

            Recipes = await DatabaseService.GetRecipesAsync();
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            await SetupMainPage();
        }
    }
}
