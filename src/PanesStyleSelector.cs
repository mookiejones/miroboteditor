/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/20/2013
 * Time: 9:31 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Windows;
using System.Windows.Controls;
using miRobotEditor.ViewModel;

namespace miRobotEditor
{
	/// <summary>
	/// Description of PanesStyleSelector.
	/// </summary>
    class PanesStyleSelector : StyleSelector
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
}
