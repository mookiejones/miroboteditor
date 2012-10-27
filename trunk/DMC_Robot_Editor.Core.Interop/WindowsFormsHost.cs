using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing.Printing;
using System.Runtime.InteropServices;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Forms;
using System.Windows.Input;
using System.Windows.Interop;
using System.Windows.Media;
using System.Windows.Threading;
using Clipboard = System.Windows.Clipboard;

namespace miRobotEditor.Core
{
    /// <summary>
    /// WindowsFormsHost used in mirobotEditor.
    /// </summary>
    public class WindowsFormsHost : CustomWindowsFormsHost
    {
        /// <summary>
        /// Creates a new WindowsFormsHost instance.
        /// </summary>
        /// <param name="processShortcutsInWPF">
        /// Determines whether the shortcuts for the default actions (Cut,Copy,Paste,Undo, etc.)
        /// are processed by the WPF command system.
        /// The default value is false. Pass true only if WinForms does not handle those shortcuts by itself.
        /// See SD-1671 and SD-1737.
        /// </param>
        public WindowsFormsHost(bool processShortcutsInWPF = false)
        {
            DisposeChild = true;
            CreateBindings(processShortcutsInWPF);
        }

        #region Binding
        void CreateBindings(bool processShortcutsInWPF)
        {
            AddBinding(processShortcutsInWPF, ApplicationCommands.Copy, (IClipboardHandler c) => c.Copy(), c => c.EnableCopy);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Cut, (IClipboardHandler c) => c.Cut(), c => c.EnableCut);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Paste, (IClipboardHandler c) => c.Paste(), c => c.EnablePaste);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Delete, (IClipboardHandler c) => c.Delete(), c => c.EnableDelete);
            AddBinding(processShortcutsInWPF, ApplicationCommands.SelectAll, (IClipboardHandler c) => c.SelectAll(), c => c.EnableSelectAll);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Help, (IContextHelpProvider h) => h.ShowHelp(), h => true);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Undo, (IUndoHandler u) => u.Undo(), u => u.EnableUndo);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Redo, (IUndoHandler u) => u.Redo(), u => u.EnableRedo);
            AddBinding(processShortcutsInWPF, ApplicationCommands.Print, (IPrintable p) => WindowsFormsPrinting.Print(p), p => true);
            AddBinding(processShortcutsInWPF, ApplicationCommands.PrintPreview, (IPrintable p) => WindowsFormsPrinting.PrintPreview(p), p => true);
        }

        void AddBinding<T>(bool processShortcutsInWPF, ICommand command, Action<T> execute, Predicate<T> canExecute) where T : class
        {
            ExecutedRoutedEventHandler onExecuted = (sender, e) =>
            {
                if (e.Command == command)
                {
                    var cbh = GetInterface<T>();
                    if (cbh != null)
                    {
                        e.Handled = true;
                        if (canExecute(cbh))
                            execute(cbh);
                    }
                }
            };
            CanExecuteRoutedEventHandler onCanExecute = (sender, e) =>
                                                            {
                                                                if (e.Command != command) return;
                                                                var cbh = GetInterface<T>();
                                                                if (cbh == null) return;
                                                                e.Handled = true;
                                                                e.CanExecute = canExecute(cbh);
                                                            };
            if (processShortcutsInWPF)
            {
                CommandBindings.Add(new CommandBinding(command, onExecuted, onCanExecute));
            }
            else
            {
                // Don't use this.CommandBindings because CommandBindings with built-in shortcuts would handle the key press
                // before WinForms gets to see it. Using the events ensures that the command gets executed only when the user
                // clicks on the menu/toolbar item. (this fixes SD2-1671)
                CommandManager.AddCanExecuteHandler(this, onCanExecute);
                CommandManager.AddExecutedHandler(this, onExecuted);
            }
        }
        #endregion

        public override string ToString()
        {
            if (ServiceObject != null)
                return "[WindowsFormsHost " + Child + " for " + ServiceObject + "]";
            return "[WindowsFormsHost " + Child + "]";
        }

        #region Service Object
        /// <summary>
        /// Gets/Sets the object that implements the IClipboardHandler, IUndoHandler etc. interfaces...
        /// </summary>
        public object ServiceObject { get; set; }

        T GetInterface<T>() where T : class
        {
            T instance = ServiceObject as T ?? GetServiceWrapper(GetActiveControl()) as T;
            return instance;
        }

        System.Windows.Forms.Control GetActiveControl()
        {
            ContainerControl container = null;
            System.Windows.Forms.Control ctl = Child;
            while (ctl != null)
            {
                container = ctl as ContainerControl;
                if (container == null)
                    return ctl;
                ctl = container.ActiveControl;
            }
            return container;
        }

        static object GetServiceWrapper(System.Windows.Forms.Control ctl)
        {
            var tb = ctl as TextBoxBase;
            if (tb != null)
                return new TextBoxWrapper(tb);
            var cb = ctl as System.Windows.Forms.ComboBox;
            if (cb != null && cb.DropDownStyle != ComboBoxStyle.DropDownList)
                return new ComboBoxWrapper(cb);
            return ctl;
        }

        sealed class TextBoxWrapper : IClipboardHandler, IUndoHandler
        {
            TextBoxBase textBox;
            public TextBoxWrapper(TextBoxBase textBox)
            {
                this.textBox = textBox;
            }
            public bool EnableCut
            {
                get { return !textBox.ReadOnly && textBox.SelectionLength > 0; }
            }
            public bool EnableCopy
            {
                get { return textBox.SelectionLength > 0; }
            }
            public bool EnablePaste
            {
                get { return !textBox.ReadOnly && Clipboard.ContainsText(); }
            }
            public bool EnableDelete
            {
                get { return !textBox.ReadOnly && textBox.SelectionLength > 0; }
            }
            public bool EnableSelectAll
            {
                get { return textBox.TextLength > 0; }
            }
            public void Cut() { textBox.Cut(); }
            public void Copy() { textBox.Copy(); }
            public void Paste() { textBox.Paste(); }
            public void Delete() { textBox.SelectedText = ""; }
            public void SelectAll() { textBox.SelectAll(); }

            public bool EnableUndo { get { return textBox.CanUndo; } }
            public bool EnableRedo { get { return false; } }

            public void Undo()
            {
                textBox.Undo();
            }

            public void Redo()
            {
            }
        }

        sealed class ComboBoxWrapper : IClipboardHandler
        {
            System.Windows.Forms.ComboBox comboBox;
            public ComboBoxWrapper(System.Windows.Forms.ComboBox comboBox)
            {
                this.comboBox = comboBox;
            }
            public bool EnableCut
            {
                get { return comboBox.SelectionLength > 0; }
            }
            public bool EnableCopy
            {
                get { return comboBox.SelectionLength > 0; }
            }
            public bool EnablePaste
            {
                get { return Clipboard.ContainsText(); }
            }
            public bool EnableDelete
            {
                get { return true; }
            }
            public bool EnableSelectAll
            {
                get { return comboBox.Text.Length > 0; }
            }
            public void Cut() { ClipboardWrapper.SetText(comboBox.SelectedText); comboBox.SelectedText = ""; }
            public void Copy() { ClipboardWrapper.SetText(comboBox.SelectedText); }
            public void Paste() { comboBox.SelectedText = ClipboardWrapper.GetText(); }
            public void Delete() { comboBox.SelectedText = ""; }
            public void SelectAll() { comboBox.SelectAll(); }
        }
        #endregion

        /// <summary>
        /// Gets/Sets whether the windows forms control will be disposed
        /// when the WindowsFormsHost is disposed.
        /// The default value is true.
        /// </summary>
        /// <remarks>
        /// The default WindowsFormsHost disposes its child when the WPF application shuts down,
        /// but some events in SharpDevelop occur after the WPF shutdown (e.g. SolutionClosed), so we must
        /// not dispose pads that could still be handling them.
        /// </remarks>
        public bool DisposeChild { get; set; }

        public bool IsDisposed { get; private set; }

        protected override void Dispose(bool disposing)
        {
            if (disposing && !DisposeChild && Child != null)
            {
                // prevent child from being disposed
                Child = null;
            }
            IsDisposed = disposing;
            base.Dispose(disposing);
        }
    }
    /// <summary>
    /// A custom Windows Forms Host implementation.
    /// Hopefully fixes SD-1842 - ArgumentException in SetActiveControlInternal (WindowsFormsHost.RestoreFocusedChild)
    /// </summary>
    [Localizable(false)]
    public class CustomWindowsFormsHost : HwndHost, IKeyboardInputSink
    {
        // Interactions of the MS WinFormsHost:
        //   IME
        //   Font sync
        //   Property sync
        //   Background sync (rendering bitmaps!)
        //   Access keys
        //   Tab Navigation
        //   Save/Restore focus for app switch
        //   Size feedback (WinForms control tells WPF desired size)
        //   Focus enter/leave - validation events
        //   ...

        // We don't need most of that.

        // Bugs in our implementation:
        //  - Slight background color mismatch in project options

        static class Win32
        {
            [DllImport("user32.dll")]
            public static extern IntPtr GetFocus();

            [DllImport("user32.dll")]
            [return: MarshalAs(UnmanagedType.Bool)]
            public static extern bool IsChild(IntPtr hWndParent, IntPtr hwnd);

            [DllImport("user32.dll")]
            internal static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

            [DllImport("user32.dll")]
            internal static extern IntPtr SetParent(IntPtr hWnd, IntPtr hWndParent);
        }

        #region Remember/RestoreFocusedChild
        sealed class HostNativeWindow : NativeWindow
        {
            readonly CustomWindowsFormsHost host;

            public HostNativeWindow(CustomWindowsFormsHost host)
            {
                this.host = host;
            }

            protected override void WndProc(ref Message m)
            {
                const int WM_ACTIVATEAPP = 0x1C;
                if (m.Msg == WM_ACTIVATEAPP)
                {
                    if (m.WParam == IntPtr.Zero)
                    {
                        // The window is being deactivated:
                        // If a WinForms control within this host has focus, remember it.
                        IntPtr focus = Win32.GetFocus();
                        if (focus == host.Handle || Win32.IsChild(host.Handle, focus))
                        {
                            host.RememberActiveControl();
                            host.Log("Window deactivated; RememberActiveControl(): " + host.savedActiveControl);
                        }
                        else
                        {
                            host.Log("Window deactivated; but focus not within WinForms");
                        }
                    }
                    else
                    {
                        // The window is being activated.
                        host.Log("Window activated");
                        host.Dispatcher.BeginInvoke(
                            DispatcherPriority.Normal,
                            new Action(host.RestoreActiveControl));
                    }
                }
                base.WndProc(ref m);
            }
        }

        HostNativeWindow hostNativeWindow;
        System.Windows.Forms.Control savedActiveControl;

        void RememberActiveControl()
        {
            savedActiveControl = container.ActiveControl;
        }

        void RestoreActiveControl()
        {
            if (savedActiveControl != null)
            {
                Log("RestoreActiveControl(): " + savedActiveControl);
                savedActiveControl.Focus();
                savedActiveControl = null;
            }
        }
        #endregion

        #region Container
        sealed class HostedControlContainer : ContainerControl
        {
            System.Windows.Forms.Control child;

            protected override void OnHandleCreated(System.EventArgs e)
            {
                base.OnHandleCreated(e);
                const int WM_UPDATEUISTATE = 0x0128;
                const int UISF_HIDEACCEL = 2;
                const int UISF_HIDEFOCUS = 1;
                const int UIS_SET = 1;
                Win32.SendMessage(Handle, WM_UPDATEUISTATE, new IntPtr(UISF_HIDEACCEL | UISF_HIDEFOCUS | (UIS_SET << 16)), IntPtr.Zero);
            }

            public System.Windows.Forms.Control Child
            {
                get { return child; }
                set
                {
                    if (child != null)
                    {
                        Controls.Remove(child);
                    }
                    child = value;
                    if (value != null)
                    {
                        value.Dock = DockStyle.Fill;
                        Controls.Add(value);
                    }
                }
            }
        }
        #endregion

        readonly HostedControlContainer container;

        #region Constructors
        /// <summary>
        /// Creates a new CustomWindowsFormsHost instance.
        /// </summary>
        public CustomWindowsFormsHost()
        {
            container = new HostedControlContainer();
            Init();
        }

        /// <summary>
        /// Creates a new CustomWindowsFormsHost instance that allows hosting controls
        /// from the specified AppDomain.
        /// </summary>
        public CustomWindowsFormsHost(AppDomain childDomain)
        {
            var type = typeof(HostedControlContainer);
            if (type.FullName != null)
                container = (HostedControlContainer)childDomain.CreateInstanceAndUnwrap(type.Assembly.FullName, type.FullName);
            Init();
        }

        void Init()
        {
            EnableFontInheritance = true;
            Loaded += OnLoaded;
            Unloaded += OnUnloaded;
            Log("Instance created");
        }

        void OnLoaded(object sender, RoutedEventArgs e)
        {
            Log("OnLoaded()");
            SetFont();
            if (hwndParent.Handle != IntPtr.Zero && hostNativeWindow != null)
            {
                if (hostNativeWindow.Handle == IntPtr.Zero)
                    hostNativeWindow.AssignHandle(hwndParent.Handle);
            }
        }

        void OnUnloaded(object sender, RoutedEventArgs e)
        {
            Log("OnUnloaded()");
            if (hostNativeWindow != null)
            {
                savedActiveControl = null;
                hostNativeWindow.ReleaseHandle();
            }
        }
        #endregion

        #region Font Synchronization
        protected override void OnPropertyChanged(DependencyPropertyChangedEventArgs e)
        {
            base.OnPropertyChanged(e);
            if (e.Property == TextBlock.FontFamilyProperty || e.Property == TextBlock.FontSizeProperty)
            {
                SetFont();
            }
        }

        public bool EnableFontInheritance { get; set; }

        void SetFont()
        {
            if (!EnableFontInheritance)
                return;
            string fontFamily = TextBlock.GetFontFamily(this).Source;
            float fontSize = (float)(TextBlock.GetFontSize(this) * (72.0 / 96.0));
            container.Font = new System.Drawing.Font(fontFamily, fontSize, System.Drawing.FontStyle.Regular);
        }
        #endregion

        public System.Windows.Forms.Control Child
        {
            get { return container.Child; }
            set { container.Child = value; }
        }

        protected override Size MeasureOverride(Size constraint)
        {
            return new Size(0, 0);
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            container.Size = new System.Drawing.Size((int)finalSize.Width, (int)finalSize.Height);
            return finalSize;
        }

        HandleRef hwndParent;

        protected override HandleRef BuildWindowCore(HandleRef _hwndParent)
        {
            Log("BuildWindowCore");
            if (hostNativeWindow != null)
            {
                hostNativeWindow.ReleaseHandle();
            }
            else
            {
                hostNativeWindow = new HostNativeWindow(this);
            }
            hwndParent = _hwndParent;
            hostNativeWindow.AssignHandle(hwndParent.Handle);

            IntPtr childHandle = container.Handle;
            Win32.SetParent(childHandle, hwndParent.Handle);
            return new HandleRef(container, childHandle);
        }

        protected override void DestroyWindowCore(HandleRef hwnd)
        {
            Log("DestroyWindowCore");
            hostNativeWindow.ReleaseHandle();
            savedActiveControl = null;
            hwndParent = default(HandleRef);
        }

        protected override void Dispose(bool disposing)
        {
            Log("Dispose (disposing=" + disposing + ")");
            base.Dispose(disposing);
            if (disposing)
            {
                container.Dispose();
            }
        }

        protected override IntPtr WndProc(IntPtr hwnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case 0x0007: // WM_SETFOCUS
                case 0x0021: // WM_MOUSEACTIVATE
                    // Give the WindowsFormsHost logical focus:
                    DependencyObject focusScope = this;
                    while (focusScope != null && !FocusManager.GetIsFocusScope(focusScope))
                    {
                        focusScope = VisualTreeHelper.GetParent(focusScope);
                    }
                    if (focusScope != null)
                    {
                        FocusManager.SetFocusedElement(focusScope, this);
                    }
                    break;
            }
            return base.WndProc(hwnd, msg, wParam, lParam, ref handled);
        }

#if DEBUG
        static int hostCount;
        int instanceID = System.Threading.Interlocked.Increment(ref hostCount);
#endif

        [Conditional("DEBUG")]
        void Log(string text)
        {
#if DEBUG
            Debug.WriteLine("CustomWindowsFormsHost #{0}: {1}", instanceID, text);
#endif
        }
    }
    /// <summary>
    /// Clipboard Handler
    /// </summary>
    public interface IClipboardHandler
    {
        /// <summary>
        /// Enable Cut
        /// </summary>
        bool EnableCut
        {
            get;

        }
        /// <summary>
        /// Enable Copy
        /// </summary>
        bool EnableCopy
        {
            get;
        }
        /// <summary>
        /// Enable Paste
        /// </summary>
        bool EnablePaste
        {
            get;
        }
        /// <summary>
        /// Enable Delete
        /// </summary>
        bool EnableDelete
        {
            get;
        }
        /// <summary>
        /// Enable Select all
        /// </summary>
        bool EnableSelectAll
        {
            get;
        }
        /// <summary>
        /// Cut
        /// </summary>
        void Cut();
        /// <summary>
        /// Copy
        /// </summary>
        void Copy();
        /// <summary>
        /// Paste
        /// </summary>
        void Paste();
        /// <summary>
        /// Delete
        /// </summary>
        void Delete();
        /// <summary>
        /// Select All
        /// </summary>
        void SelectAll();
    }
    /// <summary>
    /// This interface is meant for Windows-Forms AddIns to preserve the context help handling functionality as in SharpDevelop 3.0.
    /// It works only for controls inside a <see cref="WindowsFormsHost"/>.
    /// WPF AddIns should handle the routed command 'Help' instead.
    /// </summary>
    public interface IContextHelpProvider
    {
        /// <summary>
        /// Show Help
        /// </summary>
        void ShowHelp();
    }
    /// <summary>
    /// This interface is meant for Windows-Forms AddIns to preserve the undo handling functionality as in SharpDevelop 3.0.
    /// It works only for controls inside a <see cref="WindowsFormsHost"/>.
    /// WPF AddIns should handle the routed commands 'Undo' and 'Redo' instead.
    /// </summary>
    public interface IUndoHandler
    {
        /// <summary>
        /// Enable Undo
        /// </summary>
        bool EnableUndo
        {
            get;
        }
        /// <summary>
        /// Enable Redo
        /// </summary>
        bool EnableRedo
        {
            get;
        }

        /// <summary>
        /// Undo
        /// </summary>
        void Undo();
        /// <summary>
        /// Redo
        /// </summary>
        void Redo();
    }
    /// <summary>
    /// This interface is meant for Windows-Forms AddIns to preserve the printing functionality as in SharpDevelop 3.0.
    /// It works only for controls inside a <see cref="WindowsFormsHost"/>.
    /// WPF AddIns should handle the routed commands 'Print' and 'PrintPreview' instead.
    /// 
    /// If a IViewContent object is from the type IPrintable it signals
    /// that it's contents could be printed to a printer, fax etc.
    /// </summary>
    public interface IPrintable
    {
        /// <summary>
        /// Returns the PrintDocument for this object, see the .NET reference
        /// for more information about printing.
        /// </summary>
        PrintDocument PrintDocument
        {
            get;
        }
    }
}
