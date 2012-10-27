/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/25/2012
 * Time: 9:31 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Drawing.Imaging;
using System.IO;
namespace miRobotEditor
{
	/// <summary>
	/// Description of Utilities.
	/// </summary>
	public static class Utilities
	{
		 public static BitmapImage LoadBitmap(Bitmap img)
            {
			   	var result = new BitmapImage();
			   	using (var ms = new MemoryStream())
			   	{
			   		img.Save(ms,ImageFormat.Jpeg);
			   		result.BeginInit();
			   		result.StreamSource = new MemoryStream(ms.ToArray());
			   		result.EndInit();			   		
			   	}
			   	return result;
		   }

        

         /// <summary>
         /// Converts a <see cref="System.Drawing.Image"/> into a WPF <see cref="BitmapSource"/>.
         /// </summary>
         /// <param name="source">The source image.</param>
         /// <returns>A BitmapSource</returns>
         public static BitmapSource ToBitmapSource(this Image source)
         {
             var bitmap = new System.Drawing.Bitmap(source);

             var bitSrc = bitmap.ToBitmapSource();

             bitmap.Dispose();
             bitmap = null;

             return bitSrc;
         }
		  public static BitmapImage LoadBitmap(string fileName)
            {
                // TODO: add some kind of cache to avoid reloading the image whenever the
                // VisualLine is reconstructed
                try
                { 
                   
                    if (File.Exists(fileName))
                    {
                        BitmapImage bitmap = new BitmapImage(new Uri(fileName));
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
