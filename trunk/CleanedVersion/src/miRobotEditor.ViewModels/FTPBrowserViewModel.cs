using miRobotEditor.Core;
using miRobotEditor.Core.Classes;

namespace miRobotEditor.Core
{
    public class FTPBrowserViewModel:ToolViewModel
    {
        public FTPBrowserViewModel() 
        {
        }
    }


    /// <summary>
    /// Description of FTPFolder.
    /// </summary>
    public class FTPFolder
    {

        public string Path { get; set; }
        public string Name { get; set; }
        //
        // Returns the folder name without having any path information
        //		
        [System.Diagnostics.DebuggerStepThrough]
        public static string SafeFolderName(string path)
        {
            var fileParts = path.Split('/');
            return fileParts[fileParts.Length - 1];
        }
    }
}
