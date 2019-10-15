using System;
using System.Globalization;
using System.Windows.Data;

namespace HLab.Erp.Core.ViewModels
{
    class ViewModelConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return ((double)value * (double)parameter);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return true;
        }        
    }
}
