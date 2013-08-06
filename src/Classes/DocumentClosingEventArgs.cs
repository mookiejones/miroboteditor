/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/15/2013
 * Time: 2:48 PM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System;

namespace miRobotEditor.Classes
{
    /// <summary>
    /// Arguments to the DocumentClosing event.
    /// </summary>
    public sealed class DocumentClosingEventArgs : EventArgs
    {
        internal DocumentClosingEventArgs(object document)
        {
            Document = document;
            Cancel = false;
        }

        /// <summary>
        /// View-model object for the document being closed.
        /// </summary>
        public object Document
        {
            get;
            private set;
        }

        /// <summary>
        /// Set to 'true' to cancel closing of the document.
        /// The defualt value is 'false'.
        /// </summary>
        public bool Cancel
        {
            get;
            set;
        }
    }
}