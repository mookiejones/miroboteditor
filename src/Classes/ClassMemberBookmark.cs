﻿/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/23/2012
 * Time: 12:53 PM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Windows.Input;
using miRobotEditor.Interfaces;

//TODO Remove this file!!!

namespace miRobotEditor.Classes
{
    /// <summary>
    ///     Description of ClassMemberBookmark.
    /// </summary>
    public class ClassMemberBookmark : IBookmark
    {
        private readonly IImage _image;
        private readonly int _linenumber;

        public ClassMemberBookmark(int lineNumber, IImage image)
        {
            _image = image;
            _linenumber = lineNumber;
        }


        public int LineNumber
        {
            get { return _linenumber; }
        }

        public IImage Image
        {
            get { return _image; }
        }

        public int ZOrder
        {
            get { return -10; }
        }

        public bool CanDragDrop
        {
            get { return false; }
        }

        public void MouseDown(MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                //	var f = Services.AnalyticsMonitorService.TrackFeature("ICSharpCode.SharpDevelop.Bookmarks.ClassMemberBookmark.ShowContextMenu");
                //TODO Fix this

                //var ctx = MenuService.ShowContextMenu(e.Source as UIElement, this, ContextMenuPath);
                //		ctx.Closed += delegate { f.EndTracking(); };
                e.Handled = true;
            }
        }

        public void MouseUp(MouseButtonEventArgs e)
        {
        }

        public void Drop(int lineNumber)
        {
        }
    }
}