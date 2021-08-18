using Prism.Navigation;
using Xamarin.Forms;

namespace RecipeTracker.Views
{
    public partial class EditRecipePage : ContentPage
    {
        private readonly INavigationService NavigationService;

        public EditRecipePage(INavigationService navigationService)
        {
            InitializeComponent();
            NavigationService = navigationService;
        }

        private async void ClosePage()
        {
            var response = await DisplayAlert("Pas op!", "Niet opgeslagen gegevens worden verwijderd! Weer u zeker dat u terug wilt gaan?", "Ja", "Nee");
            if (response) await NavigationService.GoBackAsync();
        }

        protected override bool OnBackButtonPressed()
        {
            var height = frame.Height;
            ClosePage();
            return true;
        }
    }
}
