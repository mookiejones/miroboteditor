#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:7:52 AM
// Modified:2018:02:01:9:48 AM:

#endregion

namespace InlineFormParser.Model
{
	public interface ICanIncDec
	{
		bool CanIncrement { get; }

		bool CanDecrement { get; }

		void Increment();

		void Decrement();
	}
}