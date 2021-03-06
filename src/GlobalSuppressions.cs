// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Code Analysis results, point to "Suppress Message", and click 
// "In Suppression File".
// You do not need to add suppressions to this file manually.

using System.Diagnostics.CodeAnalysis;

[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.Forms.FrmSplashScreen.UpdateStatusTextWithStatus(System.String,miRobotEditor.Forms.TypeOfMessage)",
        Scope = "member",
        Target =
            "miRobotEditor.Languages.AbstractLanguageClass.#ShiftProgram(miRobotEditor.GUI.Editor,miRobotEditor.Forms.FrmShift)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Console.WriteLine(System.String,System.Object)", Scope = "member",
        Target =
            "miRobotEditor.Languages.AbstractLanguageClass.#ShiftProgram(miRobotEditor.GUI.Editor,miRobotEditor.Forms.FrmShift)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member",
        Target =
            "miRobotEditor.App.#AppDispatcherUnhandledException(System.Object,System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.MessageBox.Show(System.String,System.String)", Scope = "member",
        Target =
            "miRobotEditor.App.#AppDispatcherUnhandledException(System.Object,System.Windows.Threading.DispatcherUnhandledExceptionEventArgs)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.GUI.DummyDoc.#OnPropertyChanged(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "ICSharpCode.AvalonEdit.Document.TextDocument.Insert(System.Int32,System.String)", Scope = "member",
        Target = "miRobotEditor.GUI.Editor.#ChangeIndent(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.GUI.Editor.#ChangeIndent(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.MessageBox.Show(System.String,System.String,System.Windows.MessageBoxButton,System.Windows.MessageBoxImage)",
        Scope = "member", Target = "miRobotEditor.GUI.Editor.#Reload()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "ICSharpCode.AvalonEdit.Snippets.SnippetTextElement.set_Text(System.String)", Scope = "member",
        Target = "miRobotEditor.GUI.Editor.#InsertSnippet(System.Object,System.Windows.Input.KeyEventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member",
        Target =
            "miRobotEditor.GUI.ExplorerControl.ExplorerClass.#FillTreeNode(System.Windows.Forms.TreeNode,System.String)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.GUI.ExplorerControl.ExplorerClass.#ShowTree()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.ToolStripItem.set_Text(System.String)", Scope = "member",
        Target = "miRobotEditor.GUI.ExplorerControl.FileExplorerControl.#InitializeComponent()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.Pads.FileInfo.#GetFileType()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.MessageBox.Show(System.String,System.String)", Scope = "member",
        Target = "miRobotEditor.Pads.FileInfo.#ToString()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.Classes.LogWriter.WriteLog(System.String)", Scope = "member",
        Target = "miRobotEditor.Classes.LogWriter.#.ctor()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.MessageBox.Show(System.String,System.String,System.Windows.MessageBoxButton,System.Windows.MessageBoxImage)",
        Scope = "member", Target = "miRobotEditor.Workspace.#QueryCloseApplicationWhenDocumentsModified()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.MessageBox.Show(System.String,System.String,System.Windows.MessageBoxButton,System.Windows.MessageBoxImage)",
        Scope = "member", Target = "miRobotEditor.Workspace.#QueryCloseModifiedDocument(miRobotEditor.GUI.DummyDoc)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.MessageBox.Show(System.String,System.String,System.Windows.MessageBoxButton,System.Windows.MessageBoxImage)",
        Scope = "member", Target = "miRobotEditor.Workspace.#ErrorMessage(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.ViewModel.MessageViewModel.Add(System.String,System.String,miRobotEditor.ViewModel.MSGIcon,System.Boolean)",
        Scope = "member", Target = "miRobotEditor.Workspace.#AddTool(System.Object)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.ViewModel.MessageViewModel.Add(System.String,System.String,miRobotEditor.ViewModel.MSGIcon,System.Boolean)",
        Scope = "member",
        Target = "miRobotEditor.Workspace.#OnDocumentClosing(System.Object,AvalonDock.DocumentClosingEventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Console.WriteLine(System.String)", Scope = "member",
        Target =
            "miRobotEditor.Converters.WidthConverter.#Convert(System.Object,System.Type,System.Object,System.Globalization.CultureInfo)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.ViewModel.ViewModelBase.#RaisePropertyChanged(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Console.WriteLine(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ViewModelBase.#RaisePropertyChanged(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target =
            "miRobotEditor.GUI.AngleConverter.Vector3D.#miRobotEditor.GUI.AngleConverter.IGeometricElement3D.Position")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.ViewModel.MessageViewModel.Add(System.String,System.String,miRobotEditor.ViewModel.MSGIcon,System.Boolean)",
        Scope = "member",
        Target =
            "miRobotEditor.Classes.VariableBase.#GetVariables(System.String,System.Text.RegularExpressions.Regex,System.String)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member",
        Target =
            "miRobotEditor.Languages.AbstractLanguageClass.#CreateFoldingHelper(ICSharpCode.AvalonEdit.Document.ITextSource,System.String,System.String,System.Boolean)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "ICSharpCode.AvalonEdit.Snippets.SnippetTextElement.set_Text(System.String)", Scope = "member",
        Target = "miRobotEditor.Languages.KUKA.#forSnippet")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.Language_Specific.KUKAKFDHelper.#IsNumber(System.String&)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.ViewModel.MessageViewModel.Add(System.String,System.String,System.Windows.Media.Imaging.BitmapImage,System.Boolean)",
        Scope = "member", Target = "miRobotEditor.Language_Specific.KUKAKFDHelper.#LoadBasisFiles()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member",
        Target = "miRobotEditor.Language_Specific.KUKAKFDHelper.#CheckPar(System.String,System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.Language_Specific.KUKAKFDHelper.#LoadBasisFiles()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Title(System.String)", Scope = "member",
        Target = "miRobotEditor.Language_Specific.LongTextViewModel.#GetAllLangtextFromDatabase()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member",
        Target = "miRobotEditor.Language_Specific.LongTextViewModel.#GetAllLangtextFromDatabase()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.Forms.MessageBox.Show(System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxIcon)",
        Scope = "member",
        Target = "miRobotEditor.Language_Specific.LongTextViewModel.#ImportFile(System.String,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member",
        Target = "miRobotEditor.Language_Specific.LongTextViewModel.#Open()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.Languages.KUKA+FunctionGenerator.#GetSystemFunctions()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.Language_Specific.KUKAKFDHelper.#GetPar(System.String,System.String)")
]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target = "miRobotEditor.Languages.LanguageBase.#ShiftRegex")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target = "miRobotEditor.Languages.LanguageBase.#CommentChar")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target =
            "miRobotEditor.GUI.AngleConverter.Line3D.#miRobotEditor.GUI.AngleConverter.IGeometricElement3D.Position")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member",
        Target = "miRobotEditor.Language_Specific.LongTextViewModel.#Import()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.ViewModel.MessageViewModel.Add(System.String,System.String,miRobotEditor.ViewModel.MSGIcon,System.Boolean)",
        Scope = "member", Target = "miRobotEditor.MainWindow.#DropFiles(System.Object,System.Windows.DragEventArgs)")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly", Scope = "member",
        Target = "miRobotEditor.ViewModel.MessageViewModel.#Messages")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.ViewModel.MessageViewModel.Add(System.String,System.String,miRobotEditor.ViewModel.MSGIcon,System.Boolean)",
        Scope = "member", Target = "miRobotEditor.ViewModel.ObjectBrowserViewModel.#ShowWizard(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FolderBrowserDialog.set_Description(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ObjectBrowserViewModel.#GetDirectory()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target =
            "miRobotEditor.GUI.AngleConverter.Plane3D.#miRobotEditor.GUI.AngleConverter.IGeometricElement3D.Position")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target = "miRobotEditor.ValidationRules.IsNumericValidationRule.#Error")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target = "miRobotEditor.ValidationRules.IsNumericValidationRule.#Item[System.String]")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target = "miRobotEditor.Languages.Kawasaki.#ShiftRegex")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Console.WriteLine(System.String,System.Object)", Scope = "member",
        Target =
            "miRobotEditor.Languages.AbstractLanguageClass.#ShiftProgram(miRobotEditor.GUI.Editor,miRobotEditor.ViewModel.ShiftViewModel)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "miRobotEditor.Forms.FrmSplashScreen.UpdateStatusTextWithStatus(System.String,miRobotEditor.Forms.TypeOfMessage)",
        Scope = "member",
        Target =
            "miRobotEditor.Languages.AbstractLanguageClass.#ShiftProgram(miRobotEditor.GUI.Editor,miRobotEditor.ViewModel.ShiftViewModel)"
        )]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Title(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#Import()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#Import()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Console.WriteLine(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#ReadZip()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.Forms.MessageBox.Show(System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxIcon,System.Windows.Forms.MessageBoxDefaultButton)",
        Scope = "member", Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#CheckPathExists(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#GetSignalsFromDataBase()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.Forms.MessageBox.Show(System.String,System.String,System.Windows.Forms.MessageBoxButtons,System.Windows.Forms.MessageBoxIcon)",
        Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#ImportFile(System.String,System.Boolean)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Title(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#GetSignalsFromDataBase()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "System.Windows.Forms.FileDialog.set_Filter(System.String)", Scope = "member",
        Target = "miRobotEditor.ViewModel.ArchiveInfoViewModel.#Open()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1065:DoNotRaiseExceptionsInUnexpectedLocations", Scope = "member",
        Target = "miRobotEditor.Languages.Fanuc.#ShiftRegex")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "type",
        Target = "miRobotEditor.Interfaces.IconBarMargin")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1063:ImplementIDisposableCorrectly", Scope = "member",
        Target = "miRobotEditor.Interfaces.IconBarMargin.#Dispose()")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.LogWriter.WriteLog(System.String)", Scope = "member",
        Target = "miRobotEditor.LogWriter.#.ctor()")]
[assembly:
    SuppressMessage("Microsoft.Design", "CA1008:EnumsShouldHaveZeroValue", Scope = "type",
        Target = "miRobotEditor.GUI.AngleConverter.Robot.ModelType")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.MessageBox.Show(System.String,System.String,System.Windows.MessageBoxButton,System.Windows.MessageBoxImage)",
        Scope = "member", Target = "miRobotEditor.Classes.RecentFileList.#PromptForDelete(System.String)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "miRobotEditor.ViewModel.MessageViewModel.AddError(System.String,System.Exception)",
        Scope = "member", Target = "miRobotEditor.Classes.RecentFileList+ApplicationAttributes.#.cctor()")]
[assembly:
    SuppressMessage("Microsoft.Usage", "CA2202:Do not dispose objects multiple times", Scope = "member",
        Target = "miRobotEditor.GUI.TextEditorOptions.#Register(System.String,System.String[])")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId =
            "System.Windows.MessageBox.Show(System.String,System.String,System.Windows.MessageBoxButton,System.Windows.MessageBoxImage)",
        Scope = "member",
        Target = "miRobotEditor.Workspace.#QueryCloseModifiedDocument(miRobotEditor.ViewModel.IDocument)")]
[assembly:
    SuppressMessage("Microsoft.Globalization", "CA1303:Do not pass literals as localized parameters",
        MessageId = "ICSharpCode.AvalonEdit.Snippets.SnippetTextElement.set_Text(System.String)", Scope = "member",
        Target = "miRobotEditor.GUI.Editor.#InsertSnippet()")]