/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/2/2012
 * Time: 7:16 AM
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

namespace miRobotEditor.GUI.AngleConverter
{
	/// <summary>
	/// Interaction logic for AngleConverterWPF.xaml
	/// </summary>
	public partial class AngleConverterWPF : UserControl
	{
		private AngleConvertor Value = new AngleConvertor();
		public AngleConverterWPF()
		{
			InitializeComponent();
			this.DataContext=Value;
			
		}
	}
}