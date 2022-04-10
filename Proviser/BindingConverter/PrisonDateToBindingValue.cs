using System;
using System.Globalization;
using Xamarin.Forms;
using Xamarin.Forms.Xaml;

namespace Proviser.BindingConverter
{
    public class PrisonDateToBindingValue : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if ((DateTime)value > System.Convert.ToDateTime("01.01.2000 00:00:00"))
            {
                return "🔗Содержание под стражей до " + ((DateTime)value).ToString("dd.MM.yyyy");
            }
            else
            {
                return "";
            }

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
      
    }
}
