using RecipeTracker.Models;
using System;
using System.Collections.Generic;
using System.Globalization;
using Xamarin.Forms;

namespace RecipeTracker.Converters
{
    class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            bool result = true;
            bool invert = false;

            if (parameter != null && parameter is string && (string)parameter == "invert") invert = true;

            if (value == null) result = false;
            else if (value is string) result = !string.IsNullOrEmpty((string)value);
            else if (value is int) result = (int)value != 0;
            else if (value is TimeSpan) result = TimeSpan.Compare((TimeSpan)value, new TimeSpan()) != 0;
            else if (value is List<Recipe>)
            {
                result = ((List<Recipe>)value).Count > 0;
            }

            return invert ? !result : result;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
