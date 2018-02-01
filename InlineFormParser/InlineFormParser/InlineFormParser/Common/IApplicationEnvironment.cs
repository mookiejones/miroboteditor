#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:38 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System.Diagnostics;
using System.Net;

#endregion

namespace InlineFormParser.Common
{
	public interface IApplicationEnvironment
	{
		string ApplicationConfigFile { get; }

		string ApplicationDirectory { get; }

		string ApplicationLogFile { get; }

		string ApplicationName { get; }

		string HelpDirectory { get; }

		KRCInstallationType InstallationType { get; }

		bool IsKRCDirectoryTreeValid { get; }

		bool IsMultiSubmitOptionSet { get; }

		string KRCDirectory { get; }

		string KRCReleaseVersion { get; }

		string KrcSystemConfigDirectory { get; }

		string KrcUserConfigDirectory { get; }

		string TechPackageDirectory { get; }

		string[] KxrDirectories { get; }

		string LogDirectory { get; }

		string UserConfigFile { get; }

		string UserDataDirectory { get; }

		string KRCMirrorPath { get; }

		string KRCLogPath { get; }

		string KRCSoftwareVersion { get; }

		IPAddress SharedMemoryVxWorksIP { get; }

		IPAddress SharedMemoryWindowsIP { get; }

		EventLog CreateApplicationEventLog(string source);

		string GetVersionInfo(VersionInfoElement element);

		string ExpandMacroPath(string pathName);
	}
}