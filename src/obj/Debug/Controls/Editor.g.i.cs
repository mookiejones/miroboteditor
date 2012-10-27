﻿#pragma checksum "..\..\..\Controls\Editor.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "7DD71518A4331674F634DECFEA9068A0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using DMC_Robot_Editor.Controls;
using ICSharpCode.AvalonEdit;
using ICSharpCode.AvalonEdit.Editing;
using ICSharpCode.AvalonEdit.Highlighting;
using ICSharpCode.AvalonEdit.Rendering;
using ICSharpCode.AvalonEdit.Search;
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


namespace DMC_Robot_Editor.Controls {
    
    
    /// <summary>
    /// Editor
    /// </summary>
    public partial class Editor : ICSharpCode.AvalonEdit.TextEditor, System.Windows.Markup.IComponentConnector {
        
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
            System.Uri resourceLocater = new System.Uri("/DMC Robot Editor;component/controls/editor.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Controls\Editor.xaml"
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
            
            #line 13 "..\..\..\Controls\Editor.xaml"
            ((DMC_Robot_Editor.Controls.Editor)(target)).TextChanged += new System.EventHandler(this.Text_Changed);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\Controls\Editor.xaml"
            ((DMC_Robot_Editor.Controls.Editor)(target)).IsVisibleChanged += new System.Windows.DependencyPropertyChangedEventHandler(this.RaiseUpdate);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\Controls\Editor.xaml"
            ((DMC_Robot_Editor.Controls.Editor)(target)).MouseHover += new System.Windows.Input.MouseEventHandler(this.Mouse_Hover);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\Controls\Editor.xaml"
            ((DMC_Robot_Editor.Controls.Editor)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.EditorPreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 16 "..\..\..\Controls\Editor.xaml"
            ((DMC_Robot_Editor.Controls.Editor)(target)).DocumentChanged += new System.EventHandler(this.TextEditor_DocumentChanged_1);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 23 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanReplace);
            
            #line default
            #line hidden
            
            #line 23 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ReplaceExecuted);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 24 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanSave);
            
            #line default
            #line hidden
            
            #line 24 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteSave);
            
            #line default
            #line hidden
            return;
            case 4:
            
            #line 25 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanSaveAs);
            
            #line default
            #line hidden
            
            #line 25 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteSaveAs);
            
            #line default
            #line hidden
            return;
            case 5:
            
            #line 26 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanGoto);
            
            #line default
            #line hidden
            
            #line 26 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteGoto);
            
            #line default
            #line hidden
            return;
            case 6:
            
            #line 27 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanToggleComment);
            
            #line default
            #line hidden
            
            #line 27 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteToggleComment);
            
            #line default
            #line hidden
            return;
            case 7:
            
            #line 28 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanToggleFolding);
            
            #line default
            #line hidden
            
            #line 28 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteToggleFolding);
            
            #line default
            #line hidden
            return;
            case 8:
            
            #line 29 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanToggleAllFolding);
            
            #line default
            #line hidden
            
            #line 29 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteToggleAllFolding);
            
            #line default
            #line hidden
            return;
            case 9:
            
            #line 30 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanOpenAllFolds);
            
            #line default
            #line hidden
            
            #line 30 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteOpenAllFolds);
            
            #line default
            #line hidden
            return;
            case 10:
            
            #line 31 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanCloseAllFolds);
            
            #line default
            #line hidden
            
            #line 31 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteCloseAllFolds);
            
            #line default
            #line hidden
            return;
            case 11:
            
            #line 32 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanShowDefinitions);
            
            #line default
            #line hidden
            
            #line 32 "..\..\..\Controls\Editor.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ExecuteShowDefinitions);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}

