using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using System;
using System.Collections.Generic;

namespace RecipeTracker.ViewModels.Settings
{
    public class SettingsPageViewModel : ViewModelBase
    {
        public DelegateCommand OnSelectionChangedCommand { get; }
        private string selectedItem;
        public string SelectedItem
        {
            get => selectedItem;
            set => SetProperty(ref selectedItem, value);
        }

        public List<string> SettingsProperties { get; } = new List<string> { "Account" };

        public SettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            OnSelectionChangedCommand = new DelegateCommand(OnSelectionChanged);
        }

        public void OnSelectionChanged()
        {
            if (!string.IsNullOrWhiteSpace(SelectedItem))
            {
                NavigateToPageAsync($"{SelectedItem}SettingsPage");
                SelectedItem = null;
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Console.WriteLine();
        }
    }
}
