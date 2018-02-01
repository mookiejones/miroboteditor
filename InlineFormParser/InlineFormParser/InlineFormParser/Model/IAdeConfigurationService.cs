namespace InlineFormParser.Model
{
	public interface IAdeConfigurationService
	{
		IAdeConfigSectionList Sections
		{
			get;
		}

		object ReadConfiguration(string sectionName);

		void WriteConfiguration(string sectionName, AdeComponentConfiguration configurationData);

		void WriteConfiguration(string sectionName, AdeComponentConfigurationListItem[] configurationData);

		void ResetUserSection(string sectionName);

		void ResetAllUserSections();
	}
}