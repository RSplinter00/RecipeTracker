using Newtonsoft.Json;

namespace RecipeTracker.Models
{
    public class Issue
    {
        public string User { get; set; }
        public string Device { get; set; }
        public string Description { get; set; }

        [JsonIgnore]
        public bool IsValid => !string.IsNullOrWhiteSpace(User) && !string.IsNullOrWhiteSpace(Description);
    }
}
