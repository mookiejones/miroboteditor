/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/23/2012
 * Time: 1:14 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Windows.Media.Imaging;
using miRobotEditor.Interfaces;

namespace miRobotEditor
{
	/// <summary>
	/// Description of BookmarkImage.
	/// </summary>
	public class BookmarkImage:IImage
	{
		
		private readonly IImage _baseimage = null;
		
		private readonly BitmapImage _bitmap;
		
		public BookmarkImage(BitmapImage bitmap)
		{
			_bitmap = bitmap;
		}
		
		public System.Windows.Media.ImageSource ImageSource {
			get {
				return _baseimage.ImageSource;
			}
		}
		
		public BitmapImage Bitmap {
			get {
			return _bitmap;
			}
		}
		
		public System.Drawing.Icon Icon {
			get {
				return _baseimage.Icon;
			}
		}
	}
}
