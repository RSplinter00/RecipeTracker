using Xamarin.Forms;

namespace ReceptTracker.Views
{
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
                this.width = width;
                this.height = height;

                if (width > height)
                {
                    // Show the portrait layout and hide the landscape layout
                    outerPortraitLayout.IsVisible = false;
                    outerLandscapeLayout.IsVisible = true;
                }
                else
                {
                    // Hide the portrait layout and show the landscape layout
                    outerPortraitLayout.IsVisible = true;
                    outerLandscapeLayout.IsVisible = false;
                }
            }
        }
    }
}
