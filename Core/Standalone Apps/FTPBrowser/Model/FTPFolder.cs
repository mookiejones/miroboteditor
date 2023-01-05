using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using CommunityToolkit.Mvvm.ComponentModel;

namespace FTPBrowser.Model
{
    /// <summary>
	/// Description of FTPAccount.
	/// </summary>
	[Serializable]
    public class FTPFolder
    {
       	
    	public string Path{get;set;}
    	public string Name{get;set;}   

        	//
        // Returns the folder name without having any path information
        //		
        [System.Diagnostics.DebuggerStepThrough()]
		public static string SafeFolderName(string path)
		{
			var fileParts = path.Split('/');
			return fileParts[fileParts.Length-1];
		}
    }
}