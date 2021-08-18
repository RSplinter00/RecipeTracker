using NUnit.Framework;
using RecipeTracker.Models;
using RecipeTracker.ViewModels;
using AutoFixture;
using System;
using System.Threading.Tasks;
using Moq;
using RecipeTracker.Views;
using Prism.Navigation;

namespace RecipeTracker.Unit.UnitTests.ViewModels
{
    [TestFixture]
    public class DisplayRecipePageViewModelTest : ViewModelBaseTest
    {
        private DisplayRecipePageViewModel DisplayRecipePageViewModel { get; set; }
        private Recipe SelectedRecipe { get; set; }

        [SetUp]
        public void SetUp()
        {
            DisplayRecipePageViewModel = new DisplayRecipePageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, AuthServiceMock.Object, DatabaseServiceMock.Object);
            SelectedRecipe = Fixture.Build<Recipe>().Without(i => i.Id).Do(i => i.Id = Fixture.Create<Guid>()).Create();
        }

        [Test]
        public void OnDeleteRecipeCommand_WhenConnectedAndAccepted_ShouldDeleteRecipeAndNavigateBack()
        {
            // Arrange
            var alertTitle = "Waarschuwing!";
            var alertMessage = "U staat op het punt dit recept te verwijderen. Dit kan niet terug gedraaid worden.";
            var alertMessageLocal = "U staat op het punt om dit recept lokaal te verwijderen. Als dit recept niet is opgeslagen in de cloud, kan dit niet worden terug gedraaid.";
            var alertAcceptButton = "Verwijder";
            var alertCancelButton = "Annuleer";
            
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => true));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessageLocal, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => true));
            DatabaseServiceMock.Setup(databaseService => databaseService.DeleteRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => true));
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.Recipe = SelectedRecipe;
            DisplayRecipePageViewModel.OnDeleteRecipeCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Alert for deleting a recipe from Firebase not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessageLocal, alertAcceptButton, alertCancelButton), Times.Never, "Alert for deleting a cached recipe called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.DeleteRecipeAsync(SelectedRecipe.Id), Times.Once, "Function IDatabaseService.DeleteRecipeAsync for the selected recipe not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
        }

        [Test]
        public void OnDeleteRecipeCommand_WhenConnectedAndCanceled_ShouldNotDeleteRecipe()
        {
            // Arrange
            var alertTitle = "Waarschuwing!";
            var alertMessage = "U staat op het punt dit recept te verwijderen. Dit kan niet terug gedraaid worden.";
            var alertMessageLocal = "U staat op het punt om dit recept lokaal te verwijderen. Als dit recept niet is opgeslagen in de cloud, kan dit niet worden terug gedraaid.";
            var alertAcceptButton = "Verwijder";
            var alertCancelButton = "Annuleer";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => false));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessageLocal, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => false));
            DatabaseServiceMock.Setup(databaseService => databaseService.DeleteRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => false));
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.Recipe = SelectedRecipe;
            DisplayRecipePageViewModel.OnDeleteRecipeCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Alert for deleting a recipe from Firebase not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessageLocal, alertAcceptButton, alertCancelButton), Times.Never, "Alert for deleting a cached recipe called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.DeleteRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.DeleteRecipeAsync for the selected recipe called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Never, "Function INavigationService.GoBackAsync called atleast once.");
        }

        [Test]
        public void OnEditRecipeCommand_WithRecipe_ShouldNavigateToPageWithParameters()
        {
            // Arrange
            var alertTitle = "Waarschuwing!";
            var alertMessage = "Niet mogelijk om dit recept aan te passen.";
            var alertCancelButton = "Ok";
            var expectedPageName = nameof(EditRecipePage);
            var expectedParameters = new NavigationParameters
            {
                { "SelectedRecipe", SelectedRecipe.Id }
            };

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(expectedPageName, expectedParameters)).Verifiable();

            // Act
            DisplayRecipePageViewModel.Recipe = SelectedRecipe;
            DisplayRecipePageViewModel.OnEditRecipeCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Never, "Alert for failed navigation called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(expectedPageName, expectedParameters), Times.Once, $"Function INavationService.NavigateAsync to {expectedPageName} with correct parameters not called exactly once.");
        }

        [Test]
        public void OnEditRecipeCommand_WithoutRecipe_ShouldNotNavigateToPage()
        {
            // Arrange
            var alertTitle = "Waarschuwing!";
            var alertMessage = "Niet mogelijk om dit recept aan te passen.";
            var alertCancelButton = "Ok";
            var expectedPageName = nameof(EditRecipePage);
            var expectedParameters = new NavigationParameters
            {
                { "SelectedRecipe", Guid.Empty }
            };

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(expectedPageName, expectedParameters)).Verifiable();

            // Act
            DisplayRecipePageViewModel.Recipe = null;
            DisplayRecipePageViewModel.OnEditRecipeCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Alert for failed navigation not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(expectedPageName, expectedParameters), Times.Never, $"Function INavationService.NavigateAsync to {expectedPageName} called atleast once.");
        }

        [Test]
        public void OnNavigatedTo_WithCorrectParameters_ShouldSetSelectedRecipe()
        {
            // Arrange
            var alertTitle = "Niet gevonden!";
            var alertMessage = "Het recept kan niet geladen worden.";
            var alertCancelButton = "Ok";
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", SelectedRecipe.Id }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Once, "Function IDatabaseService.GetRecipeAsync for selected recipe not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Never, "Alert for failed retrieving of recipe called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Never, "Function INavigationService.GoBackAsync called atleast once.");
            Assert.AreEqual(SelectedRecipe, DisplayRecipePageViewModel.Recipe);
        }

        [Test]
        public void OnNavigatedTo_WithoutParametersWhenRecipeIsNull_ShouldReturnToPreviousPage()
        {
            // Arrange
            var alertTitle = "Niet gevonden!";
            var alertMessage = "Het recept kan niet geladen worden.";
            var alertCancelButton = "Ok";

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(() => null);
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.OnNavigatedTo(null);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.GetRecipeAsync for selected recipe called atleast once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Alert for failed retrieving of recipe not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
        }

        [Test]
        public void OnNavigatedTo_WithIncorrectParameterName_ShouldReturnToPreviousPage()
        {
            // Arrange
            var alertTitle = "Niet gevonden!";
            var alertMessage = "Het recept kan niet geladen worden.";
            var alertCancelButton = "Ok";
            var parameters = new NavigationParameters
            {
                { "SelectedRecipeId", SelectedRecipe.Id }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.GetRecipeAsync for selected recipe called atleast once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Alert for failed retrieving of recipe not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
        }

        [Test]
        public void OnNavigatedTo_WithIncorrectId_ShouldReturnToPreviousPage()
        {
            // Arrange
            var alertTitle = "Niet gevonden!";
            var alertMessage = "Het recept kan niet geladen worden.";
            var alertCancelButton = "Ok";
            var incorrectId = Guid.NewGuid();
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", incorrectId }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(incorrectId)).Returns(() => null);
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.GetRecipeAsync for selected recipe called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(incorrectId), Times.Once, "Function IDatabaseService.GetRecipeAsync for non-existant recipe not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Alert for failed retrieving of recipe not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
        }

        [Test]
        public void OnNavigatedTo_WithInvalidDataTypeParameters_ShouldReturnToPreviousPage()
        {
            // Arrange
            var alertTitle = "Niet gevonden!";
            var alertMessage = "Het recept kan niet geladen worden.";
            var alertCancelButton = "Ok";
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", Fixture.Create<double>() }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            DisplayRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.GetRecipeAsync for selected recipe called atleast once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertCancelButton), Times.Once, "Alert for failed retrieving of recipe not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
        }
    }
}
