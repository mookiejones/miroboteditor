/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 4/8/2013
 * Time: 10:45 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */
using System;
using System.ComponentModel;
using System.Text;
using System.Reflection;
using System.Globalization;
using GalaSoft.MvvmLight;
using miRobotEditor.Core;

namespace miRobotEditor.ViewModel
{
	/// <summary>
	/// Description of AboutViewModel.
	/// </summary>
	[Localizable(false)]
	public class AboutViewModel:ViewModelBase
	{
        /// <summary>
        /// Copyright Information
        /// </summary>
        [Localizable(false)]
        public string Copyright{get{return "Copyright 2012-2013 Charles Berman";}}
        /// <summary>
        /// Name of Application
        /// </summary>
		public string Name{get{return Assembly.GetExecutingAssembly().GetName().Name;}}
        /// <summary>
        /// Version Number
        /// </summary>
		public string Version{get{return Assembly.GetExecutingAssembly().GetName().Version.ToString();}}


	    private const string _company = "DMC Automation";
	    public string Company{get { return _company; }}
	    public string NetVersion {get{return Environment.Version.ToString();}}
	    public string OSVersion{get{return Environment.OSVersion.ToString();}}
        /// <summary>
        /// Current Language
        /// </summary>
	    public  string CurrentCulture{get{return CultureInfo.CurrentCulture.Name;}}

	    public  string WorkingSetMemory 
		{
			get{
				return GC.GetTotalMemory(true).ToString(CultureInfo.InvariantCulture);
			}
			
		}
		public string Info{
			get{
				var sb = new StringBuilder();
				sb.AppendLine(" Name                : " + Name);
				sb.AppendLine(" .Net Version        : " + NetVersion);
				sb.AppendLine(" OS Version          : " + OSVersion);
				sb.AppendLine(" Current Culture     : " + CurrentCulture);
				sb.AppendLine(" Working Set Memory  : " + WorkingSetMemory);
				
				return sb.ToString();
			}
		}
	}
}
