/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/14/2012
 * Time: 15:55
 * 
 */
using System;

namespace FTPBrowser
{
	/// <summary>
	/// Description of FTPFolder.
	/// </summary>
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
