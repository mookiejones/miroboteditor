/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 04/16/2013
 * Time: 09:49
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.Windows;
using System.Windows.Controls;
using miRobotEditor.ViewModel;
namespace miRobotEditor.Selectors
{
	/// <summary>
	/// Description of PanesTemplateSelector.
	/// </summary>
	  class PanesTemplateSelector : DataTemplateSelector
    {
        public PanesTemplateSelector()
        {
        
        }


        public DataTemplate FileViewTemplate  {get; set;}

        public DataTemplate FileStatsViewTemplate{ get;set;}

        public override System.Windows.DataTemplate SelectTemplate(object item, System.Windows.DependencyObject container)
        {
            var itemAsLayoutContent = item as AvalonDock.Layout.LayoutContent;

       //     if (item is FileViewModel)
         //       return FileViewTemplate;


            return base.SelectTemplate(item, container);
        }
    }
}
