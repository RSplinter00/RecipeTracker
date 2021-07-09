using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;

namespace ReceptTracker.ViewModels
{
    public class DisplayRecipePageViewModel : ViewModelBase
    {
        public DelegateCommand EditRecipeCommand { get; }
        private int recipeID = -1;

        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set => SetProperty(ref recipe, value);
        }

        public DisplayRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRecipeController recipeController) : base(navigationService, pageDialogService, recipeController)
        {
            EditRecipeCommand = new DelegateCommand(EditRecipe);
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
            else Recipe = new Recipe("New Recipe", "Add category", new TimeSpan());
        }
    }
}
