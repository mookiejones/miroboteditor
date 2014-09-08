﻿/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/16/2013
 * Time: 09:46
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;
using System.Windows.Data;
using ICSharpCode.AvalonEdit.Document;

namespace miRobotEditor.Core.Converters
{
    public sealed class ActiveEditorConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is IDocument)
            {
                return value;
            }

            return Binding.DoNothing;
        }

        public object ConvertBack(object value, Type targetType, object parameter, System.Globalization.CultureInfo culture)
        {
            if (value is  IDocument)
                return value;

            return Binding.DoNothing;
        }
    }
}
