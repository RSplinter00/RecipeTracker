using Prism.Mvvm;
using System;
using SQLite;

namespace ReceptTracker.Models
{
    public class Recipe : BindableBase
    {
        [PrimaryKey, AutoIncrement]
        public int ID { get; set; }
        public string Title { get; set; }
        public string Category { get; set; }
        public TimeSpan Duration { get; set; }
        public string Method { get; set; }
        public string Ingredients { get; set; }
        public string Requirements { get; set; }
        public string Preparation { get; set; }

        public Recipe()
        {
        }

        public Recipe(string title, string category, TimeSpan duration) : base()
        {
            Title = title;
            Category = category;
            Duration = duration;
        }
    }
}
