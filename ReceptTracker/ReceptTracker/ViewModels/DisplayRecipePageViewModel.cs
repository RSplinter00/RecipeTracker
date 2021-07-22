using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;
using System.Windows.Input;
using Xamarin.Essentials;

namespace ReceptTracker.ViewModels
{
    public class DisplayRecipePageViewModel : ViewModelBase
    {
#nullable enable
        public ICommand? ForceDescriptionUpdateSizeCommand { get; set; }
        public ICommand? ForceIngredientsUpdateSizeCommand { get; set; }
        public ICommand? ForceRequirementsUpdateSizeCommand { get; set; }
        public ICommand? ForceStepsUpdateSizeCommand { get; set; }
        public ICommand? ForceServetipsUpdateSizeCommand { get; set; }
        public ICommand? ForceLandscapeUpdateSizeCommand { get; set; }
#nullable disable
        public DelegateCommand DeleteRecipeCommand { get; }
        public DelegateCommand EditRecipeCommand { get; }
        public DelegateCommand<string> NavigateToWebsiteCommand { get; }

        private int recipeID = -1;

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

        public DisplayRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRecipeController recipeController) : base(navigationService, pageDialogService, recipeController)
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
            var response = await DialogService.DisplayAlertAsync("Waarschuwing!", "U staat op het punt een recept te verwijderen. Dit kan niet terug gedraaid worden. Weet u zeker dat u door wilt gaan?", "Ja", "Nee");

            if (response)
            {
                await RecipeController.DeleteRecipeAsync(Recipe);
                GoBackAsync();
            }
        }

        public void EditRecipe()
        {
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", Recipe.ID }
            };

            NavigateToPageAsync("EditRecipePage", parameters);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe")) recipeID = (int)parameters["SelectedRecipe"];

            if (recipeID != -1) Recipe = await RecipeController.GetRecipeAsync(recipeID);

            if (Recipe == null)
            {
                await DialogService.DisplayAlertAsync("Niet gevonden!", "Het recept kan niet geladen worden.", "OK");
                GoBackAsync();
            }
        }
    }
}
