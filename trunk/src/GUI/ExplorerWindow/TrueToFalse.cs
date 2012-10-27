using System;
using System.Windows.Data;
using miRobotEditor.Classes;

namespace miRobotEditor.GUI.ExplorerWindow
{
	public class TrueToFalse : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			bool rv = false;
			try
			{
				var x = bool.Parse(value.ToString());

				rv = !x;
			}
			catch (Exception  ex)
			{
                OutputMessages.AddError(ex);
			}
			return rv;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

	}

//    [ValueConversion(typeof(string), typeof(bool))]
//    public class HeaderToImageConverter : IValueConverter
//    {
//        public static HeaderToImageConverter _instance = new HeaderToImageConverter();

//        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            if ((value as string) == "DRIVE")
//            {
//                var uri = new Uri("pack://BeeCoders.Tools.Wpf;,,,/Images/diskdrive.png");
//                var source = new BitmapImage();
//                return source;
//            }
//            else
//            {
//                if ((value as string) == "DIR")
//                {
//                    var uri = new Uri("pack://BeeCoders.Tools.Wpf;/Images/diskdrive.png");
////					var uri = new Uri("pack://application:,,,/Images/folder.png");
//                    var source = new BitmapImage(uri);
//                    return source;
//                }
//                else
//                {
//                    var uri = new Uri("pack://BeeCoders.Tools.Wpf;,,,/Images/diskdrive.png");
////					var uri = new Uri("pack://application:,,,/Images/folder.png");
//                    var source = new BitmapImage(uri);
//                    return source;
//                }
//            }
//        }

//        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
//        {
//            throw new NotSupportedException("Cannot convert back");
//        }
//    }
}
