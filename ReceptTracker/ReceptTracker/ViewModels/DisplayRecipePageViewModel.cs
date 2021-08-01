using Plugin.Connectivity;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Services;
using ReceptTracker.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace ReceptTracker.ViewModels
{
    public class DisplayRecipePageViewModel : ViewModelBase
    {
        public ICommand ForceDescriptionUpdateSizeCommand { get; set; }
        public ICommand ForceIngredientsUpdateSizeCommand { get; set; }
        public ICommand ForceRequirementsUpdateSizeCommand { get; set; }
        public ICommand ForceStepsUpdateSizeCommand { get; set; }
        public ICommand ForceServetipsUpdateSizeCommand { get; set; }
        public ICommand ForceLandscapeUpdateSizeCommand { get; set; }
        public DelegateCommand DeleteRecipeCommand { get; }
        public DelegateCommand EditRecipeCommand { get; }
        public DelegateCommand<string> NavigateToWebsiteCommand { get; }

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
            DeleteRecipeCommand = new DelegateCommand(DeleteRecipe);
            EditRecipeCommand = new DelegateCommand(EditRecipe);
            NavigateToWebsiteCommand = new DelegateCommand<string>(ToWebsiteAsync);
        }

        public async void ToWebsiteAsync(string website)
        {
            if (!(website.StartsWith("https://") || website.StartsWith("http://"))) website = website.Insert(0, "https://");

            if (Uri.TryCreate(website, UriKind.Absolute, out var uri) && DeviceInfo.Platform != DevicePlatform.Unknown) await Launcher.OpenAsync(uri);
        }

        public async void DeleteRecipe()
        {
            bool response;

            if (CrossConnectivity.Current.IsConnected)
                response = await DialogService.DisplayAlertAsync("Waarschuwing", "U staat op het punt dit recept te verwijderen. Dit kan niet terug gedraaid worden", "Verwijder", "Annuleer");
            else
                response = await DialogService.DisplayAlertAsync("Waarschuwing", "U staat op het punt om dit recept lokaal te verwijderen. Als dit recept niet is opgeslagen in de cloud, kan dit niet worden terug gedraaid.", "Verwijder", "Annuleer");

            if (response)
            {
                await DatabaseService.DeleteRecipeAsync(recipe.Id);
                GoBackAsync();
            }
        }

        public void EditRecipe()
        {
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", Recipe.Id }
            };

            NavigateToPageAsync("EditRecipePage", parameters);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe")) recipeId = (Guid)parameters["SelectedRecipe"];

            if (recipeId != null) Recipe = await DatabaseService.GetRecipeAsync(recipeId);

            if (Recipe == null)
            {
                await DialogService.DisplayAlertAsync("Niet gevonden!", "Het recept kan niet geladen worden.", "OK");
                GoBackAsync();
            }
        }
    }
}
