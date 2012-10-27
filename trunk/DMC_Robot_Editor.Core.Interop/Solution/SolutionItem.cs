using System;
using System.Text;

namespace miRobotEditor.Core
{
    /// <summary>
    /// 
    /// </summary>
    public class SolutionItem
    {

        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Solution Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="location"></param>
        public SolutionItem(string name, string location)
        {
            this.Name = name;
            this.Location = location;
        }
        /// <summary>
        /// AppendItem
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="indentString"></param>
        public void AppendItem(StringBuilder sb, string indentString)
        {
            sb.Append(indentString);
            sb.Append(Name);
            sb.Append(" = ");
            sb.Append(Location);
            sb.Append(Environment.NewLine);
        }


        public override string ToString()
        {
            return String.Format("[SolutionItem: location = {0}, name = {1}]",Location,Name);
        }

    }
}
