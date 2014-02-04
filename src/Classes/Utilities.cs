﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Classes
{
    public static class Utilities
    {
        /// <summary>
        /// Load Bitmap and Convert to BitmapImage
        /// </summary>
        /// <param name="img"></param>
        /// <returns></returns>
        public static BitmapImage LoadBitmap(Bitmap img)
        {
            var result = new BitmapImage();
            using (var ms = new MemoryStream())
            {
                img.Save(ms, ImageFormat.Jpeg);
                result.BeginInit();
                result.StreamSource = new MemoryStream(ms.ToArray());
                result.EndInit();
            }
            return result;
        }

        /// <summary>
        /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
        /// </summary>
        /// <returns>A BitmapSource</returns>
        // ReSharper disable FunctionRecursiveOnAllPaths
        //      public static BitmapSource ToBitmapSource(this Image source)
        // ReSharper restore FunctionRecursiveOnAllPaths
        //      {
        //          var bitmap = new Bitmap(source);
        //
        //          var bitSrc = bitmap.ToBitmapSource();
        //
        //          bitmap.Dispose();
        //
        //          return bitSrc;
        //      }

       public static ImageSource GetIcon(string resourcename)
       {

           System.Windows.Controls.Image img = App.Current.FindResource(resourcename) as System.Windows.Controls.Image;
           return img.Source;

        //  var bi = new BitmapImage();
        //  bi.BeginInit();
        //  bi.UriSource = new Uri(fileName);
        //  bi.EndInit();
        //  return bi;
       }


        /// <summary>
        /// Load Bitmap and Convert to Bitmap Image
        /// </summary>
        /// <param name="fileName">Filename of Image</param>
        /// <returns></returns>
        public static BitmapImage LoadBitmap(string fileName)
        {
            try
            {
                if (File.Exists(fileName))
                {
                    var fi = new FileInfo(fileName);
                    var bitmap = new BitmapImage(new Uri(fi.FullName));
                    bitmap.Freeze();
                    return bitmap;
                }
            }
            catch (ArgumentException)
            {
                // invalid filename syntax
            }
            catch (IOException)
            {
                // other IO error
            }
            return null;
        }
    }
}