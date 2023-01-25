using System;
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
using Mookie.WPF.Utilities;

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
        internal readonly List<IVariable> _allVariables = new();
        private readonly List<IVariable> _enums = new();

        private readonly ObservableCollection<MenuItem> _menuItems = new();
        private readonly ObservableCollection<IVariable> _objectBrowserVariables = new();
        private readonly ReadOnlyCollection<IVariable> _readOnlyAllVariables = null;
        private readonly ReadOnlyObservableCollection<IVariable> _readOnlyBrowserVariables = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlyFields = null;

        private readonly ReadOnlyCollection<IVariable> _readOnlyFunctions = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlyenums = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlypositions = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlysignals = null;
        private readonly ReadOnlyCollection<IVariable> _readOnlystructures = null;
        private readonly ReadOnlyObservableCollection<MenuItem> _readonlyMenuItems = null;
        private readonly List<IVariable> _signals = new();
        private readonly List<IVariable> _structures = new();
        private int _bwFilesMax;
        private int _bwFilesMin;
        private int _bwProgress;
        private Visibility _bwProgressVisibility = Visibility.Collapsed;
        private readonly List<IVariable> _fields = new();
        private string _filename = string.Empty;
        private readonly List<IVariable> _functions = new();
        private IOViewModel _ioModel;
        private string _kukaCon;
        private readonly List<IVariable> _positions = new();
        private MenuItem _robotMenuItems;
        private bool _rootFound;
        private string _rootName = string.Empty;
        private DirectoryInfo _rootPath;

        #region Constructors

        protected AbstractLanguageClass()
        {
            Instance = this;
        }

        private readonly string _fileName;

        protected virtual void Initialize()
        {
            string filename = _fileName;
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
            _fileName = filename;
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

        private readonly ObservableCollection<FileInfo> _files = new();

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


        public virtual string ExtractXYZ(string positionString) => PositionBase.ExtractXYZ(positionString);

        internal abstract string FoldTitle(FoldingSection section, TextDocument doc);

        private MenuItem GetMenuItems()
        {
            return new MenuItem();
            ResourceDictionary resourceDictionary = new()
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
            FileInfo fileInfo = new(filepath);
            string name = Path.GetFileNameWithoutExtension(fileInfo.Name);
            Debug.Assert(name != null, "file != null");
            Debug.Assert(fileInfo.DirectoryName != null, "dir != null");
            name = Path.Combine(fileInfo.DirectoryName, name);
            FileInfo src = new(name + ".src");
            FileInfo dat = new(name + ".dat");

            IEditorDocument result = src.Exists && dat.Exists
                ? new KukaViewModel(src.FullName, new KUKA(src.FullName))
                : (IEditorDocument)new DocumentViewModel(filepath, new KUKA(filepath));
            return result;
        }

        public virtual string CommentReplaceString(string text)
        {
            string pattern = string.Format("^([ ]*)([{0}]*)([^\r\n]*)", CommentChar);
            Regex regex = new(pattern);
            Match match = regex.Match(text);
            string result = match.Success ? match.Groups[1] + match.Groups[3].ToString() : text;
            return result;
        }

        public virtual int CommentOffset(string text)
        {
            Regex regex = new("(^[\\s]+)");
            Match match = regex.Match(text);
            int result = match.Success ? match.Groups[1].Length : 0;
            return result;
        }

        public virtual bool IsLineCommented(string text) => text.Trim().IndexOf(CommentChar, StringComparison.Ordinal).Equals(0);

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
            List<LanguageFold> list = new();
            Stack<int> stack = new();
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
                                        LanguageFold item = new(num2, end, text3, startFold, endFold, defaultclosed);
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
                ShiftClass shiftClass = new()
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
                ShiftClass shiftClass = new()
                {
                    Source = ShiftProgram(doc.TextBox, shift)
                };
                result = shiftClass;
            }
            return result;
        }

        private string ShiftProgram(AvalonEditor doc, ShiftViewModel shift)
        {
            Stopwatch stopwatch = new();
            stopwatch.Start();
            double num = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.X);
            double num2 = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.Y);
            double num3 = Convert.ToDouble(ShiftViewModel.Instance.DiffValues.Z);
            Regex regex = new(ShiftRegex, RegexOptions.IgnoreCase);
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

        public void GetRootDirectory()
        {
            var directory = Path.GetDirectoryName(_fileName);
            GetRootDirectory(directory);
        }
        public async void GetRootDirectory(string dir)
        {
            DirectoryInfo directoryInfo = new(dir);
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

                    DirectoryInfo directoryInfo2 = new(_rootName);
                    DirectoryInfo[] directories = directoryInfo2.GetDirectories();
                    if(directories.Any())
                    {


                        if (directories[0].Name.Equals("C") && directories[1].Name.Equals("KRC"))
                        {
                            _rootName = directoryInfo2.FullName;
                        }
                        _rootFound = true;
                       var rootFiles= GetRootFiles(_rootName);
                        FileCount = Files.Count;

                      var _ =  await  GetVariables(rootFiles);
                    }
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new("Get Root Directory", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
        }

        private List<FileInfo> GetRootFiles(string dir)
        {
            try
            {

                var newFiles = Directory.GetDirectories(dir)
                            .SelectMany(directory => Directory.GetFiles(directory, "*.*", SearchOption.AllDirectories))
                            .Select(file => new FileInfo(file))
                            .ToList();

                // There

                _kukaCon = newFiles.FirstOrDefault(o => o.Name.Equals("kuka_con.mdb", StringComparison.InvariantCultureIgnoreCase))?.Name;

                foreach (var file in newFiles)
                    _files.Add(file);


                return newFiles;
 
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new("Error When Getting Files for Object Browser", ex, MessageType.Error);
                  WeakReferenceMessenger.Default.Send<IMessage>(msg);
                throw;
            }

            
        }

        private readonly object locker = new();

        private async Task<bool> GetVariables()
        {
            var result = await GetVariablesAsync();
            _enums.AddRange(result.Enums);
            _fields.AddRange(result.Fields);
            _positions.AddRange(result.Positions);
            _signals.AddRange(result.Signals);
            _functions.AddRange(result.Functions);
            _structures.AddRange(result.Structures);

            _allVariables.AddRange(Functions);
            _allVariables.AddRange(Fields);
            _allVariables.AddRange(Positions);
            _allVariables.AddRange(Signals);

            return true;
        }

        private VariableMembers GetVariableMembers(string fileName)
        {
            VariableMembers result = new();
            result.FindVariables(fileName, this);
            return result;
        }
        private Task<VariableMembers> GetVariableMembersAsync(string filename)
        {
            var task = Task.Run(() => GetVariableMembers(filename));

            return task;
        }

        private async Task<VariableMembers> GetVariables(IEnumerable<FileInfo> files)
        {
            var result = new VariableMembers();

            var validFiles = files.Where(o => IsFileValid(o)).ToList();


            foreach(var validFile in validFiles)
            {
                var ext = Path.GetExtension(validFile.FullName);

                if (ext == ".dat")
                {

                }
                var variableMembers = await GetVariableMembersAsync(validFile.FullName);
                result.Functions.AddRange(variableMembers.Structures.ToList());
                result.Structures.AddRange(variableMembers.Structures.ToList());
                result.Fields.AddRange(variableMembers.Fields.ToList());
                result.Signals.AddRange(variableMembers.Signals.ToList());
                result.Enums.AddRange(variableMembers.Enums.ToList());
                result.Positions.AddRange(variableMembers.Positions.ToList());
            }

 

            OnPropertyChanged(nameof(Structures));
            OnPropertyChanged(nameof(Functions));
            OnPropertyChanged(nameof(Fields));
            OnPropertyChanged(nameof(Files));
            OnPropertyChanged(nameof(Positions));
            BWProgressVisibility = Visibility.Collapsed;

            MainViewModel instance = Ioc.Default.GetRequiredService<MainViewModel>();
            instance.EnableIO = File.Exists(_kukaCon);
            IOModel = new IOViewModel(_kukaCon);
            return result;
        }

        private Task<VariableMembers> GetVariablesAsync()
        {
            var task = Task.Run(()=>GetVariables(Files));
            return task;
        }

        public sealed class VariableMembers
        {
            
            public static VariableMembers Create(string fileName,ILanguageRegex regex)
            {
                var result = new VariableMembers();
                result.FindVariables(fileName, regex);
                return result;
            }


            #region Variables

            public List<IVariable> Functions { get; private set; } = new List<IVariable>();

            public List<IVariable> Structures { get; private set; } = new List<IVariable>();

            public List<IVariable> Fields { get; private set; } = new List<IVariable>();

            public List<IVariable> Signals { get; private set; } = new List<IVariable>();

            public List<IVariable> Enums { get; private set; } = new List<IVariable>();

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

        internal static IEnumerable<IVariable> FindMatches(Regex matchString, string imgPath, string filePath)
        {
            List<IVariable> list = new();

            var ext = Path.GetExtension(filePath);

            switch (ext)
            {
                case ".src":
                case ".dat":
                case ".sub":
                    Console.WriteLine();
                    break;

            }

            IEnumerable<IVariable> result;
            try
            {
                string input = File.ReadAllText(filePath);
                if (string.IsNullOrEmpty(matchString.ToString()))
                {
                    result = list;
                    return result;
                }
                Match match = matchString.Match(input);
                while (match.Success)
                {
                    list.Add(new Variable
                    {
                        Declaration = match.Groups[0].ToString(),
                        Offset = match.Index,
                        Type = match.Groups[1].ToString(),
                        Name = match.Groups[2].ToString(),
                        Value = match.Groups[3].ToString(),
                        Path = filePath,
                        Icon = ImageHelper.LoadBitmap(imgPath)
                    });
                    match = match.NextMatch();
                }
            }
            catch (Exception ex)
            {
                ErrorMessage msg = new("Find Matches", ex, MessageType.Error);
                _ = WeakReferenceMessenger.Default.Send<IMessage>(msg);
            }
            result = list;
            return result;
        }
    }
}