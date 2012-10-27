/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 9/23/2012
 * Time: 9:58 AM
 * 
 * To change this template use Tools | Options | Coding | Edit Standard Headers.
 */

namespace miRobotEditor.Interfaces
{
		/// <summary>
	/// An item in the insight window.
	/// </summary>
	public interface IInsightItem
	{
		object Header { get; }
		object Content { get; }
	}
}
