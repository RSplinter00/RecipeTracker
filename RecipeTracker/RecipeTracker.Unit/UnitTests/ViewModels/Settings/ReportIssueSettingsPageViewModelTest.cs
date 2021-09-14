using Moq;
using NUnit.Framework;
using RecipeTracker.Models;
using RecipeTracker.Services;
using RecipeTracker.ViewModels.Settings;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using AutoFixture;

namespace RecipeTracker.Unit.UnitTests.ViewModels.Settings
{
    /// <summary>
    /// Class <c>DisplayRecipePageViewModelTest</c> contains unit tests for class <seealso cref="RecipeTracker.ViewModels.DisplayRecipePageViewModel"/>.
    /// </summary>[TestFixture]
    public class ReportIssueSettingsPageViewModelTest : ViewModelBaseTest
    {
        private ReportIssueSettingsPageViewModel ReportIssueSettingsPageViewModel { get; set; }
        private Mock<IReportingService> ReportingServiceMock { get; }

        public ReportIssueSettingsPageViewModelTest()
        {
            ReportingServiceMock = MockRepository.Create<IReportingService>();
        }

        [SetUp]
        public void SetUp()
        {
            ReportIssueSettingsPageViewModel = new ReportIssueSettingsPageViewModel(NavigationServiceMock.Object, PageDialogServiceMock.Object, AuthServiceMock.Object, DatabaseServiceMock.Object, ReportingServiceMock.Object);
        }

        [Test]
        public void OnCancelPressedCommand_ShouldNavigateToPreviousPage()
        {
            // Arrange
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            ReportIssueSettingsPageViewModel.OnCancelPressedCommand?.Execute();

            // Assert
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.OnCancelPressed not called exactly once.");
        }

        [TestCase(null, null, true, TestName = "ReportIssue_WhenIssueIsNull_ShouldDisplayAlert")]
        [TestCase(null, null, TestName = "ReportIssue_WhenValuesAreNull_ShouldDisplayAlert")]
        [TestCase("", "", TestName = "ReportIssue_WhenValuesAreEmpty_ShouldDisplayAlert")]
        [TestCase(" ", " ", TestName = "ReportIssue_WhenValuesAreWhiteSpace_ShouldDisplayAlert")]
        [TestCase("User", null, TestName = "ReportIssue_WhenDescriptionIsNull_ShouldDisplayAlert")]
        [TestCase(null, "Description", TestName = "ReportIssue_WhenUserIsNull_ShouldDisplayAlert")]
        public async Task ReportIssue_WhenIssueIsInvalid_ShouldDisplayAlert(string user, string description, bool issueIsNull = false)
        {
            // Arrange
            Issue issue = null;
            if (!issueIsNull)
            {
                issue = new Issue
                {
                    User = user,
                    Description = description
                };
            }

            var alertErrorTitle = "Let op!";
            var alertErrorMessage = "Het emailadres of de beschrijving is niet ingevuld!";
            var alertErrorCancelButton = "Ok";
            var alertConfirmationTitle = "Bedankt!";
            var alertConfirmationMessage = "Bedankt voor het melden van het probleem!";
            var alertConfirmationCancelButton = "Geen probleem";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertErrorTitle, alertErrorMessage, alertErrorCancelButton)).Verifiable();
            ReportingServiceMock.Setup(reportingService => reportingService.ReportIssue(issue)).Returns(Task.Run(() => false));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertConfirmationTitle, alertConfirmationMessage, alertConfirmationCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            ReportIssueSettingsPageViewModel.Issue = null;
            await ReportIssueSettingsPageViewModel.ReportIssue();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertErrorTitle, alertErrorMessage, alertErrorCancelButton), Times.Once, "Alert for invalid issue not called exactly once.");
            ReportingServiceMock.Verify(reportingService => reportingService.ReportIssue(issue), Times.Never, "Function ReportingService.ReportIssue called atleast once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertConfirmationTitle, alertConfirmationMessage, alertConfirmationCancelButton), Times.Never, "Alert for succesfully reporting an issue called atleast once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Never, "Function INavigationService.OnCancelPressed called atleast once.");
        }

        [Test]
        public async Task ReportIssue_WhenIssueIsValid_ShouldReportTheIssue()
        {
            // Arrange
            var issue = Fixture.Create<Issue>();

            var alertErrorTitle = "Let op!";
            var alertErrorMessage = "Het emailadres of de beschrijving is niet ingevuld!";
            var alertErrorCancelButton = "Ok";
            var alertConfirmationTitle = "Bedankt!";
            var alertConfirmationMessage = "Bedankt voor het melden van het probleem!";
            var alertConfirmationCancelButton = "Geen probleem";

            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertErrorTitle, alertErrorMessage, alertErrorCancelButton)).Verifiable();
            ReportingServiceMock.Setup(reportingService => reportingService.ReportIssue(issue)).Returns(Task.Run(() => true));
            PageDialogServiceMock.Setup(dialogService => dialogService.DisplayAlertAsync(alertConfirmationTitle, alertConfirmationMessage, alertConfirmationCancelButton)).Verifiable();
            NavigationServiceMock.Setup(navigationService => navigationService.GoBackAsync()).Verifiable();

            // Act
            ReportIssueSettingsPageViewModel.Issue = issue;
            await ReportIssueSettingsPageViewModel.ReportIssue();

            // Assert
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertErrorTitle, alertErrorMessage, alertErrorCancelButton), Times.Never, "Alert for invalid issue called atleast once.");
            ReportingServiceMock.Verify(reportingService => reportingService.ReportIssue(issue), Times.Once, "Function ReportingService.ReportIssue not called exactly once.");
            PageDialogServiceMock.Verify(dialogService => dialogService.DisplayAlertAsync(alertConfirmationTitle, alertConfirmationMessage, alertConfirmationCancelButton), Times.Once, "Alert for succesfully reporting an issue not called exactly once.");
            NavigationServiceMock.Verify(navigationService => navigationService.GoBackAsync(), Times.Once, "Function INavigationService.OnCancelPressed not called exactly once.");
        }

        [Test]
        public void OnNavigatedTo_ShouldCreateNewIssue()
        {
            // Arrange


            // Act
            ReportIssueSettingsPageViewModel.OnNavigatedTo(null);

            // Assert
            Assert.NotNull(ReportIssueSettingsPageViewModel.Issue, "Attribute ReportIssueSettingsPageViewModel.Issue was not set.");
        }
    }
}
