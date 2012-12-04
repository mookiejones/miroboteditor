/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/15/2012
 * Time: 12:13
 * 
 */
using System;
using System.ComponentModel;
namespace FTPBrowser
{
	/// <summary>
	/// Description of ViewModelBase.
	/// </summary>
	public class ViewModelBase:INotifyPropertyChanged
	{
		public void OnPropertyChanged(string propertyName)
		{
			if (PropertyChanged!=null)
				PropertyChanged(this,new PropertyChangedEventArgs(propertyName));
		}
		public event PropertyChangedEventHandler PropertyChanged;
	}
}
