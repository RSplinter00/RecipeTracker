using Firebase.Database;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using RecipeTracker.Models;
using System;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    public interface IReportingService
    {
        Task<bool> ReportIssue(Issue issue);
    }
    public class ReportingService : IReportingService
    {
        private readonly string ChildName = $"issues/{ GoogleAuthenticationService.UserID }";

        private readonly GoogleAuthenticationService AuthService;
        private FirebaseClient Firebase => AuthService.Firebase;

        public ReportingService()
        {
            AuthService = new GoogleAuthenticationService();
        }

        public async Task<bool> ReportIssue(Issue issue)
        {
            try
            {
                await Firebase
                    .Child(ChildName)
                    .PostAsync(JsonConvert.SerializeObject(issue));

                return true;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return false;
            }
        }
    }
}
