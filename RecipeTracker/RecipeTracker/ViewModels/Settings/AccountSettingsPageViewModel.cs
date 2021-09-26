using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Services;
using System.Threading.Tasks;

namespace RecipeTracker.ViewModels.Settings
{
    /// <summary>
    /// Class <c>AccountSettingsPageViewModel</c> is the view model for the AccountSettingsPage.
    /// This page displays the user's account information and manages login and logout.
    /// </summary>
    public class AccountSettingsPageViewModel : ViewModelBase
    {
        public DelegateCommand OnLoginPressedCommand { get; }
        public DelegateCommand OnLogoutPressedCommand { get; }

        private GoogleUser user;
        public GoogleUser User
        {
            get => user;
            private set => SetProperty(ref user, value);
        }

        public AccountSettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            OnLoginPressedCommand = new DelegateCommand(OnLogin);
            OnLogoutPressedCommand = new DelegateCommand(OnLogout);
        }

        /// <summary>
        /// Executes task <seealso cref="LoginUserAsync"/>.
        /// </summary>
        private async void OnLogin()
        {
            await LoginUserAsync();
        }
        
        /// <summary>
        /// Executes task <seealso cref="LogoutUserAsync"/>.
        /// </summary>
        private async void OnLogout()
        {
            await LogoutUserAsync();
        }

        /// <summary>
        /// Logs in the user.
        /// </summary>
        internal async Task LoginUserAsync()
        {
            // Execute login and set the user if completed
            if (App.IsConnected())
            {
                var response = await AuthService.LoginAsync();

                if (response == GoogleActionStatus.Completed) User = AuthService.GetUser();
            }
            else await DialogService.DisplayAlertAsync("Geen internet connectie", "Het is niet mogelijk om in te loggen. Controleer uw internet connectie en probeer opnieuw.", "Ok");
        }

        /// <summary>
        /// Logs out the current user.
        /// </summary>
        internal async Task LogoutUserAsync()
        {
            // Logout the user, if successful, return to mainpage
            var response = await AuthService.LogoutAsync();

            if (response)
            {
                User = null;

                NavigateToPageAsync("../../");
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            User = AuthService.GetUser();
        }
    }
}
