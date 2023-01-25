using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using miRobotEditor.Classes;
using miRobotEditor.Interfaces;

namespace miRobotEditor.Controls.TextEditor.Language
{
    public abstract partial class AbstractLanguageClass
    {
        public sealed class VariableMembers
        {

            public static VariableMembers Create(string fileName, ILanguageRegex regex)
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

            public List<IVariable> Positions { get; private set; } = new List<IVariable>();

            #endregion Variables

            public void FindVariables(string filename, ILanguageRegex regex)
            {
                var ext = Path.GetExtension(filename);
                switch (ext)
                {
                    case ".src":
                    case ".dat":
                    case ".sub":
                        Console.WriteLine();
                        break;
                    default:
                        break;
                }
                var functions = FindMatches(regex.MethodRegex, Global.ImgMethod, filename).ToList();
                Functions.AddRange(functions) ;
                Structures.AddRange( FindMatches(regex.StructRegex, Global.ImgStruct, filename).ToList());
                Fields.AddRange(FindMatches(regex.FieldRegex, Global.ImgField, filename).ToList());
                Signals.AddRange(FindMatches(regex.SignalRegex, Global.ImgSignal, filename).ToList());
                Enums.AddRange(FindMatches(regex.EnumRegex, Global.ImgEnum, filename).ToList());
                Positions.AddRange(FindMatches(regex.XYZRegex, Global.ImgXyz, filename).ToList());
            }
        }
    }
}