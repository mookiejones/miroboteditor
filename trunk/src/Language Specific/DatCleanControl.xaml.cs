/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/20/2012
 * Time: 10:30
 * 
 */

using System.Windows.Controls;

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
			get { return _instance ?? (_instance = new DatCleanControl()); }
		}
		public DatCleanControl()
		{
			InitializeComponent();
			_instance=this;
		}
		
		
		
		
	}
	
}