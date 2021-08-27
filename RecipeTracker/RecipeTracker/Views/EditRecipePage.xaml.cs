using Prism.Navigation;
using Xamarin.Forms;

namespace RecipeTracker.Views
{
    /// <summary>
    /// View <c>EditRecipePage</c> can be used to create new recipes or edit existing ones.
    /// </summary>
    public partial class EditRecipePage : ContentPage
    {
        private readonly INavigationService NavigationService;

        public EditRecipePage(INavigationService navigationService)
        {
            InitializeComponent();
            NavigationService = navigationService;
        }

        /// <summary>
        /// Shows an alert to close the page, if the user confirms, it will return to the previous page.
        /// </summary>
        private async void ClosePage()
        {
            var response = await DisplayAlert("Pas op!", "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?", "Ja", "Nee");
            if (response) await NavigationService.GoBackAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            // Call method 'ClosePage' and prevent the base OnBackButtonPressed from executing.
            ClosePage();
            return true;
        }
    }
}
