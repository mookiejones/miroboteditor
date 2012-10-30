﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using Microsoft.Win32;
using miRobotEditor.Core.EventArgs;
using miRobotEditor.Core.Services;

namespace miRobotEditor.Core
{
    /// <summary>
    /// FileErrorPolicy
    /// </summary>
    public enum FileErrorPolicy
    { 
        /// <summary>
        /// 
        /// </summary>
        Inform,
        /// <summary>
        /// 
        /// </summary>
        ProvideAlternative
    }

    /// <summary>
    /// 
    /// </summary>
    public enum FileOperationResult
    {
        /// <summary>
        /// 
        /// </summary>
        OK,
        /// <summary>
        /// 
        /// </summary>
        Failed,
        /// <summary>
        /// 
        /// </summary>
        SavedAlternatively
    }
    /// <summary>
    /// 
    /// </summary>
    public delegate void FileOperationDelegate();
    /// <summary>
    /// 
    /// </summary>
    /// <param name="fileName"></param>
    public delegate void NamedFileOperationDelegate(string fileName);
	
    /// <summary>
    /// A utility class related to file utilities.
    /// </summary>
    public static partial class FileUtility
    {
        	/// <summary>
		/// Gets the normalized version of fileName.
		/// Slashes are replaced with backslashes, backreferences "." and ".." are 'evaluated'.
		/// </summary>
		public static string NormalizePath(string fileName)
		{
			if (string.IsNullOrEmpty(fileName)) return fileName;
			
			int i;
			
			bool isWeb = false;
			for (i = 0; i < fileName.Length; i++) {
				if (fileName[i] == '/' || fileName[i] == '\\')
					break;
				if (fileName[i] == ':') {
					if (i > 1)
						isWeb = true;
					break;
				}
			}
			
			char outputSeparator = isWeb ? '/' : System.IO.Path.DirectorySeparatorChar;
			
			StringBuilder result = new StringBuilder();
			if (isWeb == false && fileName.StartsWith(@"\\") || fileName.StartsWith("//")) {
				i = 2;
				result.Append(outputSeparator);
			} else {
				i = 0;
			}
			int segmentStartPos = i;
			for (; i <= fileName.Length; i++) {
				if (i == fileName.Length || fileName[i] == '/' || fileName[i] == '\\') {
					int segmentLength = i - segmentStartPos;
					switch (segmentLength) {
						case 0:
							// ignore empty segment (if not in web mode)
							// On unix, don't ignore empty segment if i==0
							if (isWeb || (i == 0 && Environment.OSVersion.Platform == PlatformID.Unix)) {
								result.Append(outputSeparator);
							}
							break;
						case 1:
							// ignore /./ segment, but append other one-letter segments
							if (fileName[segmentStartPos] != '.') {
								if (result.Length > 0) result.Append(outputSeparator);
								result.Append(fileName[segmentStartPos]);
							}
							break;
						case 2:
							if (fileName[segmentStartPos] == '.' && fileName[segmentStartPos + 1] == '.') {
								// remove previous segment
								int j;
								for (j = result.Length - 1; j >= 0 && result[j] != outputSeparator; j--);
								if (j > 0) {
									result.Length = j;
								}
								break;
							} else {
								// append normal segment
								goto default;
							}
						default:
							if (result.Length > 0) result.Append(outputSeparator);
							result.Append(fileName, segmentStartPos, segmentLength);
							break;
					}
					segmentStartPos = i + 1; // remember start position for next segment
				}
			}
			if (isWeb == false) {
				if (result.Length > 0 && result[result.Length - 1] == outputSeparator) {
					result.Length -= 1;
				}
				if (result.Length == 2 && result[1] == ':') {
					result.Append(outputSeparator);
				}
			}
			return result.ToString();
		}
		
		public static bool IsEqualFileName(string fileName1, string fileName2)
		{
			return string.Equals(NormalizePath(fileName1),
			                     NormalizePath(fileName2),
			                     StringComparison.OrdinalIgnoreCase);
		}
		
		public static bool IsBaseDirectory(string baseDirectory, string testDirectory)
		{
			if (baseDirectory == null || testDirectory == null)
				return false;
			baseDirectory = NormalizePath(baseDirectory) + Path.DirectorySeparatorChar;
			testDirectory = NormalizePath(testDirectory) + Path.DirectorySeparatorChar;
			
			return testDirectory.StartsWith(baseDirectory, StringComparison.OrdinalIgnoreCase);
		}
        readonly static char[] separators = { Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar, Path.VolumeSeparatorChar };
        static string applicationRootPath = AppDomain.CurrentDomain.BaseDirectory;
        const string fileNameRegEx = @"^([a-zA-Z]:)?[^:]+$";
        /// <summary>
        /// Application Root Path
        /// </summary>
        public static string ApplicationRootPath
        {
            get
            {
                return applicationRootPath;
            }
            set
            {
                applicationRootPath = value;
            }
        }

        static string GetPathFromRegistry(string key, string valueName)
        {
            using (RegistryKey installRootKey = Registry.LocalMachine.OpenSubKey(key))
            {
                if (installRootKey != null)
                {
                    object o = installRootKey.GetValue(valueName);
                    if (o != null)
                    {
                        string r = o.ToString();
                        if (!string.IsNullOrEmpty(r))
                            return r;
                    }
                }
            }
            return null;
        }

        static string GetPathFromRegistryX86(string key, string valueName)
		{
			using (RegistryKey baseKey = RegistryKey.OpenBaseKey(RegistryHive.LocalMachine, RegistryView.Registry32)) {
				using (RegistryKey installRootKey = baseKey.OpenSubKey(key)) {
					if (installRootKey != null) {
						object o = installRootKey.GetValue(valueName);
						if (o != null) {
							string r = o.ToString();
							if (!string.IsNullOrEmpty(r))
								return r;
						}
					}
				}
			}
			return null;
		}
        #region InstallRoot Properties

        static string netFrameworkInstallRoot = null;
        /// <summary>
        /// Gets the installation root of the .NET Framework (@"C:\Windows\Microsoft.NET\Framework\")
        /// </summary>
        public static string NetFrameworkInstallRoot
        {
            get
            {
                if (netFrameworkInstallRoot == null)
                {
                    netFrameworkInstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\.NETFramework", "InstallRoot") ?? string.Empty;
                }
                return netFrameworkInstallRoot;
            }
        }

        static string netSdk20InstallRoot = null;
        /// <summary>
        /// Location of the .NET 2.0 SDK install root.
        /// </summary>
        public static string NetSdk20InstallRoot
        {
            get
            {
                if (netSdk20InstallRoot == null)
                {
                    netSdk20InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\.NETFramework", "sdkInstallRootv2.0") ?? string.Empty;
                }
                return netSdk20InstallRoot;
            }
        }

        static string windowsSdk60InstallRoot = null;
        /// <summary>
        /// Location of the .NET 3.0 SDK (Windows SDK 6.0) install root.
        /// </summary>
        public static string WindowsSdk60InstallRoot
        {
            get
            {
                if (windowsSdk60InstallRoot == null)
                {
                    windowsSdk60InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk60InstallRoot;
            }
        }

        static string windowsSdk60aInstallRoot = null;
        /// <summary>
        /// Location of the Windows SDK Components in Visual Studio 2008 (.NET 3.5; Windows SDK 6.0a).
        /// </summary>
        public static string WindowsSdk60aInstallRoot
        {
            get
            {
                if (windowsSdk60aInstallRoot == null)
                {
                    windowsSdk60aInstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.0a", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk60aInstallRoot;
            }
        }

        static string windowsSdk61InstallRoot = null;
        /// <summary>
        /// Location of the .NET 3.5 SDK (Windows SDK 6.1) install root.
        /// </summary>
        public static string WindowsSdk61InstallRoot
        {
            get
            {
                if (windowsSdk61InstallRoot == null)
                {
                    windowsSdk61InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v6.1", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk61InstallRoot;
            }
        }

        static string windowsSdk70InstallRoot = null;
        /// <summary>
        /// Location of the .NET 3.5 SP1 SDK (Windows SDK 7.0) install root.
        /// </summary>
        public static string WindowsSdk70InstallRoot
        {
            get
            {
                if (windowsSdk70InstallRoot == null)
                {
                    windowsSdk70InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.0", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk70InstallRoot;
            }
        }

        static string windowsSdk71InstallRoot = null;
        /// <summary>
        /// Location of the .NET 4.0 SDK (Windows SDK 7.1) install root.
        /// </summary>
        public static string WindowsSdk71InstallRoot
        {
            get
            {
                if (windowsSdk71InstallRoot == null)
                {
                    windowsSdk71InstallRoot = GetPathFromRegistry(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v7.1", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk71InstallRoot;
            }
        }

        static string windowsSdk80InstallRoot = null;
        /// <summary>
        /// Location of the .NET 4.5 SDK (Windows SDK 8.0) install root.
        /// </summary>
        public static string WindowsSdk80NetFxTools
        {
            get
            {
                if (windowsSdk80InstallRoot == null)
                {
                    windowsSdk80InstallRoot = GetPathFromRegistryX86(@"SOFTWARE\Microsoft\Microsoft SDKs\Windows\v8.0A\WinSDK-NetFx40Tools", "InstallationFolder") ?? string.Empty;
                }
                return windowsSdk80InstallRoot;
            }
        }
        #endregion

        /// <summary>
        /// 
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        public static bool IsUrl(string path)
        {
            if (path == null)
                throw new ArgumentNullException("path");
            return path.IndexOf("://", StringComparison.Ordinal) > 0;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="fileName1"></param>
        /// <param name="fileName2"></param>
        /// <returns></returns>
        public static bool IsEqualFileName(FileName fileName1, FileName fileName2)
        {
            return fileName1 == fileName2;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="dir1"></param>
        /// <param name="dir2"></param>
        /// <returns></returns>
        public static string GetCommonBaseDirectory(string dir1, string dir2)
        {
            if (dir1 == null || dir2 == null) return null;
            if (IsUrl(dir1) || IsUrl(dir2)) return null;

            dir1 = NormalizePath(dir1);
            dir2 = NormalizePath(dir2);

            string[] aPath = dir1.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            string[] bPath = dir2.Split(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            var result = new StringBuilder();
            int indx = 0;
            for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
            {
                if (bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
                {
                    if (result.Length > 0) result.Append(Path.DirectorySeparatorChar);
                    result.Append(aPath[indx]);
                }
                else
                {
                    break;
                }
            }
            if (indx == 0)
                return null;
            else
                return result.ToString();
        }

        /// <summary>
        /// Searches all the .net sdk bin folders and return the path of the
        /// exe from the latest sdk.
        /// </summary>
        /// <param name="exeName">The EXE to search for.</param>
        /// <returns>The path of the executable, or null if the exe is not found.</returns>
        public static string GetSdkPath(string exeName)
        {
            string execPath;
            if (!string.IsNullOrEmpty(WindowsSdk80NetFxTools))
            {
                execPath = Path.Combine(WindowsSdk80NetFxTools, exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk71InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk71InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk70InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk70InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk61InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk61InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk60aInstallRoot))
            {
                execPath = Path.Combine(WindowsSdk60aInstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(WindowsSdk60InstallRoot))
            {
                execPath = Path.Combine(WindowsSdk60InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            if (!string.IsNullOrEmpty(NetSdk20InstallRoot))
            {
                execPath = Path.Combine(NetSdk20InstallRoot, "bin\\" + exeName);
                if (File.Exists(execPath)) { return execPath; }
            }
            return null;
        }

        /// <summary>
        /// Converts a given absolute path and a given base path to a path that leads
        /// from the base path to the absoulte path. (as a relative path)
        /// </summary>
        public static string GetRelativePath(string baseDirectoryPath, string absPath)
        {
            if (string.IsNullOrEmpty(baseDirectoryPath))
            {
                return absPath;
            }
            if (IsUrl(absPath) || IsUrl(baseDirectoryPath))
            {
                return absPath;
            }

            baseDirectoryPath = NormalizePath(baseDirectoryPath);
            absPath = NormalizePath(absPath);

            string[] bPath = baseDirectoryPath.Split(separators);
            string[] aPath = absPath.Split(separators);
            int indx = 0;
            for (; indx < Math.Min(bPath.Length, aPath.Length); ++indx)
            {
                if (!bPath[indx].Equals(aPath[indx], StringComparison.OrdinalIgnoreCase))
                    break;
            }

            if (indx == 0)
            {
                return absPath;
            }

            var erg = new StringBuilder();

            if (indx == bPath.Length)
            {
                //				erg.Append('.');
                //				erg.Append(Path.DirectorySeparatorChar);
            }
            else
            {
                for (int i = indx; i < bPath.Length; ++i)
                {
                    erg.Append("..");
                    erg.Append(Path.DirectorySeparatorChar);
                }
            }
            erg.Append(String.Join(Path.DirectorySeparatorChar.ToString(), aPath, indx, aPath.Length - indx));
            return erg.ToString();
        }

        /// <summary>
        /// Combines baseDirectoryPath with relPath and normalizes the resulting path.
        /// </summary>
        public static string GetAbsolutePath(string baseDirectoryPath, string relPath)
        {
            return NormalizePath(Path.Combine(baseDirectoryPath, relPath));
        }
        /// <summary>
        /// RenameBaseDirectory
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="oldDirectory"></param>
        /// <param name="newDirectory"></param>
        /// <returns></returns>
        public static string RenameBaseDirectory(string fileName, string oldDirectory, string newDirectory)
        {
            fileName = NormalizePath(fileName);
            oldDirectory = NormalizePath(oldDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            newDirectory = NormalizePath(newDirectory.TrimEnd(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar));
            if (IsBaseDirectory(oldDirectory, fileName))
            {
                if (fileName.Length == oldDirectory.Length)
                {
                    return newDirectory;
                }
                return Path.Combine(newDirectory, fileName.Substring(oldDirectory.Length + 1));
            }
            return fileName;
        }
        /// <summary>
        /// DeepCopy
        /// </summary>
        /// <param name="sourceDirectory"></param>
        /// <param name="destinationDirectory"></param>
        /// <param name="overwrite"></param>
        public static void DeepCopy(string sourceDirectory, string destinationDirectory, bool overwrite)
        {
            if (!Directory.Exists(destinationDirectory))
            {
                Directory.CreateDirectory(destinationDirectory);
            }
            foreach (string fileName in Directory.GetFiles(sourceDirectory))
            {
                File.Copy(fileName, Path.Combine(destinationDirectory, Path.GetFileName(fileName)), overwrite);
            }
            foreach (string directoryName in Directory.GetDirectories(sourceDirectory))
            {
                DeepCopy(directoryName, Path.Combine(destinationDirectory, Path.GetFileName(directoryName)), overwrite);
            }
        }
        /// <summary>
        /// SearchDirectory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filemask"></param>
        /// <param name="searchSubdirectories"></param>
        /// <param name="ignoreHidden"></param>
        /// <returns></returns>
        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
        {
            return SearchDirectoryInternal(directory, filemask, searchSubdirectories, ignoreHidden).Select(file => file.ToString()).ToList();
        }
        /// <summary>
        /// SearchDirectory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filemask"></param>
        /// <param name="searchSubdirectories"></param>
        /// <returns></returns>
        public static List<string> SearchDirectory(string directory, string filemask, bool searchSubdirectories)
        {
            return SearchDirectory(directory, filemask, searchSubdirectories, true);
        }
        /// <summary>
        /// SearchDirectory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filemask"></param>
        /// <returns></returns>
        public static List<string> SearchDirectory(string directory, string filemask)
        {
            return SearchDirectory(directory, filemask, true, true);
        }
        /// <summary>
        /// LazySearchDirectory
        /// </summary>
        /// <param name="directory"></param>
        /// <param name="filemask"></param>
        /// <param name="searchSubdirectories"></param>
        /// <param name="ignoreHidden"></param>
        /// <returns></returns>
        public static IEnumerable<FileName> LazySearchDirectory(string directory, string filemask, bool searchSubdirectories = true, bool ignoreHidden = true)
        {
            return SearchDirectoryInternal(directory, filemask, searchSubdirectories, ignoreHidden);
        }

        /// <summary>
        /// Finds all files which are valid to the mask <paramref name="filemask"/> in the path
        /// <paramref name="directory"/> and all subdirectories
        /// (if <paramref name="searchSubdirectories"/> is true).
        /// If <paramref name="ignoreHidden"/> is true, hidden files and folders are ignored.
        /// </summary>
        static IEnumerable<FileName> SearchDirectoryInternal(string directory, string filemask, bool searchSubdirectories, bool ignoreHidden)
        {
            // If Directory.GetFiles() searches the 8.3 name as well as the full name so if the filemask is
            // "*.xpt" it will return "Template.xpt~"
            bool isExtMatch = Regex.IsMatch(filemask, @"^\*\..{3}$");
            string ext = null;
            if (isExtMatch) ext = filemask.Remove(0, 1);
            string[] empty = new string[0];
            IEnumerable<string> dir = new[] { directory };

            if (searchSubdirectories)
                dir = dir.Flatten(
                    d =>
                    {
                        try
                        {
                            if (ignoreHidden)
                                return Directory.EnumerateDirectories(d).Where(child => IsNotHidden(child));
                            else
                                return Directory.EnumerateDirectories(d);
                        }
                        catch (UnauthorizedAccessException)
                        {
                            return empty;
                        }
                    });
            foreach (string d in dir)
            {
                IEnumerable<string> files;
                try
                {
                    files = Directory.EnumerateFiles(d, filemask);
                }
                catch (UnauthorizedAccessException)
                {
                    continue;
                }
                foreach (string f in files)
                {
                    if (!ignoreHidden || IsNotHidden(f))
                        yield return new FileName(f);
                }
            }
        }

        static bool IsNotHidden(string dir)
        {
            try
            {
                return (File.GetAttributes(dir) & FileAttributes.Hidden) != FileAttributes.Hidden;
            }
            catch (UnauthorizedAccessException)
            {
                return false;
            }
        }

        
        /// <summary>
        /// This is an arbitrary limitation built into the .NET Framework.
        /// Windows supports paths up to 32k length.
        /// </summary>
        public static readonly int MaxPathLength = 260;

        /// <summary>
        /// This method checks if a path (full or relative) is valid.
        /// </summary>
        public static bool IsValidPath(string fileName)
        {
            // Fixme: 260 is the hardcoded maximal length for a path on my Windows XP system
            //        I can't find a .NET property or method for determining this variable.

            if (string.IsNullOrEmpty(fileName) || fileName.Length >= MaxPathLength)
            {
                return false;
            }

            // platform independend : check for invalid path chars

            if (fileName.IndexOfAny(Path.GetInvalidPathChars()) >= 0)
            {
                return false;
            }
            if (fileName.IndexOf('?') >= 0 || fileName.IndexOf('*') >= 0)
            {
                return false;
            }

            if (!Regex.IsMatch(fileName, fileNameRegEx))
            {
                return false;
            }

            if (fileName[fileName.Length - 1] == ' ')
            {
                return false;
            }

            if (fileName[fileName.Length - 1] == '.')
            {
                return false;
            }

            // platform dependend : Check for invalid file names (DOS)
            // this routine checks for follwing bad file names :
            // CON, PRN, AUX, NUL, COM1-9 and LPT1-9

            string nameWithoutExtension = Path.GetFileNameWithoutExtension(fileName);
            if (nameWithoutExtension != null)
            {
                nameWithoutExtension = nameWithoutExtension.ToUpperInvariant();
            }

            if (nameWithoutExtension == "CON" ||
                nameWithoutExtension == "PRN" ||
                nameWithoutExtension == "AUX" ||
                nameWithoutExtension == "NUL")
            {
                return false;
            }

            char ch = nameWithoutExtension.Length == 4 ? nameWithoutExtension[3] : '\0';

            return !((nameWithoutExtension.StartsWith("COM") ||
                      nameWithoutExtension.StartsWith("LPT")) &&
                     Char.IsDigit(ch));
        }

        /// <summary>
        /// Checks that a single directory name (not the full path) is valid.
        /// </summary>
        [ObsoleteAttribute("Use IsValidDirectoryEntryName instead")]
        public static bool IsValidDirectoryName(string name)
        {
            return IsValidDirectoryEntryName(name);
        }

        /// <summary>
        /// Checks that a single directory name (not the full path) is valid.
        /// </summary>
        public static bool IsValidDirectoryEntryName(string name)
        {
            if (!IsValidPath(name))
            {
                return false;
            }
            if (name.IndexOfAny(new char[] { Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar, Path.VolumeSeparatorChar }) >= 0)
            {
                return false;
            }
            if (name.Trim(' ').Length == 0)
            {
                return false;
            }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool TestFileExists(string filename)
        {
            if (!File.Exists(filename))
            {
                MessageService.ShowWarning(String.Format("Cannot file file {0}",filename));
                return false;
           }
            return true;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <returns></returns>
        public static bool IsDirectory(string filename)
        {
            if (!Directory.Exists(filename))
            {
                return false;
            }
            FileAttributes attr = File.GetAttributes(filename);
            return (attr & FileAttributes.Directory) != 0;
        }

        //TODO This code is Windows specific
        static bool MatchN(string src, int srcidx, string pattern, int patidx)
        {
            int patlen = pattern.Length;
            int srclen = src.Length;
            char next_char;

            for (; ; )
            {
                if (patidx == patlen)
                    return (srcidx == srclen);
                next_char = pattern[patidx++];
                if (next_char == '?')
                {
                    if (srcidx == src.Length)
                        return false;
                    srcidx++;
                }
                else if (next_char != '*')
                {
                    if ((srcidx == src.Length) || (src[srcidx] != next_char))
                        return false;
                    srcidx++;
                }
                else
                {
                    if (patidx == pattern.Length)
                        return true;
                    while (srcidx < srclen)
                    {
                        if (MatchN(src, srcidx, pattern, patidx))
                            return true;
                        srcidx++;
                    }
                    return false;
                }
            }
        }

        static bool Match(string src, string pattern)
        {
            if (pattern[0] == '*')
            {
                // common case optimization
                int i = pattern.Length;
                int j = src.Length;
                while (--i > 0)
                {
                    if (pattern[i] == '*')
                        return MatchN(src, 0, pattern, 0);
                    if (j-- == 0)
                        return false;
                    if ((pattern[i] != src[j]) && (pattern[i] != '?'))
                        return false;
                }
                return true;
            }
            return MatchN(src, 0, pattern, 0);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="filename"></param>
        /// <param name="pattern"></param>
        /// <returns></returns>
        public static bool MatchesPattern(string filename, string pattern)
        {
            filename = filename.ToUpper();
            pattern = pattern.ToUpper();
            string[] patterns = pattern.Split(';');
            return patterns.Any(p => Match(filename, p));
        }

        /// <summary>
        /// Observe SAVE functions
        /// </summary>
        /// <param name="saveFile"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            System.Diagnostics.Debug.Assert(IsValidPath(fileName));
            try
            {
                saveFile();
                RaiseFileSaved(new FileNameEventArgs(fileName));
                return FileOperationResult.OK;
            }
            catch (IOException e)
            {
                return ObservedSaveHandleException(e, saveFile, fileName, message, policy);
            }
            catch (UnauthorizedAccessException e)
            {
                return ObservedSaveHandleException(e, saveFile, fileName, message, policy);
            }
        }

        static FileOperationResult ObservedSaveHandleException(Exception e, FileOperationDelegate saveFile, string fileName, string message, FileErrorPolicy policy)
        {
            switch (policy)
            {
                case FileErrorPolicy.Inform:
                    ServiceManager.Instance.MessageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e);
                    break;
                case FileErrorPolicy.ProvideAlternative:
                    ChooseSaveErrorResult r = ServiceManager.Instance.MessageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, false);
                    if (r.IsRetry)
                    {
                        return ObservedSave(saveFile, fileName, message, policy);
                    }
                    if (r.IsIgnore)
                    {
                        return FileOperationResult.Failed;
                    }
                    break;
            }
            return FileOperationResult.Failed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFile"></param>
        /// <param name="fileName"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedSave(FileOperationDelegate saveFile, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedSave(saveFile,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
                                policy);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFileAs"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            System.Diagnostics.Debug.Assert(IsValidPath(fileName));
            try
            {
                string directory = Path.GetDirectoryName(fileName);
                if (!Directory.Exists(directory))
                {
                    Directory.CreateDirectory(directory);
                }
                saveFileAs(fileName);
                RaiseFileSaved(new FileNameEventArgs(fileName));
                return FileOperationResult.OK;
            }
            catch (IOException e)
            {
                return ObservedSaveHandleError(e, saveFileAs, fileName, message, policy);
            }
            catch (UnauthorizedAccessException e)
            {
                return ObservedSaveHandleError(e, saveFileAs, fileName, message, policy);
            }
        }

        static FileOperationResult ObservedSaveHandleError(Exception e, NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy)
        {
            switch (policy)
            {
                case FileErrorPolicy.Inform:
                    ServiceManager.Instance.MessageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e);
                    break;
                case FileErrorPolicy.ProvideAlternative:
                    ChooseSaveErrorResult r = ServiceManager.Instance.MessageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileSaving}", e, true);
                    if (r.IsRetry)
                    {
                        return ObservedSave(saveFileAs, fileName, message, policy);
                    }
                    else if (r.IsIgnore)
                    {
                        return FileOperationResult.Failed;
                    }
                    else if (r.IsSaveAlternative)
                    {
                        return ObservedSave(saveFileAs, r.AlternativeFileName, message, policy);
                    }
                    break;
            }
            return FileOperationResult.Failed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFileAs"></param>
        /// <param name="fileName"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedSave(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedSave(saveFileAs,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantSaveFileStandardText"),
                                policy);
        }

        // Observe LOAD functions
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadFile"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, string message, FileErrorPolicy policy)
        {
            try
            {
                loadFile();
                OnFileLoaded(new FileNameEventArgs(fileName));
                return FileOperationResult.OK;
            }
            catch (IOException e)
            {
                return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
            }
            catch (UnauthorizedAccessException e)
            {
                return ObservedLoadHandleException(e, loadFile, fileName, message, policy);
            }
        }

        static FileOperationResult ObservedLoadHandleException(Exception e, FileOperationDelegate loadFile, string fileName, string message, FileErrorPolicy policy)
        {
            switch (policy)
            {
                case FileErrorPolicy.Inform:
                    ServiceManager.Instance.MessageService.InformSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e);
                    break;
                case FileErrorPolicy.ProvideAlternative:
                    ChooseSaveErrorResult r = ServiceManager.Instance.MessageService.ChooseSaveError(fileName, message, "${res:FileUtilityService.ErrorWhileLoading}", e, false);
                    if (r.IsRetry)
                        return ObservedLoad(loadFile, fileName, message, policy);
                    else if (r.IsIgnore)
                        return FileOperationResult.Failed;
                    break;
            }
            return FileOperationResult.Failed;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="loadFile"></param>
        /// <param name="fileName"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedLoad(FileOperationDelegate loadFile, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedLoad(loadFile,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
                                policy);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFileAs"></param>
        /// <param name="fileName"></param>
        /// <param name="message"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, string message, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedLoad(new FileOperationDelegate(delegate { saveFileAs(fileName); }), fileName, message, policy);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="saveFileAs"></param>
        /// <param name="fileName"></param>
        /// <param name="policy"></param>
        /// <returns></returns>
        public static FileOperationResult ObservedLoad(NamedFileOperationDelegate saveFileAs, string fileName, FileErrorPolicy policy = FileErrorPolicy.Inform)
        {
            return ObservedLoad(saveFileAs,
                                fileName,
                                ResourceService.GetString("ICSharpCode.Services.FileUtilityService.CantLoadFileStandardText"),
                                policy);
        }

        static void OnFileLoaded(FileNameEventArgs e)
        {
            if (FileLoaded != null)
            {
                FileLoaded(null, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="e"></param>
        public static void RaiseFileSaved(FileNameEventArgs e)
        {
            if (FileSaved != null)
            {
                FileSaved(null, e);
            }
        }
        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler<FileNameEventArgs> FileLoaded;
        /// <summary>
        /// 
        /// </summary>
        public static event EventHandler<FileNameEventArgs> FileSaved;
    }
}