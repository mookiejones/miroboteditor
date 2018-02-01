using System.Collections;

namespace InlineFormParser.Model
{
	public interface IAdeCommandProvider
	{
		IEnumerable Commands
		{
			get;
		}
	}
}