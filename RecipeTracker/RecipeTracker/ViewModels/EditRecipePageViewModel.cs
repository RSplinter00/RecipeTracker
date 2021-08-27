using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using Xamarin.Essentials;
using System.Threading.Tasks;

namespace RecipeTracker.ViewModels
{
    /// <summary>
    /// Class <c>EditRecipePageViewModel</c> is the viewmodel for the edit recipe page.
    /// This page allows the user to create or edit recipes.
    /// </summary>
    public class EditRecipePageViewModel : ViewModelBase, INotifyPropertyChanged
    {
        public string PageName => CreateMode ? "Nieuw recept" : "Recept wijzigen";

        private bool createMode;
        public bool CreateMode
        {
            get => createMode;
            internal set
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

        /// <summary>
        /// Executes the property changed events for the given property.
        /// </summary>
        /// <param name="propertyName">Name of the property which has been changed.</param>
        private void OnPropertyChanged(string propertyName)
        {
            if (DeviceInfo.Platform != DevicePlatform.Unknown) PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        /// <summary>
        /// Cancels the editting of the recipe and navigates back to the previous page, without saving any changes.
        /// </summary>
        public async void OnCancelPressed()
        {
            if (await DialogService.DisplayAlertAsync("Pas op!", "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?", "Ja", "Nee"))
            {
                if (CreateMode) GoBackAsync();
                else
                {
                    var paramaters = new NavigationParameters
                    {
                        { "SelectedRecipe", Recipe.Id }
                    };

                    NavigateToPageAsync("../../DisplayRecipePage", paramaters);
                }
            }
        }

        /// <summary>
        /// Saves any changes made to the recipe and navigates to the DisplayRecipePage.
        /// </summary>
        public async void OnSubmit()
        {
            if (!IsValid())
            {
                await DialogService.DisplayAlertAsync("Let op!", "Het recept kan niet opgeslagen worden. De naam en/of de bereidingswijze ontbreken.", "Ok");
                return;
            }

            var response = true;

            if (!CreateMode) response = await DialogService.DisplayAlertAsync("Pas op!", "Deze actie kan niet ongedaan worden.", "Opslaan", "Annuleer");

            if (response)
            {
                await DatabaseService.SaveRecipeAsync(Recipe);

                var navigationPath = "../DisplayRecipePage";
                if (!CreateMode) navigationPath = navigationPath.Insert(0, "../");
                var parameters = new NavigationParameters
                {
                    { "SelectedRecipe", Recipe.Id }
                };

                NavigateToPageAsync(navigationPath, parameters);
            }
        }

        /// <summary>
        /// Checks if the recipe and its required properties exist.
        /// </summary>
        /// <returns>If the recipe is considered valid.</returns>
        internal bool IsValid()
        {
            return Recipe != null && !string.IsNullOrWhiteSpace(Recipe.Name) && !string.IsNullOrWhiteSpace(Recipe.Steps);
        }

        /// <summary>
        /// Executes task <see cref="AddProperty"/>.
        /// </summary>
        private async void OnAddPropertyPressed()
        {
            await AddProperty();
        }

        /// <summary>
        /// Executes task <see cref="RemoveProperty(string)"/>.
        /// </summary>
        /// <param name="propertyName"></param>
        private async void OnRemovePropertyPressed(string propertyName)
        {
            await RemoveProperty(propertyName);
        }

        /// <summary>
        /// Prompts the user to add a new property to the recipe and shows the selected property on the page.
        /// </summary>
        public async Task AddProperty()
        {
            var cancelButton = "Annuleer";
            var hiddenProperties = HideableProperties.Except(ShowProperties).ToArray();

            // Makes a list of all properties in Dutch
            for (int i = 0; i < hiddenProperties.Length; i++) hiddenProperties[i] = Recipe.EnToNlTranslation(hiddenProperties[i]);

            // Prompts the user to choose which property to add to the page.
            var action = await DialogService.DisplayActionSheetAsync("Voeg nieuw veld toe", cancelButton, null, hiddenProperties);

            if (action != null && action != cancelButton)
            {
                // If the user has selected a property, show it on the page.
                action = Recipe.NlToEnTranslation(action);

                if (!string.IsNullOrEmpty(action))
                {
                    ShowProperties.Add(action);
                    OnPropertyChanged("ShowProperties");
                }
            }
        }

        /// <summary>
        /// Removes the selected property from the recipe, by hiding it on the page and resetting its value.
        /// </summary>
        /// <param name="propertyName">Name of the selected property to be deleted.</param>
        public async Task RemoveProperty(string propertyName)
        {
            var response = await DialogService.DisplayAlertAsync("Pas op!", $"Weet u zeker dat u het veld {Recipe.EnToNlTranslation(propertyName)} wilt verwijderen?", "Ja", "Nee");

            if (response)
            {
                // If the user confirms deleting the property, the value will be reset and the property will be hidden on the page.
                Recipe.GetType().GetProperty(propertyName).SetValue(Recipe, null);

                ShowProperties.Remove(propertyName);

                OnPropertyChanged("Recipe");
                OnPropertyChanged("ShowProperties");
            }
        }

        /// <summary>
        /// Checks which properties should be displayed on the page, based on if their value is not the default value.
        /// </summary>
        internal void PopulatePage()
        {
            foreach (var property in HideableProperties)
            {
                try
                {
                    // Get the value for each property of the recipe
                    var value = Recipe.GetType().GetProperty(property).GetValue(Recipe);

                    // If the value is not the default value, show it on the page.
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
            try
            {
                if (parameters.ContainsKey("SelectedRecipe"))
                {
                    // Retrieve and set the selected recipe
                    var recipeID = (Guid)parameters["SelectedRecipe"];

                    Recipe = await DatabaseService.GetRecipeAsync(recipeID);
                }
            }
            catch (Exception)
            {
            }

            if (Recipe == null)
            {
                // If Recipe is null, then the user is creating a new recipe.
                Recipe = new Recipe();
                CreateMode = true;
            }
            else
            {
                // If the Recipe is not null, then the user is editting te selected recipe.
                CreateMode = false;
                PopulatePage();
            }
        }
    }
}
