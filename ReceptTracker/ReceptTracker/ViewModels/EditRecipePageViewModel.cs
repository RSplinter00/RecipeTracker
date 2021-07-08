using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;

namespace ReceptTracker.ViewModels
{
    public class EditRecipePageViewModel : ViewModelBase
    {
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

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Recipe")) Recipe = (Recipe)parameters["Recipe"];
            else Recipe = new Recipe("New Recipe", "Add category", new TimeSpan());
        }
    }
}
