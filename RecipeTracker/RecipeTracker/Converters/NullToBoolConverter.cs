using System;
using System.Globalization;
using Xamarin.Forms;

namespace RecipeTracker.Converters
{
    class NullToBoolConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is string) return !string.IsNullOrEmpty((string)value);
            else if (value is int) return (int)value != 0;
            else if (value is TimeSpan) return TimeSpan.Compare((TimeSpan)value, new TimeSpan()) != 0;

            return value != null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
