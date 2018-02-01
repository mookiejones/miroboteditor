#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:33 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public interface IModuleElement
	{
		ILoadedModule Module { get; }

		string ModuleName { get; }

		string Key { get; }

		string Text { get; }

		string LanguageCode { get; }

		int MessageNumber { get; }

		MessageType MessageType { get; }
	}
}