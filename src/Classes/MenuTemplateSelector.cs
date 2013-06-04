﻿using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Classes
{
    public class MenuTemplateSelector:DataTemplateSelector
    {

        public DataTemplate KUKATemplate { get; set; }
        public DataTemplate FanucTemplate { get; set; }
        public DataTemplate NachiTemplate { get; set; }
        public DataTemplate ABBTemplate { get; set; }

        

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            FrameworkElement element = container as FrameworkElement;

            if (element != null && item != null && item is Task)
            {
                Task taskitem = item as Task;

              // if (taskitem.Priority == 1)
              //     return
              //         element.FindResource("importantTaskTemplate") as DataTemplate;
              // else
              //     return
              //         element.FindResource("myTaskTemplate") as DataTemplate;
            }

            return null;
        }
    }
}
