using System;
using System.Windows.Controls;

namespace InlineFormParser.Model
{
	public interface IInlineFormFactory
	{
		int InlineFormCommandMessage
		{
			get;
		}

		int InlineFormNotificationMessage
		{
			get;
		}

		IntPtr CreateInlineFormWindow(string xmlDescription, IntPtr hwndParent, int x, int y, int width);

		IntPtr CreateInlineFormWindow(Control control, IntPtr hwndParent, int x, int y);

		T CreateInlineFormWPFControl<T>(string xmlDescription, int width);

		void UpdateInlineFormWPFControl<T>(T control, string xmlDescription);

		InlineForm CreateInlineForm(string xmlDescription);
	}
}