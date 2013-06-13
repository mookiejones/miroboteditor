﻿using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace miRobotEditor.Core
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


 

        public static ImageSource GetIcon(string fileName)
        {
            var bi = new BitmapImage();
            bi.BeginInit();
            bi.UriSource = new Uri(fileName);
            bi.EndInit();
            return bi;
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