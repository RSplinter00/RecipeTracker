using Prism.Mvvm;
using System;
using System.Collections.Generic;

namespace ReceptTracker.Models
{
    public class Recipe : BindableBase
    {
        public string Title { get; set; }
        public string Category { get; set; }
        public TimeSpan Duration { get; set; }
        public List<string> Ingredients { get; set; }
        public List<string> Requirements { get; set; }
        public string Preparation { get; set; }
        public string Method { get; set; }


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
