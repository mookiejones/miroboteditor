﻿#pragma checksum "..\..\..\Forms\frmShift.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "DB7E737656FB4A70811DCA83C96DAC64"
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
using miRobotEditor.Controls;


namespace miRobotEditor.Forms {
    
    
    /// <summary>
    /// FrmShift
    /// </summary>
    public partial class FrmShift : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 8 "..\..\..\Forms\frmShift.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox gOldValues;
        
        #line default
        #line hidden
        
        
        #line 9 "..\..\..\Forms\frmShift.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.Controls.ShiftBox OldShift;
        
        #line default
        #line hidden
        
        
        #line 11 "..\..\..\Forms\frmShift.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox gNewValues;
        
        #line default
        #line hidden
        
        
        #line 12 "..\..\..\Forms\frmShift.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.Controls.ShiftBox NewShift;
        
        #line default
        #line hidden
        
        
        #line 14 "..\..\..\Forms\frmShift.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.GroupBox gDifference;
        
        #line default
        #line hidden
        
        
        #line 15 "..\..\..\Forms\frmShift.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.Controls.ShiftBox Difference;
        
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
            System.Uri resourceLocater = new System.Uri("/miRobotEditor;component/forms/frmshift.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\..\Forms\frmShift.xaml"
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
            this.gOldValues = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 2:
            this.OldShift = ((miRobotEditor.Controls.ShiftBox)(target));
            return;
            case 3:
            this.gNewValues = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 4:
            this.NewShift = ((miRobotEditor.Controls.ShiftBox)(target));
            return;
            case 5:
            this.gDifference = ((System.Windows.Controls.GroupBox)(target));
            return;
            case 6:
            this.Difference = ((miRobotEditor.Controls.ShiftBox)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}
