﻿#pragma checksum "..\..\..\Controls\Editor.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "A9FCBA8A95DEBB4096784C931F88FD1C"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.18444
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

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
using miRobotEditor.Classes;


namespace miRobotEditor.Controls {
    
    
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
            System.Uri resourceLocater = new System.Uri("/miRobotEditor;component/controls/editor.xaml", System.UriKind.Relative);
            
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
            
            #line 9 "..\..\..\Controls\Editor.xaml"
            ((miRobotEditor.Controls.Editor)(target)).MouseHover += new System.Windows.Input.MouseEventHandler(this.Mouse_OnHover);
            
            #line default
            #line hidden
            
            #line 10 "..\..\..\Controls\Editor.xaml"
            ((miRobotEditor.Controls.Editor)(target)).PreviewMouseWheel += new System.Windows.Input.MouseWheelEventHandler(this.EditorPreviewMouseWheel);
            
            #line default
            #line hidden
            
            #line 12 "..\..\..\Controls\Editor.xaml"
            ((miRobotEditor.Controls.Editor)(target)).GotFocus += new System.Windows.RoutedEventHandler(this.TextEditorGotFocus);
            
            #line default
            #line hidden
            
            #line 14 "..\..\..\Controls\Editor.xaml"
            ((miRobotEditor.Controls.Editor)(target)).PreviewKeyDown += new System.Windows.Input.KeyEventHandler(this.TextEditor_PreviewKeyDown);
            
            #line default
            #line hidden
            return;
            }
            this._contentLoaded = true;
        }
    }
}
