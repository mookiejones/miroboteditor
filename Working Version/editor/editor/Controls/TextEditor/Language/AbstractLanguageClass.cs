﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using CommunityToolkit.Mvvm.DependencyInjection;
using CommunityToolkit.Mvvm.Messaging;
using ICSharpCode.AvalonEdit.CodeCompletion;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Classes;
using miRobotEditor.Controls.TextEditor.Folding;
using miRobotEditor.Enums;
using miRobotEditor.Interfaces;
using miRobotEditor.Languages;
using miRobotEditor.Messages;
using miRobotEditor.Position;
using miRobotEditor.Variables;
using miRobotEditor.ViewModel;

namespace miRobotEditor.Controls.TextEditor.Language
{
    [Localizable(false)]
    public abstract class AbstractLanguageClass : ViewModelBase, ILanguageRegex
    {
        public const string RootPathPropertyName = "RootPath";
        public const string FileNamePropertyName = "FileName";
        public const string RobotMenuItemsPropertyName = "RobotMenuItems";
        public const string BWProgressPropertyName = "BWProgress";
        public const string BWFilesMinPropertyName = "BWFilesMin";
        public const string BWFilesMaxPropertyName = "BWFilesMax";
        public const string BWProgressVisibilityPropertyName = "BWProgressVisibility";
        internal readonly List<IVariable> _allVariables = new List<IVariable>();
        private readonly List<IVariable> _enums = new List<IVariable>();

        private readonly ObservableCollection<MenuItem> _menuItems = new ObservableCollection<MenuItem>();
        private readonly ObservableCollection<IVariable> _objectBrowserVariables = new ObservableCollection<IVariable>();
        private readonly ReadOnlyCollection<IVariable> _readOnlyAllVariables = null;
        private readonly ReadOnlyObservableCollection<IVariable> _readOnlyBrowserVariables = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlyFields = null;

        private readonly ReadOnlyCollection<IVariable> _readOnlyFunctions = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlyenums = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlypositions = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlysignals = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlystructures = null;
        private readonly ReadOnlyObservableCollection<MenuItem> _readonlyMenuItems = null;
        private readonly List<IVariable> _signals = new List<IVariable>();
        private readonly List<IVariable> _structures = new List<IVariable>();
        private int _bwFilesMax;
        private int _bwFilesMin;
        private int _bwProgress;
        private Visibility _bwProgressVisibility = Visibility.Collapsed;
        private readonly List<IVariable> _fields = new List<IVariable>();
        private string _filename = string.Empty;
        private readonly List<IVariable> _functions = new List<IVariable>();
        private IOViewModel _ioModel;
        private string _kukaCon;
        private readonly List<IVariable> _positions = new List<IVariable>();
        private MenuItem _robotMenuItems;
        private bool _rootFound;
        private string _rootName = string.Empty;
        private DirectoryInfo _rootPath;

        #region Constructors

        protected AbstractLanguageClass()
        {
            Instance = this;
        }

        private readonly string flename;

        protected virtual void Initialize()
        {
            string filename = flename;
            DataText = string.Empty;
            SourceText = string.Empty;
            string directoryName = Path.GetDirectoryName(filename);
            bool flag = directoryName != null && Directory.Exists(directoryName);
            string extension = Path.GetExtension(filename);
            string fileNameWithoutExtension = Path.GetFileNameWithoutExtension(filename);
            if (this is KUKA && extension == ".sub")
            {
                SourceName = Path.GetFileName(filename);
            }
            else
            {
                if (this is KUKA && (extension == ".src" || extension == ".dat"))
                {
                    SourceName = fileNameWithoutExtension + ".src";
                    DataName = fileNameWithoutExtension + ".dat";
                }
                else
                {
                    SourceName = Path.GetFileName(filename);
                    DataName = string.Empty;
                }
            }
            if (SourceName != null && flag && File.Exists(Path.Combine(directoryName, SourceName)))
            {
                SourceText += File.ReadAllText(Path.Combine(directoryName, SourceName));
            }
            if (DataName != null && flag && File.Exists(Path.Combine(directoryName, DataName)))
            {
                DataText += File.ReadAllText(Path.Combine(directoryName, DataName));
            }
            RawText = SourceText + DataText;
            Instance = this;
            RobotMenuItems = GetMenuItems();
        }

        public abstract void Initialize(string filename);

        protected AbstractLanguageClass(string filename)
        {
            flename = filename;
            Initialize(filename);
        }

        #endregion Constructors

        #region RootPath

        public DirectoryInfo RootPath
        {
            get => _rootPath;
            set
            {
                if (_rootPath != value)
                {
                    _rootPath = value;
                    OnPropertyChanged(nameof(RootPath));
                }
            }
        }

        #endregion RootPath

        #region FileName

        public string FileName
        {
            get => _filename;
            set
            {
                if (_filename == value)
                {
                    return;
                }

                _filename = value;
                OnPropertyChanged(nameof(FileName));
            }
        }

        #endregion FileName

        #region RobotMenuItems

        public MenuItem RobotMenuItems
        {
            get => _robotMenuItems;
            set
            {
                if (_robotMenuItems != value)
                {
                    _robotMenuItems = value;
                    OnPropertyChanged(nameof(RobotMenuItems));
                }
            }
        }

        #endregion RobotMenuItems

        #region Name

        public string Name => RobotType == Typlanguage.None ? string.Empty : RobotType.ToString();

        #endregion Name

        #region DataName

        internal string DataName { get; private set; }

        #endregion DataName

        #region SourceName

        internal string SourceName { get; private set; }

        #endregion SourceName

        #region Progress

        public static int Progress { get; private set; }

        #endregion Progress

        public ReadOnlyObservableCollection<IVariable> ObjectBrowserVariable => _readOnlyBrowserVariables ?? new ReadOnlyObservableCollection<IVariable>(_objectBrowserVariables);

        public IEnumerable<MenuItem> MenuItems => _readonlyMenuItems ?? new ReadOnlyObservableCollection<MenuItem>(_menuItems);

        #region Files

        private readonly ObservableCollection<FileInfo> _files = new ObservableCollection<FileInfo>();

#pragma warning disable 649
        private readonly ReadOnlyObservableCollection<FileInfo> _readOnlyFiles;
#pragma warning restore 649

        public ReadOnlyObservableCollection<FileInfo> Files => _readOnlyFiles ?? new ReadOnlyObservableCollection<FileInfo>(_files);

        #endregion Files

        internal string RawText { private get; set; }

        private static AbstractLanguageClass Instance { get; set; }

        internal string SourceText { get; private set; }

        internal string DataText { get; private set; }

        public string SnippetPath => ".\\Editor\\Config _files\\Snippet.xml";

        protected string Intellisense => RobotType + "Intellisense.xml";

        protected string SnippetFilePath => RobotType + "Snippets.xml";

        internal string Filename { get; set; }

        protected string ConfigFilePath => RobotType + "Config.xml";

        internal string SyntaxHighlightFilePath => RobotType + "Highlight.xshd";

        internal string StyleFilePath => RobotType + "Style.xshd";

        public static int FileCount { get; private set; }

        public ReadOnlyCollection<IVariable> AllVariables => _readOnlyAllVariables ?? new ReadOnlyCollection<IVariable>(_allVariables);

        public ReadOnlyCollection<IVariable> Functions => _readOnlyFunctions ?? new ReadOnlyCollection<IVariable>(_functions);

        public ReadOnlyCollection<IVariable> Fields => _readOnlyFields ?? new ReadOnlyCollection<IVariable>(_fields);

        public ReadOnlyCollection<IVariable> Positions => _readOnlypositions ?? new ReadOnlyCollection<IVariable>(_positions);

        public ReadOnlyCollection<IVariable> Enums => _readOnlyenums ?? new ReadOnlyCollection<IVariable>(_enums);

        public ReadOnlyCollection<IVariable> Structures => _readOnlystructures ?? new ReadOnlyCollection<IVariable>(_structures);

        public ReadOnlyCollection<IVariable> Signals => _readOnlysignals ?? new ReadOnlyCollection<IVariable>(_signals);

        public abstract string CommentChar { get; }

        public abstract List<string> SearchFilters { get; }

        internal abstract Typlanguage RobotType { get; }

        protected abstract string ShiftRegex { get; }

        public abstract Regex MethodRegex { get; }

        public abstract Regex FieldRegex { get; }

        public abstract Regex EnumRegex { get; }

        public abstract Regex XYZRegex { get; }

        public abstract Regex StructRegex { get; }

        public abstract Regex SignalRegex { get; }

        internal abstract string FunctionItems { get; }

        internal abstract IList<ICompletionData> CodeCompletion { get; }

        internal abstract AbstractFoldingStrategy FoldingStrategy { get; set; }

        internal abstract string SourceFile { get; }

        public IOViewModel IOModel
        {
            get => _ioModel;
            set => SetProperty(ref _ioModel, value);
        }

        public int BWProgress
        {
            get => _bwProgress;
            set
            {
                if (_bwProgress != value)
                {
                    _bwProgress = value;
                    OnPropertyChanged(nameof(BWProgress));
                }
            }
        }

        public int BWFilesMin
        {
            get => _bwFilesMin;
            set
            {
                if (_bwFilesMin != value)
                {
                    _bwFilesMin = value;
                    OnPropertyChanged(nameof(BWFilesMin));
                }
            }
        }

        public int BWFilesMax
        {
            get => _bwFilesMax;
            set
            {
                if (_bwFilesMax != value)
                {
                    _bwFilesMax = value;
                    OnPropertyChanged(nameof(BWFilesMax));
                }
            }
        }

        public Visibility BWProgressVisibility
        {
            get => _bwProgressVisibility;
            set
            {
                if (_bwProgressVisibility != value)
                {
                    _bwProgressVisibility = value;
                    OnPropertyChanged(nameof(BWProgressVisibility));
                }
            }
        }

        /// <summary>
        ///     Check to see if file is valid
        /// </summary>
        /// <param name="file"></param>
        /// <returns>File is valid</returns>
        protected abstract bool IsFileValid(FileInfo file);

        public abstract DocumentViewModel GetFile(string filename);


        public virtual string ExtractXYZ(string positionString)
        {
            return PositionBase.ExtractXYZ(positionString);
        }

        internal abstract string FoldTitle(FoldingSection section, TextDocument doc);

        private MenuItem GetMenuItems()
        {
            return new MenuItem();
            ResourceDictionary resourceDictionary = new ResourceDictionary
            {
                Source = new Uri("/miRobotEditor;component/Templates/MenuDictionary.xaml", UriKind.RelativeOrAbsolute)
            };
            return resourceDictionary[RobotType + "Menu"] as MenuItem ?? new MenuItem();
        }

        public static IEditorDocument GetViewModel(string filepath)
        {
            IEditorDocument result;
            if (!string.IsNullOrEmpty(filepath))
            {
                string extension = Path.GetExtension(filepath);
                string text = extension.ToLower();
                switch (text)
                {
                    case ".as":
                    case ".pg":
                        result = new DocumentViewModel(filepath, new Kawasaki(filepath));
                        return result;

                    case ".src":
                    case ".dat":
                        result = GetKukaViewModel(filepath);
                        return result;

                    case ".rt":
                    case ".sub":
                    case ".kfd":
                        result = new DocumentViewModel(filepath, new KUKA(filepath));
                        return result;

                    case ".mod":
                    case ".prg":
                        result = new DocumentViewModel(filepath, new ABB(filepath));
                        return result;

                    case ".bas":
                        result = new DocumentViewModel(filepath, new VBA(filepath));
                        return result;

                    case ".ls":
                        result = new DocumentViewModel(filepath, new Fanuc(filepath));
                        return result;
                }
                result = new DocumentViewModel(filepath, new LanguageBase(filepath));
            }
            else
            {
                result = new DocumentViewModel(null, new LanguageBase(filepath));
            }
            return result;
        }

        private static IEditorDocument GetKukaViewModel(string filepath)
        {
            FileInfo fileInfo = new FileInfo(filepath);
            string name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            Debug.Assert(name != null, "file != null");
            Debug.Assert(fileInfo.DirectoryName != null, "dir != null");
            name = Path.Combine(fileInfo.DirectoryName, name);
            FileInfo src = new FileInfo(name + ".src");
            FileInfo dat = new FileInfo(name + ".dat");

            IEditorDocument result = src.Exists && dat.Exists
                ? new KukaViewModel(src.FullName, new KUKA(src.FullName))
                : (IEditorDocument)new DocumentViewModel(filepath, new KUKA(filepath));
            return result;
        }

        public virtual string CommentReplaceString(string text)
        {
            string pattern = string.Format("^([ ]*)([{0}]*)([^\r\n]*)", CommentChar);
            Regex regex = new Regex(pattern);
            Match match = regex.Match(text);
            string result = match.Success ? match.Groups[1] + match.Groups[3].ToString() : text;
            return result;
        }

        public virtual int CommentOffset(string text)
        {
            Regex regex = new Regex("(^[\\s]+)");
            Match match = regex.Match(text);
            int result = match.Success ? match.Groups[1].Length : 0;
            return result;
        }

        public virtual bool IsLineCommented(string text)
        {
            return text.Trim().IndexOf(CommentChar, StringComparison.Ordinal).Equals(0);
        }

        private static bool IsValidFold(string text, string s, string e)
        {
            text = text.Trim();
            bool flag = text.StartsWith(s);
            bool flag2 = text.StartsWith(e);
            bool result;
            if (!(flag | flag2))
            {
                result = false;
            }
            else
            {
                string text2 = flag ? s : e;
                if (text.Substring(text.IndexOf(text2, StringComparison.Ordinal) + text2.Length).Length == 0)
                {
                    result = true;
                }
                else
                {
                    string value = text.Substring(text.IndexOf(text2, StringComparison.Ordinal) + text2.Length, 1);
                    char c = Convert.ToChar(value);
                    bool flag3 = char.IsLetterOrDigit(c);
                    result = !flag3;
                }
            }
            return result;
        }

        protected static IEnumerable<LanguageFold> CreateFoldingHelper(ITextSource document, string startFold,
            string endFold, bool defaultclosed)
        {
            List<LanguageFold> list = new List<LanguageFold>();
            Stack<int> stack = new Stack<int>();
            endFold = endFold.ToLower();
            int num = 0;
            if (document is TextDocument textDocument)
            {
                foreach (DocumentLine current in textDocument.Lines)
                {
                    DocumentLine lineByNumber = textDocument.GetLineByNumber(current.LineNumber);
                    string text = textDocument.GetText(lineByNumber.Offset, lineByNumber.Length).ToLower();
                    string text2 = text.TrimStart(new char[0]);
                    try
                    {
                        if (IsValidFold(text, startFold, endFold))
                        {
                            if (text2.StartsWith(startFold))
                            {
                                stack.Push(lineByNumber.Offset);
                            }
                            else
                            {
                                if (text2.StartsWith(endFold) && stack.Count > 0)
                                {
                                    bool flag;
                                    if (endFold == "end")
                                    {
                                        if (text.Length == endFold.Length)
                                        {
                                            flag = true;
                                        }
                                        else
                                        {
                                            char[] array = text.ToCharArray(endFold.Length, 1);
                                            flag = !char.IsLetterOrDigit(array[0]);
                                        }
                                    }
                                    else
                                    {
                                        flag = true;
                                    }
                                    if (flag)
                                    {
                                        int num2 = stack.Pop();
                                        int end = lineByNumber.Offset + text.Length;
                                        string text3 = textDocument.GetText(num2 + startFold.Length + 1,
                                            lineByNumber.Offset - num2 - endFold.Length);
                                        LanguageFold item = new LanguageFold(num2, end, text3, startFold, endFold, defaultclosed);
                                        list.Add(item);
                                    }
                                }
                                else
                                {
                                    num++;
                                }
                            }
                        }
                    }
                    catch (Exception)
                    {
                    }
                }
            }
            return list;
        }

        public ShiftClass ShiftProgram(IEditorDocument doc, ShiftViewModel shift)
        {
            ShiftClass result;
            if (doc is KukaViewModel)
            {
                KukaViewModel kukaViewModel = doc as KukaViewModel;
                ShiftClass shiftClass = new ShiftClass
                {
                    Source = ShiftProgram(kukaViewModel.Source, shift)
                };
                if (!string.IsNullOrEmpty(kukaViewModel.Data.Text))
                {
                    shiftClass.Data = ShiftProgram(kukaViewModel.Data, shift);
                }
                result = shiftClass;
            }
            else
            {
                ShiftClass shiftClass = new ShiftClass
                {
                    Source = ShiftProgram(doc.TextBox, shift)
                };
                result = shiftClass;
            }
            return result;
        }

        private string ShiftProgram(AvalonEditor doc, ShiftViewModel shift)
        {
            Stopwatch stopwatch = new Stopwatch();
            stopwatch.Start();
            double num = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.X);
            double num2 = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.Y);
            double num3 = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.Z);
            Regex regex = new Regex(ShiftRegex, RegexOptions.IgnoreCase);
            MatchCollection matchCollection = regex.Matches(doc.Text);
            int count = matchCollection.Count;
            double num4 = 0.0;
            double num5 = count > 0 ? 100 / count : count;
            foreach (Match match in regex.Matches(doc.Text))
            {
                // ReSharper disable UnusedVariable
                num4 += num5;
                double num6 = Convert.ToDouble(match.Groups[3].Value) + num;

                double num7 = Convert.ToDouble(match.Groups[4].Value) + num2;

                double num8 = Convert.ToDouble(match.Groups[5].Value) + num3;
                // ReSharper restore UnusedVariable
                switch (RobotType)
                {
                    case Typlanguage.KUKA:
                        doc.ReplaceAll();
                        break;

                    case Typlanguage.ABB:
                        doc.ReplaceAll();
                        break;
                }
            }
            Thread.Sleep(500);
            stopwatch.Stop();
            Console.WriteLine("{0}ms to parse shift", stopwatch.ElapsedMilliseconds);
            return doc.Text;
        }

        public void GetRootDirectory(string dir)
        {
            DirectoryInfo directoryInfo = new DirectoryInfo(dir);
            if (directoryInfo.Name == directoryInfo.Root.Name)
            {
                _rootFound = true;
            }
            try
            {
                while (directoryInfo.Parent != null && !_rootFound && directoryInfo.Parent.Name != "KRC")
                {
                    GetRootDirectory(directoryInfo.Parent.FullName);
                }
                if (!_rootFound)
                {
                    if (directoryInfo.Parent != null && directoryInfo.Parent.Parent != null &&
                        directoryInfo.Parent.Parent.Parent != null)
                    {
                        _rootName = directoryInfo.Parent != null && directoryInfo.Parent.Parent.Name != "C"
                            ? directoryInfo.Parent.Parent.FullName
                            : directoryInfo.Parent.Parent.Parent.FullName;
                    }
                    DirectoryInfo directoryInfo2 = new DirectoryInfo(_rootName);
                    DirectoryInfo[] directories = directoryInfo2.GetDirectories();
                    if (directories.Length >= 1)
                    {
                        if (directories[0].Name == "C" && directories[1].Name == "KRC")
                        {
                            _rootName = directoryInfo2.FullName;
                        }
                        _rootFound = true;
                        GetRootFiles(_rootName);
                        FileCount = Files.Count;

                        GetVariables();
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new ErrorMessage("Get Root Directory", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private void GetRootFiles(string dir)
        {
            try
            {
                List<FileInfo> newFiles = (from directory in Directory.GetDirectories(dir)
                                           let files = Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories)
                                           from file in files
                                           select new FileInfo(file)).ToList();

                foreach (FileInfo file in newFiles)
                {
                    if (file.Name.Equals("kuka_con.mdb", StringComparison.OrdinalIgnoreCase))
                    {
                        _kukaCon = file.Name;
                    }

                    _files.Add(file);
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new ErrorMessage("Error When Getting Files for Object Browser", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private readonly object locker = new object();

        private void GetVariables()
        {
            Task<VariableMembers> task = GetVariablesAsync();
            _ = task.ContinueWith((r) =>
            {
                _enums.AddRange(r.Result.Enums);
                _fields.AddRange(r.Result.Fields);
                _positions.AddRange(r.Result.Positions);
                _signals.AddRange(r.Result.Signals);
                _functions.AddRange(r.Result.Functions);
                _structures.AddRange(r.Result.Structures);

                _allVariables.AddRange(Functions);
                _allVariables.AddRange(Fields);
                _allVariables.AddRange(Positions);
                _allVariables.AddRange(Signals);
            })
            ;
        }

        private Task<VariableMembers> GetVariableMembers(string filename)
        {
            Task<VariableMembers> task = Task.Factory.StartNew(() =>
            {
                VariableMembers result = new VariableMembers();
                result.FindVariables(filename, this);
                return result;
            });

            return task;
        }

        private VariableMembers GetVariables(IEnumerable<FileInfo> files)
        {
            VariableMembers variableMembers = new VariableMembers();
            IEnumerable<FileInfo> validFiles = from file in files
                                               where IsFileValid(file)
                                               select file;

            IEnumerable<Task> results = from file in validFiles
                                        select GetVariableMembers(file.FullName).ContinueWith((var) =>
                                        {
                                            variableMembers.Functions.AddRange(var.Result.Structures.ToList());
                                            variableMembers.Structures.AddRange(var.Result.Structures.ToList());
                                            variableMembers.Fields.AddRange(var.Result.Fields.ToList());
                                            variableMembers.Signals.AddRange(var.Result.Signals.ToList());
                                            variableMembers.Enums.AddRange(var.Result.Enums.ToList());
                                            variableMembers.Positions.AddRange(var.Result.Positions.ToList());
                                        });

            Task.WaitAll(results.ToArray());

            OnPropertyChanged(nameof(Structures));
            OnPropertyChanged(nameof(Functions));
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(Positions));
            BWProgressVisibility = Visibility.Collapsed;

            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.EnableIO = File.Exists(_kukaCon);
            IOModel = new IOViewModel(_kukaCon);
            return variableMembers;
        }

        private Task<VariableMembers> GetVariablesAsync()
        {
            Task<VariableMembers> task = Task<VariableMembers>.Factory.StartNew(() => GetVariables(Files));

            return task;
        }

        public sealed class VariableMembers
        {
            private void Initialize()
            {
                Functions = new List<IVariable>();
                Structures = new List<IVariable>();
                Fields = new List<IVariable>();
                Signals = new List<IVariable>();
                Enums = new List<IVariable>();
                Positions = new List<IVariable>();
            }

            public VariableMembers()
            {
                Initialize();
            }

            #region Variables

            public List<IVariable> Functions { get; private set; }

            public List<IVariable> Structures { get; private set; }

            public List<IVariable> Fields { get; private set; }

            public List<IVariable> Signals { get; private set; }

            public List<IVariable> Enums { get; private set; }

            public List<IVariable> Positions { get; private set; }

            #endregion Variables

            public void FindVariables(string filename, ILanguageRegex regex)
            {
                Functions = FindMatches(regex.MethodRegex, Global.ImgMethod, filename).ToList();
                Structures = FindMatches(regex.StructRegex, Global.ImgStruct, filename).ToList();
                Fields = FindMatches(regex.FieldRegex, Global.ImgField, filename).ToList();
                Signals = FindMatches(regex.SignalRegex, Global.ImgSignal, filename).ToList();
                Enums = FindMatches(regex.EnumRegex, Global.ImgEnum, filename).ToList();
                Positions = FindMatches(regex.XYZRegex, Global.ImgXyz, filename).ToList();
            }
        }

        private static IEnumerable<IVariable> FindMatches(Regex matchstring, string imgPath, string filepath)
        {
            List<IVariable> list = new List<IVariable>();

            IEnumerable<IVariable> result;
            try
            {
                string input = File.ReadAllText(filepath);
                if (string.IsNullOrEmpty(matchstring.ToString()))
                {
                    result = list;
                    return result;
                }
                Match match = matchstring.Match(input);
                while (match.Success)
                {
                    list.Add(new Variable
                    {
                        Declaration = match.Groups[0].ToString(),
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = match.Groups[3].ToString(),
                        Path = filepath,
                        Icon = ImageHelper.LoadBitmap(imgPath)
                    });
                    match = match.NextMatch();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new ErrorMessage("Find Matches", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
            result = list;
            return result;
        }
    }
}