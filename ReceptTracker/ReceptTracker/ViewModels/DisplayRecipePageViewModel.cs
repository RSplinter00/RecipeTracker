using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Models;
using System;

namespace ReceptTracker.ViewModels
{
    public class DisplayRecipePageViewModel : ViewModelBase
    {
        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set => SetProperty(ref recipe, value);
        }

        public DisplayRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe"))
            {
                Recipe = (Recipe) parameters["SelectedRecipe"];
            }
            else
            {
                Recipe = new Recipe("New Recipe", "Add category", new TimeSpan());
            }
        }
    }
}
