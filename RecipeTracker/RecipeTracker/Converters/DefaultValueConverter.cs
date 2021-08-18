using System;
using System.Globalization;
using Xamarin.Forms;

namespace RecipeTracker.Converters
{
    class DefaultValueConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) return "";
            else if (value is int) return (int)value == 0 ? "" : value.ToString();
            else return value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            var valueString = (string)value;

            if (string.IsNullOrEmpty(valueString) || valueString.Equals(culture.NumberFormat.NumberDecimalSeparator)) return 0;
            
            var confirmation = int.TryParse(valueString, out int result);
            if (confirmation) return result;
            else return valueString;
        }
    }
}
