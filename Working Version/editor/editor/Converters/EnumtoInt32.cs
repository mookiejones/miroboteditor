﻿using miRobotEditor.Enums;
using System;
using System.Globalization;
using System.Windows.Data;

namespace miRobotEditor.Converters
{
    public sealed class EnumtoInt32 : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture) => (int)((CartesianEnum)value);

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture) => (CartesianEnum)Enum.Parse(typeof(CartesianEnum), ((int)value).ToString(CultureInfo.InvariantCulture));
    }
}