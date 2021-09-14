using Xamarin.Forms;

namespace RecipeTracker.Views
{
    /// <summary>
    /// View <c>DisplayRecipePage</c> displays the details of the selected recipe.
    /// </summary>
    public partial class DisplayRecipePage : ContentPage
    {
        private double width = 0;
        private double height = 0;

        public DisplayRecipePage()
        {
            InitializeComponent();
        }

        protected override void OnSizeAllocated(double width, double height)
        {
            base.OnSizeAllocated(width, height);

            if (this.width != width || this.height != height)
            {
                // Keep track of the page's width and height, if it has been changed.
                this.width = width;
                this.height = height;

                if (width > height)
                {
                    // Hide the portrait layout and show the landscape layout.
                    outerPortraitLayout.IsVisible = false;
                    outerLandscapeLayout.IsVisible = true;

                    // If either the ingredients or requirements is expanded, expand outerLandscapeLayout.
                    // If both are collapsed, collapse outerLandscapeLayour aswell.
                    outerLandscapeLayout.IsExpanded = ingredientsExpander.IsExpanded || requirementsExpander.IsExpanded;
                }
                else
                {
                    // Show the portrait layout and hide the landscape layout.
                    outerPortraitLayout.IsVisible = true;
                    outerLandscapeLayout.IsVisible = false;

                    // If outerLandscapeLayout is expanded, expand the ingredients and requirements.
                    // If outerLandscapeLayout is collapsed, collapse the ingredients and requirements.
                    ingredientsExpander.IsExpanded = outerLandscapeLayout.IsExpanded;
                    requirementsExpander.IsExpanded = outerLandscapeLayout.IsExpanded;
                }
            }
        }
    }
}
