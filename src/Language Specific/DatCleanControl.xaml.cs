/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/20/2012
 * Time: 10:30
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

namespace miRobotEditor.Language_Specific
{
	/// <summary>
	/// Interaction logic for DatCleanControl.xaml
	/// </summary>
	public partial class DatCleanControl : UserControl
	{
		private static DatCleanControl _instance;
		public static DatCleanControl Instance
		{
			get{
				if (_instance == null)
					_instance=new DatCleanControl();
				
				return _instance;}
		}
		public DatCleanControl()
		{
			InitializeComponent();
			_instance=this;
		}
		
		
		
		
	}
	
}