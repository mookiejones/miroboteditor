namespace InlineFormParser.Model
{
	public interface IAdeConfigSectionList
	{
		IAdeConfigSection this[string sectionName]
		{
			get;
		}
	}
}