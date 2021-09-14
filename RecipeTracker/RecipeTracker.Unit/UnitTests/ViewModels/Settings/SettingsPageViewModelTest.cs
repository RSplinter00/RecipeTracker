using Moq;
using NUnit.Framework;
using RecipeTracker.ViewModels.Settings;

namespace RecipeTracker.Unit.UnitTests.ViewModels.Settings
{
    /// <summary>
    /// Class <c>SettingsPageViewModelTest</c> contains unit tests for class <seealso cref="RecipeTracker.ViewModels.Settings.SettingsPageViewModel"/>.
    /// </summary>
    [TestFixture]
    public class SettingsPageViewModelTest : ViewModelBaseTest
    {
        private SettingsPageViewModel SettingsPageViewModel { get; set; }

        [SetUp]
        public void SetUp()
        {
            SettingsPageViewModel = new SettingsPageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, AuthServiceMock.Object, DatabaseServiceMock.Object);
        }

        [TestCase("Account", "AccountSettingsPage")]
        [TestCase("Probleem melden", "ReportIssueSettingsPage")]
        public void OnSelectionChanged_WithSelectedItem_ShouldNavigateToSelectedPage(string selectedItem, string expectedPageName)
        {
            // Arrange
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync(expectedPageName)).Verifiable();

            // Act
            SettingsPageViewModel.SelectedItem = selectedItem;
            SettingsPageViewModel.OnSelectionChanged();

            // Assert
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync(expectedPageName), Times.Once, $"Method OnSelectionChanged did not navigate user to {selectedItem}SettingsPage.");
            Assert.IsNull(SettingsPageViewModel.SelectedItem, "Attribute SettingsPageViewModel.SelectedItem was not reset upon navigating.");
        }

        [TestCase(null, TestName = "OnSelectionChanged_WithoutSelectedItem_ShouldDoNothing")]
        [TestCase("", TestName = "OnSelectionChanged_WithEmptyString_ShouldDoNothing")]
        [TestCase(" ", TestName = "OnSelectionChanged_WithWhiteSpace_ShouldDoNothing")]
        public void OnSelectionChanged_WithoutSelectedItem_ShouldDoNothing(string selectedItem)
        {
            // Arrange
            NavigationServiceMock.Setup(navigationService => navigationService.NavigateAsync($"{selectedItem}SettingsPage")).Verifiable();

            // Act
            SettingsPageViewModel.SelectedItem = selectedItem;
            SettingsPageViewModel.OnSelectionChanged();

            // Assert
            NavigationServiceMock.Verify(navigationService => navigationService.NavigateAsync($"{selectedItem}SettingsPage"), Times.Never, $"Method OnSelectionChanged navigated user to {selectedItem}SettingsPage.");
        }
    }
}
