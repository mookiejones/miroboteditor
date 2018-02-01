#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:18 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.Diagnostics;

#endregion

namespace InlineFormParser.Zip
{
	internal abstract class SelectionCriterion
	{
		internal virtual bool Verbose { get; set; }

		internal abstract bool Evaluate(string filename);

		[Conditional("SelectorTrace")]
		protected static void CriterionTrace(string format, params object[] args)
		{
			Console.WriteLine("  {0}", format);
		}

		internal abstract bool Evaluate(ZipEntry entry);
	}
}