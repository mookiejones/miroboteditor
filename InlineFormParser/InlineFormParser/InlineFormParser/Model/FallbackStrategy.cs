#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:32 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace InlineFormParser.Model
{
	public static class FallbackStrategy
	{
		private const string defaultLanguageCode = "en";

		public static string[] GetLanguageFilter(string requestedCode)
		{
			if (string.IsNullOrEmpty(requestedCode)) throw new ArgumentNullException(nameof(requestedCode));

			var list = new List<string> {requestedCode};
			var languageCode = Helpers.GetLanguageCode(requestedCode);
			if (Helpers.HasRegionCode(requestedCode))
			{
				if (Helpers.AreCodesEqual(Helpers.GetRegionCode(requestedCode), Helpers.DeveloperRegionCode))
					throw new ArgumentException("requestedCode must not have developer region code");
				list.Add(languageCode);
			}

			list.Add(Helpers.MakeFullCode(languageCode, Helpers.DeveloperRegionCode));
			if (!Helpers.AreCodesEqual(languageCode, "en"))
			{
				list.Add("en");
				list.Add(Helpers.MakeFullCode("en", Helpers.DeveloperRegionCode));
			}

			return list.ToArray();
		}
	}
}