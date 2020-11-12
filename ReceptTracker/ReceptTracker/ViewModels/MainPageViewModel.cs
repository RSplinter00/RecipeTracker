using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;

namespace ReceptTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        private List<Recipe> recipes;
        public List<Recipe> Recipes 
        { 
            get => recipes;
            private set => SetProperty(ref recipes, value);
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Recipes = new List<Recipe>()
            {
                new Recipe("Recipe1", "Recipe text 1", "Beef", new TimeSpan(1, 30, 0)),
                new Recipe("Recipe2", "Recipe text 2", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe3", "Recipe text 3", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe4", "Recipe text 4", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe5", "Recipe text 5", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe6", "Recipe text 6", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe7", "Recipe text 7", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe8", "Recipe text 8", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe9", "Recipe text 9", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe10", "Recipe text 10", "Beef", new TimeSpan(1, 30, 0))
            };
        }
    }
}
