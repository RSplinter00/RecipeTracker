using AutoFixture;
using Moq;
using NUnit.Framework;
using Prism.Navigation;
using ReceptTracker.Models;
using ReceptTracker.ViewModels;
using ReceptTracker.Views;
using System;
using System.Threading.Tasks;

namespace ReceptTracker.Unit.UnitTests.ViewModels
{
    [TestFixture]
    public class EditRecipePageViewModelTest : ViewModelBaseTest
    {
        private EditRecipePageViewModel EditRecipePageViewModel { get; set; }
        private Recipe SelectedRecipe { get; set; }
        private readonly string[] HiddenPropertiesEn = new string[]
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
        private readonly string[] HiddenPropertiesNl = new string[]
        {
            "Voorbereidingstijd",
            "Rusttijd",
            "Methode",
            "Aantal Porties",
            "Recept",
            "Beschrijving",
            "Benodigdheden",
            "Serveertips"
        };

        [SetUp]
        public void SetUp()
        {
            EditRecipePageViewModel = new EditRecipePageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, AuthServiceMock.Object, DatabaseServiceMock.Object);
            SelectedRecipe = Fixture.Build<Recipe>().Without(i => i.Id).Do(i => i.Id = Fixture.Create<Guid>()).Create();
        }

        [Test]
        public void PageName_IfCreateMode_ShouldIndicateNewRecipe()
        {
            // Arrange
            var expectedPageName = "Nieuw recept";

            // Act
            EditRecipePageViewModel.CreateMode = true;

            // Assert
            Assert.IsTrue(EditRecipePageViewModel.CreateMode);
            Assert.AreEqual(expectedPageName, EditRecipePageViewModel.PageName);
        }

        [Test]
        public void PageName_IfNotCreateMode_ShouldIndicateEditRecipe()
        {
            // Arrange
            var expectedPageName = "Recept wijzigen";

            // Act
            EditRecipePageViewModel.CreateMode = false;

            // Assert
            Assert.IsFalse(EditRecipePageViewModel.CreateMode);
            Assert.AreEqual(expectedPageName, EditRecipePageViewModel.PageName);
        }

        [TestCase(true, TestName = "OnCancelCommand_WhenConfirmed_ShouldNavigateBack")]
        [TestCase(false, TestName = "OnCancelCommand_WhenCanceled_ShouldNotNavigateBack")]
        public void OnCancelCommand_Response(bool confirmAlert)
        {
            // Arrange
            var alertTitle = "Pas op!";
            var alertMessage = "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?";
            var alertAcceptButton = "Ja";
            var alertCancelButton = "Nee";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => confirmAlert));
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            EditRecipePageViewModel.OnCancelCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Confirmation alert for cancelling editting the recipe not called exactly once.");
            if (confirmAlert) NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
            else NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Never, "Function INavigationService.GoBackAsync called atleast once.");
        }

        [Test]
        public void OnSubmitCommand_InCreateMode_ShouldAddRecipeAndNavigateToItsPage()
        {
            // Arrange
            var alertTitle = "Pas op!";
            var alertMessage = "Deze actie kan niet ongedaan worden.";
            var alertAcceptButton = "Opslaan";
            var alertCancelButton = "Annuleer";
            var displayRecipePageName = $"../{nameof(DisplayRecipePage)}";
            var expectedParameters = new NavigationParameters
            {
                { "SelectedRecipe", SelectedRecipe.Id }
            };

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => false));
            DatabaseServiceMock.Setup(databaseService => databaseService.SaveRecipeAsync(SelectedRecipe)).Returns(Task.Run(() => true));
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(displayRecipePageName, expectedParameters)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            EditRecipePageViewModel.CreateMode = true;
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.OnSubmitCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Never, "Confirmation alert for editting existing recipe called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SaveRecipeAsync(SelectedRecipe), Times.Once, "Function IDatabaseService.SaveRecipeAsync not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(displayRecipePageName, expectedParameters), Times.Once, "Function to navigate to display recipe page for the created recipe not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Never, "Function INavigationService.GoBackAsync called atleast once.");
        }

        [Test]
        public void OnSubmitCommand_NotInCreateModeWhenConfirmed_ShouldSaveRecipeAndNavigateBack()
        {
            // Arrange
            var alertTitle = "Pas op!";
            var alertMessage = "Deze actie kan niet ongedaan worden.";
            var alertAcceptButton = "Opslaan";
            var alertCancelButton = "Annuleer";
            var displayRecipePageName = $"../{nameof(DisplayRecipePage)}";
            var expectedParameters = new NavigationParameters
            {
                { "SelectedRecipe", SelectedRecipe.Id }
            };

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => true));
            DatabaseServiceMock.Setup(databaseService => databaseService.SaveRecipeAsync(SelectedRecipe)).Returns(Task.Run(() => true));
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(displayRecipePageName, expectedParameters)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            EditRecipePageViewModel.CreateMode = false;
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.OnSubmitCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Confirmation alert for editting existing recipe not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SaveRecipeAsync(SelectedRecipe), Times.Once, "Function IDatabaseService.SaveRecipeAsync not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(displayRecipePageName, expectedParameters), Times.Never, "Function to navigate to display recipe page for the created recipe called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.GoBackAsync not called exactly once.");
        }

        [Test]
        public void OnSubmitCommand_NotInCreateModeWhenCanceled_ShouldDoNothing()
        {
            // Arrange
            var alertTitle = "Pas op!";
            var alertMessage = "Deze actie kan niet ongedaan worden.";
            var alertAcceptButton = "Opslaan";
            var alertCancelButton = "Annuleer";
            var displayRecipePageName = $"../{nameof(DisplayRecipePage)}";
            var expectedParameters = new NavigationParameters
            {
                { "SelectedRecipe", SelectedRecipe.Id }
            };

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => false));
            DatabaseServiceMock.Setup(databaseService => databaseService.SaveRecipeAsync(SelectedRecipe)).Returns(Task.Run(() => true));
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(displayRecipePageName, expectedParameters)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            EditRecipePageViewModel.CreateMode = false;
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.OnSubmitCommand?.Execute();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Confirmation alert for editting existing recipe not called exactly once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.SaveRecipeAsync(SelectedRecipe), Times.Never, "Function IDatabaseService.SaveRecipeAsync called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(displayRecipePageName, expectedParameters), Times.Never, "Function to navigate to display recipe page for the created recipe called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Never, "Function INavigationService.GoBackAsync called atleast once.");
        }

        [TestCase("PrepTime")]
        [TestCase("RestTime")]
        [TestCase("Method")]
        [TestCase("NumPortions")]
        [TestCase("OriginalRecipe")]
        [TestCase("Description")]
        [TestCase("Requirements")]
        [TestCase("ServeTips")]
        public async Task AddProperty_WhenPropertySelected_ShouldAddToShowPropertiesList(string propertyName)
        {
            // Arrange
            var actionSheetTitle = "Voeg nieuw veld toe";
            var actionSheetCancelButton = "Annuleer";
            var propertyNl = SelectedRecipe.EnToNlTranslation(propertyName);
            
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayActionSheetAsync(actionSheetTitle, actionSheetCancelButton, null, HiddenPropertiesNl)).Returns(Task.Run(() => propertyNl));

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            await EditRecipePageViewModel.AddProperty();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayActionSheetAsync(actionSheetTitle, actionSheetCancelButton, null, HiddenPropertiesNl), Times.Once, "Action Sheet to select which property to add was not called.");
            Assert.Contains(propertyName, EditRecipePageViewModel.ShowProperties, $"Property {propertyName} not added to attribute EditRecipePageViewModel.ShowProperties.");
        }

        [Test]
        public async Task AddProperty_WhenCancelButtonSelected_ShouldNotAddToShowPropertiesList()
        {
            // Arrange
            var actionSheetTitle = "Voeg nieuw veld toe";
            var actionSheetCancelButton = "Annuleer";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayActionSheetAsync(actionSheetTitle, actionSheetCancelButton, null, HiddenPropertiesNl)).Returns(Task.Run(() => actionSheetCancelButton));

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            await EditRecipePageViewModel.AddProperty();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayActionSheetAsync(actionSheetTitle, actionSheetCancelButton, null, HiddenPropertiesNl), Times.Once, "Action Sheet to select which property to add was not called.");
            Assert.IsEmpty(EditRecipePageViewModel.ShowProperties, "A property was added to attribute EditRecipePageViewModel.ShowProperties.");
        }

        [TestCase(null, TestName = "AddProperty_WithNullValue_ShouldNotAddToShowPropertiesList")]
        [TestCase("", TestName = "AddProperty_WithEmptyString_ShouldNotAddToShowPropertiesList")]
        [TestCase("Name", TestName = "AddProperty_WithInvalidProperty_ShouldNotAddToShowPropertiesList")]
        public async Task AddProperty_WithInvalidProperty_ShouldNotAddToShowPropertiesList(string propertyName = null)
        {
            // Arrange
            var actionSheetTitle = "Voeg nieuw veld toe";
            var actionSheetCancelButton = "Annuleer";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayActionSheetAsync(actionSheetTitle, actionSheetCancelButton, null, HiddenPropertiesNl)).Returns(Task.Run(() => "Name"));

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            await EditRecipePageViewModel.AddProperty();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayActionSheetAsync(actionSheetTitle, actionSheetCancelButton, null, HiddenPropertiesNl), Times.Once, "Action Sheet to select which property to add was not called.");
            Assert.IsEmpty(EditRecipePageViewModel.ShowProperties, "A property was added to attribute EditRecipePageViewModel.ShowProperties.");
        }

        [TestCase("PrepTime")]
        [TestCase("RestTime")]
        [TestCase("Method")]
        [TestCase("NumPortions")]
        [TestCase("OriginalRecipe")]
        [TestCase("Description")]
        [TestCase("Requirements")]
        [TestCase("ServeTips")]
        public async Task OnRemovePropertyCommand_WhenConfirmed_ShouldRemoveProperty(string propertyName)
        {
            // Arrange
            var propertyNl = SelectedRecipe.EnToNlTranslation(propertyName);
            var alertTitle = "Pas op!";
            var alertMessage = $"Weet u zeker dat u het veld {propertyNl} wilt verwijderen?";
            var alertAcceptButton = "Ja";
            var alertCancelButton = "Nee";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => true));

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.ShowProperties.Add(propertyName);
            Assert.Contains(propertyName, EditRecipePageViewModel.ShowProperties, $"Property {propertyName} not added to attribute EditRecipePageViewModel.ShowProperties prior to the test");
            await EditRecipePageViewModel.RemoveProperty(propertyName);

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Alert for confirmation of removing property not called exactly once.");
            Assert.That(!EditRecipePageViewModel.ShowProperties.Contains(propertyName), $"Property {propertyName} not removed from attribute EditRecipePageViewModel.ShowProperties.");
        }

        [Test]
        public async Task OnRemovePropertyCommand_WhenCanceled_ShouldNotRemoveProperty()
        {
            // Arrange
            var propertyName = "PrepTime";
            var propertyNl = SelectedRecipe.EnToNlTranslation(propertyName);
            var alertTitle = "Pas op!";
            var alertMessage = $"Weet u zeker dat u het veld {propertyNl} wilt verwijderen?";
            var alertAcceptButton = "Ja";
            var alertCancelButton = "Nee";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton)).Returns(Task.Run(() => false));

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.ShowProperties.Add(propertyName);
            Assert.Contains(propertyName, EditRecipePageViewModel.ShowProperties, $"Property {propertyName} not added to attribute EditRecipePageViewModel.ShowProperties prior to the test");
            await EditRecipePageViewModel.RemoveProperty("PrepTime");

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertTitle, alertMessage, alertAcceptButton, alertCancelButton), Times.Once, "Alert for confirmation of removing property not called exactly once.");
            Assert.Contains(propertyName, EditRecipePageViewModel.ShowProperties, $"Property {propertyName} removed from attribute EditRecipePageViewModel.ShowProperties.");
        }

        // TODO: PopulatePage()
        [Test]
        public void PopulatePage_WithRecipePopulatedWithAllProperties_ShouldShowAllProperties()
        {
            // Arrange

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.PopulatePage();

            // Assert
            foreach (var property in HiddenPropertiesEn) Assert.Contains(property, EditRecipePageViewModel.ShowProperties, $"Property {property} not added to attribute EditRecipePageViewModel.ShowProperties.");
        }

        [Test]
        public void PopulatePage_WithRecipePopulatedWithAProperty_ShouldShowOneProperty()
        {
            // Arrange
            SelectedRecipe = new Recipe { PrepTime = new TimeSpan(1, 30, 0) };

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.PopulatePage();

            // Assert
            Assert.That(EditRecipePageViewModel.ShowProperties.Count == 1, "Size of property EditRecipePageViewModel.ShowProperties not exactly 1.");
            Assert.Contains("PrepTime", EditRecipePageViewModel.ShowProperties, $"Property PrepTime not added to attribute EditRecipePageViewModel.ShowProperties.");
        }

        [Test]
        public void PopulatePage_WithNewRecipe_ShouldShowNoProperties()
        {
            // Arrange
            SelectedRecipe = new Recipe();

            // Act
            EditRecipePageViewModel.Recipe = SelectedRecipe;
            EditRecipePageViewModel.PopulatePage();

            // Assert
            Assert.IsEmpty(EditRecipePageViewModel.ShowProperties, "Attribute EditRecipePageViewModel.ShowProperties is not empty.");
        }

        // TODO: OnNavigatedTo(parameters)
        [Test]
        public void OnNavigatedTo_WithCorrectParameters_ShouldSetSelectedRecipe()
        {
            // Arrange
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", SelectedRecipe.Id }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));

            // Act
            EditRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Once, "Function IDatabaseService.GetRecipeAsync for selected recipe not called exactly once.");
            Assert.AreEqual(SelectedRecipe, EditRecipePageViewModel.Recipe, "Attribute EditRecipePageViewModel.Recipe is not the expected value.");
            Assert.IsFalse(EditRecipePageViewModel.CreateMode, "Attribute EditRecipePageViewModel.CreateMode is true, but should be false.");
        }

        [Test]
        public void OnNavigatedTo_WithoutParameters_ShouldCreateNewRecipe()
        {
            // Arrange
            var expectedRecipe = new Recipe();

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(Guid.Empty)).Verifiable();

            // Act
            EditRecipePageViewModel.OnNavigatedTo(null);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(Guid.Empty), Times.Never, "Function IDatabaseService.GetRecipeASync for selected recipe called atleast once.");
            Assert.AreEqual(expectedRecipe, EditRecipePageViewModel.Recipe, "Attribute EditRecipePageViewModel.Recipe is not the expected value.");
            Assert.IsTrue(EditRecipePageViewModel.CreateMode, "Attribute EditRecipePageViewModel.CreateMode is false, but should be true.");
        }

        [Test]
        public void OnNavigatedTo_WithIncorrectParameterName_ShouldCreateNewRecipe()
        {
            // Arrange
            var expectedRecipe = new Recipe();
            var parameters = new NavigationParameters
            {
                { "SelectedRecipeId", SelectedRecipe.Id }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));

            // Act
            EditRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.GetRecipeASync for selected recipe called atleast once.");
            Assert.AreEqual(expectedRecipe, EditRecipePageViewModel.Recipe, "Attribute EditRecipePageViewModel.Recipe is not the expected value.");
            Assert.IsTrue(EditRecipePageViewModel.CreateMode, "Attribute EditRecipePageViewModel.CreateMode is false, but should be true.");
        }

        [Test]
        public void OnNavigatedTo_WithIncorrectId_ShouldCreateNewRecipe()
        {
            // Arrange
            var expectedRecipe = new Recipe();
            var id = Guid.NewGuid();
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", id }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id)).Returns(Task.Run(() => SelectedRecipe));
            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(id)).Returns(() => null);

            // Act
            EditRecipePageViewModel.OnNavigatedTo(parameters);


            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(SelectedRecipe.Id), Times.Never, "Function IDatabaseService.GetRecipeASync for selected recipe called atleast once.");
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(id), Times.Once, "Function IDatabaseService.GetRecipeASync for incorrect recipe id not called exactly once.");
            Assert.AreEqual(expectedRecipe, EditRecipePageViewModel.Recipe, "Attribute EditRecipePageViewModel.Recipe is not the expected value.");
            Assert.IsTrue(EditRecipePageViewModel.CreateMode, "Attribute EditRecipePageViewModel.CreateMode is false, but should be true.");
        }

        [Test]
        public void OnNavigatedTo_WithInvalidDataTypeParameters_ShouldCreateNewRecipe()
        {
            // Arrange
            var expectedRecipe = new Recipe();
            var id = Fixture.Create<double>();
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", id }
            };

            DatabaseServiceMock.Setup(databaseService => databaseService.GetRecipeAsync(Guid.Empty)).Returns(() => null);

            // Act
            EditRecipePageViewModel.OnNavigatedTo(parameters);

            // Assert
            DatabaseServiceMock.Verify(databaseService => databaseService.GetRecipeAsync(Guid.Empty), Times.Never, "Function IDatabaseService.GetRecipeASync for selected recipe called atleast once.");
            Assert.AreEqual(expectedRecipe, EditRecipePageViewModel.Recipe, "Attribute EditRecipePageViewModel.Recipe is not the expected value.");
            Assert.IsTrue(EditRecipePageViewModel.CreateMode, "Attribute EditRecipePageViewModel.CreateMode is false, but should be true.");
        }
    }
}
