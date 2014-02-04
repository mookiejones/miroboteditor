﻿using System;
using System.Globalization;
using System.IO;
using System.Windows.Data;
using miRobotEditor.Core;
using miRobotEditor.Resources;
using miRobotEditor.Resources.StringResources;
using Global = miRobotEditor.Classes.Global;
using Utilities = miRobotEditor.Classes.Utilities;

namespace miRobotEditor.ViewModel
{
    public class GetFileIconConverter : IValueConverter
    {
        public object Convert(object value, Type targetType, object parameter, CultureInfo culture)
        {
            try
            {
                switch (Path.GetExtension(value.ToString().ToLower()))
                {
                    case ".src":
                        var ico =  Utilities.LoadBitmap(Global.ImgSrc);
                        return ico;
                    case ".dat":
                        return Utilities.LoadBitmap(Global.ImgDat);
                    case ".sub":
                        return Utilities.LoadBitmap(Global.ImgSps);
                }
                

            }
            catch(Exception ex )
            {
                MessageViewModel.AddError(ErrorResources.ConvertError,ex);
            }
            return null;
        }

        public object ConvertBack(object value, Type targetType, object parameter, CultureInfo culture)
        {
            throw new NotImplementedException();
        }

    }
}