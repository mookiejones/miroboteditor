#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:38 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;

#endregion

namespace InlineFormParser.Common.LockHandling
{
	public interface ILockCondition
	{
		bool IsConditionFullfilled { get; }

		IList<object> Targets { get; }

		event EventHandler RequerySuggested;

		void RaiseRequerySuggested(EventArgs e);

		void GetLockInfo(out IEnumerable<string> result);
	}
}