using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Shell;
using miRobotEditor.Core.EventArgs;

namespace miRobotEditor.Core
{
    /// <summary>
    /// This class handles the recent open files and the recent open project files of SharpDevelop
    /// it checks, if the files exists at every creation, and if not it doesn't list them in the
    /// recent files, and they'll not be saved during the next option save.
    /// </summary>
    public sealed class RecentOpen
    {
        /// <summary>
        /// This variable is the maximal length of lastfile/lastopen entries
        /// must be > 0
        /// </summary>
        int MAX_LENGTH = 10;

        readonly ObservableCollection<string> lastfile = new ObservableCollection<string>();
        readonly ObservableCollection<string> lastproject = new ObservableCollection<string>();

        /// <summary>
        /// 
        /// </summary>
        public IList<string> RecentFile
        {
            get
            {
                return lastfile;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public IList<string> RecentProject
        {
            get
            {
                return lastproject;
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public RecentOpen()
        {
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="p"></param>
        public RecentOpen(Core.Services.Properties p)
        {
            // don't check whether files exist because that might be slow (e.g. if file is on network
            // drive that's unavailable)

            // if one of these entries is a string, then it's from a previous SharpDevelop version - don't try loading it
            if (p.Contains("Files") && !(p.Get("Files") is string))
            {
                lastfile.AddRange(p.Get("Files", new string[0]));
            }

            if (p.Contains("Projects") && !(p.Get("Files") is string))
            {
                lastproject.AddRange(p.Get("Projects", new string[0]));
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddLastFile(string name)
        {
            for (int i = 0; i < lastfile.Count; ++i)
            {
                if (lastfile[i].Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lastfile.RemoveAt(i);
                }
            }

            while (lastfile.Count >= MAX_LENGTH)
            {
                lastfile.RemoveAt(lastfile.Count - 1);
            }

            lastfile.Insert(0, name);
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearRecentFiles()
        {
            lastfile.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        public void ClearRecentProjects()
        {
            lastproject.Clear();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        public void AddLastProject(string name)
        {
            for (int i = 0; i < lastproject.Count; ++i)
            {
                if (lastproject[i].ToString().Equals(name, StringComparison.OrdinalIgnoreCase))
                {
                    lastproject.RemoveAt(i);
                }
            }

            while (lastproject.Count >= MAX_LENGTH)
            {
                lastproject.RemoveAt(lastproject.Count - 1);
            }

            lastproject.Insert(0, name);
            JumpList.AddToRecentCategory(name);
        }
        /// <summary>
        /// From XmlElement
        /// </summary>
        /// <param name="properties"></param>
        /// <returns></returns>
        public static RecentOpen FromXmlElement(Services.Properties properties)
        {
            return new RecentOpen(properties);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Services.Properties ToProperties()
        {
            var p = new Services.Properties();
            p.Set("Files", lastfile.ToArray());
            p.Set("Projects", lastproject.ToArray());
            return p;
        }

        internal void FileRemoved(object sender, FileEventArgs e)
        {
            for (int i = 0; i < lastfile.Count; ++i)
            {
                string file = lastfile[i];
                if (e.FileName == file)
                {
                    lastfile.RemoveAt(i);
                    break;
                }
            }
        }

        internal void FileRenamed(object sender, FileRenameEventArgs e)
        {
            for (int i = 0; i < lastfile.Count; ++i)
            {
                string file = lastfile[i];
                if (e.SourceFile != file) continue;
                lastfile.RemoveAt(i);
                lastfile.Insert(i, e.TargetFile);
                break;
            }
        }
    }
}
