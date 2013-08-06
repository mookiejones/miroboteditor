/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/20/2013
 * Time: 9:31 AM
 *
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using miRobotEditor.Core;
using miRobotEditor.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor
{
    /// <summary>
    /// Description of PanesStyleSelector.
    /// </summary>
    internal class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle
        {
            get;
            set;
        }

        public Style FileStyle
        {
            get;
            set;
        }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolViewModel)
                return ToolStyle;

            if (item is IDocument)
                return FileStyle;

            return base.SelectStyle(item, container);
        }
    }

    internal class PanesTemplateSelector : DataTemplateSelector
    {
        public DataTemplate FileViewTemplate
        {
            get;
            set;
        }

        public DataTemplate FileStatsViewTemplate
        {
            get;
            set;
        }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item is IDocument)
                return FileViewTemplate;
            //
            //   if (item is FileStatsViewModel)
            //       return FileStatsViewTemplate;

            return base.SelectTemplate(item, container);
        }
    }

    public interface IPaneViewModel
    {
        string Title { get; set; }

        System.Windows.Media.ImageSource IconSource { get; set; }

        string ContentId { get; set; }

        bool IsSelected { get; set; }

        bool IsActive { get; set; }
    }
}