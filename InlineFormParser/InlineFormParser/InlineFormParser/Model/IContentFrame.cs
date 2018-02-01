using System.Windows;

namespace InlineFormParser.Model
{
	public interface IContentFrame
	{
		UIElement Content
		{
			get;
			set;
		}
	}
}