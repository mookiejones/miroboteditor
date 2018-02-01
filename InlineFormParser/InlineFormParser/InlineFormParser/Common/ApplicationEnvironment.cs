#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:37 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Xml.XPath;
using Microsoft.Win32;

#endregion

namespace InlineFormParser.Common
{
	public class ApplicationEnvironment : IApplicationEnvironment
	{
		private const string knetConfigFile = "Roboter\\Config\\User\\Common\\VxWin\\knet.config";

		private const string krcXmlFile = "Roboter\\Config\\System\\Common\\KRC.xml";

		private const string krcConfigFile = "Roboter\\Config\\System\\Common\\ConfigXML.xml";

		private const string krcMessageResourceFile = "Bin\\KrcEventLogMessages.dll";

		private const string versionInfoRegistryPath = "Software\\KUKA Roboter GmbH\\Version\\";

		private const string versionIniFileName = "Version.ini";

		private const string relativeTpDirectory = "TP\\";

		private static readonly Regex MakroRegex = new Regex("%(?<Meta>\\S+)%[\\\\/](?<Path>.+)", RegexOptions.Compiled);

		private static IApplicationEnvironment instance;

		private string krcReleaseVersion;

		private string krcSoftwareVersion;

		private readonly string kxrDataDirectory;

		private IPAddress sharedMemoryVxWorksIP;

		private IPAddress sharedMemoryWindowsIP;

		private Dictionary<VersionInfoElement, string> versionInfos;

		private ApplicationEnvironment()
		{
			GetApplicationInformation();
			IsKRCDirectoryTreeValid = false;
			GetKrcDirectory();
			UserDataDirectory = Path.Combine(KRCDirectory, "User");
			kxrDataDirectory = Path.Combine(KRCDirectory, "Data");
			if (Directory.Exists(UserDataDirectory) && Directory.Exists(kxrDataDirectory)) IsKRCDirectoryTreeValid = true;
			if (!IsKRCDirectoryTreeValid)
			{
				KRCDirectory = string.Empty;
				KrcUserConfigDirectory = KrcSystemConfigDirectory = string.Empty;
				KRCMirrorPath = string.Empty;
				UserDataDirectory = Path.Combine(ApplicationDirectory, "User");
				if (!Directory.Exists(UserDataDirectory)) UserDataDirectory = ApplicationDirectory;
			}

			var path = ApplicationName + ".User.config";
			UserConfigFile = Path.Combine(UserDataDirectory, path);
			if (!File.Exists(UserConfigFile)) CreateUserConfigFile(UserConfigFile);
			LogDirectory = ApplicationDirectory;
			ApplicationLogFile = Path.Combine(LogDirectory, ApplicationName + ".log");
			if (IsKRCDirectoryTreeValid)
			{
				HelpDirectory = Path.Combine(KRCDirectory, "Help");
			}
			else
			{
				HelpDirectory = Path.Combine(ApplicationDirectory, "Help");
				if (!Directory.Exists(HelpDirectory)) HelpDirectory = ApplicationDirectory;
			}

			SetTpDirectory();
			CreateKxrDirectories();
			InstallationType = KRCInstallationType.Unknown;
			if (IsKRCDirectoryTreeValid && !string.IsNullOrEmpty(KRCReleaseVersion))
			{
				var text = Path.Combine(KRCDirectory, "Roboter\\Config\\System\\Common\\ConfigXML.xml");
				if (File.Exists(text)) ReadConfigFile(text);
				InitializeDirectories();
			}

			IsMultiSubmitOptionSet = false;
			var text2 = Path.Combine(KRCDirectory, "Roboter\\Config\\System\\Common\\KRC.xml");
			if (File.Exists(text2)) ReadKrcXmlConfig(text2);
		}

		public static IApplicationEnvironment Current
		{
			get => instance ?? (instance = new ApplicationEnvironment());
			set => instance = value;
		}

		public string ApplicationConfigFile { get; private set; }

		public string ApplicationDirectory { get; private set; }

		public string ApplicationLogFile { get; }

		public string ApplicationName { get; private set; }

		public string HelpDirectory { get; }

		public KRCInstallationType InstallationType { get; private set; }

		public bool IsKRCDirectoryTreeValid { get; }

		public bool IsMultiSubmitOptionSet { get; private set; }

		public string KRCDirectory { get; private set; }

		public string KRCMirrorPath { get; private set; }

		public string KRCLogPath { get; private set; }

		public string TechPackageDirectory { get; private set; }

		public string KRCReleaseVersion
		{
			get
			{
				if (krcReleaseVersion == null)
				{
					var stringBuilder = new StringBuilder();
					stringBuilder.Append(string.Format(CultureInfo.InvariantCulture, "{0} {1}",
						GetVersionInfo(VersionInfoElement.KRCVersion), GetVersionInfo(VersionInfoElement.Version)).Trim());
					var versionInfo = GetVersionInfo(VersionInfoElement.Build);
					if (!string.IsNullOrEmpty(versionInfo))
					{
						if (stringBuilder.Length > 0) stringBuilder.Append(' ');
						stringBuilder.AppendFormat("B{0}", versionInfo);
					}

					krcReleaseVersion = stringBuilder.ToString();
				}

				return krcReleaseVersion;
			}
		}

		public string KRCSoftwareVersion
		{
			get
			{
				if (krcSoftwareVersion == null)
				{
					var versionInfo = GetVersionInfo(VersionInfoElement.Version);
					var num = 0;
					var num2 = versionInfo.IndexOf(" ", StringComparison.OrdinalIgnoreCase) - 1;
					if (num2 < 0) num2 = versionInfo.Length - 1;
					if (versionInfo.StartsWith("V", StringComparison.Ordinal)) num = 1;
					krcSoftwareVersion = versionInfo.Substring(num, num2 - num + 1);
				}

				return krcSoftwareVersion;
			}
		}

		public string KrcSystemConfigDirectory { get; private set; }

		public string KrcUserConfigDirectory { get; private set; }

		public string[] KxrDirectories { get; private set; }

		public string LogDirectory { get; }

		public IPAddress SharedMemoryVxWorksIP =>
			sharedMemoryVxWorksIP ?? (sharedMemoryVxWorksIP = ReadIPAddressFromKnet("\"VXWORKS\""));

		public IPAddress SharedMemoryWindowsIP =>
			sharedMemoryWindowsIP ?? (sharedMemoryWindowsIP = ReadIPAddressFromKnet("\"WINDOWS\""));

		public string UserConfigFile { get; }

		public string UserDataDirectory { get; }

		public EventLog CreateApplicationEventLog(string source)
		{
			if (string.IsNullOrEmpty(source)) throw new ArgumentNullException(nameof(source));
			var source2 = string.Format(CultureInfo.InvariantCulture, "{0}.{1}", ApplicationName, source);
			EventLog eventLog = null;
			if (!EventLog.SourceExists(source2))
			{
				var eventSourceCreationData = new EventSourceCreationData(source2, ApplicationName);
				if (IsKRCDirectoryTreeValid)
				{
					var text = Path.Combine(KRCDirectory, "Bin\\KrcEventLogMessages.dll");
					if (File.Exists(text)) eventSourceCreationData.MessageResourceFile = text;
				}

				EventLog.CreateEventSource(eventSourceCreationData);
				var eventLog2 = new EventLog();
				eventLog2.Source = source2;
				eventLog = eventLog2;
				eventLog.ModifyOverflowPolicy(OverflowAction.OverwriteAsNeeded, 1);
				eventLog.MaximumKilobytes = 1024L;
			}
			else
			{
				var eventLog3 = new EventLog();
				eventLog3.Source = source2;
				eventLog = eventLog3;
			}

			return eventLog;
		}

		public string GetVersionInfo(VersionInfoElement element)
		{
			if (versionInfos == null)
			{
				versionInfos = new Dictionary<VersionInfoElement, string>();
				ReadVersionInfos();
			}

			if (versionInfos.ContainsKey(element)) return versionInfos[element];
			return string.Empty;
		}

		public string ExpandMacroPath(string pathName)
		{
			var match = MakroRegex.Match(pathName);
			var result = pathName;
			if (match.Success)
			{
				var value = match.Groups["Meta"].Value;
				var value2 = match.Groups["Path"].Value;
				if (string.Compare(value, "InstallationDir", StringComparison.OrdinalIgnoreCase) == 0)
					result = Path.Combine(KRCDirectory, value2);
			}
			else
			{
				result = Path.Combine(KRCDirectory, pathName);
			}

			return result;
		}

		public static string ReadIniFileValue(string filePath, string section, string key)
		{
			var stringBuilder = new StringBuilder(255);
			var privateProfileString =
				NativeMethods.GetPrivateProfileString(section, key, string.Empty, stringBuilder, stringBuilder.Capacity, filePath);
			if (privateProfileString <= 0) return null;
			return stringBuilder.ToString();
		}

		private static string GetVersionIniKeyForElement(VersionInfoElement element)
		{
			switch (element)
			{
				case VersionInfoElement.Version:
				case VersionInfoElement.Build:
				case VersionInfoElement.Customer:
					return element.ToString();
				case VersionInfoElement.KRCVersion:
					return "Name";
				default:
					return null;
			}
		}

		private static bool TryGetParentDirectory(string path, out string parent)
		{
			try
			{
				var parent2 = Directory.GetParent(path);
				if (parent2 != null && parent2.Exists &&
				    string.Compare(parent2.FullName, path, StringComparison.OrdinalIgnoreCase) != 0)
				{
					parent = parent2.FullName;
					return true;
				}
			}
			catch
			{
			}

			parent = string.Empty;
			return false;
		}

		private static void CreateUserConfigFile(string userConfigFilePath)
		{
			try
			{
				using (var streamWriter = new StreamWriter(userConfigFilePath))
				{
					streamWriter.WriteLine("<configuration />");
				}
			}
			catch
			{
			}
		}

		private void SetTpDirectory()
		{
			TechPackageDirectory = Path.Combine(KRCDirectory, "TP\\");
		}

		private void GetApplicationInformation()
		{
			var assembly = typeof(ApplicationEnvironment).Assembly;
			ApplicationDirectory = Path.GetDirectoryName(assembly.Location);
			var entryAssembly = Assembly.GetEntryAssembly();
			if (entryAssembly != null)
			{
				ApplicationName = entryAssembly.GetName().Name;
				ApplicationConfigFile = entryAssembly.Location + ".config";
			}
			else
			{
				var fileName = Process.GetCurrentProcess().MainModule.FileName;
				ApplicationName = Path.GetFileNameWithoutExtension(fileName);
				ApplicationConfigFile = Path.GetFileName(fileName) + ".config";
			}
		}

		private void InitializeDirectories()
		{
			var path = Path.Combine(KRCDirectory, "Roboter");
			KrcUserConfigDirectory = Path.Combine(path, "Config\\User");
			KrcSystemConfigDirectory = Path.Combine(path, "Config\\System");
			KRCMirrorPath = Path.Combine(path, "KRC");
			KRCLogPath = Path.Combine(path, "LOG");
		}

		private void ReadKrcXmlConfig(string krcXmlConfigFilePath)
		{
			try
			{
				using (var stream = File.OpenRead(krcXmlConfigFilePath))
				{
					var xPathDocument = new XPathDocument(stream);
					var xPathNavigator = xPathDocument.CreateNavigator();
					var text = xPathNavigator.Evaluate("string(/KRC/MultiSubmit/@Enable)") as string;
					if (!string.IsNullOrEmpty(text) && text.Trim().ToUpper(CultureInfo.InvariantCulture) == "TRUE")
						IsMultiSubmitOptionSet = true;
				}
			}
			catch (Exception)
			{
			}
		}

		private void ReadConfigFile(string configFilePath)
		{
			try
			{
				using (var stream = File.OpenRead(configFilePath))
				{
					var xPathDocument = new XPathDocument(stream);
					var xPathNavigator = xPathDocument.CreateNavigator();
					var text = xPathNavigator.Evaluate("string(/ConfigList/Environment/@EnvPath)") as string;
					if (!string.IsNullOrEmpty(text))
					{
						var text2 = text.Trim().ToUpper(CultureInfo.InvariantCulture);
						string a;
						if ((a = text2) != null)
							if (!(a == "CABINET/"))
								if (!(a == "OFFICE/"))
								{
									if (a == "OFFICELITE/") InstallationType = KRCInstallationType.OfficeLite;
								}
								else
								{
									InstallationType = KRCInstallationType.Office;
								}
							else
								InstallationType = KRCInstallationType.Cabinet;
					}
				}
			}
			catch (Exception)
			{
			}
		}

		private void GetKrcDirectory()
		{
			KRCDirectory = GetVersionInfo(VersionInfoElement.InstallationDir);
			if (string.IsNullOrEmpty(KRCDirectory))
			{
				var text = ApplicationDirectory;
				while (TryGetParentDirectory(text, out text))
				{
					var text2 = text.Substring(text.LastIndexOf('\\') + 1);
					if (text2.IndexOf("KRC", StringComparison.OrdinalIgnoreCase) >= 0)
					{
						KRCDirectory = text;
						break;
					}
				}
			}

			if (string.IsNullOrEmpty(KRCDirectory)) KRCDirectory = "C:\\Krc\\";
		}

		private IPAddress ReadIPAddressFromKnet(string ipKey)
		{
			var text = Path.Combine(KRCDirectory, "Roboter\\Config\\User\\Common\\VxWin\\knet.config");
			if (!File.Exists(text)) return null;
			var result = default(IPAddress);
			if (IPAddress.TryParse(ReadIniFileValue(text, "HostTable", ipKey), out result)) return result;
			return null;
		}

		private void ReadVersionInfos()
		{
			versionInfos.Clear();
			var flag = false;
			try
			{
				using (var registryKey = Registry.LocalMachine.OpenSubKey("Software\\KUKA Roboter GmbH\\Version\\", false))
				{
					if (registryKey != null)
						foreach (VersionInfoElement value in Enum.GetValues(typeof(VersionInfoElement)))
						{
							var text = registryKey.GetValue(value.ToString()) as string;
							if (text != null)
							{
								versionInfos[value] = text;
								flag = true;
							}
						}
				}
			}
			catch (Exception)
			{
			}

			if (!flag)
			{
				var text2 = Path.Combine(ApplicationDirectory, "Version.ini");
				if (File.Exists(text2))
					foreach (VersionInfoElement value2 in Enum.GetValues(typeof(VersionInfoElement)))
					{
						var versionIniKeyForElement = GetVersionIniKeyForElement(value2);
						if (versionIniKeyForElement != null)
						{
							var text3 = ReadIniFileValue(text2, "Version", versionIniKeyForElement);
							if (text3 != null) versionInfos[value2] = text3;
						}
					}
			}
		}

		private void CreateKxrDirectories()
		{
			var list = new List<string>();
			var text = Path.Combine(ApplicationDirectory, "Resources");
			if (Directory.Exists(text)) list.Add(text);
			var text2 = Path.GetDirectoryName(Assembly.GetExecutingAssembly().CodeBase);
			if (text2.StartsWith("file:\\", StringComparison.Ordinal)) text2 = text2.Substring(6);
			text = Path.Combine(text2, "Resources");
			if (Directory.Exists(text) && !list.Contains(text)) list.Add(text);
			if (IsKRCDirectoryTreeValid)
			{
				var text3 = Path.Combine(UserDataDirectory, "Data");
				if (!Directory.Exists(text3)) Directory.CreateDirectory(text3);
				list.Add(text3);
				list.Add(Path.Combine(KRCDirectory, "Data"));
			}
			else if (list.Count == 0)
			{
				list.Add(text2);
			}

			if (Directory.Exists(TechPackageDirectory) && !list.Contains(TechPackageDirectory)) list.Add(TechPackageDirectory);
			KxrDirectories = list.ToArray();
		}

		private static class NativeMethods
		{
			[DllImport("kernel32", CharSet = CharSet.Unicode)]
			internal static extern int GetPrivateProfileString(string section, string key, string def, StringBuilder retVal,
				int size, string filePath);
		}
	}
}