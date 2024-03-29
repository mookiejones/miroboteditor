﻿using System;
using System.Globalization;
using System.Windows.Data;
using miRobotEditor.Position;

namespace miRobotEditor.Converters
{
    public sealed class EnumtoInt32 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (int)(CartesianEnum)value;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            return (CartesianEnum)Enum.Parse(typeof(CartesianEnum), ((int)value).ToString(CultureInfo.InvariantCulture));
        }
    }
}