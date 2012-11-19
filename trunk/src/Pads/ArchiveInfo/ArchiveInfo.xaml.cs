/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/16/2012
 * Time: 6:44 AM
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

namespace miRobotEditor.Pads
{
	/// <summary>
	/// Interaction logic for ArchiveInfo.xaml
	/// </summary>
	public partial class ArchiveInfo : UserControl
	{
		public ArchiveInfo()
		{
			InitializeComponent();
		}
		
		void ListView_TargetUpdated(object sender, DataTransferEventArgs e)
		{
			var view = (sender as ListView).View as GridView;
 			 AutoResizeGridViewColumns(view);
		}
		static void AutoResizeGridViewColumns(GridView view)
		{
		  if (view == null || view.Columns.Count < 1) return;
		  // Simulates column auto sizing
		  foreach (var column in view.Columns)
		  {
		 	// Forcing change
			 if (double.IsNaN(column.Width))
		   		column.Width = 1;
		 	
			 column.Width = double.NaN;
		  }
		}
	}
}