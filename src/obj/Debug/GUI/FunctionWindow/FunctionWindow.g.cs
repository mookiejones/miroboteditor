﻿#pragma checksum "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "F8655330CE8104D886B029428A18E1B7"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Forms.Integration;
using System.Windows.Ink;
using System.Windows.Input;
using System.Windows.Markup;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Media.TextFormatting;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Windows.Shell;


namespace miRobotEditor.GUI.FunctionWindow {
    
    
    /// <summary>
    /// FunctionWindow
    /// </summary>
    public partial class FunctionWindow : System.Windows.Controls.UserControl, System.Windows.Markup.IComponentConnector {
        
        
        #line 9 "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ListView liItems;
        
        #line default
        #line hidden
        
        private bool _contentLoaded;
        
        /// <summary>
        /// InitializeComponent
        /// </summary>
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        public void InitializeComponent() {
            if (_contentLoaded) {
                return;
            }
            _contentLoaded = true;
            System.Uri resourceLocater = new System.Uri("/miRobotEditor;component/gui/functionwindow/functionwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.ComponentModel.EditorBrowsableAttribute(System.ComponentModel.EditorBrowsableState.Never)]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Maintainability", "CA1502:AvoidExcessiveComplexity")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1800:DoNotCastUnnecessarily")]
        void System.Windows.Markup.IComponentConnector.Connect(int connectionId, object target) {
            switch (connectionId)
            {
            case 1:
            this.liItems = ((System.Windows.Controls.ListView)(target));
            
            #line 9 "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml"
            this.liItems.ToolTipOpening += new System.Windows.Controls.ToolTipEventHandler(this.liItems_ToolTipOpening);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml"
            this.liItems.SelectionChanged += new System.Windows.Controls.SelectionChangedEventHandler(this.SelectedItemChanged);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml"
            this.liItems.MouseMove += new System.Windows.Input.MouseEventHandler(this.ListBox_MouseMove);
            
            #line default
            #line hidden
            
            #line 9 "..\..\..\..\GUI\FunctionWindow\FunctionWindow.xaml"
            this.liItems.MouseDoubleClick += new System.Windows.Input.MouseButtonEventHandler(this.DoubleClick);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
