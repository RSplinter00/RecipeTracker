using Moq;
using NUnit.Framework;
using Plugin.GoogleClient;
using RecipeTracker.ViewModels.Settings;
using System.Threading.Tasks;

namespace RecipeTracker.Unit.UnitTests.ViewModels.Settings
{
    /// <summary>
    /// Class <c>AccountSettingsPageViewModelTest</c> contains unit tests for class <seealso cref="RecipeTracker.ViewModels.Settings.AccountSettingsPageViewModelTest"/>.
    /// </summary>
    [TestFixture]
    public class AccountSettingsPageViewModelTest : ViewModelBaseTest
    {
        private AccountSettingsPageViewModel AccountSettingsPageViewModel { get; set; }

        [SetUp]
        public void SetUp()
        {
            AccountSettingsPageViewModel = new AccountSettingsPageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, AuthServiceMock.Object, DatabaseServiceMock.Object);
        }

        [Test]
        public async Task LoginUserAsync_WhenLoggingInWithInternetConnection_ShouldLoginAndSetUser()
        {
            // Arrange
            var alertTitle = "Geen internet connectie";
            var alertMessage = "Het is niet mogelijk om in te loggen. Controleer uw internet connectie en probeer opnieuw.";
            var alertCancelButton = "Ok";

            AuthServiceMock.Setup(authService => authService.LoginAsync()).Returns(Task.Run(() => GoogleActionStatus.Completed));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => CurrentUser);
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();

            // Act
            await AccountSettingsPageViewModel.LoginUserAsync();

            // Assert
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Once, "Function AuthenticationService.LoginAsync not called exactly once.");
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Once, "Function AuthenticationService.GetUser not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Never, "Alert for no internet connection called atleast once.");
            Assert.AreEqual(CurrentUser, AccountSettingsPageViewModel.User, "Attribute AccountSettingsPageViewModel.User does not equal the expected user.");
        }

        [Test]
        public async Task LoginUserAsync_WhenRefusingLoginWithInternetConnection_ShouldDoNothing()
        {
            // Arrange
            var alertTitle = "Geen internet connectie";
            var alertMessage = "Het is niet mogelijk om in te loggen. Controleer uw internet connectie en probeer opnieuw.";
            var alertCancelButton = "Ok";

            AuthServiceMock.Setup(authService => authService.LoginAsync()).Returns(Task.Run(() => GoogleActionStatus.Canceled));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => null);
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();

            // Act
            await AccountSettingsPageViewModel.LoginUserAsync();

            // Assert
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Once, "Function AuthenticationService.LoginAsync not called exactly once.");
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Never, "Function AuthenticationService.GetUser called atleast once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Never, "Alert for no internet connection called atleast once.");
            Assert.IsNull(AccountSettingsPageViewModel.User, "Attribute AccountSettingsPageViewModel.User is not null.");
        }

        [Test]
        public async Task LogoutUserAsync_WhenLoggingOut_ShouldLogoutAndResetUser()
        {
            // Arrange
            AuthServiceMock.Setup(authService => authService.LogoutAsync()).Returns(Task.Run(() => true));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => null);

            // Act
            await AccountSettingsPageViewModel.LogoutUserAsync();

            // Assert
            AuthServiceMock.Verify(authService => authService.LogoutAsync(), Times.Once, "Function AuthenticationService.LoginAsync not called exactly once.");
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Never, "Function AuthenticationService.GetUser called atleast once.");
            Assert.IsNull(AccountSettingsPageViewModel.User, "Attribute AccountSettingsPageViewModel.User is not null.");
        }

        [Test]
        public async Task LogoutUserAsync_WhenFailingLogout_ShouldDoNothing()
        {
            // Arrange
            AuthServiceMock.Setup(authService => authService.LogoutAsync()).Returns(Task.Run(() => false));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => null);

            // Act
            await AccountSettingsPageViewModel.LogoutUserAsync();

            // Assert
            AuthServiceMock.Verify(authService => authService.LogoutAsync(), Times.Once, "Function AuthenticationService.LoginAsync not called exactly once.");
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Never, "Function AuthenticationService.GetUser called atleast once.");
            Assert.IsNull(AccountSettingsPageViewModel.User, "Attribute AccountSettingsPageViewModel.User is not null.");
        }

        [Test]
        public void OnNavigatedTo_WhenLoggedIn_ShouldSetCurrentUser()
        {
            // Arrange
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => CurrentUser);

            // Act
            AccountSettingsPageViewModel.OnNavigatedTo(null);

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Once, "Function AuthenticationService.GetUser not called exactly once.");
            Assert.AreEqual(CurrentUser, AccountSettingsPageViewModel.User, "Attribute AccountSettingsPageViewModel.User does not equal the expected user.");
        }

        [Test]
        public void OnNavigatedTo_WhenNotLoggedIn_ShouldSetUserNull()
        {
            // Arrange
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => null);

            // Act
            AccountSettingsPageViewModel.OnNavigatedTo(null);

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Once, "Function AuthenticationService.GetUser not called exactly once.");
            Assert.IsNull(AccountSettingsPageViewModel.User, "Attribute AccountSettingsPageViewModel.User is not null.");
        }
    }
}
