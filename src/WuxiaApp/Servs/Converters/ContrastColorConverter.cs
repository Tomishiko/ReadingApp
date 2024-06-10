using System;
using System.Collections.Generic;
using System.Globalization;
using CommunityToolkit.Maui.Behaviors;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommunityToolkit.Maui.Core;

namespace WuxiaApp.Servs.Converters
{
    public class ContrastColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value == null) { return Color.FromRgb(200,200,200); }
            var color = value as Color;
            double luminance = (0.299 * color.Red + 0.587 * color.Green + 0.114 * color.Blue);
            if(targetType == typeof(StatusBarStyle))
            {
                if (luminance > 0.5)
                    return StatusBarStyle.DarkContent;
                else
                    return StatusBarStyle.LightContent;
            }
            else
            {
                if (luminance > 0.5)
                    return Color.FromRgb(50, 50, 50);
                else
                    return Color.FromRgb(200, 200, 200);
            }
            

        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return value;
        }
    }
}
