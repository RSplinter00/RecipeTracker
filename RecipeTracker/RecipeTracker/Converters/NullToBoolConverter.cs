using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace RecipeTracker.Converters
{
    /// <summary>
    /// Class <c>NullToBoolConverter</c> converts values to a boolean.
    /// Returns true if the value is not null, else returns false.
    /// </summary>
    public class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            bool invert = false;

            // Check the parameter to see if the result needs to be inverted.
            if (parameter != null && parameter is string && (string)parameter == "invert") invert = true;

            // Check if the value is null.
            if (value == null) result = false;
            else if (value is string) result = !string.IsNullOrEmpty((string)value);
            else if (value is int) result = (int)value != 0;
            else if (value is TimeSpan) result = TimeSpan.Compare((TimeSpan)value, new TimeSpan()) != 0;
            else if (value is List<Recipe>) result = ((List<Recipe>)value).Count > 0;

            return invert ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
