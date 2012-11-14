﻿using System;
using System.ComponentModel;
using System.Globalization;

namespace miRobotEditor.Core.Convertors
{
    /// <summary>
    /// BooleanTypeConverter
    /// </summary>
    public class BooleanTypeConverter : TypeConverter
    {
        string True
        {
            get
            {
                return "True";
            }
        }
        string False
        {
            get
            {
                return "False";
            }
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            return sourceType == typeof(bool) || sourceType == typeof(string);
        }
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            return destinationType == typeof(bool) || destinationType == typeof(string);
        }

        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            return true;
        }
        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override System.ComponentModel.TypeConverter.StandardValuesCollection GetStandardValues(System.ComponentModel.ITypeDescriptorContext context)
        {
            return new StandardValuesCollection(new object[] { True, False });
        }

        // Overrides the ConvertFrom method of TypeConverter.
        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string)
            {
                return value.ToString() == True;
            }
            return value;
        }

        // Overrides the ConvertTo method of TypeConverter.
        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is bool)
            {
                return ((bool)value) ? True : False;
            }
            return value;
        }
    }
}