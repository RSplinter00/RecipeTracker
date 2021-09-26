using Firebase.Auth;
using Firebase.Database;
using Microsoft.AppCenter.Crashes;
using Newtonsoft.Json;
using Plugin.GoogleClient;
using Plugin.GoogleClient.Shared;
using System;
using System.Diagnostics;
using System.Threading.Tasks;

namespace RecipeTracker.Services
{
    /// <summary>
    /// Class <c>GoogleAuthenticationService</c> deals with authenticating the user, using their Google account.
    /// </summary>
    public class GoogleAuthenticationService : IAuthenticationService
    {
        private readonly IGoogleClientManager GoogleService = CrossGoogleClient.Current;
        public static string UserID { get; private set; } = "UnknownUser";

        private static FirebaseClient firebase;
        public FirebaseClient Firebase
        {
            get
            {
                if (firebase == null) SetupFirebaseClientAsync().Wait();

                return firebase;
            }
            private set => firebase = value;
        }

        /// <summary>
        /// Sets up the Firebase client, to provide authentication when the user is logged in.
        /// </summary>
        private async Task SetupFirebaseClientAsync()
        {
            try
            {
                FirebaseOptions options = null;

                if (!string.IsNullOrEmpty(GoogleService.AccessToken))
                {
                    // If the user has received an access token, retrieve the users Firebase token.
                    var authProvider = new FirebaseAuthProvider(new FirebaseConfig(AppSettingsManager.Settings["AndroidAPIKey"]));
                    var auth = await authProvider.SignInWithOAuthAsync(FirebaseAuthType.Google, GoogleService.AccessToken);

                    // Creates the firebase options, so the user can be authenticated by Firebase.
                    UserID = auth.User.LocalId;
                    options = new FirebaseOptions
                    {
                        AuthTokenAsyncFactory = () => Task.FromResult(auth.FirebaseToken)
                    };
                }
                else UserID = "UnknownUser";

                Firebase = new FirebaseClient(AppSettingsManager.Settings["FirebaseDatabasePath"], options);
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }
        }

        /// <summary>
        /// Retrieves the user that is logged in from the Google client manager.
        /// </summary>
        /// <returns>The current user.</returns>
        public GoogleUser GetUser() => GoogleService.CurrentUser;

        /// <summary>
        /// Logs the user in with their Google account.
        /// </summary>
        /// <returns>The status of the login attempt.</returns>
        public async Task<GoogleActionStatus> LoginAsync()
        {
            try
            {
                GoogleActionStatus status = GoogleActionStatus.Unauthorized;

                // Create the function to login the user.
                EventHandler<GoogleClientResultEventArgs<GoogleUser>> userLoginDelegate = null;
                userLoginDelegate = (object sender, GoogleClientResultEventArgs<GoogleUser> e) =>
                {
                    status = e.Status;

#if DEBUG
                    var googleUserString = JsonConvert.SerializeObject(e.Data);
                    Debug.WriteLine($"Google Login attempt by {googleUserString}: {e.Status}");
#endif

                    GoogleService.OnLogin -= userLoginDelegate;
                };

                GoogleService.OnLogin += userLoginDelegate;
                await GoogleService.LoginAsync();

                // If the login was successful, setup the Firebase client.
                if (status == GoogleActionStatus.Completed) await SetupFirebaseClientAsync();

                return status;
            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
                return GoogleActionStatus.Error;
            }
        }

        /// <summary>
        /// Logs the user out with their Google account.
        /// </summary>
        /// <returns>If the user was successfully logged out.</returns>
        public async Task<bool> LogoutAsync()
        {
            try
            {
                GoogleService.Logout();
                // Reset the Firebase Client, to deny the app from accessing Firebase resources.
                await SetupFirebaseClientAsync();

            }
            catch (Exception e)
            {
                Crashes.TrackError(e);
            }

            return !GoogleService.IsLoggedIn;
        }
    }
}
