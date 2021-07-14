using Prism.Mvvm;
using System;
using SQLite;

namespace ReceptTracker.Models
{
    public class Recipe : BindableBase
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Name { get; set; }
        public string Category { get; set; }
        // Time it takes to prepare the recipe
        public TimeSpan PrepTime { get; set; }
        // Time it takes to cook the meal
        public TimeSpan CookingTime { get; set; }
        // Time it takes for the meat to rest
        public TimeSpan RestTime { get; set; }
        // Sum of all timespans
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
    }
}
