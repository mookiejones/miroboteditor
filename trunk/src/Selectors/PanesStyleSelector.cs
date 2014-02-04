﻿using miRobotEditor.Core;
using miRobotEditor.ViewModel;
using System.Windows;
using System.Windows.Controls;

namespace miRobotEditor.Selectors
{
    public class PanesStyleSelector : StyleSelector
    {
        public Style ToolStyle { get; set; }

        public Style FileStyle { get; set; }

        public override Style SelectStyle(object item, DependencyObject container)
        {
            if (item is ToolViewModel) return ToolStyle;
            if (item is IDocument) return FileStyle;

            //TODO Still Need to add file explorer
            return base.SelectStyle(item, container);
        }
    }
}