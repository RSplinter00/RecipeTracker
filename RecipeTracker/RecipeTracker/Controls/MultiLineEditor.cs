using Xamarin.Forms;

namespace RecipeTracker.Controls
{
    /// <summary>
    /// Customised version of the Editor, which allows for a maximum height to be set.
    /// 
    /// <para>
    ///     For the original, see <seealso cref="Editor"/>.
    /// </para>
    /// </summary>
    public class MultiLineEditor : Editor
    {
        // Default height is 45.33
        // If AutoSize == TextChanges: Height is increased by 21 per line
        public double MaxHeight { get; set; }

        protected override SizeRequest OnMeasure(double widthConstraint, double heightConstraint)
        {
            // Prevent the height of the editor to not exceed the maximum height
            var sizeRequest = base.OnMeasure(widthConstraint, heightConstraint);
            var newHeight = sizeRequest.Request.Height;

            if (newHeight > MaxHeight) newHeight = MaxHeight;

            return new SizeRequest(new Size(sizeRequest.Request.Width, newHeight));
        }
    }
}
