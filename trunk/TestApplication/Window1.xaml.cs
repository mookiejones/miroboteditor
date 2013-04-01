/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 3/28/2013
 * Time: 6:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

using System.Windows;

namespace TestApplication
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

	    private void TestItem(object sender, RoutedEventArgs e)
	    {
	    	FanucREPCDViewModel.Instance.REPTCDItems=  	FanucTools.FanucDiagnostics.GetREPTCD();
	    }
	}
}