/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/2/2012
 * Time: 7:16 AM
 * 
 */

using System.Windows.Controls;

namespace miRobotEditor.GUI.AngleConverter
{
	/// <summary>
	/// Interaction logic for AngleConverterWPF.xaml
	/// </summary>
	public partial class AngleConverterWpf : AvalonDock.Layout.LayoutAnchorable
	{
		private static AngleConverterWpf _instance;
		public static AngleConverterWpf Instance
		{
			get { return _instance ?? (_instance = new AngleConverterWpf()); }
		    set{_instance=value;}
		}
		public AngleConverterWpf()
		{
			Instance=this;
			InitializeComponent();
			
		}    
	}
}