using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using RecipeTracker.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RecipeTracker.ViewModels
{
    /// <summary>
    /// Class <c>DisplayRecipePageViewModel</c> is the viewmodel for the display recipe page.
    /// This page displays the contents of a selected recipe.
    /// </summary>
    public class DisplayRecipePageViewModel : ViewModelBase
    {
        public ICommand ForceDescriptionUpdateSizeCommand { get; set; }
        public ICommand ForceIngredientsUpdateSizeCommand { get; set; }
        public ICommand ForceRequirementsUpdateSizeCommand { get; set; }
        public ICommand ForceStepsUpdateSizeCommand { get; set; }
        public ICommand ForceServetipsUpdateSizeCommand { get; set; }
        public ICommand ForceLandscapeUpdateSizeCommand { get; set; }
        public DelegateCommand OnDeleteRecipeCommand { get; }
        public DelegateCommand OnEditRecipeCommand { get; }
        public DelegateCommand<string> OnNavigateToWebsiteCommand { get; }

        private Guid recipeId;
        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set
            {
                SetProperty(ref recipe, value);
                ForceDescriptionUpdateSizeCommand?.Execute(null);
                ForceIngredientsUpdateSizeCommand?.Execute(null);
                ForceRequirementsUpdateSizeCommand?.Execute(null);
                ForceStepsUpdateSizeCommand?.Execute(null);
                ForceServetipsUpdateSizeCommand?.Execute(null);
                ForceLandscapeUpdateSizeCommand?.Execute(null);
            }
        }

        public DisplayRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            OnDeleteRecipeCommand = new DelegateCommand(OnDeleteRecipe);
            OnEditRecipeCommand = new DelegateCommand(EditRecipeAsync);
            OnNavigateToWebsiteCommand = new DelegateCommand<string>(ToWebsiteAsync);
        }

        /// <summary>
        /// Opens the user's browser page and navigates to the given recipe.
        /// </summary>
        /// <param name="website">Url of the website to navigate to.</param>
        public async void ToWebsiteAsync(string website)
        {
            // Add 'https://' if the url doesn't begin with it.
            if (!(website.StartsWith("https://") || website.StartsWith("http://"))) website = website.Insert(0, "https://");

            // Open the browser and navigate to the website.
            if (Uri.TryCreate(website, UriKind.Absolute, out var uri) && DeviceInfo.Platform != DevicePlatform.Unknown) await Launcher.OpenAsync(uri);
        }

        private async void OnDeleteRecipe()
        {
            await DeleteRecipeAsync();
        }

        /// <summary>
        /// Deletes the recipe displayed on this page.
        /// </summary>
        public async Task DeleteRecipeAsync()
        {
            bool response = await DialogService.DisplayAlertAsync("Waarschuwing!", "U staat op het punt dit recept te verwijderen. Dit kan niet terug gedraaid worden.", "Verwijder", "Annuleer");

            if (response)
            {
                await DatabaseService.DeleteRecipeAsync(Recipe.Id);
                GoBackAsync();
            }
        }

        /// <summary>
        /// Navigates to EditRecipePage, for the user to edit the recipe displayed on the page.
        /// </summary>
        public async void EditRecipeAsync()
        {
            if (Recipe == null)
            {
                // Cancel the action, if the recipe doesn't exist
                await DialogService.DisplayAlertAsync("Waarschuwing!", "Niet mogelijk om dit recept aan te passen.", "Ok");
                return;
            }

            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", Recipe.Id }
            };

            NavigateToPageAsync("EditRecipePage", parameters);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            try
            {
                // Get the recipe id from the parameters and retrieve the selected recipe.
                if (parameters.ContainsKey("SelectedRecipe")) recipeId = (Guid)parameters["SelectedRecipe"];

                if (recipeId != Guid.Empty) Recipe = await DatabaseService.GetRecipeAsync(recipeId);

                if (Recipe != null) return;
            }
            catch (Exception e)
            {
                Debug.WriteLine(e.Message);
            }

            // If the recipe doesn't exist, return to the previous page.
            await DialogService.DisplayAlertAsync("Niet gevonden!", "Het recept kan niet geladen worden.", "Ok");
            GoBackAsync();
        }
    }
}
