using System;

namespace miRobotEditor.Core.Content
{
    /// <summary>
    /// A simple view content that does not use any files and simply displays a fixed control.
    /// </summary>
    public class SimpleViewContent : AbstractViewContent
    {
        readonly object content;

        public override object Control
        {
            get
            {
                return content;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="content"></param>
        public SimpleViewContent(object content)
        {
            if (content == null)
                throw new ArgumentNullException("content");
            this.content = content;
        }

        // make this method public
        /// <inheritdoc/>
        public new void SetLocalizedTitle(string text)
        {
            base.SetLocalizedTitle(text);
        }
        /// <summary>
        /// 
        /// </summary>
        public new string TitleName
        {
            get { return base.TitleName; }
            set { base.TitleName = value; } // make setter public
        }
    }
}
