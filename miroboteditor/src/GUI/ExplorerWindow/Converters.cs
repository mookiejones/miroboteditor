using System;
using System.Windows;
using System.Windows.Data;

namespace miRobotEditor.Controls.Infrastructure
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
			catch (Exception)
			{
			}
			return rv;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

	}

	public class BooleanToHiddenVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Visibility rv = Visibility.Visible;
			try
			{
                rv = bool.Parse(value.ToString()) ? Visibility.Hidden : Visibility.Visible;
			}
			catch (Exception)
			{
			}
			return rv;
		}

		public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			return value;
		}

	}

	public class BooleanToVisualVisibility : IValueConverter
	{
		public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
		{
			Visibility rv = Visibility.Visible;
			try
			{
                rv = bool.Parse(value.ToString()) ? Visibility.Visible : Visibility.Hidden;
			}
			catch (Exception)
			{
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
