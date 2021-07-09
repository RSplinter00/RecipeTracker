using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;

namespace ReceptTracker.ViewModels
{
    public class EditRecipePageViewModel : ViewModelBase
    {
        public DelegateCommand OnCancelCommand { get; }
        public DelegateCommand OnSubmitCommand { get; }

        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set => SetProperty(ref recipe, value);
        }

        private string test = "Test";
        public string Test
        {
            get => test;
            set => SetProperty(ref test, value);
        }

        public EditRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRecipeController recipeController) : base(navigationService, pageDialogService, recipeController)
        {
            OnCancelCommand = new DelegateCommand(OnCancelPressed);
            OnSubmitCommand = new DelegateCommand(OnSubmit);
        }

        public async void OnCancelPressed()
        {
            var response = await DialogService.DisplayAlertAsync("Pas op!", "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?", "Ja", "Nee");
            if (response) GoBackAsync();
        }

        public async void OnSubmit()
        {
            var response = await DialogService.DisplayAlertAsync("Pas op!", "Deze actie kan niet ongedaan worden. Weer u het zeker?", "Ja", "Nee");
            if (response)
            {
                await RecipeController.SaveRecipeAsync(Recipe);
                GoBackAsync();
            }
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe"))
            {
                var recipeID = (int)parameters["SelectedRecipe"];
                Recipe = await RecipeController.GetRecipeAsync(recipeID);
            }
            
            if (Recipe == null) Recipe = new Recipe("New Recipe", "Add category", new TimeSpan());
        }
    }
}
