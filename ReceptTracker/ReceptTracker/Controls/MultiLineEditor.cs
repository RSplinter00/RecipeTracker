using Xamarin.Forms;

namespace ReceptTracker.Controls
{
    class MultiLineEditor : Editor
    {
        // Default height is 45.33
        // If AutoSize == TextChanges: Height is increased by 21 per line
        public double MaxHeight { get; set; }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            var sizeRequest = base.OnMeasure(widthConstraint, heightConstraint);

            var newHeight = sizeRequest.Request.Height;
            if (newHeight > MaxHeight)
                newHeight = MaxHeight;

            return new SizeRequest(new Size(sizeRequest.Request.Width, newHeight));
        }
    }
}
