/*
 * Created by SharpDevelop.
 * User: cberman
 * Date: 11/21/2012
 * Time: 10:44 AM
 * 
 */
using System;
using System.Collections.Generic;
using miRobotEditor.Classes;
namespace miRobotEditor.Services
{
	/// <summary>
	/// Description of VariableService.
	/// </summary>
	public static class VariableService
	{

	
	
	private static List<IVariable> GetVariables()
	{
		List<IVariable> result = new List<IVariable>();
		
		return result;
	}
	public static List<IVariable> GetAllVariables()
	{
		return GetVariables();
	}
	public static List<IVariable> GetPositionVariables()
	{
		return GetVariables();
	}
	}
}
