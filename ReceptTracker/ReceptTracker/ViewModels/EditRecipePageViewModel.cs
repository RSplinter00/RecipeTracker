using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Controllers;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;

namespace ReceptTracker.ViewModels
{
    public class EditRecipePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string PageName => CreateMode ? "Nieuw recept" : "Recept wijzigen";

        private bool createMode;
        protected bool CreateMode
        {
            get => createMode;
            set
            {
                createMode = value;
                OnPropertyChanged("PageName");
            }
        }

        public DelegateCommand OnCancelCommand { get; }
        public DelegateCommand OnSubmitCommand { get; }
        public DelegateCommand OnAddPropertyCommand { get; }
        public DelegateCommand<string> OnRemovePropertyCommand { get; }

        private Recipe recipe;
        public Recipe Recipe
        {
            get => recipe;
            set
            {
                recipe = value;
                OnPropertyChanged("Recipe");
            }
        }

        private List<string> HideableProperties { get; set; }
        public ObservableCollection<string> ShowProperties { get; set; }

        public new event PropertyChangedEventHandler PropertyChanged;

        public EditRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IRecipeController recipeController) : base(navigationService, pageDialogService, recipeController)
        {
            OnCancelCommand = new DelegateCommand(OnCancelPressed);
            OnSubmitCommand = new DelegateCommand(OnSubmit);
            OnAddPropertyCommand = new DelegateCommand(OnAddPropertyPressed);
            OnRemovePropertyCommand = new DelegateCommand<string>(OnRemovedPropertyPressed);

            HideableProperties = new List<string>
            {
                "PrepTime",
                "RestTime",
                "Method",
                "NumPortions",
                "OriginalRecipe",
                "Description",
                "Requirements",
                "ServeTips"
            };

            ShowProperties = new ObservableCollection<string>();
        }

        private void OnPropertyChanged(string propertyName)
        {
            PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        public async void OnCancelPressed()
        {
            var response = await DialogService.DisplayAlertAsync("Pas op!", "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?", "Ja", "Nee");
            if (response) GoBackAsync();
        }

        public async void OnSubmit()
        {
            if (CreateMode)
            {
                await RecipeController.SaveRecipeAsync(Recipe);

                var parameters = new NavigationParameters
                {
                    { "SelectedRecipe", Recipe.ID }
                };

                NavigateToPageAsync("../DisplayRecipePage", parameters);
            }
            else
            {
                var response = await DialogService.DisplayAlertAsync("Pas op!", "Deze actie kan niet ongedaan worden. Weer u het zeker?", "Ja", "Nee");

                if (response)
                {
                    await RecipeController.SaveRecipeAsync(Recipe);
                    GoBackAsync();
                }
            }
        }

        public async void OnAddPropertyPressed()
        {
            var cancelButton = "Annuleer";
            var hiddenProperties = HideableProperties.Except(ShowProperties).ToArray();

            for (int i = 0; i < hiddenProperties.Length; i++) hiddenProperties[i] = Recipe.EnToDutchTranslation[hiddenProperties[i]];

            string action = await DialogService.DisplayActionSheetAsync("Voeg nieuw veld toe", cancelButton, null, hiddenProperties);

            if (action != cancelButton)
            {
                action = Recipe.EnToDutchTranslation.FirstOrDefault(x => x.Value == action).Key;

                ShowProperties.Add(action);
                OnPropertyChanged("ShowProperties");
            }
        }

        public async void OnRemovedPropertyPressed(string property)
        {
            var response = await DialogService.DisplayAlertAsync("Pas op!", $"Weet u zeker dat u het veld {Recipe.EnToDutchTranslation[property]} wilt verwijderen?", "Ja", "Nee");

            if (response)
            {
                ShowProperties.Remove(property);
                OnPropertyChanged("ShowProperties");
            }
        }

        private void PopulatePage()
        {
            foreach (var property in HideableProperties)
            {
                try
                {
                    var value = Recipe.GetType().GetProperty(property).GetValue(Recipe);

                    if (value == null) { }
                    else if (value is TimeSpan time && time != new TimeSpan()) ShowProperties.Add(property);
                    else if (value is string) ShowProperties.Add(property);
                    else if (value is int val && val != 0) ShowProperties.Add(property);
                }
                catch (Exception)
                {
                }
            }

            if (ShowProperties.Count > 0) OnPropertyChanged("ShowProperties");
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe"))
            {
                var recipeID = (int)parameters["SelectedRecipe"];
                Recipe = await RecipeController.GetRecipeAsync(recipeID);

                PopulatePage();
            }

            if (Recipe == null)
            {
                Recipe = new Recipe();
                CreateMode = true;
            }
            else CreateMode = false;
        }
    }
}
