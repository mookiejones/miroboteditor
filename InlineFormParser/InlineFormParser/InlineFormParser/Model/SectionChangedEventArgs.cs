using System;

namespace InlineFormParser.Model
{
	public class SectionChangedEventArgs : EventArgs
	{
		public string SectionName
		{
			get;
			private set;
		}

		public object ConfigurationSection
		{
			get;
			private set;
		}

		public SectionChangedEventArgs(string sectionName, object configurationSection)
		{
			SectionName = sectionName;
			ConfigurationSection = configurationSection;
		}
	}
}