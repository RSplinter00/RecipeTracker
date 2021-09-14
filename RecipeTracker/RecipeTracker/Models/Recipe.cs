using System;
using SQLite;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace RecipeTracker.Models
{
    /// <summary>
    /// Class <c>Recipe</c> represents the recipe for a dish.
    /// </summary>
    public class Recipe
    {
        [JsonIgnore]
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
        
        [PrimaryKey]
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
        
        [Ignore, JsonIgnore]
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

        /// <summary>
        /// Translates an English property name to Dutch.
        /// 
        /// <para>
        ///     For the Dutch to English translation, see <seealso cref="NlToEnTranslation(string)"/>.
        /// </para>
        /// </summary>
        /// <param name="propertyName">Name of the property, in English, to be translated to Dutch.</param>
        /// <returns></returns>
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

        /// <summary>
        /// Translates a Dutch property name to English.
        /// 
        /// <para>
        ///     For the English to Dutch translation, see <seealso cref="EnToNlTranslation(string)"/>.
        /// </para>
        /// </summary>
        /// <param name="propertyName">Name of the property, in Dutch, to be translated to English.</param>
        /// <returns></returns>
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

        public override bool Equals(object obj)
        {
            if (!(obj is Recipe)) return false;

            // Define if the recipe is unique by its Id property.
            return this.Id == ((Recipe)obj).Id;

        }
    }
}
