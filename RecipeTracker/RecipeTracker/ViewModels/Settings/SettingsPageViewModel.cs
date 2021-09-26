using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using System.Collections.Generic;
using System.Linq;

namespace RecipeTracker.ViewModels.Settings
{
    /// <summary>
    /// Class <c>SettingsPageViewModel</c> is the view model for the SettingsPage.
    /// This page displays a list of all possible settings.
    /// </summary>
    public class SettingsPageViewModel : ViewModelBase
    {
        public DelegateCommand OnSelectionChangedCommand { get; }

        private string selectedItem;
        public string SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        // List of all possible settings.
        // Key: Name of the corresponding page
        // Value: The to be displayed value
        public Dictionary<string, string> SettingsProperties { get; } = new Dictionary<string, string>()
        {
            { "Account", "Account" },
            { "ReportIssue", "Probleem melden" }
        };

        public SettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            OnSelectionChangedCommand = new DelegateCommand(OnSelectionChanged);
        }

        /// <summary>
        /// Navigates to the corresponding page, after selecting a setting.
        /// </summary>
        public void OnSelectionChanged()
        {
            if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                // If SelectedItem is a valid string, check the page name
                var pageName = SettingsProperties.FirstOrDefault(x => x.Value == SelectedItem).Key;

                // Reset the selected item and navigate to the settings page.
                SelectedItem = null;
                NavigateToPageAsync($"{pageName}SettingsPage");
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
        }
    }
}
