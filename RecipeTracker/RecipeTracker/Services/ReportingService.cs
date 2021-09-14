using Firebase.Database;
using Newtonsoft.Json;
using RecipeTracker.Models;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    public interface IReportingService
    {
        Task<bool> ReportIssue(Issue issue);
    }
    public class ReportingService : IReportingService
    {
        private string ChildName { get => $"issues/{ GoogleAuthenticationService.UserID }"; }

        private readonly GoogleAuthenticationService AuthService;
        private FirebaseClient Firebase { get => AuthService.Firebase; }

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
                Debug.WriteLine(e.Message);
                return false;
            }
        }
    }
}
