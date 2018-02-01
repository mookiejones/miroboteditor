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

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace InlineFormParser.Model
{
	public interface ILoadedModule
	{
		string Name { get; }

		string Shortcut { get; }

		int NumEntries { get; }

		IEnumerable<IModuleElement> Entries { get; }

		DateTime LoadingTime { get; }

		IEnumerable<string> ReadFilePathes { get; }

		string[] LanguageFilter { get; }

		string[] GetContainedLanguages();
	}
}