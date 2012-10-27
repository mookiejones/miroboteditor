using System;

namespace miRobotEditor.Core
{
    /// <summary>
    /// 
    /// </summary>
    /// <param name="sender"></param>
    /// <param name="e"></param>
    public delegate void ViewContentEventHandler(object sender, ViewContentEventArgs e);
    /// <summary>
    /// 
    /// </summary>
    public class ViewContentEventArgs : System.EventArgs
    {
        IViewContent content;
        /// <summary>
        /// Content
        /// </summary>
        public IViewContent Content
        {
            get
            {
                return content;
            }
        }
        /// <summary>
        /// View Content Event Args
        /// </summary>
        /// <param name="content"></param>
        public ViewContentEventArgs(IViewContent content)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            this.content = content;
        }
    }
}
