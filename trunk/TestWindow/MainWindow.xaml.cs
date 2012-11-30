/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/16/2012
 * Time: 07:00
 * 
 */
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace TestWindow
{
	/// <summary>
	/// Interaction logic for Window1.xaml
	/// </summary>
	public partial class Window1 : Window
	{
		
		public Window1()
		{
			InitializeComponent();
		}
		
		void LoadArchiveClick(object sender, RoutedEventArgs e)
		{	
			//miRobotEditor.Pads.ArchiveInfo.ArchiveInfoViewModel helper = new miRobotEditor.Pads.ArchiveInfo.ArchiveInfoViewModel();
		//	this.DataContext=miRobotEditor.Pads.ArchiveInfo.ArchiveInfoViewModel.Instance;
			
		//	this.DataContext = new miRobotEditor.Pads.ArchiveInfo.ArchiveInfoViewModel(this);			
		}
	}
}