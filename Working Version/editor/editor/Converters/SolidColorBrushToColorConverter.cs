using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Data;
using System.Windows.Media;

namespace miRobotEditor.Converters
{
    public class SolidColorBrushToColorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            SolidColorBrush solidColorBrush = value as SolidColorBrush;

          
            if (solidColorBrush != null)
            {
                return solidColorBrush.Color;
            }
            return default(Color);
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            if (value != null)
            {
                Color color = (Color)value;
                return new SolidColorBrush(color);
            }
            return null;
        }
    }

}
