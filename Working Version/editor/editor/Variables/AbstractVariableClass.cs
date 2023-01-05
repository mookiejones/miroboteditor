using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using System.Windows.Controls;
using miRobotEditor.Abstract;

namespace miRobotEditor.Variables
{
    internal abstract class AbstractVariableClass
    {
        public abstract string Type { get; }
        protected abstract string Expression { get; }
        public abstract string Name { get; set; }
        private string Raw { get; set; }
        public string Scope { get; set; }
        public ToolTip ToolTip { get; set; }
        private List<object> Items { get; set; }
        internal abstract AbstractParser Parser { get; }

        protected static GroupCollection GetMatchCollection(string text, string matchstring)
        {
            Regex regex = new Regex(matchstring, RegexOptions.IgnoreCase);
            Match match = regex.Match(text);
            return match.Success ? match.Groups : null;
        }

        public void Add(string text, AbstractVariableClass vartype)
        {
            Regex regex = new Regex(Expression, RegexOptions.IgnoreCase);
            Match match = regex.Match(text);
            while (match.Success)
            {
                Raw = match.ToString();
                Items.Add(vartype);
                match = match.NextMatch();
            }
        }

        internal abstract void GetVariable(GroupCollection m);

        public string Key(string line)
        {
            throw new NotImplementedException();
        }
    }
}