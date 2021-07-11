using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System.Windows.Input;

namespace ReceptTracker.ViewModels
{
    public class DisplayRecipePageViewModel : ViewModelBase
    {
#nullable enable
        public ICommand? ForceIngredientsUpdateSizeCommand { get; set; }
        public ICommand? ForceRequirementsUpdateSizeCommand { get; set; }
        public ICommand? ForcePreparationUpdateSizeCommand { get; set; }
#nullable disable
        public DelegateCommand DeleteRecipeCommand { get; }
        public DelegateCommand EditRecipeCommand { get; }

        private int recipeID = -1;

        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set
            {
                SetProperty(ref recipe, value);
                ForceIngredientsUpdateSizeCommand?.Execute(null);
                ForceRequirementsUpdateSizeCommand?.Execute(null);
                ForcePreparationUpdateSizeCommand?.Execute(null);
            }
        }

        public DisplayRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRecipeController recipeController) : base(navigationService, pageDialogService, recipeController)
        {
            DeleteRecipeCommand = new DelegateCommand(DeleteRecipe);
            EditRecipeCommand = new DelegateCommand(EditRecipe);
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

            var prep = Recipe.Preparation;

            if (Recipe == null)
            {
                await DialogService.DisplayAlertAsync("Niet gevonden!", "Het recept kan niet geladen worden.", "OK");
                GoBackAsync();
            }
        }
    }
}
