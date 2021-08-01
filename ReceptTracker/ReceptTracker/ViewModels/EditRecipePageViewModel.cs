using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Services;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
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

        public EditRecipePageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            OnCancelCommand = new DelegateCommand(OnCancelPressed);
            OnSubmitCommand = new DelegateCommand(OnSubmit);
            OnAddPropertyCommand = new DelegateCommand(OnAddPropertyPressed);
            OnRemovePropertyCommand = new DelegateCommand<string>(OnRemovePropertyPressed);

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
            if (await DialogService.DisplayAlertAsync("Pas op!", "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?", "Ja", "Nee")) GoBackAsync();
        }

        public async void OnSubmit()
        {
            var response = true;

            if (!CreateMode) response = await DialogService.DisplayAlertAsync("Pas op!", "Deze actie kan niet ongedaan worden.", "Opslaan", "Annuleer");

            if (response) await DatabaseService.SaveRecipeAsync(Recipe);

            if (CreateMode)
            {
                var parameters = new NavigationParameters
                {
                    { "SelectedRecipe", Recipe.Id }
                };

                NavigateToPageAsync("../DisplayRecipePage", parameters);
            }
            else GoBackAsync();
        }

        public async void OnAddPropertyPressed()
        {
            var cancelButton = "Annuleer";
            var hiddenProperties = HideableProperties.Except(ShowProperties).ToArray();

            for (int i = 0; i < hiddenProperties.Length; i++) hiddenProperties[i] = Recipe.EnToNlTranslation(hiddenProperties[i]);

            var action = await DialogService.DisplayActionSheetAsync("Voeg nieuw veld toe", cancelButton, null, hiddenProperties);

            if (action != cancelButton)
            {
                action = Recipe.NlToEnTranslation(action);

                ShowProperties.Add(action);
                OnPropertyChanged("ShowProperties");
            }
        }

        public async void OnRemovePropertyPressed(string propertyName)
        {
            var response = await DialogService.DisplayAlertAsync("Pas op!", $"Weet u zeker dat u het veld {Recipe.EnToNlTranslation(propertyName)} wilt verwijderen?", "Ja", "Nee");

            if (response)
            {
                Recipe.GetType().GetProperty(propertyName).SetValue(Recipe, null);

                ShowProperties.Remove(propertyName);

                OnPropertyChanged("Recipe");
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
                catch (Exception e)
                {
                    Debug.WriteLine(e.Message);
                }
            }

            if (ShowProperties.Count > 0) OnPropertyChanged("ShowProperties");
        }

        public async override void OnNavigatedTo(INavigationParameters parameters)
        {
            if (parameters.ContainsKey("SelectedRecipe"))
            {
                var recipeID = (Guid)parameters["SelectedRecipe"];

                Recipe = await DatabaseService.GetRecipeAsync(recipeID);
            }

            if (Recipe == null)
            {
                Recipe = new Recipe();
                CreateMode = true;
            }
            else
            {
                CreateMode = false;
                PopulatePage();
            }
        }
    }
}
