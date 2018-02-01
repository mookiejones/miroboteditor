#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:42 AM
// Modified:2018:02:01:9:48 AM:

#endregion

#region Usings

using System;
using System.CodeDom.Compiler;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using Microsoft.CSharp;

#endregion

namespace InlineFormParser.Zip
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00005")]
	[ComVisible(true)]
	public class ZipFile : IEnumerable<ZipEntry>, IEnumerable, IDisposable
	{
		private static readonly ExtractorSettings[] SettingsList = new ExtractorSettings[2]
		{
			new ExtractorSettings
			{
				Flavor = SelfExtractorFlavor.WinFormsApplication,
				ReferencedAssemblies = new List<string>
				{
					"System.dll",
					"System.Windows.Forms.dll",
					"System.Drawing.dll"
				},
				CopyThroughResources = new List<string>
				{
					"Ionic.Zip.WinFormsSelfExtractorStub.resources",
					"Ionic.Zip.Forms.PasswordDialog.resources",
					"Ionic.Zip.Forms.ZipContentsDialog.resources"
				},
				ResourcesToCompile = new List<string>
				{
					"WinFormsSelfExtractorStub.cs",
					"WinFormsSelfExtractorStub.Designer.cs",
					"PasswordDialog.cs",
					"PasswordDialog.Designer.cs",
					"ZipContentsDialog.cs",
					"ZipContentsDialog.Designer.cs",
					"FolderBrowserDialogEx.cs"
				}
			},
			new ExtractorSettings
			{
				Flavor = SelfExtractorFlavor.ConsoleApplication,
				ReferencedAssemblies = new List<string>
				{
					"System.dll"
				},
				CopyThroughResources = null,
				ResourcesToCompile = new List<string>
				{
					"CommandLineSelfExtractorStub.cs"
				}
			}
		};

		public static readonly int BufferSizeDefault = 32768;

		private bool _addOperationCanceled;

		private bool _CaseSensitiveRetrieval;

		private string _Comment;

		private bool _contentsChanged;

		private uint _diskNumberWithCd;

		private bool _disposed;

		private EncryptionAlgorithm _Encryption;

		private Dictionary<string, ZipEntry> _entries;

		private bool _extractOperationCanceled;

		private bool _fileAlreadyExists;

		private bool _hasBeenSaved;

		internal bool _inExtractAll;

		private bool _JustSaved;

		private long _lengthOfReadStream = -99L;

		private long _locEndOfCDS = -1L;

		private int _maxBufferPairs = 16;

		private int _maxOutputSegmentSize;

		private uint _numberOfSegmentsForMostRecentSave;

		private uint _OffsetOfCentralDirectory;

		private long _OffsetOfCentralDirectory64;

		private long _ParallelDeflateThreshold;

		internal string _Password;

		private string _readName;

		private Stream _readstream;

		private bool _ReadStreamIsOurs = true;

		private bool _saveOperationCanceled;

		private bool _SavingSfx;

		private string _TempFileFolder;

		private string _temporaryFileName;

		private ushort _versionMadeBy;

		private ushort _versionNeededToExtract;

		private Stream _writestream;

		internal Zip64Option _zip64;

		private List<ZipEntry> _zipEntriesAsList;

		private ZipErrorAction _zipErrorAction;

		private readonly object LOCK = new object();

		internal ParallelDeflateOutputStream ParallelDeflater;

		public ZipFile(string fileName)
		{
			try
			{
				_InitInstance(fileName, null);
			}
			catch (Exception innerException)
			{
				throw new ZipException($"Could not read {fileName} as a zip file", innerException);
			}
		}

		public ZipFile(string fileName, Encoding encoding)
		{
			try
			{
				AlternateEncoding = encoding;
				AlternateEncodingUsage = ZipOption.Always;
				_InitInstance(fileName, null);
			}
			catch (Exception innerException)
			{
				throw new ZipException($"{fileName} is not a valid zip file", innerException);
			}
		}

		public ZipFile()
		{
			_InitInstance(null, null);
		}

		public ZipFile(Encoding encoding)
		{
			AlternateEncoding = encoding;
			AlternateEncodingUsage = ZipOption.Always;
			_InitInstance(null, null);
		}

		public ZipFile(string fileName, TextWriter statusMessageWriter)
		{
			try
			{
				_InitInstance(fileName, statusMessageWriter);
			}
			catch (Exception innerException)
			{
				throw new ZipException($"{fileName} is not a valid zip file", innerException);
			}
		}

		public ZipFile(string fileName, TextWriter statusMessageWriter, Encoding encoding)
		{
			try
			{
				AlternateEncoding = encoding;
				AlternateEncodingUsage = ZipOption.Always;
				_InitInstance(fileName, statusMessageWriter);
			}
			catch (Exception innerException)
			{
				throw new ZipException($"{fileName} is not a valid zip file", innerException);
			}
		}

		public string Info
		{
			get
			{
				var stringBuilder = new StringBuilder();
				stringBuilder.Append($"          ZipFile: {Name}\n");
				if (!string.IsNullOrEmpty(_Comment)) stringBuilder.Append($"          Comment: {_Comment}\n");
				if (_versionMadeBy != 0) stringBuilder.Append($"  version made by: 0x{_versionMadeBy:X4}\n");
				if (_versionNeededToExtract != 0)
					stringBuilder.Append($"needed to extract: 0x{_versionNeededToExtract:X4}\n");
				stringBuilder.Append($"       uses ZIP64: {InputUsesZip64}\n");
				stringBuilder.Append($"     disk with CD: {_diskNumberWithCd}\n");
				if (_OffsetOfCentralDirectory == 4294967295u)
					stringBuilder.Append($"      CD64 offset: 0x{_OffsetOfCentralDirectory64:X16}\n");
				else
					stringBuilder.Append($"        CD offset: 0x{_OffsetOfCentralDirectory:X8}\n");
				stringBuilder.Append("\n");
				foreach (var value in _entries.Values) stringBuilder.Append(value.Info);
				return stringBuilder.ToString();
			}
		}

		public bool FullScan { get; set; }

		public bool SortEntriesBeforeSaving { get; set; }

		public bool AddDirectoryWillTraverseReparsePoints { get; set; }

		public int BufferSize { get; set; } = BufferSizeDefault;

		public int CodecBufferSize { get; set; }

		public bool FlattenFoldersOnExtract { get; set; }

		public CompressionStrategy Strategy { get; set; }

		public string Name { get; set; }

		public CompressionLevel CompressionLevel { get; set; }

		public CompressionMethod CompressionMethod { get; set; } = CompressionMethod.Deflate;

		public string Comment
		{
			get => _Comment;
			set
			{
				_Comment = value;
				_contentsChanged = true;
			}
		}

		public bool EmitTimesInWindowsFormatWhenSaving { get; set; } = true;

		public bool EmitTimesInUnixFormatWhenSaving { get; set; }

		internal bool Verbose => StatusMessageTextWriter != null;

		public bool CaseSensitiveRetrieval
		{
			get => _CaseSensitiveRetrieval;
			set
			{
				if (value != _CaseSensitiveRetrieval)
				{
					_CaseSensitiveRetrieval = value;
					_initEntriesDictionary();
				}
			}
		}

		[Obsolete(
			"Beginning with v1.9.1.6 of DotNetZip, this property is obsolete.  It will be removed in a future version of the library. Your applications should  use AlternateEncoding and AlternateEncodingUsage instead.")]
		public bool UseUnicodeAsNecessary
		{
			get
			{
				if (AlternateEncoding == Encoding.GetEncoding("UTF-8")) return AlternateEncodingUsage == ZipOption.AsNecessary;
				return false;
			}
			set
			{
				if (value)
				{
					AlternateEncoding = Encoding.GetEncoding("UTF-8");
					AlternateEncodingUsage = ZipOption.AsNecessary;
				}
				else
				{
					AlternateEncoding = DefaultEncoding;
					AlternateEncodingUsage = ZipOption.Default;
				}
			}
		}

		public Zip64Option UseZip64WhenSaving
		{
			get => _zip64;
			set => _zip64 = value;
		}

		public bool? RequiresZip64
		{
			get
			{
				if (_entries.Count > 65534) return true;
				if (_hasBeenSaved && !_contentsChanged)
				{
					foreach (var value in _entries.Values)
						if (value.RequiresZip64.Value)
							return true;
					return false;
				}

				return null;
			}
		}

		public bool? OutputUsedZip64 { get; private set; }

		public bool? InputUsesZip64
		{
			get
			{
				if (_entries.Count > 65534) return true;
				foreach (var item in this)
				{
					if (item.Source != ZipEntrySource.ZipFile) return null;
					if (item._InputUsesZip64) return true;
				}

				return false;
			}
		}

		[Obsolete("use AlternateEncoding instead.")]
		public Encoding ProvisionalAlternateEncoding
		{
			get
			{
				if (AlternateEncodingUsage == ZipOption.AsNecessary) return AlternateEncoding;
				return null;
			}
			set
			{
				AlternateEncoding = value;
				AlternateEncodingUsage = ZipOption.AsNecessary;
			}
		}

		public Encoding AlternateEncoding { get; set; } = Encoding.GetEncoding("IBM437");

		public ZipOption AlternateEncodingUsage { get; set; }

		public static Encoding DefaultEncoding { get; } = Encoding.GetEncoding("IBM437");

		public TextWriter StatusMessageTextWriter { get; set; }

		public string TempFileFolder
		{
			get => _TempFileFolder;
			set
			{
				_TempFileFolder = value;
				if (value != null && !Directory.Exists(value))
					throw new FileNotFoundException($"That directory ({value}) does not exist.");
			}
		}

		public string Password
		{
			private get { return _Password; }
			set
			{
				_Password = value;
				if (_Password == null)
					Encryption = EncryptionAlgorithm.None;
				else if (Encryption == EncryptionAlgorithm.None)
					Encryption = EncryptionAlgorithm.PkzipWeak;
			}
		}

		public ExtractExistingFileAction ExtractExistingFile { get; set; }

		public ZipErrorAction ZipErrorAction
		{
			get
			{
				if (ZipError != null) _zipErrorAction = ZipErrorAction.InvokeErrorEvent;
				return _zipErrorAction;
			}
			set
			{
				_zipErrorAction = value;
				if (_zipErrorAction != ZipErrorAction.InvokeErrorEvent && ZipError != null) ZipError = null;
			}
		}

		public EncryptionAlgorithm Encryption
		{
			get => _Encryption;
			set
			{
				if (value == EncryptionAlgorithm.Unsupported)
					throw new InvalidOperationException("You may not set Encryption to that value.");
				_Encryption = value;
			}
		}

		public SetCompressionCallback SetCompression { get; set; }

		public int MaxOutputSegmentSize
		{
			get => _maxOutputSegmentSize;
			set
			{
				if (value < 65536 && value != 0) throw new ZipException("The minimum acceptable segment size is 65536.");
				_maxOutputSegmentSize = value;
			}
		}

		public int NumberOfSegmentsForMostRecentSave => (int) (_numberOfSegmentsForMostRecentSave + 1);

		public long ParallelDeflateThreshold
		{
			get => _ParallelDeflateThreshold;
			set
			{
				if (value != 0 && value != -1 && value < 65536)
					throw new ArgumentOutOfRangeException("ParallelDeflateThreshold should be -1, 0, or > 65536");
				_ParallelDeflateThreshold = value;
			}
		}

		public int ParallelDeflateMaxBufferPairs
		{
			get => _maxBufferPairs;
			set
			{
				if (value < 4)
					throw new ArgumentOutOfRangeException("ParallelDeflateMaxBufferPairs", "Value must be 4 or greater.");
				_maxBufferPairs = value;
			}
		}

		public static Version LibraryVersion => Assembly.GetExecutingAssembly().GetName().Version;

		private List<ZipEntry> ZipEntriesAsList
		{
			get
			{
				if (_zipEntriesAsList == null) _zipEntriesAsList = new List<ZipEntry>(_entries.Values);
				return _zipEntriesAsList;
			}
		}

		public ZipEntry this[int ix] => ZipEntriesAsList[ix];

		public ZipEntry this[string fileName]
		{
			get
			{
				var text = SharedUtilities.NormalizePathForUseInZipFile(fileName);
				if (_entries.ContainsKey(text)) return _entries[text];
				text = text.Replace("/", "\\");
				if (_entries.ContainsKey(text)) return _entries[text];
				return null;
			}
		}

		public ICollection<string> EntryFileNames => _entries.Keys;

		public ICollection<ZipEntry> Entries => _entries.Values;

		public ICollection<ZipEntry> EntriesSorted
		{
			get
			{
				var list = new List<ZipEntry>();
				foreach (var entry in Entries) list.Add(entry);
				var sc = (StringComparison) (CaseSensitiveRetrieval ? 4 : 5);
				list.Sort((x, y) => string.Compare(x.FileName, y.FileName, sc));
				return list.AsReadOnly();
			}
		}

		public int Count => _entries.Count;

		internal Stream ReadStream
		{
			get
			{
				if (_readstream == null && (_readName != null || Name != null))
				{
					_readstream = File.Open(_readName ?? Name, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					_ReadStreamIsOurs = true;
				}

				return _readstream;
			}
		}

		private Stream WriteStream
		{
			get
			{
				if (_writestream != null) return _writestream;
				if (Name == null) return _writestream;
				if (_maxOutputSegmentSize != 0)
				{
					_writestream = ZipSegmentedStream.ForWriting(Name, _maxOutputSegmentSize);
					return _writestream;
				}

				SharedUtilities.CreateAndOpenUniqueTempFile(TempFileFolder ?? Path.GetDirectoryName(Name), out _writestream,
					out _temporaryFileName);
				return _writestream;
			}
			set
			{
				if (value != null) throw new ZipException("Cannot set the stream to a non-null value.");
				_writestream = null;
			}
		}

		private string ArchiveNameForEvent
		{
			get
			{
				if (Name == null) return "(stream)";
				return Name;
			}
		}

		private long LengthOfReadStream
		{
			get
			{
				if (_lengthOfReadStream == -99) _lengthOfReadStream = _ReadStreamIsOurs ? SharedUtilities.GetFileLength(Name) : -1;
				return _lengthOfReadStream;
			}
		}

		public void Dispose()
		{
			Dispose(true);
			GC.SuppressFinalize(this);
		}

		public IEnumerator<ZipEntry> GetEnumerator()
		{
			foreach (var value in _entries.Values) yield return value;
		}

		IEnumerator IEnumerable.GetEnumerator()
		{
			return GetEnumerator();
		}

		public event EventHandler<SaveProgressEventArgs> SaveProgress;

		public event EventHandler<ReadProgressEventArgs> ReadProgress;

		public event EventHandler<ExtractProgressEventArgs> ExtractProgress;

		public event EventHandler<AddProgressEventArgs> AddProgress;

		public event EventHandler<ZipErrorEventArgs> ZipError;

		public static bool CheckZip(string zipFileName)
		{
			return CheckZip(zipFileName, false, null);
		}

		public static bool CheckZip(string zipFileName, bool fixIfNecessary, TextWriter writer)
		{
			ZipFile zipFile = null;
			ZipFile zipFile2 = null;
			var flag = true;
			try
			{
				zipFile = new ZipFile {FullScan = true};
				zipFile.Initialize(zipFileName);
				zipFile2 = Read(zipFileName);
				foreach (var item in zipFile)
				foreach (var item2 in zipFile2)
					if (item.FileName == item2.FileName)
					{
						if (item._RelativeOffsetOfLocalHeader != item2._RelativeOffsetOfLocalHeader)
						{
							flag = false;
							writer?.WriteLine("{0}: mismatch in RelativeOffsetOfLocalHeader  (0x{1:X16} != 0x{2:X16})", item.FileName,
								item._RelativeOffsetOfLocalHeader, item2._RelativeOffsetOfLocalHeader);
						}

						if (item._CompressedSize != item2._CompressedSize)
						{
							flag = false;
							writer?.WriteLine("{0}: mismatch in CompressedSize  (0x{1:X16} != 0x{2:X16})", item.FileName,
								item._CompressedSize, item2._CompressedSize);
						}

						if (item._UncompressedSize != item2._UncompressedSize)
						{
							flag = false;
							writer?.WriteLine("{0}: mismatch in UncompressedSize  (0x{1:X16} != 0x{2:X16})", item.FileName,
								item._UncompressedSize, item2._UncompressedSize);
						}

						if (item.CompressionMethod != item2.CompressionMethod)
						{
							flag = false;
							writer?.WriteLine("{0}: mismatch in CompressionMethod  (0x{1:X4} != 0x{2:X4})", item.FileName,
								item.CompressionMethod, item2.CompressionMethod);
						}

						if (item.Crc != item2.Crc)
						{
							flag = false;
							writer?.WriteLine("{0}: mismatch in Crc32  (0x{1:X4} != 0x{2:X4})", item.FileName, item.Crc, item2.Crc);
						}

						break;
					}

				zipFile2.Dispose();
				zipFile2 = null;
				if (!flag)
				{
					if (fixIfNecessary)
					{
						var fileNameWithoutExtension = Path.GetFileNameWithoutExtension(zipFileName);
						fileNameWithoutExtension = $"{fileNameWithoutExtension}_fixed.zip";
						zipFile.Save(fileNameWithoutExtension);
						return flag;
					}

					return flag;
				}

				return flag;
			}
			finally
			{
				if (zipFile != null) zipFile.Dispose();
				if (zipFile2 != null) zipFile2.Dispose();
			}
		}

		public static void FixZipDirectory(string zipFileName)
		{
			using (var zipFile = new ZipFile())
			{
				zipFile.FullScan = true;
				zipFile.Initialize(zipFileName);
				zipFile.Save(zipFileName);
			}
		}

		public static bool CheckZipPassword(string zipFileName, string password)
		{
			var result = false;
			try
			{
				using (var zipFile = Read(zipFileName))
				{
					foreach (var item in zipFile)
						if (!item.IsDirectory && item.UsesEncryption)
							item.ExtractWithPassword(Stream.Null, password);
				}

				result = true;
				return result;
			}
			catch (BadPasswordException)
			{
				return result;
			}
		}

		public void SaveSelfExtractor(string exeToGenerate, SelfExtractorFlavor flavor)
		{
			var selfExtractorSaveOptions = new SelfExtractorSaveOptions();
			selfExtractorSaveOptions.Flavor = flavor;
			SaveSelfExtractor(exeToGenerate, selfExtractorSaveOptions);
		}

		public void SaveSelfExtractor(string exeToGenerate, SelfExtractorSaveOptions options)
		{
			if (Name == null) _writestream = null;
			_SavingSfx = true;
			Name = exeToGenerate;
			if (Directory.Exists(Name))
				throw new ZipException("Bad Directory",
					new ArgumentException("That name specifies an existing directory. Please specify a filename.",
						nameof(exeToGenerate)));
			_contentsChanged = true;
			_fileAlreadyExists = File.Exists(Name);
			_SaveSfxStub(exeToGenerate, options);
			Save();
			_SavingSfx = false;
		}

		private static void ExtractResourceToFile(Assembly a, string resourceName, string filename)
		{
			var num = 0;
			var array = new byte[1024];
			using (var stream = a.GetManifestResourceStream(resourceName))
			{
				if (stream == null) throw new ZipException($"missing resource '{resourceName}'");
				using (var fileStream = File.OpenWrite(filename))
				{
					do
					{
						num = stream.Read(array, 0, array.Length);
						fileStream.Write(array, 0, num);
					} while (num > 0);
				}
			}
		}

		private void _SaveSfxStub(string exeToGenerate, SelfExtractorSaveOptions options)
		{
			string text = null;
			string text2 = null;
			string text3 = null;
			string text4 = null;
			try
			{
				if (File.Exists(exeToGenerate) && Verbose)
					StatusMessageTextWriter.WriteLine("The existing file ({0}) will be overwritten.", exeToGenerate);
				if (!exeToGenerate.EndsWith(".exe") && Verbose)
					StatusMessageTextWriter.WriteLine("Warning: The generated self-extracting file will not have an .exe extension.");
				text4 = TempFileFolder ?? Path.GetDirectoryName(exeToGenerate);
				text2 = GenerateTempPathname(text4, "exe");
				var assembly = typeof(ZipFile).Assembly;
				using (var cSharpCodeProvider = new CSharpCodeProvider())
				{
					ExtractorSettings extractorSettings = null;
					var settingsList = SettingsList;
					foreach (var extractorSettings2 in settingsList)
						if (extractorSettings2.Flavor == options.Flavor)
						{
							extractorSettings = extractorSettings2;
							break;
						}

					if (extractorSettings == null)
						throw new BadStateException($"While saving a Self-Extracting Zip, Cannot find that flavor ({options.Flavor})?");
					var compilerParameters = new CompilerParameters();
					compilerParameters.ReferencedAssemblies.Add(assembly.Location);
					if (extractorSettings.ReferencedAssemblies != null)
						foreach (var referencedAssembly in extractorSettings.ReferencedAssemblies)
							compilerParameters.ReferencedAssemblies.Add(referencedAssembly);
					compilerParameters.GenerateInMemory = false;
					compilerParameters.GenerateExecutable = true;
					compilerParameters.IncludeDebugInformation = false;
					compilerParameters.CompilerOptions = "";
					var executingAssembly = Assembly.GetExecutingAssembly();
					var stringBuilder = new StringBuilder();
					var text5 = GenerateTempPathname(text4, "cs");
					using (var zipFile = Read(executingAssembly.GetManifestResourceStream("Ionic.Zip.Resources.ZippedResources.zip")))
					{
						text3 = GenerateTempPathname(text4, "tmp");
						if (string.IsNullOrEmpty(options.IconFile))
						{
							Directory.CreateDirectory(text3);
							var zipEntry = zipFile["zippedFile.ico"];
							if ((zipEntry.Attributes & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
								zipEntry.Attributes ^= FileAttributes.ReadOnly;
							zipEntry.Extract(text3);
							text = Path.Combine(text3, "zippedFile.ico");
							var compilerParameters2 = compilerParameters;
							compilerParameters2.CompilerOptions += $"/win32icon:\"{text}\"";
						}
						else
						{
							var compilerParameters3 = compilerParameters;
							compilerParameters3.CompilerOptions += $"/win32icon:\"{options.IconFile}\"";
						}

						compilerParameters.OutputAssembly = text2;
						if (options.Flavor == SelfExtractorFlavor.WinFormsApplication)
						{
							var compilerParameters4 = compilerParameters;
							compilerParameters4.CompilerOptions += " /target:winexe";
						}

						if (!string.IsNullOrEmpty(options.AdditionalCompilerSwitches))
						{
							var compilerParameters5 = compilerParameters;
							compilerParameters5.CompilerOptions =
								compilerParameters5.CompilerOptions + " " + options.AdditionalCompilerSwitches;
						}

						if (string.IsNullOrEmpty(compilerParameters.CompilerOptions)) compilerParameters.CompilerOptions = null;
						if (extractorSettings.CopyThroughResources != null && extractorSettings.CopyThroughResources.Count != 0)
						{
							if (!Directory.Exists(text3)) Directory.CreateDirectory(text3);
							foreach (var copyThroughResource in extractorSettings.CopyThroughResources)
							{
								var text6 = Path.Combine(text3, copyThroughResource);
								ExtractResourceToFile(executingAssembly, copyThroughResource, text6);
								compilerParameters.EmbeddedResources.Add(text6);
							}
						}

						compilerParameters.EmbeddedResources.Add(assembly.Location);
						stringBuilder.Append("// " + Path.GetFileName(text5) + "\n")
							.Append("// --------------------------------------------\n//\n")
							.Append("// This SFX source file was generated by DotNetZip ")
							.Append(LibraryVersion)
							.Append("\n//         at ")
							.Append(DateTime.Now.ToString("yyyy MMMM dd  HH:mm:ss"))
							.Append("\n//\n// --------------------------------------------\n\n\n");
						if (!string.IsNullOrEmpty(options.Description))
							stringBuilder.Append("[assembly: System.Reflection.AssemblyTitle(\"" + options.Description.Replace("\"", "") +
							                     "\")]\n");
						else
							stringBuilder.Append("[assembly: System.Reflection.AssemblyTitle(\"DotNetZip SFX Archive\")]\n");
						if (!string.IsNullOrEmpty(options.ProductVersion))
							stringBuilder.Append("[assembly: System.Reflection.AssemblyInformationalVersion(\"" +
							                     options.ProductVersion.Replace("\"", "") + "\")]\n");
						var str = string.IsNullOrEmpty(options.Copyright)
							? "Extractor: Copyright © Dino Chiesa 2008-2011"
							: options.Copyright.Replace("\"", "");
						if (!string.IsNullOrEmpty(options.ProductName))
							stringBuilder.Append("[assembly: System.Reflection.AssemblyProduct(\"")
								.Append(options.ProductName.Replace("\"", "")).Append("\")]\n");
						else
							stringBuilder.Append("[assembly: System.Reflection.AssemblyProduct(\"DotNetZip\")]\n");
						stringBuilder.Append("[assembly: System.Reflection.AssemblyCopyright(\"" + str + "\")]\n")
							.Append($"[assembly: System.Reflection.AssemblyVersion(\"{LibraryVersion}\")]\n");
						if (options.FileVersion != null)
							stringBuilder.Append($"[assembly: System.Reflection.AssemblyFileVersion(\"{options.FileVersion}\")]\n");
						stringBuilder.Append("\n\n\n");
						var text7 = options.DefaultExtractDirectory;
						if (text7 != null) text7 = text7.Replace("\"", "").Replace("\\", "\\\\");
						var text8 = options.PostExtractCommandLine;
						if (text8 != null)
						{
							text8 = text8.Replace("\\", "\\\\");
							text8 = text8.Replace("\"", "\\\"");
						}

						foreach (var item in extractorSettings.ResourcesToCompile)
							using (Stream stream = zipFile[item].OpenReader())
							{
								if (stream == null) throw new ZipException($"missing resource '{item}'");
								using (var streamReader = new StreamReader(stream))
								{
									while (streamReader.Peek() >= 0)
									{
										var text9 = streamReader.ReadLine();
										if (text7 != null) text9 = text9.Replace("@@EXTRACTLOCATION", text7);
										text9 = text9.Replace("@@REMOVE_AFTER_EXECUTE", options.RemoveUnpackedFilesAfterExecute.ToString());
										text9 = text9.Replace("@@QUIET", options.Quiet.ToString());
										if (!string.IsNullOrEmpty(options.SfxExeWindowTitle))
											text9 = text9.Replace("@@SFX_EXE_WINDOW_TITLE", options.SfxExeWindowTitle);
										text9 = text9.Replace("@@EXTRACT_EXISTING_FILE", ((int) options.ExtractExistingFile).ToString());
										if (text8 != null) text9 = text9.Replace("@@POST_UNPACK_CMD_LINE", text8);
										stringBuilder.Append(text9).Append("\n");
									}
								}

								stringBuilder.Append("\n\n");
							}
					}

					var text10 = stringBuilder.ToString();
					var compilerResults = cSharpCodeProvider.CompileAssemblyFromSource(compilerParameters, text10);
					if (compilerResults == null) throw new SfxGenerationException("Cannot compile the extraction logic!");
					if (Verbose)
					{
						var enumerator4 = compilerResults.Output.GetEnumerator();
						try
						{
							while (enumerator4.MoveNext())
							{
								var current4 = enumerator4.Current;
								StatusMessageTextWriter.WriteLine(current4);
							}
						}
						finally
						{
							var disposable = enumerator4 as IDisposable;
							if (disposable != null) disposable.Dispose();
						}
					}

					if (compilerResults.Errors.Count != 0)
					{
						using (TextWriter textWriter = new StreamWriter(text5))
						{
							textWriter.Write(text10);
							textWriter.Write("\n\n\n// ------------------------------------------------------------------\n");
							textWriter.Write("// Errors during compilation: \n//\n");
							var fileName = Path.GetFileName(text5);
							foreach (CompilerError error in compilerResults.Errors)
								textWriter.Write("//   {0}({1},{2}): {3} {4}: {5}\n//\n", fileName, error.Line, error.Column,
									error.IsWarning ? "Warning" : "error", error.ErrorNumber, error.ErrorText);
						}

						throw new SfxGenerationException($"Errors compiling the extraction logic!  {text5}");
					}

					OnSaveEvent(ZipProgressEventType.Saving_AfterCompileSelfExtractor);
					using (Stream stream2 = File.OpenRead(text2))
					{
						var array = new byte[4000];
						var num = 1;
						while (num != 0)
						{
							num = stream2.Read(array, 0, array.Length);
							if (num != 0) WriteStream.Write(array, 0, num);
						}
					}
				}

				OnSaveEvent(ZipProgressEventType.Saving_AfterSaveTempArchive);
			}
			finally
			{
				try
				{
					if (Directory.Exists(text3))
						try
						{
							Directory.Delete(text3, true);
						}
						catch (IOException arg)
						{
							StatusMessageTextWriter.WriteLine("Warning: Exception: {0}", arg);
						}

					if (File.Exists(text2))
						try
						{
							File.Delete(text2);
						}
						catch (IOException arg2)
						{
							StatusMessageTextWriter.WriteLine("Warning: Exception: {0}", arg2);
						}
				}
				catch (IOException)
				{
				}
			}
		}

		internal static string GenerateTempPathname(string dir, string extension)
		{
			string text = null;
			var name = Assembly.GetExecutingAssembly().GetName().Name;
			do
			{
				var text2 = Guid.NewGuid().ToString();
				var path = $"{name}-{DateTime.Now.ToString("yyyyMMMdd-HHmmss")}-{text2}.{extension}";
				text = Path.Combine(dir, path);
			} while (File.Exists(text) || Directory.Exists(text));

			return text;
		}

		public ZipEntry AddItem(string fileOrDirectoryName)
		{
			return AddItem(fileOrDirectoryName, null);
		}

		public ZipEntry AddItem(string fileOrDirectoryName, string directoryPathInArchive)
		{
			if (File.Exists(fileOrDirectoryName)) return AddFile(fileOrDirectoryName, directoryPathInArchive);
			if (Directory.Exists(fileOrDirectoryName)) return AddDirectory(fileOrDirectoryName, directoryPathInArchive);
			throw new FileNotFoundException($"That file or directory ({fileOrDirectoryName}) does not exist!");
		}

		public ZipEntry AddFile(string fileName)
		{
			return AddFile(fileName, null);
		}

		public ZipEntry AddFile(string fileName, string directoryPathInArchive)
		{
			var nameInArchive = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
			var ze = ZipEntry.CreateFromFile(fileName, nameInArchive);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", fileName);
			return _InternalAddEntry(ze);
		}

		public void RemoveEntries(ICollection<ZipEntry> entriesToRemove)
		{
			if (entriesToRemove == null) throw new ArgumentNullException(nameof(entriesToRemove));
			foreach (var item in entriesToRemove) RemoveEntry(item);
		}

		public void RemoveEntries(ICollection<string> entriesToRemove)
		{
			if (entriesToRemove == null) throw new ArgumentNullException(nameof(entriesToRemove));
			foreach (var item in entriesToRemove) RemoveEntry(item);
		}

		public void AddFiles(IEnumerable<string> fileNames)
		{
			AddFiles(fileNames, null);
		}

		public void UpdateFiles(IEnumerable<string> fileNames)
		{
			UpdateFiles(fileNames, null);
		}

		public void AddFiles(IEnumerable<string> fileNames, string directoryPathInArchive)
		{
			AddFiles(fileNames, false, directoryPathInArchive);
		}

		public void AddFiles(IEnumerable<string> fileNames, bool preserveDirHierarchy, string directoryPathInArchive)
		{
			if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));
			_addOperationCanceled = false;
			OnAddStarted();
			if (preserveDirHierarchy)
				foreach (var fileName in fileNames)
				{
					if (_addOperationCanceled) break;
					if (directoryPathInArchive != null)
					{
						var fullPath = Path.GetFullPath(Path.Combine(directoryPathInArchive, Path.GetDirectoryName(fileName)));
						AddFile(fileName, fullPath);
					}
					else
					{
						AddFile(fileName, null);
					}
				}
			else
				foreach (var fileName2 in fileNames)
				{
					if (_addOperationCanceled) break;
					AddFile(fileName2, directoryPathInArchive);
				}

			if (!_addOperationCanceled) OnAddCompleted();
		}

		public void UpdateFiles(IEnumerable<string> fileNames, string directoryPathInArchive)
		{
			if (fileNames == null) throw new ArgumentNullException(nameof(fileNames));
			OnAddStarted();
			foreach (var fileName in fileNames) UpdateFile(fileName, directoryPathInArchive);
			OnAddCompleted();
		}

		public ZipEntry UpdateFile(string fileName)
		{
			return UpdateFile(fileName, null);
		}

		public ZipEntry UpdateFile(string fileName, string directoryPathInArchive)
		{
			var fileName2 = ZipEntry.NameInArchive(fileName, directoryPathInArchive);
			if (this[fileName2] != null) RemoveEntry(fileName2);
			return AddFile(fileName, directoryPathInArchive);
		}

		public ZipEntry UpdateDirectory(string directoryName)
		{
			return UpdateDirectory(directoryName, null);
		}

		public ZipEntry UpdateDirectory(string directoryName, string directoryPathInArchive)
		{
			return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOrUpdate);
		}

		public void UpdateItem(string itemName)
		{
			UpdateItem(itemName, null);
		}

		public void UpdateItem(string itemName, string directoryPathInArchive)
		{
			if (File.Exists(itemName))
			{
				UpdateFile(itemName, directoryPathInArchive);
				return;
			}

			if (Directory.Exists(itemName))
			{
				UpdateDirectory(itemName, directoryPathInArchive);
				return;
			}

			throw new FileNotFoundException($"That file or directory ({itemName}) does not exist!");
		}

		public ZipEntry AddEntry(string entryName, string content)
		{
			return AddEntry(entryName, content, Encoding.Default);
		}

		public ZipEntry AddEntry(string entryName, string content, Encoding encoding)
		{
			var memoryStream = new MemoryStream();
			var streamWriter = new StreamWriter(memoryStream, encoding);
			streamWriter.Write(content);
			streamWriter.Flush();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			return AddEntry(entryName, memoryStream);
		}

		public ZipEntry AddEntry(string entryName, Stream stream)
		{
			var zipEntry = ZipEntry.CreateForStream(entryName, stream);
			zipEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			return _InternalAddEntry(zipEntry);
		}

		public ZipEntry AddEntry(string entryName, WriteDelegate writer)
		{
			var ze = ZipEntry.CreateForWriter(entryName, writer);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			return _InternalAddEntry(ze);
		}

		public ZipEntry AddEntry(string entryName, OpenDelegate opener, CloseDelegate closer)
		{
			var zipEntry = ZipEntry.CreateForJitStreamProvider(entryName, opener, closer);
			zipEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			if (Verbose) StatusMessageTextWriter.WriteLine("adding {0}...", entryName);
			return _InternalAddEntry(zipEntry);
		}

		private ZipEntry _InternalAddEntry(ZipEntry ze)
		{
			ze._container = new ZipContainer(this);
			ze.CompressionMethod = CompressionMethod;
			ze.CompressionLevel = CompressionLevel;
			ze.ExtractExistingFile = ExtractExistingFile;
			ze.ZipErrorAction = ZipErrorAction;
			ze.SetCompression = SetCompression;
			ze.AlternateEncoding = AlternateEncoding;
			ze.AlternateEncodingUsage = AlternateEncodingUsage;
			ze.Password = _Password;
			ze.Encryption = Encryption;
			ze.EmitTimesInWindowsFormatWhenSaving = EmitTimesInWindowsFormatWhenSaving;
			ze.EmitTimesInUnixFormatWhenSaving = EmitTimesInUnixFormatWhenSaving;
			InternalAddEntry(ze.FileName, ze);
			AfterAddEntry(ze);
			return ze;
		}

		public ZipEntry UpdateEntry(string entryName, string content)
		{
			return UpdateEntry(entryName, content, Encoding.Default);
		}

		public ZipEntry UpdateEntry(string entryName, string content, Encoding encoding)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, content, encoding);
		}

		public ZipEntry UpdateEntry(string entryName, WriteDelegate writer)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, writer);
		}

		public ZipEntry UpdateEntry(string entryName, OpenDelegate opener, CloseDelegate closer)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, opener, closer);
		}

		public ZipEntry UpdateEntry(string entryName, Stream stream)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, stream);
		}

		private void RemoveEntryForUpdate(string entryName)
		{
			if (string.IsNullOrEmpty(entryName)) throw new ArgumentNullException(nameof(entryName));
			string directoryPathInArchive = null;
			if (entryName.IndexOf('\\') != -1)
			{
				directoryPathInArchive = Path.GetDirectoryName(entryName);
				entryName = Path.GetFileName(entryName);
			}

			var fileName = ZipEntry.NameInArchive(entryName, directoryPathInArchive);
			if (this[fileName] != null) RemoveEntry(fileName);
		}

		public ZipEntry AddEntry(string entryName, byte[] byteContent)
		{
			if (byteContent == null) throw new ArgumentException("bad argument", nameof(byteContent));
			var stream = new MemoryStream(byteContent);
			return AddEntry(entryName, stream);
		}

		public ZipEntry UpdateEntry(string entryName, byte[] byteContent)
		{
			RemoveEntryForUpdate(entryName);
			return AddEntry(entryName, byteContent);
		}

		public ZipEntry AddDirectory(string directoryName)
		{
			return AddDirectory(directoryName, null);
		}

		public ZipEntry AddDirectory(string directoryName, string directoryPathInArchive)
		{
			return AddOrUpdateDirectoryImpl(directoryName, directoryPathInArchive, AddOrUpdateAction.AddOnly);
		}

		public ZipEntry AddDirectoryByName(string directoryNameInArchive)
		{
			var zipEntry = ZipEntry.CreateFromNothing(directoryNameInArchive);
			zipEntry._container = new ZipContainer(this);
			zipEntry.MarkAsDirectory();
			zipEntry.AlternateEncoding = AlternateEncoding;
			zipEntry.AlternateEncodingUsage = AlternateEncodingUsage;
			zipEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			zipEntry.EmitTimesInWindowsFormatWhenSaving = EmitTimesInWindowsFormatWhenSaving;
			zipEntry.EmitTimesInUnixFormatWhenSaving = EmitTimesInUnixFormatWhenSaving;
			zipEntry._Source = ZipEntrySource.Stream;
			InternalAddEntry(zipEntry.FileName, zipEntry);
			AfterAddEntry(zipEntry);
			return zipEntry;
		}

		private ZipEntry AddOrUpdateDirectoryImpl(string directoryName, string rootDirectoryPathInArchive,
			AddOrUpdateAction action)
		{
			if (rootDirectoryPathInArchive == null) rootDirectoryPathInArchive = "";
			return AddOrUpdateDirectoryImpl(directoryName, rootDirectoryPathInArchive, action, true, 0);
		}

		internal void InternalAddEntry(string name, ZipEntry entry)
		{
			_entries.Add(name, entry);
			_zipEntriesAsList = null;
			_contentsChanged = true;
		}

		private ZipEntry AddOrUpdateDirectoryImpl(string directoryName, string rootDirectoryPathInArchive,
			AddOrUpdateAction action, bool recurse, int level)
		{
			if (Verbose)
				StatusMessageTextWriter.WriteLine("{0} {1}...",
					action == AddOrUpdateAction.AddOnly ? "adding" : "Adding or updating", directoryName);
			if (level == 0)
			{
				_addOperationCanceled = false;
				OnAddStarted();
			}

			if (_addOperationCanceled) return null;
			var text = rootDirectoryPathInArchive;
			ZipEntry zipEntry = null;
			if (level > 0)
			{
				var num = directoryName.Length;
				for (var num2 = level; num2 > 0; num2--) num = directoryName.LastIndexOfAny("/\\".ToCharArray(), num - 1, num - 1);
				text = directoryName.Substring(num + 1);
				text = Path.Combine(rootDirectoryPathInArchive, text);
			}

			if (level > 0 || rootDirectoryPathInArchive != "")
			{
				zipEntry = ZipEntry.CreateFromFile(directoryName, text);
				zipEntry._container = new ZipContainer(this);
				zipEntry.AlternateEncoding = AlternateEncoding;
				zipEntry.AlternateEncodingUsage = AlternateEncodingUsage;
				zipEntry.MarkAsDirectory();
				zipEntry.EmitTimesInWindowsFormatWhenSaving = EmitTimesInWindowsFormatWhenSaving;
				zipEntry.EmitTimesInUnixFormatWhenSaving = EmitTimesInUnixFormatWhenSaving;
				if (!_entries.ContainsKey(zipEntry.FileName))
				{
					InternalAddEntry(zipEntry.FileName, zipEntry);
					AfterAddEntry(zipEntry);
				}

				text = zipEntry.FileName;
			}

			if (!_addOperationCanceled)
			{
				var files = Directory.GetFiles(directoryName);
				if (recurse)
				{
					var array = files;
					foreach (var fileName in array)
					{
						if (_addOperationCanceled) break;
						if (action == AddOrUpdateAction.AddOnly)
							AddFile(fileName, text);
						else
							UpdateFile(fileName, text);
					}

					if (!_addOperationCanceled)
					{
						var directories = Directory.GetDirectories(directoryName);
						var array2 = directories;
						foreach (var text2 in array2)
						{
							var attributes = File.GetAttributes(text2);
							if (AddDirectoryWillTraverseReparsePoints || (attributes & FileAttributes.ReparsePoint) == 0)
								AddOrUpdateDirectoryImpl(text2, rootDirectoryPathInArchive, action, recurse, level + 1);
						}
					}
				}
			}

			if (level == 0) OnAddCompleted();
			return zipEntry;
		}

		public bool ContainsEntry(string name)
		{
			return _entries.ContainsKey(SharedUtilities.NormalizePathForUseInZipFile(name));
		}

		public override string ToString()
		{
			return $"ZipFile::{Name}";
		}

		internal void NotifyEntryChanged()
		{
			_contentsChanged = true;
		}

		internal Stream StreamForDiskNumber(uint diskNumber)
		{
			if (diskNumber + 1 != _diskNumberWithCd && (diskNumber != 0 || _diskNumberWithCd != 0))
				return ZipSegmentedStream.ForReading(_readName ?? Name, diskNumber, _diskNumberWithCd);
			return ReadStream;
		}

		internal void Reset(bool whileSaving)
		{
			if (_JustSaved)
			{
				using (var zipFile = new ZipFile())
				{
					zipFile._readName = zipFile.Name = whileSaving ? (_readName ?? Name) : Name;
					zipFile.AlternateEncoding = AlternateEncoding;
					zipFile.AlternateEncodingUsage = AlternateEncodingUsage;
					ReadIntoInstance(zipFile);
					foreach (var item in zipFile)
					foreach (var item2 in this)
						if (item.FileName == item2.FileName)
						{
							item2.CopyMetaData(item);
							break;
						}
				}

				_JustSaved = false;
			}
		}

		public void Initialize(string fileName)
		{
			try
			{
				_InitInstance(fileName, null);
			}
			catch (Exception innerException)
			{
				throw new ZipException($"{fileName} is not a valid zip file", innerException);
			}
		}

		private void _initEntriesDictionary()
		{
			var comparer = CaseSensitiveRetrieval ? StringComparer.Ordinal : StringComparer.OrdinalIgnoreCase;
			_entries = _entries == null
				? new Dictionary<string, ZipEntry>(comparer)
				: new Dictionary<string, ZipEntry>(_entries, comparer);
		}

		private void _InitInstance(string zipFileName, TextWriter statusMessageWriter)
		{
			Name = zipFileName;
			StatusMessageTextWriter = statusMessageWriter;
			_contentsChanged = true;
			AddDirectoryWillTraverseReparsePoints = true;
			CompressionLevel = CompressionLevel.Default;
			ParallelDeflateThreshold = 524288L;
			ParallelDeflateThreshold = -1L;
			_initEntriesDictionary();
			if (File.Exists(Name))
			{
				if (FullScan)
					ReadIntoInstance_Orig(this);
				else
					ReadIntoInstance(this);
				_fileAlreadyExists = true;
			}
		}

		public void RemoveEntry(ZipEntry entry)
		{
			if (entry == null) throw new ArgumentNullException(nameof(entry));
			_entries.Remove(SharedUtilities.NormalizePathForUseInZipFile(entry.FileName));
			_zipEntriesAsList = null;
			_contentsChanged = true;
		}

		public void RemoveEntry(string fileName)
		{
			var fileName2 = ZipEntry.NameInArchive(fileName, null);
			var zipEntry = this[fileName2];
			if (zipEntry == null) throw new ArgumentException("The entry you specified was not found in the zip archive.");
			RemoveEntry(zipEntry);
		}

		protected virtual void Dispose(bool disposeManagedResources)
		{
			if (!_disposed)
			{
				if (disposeManagedResources)
				{
					if (_ReadStreamIsOurs && _readstream != null)
					{
						_readstream.Dispose();
						_readstream = null;
					}

					if (_temporaryFileName != null && Name != null && _writestream != null)
					{
						_writestream.Dispose();
						_writestream = null;
					}

					if (ParallelDeflater != null)
					{
						ParallelDeflater.Dispose();
						ParallelDeflater = null;
					}
				}

				_disposed = true;
			}
		}

		internal bool OnSaveBlock(ZipEntry entry, long bytesXferred, long totalBytesToXfer)
		{
			var saveProgress = SaveProgress;
			if (saveProgress != null)
			{
				var saveProgressEventArgs =
					SaveProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry, bytesXferred, totalBytesToXfer);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel) _saveOperationCanceled = true;
			}

			return _saveOperationCanceled;
		}

		private void OnSaveEntry(int current, ZipEntry entry, bool before)
		{
			var saveProgress = SaveProgress;
			if (saveProgress != null)
			{
				var saveProgressEventArgs = new SaveProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, entry);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel) _saveOperationCanceled = true;
			}
		}

		private void OnSaveEvent(ZipProgressEventType eventFlavor)
		{
			var saveProgress = SaveProgress;
			if (saveProgress != null)
			{
				var saveProgressEventArgs = new SaveProgressEventArgs(ArchiveNameForEvent, eventFlavor);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel) _saveOperationCanceled = true;
			}
		}

		private void OnSaveStarted()
		{
			var saveProgress = SaveProgress;
			if (saveProgress != null)
			{
				var saveProgressEventArgs = SaveProgressEventArgs.Started(ArchiveNameForEvent);
				saveProgress(this, saveProgressEventArgs);
				if (saveProgressEventArgs.Cancel) _saveOperationCanceled = true;
			}
		}

		private void OnSaveCompleted()
		{
			var saveProgress = SaveProgress;
			if (saveProgress != null)
			{
				var e = SaveProgressEventArgs.Completed(ArchiveNameForEvent);
				saveProgress(this, e);
			}
		}

		private void OnReadStarted()
		{
			var readProgress = ReadProgress;
			if (readProgress != null)
			{
				var e = ReadProgressEventArgs.Started(ArchiveNameForEvent);
				readProgress(this, e);
			}
		}

		private void OnReadCompleted()
		{
			var readProgress = ReadProgress;
			if (readProgress != null)
			{
				var e = ReadProgressEventArgs.Completed(ArchiveNameForEvent);
				readProgress(this, e);
			}
		}

		internal void OnReadBytes(ZipEntry entry)
		{
			var readProgress = ReadProgress;
			if (readProgress != null)
			{
				var e = ReadProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry, ReadStream.Position, LengthOfReadStream);
				readProgress(this, e);
			}
		}

		internal void OnReadEntry(bool before, ZipEntry entry)
		{
			var readProgress = ReadProgress;
			if (readProgress != null)
			{
				var e = before
					? ReadProgressEventArgs.Before(ArchiveNameForEvent, _entries.Count)
					: ReadProgressEventArgs.After(ArchiveNameForEvent, entry, _entries.Count);
				readProgress(this, e);
			}
		}

		private void OnExtractEntry(int current, bool before, ZipEntry currentEntry, string path)
		{
			var extractProgress = ExtractProgress;
			if (extractProgress != null)
			{
				var extractProgressEventArgs =
					new ExtractProgressEventArgs(ArchiveNameForEvent, before, _entries.Count, current, currentEntry, path);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel) _extractOperationCanceled = true;
			}
		}

		internal bool OnExtractBlock(ZipEntry entry, long bytesWritten, long totalBytesToWrite)
		{
			var extractProgress = ExtractProgress;
			if (extractProgress != null)
			{
				var extractProgressEventArgs =
					ExtractProgressEventArgs.ByteUpdate(ArchiveNameForEvent, entry, bytesWritten, totalBytesToWrite);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel) _extractOperationCanceled = true;
			}

			return _extractOperationCanceled;
		}

		internal bool OnSingleEntryExtract(ZipEntry entry, string path, bool before)
		{
			var extractProgress = ExtractProgress;
			if (extractProgress != null)
			{
				var extractProgressEventArgs = before
					? ExtractProgressEventArgs.BeforeExtractEntry(ArchiveNameForEvent, entry, path)
					: ExtractProgressEventArgs.AfterExtractEntry(ArchiveNameForEvent, entry, path);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel) _extractOperationCanceled = true;
			}

			return _extractOperationCanceled;
		}

		internal bool OnExtractExisting(ZipEntry entry, string path)
		{
			var extractProgress = ExtractProgress;
			if (extractProgress != null)
			{
				var extractProgressEventArgs = ExtractProgressEventArgs.ExtractExisting(ArchiveNameForEvent, entry, path);
				extractProgress(this, extractProgressEventArgs);
				if (extractProgressEventArgs.Cancel) _extractOperationCanceled = true;
			}

			return _extractOperationCanceled;
		}

		private void OnExtractAllCompleted(string path)
		{
			var extractProgress = ExtractProgress;
			if (extractProgress != null)
			{
				var e = ExtractProgressEventArgs.ExtractAllCompleted(ArchiveNameForEvent, path);
				extractProgress(this, e);
			}
		}

		private void OnExtractAllStarted(string path)
		{
			var extractProgress = ExtractProgress;
			if (extractProgress != null)
			{
				var e = ExtractProgressEventArgs.ExtractAllStarted(ArchiveNameForEvent, path);
				extractProgress(this, e);
			}
		}

		private void OnAddStarted()
		{
			var addProgress = AddProgress;
			if (addProgress != null)
			{
				var addProgressEventArgs = AddProgressEventArgs.Started(ArchiveNameForEvent);
				addProgress(this, addProgressEventArgs);
				if (addProgressEventArgs.Cancel) _addOperationCanceled = true;
			}
		}

		private void OnAddCompleted()
		{
			var addProgress = AddProgress;
			if (addProgress != null)
			{
				var e = AddProgressEventArgs.Completed(ArchiveNameForEvent);
				addProgress(this, e);
			}
		}

		internal void AfterAddEntry(ZipEntry entry)
		{
			var addProgress = AddProgress;
			if (addProgress != null)
			{
				var addProgressEventArgs = AddProgressEventArgs.AfterEntry(ArchiveNameForEvent, entry, _entries.Count);
				addProgress(this, addProgressEventArgs);
				if (addProgressEventArgs.Cancel) _addOperationCanceled = true;
			}
		}

		internal bool OnZipErrorSaving(ZipEntry entry, Exception exc)
		{
			if (ZipError != null)
				lock (LOCK)
				{
					var zipErrorEventArgs = ZipErrorEventArgs.Saving(Name, entry, exc);
					ZipError(this, zipErrorEventArgs);
					if (zipErrorEventArgs.Cancel) _saveOperationCanceled = true;
				}

			return _saveOperationCanceled;
		}

		public void ExtractAll(string path)
		{
			_InternalExtractAll(path, true);
		}

		public void ExtractAll(string path, ExtractExistingFileAction extractExistingFile)
		{
			ExtractExistingFile = extractExistingFile;
			_InternalExtractAll(path, true);
		}

		private void _InternalExtractAll(string path, bool overrideExtractExistingProperty)
		{
			var flag = Verbose;
			_inExtractAll = true;
			try
			{
				OnExtractAllStarted(path);
				var num = 0;
				foreach (var value in _entries.Values)
				{
					if (flag)
					{
						StatusMessageTextWriter.WriteLine("\n{1,-22} {2,-8} {3,4}   {4,-8}  {0}", "Name", "Modified", "Size", "Ratio",
							"Packed");
						StatusMessageTextWriter.WriteLine(new string('-', 72));
						flag = false;
					}

					if (Verbose)
					{
						StatusMessageTextWriter.WriteLine("{1,-22} {2,-8} {3,4:F0}%   {4,-8} {0}", value.FileName,
							value.LastModified.ToString("yyyy-MM-dd HH:mm:ss"), value.UncompressedSize, value.CompressionRatio,
							value.CompressedSize);
						if (!string.IsNullOrEmpty(value.Comment)) StatusMessageTextWriter.WriteLine("  Comment: {0}", value.Comment);
					}

					value.Password = _Password;
					OnExtractEntry(num, true, value, path);
					if (overrideExtractExistingProperty) value.ExtractExistingFile = ExtractExistingFile;
					value.Extract(path);
					num++;
					OnExtractEntry(num, false, value, path);
					if (_extractOperationCanceled) break;
				}

				if (!_extractOperationCanceled)
				{
					foreach (var value2 in _entries.Values)
						if (value2.IsDirectory || value2.FileName.EndsWith("/"))
						{
							var fileOrDirectory = value2.FileName.StartsWith("/")
								? Path.Combine(path, value2.FileName.Substring(1))
								: Path.Combine(path, value2.FileName);
							value2._SetTimes(fileOrDirectory, false);
						}

					OnExtractAllCompleted(path);
				}
			}
			finally
			{
				_inExtractAll = false;
			}
		}

		public static ZipFile Read(string fileName)
		{
			return Read(fileName, null, null, null);
		}

		public static ZipFile Read(string fileName, ReadOptions options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));
			return Read(fileName, options.StatusMessageWriter, options.Encoding, options.ReadProgress);
		}

		private static ZipFile Read(string fileName, TextWriter statusMessageWriter, Encoding encoding,
			EventHandler<ReadProgressEventArgs> readProgress)
		{
			var zipFile = new ZipFile();
			zipFile.AlternateEncoding = encoding ?? DefaultEncoding;
			zipFile.AlternateEncodingUsage = ZipOption.Always;
			zipFile.StatusMessageTextWriter = statusMessageWriter;
			zipFile.Name = fileName;
			if (readProgress != null) zipFile.ReadProgress = readProgress;
			if (zipFile.Verbose) zipFile.StatusMessageTextWriter.WriteLine("reading from {0}...", fileName);
			ReadIntoInstance(zipFile);
			zipFile._fileAlreadyExists = true;
			return zipFile;
		}

		public static ZipFile Read(Stream zipStream)
		{
			return Read(zipStream, null, null, null);
		}

		public static ZipFile Read(Stream zipStream, ReadOptions options)
		{
			if (options == null) throw new ArgumentNullException(nameof(options));
			return Read(zipStream, options.StatusMessageWriter, options.Encoding, options.ReadProgress);
		}

		private static ZipFile Read(Stream zipStream, TextWriter statusMessageWriter, Encoding encoding,
			EventHandler<ReadProgressEventArgs> readProgress)
		{
			if (zipStream == null) throw new ArgumentNullException(nameof(zipStream));
			var zipFile = new ZipFile();
			zipFile.StatusMessageTextWriter = statusMessageWriter;
			zipFile.AlternateEncoding = encoding ?? DefaultEncoding;
			zipFile.AlternateEncodingUsage = ZipOption.Always;
			if (readProgress != null) zipFile.ReadProgress += readProgress;
			zipFile._readstream = zipStream.Position == 0 ? zipStream : new OffsetStream(zipStream);
			zipFile._ReadStreamIsOurs = false;
			if (zipFile.Verbose) zipFile.StatusMessageTextWriter.WriteLine("reading from stream...");
			ReadIntoInstance(zipFile);
			return zipFile;
		}

		private static void ReadIntoInstance(ZipFile zf)
		{
			var readStream = zf.ReadStream;
			try
			{
				zf._readName = zf.Name;
				if (!readStream.CanSeek)
				{
					ReadIntoInstance_Orig(zf);
				}
				else
				{
					zf.OnReadStarted();
					var num = ReadFirstFourBytes(readStream);
					if (num != 101010256)
					{
						var num2 = 0;
						var flag = false;
						var num3 = readStream.Length - 64;
						var num4 = Math.Max(readStream.Length - 16384, 10L);
						do
						{
							if (num3 < 0) num3 = 0L;
							readStream.Seek(num3, SeekOrigin.Begin);
							var num5 = SharedUtilities.FindSignature(readStream, 101010256);
							if (num5 != -1)
							{
								flag = true;
							}
							else
							{
								if (num3 == 0) break;
								num2++;
								num3 -= 32 * (num2 + 1) * num2;
							}
						} while (!flag && num3 > num4);

						if (flag)
						{
							zf._locEndOfCDS = readStream.Position - 4;
							var array = new byte[16];
							readStream.Read(array, 0, array.Length);
							zf._diskNumberWithCd = BitConverter.ToUInt16(array, 2);
							if (zf._diskNumberWithCd == 65535)
								throw new ZipException("Spanned archives with more than 65534 segments are not supported at this time.");
							zf._diskNumberWithCd += 1u;
							var startIndex = 12;
							var num6 = BitConverter.ToUInt32(array, startIndex);
							if (num6 == 4294967295u)
							{
								Zip64SeekToCentralDirectory(zf);
							}
							else
							{
								zf._OffsetOfCentralDirectory = num6;
								readStream.Seek(num6, SeekOrigin.Begin);
							}

							ReadCentralDirectory(zf);
						}
						else
						{
							readStream.Seek(0L, SeekOrigin.Begin);
							ReadIntoInstance_Orig(zf);
						}

						goto end_IL_0007;
					}
				}

				return;
				end_IL_0007:;
			}
			catch (Exception innerException)
			{
				if (zf._ReadStreamIsOurs && zf._readstream != null)
				{
					zf._readstream.Dispose();
					zf._readstream = null;
				}

				throw new ZipException("Cannot read that as a ZipFile", innerException);
			}

			zf._contentsChanged = false;
		}

		private static void Zip64SeekToCentralDirectory(ZipFile zf)
		{
			var readStream = zf.ReadStream;
			var array = new byte[16];
			readStream.Seek(-40L, SeekOrigin.Current);
			readStream.Read(array, 0, 16);
			var num = BitConverter.ToInt64(array, 8);
			zf._OffsetOfCentralDirectory = 4294967295u;
			zf._OffsetOfCentralDirectory64 = num;
			readStream.Seek(num, SeekOrigin.Begin);
			var num2 = (uint) SharedUtilities.ReadInt(readStream);
			if (num2 != 101075792)
				throw new BadReadException(
					$"  Bad signature (0x{num2:X8}) looking for ZIP64 EoCD Record at position 0x{readStream.Position:X8}");
			readStream.Read(array, 0, 8);
			var num3 = BitConverter.ToInt64(array, 0);
			array = new byte[num3];
			readStream.Read(array, 0, array.Length);
			num = BitConverter.ToInt64(array, 36);
			readStream.Seek(num, SeekOrigin.Begin);
		}

		private static uint ReadFirstFourBytes(Stream s)
		{
			return (uint) SharedUtilities.ReadInt(s);
		}

		private static void ReadCentralDirectory(ZipFile zf)
		{
			var flag = false;
			var dictionary = new Dictionary<string, object>();
			ZipEntry zipEntry;
			while ((zipEntry = ZipEntry.ReadDirEntry(zf, dictionary)) != null)
			{
				zipEntry.ResetDirEntry();
				zf.OnReadEntry(true, null);
				if (zf.Verbose) zf.StatusMessageTextWriter.WriteLine("entry {0}", zipEntry.FileName);
				zf._entries.Add(zipEntry.FileName, zipEntry);
				if (zipEntry._InputUsesZip64) flag = true;
				dictionary.Add(zipEntry.FileName, null);
			}

			if (flag) zf.UseZip64WhenSaving = Zip64Option.Always;
			if (zf._locEndOfCDS > 0) zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);
			ReadCentralDirectoryFooter(zf);
			if (zf.Verbose && !string.IsNullOrEmpty(zf.Comment))
				zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);
			if (zf.Verbose) zf.StatusMessageTextWriter.WriteLine("read in {0} entries.", zf._entries.Count);
			zf.OnReadCompleted();
		}

		private static void ReadIntoInstance_Orig(ZipFile zf)
		{
			zf.OnReadStarted();
			zf._entries = new Dictionary<string, ZipEntry>();
			if (zf.Verbose)
				if (zf.Name == null)
					zf.StatusMessageTextWriter.WriteLine("Reading zip from stream...");
				else
					zf.StatusMessageTextWriter.WriteLine("Reading zip {0}...", zf.Name);
			var first = true;
			var zc = new ZipContainer(zf);
			ZipEntry zipEntry;
			while ((zipEntry = ZipEntry.ReadEntry(zc, first)) != null)
			{
				if (zf.Verbose) zf.StatusMessageTextWriter.WriteLine("  {0}", zipEntry.FileName);
				zf._entries.Add(zipEntry.FileName, zipEntry);
				first = false;
			}

			try
			{
				var dictionary = new Dictionary<string, object>();
				ZipEntry zipEntry2;
				while ((zipEntry2 = ZipEntry.ReadDirEntry(zf, dictionary)) != null)
				{
					var zipEntry3 = zf._entries[zipEntry2.FileName];
					if (zipEntry3 != null)
					{
						zipEntry3._Comment = zipEntry2.Comment;
						if (zipEntry2.IsDirectory) zipEntry3.MarkAsDirectory();
					}

					dictionary.Add(zipEntry2.FileName, null);
				}

				if (zf._locEndOfCDS > 0) zf.ReadStream.Seek(zf._locEndOfCDS, SeekOrigin.Begin);
				ReadCentralDirectoryFooter(zf);
				if (zf.Verbose && !string.IsNullOrEmpty(zf.Comment))
					zf.StatusMessageTextWriter.WriteLine("Zip file Comment: {0}", zf.Comment);
			}
			catch (ZipException)
			{
			}
			catch (IOException)
			{
			}

			zf.OnReadCompleted();
		}

		private static void ReadCentralDirectoryFooter(ZipFile zf)
		{
			var readStream = zf.ReadStream;
			var num = SharedUtilities.ReadSignature(readStream);
			byte[] array = null;
			var num2 = 0;
			if ((long) num == 101075792)
			{
				array = new byte[52];
				readStream.Read(array, 0, array.Length);
				var num3 = BitConverter.ToInt64(array, 0);
				if (num3 < 44) throw new ZipException("Bad size in the ZIP64 Central Directory.");
				zf._versionMadeBy = BitConverter.ToUInt16(array, num2);
				num2 += 2;
				zf._versionNeededToExtract = BitConverter.ToUInt16(array, num2);
				num2 += 2;
				zf._diskNumberWithCd = BitConverter.ToUInt32(array, num2);
				num2 += 2;
				array = new byte[num3 - 44];
				readStream.Read(array, 0, array.Length);
				num = SharedUtilities.ReadSignature(readStream);
				if ((long) num != 117853008) throw new ZipException("Inconsistent metadata in the ZIP64 Central Directory.");
				array = new byte[16];
				readStream.Read(array, 0, array.Length);
				num = SharedUtilities.ReadSignature(readStream);
			}

			if ((long) num != 101010256)
			{
				readStream.Seek(-4L, SeekOrigin.Current);
				throw new BadReadException($"Bad signature ({num:X8}) at position 0x{readStream.Position:X8}");
			}

			array = new byte[16];
			zf.ReadStream.Read(array, 0, array.Length);
			if (zf._diskNumberWithCd == 0) zf._diskNumberWithCd = BitConverter.ToUInt16(array, 2);
			ReadZipFileComment(zf);
		}

		private static void ReadZipFileComment(ZipFile zf)
		{
			var array = new byte[2];
			zf.ReadStream.Read(array, 0, array.Length);
			var num = (short) (array[0] + array[1] * 256);
			if (num > 0)
			{
				array = new byte[num];
				zf.ReadStream.Read(array, 0, array.Length);
				var text = zf.Comment = zf.AlternateEncoding.GetString(array, 0, (int) array.LongLength);
			}
		}

		public static bool IsZipFile(string fileName)
		{
			return IsZipFile(fileName, false);
		}

		public static bool IsZipFile(string fileName, bool testExtract)
		{
			var result = false;
			try
			{
				if (!File.Exists(fileName)) return false;
				using (var stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite))
				{
					result = IsZipFile(stream, testExtract);
					return result;
				}
			}
			catch (IOException)
			{
				return result;
			}
			catch (ZipException)
			{
				return result;
			}
		}

		public static bool IsZipFile(Stream stream, bool testExtract)
		{
			if (stream == null) throw new ArgumentNullException(nameof(stream));
			var result = false;
			try
			{
				if (!stream.CanRead) return false;
				var @null = Stream.Null;
				using (var zipFile = Read(stream, null, null, null))
				{
					if (testExtract)
						foreach (var item in zipFile)
							if (!item.IsDirectory)
								item.Extract(@null);
				}

				result = true;
				return result;
			}
			catch (IOException)
			{
				return result;
			}
			catch (ZipException)
			{
				return result;
			}
		}

		private void DeleteFileWithRetry(string filename)
		{
			var flag = false;
			var num = 3;
			for (var i = 0; i < num; i++)
			{
				if (flag) break;
				try
				{
					File.Delete(filename);
					flag = true;
				}
				catch (UnauthorizedAccessException)
				{
					Console.WriteLine("************************************************** Retry delete.");
					Thread.Sleep(200 + i * 200);
				}
			}
		}

		public void Save()
		{
			try
			{
				var flag = false;
				_saveOperationCanceled = false;
				_numberOfSegmentsForMostRecentSave = 0u;
				OnSaveStarted();
				if (WriteStream == null) throw new BadStateException("You haven't specified where to save the zip.");
				if (Name != null && Name.EndsWith(".exe") && !_SavingSfx)
					throw new BadStateException("You specified an EXE for a plain zip file.");
				ICollection<ZipEntry> collection;
				if (!_contentsChanged)
				{
					OnSaveCompleted();
					if (Verbose) StatusMessageTextWriter.WriteLine("No save is necessary....");
				}
				else
				{
					Reset(true);
					if (Verbose) StatusMessageTextWriter.WriteLine("saving....");
					if (_entries.Count >= 65535 && _zip64 == Zip64Option.Default)
						throw new ZipException(
							"The number of entries is 65535 or greater. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
					var num = 0;
					collection = SortEntriesBeforeSaving ? EntriesSorted : Entries;
					foreach (var item in collection)
					{
						OnSaveEntry(num, item, true);
						item.Write(WriteStream);
						if (_saveOperationCanceled) break;
						num++;
						OnSaveEntry(num, item, false);
						if (_saveOperationCanceled) break;
						if (item.IncludedInMostRecentSave) flag |= item.OutputUsedZip64.Value;
					}

					if (!_saveOperationCanceled)
					{
						var zipSegmentedStream = WriteStream as ZipSegmentedStream;
						_numberOfSegmentsForMostRecentSave = zipSegmentedStream == null ? 1 : zipSegmentedStream.CurrentSegment;
						var flag2 = ZipOutput.WriteCentralDirectoryStructure(WriteStream, collection, _numberOfSegmentsForMostRecentSave,
							_zip64, Comment, new ZipContainer(this));
						OnSaveEvent(ZipProgressEventType.Saving_AfterSaveTempArchive);
						_hasBeenSaved = true;
						_contentsChanged = false;
						flag |= flag2;
						OutputUsedZip64 = flag;
						if (Name != null && (_temporaryFileName != null || zipSegmentedStream != null))
						{
							WriteStream.Dispose();
							if (!_saveOperationCanceled)
							{
								if (_fileAlreadyExists && _readstream != null)
								{
									_readstream.Close();
									_readstream = null;
									foreach (var item2 in collection)
									{
										var zipSegmentedStream2 = item2._archiveStream as ZipSegmentedStream;
										if (zipSegmentedStream2 != null) zipSegmentedStream2.Dispose();
										item2._archiveStream = null;
									}
								}

								string text = null;
								if (File.Exists(Name))
								{
									text = Name + "." + Path.GetRandomFileName();
									if (File.Exists(text)) DeleteFileWithRetry(text);
									File.Move(Name, text);
								}

								OnSaveEvent(ZipProgressEventType.Saving_BeforeRenameTempArchive);
								File.Move(zipSegmentedStream != null ? zipSegmentedStream.CurrentTempName : _temporaryFileName, Name);
								OnSaveEvent(ZipProgressEventType.Saving_AfterRenameTempArchive);
								if (text != null)
									try
									{
										if (File.Exists(text)) File.Delete(text);
									}
									catch
									{
									}

								_fileAlreadyExists = true;
								goto IL_0300;
							}

							goto end_IL_0000;
						}

						goto IL_0300;
					}
				}

				goto end_IL_0000;
				IL_0300:
				NotifyEntriesSaveComplete(collection);
				OnSaveCompleted();
				_JustSaved = true;
				end_IL_0000:;
			}
			finally
			{
				CleanupAfterSaveOperation();
			}
		}

		private static void NotifyEntriesSaveComplete(ICollection<ZipEntry> c)
		{
			foreach (var item in c) item.NotifySaveComplete();
		}

		private void RemoveTempFile()
		{
			try
			{
				if (File.Exists(_temporaryFileName)) File.Delete(_temporaryFileName);
			}
			catch (IOException ex)
			{
				if (Verbose) StatusMessageTextWriter.WriteLine("ZipFile::Save: could not delete temp file: {0}.", ex.Message);
			}
		}

		private void CleanupAfterSaveOperation()
		{
			if (Name != null)
			{
				if (_writestream != null)
					try
					{
						_writestream.Dispose();
					}
					catch (IOException)
					{
					}

				_writestream = null;
				if (_temporaryFileName != null)
				{
					RemoveTempFile();
					_temporaryFileName = null;
				}
			}
		}

		public void Save(string fileName)
		{
			if (Name == null)
				_writestream = null;
			else
				_readName = Name;
			Name = fileName;
			if (Directory.Exists(Name))
				throw new ZipException("Bad Directory",
					new ArgumentException("That name specifies an existing directory. Please specify a filename.", nameof(fileName)));
			_contentsChanged = true;
			_fileAlreadyExists = File.Exists(Name);
			Save();
		}

		public void Save(Stream outputStream)
		{
			if (outputStream == null) throw new ArgumentNullException(nameof(outputStream));
			if (!outputStream.CanWrite) throw new ArgumentException("Must be a writable stream.", nameof(outputStream));
			Name = null;
			_writestream = new CountingStream(outputStream);
			_contentsChanged = true;
			_fileAlreadyExists = false;
			Save();
		}

		public void AddSelectedFiles(string selectionCriteria)
		{
			AddSelectedFiles(selectionCriteria, ".", null, false);
		}

		public void AddSelectedFiles(string selectionCriteria, bool recurseDirectories)
		{
			AddSelectedFiles(selectionCriteria, ".", null, recurseDirectories);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk)
		{
			AddSelectedFiles(selectionCriteria, directoryOnDisk, null, false);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk, bool recurseDirectories)
		{
			AddSelectedFiles(selectionCriteria, directoryOnDisk, null, recurseDirectories);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive)
		{
			AddSelectedFiles(selectionCriteria, directoryOnDisk, directoryPathInArchive, false);
		}

		public void AddSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive,
			bool recurseDirectories)
		{
			AddOrUpdateSelectedFiles(selectionCriteria, directoryOnDisk, directoryPathInArchive, recurseDirectories, false);
		}

		public void UpdateSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive,
			bool recurseDirectories)
		{
			AddOrUpdateSelectedFiles(selectionCriteria, directoryOnDisk, directoryPathInArchive, recurseDirectories, true);
		}

		public ICollection<ZipEntry> SelectEntries(string selectionCriteria)
		{
			var fileSelector = new FileSelector(selectionCriteria, AddDirectoryWillTraverseReparsePoints);
			return fileSelector.SelectEntries(this);
		}

		public ICollection<ZipEntry> SelectEntries(string selectionCriteria, string directoryPathInArchive)
		{
			var fileSelector = new FileSelector(selectionCriteria, AddDirectoryWillTraverseReparsePoints);
			return fileSelector.SelectEntries(this, directoryPathInArchive);
		}

		public int RemoveSelectedEntries(string selectionCriteria)
		{
			var collection = SelectEntries(selectionCriteria);
			RemoveEntries(collection);
			return collection.Count;
		}

		public int RemoveSelectedEntries(string selectionCriteria, string directoryPathInArchive)
		{
			var collection = SelectEntries(selectionCriteria, directoryPathInArchive);
			RemoveEntries(collection);
			return collection.Count;
		}

		public void ExtractSelectedEntries(string selectionCriteria)
		{
			foreach (var item in SelectEntries(selectionCriteria))
			{
				item.Password = _Password;
				item.Extract();
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, ExtractExistingFileAction extractExistingFile)
		{
			foreach (var item in SelectEntries(selectionCriteria))
			{
				item.Password = _Password;
				item.Extract(extractExistingFile);
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, string directoryPathInArchive)
		{
			foreach (var item in SelectEntries(selectionCriteria, directoryPathInArchive))
			{
				item.Password = _Password;
				item.Extract();
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, string directoryInArchive, string extractDirectory)
		{
			foreach (var item in SelectEntries(selectionCriteria, directoryInArchive))
			{
				item.Password = _Password;
				item.Extract(extractDirectory);
			}
		}

		public void ExtractSelectedEntries(string selectionCriteria, string directoryPathInArchive, string extractDirectory,
			ExtractExistingFileAction extractExistingFile)
		{
			foreach (var item in SelectEntries(selectionCriteria, directoryPathInArchive))
			{
				item.Password = _Password;
				item.Extract(extractDirectory, extractExistingFile);
			}
		}

		private static string ReplaceLeadingDirectory(string original, string pattern, string replacement)
		{
			var text = original.ToUpper();
			var text2 = pattern.ToUpper();
			if (text.IndexOf(text2, StringComparison.Ordinal) != 0) return original;
			return replacement + original.Substring(text2.Length);
		}

		private void AddOrUpdateSelectedFiles(string selectionCriteria, string directoryOnDisk, string directoryPathInArchive,
			bool recurseDirectories, bool wantUpdate)
		{
			if (directoryOnDisk == null && Directory.Exists(selectionCriteria))
			{
				directoryOnDisk = selectionCriteria;
				selectionCriteria = "*.*";
			}
			else if (string.IsNullOrEmpty(directoryOnDisk))
			{
				directoryOnDisk = ".";
			}

			while (directoryOnDisk.EndsWith("\\")) directoryOnDisk = directoryOnDisk.Substring(0, directoryOnDisk.Length - 1);
			if (Verbose)
				StatusMessageTextWriter.WriteLine("adding selection '{0}' from dir '{1}'...", selectionCriteria, directoryOnDisk);
			var fileSelector = new FileSelector(selectionCriteria, AddDirectoryWillTraverseReparsePoints);
			var readOnlyCollection = fileSelector.SelectFiles(directoryOnDisk, recurseDirectories);
			if (Verbose) StatusMessageTextWriter.WriteLine("found {0} files...", readOnlyCollection.Count);
			OnAddStarted();
			var action = (AddOrUpdateAction) (wantUpdate ? 1 : 0);
			foreach (var item in readOnlyCollection)
			{
				var text = directoryPathInArchive == null
					? null
					: ReplaceLeadingDirectory(Path.GetDirectoryName(item), directoryOnDisk, directoryPathInArchive);
				if (File.Exists(item))
					if (wantUpdate)
						UpdateFile(item, text);
					else
						AddFile(item, text);
				else
					AddOrUpdateDirectoryImpl(item, text, action, false, 0);
			}

			OnAddCompleted();
		}

		[DispId(-4)]
		public IEnumerator GetNewEnum()
		{
			return GetEnumerator();
		}

		private class ExtractorSettings
		{
			public List<string> CopyThroughResources;
			public SelfExtractorFlavor Flavor;

			public List<string> ReferencedAssemblies;

			public List<string> ResourcesToCompile;
		}
	}
}