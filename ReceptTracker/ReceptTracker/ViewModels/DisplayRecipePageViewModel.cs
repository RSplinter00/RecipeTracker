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
                { "Recipe", Recipe }
            };

            NavigateToPageAsync("EditRecipePage", parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe")) Recipe = (Recipe)parameters["SelectedRecipe"];
            else if (Recipe == null) Recipe = new Recipe("New Recipe", "Add category", new TimeSpan());
        }
    }
}
