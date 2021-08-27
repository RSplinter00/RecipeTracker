using System;
using System.Globalization;
using Xamarin.Forms;

namespace RecipeTracker.Converters
{
    /// <summary>
    /// Class <c>DefaultValueConverter</c> can be used to return an empty string if the value is null or 0.
    /// </summary>
    public class DefaultValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // If the value is null or 0 (in the case it is an integer), return an empty string, else return the value.
            if (value == null) return "";
            else if (value is int) return (int)value == 0 ? "" : value.ToString();
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            // Return the stringified version of the value.
            var valueString = (string)value;

            if (string.IsNullOrEmpty(valueString) || valueString.Equals(culture.NumberFormat.NumberDecimalSeparator)) return 0;
            
            var confirmation = int.TryParse(valueString, out int result);
            if (confirmation) return result;
            else return valueString;
        }
    }
}
