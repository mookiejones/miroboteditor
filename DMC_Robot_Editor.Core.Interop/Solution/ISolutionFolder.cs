using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Text.RegularExpressions;

namespace miRobotEditor.Core
{
    /// <summary>
    /// An project-like entry in a solution, for example a solution folder or a project.
    /// Implementing classes are required to implement this interface in a thread-safe manner.
    /// Thread-safe members lock on the SyncRoot. Non-thread-safe members may only be called from the main thread.
    /// </summary>
    public interface ISolutionFolder
    {
        /// <summary>
        /// Gets the object used for thread-safe synchronization.
        /// Thread-safe members lock on this object, but if you manipulate underlying structures
        /// (such as the MSBuild project for MSBuildBasedProjects) directly, you will have to lock on this object.
        /// </summary>
        object SyncRoot
        {
            get;
        }

        /// <summary>
        /// Gets/Sets the container that contains this folder. This member is thread-safe.
        /// </summary>
        ISolutionFolderContainer Parent
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        string TypeGuid
        {
            get;
            set;
        }

        /// <summary>
        /// 
        /// </summary>
        string IdGuid
        {
            get;
            set;
        }
        /// <summary>
        /// 
        /// </summary>
        string Location
        {
            get;
            set;
        }
        /// <summary>
        /// Name
        /// </summary>
        string Name
        {
            get;
            set;
        }
    }
    /// <summary>
    /// Description of ISolutionFolderContainer.
    /// </summary>
    public interface ISolutionFolderContainer
    {
        /// <summary>
        /// Parent Solution
        /// </summary>
        Solution ParentSolution
        {
            get;
        }
        /// <summary>
        /// Sections
        /// </summary>
        List<ProjectSection> Sections
        {
            get;
        }

        /// <summary>
        /// Folders
        /// </summary>
        List<ISolutionFolder> Folders
        {
            get;
        }

        /// <summary>
        /// SolutionItems
        /// </summary>
        ProjectSection SolutionItems
        {
            get;
        }

        /// <summary>
        /// AddFolder
        /// </summary>
        /// <param name="folder"></param>
        void AddFolder(ISolutionFolder folder);
        /// <summary>
        /// RemoveFolder
        /// </summary>
        /// <param name="folder"></param>
        void RemoveFolder(ISolutionFolder folder);
        /// <summary>
        /// IsAncestorOf
        /// </summary>
        /// <param name="folder"></param>
        /// <returns></returns>
        bool IsAncestorOf(ISolutionFolder folder);
    }
    /// <summary>
    /// Description of ProjectSection.
    /// </summary>
    public class ProjectSection
    {

        List<SolutionItem> items = new List<SolutionItem>();
        /// <summary>
        /// Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        /// SectionType
        /// </summary>
        public string SectionType { get; private set; }
        /// <summary>
        /// Items
        /// </summary>
        public List<SolutionItem> Items
        {
            get
            {
                return items;
            }
        }
        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="name"></param>
        /// <param name="sectionType"></param>
        public ProjectSection(string name, string sectionType)
        {
            this.Name = name;
            this.SectionType = sectionType;
        }
        /// <summary>
        /// Append Section
        /// </summary>
        /// <param name="sb"></param>
        /// <param name="indentString"></param>
        public void AppendSection(StringBuilder sb, string indentString)
        {
            foreach (SolutionItem item in items)
            {
                item.AppendItem(sb, indentString);
            }
        }

        static Regex sectionPattern = new Regex("\\s*(?<Key>.*\\S)\\s*=\\s*(?<Value>.*\\S)\\s*", RegexOptions.Compiled);
        /// <summary>
        /// Read Global Section
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="name"></param>
        /// <param name="sectionType"></param>
        /// <returns></returns>
        public static ProjectSection ReadGlobalSection(TextReader sr, string name, string sectionType)
        {
            return ReadSection(sr, name, sectionType, "EndGlobalSection");
        }
        /// <summary>
        /// Read Project Section
        /// </summary>
        /// <param name="sr"></param>
        /// <param name="name"></param>
        /// <param name="sectionType"></param>
        /// <returns></returns>
        public static ProjectSection ReadProjectSection(TextReader sr, string name, string sectionType)
        {
            return ReadSection(sr, name, sectionType, "EndProjectSection");
        }

        static ProjectSection ReadSection(TextReader sr, string name, string sectionType, string endTag)
        {
            var newFolder = new ProjectSection(name, sectionType);
            while (true)
            {
                string line = sr.ReadLine();
                if (line == null || line.Trim() == endTag)
                {
                    break;
                }
                Match match = sectionPattern.Match(line);
                if (match.Success)
                {
                    newFolder.Items.Add(new SolutionItem(match.Result("${Key}"), match.Result("${Value}")));
                }
            }
            return newFolder;
        }

    }
}
