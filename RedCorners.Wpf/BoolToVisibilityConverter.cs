using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;
using System.Windows;
using System.Windows.Data;

namespace RedCorners.Wpf
{
    public class BoolToVisibilityConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is bool b)
            {
                if (b) return Visibility.Visible;
                return Visibility.Collapsed;
            }

            if (value == null)
                throw new ArgumentNullException("BoolToVisibilityConverter -> needs a bool. null passed instead.");
            throw new ArgumentException($"BoolToVisibilityConverter -> needs a bool. {value.GetType().Name} passed instead.");
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value is Visibility v)
            {
                if (v == Visibility.Visible) return true;
                return false;
            }

            if (value == null)
                throw new ArgumentNullException("BoolToVisibilityConverter <- needs a Visibility. null passed instead.");
            throw new ArgumentException($"BoolToVisibilityConverter <- needs a Visibility. {value.GetType().Name} passed instead.");
        }
    }
}
