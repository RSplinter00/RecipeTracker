using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using RecipeTracker.Models;
using RecipeTracker.Services;
using System;
using System.Threading.Tasks;

namespace RecipeTracker.ViewModels.Settings
{
    /// <summary>
    /// Class <c>ReportIssueSettingsPageViewModel</c> is the view model for the ReportIssueSettingsPageViewModel.
    /// This page allows the user to report issues.
    /// </summary>
    public class ReportIssueSettingsPageViewModel : ViewModelBase
    {
        private readonly IReportingService ReportingService;

        public DelegateCommand OnSubmitCommand { get; }
        public DelegateCommand OnCancelPressedCommand { get; }

        private Issue issue;
        public Issue Issue
        {
            get => issue;
            set => SetProperty(ref issue, value);
        }

        public ReportIssueSettingsPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService, IAuthenticationService authService, IDatabaseService databaseService, IReportingService reportingService)
            : base(navigationService, pageDialogService, authService, databaseService)
        {
            ReportingService = reportingService;

            OnSubmitCommand = new DelegateCommand(OnSubmit);
            OnCancelPressedCommand = new DelegateCommand(GoBackAsync);
        }

        /// <summary>
        /// Executes task <seealso cref="ReportIssue"/>.
        /// </summary>
        private async void OnSubmit()
        {
            await ReportIssue();
        }

        /// <summary>
        /// Reports the user's issue, if valid.
        /// </summary>
        public async Task ReportIssue()
        {
            if (Issue == null || !Issue.IsValid)
            {
                // Display an alert if the issue is invalid
                await DialogService.DisplayAlertAsync("Let op!", "Het emailadres of de beschrijving is niet ingevuld!", "Ok");
            }
            else
            {
                // If the issue is valid, report it and return to the previous page.
                var response = await ReportingService.ReportIssue(Issue);

                if (response)
                {
                    await DialogService.DisplayAlertAsync("Bedankt!", "Bedankt voor het melden van het probleem!", "Geen probleem");
                    GoBackAsync();
                }
            }
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Issue = new Issue();
        }
    }
}
