using Prism.Mvvm;
using System;

namespace ReceptTracker.Models
{
    public class Recipe : BindableBase
    {
        public string Title { get; set; }
        public string Text { get; set; }
        public string Category { get; set; }
        public TimeSpan Duration { get; set; }

        public Recipe()
        {
        }

        public Recipe(string title, string text, string category, TimeSpan duration)
        {
            Title = title;
            Text = text;
            Category = category;
            Duration = duration;
        }
    }
}
