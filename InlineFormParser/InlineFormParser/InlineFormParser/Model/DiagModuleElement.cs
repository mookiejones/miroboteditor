#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:40 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public class DiagModuleElement : IModuleElement
	{
		private readonly TextEntry entry;
		private readonly LoadedModule module;

		internal DiagModuleElement(LoadedModule module, string key)
		{
			this.module = module;
			Key = key;
			entry = module.GetTextEntry(key);
		}

		public ILoadedModule Module => module;

		public string ModuleName => module.Name;

		public string Key { get; }

		public string Text => entry.Text;

		public string LanguageCode => module.GetLanguageOfEntry(entry);

		public int MessageNumber => entry.MessageNumber;

		public MessageType MessageType => entry.MessageType;
	}
}