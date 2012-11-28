/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/27/2012
 * Time: 10:20 AM
 * 
 */
using System;
using System.Collections.ObjectModel;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;

namespace miRobotEditor.GUI.ExplorerControl
{
	
	/// <summary>
	/// Interaction logic for ExplorerControlWPF.xaml
	/// </summary>
	public partial class ExplorerControlWPF : UserControl
	{
		
		//TODO Get ToolTip Based On FileInfo
		
		public ObservableCollection<string> FilterItems {get;set;}
				
		
		public ExplorerControlWPF()
		{
			
			InitializeComponent();
		}
		
	
		
		void _explorer_FileSelected(object sender, FileSelectedEventArgs e)
		{
				MainWindow.Instance.OpenFile(e.Filename);
		}
		
	}
	
	public class ExplorerArgs:EventArgs
	{
		
	}
	

}

