﻿#pragma checksum "..\..\..\..\GUI\FileExplorer\DMC_FileExplorer.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "FD4FBF4B6DB9B247E8E726355E30D256"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17379
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AvalonDock;
using DMC_Robot_Editor.GUI.FileExplorer;
using System;
using System.Diagnostics;
using System.Windows;
using System.Windows.Automation;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Data;
using System.Windows.Documents;
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


namespace DMC_Robot_Editor.GUI.FileExplorer {
    
    
    /// <summary>
    /// DMC_FileExplorer
    /// </summary>
    public partial class DMC_FileExplorer : AvalonDock.DocumentContent, System.Windows.Markup.IComponentConnector {
        
        
        #line 27 "..\..\..\..\GUI\FileExplorer\DMC_FileExplorer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ComboBox cmbFilter;
        
        #line default
        #line hidden
        
        
        #line 29 "..\..\..\..\GUI\FileExplorer\DMC_FileExplorer.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.TreeView foldersItem;
        
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
            System.Uri resourceLocater = new System.Uri("/DMC Robot Editor;component/gui/fileexplorer/dmc_fileexplorer.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\..\GUI\FileExplorer\DMC_FileExplorer.xaml"
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
            this.cmbFilter = ((System.Windows.Controls.ComboBox)(target));
            return;
            case 2:
            this.foldersItem = ((System.Windows.Controls.TreeView)(target));
            
            #line 29 "..\..\..\..\GUI\FileExplorer\DMC_FileExplorer.xaml"
            this.foldersItem.SelectedItemChanged += new System.Windows.RoutedPropertyChangedEventHandler<object>(this.foldersItem_SelectedItemChanged);
            
            #line default
            #line hidden
            
            #line 29 "..\..\..\..\GUI\FileExplorer\DMC_FileExplorer.xaml"
            this.foldersItem.Loaded += new System.Windows.RoutedEventHandler(this.Window_Loaded);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

