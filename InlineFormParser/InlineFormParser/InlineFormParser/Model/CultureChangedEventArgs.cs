#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:26 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Globalization;

#endregion

namespace InlineFormParser.Model
{
	public class CultureChangedEventArgs : EventArgs
	{
		public CultureChangedEventArgs(CultureInfo oldCulture, CultureInfo newCulture, bool isAsync = false)
		{
			OldCulture = oldCulture ?? throw new ArgumentNullException(nameof(oldCulture));
			NewCulture = newCulture ?? throw new ArgumentNullException(nameof(newCulture));
			IsAsync = isAsync;
		}

		public CultureInfo OldCulture { get; }

		public CultureInfo NewCulture { get; }

		public bool IsAsync { get; }
	}
}