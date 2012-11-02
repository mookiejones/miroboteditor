﻿#pragma checksum "..\..\MainWindow.xaml" "{406ea660-64cf-4c82-b6f0-42d48172a799}" "2609F4E1CAFFE41227B9DA284A1485E0"
//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.17929
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

using AvalonDock;
using AvalonDock.Controls;
using AvalonDock.Converters;
using AvalonDock.Layout;
using AvalonDock.Themes;
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
using miRobotEditor.Classes;
using miRobotEditor.Controls;
using miRobotEditor.Controls.ExplorerControl;
using miRobotEditor.GUI.AngleConverter;
using miRobotEditor.GUI.Editor;
using miRobotEditor.GUI.FunctionWindow;
using miRobotEditor.GUI.ObjectBrowser;
using miRobotEditor.GUI.OutputWindow;


namespace miRobotEditor {
    
    
    /// <summary>
    /// MainWindow
    /// </summary>
    public partial class MainWindow : System.Windows.Window, System.Windows.Markup.IComponentConnector {
        
        
        #line 75 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.Menu menu;
        
        #line default
        #line hidden
        
        
        #line 100 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuFile;
        
        #line default
        #line hidden
        
        
        #line 103 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuNew;
        
        #line default
        #line hidden
        
        
        #line 112 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuOpen;
        
        #line default
        #line hidden
        
        
        #line 122 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuSave;
        
        #line default
        #line hidden
        
        
        #line 132 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuSaveAs;
        
        #line default
        #line hidden
        
        
        #line 143 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuReloadFile;
        
        #line default
        #line hidden
        
        
        #line 149 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuImport;
        
        #line default
        #line hidden
        
        
        #line 153 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuPrint;
        
        #line default
        #line hidden
        
        
        #line 163 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuPrintPreview;
        
        #line default
        #line hidden
        
        
        #line 181 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.Classes.RecentFileList RecentFileList;
        
        #line default
        #line hidden
        
        
        #line 192 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Edit;
        
        #line default
        #line hidden
        
        
        #line 195 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Undo;
        
        #line default
        #line hidden
        
        
        #line 279 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Formatting;
        
        #line default
        #line hidden
        
        
        #line 342 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem FindAndReplace;
        
        #line default
        #line hidden
        
        
        #line 399 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem View;
        
        #line default
        #line hidden
        
        
        #line 434 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem ViewAs;
        
        #line default
        #line hidden
        
        
        #line 454 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Tools;
        
        #line default
        #line hidden
        
        
        #line 464 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Window;
        
        #line default
        #line hidden
        
        
        #line 497 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuRobot;
        
        #line default
        #line hidden
        
        
        #line 500 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuKUKA;
        
        #line default
        #line hidden
        
        
        #line 510 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem mnuABB;
        
        #line default
        #line hidden
        
        
        #line 524 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.MenuItem Help;
        
        #line default
        #line hidden
        
        
        #line 613 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal System.Windows.Controls.ToolBar EditorFormatting;
        
        #line default
        #line hidden
        
        
        #line 655 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal AvalonDock.DockingManager dockManager;
        
        #line default
        #line hidden
        
        
        #line 668 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.Controls.ExplorerControl.ExplorerControlWPF _explorer;
        
        #line default
        #line hidden
        
        
        #line 691 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.GUI.FunctionWindow.FunctionWindow Functions;
        
        #line default
        #line hidden
        
        
        #line 697 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.GUI.AngleConverter.AngleConverterWPF _angleconverter;
        
        #line default
        #line hidden
        
        
        #line 709 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal AvalonDock.Layout.LayoutAnchorable OutputHandler;
        
        #line default
        #line hidden
        
        
        #line 712 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.GUI.OutputWindow.OutputWindow Output;
        
        #line default
        #line hidden
        
        
        #line 722 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.Controls.frmNotes notes;
        
        #line default
        #line hidden
        
        
        #line 728 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal AvalonDock.Layout.LayoutAnchorable tabObjectBrowser;
        
        #line default
        #line hidden
        
        
        #line 735 "..\..\MainWindow.xaml"
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields")]
        internal miRobotEditor.GUI.ObjectBrowser.ObjectBrowserTool objectBrowser;
        
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
            System.Uri resourceLocater = new System.Uri("/miRobotEditor;component/mainwindow.xaml", System.UriKind.Relative);
            
            #line 1 "..\..\MainWindow.xaml"
            System.Windows.Application.LoadComponent(this, resourceLocater);
            
            #line default
            #line hidden
        }
        
        [System.Diagnostics.DebuggerNonUserCodeAttribute()]
        [System.CodeDom.Compiler.GeneratedCodeAttribute("PresentationBuildTasks", "4.0.0.0")]
        [System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal System.Delegate _CreateDelegate(System.Type delegateType, string handler) {
            return System.Delegate.CreateDelegate(delegateType, this, handler);
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
            
            #line 6 "..\..\MainWindow.xaml"
            ((miRobotEditor.MainWindow)(target)).Closing += new System.ComponentModel.CancelEventHandler(this.WindowClosing);
            
            #line default
            #line hidden
            
            #line 8 "..\..\MainWindow.xaml"
            ((miRobotEditor.MainWindow)(target)).Loaded += new System.Windows.RoutedEventHandler(this.WindowLoaded);
            
            #line default
            #line hidden
            
            #line 12 "..\..\MainWindow.xaml"
            ((miRobotEditor.MainWindow)(target)).Drop += new System.Windows.DragEventHandler(this.DropFiles);
            
            #line default
            #line hidden
            
            #line 13 "..\..\MainWindow.xaml"
            ((miRobotEditor.MainWindow)(target)).DragEnter += new System.Windows.DragEventHandler(this.onDragEnter);
            
            #line default
            #line hidden
            
            #line 14 "..\..\MainWindow.xaml"
            ((miRobotEditor.MainWindow)(target)).PreviewKeyUp += new System.Windows.Input.KeyEventHandler(this.ManageKeys);
            
            #line default
            #line hidden
            return;
            case 2:
            
            #line 26 "..\..\MainWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanImport);
            
            #line default
            #line hidden
            
            #line 27 "..\..\MainWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.ImportRobot);
            
            #line default
            #line hidden
            return;
            case 3:
            
            #line 30 "..\..\MainWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).CanExecute += new System.Windows.Input.CanExecuteRoutedEventHandler(this.CanClose);
            
            #line default
            #line hidden
            
            #line 31 "..\..\MainWindow.xaml"
            ((System.Windows.Input.CommandBinding)(target)).Executed += new System.Windows.Input.ExecutedRoutedEventHandler(this.CloseWindow);
            
            #line default
            #line hidden
            return;
            case 4:
            this.menu = ((System.Windows.Controls.Menu)(target));
            return;
            case 5:
            this.mnuFile = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 6:
            this.mnuNew = ((System.Windows.Controls.MenuItem)(target));
            
            #line 105 "..\..\MainWindow.xaml"
            this.mnuNew.Click += new System.Windows.RoutedEventHandler(this.AddNewFile);
            
            #line default
            #line hidden
            return;
            case 7:
            this.mnuOpen = ((System.Windows.Controls.MenuItem)(target));
            
            #line 115 "..\..\MainWindow.xaml"
            this.mnuOpen.Click += new System.Windows.RoutedEventHandler(this.OpenFile);
            
            #line default
            #line hidden
            return;
            case 8:
            this.mnuSave = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 9:
            this.mnuSaveAs = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 10:
            this.mnuReloadFile = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 11:
            this.mnuImport = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 12:
            this.mnuPrint = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 13:
            this.mnuPrintPreview = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 14:
            this.RecentFileList = ((miRobotEditor.Classes.RecentFileList)(target));
            return;
            case 15:
            
            #line 184 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ExitClick);
            
            #line default
            #line hidden
            return;
            case 16:
            this.Edit = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 17:
            this.Undo = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 18:
            this.Formatting = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 19:
            this.FindAndReplace = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 20:
            this.View = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 21:
            this.ViewAs = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 22:
            
            #line 438 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeViewAs);
            
            #line default
            #line hidden
            return;
            case 23:
            
            #line 441 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeViewAs);
            
            #line default
            #line hidden
            return;
            case 24:
            
            #line 444 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeViewAs);
            
            #line default
            #line hidden
            return;
            case 25:
            
            #line 447 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeViewAs);
            
            #line default
            #line hidden
            return;
            case 26:
            
            #line 450 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ChangeViewAs);
            
            #line default
            #line hidden
            return;
            case 27:
            this.Tools = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 28:
            
            #line 458 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 29:
            
            #line 461 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.ShowOptions);
            
            #line default
            #line hidden
            return;
            case 30:
            this.Window = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 31:
            
            #line 470 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 32:
            
            #line 473 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 33:
            
            #line 476 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 34:
            
            #line 479 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 35:
            
            #line 482 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 36:
            
            #line 485 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.AddTool);
            
            #line default
            #line hidden
            return;
            case 37:
            
            #line 489 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.MenuItem)(target)).Click += new System.Windows.RoutedEventHandler(this.SplitWindow);
            
            #line default
            #line hidden
            return;
            case 38:
            this.mnuRobot = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 39:
            this.mnuKUKA = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 40:
            this.mnuABB = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 41:
            this.Help = ((System.Windows.Controls.MenuItem)(target));
            return;
            case 42:
            
            #line 559 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.AddNewFile);
            
            #line default
            #line hidden
            return;
            case 43:
            
            #line 564 "..\..\MainWindow.xaml"
            ((System.Windows.Controls.Button)(target)).Click += new System.Windows.RoutedEventHandler(this.OpenFile);
            
            #line default
            #line hidden
            return;
            case 44:
            this.EditorFormatting = ((System.Windows.Controls.ToolBar)(target));
            return;
            case 45:
            this.dockManager = ((AvalonDock.DockingManager)(target));
            
            #line 656 "..\..\MainWindow.xaml"
            this.dockManager.DocumentClosing += new System.EventHandler<AvalonDock.DocumentClosingEventArgs>(this.OnDocumentClosing);
            
            #line default
            #line hidden
            
            #line 658 "..\..\MainWindow.xaml"
            this.dockManager.ActiveContentChanged += new System.EventHandler(this.OnActiveContentChanged);
            
            #line default
            #line hidden
            return;
            case 46:
            this._explorer = ((miRobotEditor.Controls.ExplorerControl.ExplorerControlWPF)(target));
            return;
            case 47:
            this.Functions = ((miRobotEditor.GUI.FunctionWindow.FunctionWindow)(target));
            return;
            case 48:
            this._angleconverter = ((miRobotEditor.GUI.AngleConverter.AngleConverterWPF)(target));
            return;
            case 49:
            this.OutputHandler = ((AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 50:
            this.Output = ((miRobotEditor.GUI.OutputWindow.OutputWindow)(target));
            return;
            case 51:
            this.notes = ((miRobotEditor.Controls.frmNotes)(target));
            return;
            case 52:
            this.tabObjectBrowser = ((AvalonDock.Layout.LayoutAnchorable)(target));
            return;
            case 53:
            this.objectBrowser = ((miRobotEditor.GUI.ObjectBrowser.ObjectBrowserTool)(target));
            return;
            }
            this._contentLoaded = true;
        }
    }
}

