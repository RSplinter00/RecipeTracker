using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System.Collections.Generic;

namespace ReceptTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand<Recipe> RecipeSelectedCommand { get; }

        private List<Recipe> recipes;
        public List<Recipe> Recipes
        {
            get => recipes;
            private set => SetProperty(ref recipes, value);
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRecipeController recipeController) : base(navigationService, pageDialogService, recipeController)
        {
            RecipeSelectedCommand = new DelegateCommand<Recipe>(RecipeSelected);
        }

        public void RecipeSelected(Recipe selectedRecipe)
        {
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", selectedRecipe }
            };

            NavigateToPageAsync("DisplayRecipePage", parameters);
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            Recipes = await RecipeController.GetRecipesAsync();
        }
    }
}
