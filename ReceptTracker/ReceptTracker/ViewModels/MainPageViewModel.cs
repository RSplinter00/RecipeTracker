using Prism.Commands;
using Prism.Navigation;
using Prism.Services;
using ReceptTracker.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace ReceptTracker.ViewModels
{
    public class MainPageViewModel : ViewModelBase
    {
        public DelegateCommand<Recipe> RecipeSelectedCommand { get; }

        private List<Recipe> recipes;
        public List<Recipe> Recipes 
        { 
            get => recipes;
            private set => SetProperty(ref recipes, value);
        }

        public MainPageViewModel(INavigationService navigationService, IPageDialogService pageDialogService) : base(navigationService, pageDialogService)
        {
            RecipeSelectedCommand = new DelegateCommand<Recipe>(RecipeSelected);
        }

        public void RecipeSelected(Recipe selectedRecipe)
        {
            var parameters = new NavigationParameters
            {
                { "SelectedRecipe", selectedRecipe }
            };

            NavigateToPageAsync("DisplayRecipePage", parameters);
        }

        public override void OnNavigatedTo(INavigationParameters parameters)
        {
            Recipes = new List<Recipe>()
            {
                new Recipe("Pulled Pork", "Pork", new TimeSpan(8, 30, 0)),
                new Recipe("Recipe2", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe3", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe4", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe5", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe6", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe7", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe8", "Chicken", new TimeSpan(0, 5, 0)),
                new Recipe("Recipe9", "Pork", new TimeSpan(2, 25, 12)),
                new Recipe("Recipe10", "Beef", new TimeSpan(1, 30, 0))
            };

            Recipes[0].Ingredients = new List<string>
            {
                "1 Procureur, zo'n 2,5kg",
                "PP injectiemix",
                "BBQ kruiden voor pulled pork",
                "PP Glaze",
                "PP Eindsaus"
            };
            Recipes[0].Requirements = new List<string>
            {
                "Kerntemperatuurmeter",
                "Plasticfolie",
                "Aluminiumfolie"
            };
            Recipes[0].Preparation = "1. Snij het grootste deel van het overtollige vet van de procureur. Als er nog een beetje op zit is dat niet erg, als de grote dikke stukken maar weg zijn.\r\n" +
                "2. Bestrooi rijkelijk met de barbecuekruiden en pak in plasticfolie in. Laat 4 tot 8 uur in de koelkast liggen.\n" +
                "3. Maak je barbecue klaar voor indirect  barbecueën op 121°C. Als je niet weet hoe je dat moet doen dan laat ik je dat hier stap voor stap zien.\n" +
                "4. Haal het vlees uit het plastic en plaats het in de barbecue weg van het vuur. Plaats de voeler van de kernthermometer en sluit de deksel." +
                "5. Na 5 uur of als de kerntemperatuur 74°C is (wat het eerste gebeurt), smeer je het vlees in met opgewarmde PP Glaze." +
                "6. Laat nog twee uur liggen, je hebt nu een mooie bark.\n" +
                "7. Haal het vlees uit de barbecue en pak het strak in twee lagen aluminiumfolie in." +
                "8. Plaats terug in de barbecue en laat liggen totdat de kerntemperatuur 88°C is.\n" +
                "Prik dan met de voeler van de thermometer of een metalen satéprikker hier en daar in het vlees om te voelen of het gaar is.\n" +
                "De voeler of prikker moet er zonder al te veel weerstand in gaan.Denk aan boterzacht.\n" +
                "Als dat zo is dan mag het vlees van de barbecue.Is dat niet zo, kijk dan na 15 minuten nog een keer. Soms is het vlees pas bij 96°C gaar.\n" +
                "9. Haal de procureur van de barbecue, haal hem uit de folie en laat het vlees een kwartier uitdampen. Dat voorkomt dat het verder doorgaart.\n" +
                "10. Bewaar het vocht dat uit de folie komt komt voor de PP Eindsaus.\n" +
                "11. Pak het vlees opnieuw in aluminiumfolie in en laat het zo een uur rusten.\n" +
                "12. Trek het vlees uit elkaar en vermeng met zoveel PP Eindsaus dat het goed op smaak en mooi sappig is.";
            Recipes[0].Method = "Indirect";
        }
    }
}
