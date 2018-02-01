using System.Diagnostics.CodeAnalysis;

namespace InlineFormParser.Model
{
	[SuppressMessage("Microsoft.Design", "CA1003:UseGenericEventHandlerInstances")]
	public delegate void SectionChangedEventHandler(object sender, SectionChangedEventArgs e);
}