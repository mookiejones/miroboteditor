using ICSharpCode.AvalonEdit.CodeCompletion;
using System;
using System.Text.RegularExpressions;
using System.Collections.Generic;
using System.IO;
using System.Collections.ObjectModel;
using ICSharpCode.AvalonEdit.Document;
using ICSharpCode.AvalonEdit.Folding;
using miRobotEditor.Classes;

namespace miRobotEditor.Robot
{

    public class VBA : AbstractRobotClass
    {

        //TODO Set Filter for this
        /// <summary>
        /// Sets ComboBox Filter Items for searching
        /// </summary>
        /// <returns></returns>

        public override string[] SearchFilters
        {
            get { return new string[] { "*.*", "*.dat", "*.src", "*.ini", "*.sub", "*.zip", "*.kfd" }; }
        }

        public VBA(FileInfo file)
        {
            _file = file;
            FoldingStrategy = new RegionFoldingStrategy();
        }

        internal override Enums.TYPROBOT RobotType { get { return Enums.TYPROBOT.VBA; } }

        internal override string SourceFile
        {
            get { return String.Empty; }
        }



        internal override sealed AbstractFoldingStrategy FoldingStrategy { get; set; }

        protected override string ShiftRegex
        {
            get { return @"((RobTarget\s*[\w]*\s*:=\s*\[\[)([\d.-]*),([\d.-]*),([-.\d]*))"; }
        }

        internal override Collection<string> FunctionItems
        {
            get { return new Collection<string>() { @"((?<!END)()()PROC\s([\d\w]*)[\(\)\w\d_. ]*)" }; }
        }

        #region Folding Section


        internal override string FoldTitle(ICSharpCode.AvalonEdit.Folding.FoldingSection section, ICSharpCode.AvalonEdit.Document.TextDocument doc)
        {
            var s = System.Text.RegularExpressions.Regex.Split(section.Title, "æ");

            var start = section.StartOffset + s[0].Length;
            var end = section.Length - (s[0].Length + s[1].Length);


            return doc.GetText(start, end);
        }
        /// <summary>
        /// The class to generate the foldings, it implements ICSharpCode.TextEditor.Document.IFoldingStrategy
        /// </summary>
        public class RegionFoldingStrategy : AbstractFoldingStrategy
        {

    
            /// <summary>
            /// Create <see cref="NewFolding"/>s for the specified document.
            /// </summary>
            public override IEnumerable<NewFolding> CreateNewFoldings(TextDocument document, out int firstErrorOffset)
            {
                firstErrorOffset = -1;
                return CreateNewFoldings(document);
            }

            /// <summary>
            /// Create <see cref="NewFolding"/>s for the specified document.
            /// </summary>
            public IEnumerable<NewFolding> CreateNewFoldings(ITextSource document)
            {
                var newFoldings = new List<NewFolding>();

                newFoldings.AddRange(CreateFoldingHelper(document, "public function", "end function", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "private function", "end function", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "public sub", "end sub", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "private sub", "end sub", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "property", "end property", true));
                newFoldings.AddRange(CreateFoldingHelper(document, "If", "End If", false));
                newFoldings.AddRange(CreateFoldingHelper(document, "Select Case", "End Select", false));
                //                /Open Fold Strings = "Do While" "If" "ElseIf" "Function" "Sub" "With" "For" "Select Case" "Case Else" "Case" "Else"
                //Close Fold Strings = "ElseIf" "End If" "End Function" "End Sub" "End With" "Loop" "Next" "Wend" "End Select" "Case Else" "Case" "Else"

                newFoldings.Sort((a, b) => a.StartOffset.CompareTo(b.StartOffset));
                return newFoldings;
            }


        }

        #endregion

        #region Code Completion Section

        internal override IList<ICompletionData> CodeCompletion
        {
            get
            {
                List<ICompletionData> codeCompletionList = new List<ICompletionData>();
                codeCompletionList.Add(new Classes.CodeCompletion("Item1"));
                return codeCompletionList;
            }
        }
        #endregion


        internal override System.Windows.Media.Color FocusedColor
        {
            get { return System.Windows.Media.Colors.AntiqueWhite; }
        }

        internal override System.Windows.Media.Color UnfocusedColor
        {
            get { return System.Windows.Media.Colors.WhiteSmoke; }
        }

        public override Regex MethodRegex { get { return new Regex("( sub )", RegexOptions.IgnoreCase); } }

        public override Regex StructRegex { get { return new Regex("( struc )", RegexOptions.IgnoreCase); } }

        public override Regex FieldRegex { get { return new Regex("( boolean )", RegexOptions.IgnoreCase); } }

        public override Regex EnumRegex { get { return new Regex("( enum )", RegexOptions.IgnoreCase); } }

        public override string CommentChar { get { return "'"; } }

        public override Regex SignalRegex { get { return new Regex(String.Empty); } }

        public override Regex XYZRegex { get { return new Regex(String.Empty); } }

        public override Classes.FileModel GetFile(FileInfo file)
        {
            return new FileModel(){File=file};
        }
    }
    }

