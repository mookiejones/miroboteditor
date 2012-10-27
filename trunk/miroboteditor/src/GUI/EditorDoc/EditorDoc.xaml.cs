/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/26/2012
 * Time: 6:26 PM
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
using miRobotEditor.Classes;
using miRobotEditor.Controls;
namespace miRobotEditor.GUI.EditorDoc
{
	/// <summary>
	/// Interaction logic for EditorDoc.xaml
	/// </summary>
	public partial class EditorDoc : UserControl
	{
		
		public event miRobotEditor.Controls.UpdateFunctionEventHandler UpdateFunctions;
		public event EventHandler TextChanged;
		
		public EditorDoc()
		{
			InitializeComponent();
			dataeditor.TextChanged+= new EventHandler(dataeditor_TextChanged);
			dataeditor.UpdateFunctions+= new UpdateFunctionEventHandler(dataeditor_UpdateFunctions);
		}

		void dataeditor_TextChanged(object sender, EventArgs e)
		{
			TextChanged(sender,e);
		}

		void dataeditor_UpdateFunctions(object sender, FunctionEventArgs e)
		{
			UpdateFunctions(sender,e);
		}
	}
}