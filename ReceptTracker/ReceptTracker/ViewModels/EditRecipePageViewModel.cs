using Prism.Commands;
using Prism.Mvvm;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Linq;

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

        public EditRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {

        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("Recipe")) Recipe = (Recipe)parameters["Recipe"];
            else Recipe = new Recipe("New Recipe", "Add category", new TimeSpan());
        }
    }
}
