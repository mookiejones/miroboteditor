/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/23/2012
 * Time: 12:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows.Controls;
using miRobotEditor.Interfaces;

namespace miRobotEditor
{
	/// <summary>
	/// Description of ClassMemberBookmark.
	/// </summary>
	public class ClassMemberBookmark:IBookmark	
	{
		
		private int _linenumber;
		private IImage _image;
		public ClassMemberBookmark(int lineNumber,IImage image)
		{
			
			_image = image;
			_linenumber=lineNumber;
		}
		
		
		
		public int LineNumber {
			get {
				return _linenumber;
			}
		}
		
		public IImage Image {
			get {
				return _image;
			}
		}
		
		public int ZOrder {
			get {
				return -10;
			}
		}
		
		public bool CanDragDrop {
			get {
				return false;
			}
		}
		
		public void MouseDown(System.Windows.Input.MouseButtonEventArgs e)
		{
			
		}
		
		public void MouseUp(System.Windows.Input.MouseButtonEventArgs e)
		{
		}
		
		public void Drop(int lineNumber)
		{
		}
	}
}
