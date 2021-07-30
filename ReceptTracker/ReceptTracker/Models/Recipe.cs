using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace ReceptTracker.Models
{
    public class Recipe
    {
        private readonly Dictionary<string, string> EnNlTranslations = new Dictionary<string, string>()
        {
            { "Name", "Naam" },
            { "Category", "Categorie" },
            { "PrepTime", "Voorbereidingstijd" },
            { "CookingTime", "Bereidingstijd" },
            { "RestTime", "Rusttijd" },
            { "TotalDuration", "Totale tijd" },
            { "Method", "Methode" },
            { "NumPortions", "Aantal Porties" },
            { "OriginalRecipe", "Recept" },
            { "Description", "Beschrijving" },
            { "Ingredients", "Ingrediënten" },
            { "Requirements", "Benodigdheden" },
            { "Steps", "Bereidingswijze" },
            { "ServeTips", "Serveertips" }
        };
        
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public Guid Id { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        // Time it takes to prepare the recipe
        public TimeSpan PrepTime { get; set; }
        // Time it takes to cook the meal
        public TimeSpan CookingTime { get; set; }
        // Time it takes for the meat to rest
        public TimeSpan RestTime { get; set; }
        // Sum of all timespans
        [Ignore]
        public TimeSpan TotalDuration
        {
            get => PrepTime + CookingTime + RestTime;
        }
        public string Method { get; set; }
        public int NumPortions { get; set; }
        public string OriginalRecipe { get; set; }
        public string Description { get; set; }
        public string Ingredients { get; set; }
        public string Requirements { get; set; }
        public string Steps { get; set; }
        public string ServeTips { get; set; }

        public Recipe()
        {
        }

        public string EnToNlTranslation(string propertyName)
        {
            try
            {
                return EnNlTranslations[propertyName];
            }
            catch (Exception)
            {
                return "";
            }
        }

        public string NlToEnTranslation(string propertyName)
        {
            try
            {
                var translation = EnNlTranslations.FirstOrDefault(x => x.Value == propertyName).Key;

                if (translation == null) return "";
                else return translation;
            }
            catch (Exception)
            {
                return "";
            }
        }

        public override string ToString()
        {
            return JsonConvert.SerializeObject(this);
        }
    }
}
