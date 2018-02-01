using System.Windows.Controls;

namespace InlineFormParser.Model
{
	public static class TabControlExtension
	{
		public static void DisableTabItemContent(this TabControl element)
		{
			element.SelectionChanged += ElementOnSelectionChanged;
			EnableTabControlContent(element, false);
		}

		public static void EnableTabItemContent(this TabControl element)
		{
			element.SelectionChanged -= ElementOnSelectionChanged;
			EnableTabControlContent(element, true);
		}

		private static void ElementOnSelectionChanged(object sender, SelectionChangedEventArgs selectionChangedEventArgs)
		{
			EnableTabControlContent((TabControl)sender, true);
		}

		private static void EnableTabControlContent(TabControl tabControl, bool enable)
		{
			if (tabControl.SelectedContent is ContentControl)
			{
				(tabControl.SelectedContent as ContentControl).IsEnabled = enable;
			}
		}
	}
}