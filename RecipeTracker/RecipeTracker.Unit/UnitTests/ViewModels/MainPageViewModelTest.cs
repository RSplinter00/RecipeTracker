using Moq;
using NUnit.Framework;
using Plugin.GoogleClient;
using Prism.Navigation;
using RecipeTracker.Models;
using RecipeTracker.ViewModels;
using RecipeTracker.Views;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace RecipeTracker.Unit.UnitTests.ViewModels
{
    /// <summary>
    /// Class <c>MainPageViewModeltest</c> contains unit tests for class <seealso cref="RecipeTracker.ViewModels.MainPageViewModel"/>.
    /// </summary>
    [TestFixture]
    public class MainPageViewModelTest : ViewModelBaseTest
    {
        private MainPageViewModel MainPageViewModel { get; set; }

        [SetUp]
        public void SetUp()
        {
            MainPageViewModel = new MainPageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, AuthServiceMock.Object, DatabaseServiceMock.Object);
        }

        [Test]
        public async Task ToggleLogin_WhenUserNull_ShouldLogin()
        {
            // Arrange
            AuthServiceMock.SetupSequence(authService => authService.GetUser())
                .Returns(() => null)
                .Returns(CurrentUser);
            AuthServiceMock.Setup(authService => authService.LoginAsync()).Returns(Task.Run(() => GoogleActionStatus.Completed));
            DatabaseServiceMock.Setup(databaseService => databaseService.SyncRecipesAsync()).Verifiable();
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipesAsync()).Returns(Task.Run(() => new List<Recipe>()));

            // Act
            await MainPageViewModel.ToggleLogin();

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.AtLeastOnce, "Function IAuthenticationService.GetUser not called atleast once.");
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Once, "Function IAuthenticationService.LoginAsync not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SyncRecipesAsync(), Times.Once, "Function IDatabaseService.SyncRecipesAsync not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipesAsync(), Times.Once, "Function IDatabaseService.GetRecipesAsync not called exactly once.");
        }

        [Test]
        public async Task ToggleLogin_WhenUserNotNull_ShouldLogout()
        {
            // Arrange
            var alertTitle = "Uitgelogd!";
            var alertMessage = "U bent succesvol uitgelogd.";
            var alertCancelButton = "Ok";

            AuthServiceMock.SetupSequence(authService => authService.GetUser())
                .Returns(CurrentUser)
                .Returns(() => null);
            AuthServiceMock.Setup(authService => authService.LogoutAsync()).Returns(Task.Run(() => true));
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipesAsync()).Returns(Task.Run(() => new List<Recipe>()));

            // Act
            await MainPageViewModel.ToggleLogin();

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.AtLeastOnce, "Function IAuthenticationService.GetUser not called atleast once.");
            AuthServiceMock.Verify(authService => authService.LogoutAsync(), Times.Once, "Function IAuthenticationService.Logout not called exactly once.");
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Never, "Function IAuthenticationService.LoginAsinc called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SyncRecipesAsync(), Times.Never, "Function IDatabaseService.SyncRecipes called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipesAsync(), Times.Once, "Function IDatabaseService.GetRecipesAsync not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Logout alert not displayed exactly once.");
        }

        [Test]
        public async Task ToggleLogin_WhenRefusingLogin_ShouldNotDisplayAlert()
        {
            // Arrange
            var alertTitle = "Uitgelogd!";
            var alertMessage = "U bent succesvol uitgelogd.";
            var alertCancelButton = "Ok";

            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(CurrentUser);
            AuthServiceMock.Setup(authService => authService.LogoutAsync()).Returns(Task.Run(() => false));

            // Act
            await MainPageViewModel.ToggleLogin();

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.AtLeastOnce, "Function IAuthenticationService.GetUser not called atleast once.");
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Never, "Function IAuthenticationService.LoginAsync called atleast once.");
            AuthServiceMock.Verify(authService => authService.LogoutAsync(), Times.Once, "Function IAuthenticationService.Logout not not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipesAsync(), Times.Never, "Function IDatabaseService.GetRecipesAsync called atleast once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Never, "Logout alert displayed atleast once.");
        }

        [Test]
        public void SetLoginToolbarText_WhenUserNotNull_ShouldSetToolbarTextForLogout()
        {
            // Arrange
            var expectedToolbarText = "Uitloggen";
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(CurrentUser);

            // Act
            MainPageViewModel.SetLoginToolbarText();

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Once, "Function IAuthenticationService.GetUser not called exactly once.");
            Assert.AreEqual(expectedToolbarText, MainPageViewModel.LoginToolbarItemText, "Attribute MainPageViewModel.LoginToolbarItemText did not equal expected toolbar text.");
        }

        [Test]
        public void SetLoginToolbarText_WhenUserNull_ShouldSetToolbarTextForLogin()
        {
            // Arrange
            var expectedToolbarText = "Inloggen";
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => null);

            // Act
            MainPageViewModel.SetLoginToolbarText();

            // Assert
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.Once, "Function IAuthenticationService.GetUser not called exactly once.");
            Assert.AreEqual(expectedToolbarText, MainPageViewModel.LoginToolbarItemText, "Attribute MainPageViewModel.LoginToolbarItemText did not equal expected toolbar text.");
        }

        [Test]
        public void OnRecipeSelectedCommand_WithValidRecipe_ShouldNavigateToDisplayRecipePage()
        {
            // Arange
            var alertTitle = "Incorrecte recept!";
            var alertMessage = "Dit recept bestaat niet.";
            var alertCancelButton = "Ok";

            var selectedRecipe = new Recipe { Id = Guid.NewGuid() };
            var expectedNavigationParams = new NavigationParameters
            {
                { "SelectedRecipe", selectedRecipe.Id }
            };

            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(nameof(DisplayRecipePage))).Verifiable();

            // Act
            MainPageViewModel.OnRecipeSelectedCommand?.Execute(selectedRecipe);

            // Assert
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(nameof(DisplayRecipePage), expectedNavigationParams), Times.Once, "Command OnRecipeSelected did not navigate user to DisplayRecipePage with correct parameters.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Never, "Incorrect recipe alert displayed atleast once.");
        }

        [Test]
        public void OnRecipeSelectedCommand_WithNullValue_ShouldDisplayAlert()
        {
            // Arrange
            var alertTitle = "Incorrecte recept!";
            var alertMessage = "Dit recept bestaat niet.";
            var alertCancelButton = "Ok";

            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(nameof(DisplayRecipePage)));

            // Act
            MainPageViewModel.OnRecipeSelectedCommand?.Execute(null);

            //Assert
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(nameof(DisplayRecipePage)), Times.Never, "Command OnRecipeSelected navigated user to DisplayRecipePage.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Incorrect recipe alert not displayed exactly once.");
        }

        [Test]
        public void OnRefreshCommand_ShouldGetRecipes()
        {
            // Arrange
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipesAsync()).Returns(Task.Run(() => new List<Recipe>()));

            // Act
            MainPageViewModel.OnRefreshCommand?.Execute();

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipesAsync(), Times.Once, "Function IDatabaseService.GetRecipesAsync not called exactly once.");
            Assert.IsNotNull(MainPageViewModel.Recipes, "Attribute MainPageViewModel.Recipes is null.");
        }

        [Test]
        public void OnNavigatedTo_WhenLoggingIn_ShouldSetupMainPage()
        {
            // Arrange
            var expectedToolbarText = "Uitloggen";

            AuthServiceMock.Setup(authService => authService.LoginAsync()).Returns(Task.Run(() => GoogleActionStatus.Completed));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(CurrentUser);
            DatabaseServiceMock.Setup(databaseService => databaseService.SyncRecipesAsync()).Verifiable();
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipesAsync()).Returns(Task.Run(() => new List<Recipe>()));

            // Act
            MainPageViewModel.OnNavigatedTo(null);

            // Assert
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Once, "Function IAuthenticationService.LoginAsync not called exactly once.");
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.AtLeastOnce, "Function IAuthenticationService.GetUser not called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SyncRecipesAsync(), Times.Once, "Function IDatabaseService.SyncRecipes not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipesAsync(), Times.Once, "Function IDatabaseService.GetRecipesAsync not called exactly once.");
            Assert.AreEqual(expectedToolbarText, MainPageViewModel.LoginToolbarItemText, "Attribute MainPageViewModel.LoginToolbarItemText did not equal expected toolbar text.");
            Assert.IsNotNull(MainPageViewModel.Recipes, "Attribute MainPageViewModel.Recipes is null");
        }

        [Test]
        public void OnNavigatedTo_WithoutLoggingIn_ShouldSetupMainPage()
        {
            // Arrange
            var expectedToolbarText = "Inloggen";

            AuthServiceMock.Setup(authService => authService.LoginAsync()).Returns(Task.Run(() => GoogleActionStatus.Canceled));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(() => null);
            DatabaseServiceMock.Setup(databaseService => databaseService.SyncRecipesAsync()).Verifiable();
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipesAsync()).Returns(Task.Run(() => new List<Recipe>()));

            // Act
            MainPageViewModel.OnNavigatedTo(null);

            // Assert
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Once, "Function IAuthenticationService.LoginAsync not called exactly once.");
            AuthServiceMock.Verify(authService => authService.GetUser(), Times.AtLeastOnce, "Function IAuthenticationService.GetUser not called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SyncRecipesAsync(), Times.Never, "Function IDatabaseService.SyncRecipes called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipesAsync(), Times.Once, "Function IDatabaseService.GetRecipesAsync not called exactly once.");
            Assert.AreEqual(expectedToolbarText, MainPageViewModel.LoginToolbarItemText, "Attribute MainPageViewModel.LoginToolbarItemText did not equal expected toolbar text.");
            Assert.IsNotNull(MainPageViewModel.Recipes, "Attribute MainPageViewModel.Recipes is null");
        }

        [Test]
        public void OnNavigatedTo_AfterSecondCall_ShouldNotLoginAndSync()
        {
            // Arrange
            AuthServiceMock.Setup(authService => authService.LoginAsync()).Returns(Task.Run(() => GoogleActionStatus.Completed));
            AuthServiceMock.Setup(authService => authService.GetUser()).Returns(CurrentUser);
            DatabaseServiceMock.Setup(databaseService => databaseService.SyncRecipesAsync()).Verifiable();
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipesAsync()).Returns(Task.Run(() => new List<Recipe>()));

            // Act
            MainPageViewModel.OnNavigatedTo(null);
            MainPageViewModel.OnNavigatedTo(null);

            // Assert
            AuthServiceMock.Verify(authService => authService.LoginAsync(), Times.Once, "Function IAuthenticationService.LoginAsync not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SyncRecipesAsync(), Times.Once, "Function IDatabaseService.SyncRecipes not called exactly once.");
        }
    }
}
