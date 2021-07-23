using System;
using System.Collections.ObjectModel;
using System.Globalization;
using Xamarin.Forms;

namespace ReceptTracker.Converters
{
    class ShowPropertyConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null || parameter == null) return false;

            if (value is ObservableCollection<string> && parameter is string)
            {
                var val = (ObservableCollection<string>)value;
                var param = (string)parameter;

                return val.Contains(param);
            }

            return false;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }
    }
}
