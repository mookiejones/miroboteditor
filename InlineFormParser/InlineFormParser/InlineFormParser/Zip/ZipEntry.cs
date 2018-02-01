﻿#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:43 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

#endregion

namespace InlineFormParser.Zip
{
	public delegate void WriteDelegate(string entryName, Stream stream);

	public delegate Stream OpenDelegate(string entryName);

	public delegate void CloseDelegate(string entryName, Stream stream);

	public delegate CompressionLevel SetCompressionCallback(string localFileName, string fileNameInArchive);

	[ComVisible(true)]
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[Guid("ebc25cf6-9120-4283-b972-0e5520d00004")]
	public class ZipEntry
	{
		private static readonly Encoding ibm437 = Encoding.GetEncoding("IBM437");

		private static DateTime _unixEpoch = new DateTime(1970, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private static readonly DateTime _win32Epoch = DateTime.FromFileTimeUtc(0L);

		private static DateTime _zeroHour = new DateTime(1, 1, 1, 0, 0, 0, DateTimeKind.Utc);

		private long __FileDataPosition = -1L;

		private Encoding _actualEncoding;

		internal Stream _archiveStream;

		private DateTime _Atime;

		internal short _BitField;

		private CloseDelegate _CloseDelegate;

		internal string _Comment;

		private byte[] _CommentBytes;

		private short _commentLength;

		internal long _CompressedFileDataSize;

		internal long _CompressedSize;

		private CompressionLevel _CompressionLevel;

		internal short _CompressionMethod;

		private short _CompressionMethod_FromZipFile;

		internal ZipContainer _container;

		internal int _Crc32;

		private bool _crcCalculated;

		private DateTime _Ctime;

		private uint _diskNumber;

		private bool _emitNtfsTimes = true;

		private bool _emitUnixTimes;

		internal EncryptionAlgorithm _Encryption;

		internal EncryptionAlgorithm _Encryption_FromZipFile;

		private byte[] _EntryHeader;

		private int _ExternalFileAttrs;

		internal byte[] _Extra;

		private short _extraFieldLength;

		private string _FileNameInArchive;

		private short _filenameLength;

		private long _future_ROLH;

		private Stream _inputDecryptorStream;

		internal bool _InputUsesZip64;

		private short _InternalFileAttrs;

		private bool _ioOperationCanceled;

		internal DateTime _LastModified;

		private int _LengthOfHeader;

		private int _LengthOfTrailer;

		internal string _LocalFileName;

		private bool _metadataChanged;

		private DateTime _Mtime;

		private bool _ntfsTimesAreSet;

		private OpenDelegate _OpenDelegate;

		private readonly object _outputLock = new object();

		internal string _Password;

		private bool _presumeZip64;

		private int _readExtraDepth;

		internal long _RelativeOffsetOfLocalHeader;

		private bool _restreamRequiredOnSave;

		private bool _skippedDuringSave;

		internal ZipEntrySource _Source;

		private bool _sourceIsEncrypted;

		private Stream _sourceStream;

		private long? _sourceStreamOriginalPosition;

		internal int _TimeBlob;

		private long _TotalEntrySize;

		private readonly bool _TrimVolumeFromFullyQualifiedPaths = true;

		internal long _UncompressedSize;

		private uint _UnsupportedAlgorithmId;

		private short _VersionMadeBy;

		internal short _VersionNeeded;

		internal byte[] _WeakEncryptionHeader;

		private WriteDelegate _WriteDelegate;

		private ZipCrypto _zipCrypto_forExtract;

		private ZipCrypto _zipCrypto_forWrite;

		public ZipEntry()
		{
			_CompressionMethod = 8;
			_CompressionLevel = CompressionLevel.Default;
			_Encryption = EncryptionAlgorithm.None;
			_Source = ZipEntrySource.None;
			AlternateEncoding = Encoding.GetEncoding("IBM437");
			AlternateEncodingUsage = ZipOption.Default;
		}

		internal bool AttributesIndicateDirectory
		{
			get
			{
				if (_InternalFileAttrs == 0) return (_ExternalFileAttrs & 0x10) == 16;
				return false;
			}
		}

		public string Info
		{
			get
			{
				StringBuilder stringBuilder = new StringBuilder();
				stringBuilder.Append($"          ZipEntry: {FileName}\n")
					.Append($"   Version Made By: {_VersionMadeBy}\n")
					.Append($" Needed to extract: {VersionNeeded}\n");
				if (IsDirectory)
					stringBuilder.Append("        Entry type: directory\n");
				else
					stringBuilder.Append($"         File type: {(IsText ? "text" : "binary")}\n")
						.Append($"       Compression: {CompressionMethod}\n")
						.Append($"        Compressed: 0x{CompressedSize:X}\n")
						.Append($"      Uncompressed: 0x{UncompressedSize:X}\n")
						.Append($"             CRC32: 0x{_Crc32:X8}\n");
				stringBuilder.Append($"       Disk Number: {_diskNumber}\n");
				if (_RelativeOffsetOfLocalHeader > 4294967295u)
					stringBuilder.Append($"   Relative Offset: 0x{_RelativeOffsetOfLocalHeader:X16}\n");
				else
					stringBuilder.Append($"   Relative Offset: 0x{_RelativeOffsetOfLocalHeader:X8}\n");
				stringBuilder.Append($"         Bit Field: 0x{_BitField:X4}\n")
					.Append($"        Encrypted?: {_sourceIsEncrypted}\n")
					.Append($"          Timeblob: 0x{_TimeBlob:X8}\n")
					.Append($"              Time: {SharedUtilities.PackedToDateTime(_TimeBlob)}\n");
				stringBuilder.Append($"         Is Zip64?: {_InputUsesZip64}\n");
				if (!string.IsNullOrEmpty(_Comment)) stringBuilder.Append($"           Comment: {_Comment}\n");
				stringBuilder.Append("\n");
				return stringBuilder.ToString();
			}
		}

		public DateTime LastModified
		{
			get => _LastModified.ToLocalTime();
			set
			{
				_LastModified = value.Kind == DateTimeKind.Unspecified
					? DateTime.SpecifyKind(value, DateTimeKind.Local)
					: value.ToLocalTime();
				_Mtime = SharedUtilities.AdjustTime_Reverse(_LastModified).ToUniversalTime();
				_metadataChanged = true;
			}
		}

		private int BufferSize => _container.BufferSize;

		public DateTime ModifiedTime
		{
			get => _Mtime;
			set => SetEntryTimes(_Ctime, _Atime, value);
		}

		public DateTime AccessedTime
		{
			get => _Atime;
			set => SetEntryTimes(_Ctime, value, _Mtime);
		}

		public DateTime CreationTime
		{
			get => _Ctime;
			set => SetEntryTimes(value, _Atime, _Mtime);
		}

		public bool EmitTimesInWindowsFormatWhenSaving
		{
			get => _emitNtfsTimes;
			set
			{
				_emitNtfsTimes = value;
				_metadataChanged = true;
			}
		}

		public bool EmitTimesInUnixFormatWhenSaving
		{
			get => _emitUnixTimes;
			set
			{
				_emitUnixTimes = value;
				_metadataChanged = true;
			}
		}

		public ZipEntryTimestamp Timestamp { get; private set; }

		public FileAttributes Attributes
		{
			get => (FileAttributes) _ExternalFileAttrs;
			set
			{
				_ExternalFileAttrs = (int) value;
				_VersionMadeBy = 45;
				_metadataChanged = true;
			}
		}

		internal string LocalFileName => _LocalFileName;

		public string FileName
		{
			get => _FileNameInArchive;
			set
			{
				if (_container.ZipFile == null)
					throw new ZipException("Cannot rename; this is not supported in ZipOutputStream/ZipInputStream.");
				if (string.IsNullOrEmpty(value)) throw new ZipException("The FileName must be non empty and non-null.");
				string text = NameInArchive(value, null);
				if (!(_FileNameInArchive == text))
				{
					_container.ZipFile.RemoveEntry(this);
					_container.ZipFile.InternalAddEntry(text, this);
					_FileNameInArchive = text;
					_container.ZipFile.NotifyEntryChanged();
					_metadataChanged = true;
				}
			}
		}

		public Stream InputStream
		{
			get => _sourceStream;
			set
			{
				if (_Source != ZipEntrySource.Stream) throw new ZipException("You must not set the input stream for this entry.");
				InputStreamWasJitProvided = true;
				_sourceStream = value;
			}
		}

		public bool InputStreamWasJitProvided { get; private set; }

		public ZipEntrySource Source => _Source;

		public short VersionNeeded => _VersionNeeded;

		public string Comment
		{
			get => _Comment;
			set
			{
				_Comment = value;
				_metadataChanged = true;
			}
		}

		public bool? RequiresZip64 { get; private set; }

		public bool? OutputUsedZip64 { get; private set; }

		public short BitField => _BitField;

		public CompressionMethod CompressionMethod
		{
			get => (CompressionMethod) _CompressionMethod;
			set
			{
				if (value != (CompressionMethod) _CompressionMethod)
				{
					if (value != 0 && value != CompressionMethod.Deflate)
						throw new InvalidOperationException("Unsupported compression method.");
					_CompressionMethod = (short) value;
					if (_CompressionMethod == 0)
						_CompressionLevel = CompressionLevel.None;
					else if (CompressionLevel == CompressionLevel.None)
						_CompressionLevel = CompressionLevel.Default;
					if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
					_restreamRequiredOnSave = true;
				}
			}
		}

		public CompressionLevel CompressionLevel
		{
			get => _CompressionLevel;
			set
			{
				if (_CompressionMethod != 8 && _CompressionMethod != 0) return;
				if (value == CompressionLevel.Default && _CompressionMethod == 8) return;
				_CompressionLevel = value;
				if (value == CompressionLevel.None && _CompressionMethod == 0) return;
				if (_CompressionLevel == CompressionLevel.None)
					_CompressionMethod = 0;
				else
					_CompressionMethod = 8;
				if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
				_restreamRequiredOnSave = true;
			}
		}

		public long CompressedSize => _CompressedSize;

		public long UncompressedSize => _UncompressedSize;

		public double CompressionRatio
		{
			get
			{
				if (UncompressedSize == 0) return 0.0;
				return 100.0 * (1.0 - 1.0 * CompressedSize / (1.0 * UncompressedSize));
			}
		}

		public int Crc => _Crc32;

		public bool IsDirectory { get; private set; }

		public bool UsesEncryption => _Encryption_FromZipFile != EncryptionAlgorithm.None;

		public EncryptionAlgorithm Encryption
		{
			get => _Encryption;
			set
			{
				if (value != _Encryption)
				{
					if (value == EncryptionAlgorithm.Unsupported)
						throw new InvalidOperationException("You may not set Encryption to that value.");
					_Encryption = value;
					_restreamRequiredOnSave = true;
					if (_container.ZipFile != null) _container.ZipFile.NotifyEntryChanged();
				}
			}
		}

		public string Password
		{
			private get { return _Password; }
			set
			{
				_Password = value;
				if (_Password == null)
				{
					_Encryption = EncryptionAlgorithm.None;
				}
				else
				{
					if (_Source == ZipEntrySource.ZipFile && !_sourceIsEncrypted) _restreamRequiredOnSave = true;
					if (Encryption == EncryptionAlgorithm.None) _Encryption = EncryptionAlgorithm.PkzipWeak;
				}
			}
		}

		internal bool IsChanged => _restreamRequiredOnSave | _metadataChanged;

		public ExtractExistingFileAction ExtractExistingFile { get; set; }

		public ZipErrorAction ZipErrorAction { get; set; }

		public bool IncludedInMostRecentSave => !_skippedDuringSave;

		public SetCompressionCallback SetCompression { get; set; }

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
					AlternateEncoding = ZipFile.DefaultEncoding;
					AlternateEncodingUsage = ZipOption.Default;
				}
			}
		}

		[Obsolete("This property is obsolete since v1.9.1.6. Use AlternateEncoding and AlternateEncodingUsage instead.",
			true)]
		public Encoding ProvisionalAlternateEncoding { get; set; }

		public Encoding AlternateEncoding { get; set; }

		public ZipOption AlternateEncodingUsage { get; set; }

		public bool IsText { get; set; }

		internal Stream ArchiveStream
		{
			get
			{
				if (_archiveStream == null)
					if (_container.ZipFile != null)
					{
						ZipFile zipFile = _container.ZipFile;
						zipFile.Reset(false);
						_archiveStream = zipFile.StreamForDiskNumber(_diskNumber);
					}
					else
					{
						_archiveStream = _container.ZipOutputStream.OutputStream;
					}

				return _archiveStream;
			}
		}

		internal long FileDataPosition
		{
			get
			{
				if (__FileDataPosition == -1) SetFdpLoh();
				return __FileDataPosition;
			}
		}

		private int LengthOfHeader
		{
			get
			{
				if (_LengthOfHeader == 0) SetFdpLoh();
				return _LengthOfHeader;
			}
		}

		private string UnsupportedAlgorithm
		{
			get
			{
				string empty = string.Empty;
				switch (_UnsupportedAlgorithmId)
				{
					case 0u:
						return "--";
					case 26113u:
						return "DES";
					case 26114u:
						return "RC2";
					case 26115u:
						return "3DES-168";
					case 26121u:
						return "3DES-112";
					case 26126u:
						return "PKWare AES128";
					case 26127u:
						return "PKWare AES192";
					case 26128u:
						return "PKWare AES256";
					case 26370u:
						return "RC2";
					case 26400u:
						return "Blowfish";
					case 26401u:
						return "Twofish";
					case 26625u:
						return "RC4";
					default:
						return $"Unknown (0x{_UnsupportedAlgorithmId:X4})";
				}
			}
		}

		private string UnsupportedCompressionMethod
		{
			get
			{
				string empty = string.Empty;
				switch (_CompressionMethod)
				{
					case 0:
						return "Store";
					case 1:
						return "Shrink";
					case 8:
						return "DEFLATE";
					case 9:
						return "Deflate64";
					case 12:
						return "BZIP2";
					case 14:
						return "LZMA";
					case 19:
						return "LZ77";
					case 98:
						return "PPMd";
					default:
						return $"Unknown (0x{_CompressionMethod:X4})";
				}
			}
		}

		internal void ResetDirEntry()
		{
			__FileDataPosition = -1L;
			_LengthOfHeader = 0;
		}

		internal static ZipEntry ReadDirEntry(ZipFile zf, Dictionary<string, object> previouslySeen)
		{
			Stream readStream = zf.ReadStream;
			Encoding encoding = zf.AlternateEncodingUsage == ZipOption.Always ? zf.AlternateEncoding : ZipFile.DefaultEncoding;
			int num = SharedUtilities.ReadSignature(readStream);
			if (IsNotValidZipDirEntrySig(num))
			{
				readStream.Seek(-4L, SeekOrigin.Current);
				if ((long) num != 101010256 && (long) num != 101075792 && num != 67324752)
					throw new BadReadException($"  Bad signature (0x{num:X8}) at position 0x{readStream.Position:X8}");
				return null;
			}

			int num2 = 46;
			byte[] array = new byte[42];
			int num3 = readStream.Read(array, 0, array.Length);
			if (num3 != array.Length) return null;
			int num4 = 0;
			ZipEntry zipEntry = new ZipEntry();
			zipEntry.AlternateEncoding = encoding;
			zipEntry._Source = ZipEntrySource.ZipFile;
			zipEntry._container = new ZipContainer(zf);
			zipEntry._VersionMadeBy = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._VersionNeeded = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._BitField = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._CompressionMethod = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._TimeBlob =
				array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 + array[num4++] * 256 * 256 * 256;
			zipEntry._LastModified = SharedUtilities.PackedToDateTime(zipEntry._TimeBlob);
			zipEntry.Timestamp |= ZipEntryTimestamp.DOS;
			zipEntry._Crc32 = array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 + array[num4++] * 256 * 256 * 256;
			zipEntry._CompressedSize = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
			                                   array[num4++] * 256 * 256 * 256);
			zipEntry._UncompressedSize = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
			                                     array[num4++] * 256 * 256 * 256);
			zipEntry._CompressionMethod_FromZipFile = zipEntry._CompressionMethod;
			zipEntry._filenameLength = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._extraFieldLength = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._commentLength = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._diskNumber = (uint) (array[num4++] + array[num4++] * 256);
			zipEntry._InternalFileAttrs = (short) (array[num4++] + array[num4++] * 256);
			zipEntry._ExternalFileAttrs = array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
			                              array[num4++] * 256 * 256 * 256;
			zipEntry._RelativeOffsetOfLocalHeader = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
			                                                array[num4++] * 256 * 256 * 256);
			zipEntry.IsText = (zipEntry._InternalFileAttrs & 1) == 1;
			array = new byte[zipEntry._filenameLength];
			num3 = readStream.Read(array, 0, array.Length);
			num2 += num3;
			if ((zipEntry._BitField & 0x800) == 2048)
				zipEntry._FileNameInArchive = SharedUtilities.Utf8StringFromBuffer(array);
			else
				zipEntry._FileNameInArchive = SharedUtilities.StringFromBuffer(array, encoding);
			while (previouslySeen.ContainsKey(zipEntry._FileNameInArchive))
			{
				zipEntry._FileNameInArchive = CopyHelper.AppendCopyToFileName(zipEntry._FileNameInArchive);
				zipEntry._metadataChanged = true;
			}

			if (zipEntry.AttributesIndicateDirectory)
				zipEntry.MarkAsDirectory();
			else if (zipEntry._FileNameInArchive.EndsWith("/"))
				zipEntry.MarkAsDirectory();
			zipEntry._CompressedFileDataSize = zipEntry._CompressedSize;
			if ((zipEntry._BitField & 1) == 1)
			{
				zipEntry._Encryption_FromZipFile = zipEntry._Encryption = EncryptionAlgorithm.PkzipWeak;
				zipEntry._sourceIsEncrypted = true;
			}

			if (zipEntry._extraFieldLength > 0)
			{
				zipEntry._InputUsesZip64 = zipEntry._CompressedSize == 4294967295u || zipEntry._UncompressedSize == 4294967295u ||
				                           zipEntry._RelativeOffsetOfLocalHeader == 4294967295u;
				num2 += zipEntry.ProcessExtraField(readStream, zipEntry._extraFieldLength);
				zipEntry._CompressedFileDataSize = zipEntry._CompressedSize;
			}

			if (zipEntry._Encryption == EncryptionAlgorithm.PkzipWeak) zipEntry._CompressedFileDataSize -= 12L;
			if ((zipEntry._BitField & 8) == 8)
				if (zipEntry._InputUsesZip64)
					zipEntry._LengthOfTrailer += 24;
				else
					zipEntry._LengthOfTrailer += 16;
			zipEntry.AlternateEncoding = (zipEntry._BitField & 0x800) == 2048 ? Encoding.UTF8 : encoding;
			zipEntry.AlternateEncodingUsage = ZipOption.Always;
			if (zipEntry._commentLength > 0)
			{
				array = new byte[zipEntry._commentLength];
				num3 = readStream.Read(array, 0, array.Length);
				num2 += num3;
				if ((zipEntry._BitField & 0x800) == 2048)
					zipEntry._Comment = SharedUtilities.Utf8StringFromBuffer(array);
				else
					zipEntry._Comment = SharedUtilities.StringFromBuffer(array, encoding);
			}

			return zipEntry;
		}

		internal static bool IsNotValidZipDirEntrySig(int signature)
		{
			return signature != 33639248;
		}

		public void SetEntryTimes(DateTime created, DateTime accessed, DateTime modified)
		{
			_ntfsTimesAreSet = true;
			if (created == _zeroHour && created.Kind == _zeroHour.Kind) created = _win32Epoch;
			if (accessed == _zeroHour && accessed.Kind == _zeroHour.Kind) accessed = _win32Epoch;
			if (modified == _zeroHour && modified.Kind == _zeroHour.Kind) modified = _win32Epoch;
			_Ctime = created.ToUniversalTime();
			_Atime = accessed.ToUniversalTime();
			_Mtime = modified.ToUniversalTime();
			_LastModified = _Mtime;
			if (!_emitUnixTimes && !_emitNtfsTimes) _emitNtfsTimes = true;
			_metadataChanged = true;
		}

		internal static string NameInArchive(string filename, string directoryPathInArchive)
		{
			string text = null;
			text = directoryPathInArchive != null
				? (!string.IsNullOrEmpty(directoryPathInArchive)
					? Path.Combine(directoryPathInArchive, Path.GetFileName(filename))
					: Path.GetFileName(filename))
				: filename;
			return SharedUtilities.NormalizePathForUseInZipFile(text);
		}

		internal static ZipEntry CreateFromNothing(string nameInArchive)
		{
			return Create(nameInArchive, ZipEntrySource.None, null, null);
		}

		internal static ZipEntry CreateFromFile(string filename, string nameInArchive)
		{
			return Create(nameInArchive, ZipEntrySource.FileSystem, filename, null);
		}

		internal static ZipEntry CreateForStream(string entryName, Stream s)
		{
			return Create(entryName, ZipEntrySource.Stream, s, null);
		}

		internal static ZipEntry CreateForWriter(string entryName, WriteDelegate d)
		{
			return Create(entryName, ZipEntrySource.WriteDelegate, d, null);
		}

		internal static ZipEntry CreateForJitStreamProvider(string nameInArchive, OpenDelegate opener, CloseDelegate closer)
		{
			return Create(nameInArchive, ZipEntrySource.JitStream, opener, closer);
		}

		internal static ZipEntry CreateForZipOutputStream(string nameInArchive)
		{
			return Create(nameInArchive, ZipEntrySource.ZipOutputStream, null, null);
		}

		private static ZipEntry Create(string nameInArchive, ZipEntrySource source, object arg1, object arg2)
		{
			if (string.IsNullOrEmpty(nameInArchive)) throw new ZipException("The entry name must be non-null and non-empty.");
			ZipEntry zipEntry = new ZipEntry();
			zipEntry._VersionMadeBy = 45;
			zipEntry._Source = source;
			zipEntry._Mtime = zipEntry._Atime = zipEntry._Ctime = DateTime.UtcNow;
			switch (source)
			{
				case ZipEntrySource.Stream:
					zipEntry._sourceStream = arg1 as Stream;
					break;
				case ZipEntrySource.WriteDelegate:
					zipEntry._WriteDelegate = arg1 as WriteDelegate;
					break;
				case ZipEntrySource.JitStream:
					zipEntry._OpenDelegate = arg1 as OpenDelegate;
					zipEntry._CloseDelegate = arg2 as CloseDelegate;
					break;
				case ZipEntrySource.None:
					zipEntry._Source = ZipEntrySource.FileSystem;
					break;
				default:
				{
					string text = arg1 as string;
					if (string.IsNullOrEmpty(text)) throw new ZipException("The filename must be non-null and non-empty.");
					try
					{
						zipEntry._Mtime = File.GetLastWriteTime(text).ToUniversalTime();
						zipEntry._Ctime = File.GetCreationTime(text).ToUniversalTime();
						zipEntry._Atime = File.GetLastAccessTime(text).ToUniversalTime();
						if (File.Exists(text) || Directory.Exists(text)) zipEntry._ExternalFileAttrs = (int) File.GetAttributes(text);
						zipEntry._ntfsTimesAreSet = true;
						zipEntry._LocalFileName = Path.GetFullPath(text);
					}
					catch (PathTooLongException innerException)
					{
						string message = $"The path is too long, filename={text}";
						throw new ZipException(message, innerException);
					}

					break;
				}
				case ZipEntrySource.ZipOutputStream:
					break;
			}

			zipEntry._LastModified = zipEntry._Mtime;
			zipEntry._FileNameInArchive = SharedUtilities.NormalizePathForUseInZipFile(nameInArchive);
			return zipEntry;
		}

		internal void MarkAsDirectory()
		{
			IsDirectory = true;
			if (!_FileNameInArchive.EndsWith("/")) _FileNameInArchive += "/";
		}

		public override string ToString()
		{
			return $"ZipEntry::{FileName}";
		}

		private void SetFdpLoh()
		{
			long position = ArchiveStream.Position;
			try
			{
				ArchiveStream.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			}
			catch (IOException innerException)
			{
				string message =
					$"Exception seeking  entry({FileName}) offset(0x{_RelativeOffsetOfLocalHeader:X8}) len(0x{ArchiveStream.Length:X8})";
				throw new BadStateException(message, innerException);
			}

			byte[] array = new byte[30];
			ArchiveStream.Read(array, 0, array.Length);
			short num = (short) (array[26] + array[27] * 256);
			short num2 = (short) (array[28] + array[29] * 256);
			ArchiveStream.Seek(num + num2, SeekOrigin.Current);
			_LengthOfHeader = 30 + num2 + num + GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);
			__FileDataPosition = _RelativeOffsetOfLocalHeader + _LengthOfHeader;
			ArchiveStream.Seek(position, SeekOrigin.Begin);
		}

		internal static int GetLengthOfCryptoHeaderBytes(EncryptionAlgorithm a)
		{
			switch (a)
			{
				case EncryptionAlgorithm.None:
					return 0;
				case EncryptionAlgorithm.PkzipWeak:
					return 12;
				default:
					throw new ZipException("internal error");
			}
		}

		public void Extract()
		{
			InternalExtract(".", null, null);
		}

		public void Extract(ExtractExistingFileAction extractExistingFile)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(".", null, null);
		}

		public void Extract(Stream stream)
		{
			InternalExtract(null, stream, null);
		}

		public void Extract(string baseDirectory)
		{
			InternalExtract(baseDirectory, null, null);
		}

		public void Extract(string baseDirectory, ExtractExistingFileAction extractExistingFile)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(baseDirectory, null, null);
		}

		public void ExtractWithPassword(string password)
		{
			InternalExtract(".", null, password);
		}

		public void ExtractWithPassword(string baseDirectory, string password)
		{
			InternalExtract(baseDirectory, null, password);
		}

		public void ExtractWithPassword(ExtractExistingFileAction extractExistingFile, string password)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(".", null, password);
		}

		public void ExtractWithPassword(string baseDirectory, ExtractExistingFileAction extractExistingFile, string password)
		{
			ExtractExistingFile = extractExistingFile;
			InternalExtract(baseDirectory, null, password);
		}

		public void ExtractWithPassword(Stream stream, string password)
		{
			InternalExtract(null, stream, password);
		}

		public CrcCalculatorStream OpenReader()
		{
			if (_container.ZipFile == null) throw new InvalidOperationException("Use OpenReader() only with ZipFile.");
			return InternalOpenReader(_Password ?? _container.Password);
		}

		public CrcCalculatorStream OpenReader(string password)
		{
			if (_container.ZipFile == null) throw new InvalidOperationException("Use OpenReader() only with ZipFile.");
			return InternalOpenReader(password);
		}

		internal CrcCalculatorStream InternalOpenReader(string password)
		{
			ValidateCompression();
			ValidateEncryption();
			SetupCryptoForExtract(password);
			if (_Source != ZipEntrySource.ZipFile)
				throw new BadStateException("You must call ZipFile.Save before calling OpenReader");
			long length = _CompressionMethod_FromZipFile == 0 ? _CompressedFileDataSize : UncompressedSize;
			Stream archiveStream = ArchiveStream;
			ArchiveStream.Seek(FileDataPosition, SeekOrigin.Begin);
			_inputDecryptorStream = GetExtractDecryptor(archiveStream);
			Stream extractDecompressor = GetExtractDecompressor(_inputDecryptorStream);
			return new CrcCalculatorStream(extractDecompressor, length);
		}

		private void OnExtractProgress(long bytesWritten, long totalBytesToWrite)
		{
			if (_container.ZipFile != null)
				_ioOperationCanceled = _container.ZipFile.OnExtractBlock(this, bytesWritten, totalBytesToWrite);
		}

		private void OnBeforeExtract(string path)
		{
			if (_container.ZipFile != null && !_container.ZipFile._inExtractAll)
				_ioOperationCanceled = _container.ZipFile.OnSingleEntryExtract(this, path, true);
		}

		private void OnAfterExtract(string path)
		{
			if (_container.ZipFile != null && !_container.ZipFile._inExtractAll)
				_container.ZipFile.OnSingleEntryExtract(this, path, false);
		}

		private void OnExtractExisting(string path)
		{
			if (_container.ZipFile != null) _ioOperationCanceled = _container.ZipFile.OnExtractExisting(this, path);
		}

		private static void ReallyDelete(string fileName)
		{
			if ((File.GetAttributes(fileName) & FileAttributes.ReadOnly) == FileAttributes.ReadOnly)
				File.SetAttributes(fileName, FileAttributes.Normal);
			File.Delete(fileName);
		}

		private void WriteStatus(string format, params object[] args)
		{
			if (_container.ZipFile != null && _container.ZipFile.Verbose)
				_container.ZipFile.StatusMessageTextWriter.WriteLine(format, args);
		}

		private void InternalExtract(string baseDir, Stream outstream, string password)
		{
			if (_container == null) throw new BadStateException("This entry is an orphan");
			if (_container.ZipFile == null) throw new InvalidOperationException("Use Extract() only with ZipFile.");
			_container.ZipFile.Reset(false);
			if (_Source != ZipEntrySource.ZipFile)
				throw new BadStateException("You must call ZipFile.Save before calling any Extract method");
			OnBeforeExtract(baseDir);
			_ioOperationCanceled = false;
			string text = null;
			Stream stream = null;
			bool flag = false;
			bool flag2 = false;
			try
			{
				ValidateCompression();
				ValidateEncryption();
				if (ValidateOutput(baseDir, outstream, out text))
				{
					WriteStatus("extract dir {0}...", text);
					OnAfterExtract(baseDir);
					goto end_IL_0066;
				}

				if (text != null && File.Exists(text))
				{
					flag = true;
					int num = CheckExtractExistingFile(baseDir, text);
					if (num != 2 && num != 1) goto IL_00cd;
					goto end_IL_0066;
				}

				IL_00cd:
				string text2 = password ?? _Password ?? _container.Password;
				if (_Encryption_FromZipFile != 0)
				{
					if (text2 == null) throw new BadPasswordException();
					SetupCryptoForExtract(text2);
				}

				if (text != null)
				{
					WriteStatus("extract file {0}...", text);
					text += ".tmp";
					string directoryName = Path.GetDirectoryName(text);
					if (!Directory.Exists(directoryName))
						Directory.CreateDirectory(directoryName);
					else if (_container.ZipFile != null)
						flag2 = _container.ZipFile._inExtractAll;
					if (File.Exists(text)) File.Delete(text);
					stream = new FileStream(text, FileMode.CreateNew);
				}
				else
				{
					WriteStatus("extract entry {0} to stream...", FileName);
					stream = outstream;
				}

				if (!_ioOperationCanceled)
				{
					int actualCrc = ExtractOne(stream);
					if (!_ioOperationCanceled)
					{
						VerifyCrcAfterExtract(actualCrc);
						if (text != null)
						{
							stream.Close();
							stream = null;
							string text3 = text;
							string text4 = null;
							text = text3.Substring(0, text3.Length - 4);
							if (flag)
							{
								text4 = text + ".PendingOverwrite";
								if (File.Exists(text4)) File.Delete(text4);
								File.Move(text, text4);
							}

							File.Move(text3, text);
							_SetTimes(text, true);
							if (text4 != null && File.Exists(text4)) ReallyDelete(text4);
							if (flag2 && FileName.IndexOf('/') != -1)
							{
								string directoryName2 = Path.GetDirectoryName(FileName);
								if (_container.ZipFile[directoryName2] == null) _SetTimes(Path.GetDirectoryName(text), false);
							}

							if ((_VersionMadeBy & 0xFF00) == 2560 || (_VersionMadeBy & 0xFF00) == 0)
								File.SetAttributes(text, (FileAttributes) _ExternalFileAttrs);
						}

						OnAfterExtract(baseDir);
					}
				}

				end_IL_0066:;
			}
			catch (Exception)
			{
				_ioOperationCanceled = true;
				throw;
			}
			finally
			{
				if (_ioOperationCanceled && text != null)
				{
					if (stream != null) stream.Close();
					if (File.Exists(text) && !flag) File.Delete(text);
				}
			}
		}

		internal void VerifyCrcAfterExtract(int actualCrc32)
		{
			if (actualCrc32 == _Crc32) return;
			throw new BadCrcException("CRC error: the file being extracted appears to be corrupted. " +
			                          $"Expected 0x{_Crc32:X8}, Actual 0x{actualCrc32:X8}");
		}

		private int CheckExtractExistingFile(string baseDir, string targetFileName)
		{
			int num = 0;
			while (true)
			{
				switch (ExtractExistingFile)
				{
					case ExtractExistingFileAction.OverwriteSilently:
						WriteStatus("the file {0} exists; will overwrite it...", targetFileName);
						return 0;
					case ExtractExistingFileAction.DoNotOverwrite:
						WriteStatus("the file {0} exists; not extracting entry...", FileName);
						OnAfterExtract(baseDir);
						return 1;
					case ExtractExistingFileAction.InvokeExtractProgressEvent:
						break;
					default:
						throw new ZipException($"The file {targetFileName} already exists.");
				}

				if (num > 0) throw new ZipException($"The file {targetFileName} already exists.");
				OnExtractExisting(baseDir);
				if (!_ioOperationCanceled)
				{
					num++;
					continue;
				}

				break;
			}

			return 2;
		}

		private void _CheckRead(int nbytes)
		{
			if (nbytes != 0) return;
			throw new BadReadException($"bad read of entry {FileName} from compressed archive.");
		}

		private int ExtractOne(Stream output)
		{
			int num = 0;
			Stream archiveStream = ArchiveStream;
			try
			{
				archiveStream.Seek(FileDataPosition, SeekOrigin.Begin);
				byte[] array = new byte[BufferSize];
				long num2 = _CompressionMethod_FromZipFile != 0 ? UncompressedSize : _CompressedFileDataSize;
				_inputDecryptorStream = GetExtractDecryptor(archiveStream);
				Stream extractDecompressor = GetExtractDecompressor(_inputDecryptorStream);
				long num3 = 0L;
				using (CrcCalculatorStream crcCalculatorStream = new CrcCalculatorStream(extractDecompressor))
				{
					while (num2 > 0)
					{
						int count = (int) (num2 > array.Length ? array.Length : num2);
						int num4 = crcCalculatorStream.Read(array, 0, count);
						_CheckRead(num4);
						output.Write(array, 0, num4);
						num2 -= num4;
						num3 += num4;
						OnExtractProgress(num3, UncompressedSize);
						if (_ioOperationCanceled) break;
					}

					return crcCalculatorStream.Crc;
				}
			}
			finally
			{
				ZipSegmentedStream zipSegmentedStream = archiveStream as ZipSegmentedStream;
				if (zipSegmentedStream != null)
				{
					zipSegmentedStream.Dispose();
					_archiveStream = null;
				}
			}
		}

		internal Stream GetExtractDecompressor(Stream input2)
		{
			switch (_CompressionMethod_FromZipFile)
			{
				case 0:
					return input2;
				case 8:
					return new DeflateStream(input2, CompressionMode.Decompress, true);
				default:
					return null;
			}
		}

		internal Stream GetExtractDecryptor(Stream input)
		{
			Stream stream = null;
			if (_Encryption_FromZipFile == EncryptionAlgorithm.PkzipWeak)
				return new ZipCipherStream(input, _zipCrypto_forExtract, CryptoMode.Decrypt);
			return input;
		}

		internal void _SetTimes(string fileOrDirectory, bool isFile)
		{
			try
			{
				if (_ntfsTimesAreSet)
				{
					if (isFile)
					{
						if (File.Exists(fileOrDirectory))
						{
							File.SetCreationTimeUtc(fileOrDirectory, _Ctime);
							File.SetLastAccessTimeUtc(fileOrDirectory, _Atime);
							File.SetLastWriteTimeUtc(fileOrDirectory, _Mtime);
						}
					}
					else if (Directory.Exists(fileOrDirectory))
					{
						Directory.SetCreationTimeUtc(fileOrDirectory, _Ctime);
						Directory.SetLastAccessTimeUtc(fileOrDirectory, _Atime);
						Directory.SetLastWriteTimeUtc(fileOrDirectory, _Mtime);
					}
				}
				else
				{
					DateTime lastWriteTime = SharedUtilities.AdjustTime_Reverse(LastModified);
					if (isFile)
						File.SetLastWriteTime(fileOrDirectory, lastWriteTime);
					else
						Directory.SetLastWriteTime(fileOrDirectory, lastWriteTime);
				}
			}
			catch (IOException ex)
			{
				WriteStatus("failed to set time on {0}: {1}", fileOrDirectory, ex.Message);
			}
		}

		internal void ValidateEncryption()
		{
			if (Encryption == EncryptionAlgorithm.PkzipWeak) return;
			if (Encryption == EncryptionAlgorithm.None) return;
			if (_UnsupportedAlgorithmId != 0)
				throw new ZipException(
					$"Cannot extract: Entry {FileName} is encrypted with an algorithm not supported by DotNetZip: {UnsupportedAlgorithm}");
			throw new ZipException(
				$"Cannot extract: Entry {FileName} uses an unsupported encryption algorithm ({(int) Encryption:X2})");
		}

		private void ValidateCompression()
		{
			if (_CompressionMethod_FromZipFile == 0) return;
			if (_CompressionMethod_FromZipFile == 8) return;
			throw new ZipException(
				$"Entry {FileName} uses an unsupported compression method (0x{_CompressionMethod_FromZipFile:X2}, {UnsupportedCompressionMethod})");
		}

		private void SetupCryptoForExtract(string password)
		{
			if (_Encryption_FromZipFile != 0 && _Encryption_FromZipFile == EncryptionAlgorithm.PkzipWeak)
			{
				if (password == null) throw new ZipException("Missing password.");
				ArchiveStream.Seek(FileDataPosition - 12, SeekOrigin.Begin);
				_zipCrypto_forExtract = ZipCrypto.ForRead(password, this);
			}
		}

		private bool ValidateOutput(string basedir, Stream outstream, out string outFileName)
		{
			if (basedir != null)
			{
				string text = FileName.Replace("\\", "/");
				if (text.IndexOf(':') == 1) text = text.Substring(2);
				if (text.StartsWith("/")) text = text.Substring(1);
				if (_container.ZipFile.FlattenFoldersOnExtract)
					outFileName = Path.Combine(basedir, text.IndexOf('/') != -1 ? Path.GetFileName(text) : text);
				else
					outFileName = Path.Combine(basedir, text);
				outFileName = outFileName.Replace("/", "\\");
				if (!IsDirectory && !FileName.EndsWith("/")) return false;
				if (!Directory.Exists(outFileName))
				{
					Directory.CreateDirectory(outFileName);
					_SetTimes(outFileName, false);
				}
				else if (ExtractExistingFile == ExtractExistingFileAction.OverwriteSilently)
				{
					_SetTimes(outFileName, false);
				}

				return true;
			}

			if (outstream != null)
			{
				outFileName = null;
				if (!IsDirectory && !FileName.EndsWith("/")) return false;
				return true;
			}

			throw new ArgumentNullException(nameof(outstream));
		}

		private void ReadExtraField()
		{
			_readExtraDepth++;
			long position = ArchiveStream.Position;
			ArchiveStream.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			byte[] array = new byte[30];
			ArchiveStream.Read(array, 0, array.Length);
			int num = 26;
			short num4 = (short) (array[num++] + array[num++] * 256);
			short extraFieldLength = (short) (array[num++] + array[num++] * 256);
			ArchiveStream.Seek(num4, SeekOrigin.Current);
			ProcessExtraField(ArchiveStream, extraFieldLength);
			ArchiveStream.Seek(position, SeekOrigin.Begin);
			_readExtraDepth--;
		}

		private static bool ReadHeader(ZipEntry ze, Encoding defaultEncoding)
		{
			int num = 0;
			ze._RelativeOffsetOfLocalHeader = ze.ArchiveStream.Position;
			int num2 = SharedUtilities.ReadEntrySignature(ze.ArchiveStream);
			num += 4;
			if (IsNotValidSig(num2))
			{
				ze.ArchiveStream.Seek(-4L, SeekOrigin.Current);
				if (IsNotValidZipDirEntrySig(num2) && (long) num2 != 101010256)
					throw new BadReadException($"  Bad signature (0x{num2:X8}) at position  0x{ze.ArchiveStream.Position:X8}");
				return false;
			}

			byte[] array = new byte[26];
			int num3 = ze.ArchiveStream.Read(array, 0, array.Length);
			if (num3 != array.Length) return false;
			num += num3;
			int num4 = 0;
			ze._VersionNeeded = (short) (array[num4++] + array[num4++] * 256);
			ze._BitField = (short) (array[num4++] + array[num4++] * 256);
			ze._CompressionMethod_FromZipFile = ze._CompressionMethod = (short) (array[num4++] + array[num4++] * 256);
			ze._TimeBlob = array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 + array[num4++] * 256 * 256 * 256;
			ze._LastModified = SharedUtilities.PackedToDateTime(ze._TimeBlob);
			ze.Timestamp |= ZipEntryTimestamp.DOS;
			if ((ze._BitField & 1) == 1)
			{
				ze._Encryption_FromZipFile = ze._Encryption = EncryptionAlgorithm.PkzipWeak;
				ze._sourceIsEncrypted = true;
			}

			ze._Crc32 = array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 + array[num4++] * 256 * 256 * 256;
			ze._CompressedSize = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
			                             array[num4++] * 256 * 256 * 256);
			ze._UncompressedSize = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
			                               array[num4++] * 256 * 256 * 256);
			if ((int) ze._CompressedSize == -1 || (int) ze._UncompressedSize == -1) ze._InputUsesZip64 = true;
			short num29 = (short) (array[num4++] + array[num4++] * 256);
			short extraFieldLength = (short) (array[num4++] + array[num4++] * 256);
			array = new byte[num29];
			num3 = ze.ArchiveStream.Read(array, 0, array.Length);
			num += num3;
			if ((ze._BitField & 0x800) == 2048)
			{
				ze.AlternateEncoding = Encoding.UTF8;
				ze.AlternateEncodingUsage = ZipOption.Always;
			}

			ze._FileNameInArchive = ze.AlternateEncoding.GetString(array, 0, array.Length);
			if (ze._FileNameInArchive.EndsWith("/")) ze.MarkAsDirectory();
			num += ze.ProcessExtraField(ze.ArchiveStream, extraFieldLength);
			ze._LengthOfTrailer = 0;
			if (!ze._FileNameInArchive.EndsWith("/") && (ze._BitField & 8) == 8)
			{
				long position = ze.ArchiveStream.Position;
				bool flag = true;
				long num32 = 0L;
				int num33 = 0;
				while (flag)
				{
					num33++;
					if (ze._container.ZipFile != null) ze._container.ZipFile.OnReadBytes(ze);
					long num34 = SharedUtilities.FindSignature(ze.ArchiveStream, 134695760);
					if (num34 == -1) return false;
					num32 += num34;
					if (ze._InputUsesZip64)
					{
						array = new byte[20];
						num3 = ze.ArchiveStream.Read(array, 0, array.Length);
						if (num3 != 20) return false;
						num4 = 0;
						ze._Crc32 = array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 + array[num4++] * 256 * 256 * 256;
						ze._CompressedSize = BitConverter.ToInt64(array, num4);
						num4 += 8;
						ze._UncompressedSize = BitConverter.ToInt64(array, num4);
						num4 += 8;
						ze._LengthOfTrailer += 24;
					}
					else
					{
						array = new byte[12];
						num3 = ze.ArchiveStream.Read(array, 0, array.Length);
						if (num3 != 12) return false;
						num4 = 0;
						ze._Crc32 = array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 + array[num4++] * 256 * 256 * 256;
						ze._CompressedSize = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
						                             array[num4++] * 256 * 256 * 256);
						ze._UncompressedSize = (uint) (array[num4++] + array[num4++] * 256 + array[num4++] * 256 * 256 +
						                               array[num4++] * 256 * 256 * 256);
						ze._LengthOfTrailer += 16;
					}

					flag = num32 != ze._CompressedSize;
					if (flag)
					{
						ze.ArchiveStream.Seek(-12L, SeekOrigin.Current);
						num32 += 4;
					}
				}

				ze.ArchiveStream.Seek(position, SeekOrigin.Begin);
			}

			ze._CompressedFileDataSize = ze._CompressedSize;
			if ((ze._BitField & 1) == 1)
			{
				ze._WeakEncryptionHeader = new byte[12];
				num += ReadWeakEncryptionHeader(ze._archiveStream, ze._WeakEncryptionHeader);
				ze._CompressedFileDataSize -= 12L;
			}

			ze._LengthOfHeader = num;
			ze._TotalEntrySize = ze._LengthOfHeader + ze._CompressedFileDataSize + ze._LengthOfTrailer;
			return true;
		}

		internal static int ReadWeakEncryptionHeader(Stream s, byte[] buffer)
		{
			int num = s.Read(buffer, 0, 12);
			if (num != 12) throw new ZipException($"Unexpected end of data at position 0x{s.Position:X8}");
			return num;
		}

		private static bool IsNotValidSig(int signature)
		{
			return signature != 67324752;
		}

		internal static ZipEntry ReadEntry(ZipContainer zc, bool first)
		{
			ZipFile zipFile = zc.ZipFile;
			Stream readStream = zc.ReadStream;
			Encoding alternateEncoding = zc.AlternateEncoding;
			ZipEntry zipEntry = new ZipEntry();
			zipEntry._Source = ZipEntrySource.ZipFile;
			zipEntry._container = zc;
			zipEntry._archiveStream = readStream;
			if (zipFile != null) zipFile.OnReadEntry(true, null);
			if (first) HandlePK00Prefix(readStream);
			if (!ReadHeader(zipEntry, alternateEncoding)) return null;
			zipEntry.__FileDataPosition = zipEntry.ArchiveStream.Position;
			readStream.Seek(zipEntry._CompressedFileDataSize + zipEntry._LengthOfTrailer, SeekOrigin.Current);
			HandleUnexpectedDataDescriptor(zipEntry);
			if (zipFile != null)
			{
				zipFile.OnReadBytes(zipEntry);
				zipFile.OnReadEntry(false, zipEntry);
			}

			return zipEntry;
		}

		internal static void HandlePK00Prefix(Stream s)
		{
			uint num = (uint) SharedUtilities.ReadInt(s);
			if (num != 808471376) s.Seek(-4L, SeekOrigin.Current);
		}

		private static void HandleUnexpectedDataDescriptor(ZipEntry entry)
		{
			Stream archiveStream = entry.ArchiveStream;
			uint num = (uint) SharedUtilities.ReadInt(archiveStream);
			if (num == entry._Crc32)
			{
				int num2 = SharedUtilities.ReadInt(archiveStream);
				if (num2 == entry._CompressedSize)
				{
					num2 = SharedUtilities.ReadInt(archiveStream);
					if (num2 != entry._UncompressedSize) archiveStream.Seek(-12L, SeekOrigin.Current);
				}
				else
				{
					archiveStream.Seek(-8L, SeekOrigin.Current);
				}
			}
			else
			{
				archiveStream.Seek(-4L, SeekOrigin.Current);
			}
		}

		internal static int FindExtraFieldSegment(byte[] extra, int offx, ushort targetHeaderId)
		{
			short num6;
			for (int i = offx; i + 3 < extra.Length; i += num6)
			{
				ushort num3 = (ushort) (extra[i++] + extra[i++] * 256);
				if (num3 == targetHeaderId) return i - 2;
				num6 = (short) (extra[i++] + extra[i++] * 256);
			}

			return -1;
		}

		internal int ProcessExtraField(Stream s, short extraFieldLength)
		{
			int num = 0;
			if (extraFieldLength > 0)
			{
				byte[] array = _Extra = new byte[extraFieldLength];
				num = s.Read(array, 0, array.Length);
				long posn = s.Position - num;
				int num2 = 0;
				while (num2 + 3 < array.Length)
				{
					int num3 = num2;
					ushort num6 = (ushort) (array[num2++] + array[num2++] * 256);
					short num9 = (short) (array[num2++] + array[num2++] * 256);
					switch (num6)
					{
						case 10:
							num2 = ProcessExtraFieldWindowsTimes(array, num2, num9, posn);
							break;
						case 21589:
							num2 = ProcessExtraFieldUnixTimes(array, num2, num9, posn);
							break;
						case 22613:
							num2 = ProcessExtraFieldInfoZipTimes(array, num2, num9, posn);
							break;
						case 1:
							num2 = ProcessExtraFieldZip64(array, num2, num9, posn);
							break;
						case 23:
							num2 = ProcessExtraFieldPkwareStrongEncryption(array, num2);
							break;
					}

					num2 = num3 + num9 + 4;
				}
			}

			return num;
		}

		private int ProcessExtraFieldPkwareStrongEncryption(byte[] Buffer, int j)
		{
			j += 2;
			_UnsupportedAlgorithmId = (ushort) (Buffer[j++] + Buffer[j++] * 256);
			_Encryption_FromZipFile = _Encryption = EncryptionAlgorithm.Unsupported;
			return j;
		}

		private int ProcessExtraFieldZip64(byte[] buffer, int j, short dataSize, long posn)
		{
			_InputUsesZip64 = true;
			if (dataSize > 28)
				throw new BadReadException(
					$"  Inconsistent size (0x{dataSize:X4}) for ZIP64 extra field at position 0x{posn:X16}");
			Func<long> func = delegate
			{
				if (dataSize < 8)
					throw new BadReadException($"  Missing data for ZIP64 extra field, position 0x{posn:X16}");
				long result = BitConverter.ToInt64(buffer, j);
				j += 8;
				dataSize = (short) (dataSize - 8);
				return result;
			};
			if (_UncompressedSize == 4294967295u) _UncompressedSize = func();
			if (_CompressedSize == 4294967295u) _CompressedSize = func();
			if (_RelativeOffsetOfLocalHeader == 4294967295u) _RelativeOffsetOfLocalHeader = func();
			return j;
		}

		private int ProcessExtraFieldInfoZipTimes(byte[] buffer, int j, short dataSize, long posn)
		{
			if (dataSize != 12 && dataSize != 8)
				throw new BadReadException(
					$"  Unexpected size (0x{dataSize:X4}) for InfoZip v1 extra field at position 0x{posn:X16}");
			int num = BitConverter.ToInt32(buffer, j);
			_Mtime = _unixEpoch.AddSeconds(num);
			j += 4;
			num = BitConverter.ToInt32(buffer, j);
			_Atime = _unixEpoch.AddSeconds(num);
			j += 4;
			_Ctime = DateTime.UtcNow;
			_ntfsTimesAreSet = true;
			Timestamp |= ZipEntryTimestamp.InfoZip1;
			return j;
		}

		private int ProcessExtraFieldUnixTimes(byte[] buffer, int j, short dataSize, long posn)
		{
			if (dataSize != 13 && dataSize != 9 && dataSize != 5)
				throw new BadReadException(
					$"  Unexpected size (0x{dataSize:X4}) for Extended Timestamp extra field at position 0x{posn:X16}");
			Func<DateTime> func = delegate
			{
				int num = BitConverter.ToInt32(buffer, j);
				j += 4;
				dataSize = (short) (dataSize - 4);
				return _unixEpoch.AddSeconds(num);
			};
			if (dataSize == 13 || _readExtraDepth > 0)
			{
				byte b = buffer[++j];
				dataSize = (short) (dataSize - 1);
				if ((b & 1) != 0 && dataSize >= 4) _Mtime = func();
				_Atime = (b & 2) != 0 && dataSize >= 4 ? func() : DateTime.UtcNow;
				_Ctime = (b & 4) != 0 && dataSize >= 4 ? func() : DateTime.UtcNow;
				Timestamp |= ZipEntryTimestamp.Unix;
				_ntfsTimesAreSet = true;
				_emitUnixTimes = true;
			}
			else
			{
				ReadExtraField();
			}

			return j;
		}

		private int ProcessExtraFieldWindowsTimes(byte[] buffer, int j, short dataSize, long posn)
		{
			if (dataSize != 32)
				throw new BadReadException(
					$"  Unexpected size (0x{dataSize:X4}) for NTFS times extra field at position 0x{posn:X16}");
			j += 4;
			short num = (short) (buffer[j] + buffer[j + 1] * 256);
			short num2 = (short) (buffer[j + 2] + buffer[j + 3] * 256);
			j += 4;
			if (num == 1 && num2 == 24)
			{
				long fileTime = BitConverter.ToInt64(buffer, j);
				_Mtime = DateTime.FromFileTimeUtc(fileTime);
				j += 8;
				fileTime = BitConverter.ToInt64(buffer, j);
				_Atime = DateTime.FromFileTimeUtc(fileTime);
				j += 8;
				fileTime = BitConverter.ToInt64(buffer, j);
				_Ctime = DateTime.FromFileTimeUtc(fileTime);
				j += 8;
				_ntfsTimesAreSet = true;
				Timestamp |= ZipEntryTimestamp.Windows;
				_emitNtfsTimes = true;
			}

			return j;
		}

		internal void WriteCentralDirectoryEntry(Stream s)
		{
			byte[] array = new byte[4096];
			int num = 0;
			array[num++] = 80;
			array[num++] = 75;
			array[num++] = 1;
			array[num++] = 2;
			array[num++] = (byte) (_VersionMadeBy & 0xFF);
			array[num++] = (byte) ((_VersionMadeBy & 0xFF00) >> 8);
			short num8 = (short) (VersionNeeded != 0 ? VersionNeeded : 20);
			if (!OutputUsedZip64.HasValue) OutputUsedZip64 = _container.Zip64 == Zip64Option.Always;
			short num9 = (short) (OutputUsedZip64.Value ? 45 : num8);
			array[num++] = (byte) (num9 & 0xFF);
			array[num++] = (byte) ((num9 & 0xFF00) >> 8);
			array[num++] = (byte) (_BitField & 0xFF);
			array[num++] = (byte) ((_BitField & 0xFF00) >> 8);
			array[num++] = (byte) (_CompressionMethod & 0xFF);
			array[num++] = (byte) ((_CompressionMethod & 0xFF00) >> 8);
			array[num++] = (byte) (_TimeBlob & 0xFF);
			array[num++] = (byte) ((_TimeBlob & 0xFF00) >> 8);
			array[num++] = (byte) ((_TimeBlob & 0xFF0000) >> 16);
			array[num++] = (byte) ((_TimeBlob & 4278190080u) >> 24);
			array[num++] = (byte) (_Crc32 & 0xFF);
			array[num++] = (byte) ((_Crc32 & 0xFF00) >> 8);
			array[num++] = (byte) ((_Crc32 & 0xFF0000) >> 16);
			array[num++] = (byte) ((_Crc32 & 4278190080u) >> 24);
			int num24 = 0;
			if (OutputUsedZip64.Value)
			{
				for (num24 = 0; num24 < 8; num24++) array[num++] = 255;
			}
			else
			{
				array[num++] = (byte) (_CompressedSize & 0xFF);
				array[num++] = (byte) ((_CompressedSize & 0xFF00) >> 8);
				array[num++] = (byte) ((_CompressedSize & 0xFF0000) >> 16);
				array[num++] = (byte) ((_CompressedSize & 4278190080u) >> 24);
				array[num++] = (byte) (_UncompressedSize & 0xFF);
				array[num++] = (byte) ((_UncompressedSize & 0xFF00) >> 8);
				array[num++] = (byte) ((_UncompressedSize & 0xFF0000) >> 16);
				array[num++] = (byte) ((_UncompressedSize & 4278190080u) >> 24);
			}

			byte[] encodedFileNameBytes = GetEncodedFileNameBytes();
			short num34 = (short) encodedFileNameBytes.Length;
			array[num++] = (byte) (num34 & 0xFF);
			array[num++] = (byte) ((num34 & 0xFF00) >> 8);
			_presumeZip64 = OutputUsedZip64.Value;
			_Extra = ConstructExtraField(true);
			short num37 = (short) (_Extra != null ? _Extra.Length : 0);
			array[num++] = (byte) (num37 & 0xFF);
			array[num++] = (byte) ((num37 & 0xFF00) >> 8);
			int num40 = _CommentBytes != null ? _CommentBytes.Length : 0;
			if (num40 + num > array.Length) num40 = array.Length - num;
			array[num++] = (byte) (num40 & 0xFF);
			array[num++] = (byte) ((num40 & 0xFF00) >> 8);
			if (_container.ZipFile != null && _container.ZipFile.MaxOutputSegmentSize != 0)
			{
				array[num++] = (byte) (_diskNumber & 0xFF);
				array[num++] = (byte) ((_diskNumber & 0xFF00) >> 8);
			}
			else
			{
				array[num++] = 0;
				array[num++] = 0;
			}

			array[num++] = (byte) (IsText ? 1 : 0);
			array[num++] = 0;
			array[num++] = (byte) (_ExternalFileAttrs & 0xFF);
			array[num++] = (byte) ((_ExternalFileAttrs & 0xFF00) >> 8);
			array[num++] = (byte) ((_ExternalFileAttrs & 0xFF0000) >> 16);
			array[num++] = (byte) ((_ExternalFileAttrs & 4278190080u) >> 24);
			if (_RelativeOffsetOfLocalHeader > 4294967295u)
			{
				array[num++] = 255;
				array[num++] = 255;
				array[num++] = 255;
				array[num++] = 255;
			}
			else
			{
				array[num++] = (byte) (_RelativeOffsetOfLocalHeader & 0xFF);
				array[num++] = (byte) ((_RelativeOffsetOfLocalHeader & 0xFF00) >> 8);
				array[num++] = (byte) ((_RelativeOffsetOfLocalHeader & 0xFF0000) >> 16);
				array[num++] = (byte) ((_RelativeOffsetOfLocalHeader & 4278190080u) >> 24);
			}

			Buffer.BlockCopy(encodedFileNameBytes, 0, array, num, num34);
			num += num34;
			if (_Extra != null)
			{
				byte[] extra = _Extra;
				int srcOffset = 0;
				Buffer.BlockCopy(extra, srcOffset, array, num, num37);
				num += num37;
			}

			if (num40 != 0)
			{
				Buffer.BlockCopy(_CommentBytes, 0, array, num, num40);
				num += num40;
			}

			s.Write(array, 0, num);
		}

		private byte[] ConstructExtraField(bool forCentralDirectory)
		{
			List<byte[]> list = new List<byte[]>();
			if (_container.Zip64 == Zip64Option.Always ||
			    _container.Zip64 == Zip64Option.AsNecessary && (!forCentralDirectory || RequiresZip64.Value))
			{
				int num = 4 + (forCentralDirectory ? 28 : 16);
				byte[] array = new byte[num];
				int num2 = 0;
				if (_presumeZip64 || forCentralDirectory)
				{
					array[num2++] = 1;
					array[num2++] = 0;
				}
				else
				{
					array[num2++] = 153;
					array[num2++] = 153;
				}

				array[num2++] = (byte) (num - 4);
				array[num2++] = 0;
				Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, array, num2, 8);
				num2 += 8;
				Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, array, num2, 8);
				num2 += 8;
				if (forCentralDirectory)
				{
					Array.Copy(BitConverter.GetBytes(_RelativeOffsetOfLocalHeader), 0, array, num2, 8);
					num2 += 8;
					Array.Copy(BitConverter.GetBytes(0), 0, array, num2, 4);
				}

				list.Add(array);
			}

			if (_ntfsTimesAreSet && _emitNtfsTimes)
			{
				byte[] array = new byte[36];
				int num9 = 0;
				array[num9++] = 10;
				array[num9++] = 0;
				array[num9++] = 32;
				array[num9++] = 0;
				num9 += 4;
				array[num9++] = 1;
				array[num9++] = 0;
				array[num9++] = 24;
				array[num9++] = 0;
				long value = _Mtime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(value), 0, array, num9, 8);
				num9 += 8;
				value = _Atime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(value), 0, array, num9, 8);
				num9 += 8;
				value = _Ctime.ToFileTime();
				Array.Copy(BitConverter.GetBytes(value), 0, array, num9, 8);
				num9 += 8;
				list.Add(array);
			}

			if (_ntfsTimesAreSet && _emitUnixTimes)
			{
				int num18 = 9;
				if (!forCentralDirectory) num18 += 8;
				byte[] array = new byte[num18];
				int num19 = 0;
				array[num19++] = 85;
				array[num19++] = 84;
				array[num19++] = (byte) (num18 - 4);
				array[num19++] = 0;
				array[num19++] = 7;
				int value2 = (int) (_Mtime - _unixEpoch).TotalSeconds;
				Array.Copy(BitConverter.GetBytes(value2), 0, array, num19, 4);
				num19 += 4;
				if (!forCentralDirectory)
				{
					value2 = (int) (_Atime - _unixEpoch).TotalSeconds;
					Array.Copy(BitConverter.GetBytes(value2), 0, array, num19, 4);
					num19 += 4;
					value2 = (int) (_Ctime - _unixEpoch).TotalSeconds;
					Array.Copy(BitConverter.GetBytes(value2), 0, array, num19, 4);
					num19 += 4;
				}

				list.Add(array);
			}

			byte[] array2 = null;
			if (list.Count > 0)
			{
				int num25 = 0;
				int num26 = 0;
				for (int i = 0; i < list.Count; i++) num25 += list[i].Length;
				array2 = new byte[num25];
				for (int i = 0; i < list.Count; i++)
				{
					Array.Copy(list[i], 0, array2, num26, list[i].Length);
					num26 += list[i].Length;
				}
			}

			return array2;
		}

		private string NormalizeFileName()
		{
			string text = FileName.Replace("\\", "/");
			string text2 = null;
			if (_TrimVolumeFromFullyQualifiedPaths && FileName.Length >= 3 && FileName[1] == ':' && text[2] == '/')
				return text.Substring(3);
			if (FileName.Length >= 4 && text[0] == '/' && text[1] == '/')
			{
				int num = text.IndexOf('/', 2);
				if (num == -1) throw new ArgumentException("The path for that entry appears to be badly formatted");
				return text.Substring(num + 1);
			}

			if (FileName.Length >= 3 && text[0] == '.' && text[1] == '/') return text.Substring(2);
			return text;
		}

		private byte[] GetEncodedFileNameBytes()
		{
			string text = NormalizeFileName();
			switch (AlternateEncodingUsage)
			{
				case ZipOption.Always:
					if (_Comment != null && _Comment.Length != 0) _CommentBytes = AlternateEncoding.GetBytes(_Comment);
					_actualEncoding = AlternateEncoding;
					return AlternateEncoding.GetBytes(text);
				case ZipOption.Default:
					if (_Comment != null && _Comment.Length != 0) _CommentBytes = ibm437.GetBytes(_Comment);
					_actualEncoding = ibm437;
					return ibm437.GetBytes(text);
				default:
				{
					byte[] bytes = ibm437.GetBytes(text);
					string @string = ibm437.GetString(bytes, 0, bytes.Length);
					_CommentBytes = null;
					if (@string != text)
					{
						bytes = AlternateEncoding.GetBytes(text);
						if (_Comment != null && _Comment.Length != 0) _CommentBytes = AlternateEncoding.GetBytes(_Comment);
						_actualEncoding = AlternateEncoding;
						return bytes;
					}

					_actualEncoding = ibm437;
					if (_Comment != null && _Comment.Length != 0)
					{
						byte[] bytes2 = ibm437.GetBytes(_Comment);
						string string2 = ibm437.GetString(bytes2, 0, bytes2.Length);
						if (string2 != Comment)
						{
							bytes = AlternateEncoding.GetBytes(text);
							_CommentBytes = AlternateEncoding.GetBytes(_Comment);
							_actualEncoding = AlternateEncoding;
							return bytes;
						}

						_CommentBytes = bytes2;
						return bytes;
					}

					return bytes;
				}
			}
		}

		private bool WantReadAgain()
		{
			if (_UncompressedSize < 16) return false;
			if (_CompressionMethod == 0) return false;
			if (CompressionLevel == CompressionLevel.None) return false;
			if (_CompressedSize < _UncompressedSize) return false;
			if (_Source == ZipEntrySource.Stream && !_sourceStream.CanSeek) return false;
			if (_zipCrypto_forWrite != null && CompressedSize - 12 <= UncompressedSize) return false;
			return true;
		}

		private void MaybeUnsetCompressionMethodForWriting(int cycle)
		{
			if (cycle > 1)
			{
				_CompressionMethod = 0;
			}
			else if (IsDirectory)
			{
				_CompressionMethod = 0;
			}
			else if (_Source != ZipEntrySource.ZipFile)
			{
				if (_Source == ZipEntrySource.Stream)
				{
					if (_sourceStream != null && _sourceStream.CanSeek)
					{
						long length = _sourceStream.Length;
						if (length == 0)
						{
							_CompressionMethod = 0;
							return;
						}
					}
				}
				else if (_Source == ZipEntrySource.FileSystem && SharedUtilities.GetFileLength(LocalFileName) == 0)
				{
					_CompressionMethod = 0;
					return;
				}

				if (SetCompression != null) CompressionLevel = SetCompression(LocalFileName, _FileNameInArchive);
				if (CompressionLevel == CompressionLevel.None && CompressionMethod == CompressionMethod.Deflate)
					_CompressionMethod = 0;
			}
		}

		internal void WriteHeader(Stream s, int cycle)
		{
			CountingStream countingStream = s as CountingStream;
			_future_ROLH = countingStream != null ? countingStream.ComputedPosition : s.Position;
			int num = 0;
			int num2 = 0;
			byte[] array = new byte[30];
			array[num2++] = 80;
			array[num2++] = 75;
			array[num2++] = 3;
			array[num2++] = 4;
			_presumeZip64 = _container.Zip64 == Zip64Option.Always || _container.Zip64 == Zip64Option.AsNecessary && !s.CanSeek;
			short num7 = (short) (_presumeZip64 ? 45 : 20);
			array[num2++] = (byte) (num7 & 0xFF);
			array[num2++] = (byte) ((num7 & 0xFF00) >> 8);
			byte[] encodedFileNameBytes = GetEncodedFileNameBytes();
			short num10 = (short) encodedFileNameBytes.Length;
			if (_Encryption == EncryptionAlgorithm.None)
				_BitField &= -2;
			else
				_BitField |= 1;
			if (_actualEncoding.CodePage == Encoding.UTF8.CodePage) _BitField |= 2048;
			if (IsDirectory || cycle == 99)
			{
				_BitField &= -9;
				_BitField &= -2;
				Encryption = EncryptionAlgorithm.None;
				Password = null;
			}
			else if (!s.CanSeek)
			{
				_BitField |= 8;
			}

			array[num2++] = (byte) (_BitField & 0xFF);
			array[num2++] = (byte) ((_BitField & 0xFF00) >> 8);
			if (__FileDataPosition == -1)
			{
				_CompressedSize = 0L;
				_crcCalculated = false;
			}

			MaybeUnsetCompressionMethodForWriting(cycle);
			array[num2++] = (byte) (_CompressionMethod & 0xFF);
			array[num2++] = (byte) ((_CompressionMethod & 0xFF00) >> 8);
			if (cycle == 99) SetZip64Flags();
			_TimeBlob = SharedUtilities.DateTimeToPacked(LastModified);
			array[num2++] = (byte) (_TimeBlob & 0xFF);
			array[num2++] = (byte) ((_TimeBlob & 0xFF00) >> 8);
			array[num2++] = (byte) ((_TimeBlob & 0xFF0000) >> 16);
			array[num2++] = (byte) ((_TimeBlob & 4278190080u) >> 24);
			array[num2++] = (byte) (_Crc32 & 0xFF);
			array[num2++] = (byte) ((_Crc32 & 0xFF00) >> 8);
			array[num2++] = (byte) ((_Crc32 & 0xFF0000) >> 16);
			array[num2++] = (byte) ((_Crc32 & 4278190080u) >> 24);
			if (_presumeZip64)
			{
				for (num = 0; num < 8; num++) array[num2++] = 255;
			}
			else
			{
				array[num2++] = (byte) (_CompressedSize & 0xFF);
				array[num2++] = (byte) ((_CompressedSize & 0xFF00) >> 8);
				array[num2++] = (byte) ((_CompressedSize & 0xFF0000) >> 16);
				array[num2++] = (byte) ((_CompressedSize & 4278190080u) >> 24);
				array[num2++] = (byte) (_UncompressedSize & 0xFF);
				array[num2++] = (byte) ((_UncompressedSize & 0xFF00) >> 8);
				array[num2++] = (byte) ((_UncompressedSize & 0xFF0000) >> 16);
				array[num2++] = (byte) ((_UncompressedSize & 4278190080u) >> 24);
			}

			array[num2++] = (byte) (num10 & 0xFF);
			array[num2++] = (byte) ((num10 & 0xFF00) >> 8);
			_Extra = ConstructExtraField(false);
			short num34 = (short) (_Extra != null ? _Extra.Length : 0);
			array[num2++] = (byte) (num34 & 0xFF);
			array[num2++] = (byte) ((num34 & 0xFF00) >> 8);
			byte[] array2 = new byte[num2 + num10 + num34];
			Buffer.BlockCopy(array, 0, array2, 0, num2);
			Buffer.BlockCopy(encodedFileNameBytes, 0, array2, num2, encodedFileNameBytes.Length);
			num2 += encodedFileNameBytes.Length;
			if (_Extra != null)
			{
				Buffer.BlockCopy(_Extra, 0, array2, num2, _Extra.Length);
				num2 += _Extra.Length;
			}

			_LengthOfHeader = num2;
			ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
			if (zipSegmentedStream != null)
			{
				zipSegmentedStream.ContiguousWrite = true;
				uint num37 = zipSegmentedStream.ComputeSegment(num2);
				if (num37 != zipSegmentedStream.CurrentSegment)
					_future_ROLH = 0L;
				else
					_future_ROLH = zipSegmentedStream.Position;
				_diskNumber = num37;
			}

			if (_container.Zip64 == Zip64Option.Default && (uint) _RelativeOffsetOfLocalHeader >= 4294967295u)
				throw new ZipException(
					"Offset within the zip archive exceeds 0xFFFFFFFF. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
			s.Write(array2, 0, num2);
			if (zipSegmentedStream != null) zipSegmentedStream.ContiguousWrite = false;
			_EntryHeader = array2;
		}

		private int FigureCrc32()
		{
			if (!_crcCalculated)
			{
				Stream stream = null;
				if (_Source == ZipEntrySource.WriteDelegate)
				{
					CrcCalculatorStream crcCalculatorStream = new CrcCalculatorStream(Stream.Null);
					_WriteDelegate(FileName, crcCalculatorStream);
					_Crc32 = crcCalculatorStream.Crc;
				}
				else if (_Source != ZipEntrySource.ZipFile)
				{
					if (_Source == ZipEntrySource.Stream)
					{
						PrepSourceStream();
						stream = _sourceStream;
					}
					else if (_Source == ZipEntrySource.JitStream)
					{
						if (_sourceStream == null) _sourceStream = _OpenDelegate(FileName);
						PrepSourceStream();
						stream = _sourceStream;
					}
					else if (_Source != ZipEntrySource.ZipOutputStream)
					{
						stream = File.Open(LocalFileName, FileMode.Open, FileAccess.Read, FileShare.ReadWrite);
					}

					CRC32 cRC = new CRC32();
					_Crc32 = cRC.GetCrc32(stream);
					if (_sourceStream == null) stream.Dispose();
				}

				_crcCalculated = true;
			}

			return _Crc32;
		}

		private void PrepSourceStream()
		{
			if (_sourceStream == null)
				throw new ZipException($"The input stream is null for entry '{FileName}'.");
			if (_sourceStreamOriginalPosition.HasValue)
			{
				_sourceStream.Position = _sourceStreamOriginalPosition.Value;
				return;
			}

			if (_sourceStream.CanSeek)
			{
				_sourceStreamOriginalPosition = _sourceStream.Position;
				return;
			}

			if (Encryption != EncryptionAlgorithm.PkzipWeak) return;
			if (_Source == ZipEntrySource.ZipFile) return;
			if ((_BitField & 8) == 8) return;
			throw new ZipException("It is not possible to use PKZIP encryption on a non-seekable input stream");
		}

		internal void CopyMetaData(ZipEntry source)
		{
			__FileDataPosition = source.__FileDataPosition;
			CompressionMethod = source.CompressionMethod;
			_CompressionMethod_FromZipFile = source._CompressionMethod_FromZipFile;
			_CompressedFileDataSize = source._CompressedFileDataSize;
			_UncompressedSize = source._UncompressedSize;
			_BitField = source._BitField;
			_Source = source._Source;
			_LastModified = source._LastModified;
			_Mtime = source._Mtime;
			_Atime = source._Atime;
			_Ctime = source._Ctime;
			_ntfsTimesAreSet = source._ntfsTimesAreSet;
			_emitUnixTimes = source._emitUnixTimes;
			_emitNtfsTimes = source._emitNtfsTimes;
		}

		private void OnWriteBlock(long bytesXferred, long totalBytesToXfer)
		{
			if (_container.ZipFile != null)
				_ioOperationCanceled = _container.ZipFile.OnSaveBlock(this, bytesXferred, totalBytesToXfer);
		}

		private void _WriteEntryData(Stream s)
		{
			Stream stream = null;
			long _FileDataPosition = -1L;
			try
			{
				_FileDataPosition = s.Position;
			}
			catch (Exception)
			{
			}

			try
			{
				long num = SetInputAndFigureFileLength(ref stream);
				CountingStream countingStream = new CountingStream(s);
				Stream stream2;
				Stream stream3;
				if (num != 0)
				{
					stream2 = MaybeApplyEncryption(countingStream);
					stream3 = MaybeApplyCompression(stream2, num);
				}
				else
				{
					stream2 = stream3 = countingStream;
				}

				CrcCalculatorStream crcCalculatorStream = new CrcCalculatorStream(stream3, true);
				if (_Source == ZipEntrySource.WriteDelegate)
				{
					_WriteDelegate(FileName, crcCalculatorStream);
				}
				else
				{
					byte[] array = new byte[BufferSize];
					int count;
					while ((count = SharedUtilities.ReadWithRetry(stream, array, 0, array.Length, FileName)) != 0)
					{
						crcCalculatorStream.Write(array, 0, count);
						OnWriteBlock(crcCalculatorStream.TotalBytesSlurped, num);
						if (_ioOperationCanceled) break;
					}
				}

				FinishOutputStream(s, countingStream, stream2, stream3, crcCalculatorStream);
			}
			finally
			{
				if (_Source == ZipEntrySource.JitStream)
				{
					if (_CloseDelegate != null) _CloseDelegate(FileName, stream);
				}
				else if (stream is FileStream)
				{
					stream.Dispose();
				}
			}

			if (!_ioOperationCanceled)
			{
				__FileDataPosition = _FileDataPosition;
				PostProcessOutput(s);
			}
		}

		private long SetInputAndFigureFileLength(ref Stream input)
		{
			long result = -1L;
			if (_Source == ZipEntrySource.Stream)
			{
				PrepSourceStream();
				input = _sourceStream;
				try
				{
					result = _sourceStream.Length;
					return result;
				}
				catch (NotSupportedException)
				{
					return result;
				}
			}

			if (_Source == ZipEntrySource.ZipFile)
			{
				string password = _Encryption_FromZipFile == EncryptionAlgorithm.None ? null : (_Password ?? _container.Password);
				_sourceStream = InternalOpenReader(password);
				PrepSourceStream();
				input = _sourceStream;
				result = _sourceStream.Length;
			}
			else
			{
				if (_Source == ZipEntrySource.JitStream)
				{
					if (_sourceStream == null) _sourceStream = _OpenDelegate(FileName);
					PrepSourceStream();
					input = _sourceStream;
					try
					{
						result = _sourceStream.Length;
						return result;
					}
					catch (NotSupportedException)
					{
						return result;
					}
				}

				if (_Source == ZipEntrySource.FileSystem)
				{
					FileShare fileShare = FileShare.ReadWrite;
					fileShare |= FileShare.Delete;
					input = File.Open(LocalFileName, FileMode.Open, FileAccess.Read, fileShare);
					result = input.Length;
				}
			}

			return result;
		}

		internal void FinishOutputStream(Stream s, CountingStream entryCounter, Stream encryptor, Stream compressor,
			CrcCalculatorStream output)
		{
			if (output != null)
			{
				output.Close();
				if (compressor is DeflateStream)
					compressor.Close();
				else if (compressor is ParallelDeflateOutputStream)
					compressor.Close();
				encryptor.Flush();
				encryptor.Close();
				_LengthOfTrailer = 0;
				_UncompressedSize = output.TotalBytesSlurped;
				_CompressedFileDataSize = entryCounter.BytesWritten;
				_CompressedSize = _CompressedFileDataSize;
				_Crc32 = output.Crc;
				StoreRelativeOffset();
			}
		}

		internal void PostProcessOutput(Stream s)
		{
			CountingStream countingStream = s as CountingStream;
			if (_UncompressedSize == 0 && _CompressedSize == 0)
			{
				if (_Source != ZipEntrySource.ZipOutputStream)
				{
					if (_Password != null)
					{
						int num = 0;
						if (Encryption == EncryptionAlgorithm.PkzipWeak) num = 12;
						if (_Source == ZipEntrySource.ZipOutputStream && !s.CanSeek)
							throw new ZipException("Zero bytes written, encryption in use, and non-seekable output.");
						if (Encryption != 0)
						{
							s.Seek(-1 * num, SeekOrigin.Current);
							s.SetLength(s.Position);
							if (countingStream != null) countingStream.Adjust(num);
							_LengthOfHeader -= num;
							__FileDataPosition -= num;
						}

						_Password = null;
						_BitField &= -2;
						int num2 = 6;
						_EntryHeader[num2++] = (byte) (_BitField & 0xFF);
						_EntryHeader[num2++] = (byte) ((_BitField & 0xFF00) >> 8);
					}

					CompressionMethod = CompressionMethod.None;
					Encryption = EncryptionAlgorithm.None;
					goto IL_0126;
				}

				return;
			}

			if (_zipCrypto_forWrite != null && Encryption == EncryptionAlgorithm.PkzipWeak) _CompressedSize += 12L;
			IL_0126:
			int num5 = 8;
			_EntryHeader[num5++] = (byte) (_CompressionMethod & 0xFF);
			_EntryHeader[num5++] = (byte) ((_CompressionMethod & 0xFF00) >> 8);
			num5 = 14;
			_EntryHeader[num5++] = (byte) (_Crc32 & 0xFF);
			_EntryHeader[num5++] = (byte) ((_Crc32 & 0xFF00) >> 8);
			_EntryHeader[num5++] = (byte) ((_Crc32 & 0xFF0000) >> 16);
			_EntryHeader[num5++] = (byte) ((_Crc32 & 4278190080u) >> 24);
			SetZip64Flags();
			short num12 = (short) (_EntryHeader[26] + _EntryHeader[27] * 256);
			short num13 = (short) (_EntryHeader[28] + _EntryHeader[29] * 256);
			if (OutputUsedZip64.Value)
			{
				_EntryHeader[4] = 45;
				_EntryHeader[5] = 0;
				for (int i = 0; i < 8; i++) _EntryHeader[num5++] = 255;
				num5 = 30 + num12;
				_EntryHeader[num5++] = 1;
				_EntryHeader[num5++] = 0;
				num5 += 2;
				Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, _EntryHeader, num5, 8);
				num5 += 8;
				Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, _EntryHeader, num5, 8);
			}
			else
			{
				_EntryHeader[4] = 20;
				_EntryHeader[5] = 0;
				num5 = 18;
				_EntryHeader[num5++] = (byte) (_CompressedSize & 0xFF);
				_EntryHeader[num5++] = (byte) ((_CompressedSize & 0xFF00) >> 8);
				_EntryHeader[num5++] = (byte) ((_CompressedSize & 0xFF0000) >> 16);
				_EntryHeader[num5++] = (byte) ((_CompressedSize & 4278190080u) >> 24);
				_EntryHeader[num5++] = (byte) (_UncompressedSize & 0xFF);
				_EntryHeader[num5++] = (byte) ((_UncompressedSize & 0xFF00) >> 8);
				_EntryHeader[num5++] = (byte) ((_UncompressedSize & 0xFF0000) >> 16);
				_EntryHeader[num5++] = (byte) ((_UncompressedSize & 4278190080u) >> 24);
				if (num13 != 0)
				{
					num5 = 30 + num12;
					short num25 = (short) (_EntryHeader[num5 + 2] + _EntryHeader[num5 + 3] * 256);
					if (num25 == 16)
					{
						_EntryHeader[num5++] = 153;
						_EntryHeader[num5++] = 153;
					}
				}
			}

			if ((_BitField & 8) != 8 || _Source == ZipEntrySource.ZipOutputStream && s.CanSeek)
			{
				ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
				if (zipSegmentedStream != null && _diskNumber != zipSegmentedStream.CurrentSegment)
				{
					using (Stream stream = ZipSegmentedStream.ForUpdate(_container.ZipFile.Name, _diskNumber))
					{
						stream.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
						stream.Write(_EntryHeader, 0, _EntryHeader.Length);
					}
				}
				else
				{
					s.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
					s.Write(_EntryHeader, 0, _EntryHeader.Length);
					if (countingStream != null) countingStream.Adjust(_EntryHeader.Length);
					s.Seek(_CompressedSize, SeekOrigin.Current);
				}
			}

			if ((_BitField & 8) == 8 && !IsDirectory)
			{
				byte[] array = new byte[16 + (OutputUsedZip64.Value ? 8 : 0)];
				num5 = 0;
				Array.Copy(BitConverter.GetBytes(134695760), 0, array, num5, 4);
				num5 += 4;
				Array.Copy(BitConverter.GetBytes(_Crc32), 0, array, num5, 4);
				num5 += 4;
				if (OutputUsedZip64.Value)
				{
					Array.Copy(BitConverter.GetBytes(_CompressedSize), 0, array, num5, 8);
					num5 += 8;
					Array.Copy(BitConverter.GetBytes(_UncompressedSize), 0, array, num5, 8);
					num5 += 8;
				}
				else
				{
					array[num5++] = (byte) (_CompressedSize & 0xFF);
					array[num5++] = (byte) ((_CompressedSize & 0xFF00) >> 8);
					array[num5++] = (byte) ((_CompressedSize & 0xFF0000) >> 16);
					array[num5++] = (byte) ((_CompressedSize & 4278190080u) >> 24);
					array[num5++] = (byte) (_UncompressedSize & 0xFF);
					array[num5++] = (byte) ((_UncompressedSize & 0xFF00) >> 8);
					array[num5++] = (byte) ((_UncompressedSize & 0xFF0000) >> 16);
					array[num5++] = (byte) ((_UncompressedSize & 4278190080u) >> 24);
				}

				s.Write(array, 0, array.Length);
				_LengthOfTrailer += array.Length;
			}
		}

		private void SetZip64Flags()
		{
			RequiresZip64 = _CompressedSize >= 4294967295u || _UncompressedSize >= 4294967295u ||
			                _RelativeOffsetOfLocalHeader >= 4294967295u;
			if (_container.Zip64 == Zip64Option.Default && RequiresZip64.Value)
				throw new ZipException(
					"Compressed or Uncompressed size, or offset exceeds the maximum value. Consider setting the UseZip64WhenSaving property on the ZipFile instance.");
			OutputUsedZip64 = _container.Zip64 == Zip64Option.Always || RequiresZip64.Value;
		}

		internal void PrepOutputStream(Stream s, long streamLength, out CountingStream outputCounter, out Stream encryptor,
			out Stream compressor, out CrcCalculatorStream output)
		{
			outputCounter = new CountingStream(s);
			if (streamLength != 0)
			{
				encryptor = MaybeApplyEncryption(outputCounter);
				compressor = MaybeApplyCompression(encryptor, streamLength);
			}
			else
			{
				encryptor = compressor = outputCounter;
			}

			output = new CrcCalculatorStream(compressor, true);
		}

		private Stream MaybeApplyCompression(Stream s, long streamLength)
		{
			if (_CompressionMethod == 8 && CompressionLevel != 0)
			{
				if (_container.ParallelDeflateThreshold != 0 &&
				    (streamLength <= _container.ParallelDeflateThreshold || _container.ParallelDeflateThreshold <= 0))
				{
					DeflateStream deflateStream = new DeflateStream(s, CompressionMode.Compress, CompressionLevel, true);
					if (_container.CodecBufferSize > 0) deflateStream.BufferSize = _container.CodecBufferSize;
					deflateStream.Strategy = _container.Strategy;
					return deflateStream;
				}

				if (_container.ParallelDeflater == null)
				{
					_container.ParallelDeflater = new ParallelDeflateOutputStream(s, CompressionLevel, _container.Strategy, true);
					if (_container.CodecBufferSize > 0) _container.ParallelDeflater.BufferSize = _container.CodecBufferSize;
					if (_container.ParallelDeflateMaxBufferPairs > 0)
						_container.ParallelDeflater.MaxBufferPairs = _container.ParallelDeflateMaxBufferPairs;
				}

				ParallelDeflateOutputStream parallelDeflater = _container.ParallelDeflater;
				parallelDeflater.Reset(s);
				return parallelDeflater;
			}

			return s;
		}

		private Stream MaybeApplyEncryption(Stream s)
		{
			if (Encryption == EncryptionAlgorithm.PkzipWeak)
				return new ZipCipherStream(s, _zipCrypto_forWrite, CryptoMode.Encrypt);
			return s;
		}

		private void OnZipErrorWhileSaving(Exception e)
		{
			if (_container.ZipFile != null) _ioOperationCanceled = _container.ZipFile.OnZipErrorSaving(this, e);
		}

		internal void Write(Stream s)
		{
			CountingStream countingStream = s as CountingStream;
			ZipSegmentedStream zipSegmentedStream = s as ZipSegmentedStream;
			bool flag = false;
			do
			{
				try
				{
					if (_Source == ZipEntrySource.ZipFile && !_restreamRequiredOnSave)
					{
						CopyThroughOneEntry(s);
						return;
					}

					if (IsDirectory)
					{
						WriteHeader(s, 1);
						StoreRelativeOffset();
						RequiresZip64 = _RelativeOffsetOfLocalHeader >= 4294967295u;
						OutputUsedZip64 = _container.Zip64 == Zip64Option.Always || RequiresZip64.Value;
						if (zipSegmentedStream != null) _diskNumber = zipSegmentedStream.CurrentSegment;
						return;
					}

					bool flag2 = true;
					int num = 0;
					do
					{
						num++;
						WriteHeader(s, num);
						WriteSecurityMetadata(s);
						_WriteEntryData(s);
						_TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;
						flag2 = num <= 1 && s.CanSeek && WantReadAgain();
						if (flag2)
						{
							if (zipSegmentedStream != null)
								zipSegmentedStream.TruncateBackward(_diskNumber, _RelativeOffsetOfLocalHeader);
							else
								s.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
							s.SetLength(s.Position);
							if (countingStream != null) countingStream.Adjust(_TotalEntrySize);
						}
					} while (flag2);

					_skippedDuringSave = false;
					flag = true;
				}
				catch (Exception ex)
				{
					ZipErrorAction zipErrorAction = ZipErrorAction;
					int num2 = 0;
					while (true)
					{
						if (ZipErrorAction == ZipErrorAction.Throw) throw;
						if (ZipErrorAction != ZipErrorAction.Skip && ZipErrorAction != ZipErrorAction.Retry)
						{
							if (num2 > 0) throw;
							if (ZipErrorAction == ZipErrorAction.InvokeErrorEvent)
							{
								OnZipErrorWhileSaving(ex);
								if (_ioOperationCanceled)
								{
									flag = true;
									break;
								}
							}

							num2++;
							continue;
						}

						long num3 = countingStream != null ? countingStream.ComputedPosition : s.Position;
						long num4 = num3 - _future_ROLH;
						if (num4 > 0)
						{
							s.Seek(num4, SeekOrigin.Current);
							long position = s.Position;
							s.SetLength(s.Position);
							if (countingStream != null) countingStream.Adjust(num3 - position);
						}

						if (ZipErrorAction == ZipErrorAction.Skip)
						{
							WriteStatus("Skipping file {0} (exception: {1})", LocalFileName, ex.ToString());
							_skippedDuringSave = true;
							flag = true;
						}
						else
						{
							ZipErrorAction = zipErrorAction;
						}

						break;
					}
				}
			} while (!flag);
		}

		internal void StoreRelativeOffset()
		{
			_RelativeOffsetOfLocalHeader = _future_ROLH;
		}

		internal void NotifySaveComplete()
		{
			_Encryption_FromZipFile = _Encryption;
			_CompressionMethod_FromZipFile = _CompressionMethod;
			_restreamRequiredOnSave = false;
			_metadataChanged = false;
			_Source = ZipEntrySource.ZipFile;
		}

		internal void WriteSecurityMetadata(Stream outstream)
		{
			if (Encryption != 0)
			{
				string password = _Password;
				if (_Source == ZipEntrySource.ZipFile && password == null) password = _container.Password;
				if (password == null)
				{
					_zipCrypto_forWrite = null;
				}
				else if (Encryption == EncryptionAlgorithm.PkzipWeak)
				{
					_zipCrypto_forWrite = ZipCrypto.ForWrite(password);
					Random random = new Random();
					byte[] array = new byte[12];
					random.NextBytes(array);
					if ((_BitField & 8) == 8)
					{
						_TimeBlob = SharedUtilities.DateTimeToPacked(LastModified);
						array[11] = (byte) ((_TimeBlob >> 8) & 0xFF);
					}
					else
					{
						FigureCrc32();
						array[11] = (byte) ((_Crc32 >> 24) & 0xFF);
					}

					byte[] array2 = _zipCrypto_forWrite.EncryptMessage(array, array.Length);
					outstream.Write(array2, 0, array2.Length);
					_LengthOfHeader += array2.Length;
				}
			}
		}

		private void CopyThroughOneEntry(Stream outStream)
		{
			if (LengthOfHeader == 0) throw new BadStateException("Bad header length.");
			if (_metadataChanged || ArchiveStream is ZipSegmentedStream || outStream is ZipSegmentedStream ||
			    _InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Default ||
			    !_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Always)
				CopyThroughWithRecompute(outStream);
			else
				CopyThroughWithNoChange(outStream);
			RequiresZip64 = _CompressedSize >= 4294967295u || _UncompressedSize >= 4294967295u ||
			                _RelativeOffsetOfLocalHeader >= 4294967295u;
			OutputUsedZip64 = _container.Zip64 == Zip64Option.Always || RequiresZip64.Value;
		}

		private void CopyThroughWithRecompute(Stream outstream)
		{
			byte[] array = new byte[BufferSize];
			CountingStream countingStream = new CountingStream(ArchiveStream);
			long relativeOffsetOfLocalHeader = _RelativeOffsetOfLocalHeader;
			int lengthOfHeader = LengthOfHeader;
			WriteHeader(outstream, 0);
			StoreRelativeOffset();
			if (!FileName.EndsWith("/"))
			{
				long num = relativeOffsetOfLocalHeader + lengthOfHeader;
				int lengthOfCryptoHeaderBytes = GetLengthOfCryptoHeaderBytes(_Encryption_FromZipFile);
				num -= lengthOfCryptoHeaderBytes;
				_LengthOfHeader += lengthOfCryptoHeaderBytes;
				countingStream.Seek(num, SeekOrigin.Begin);
				long num2 = _CompressedSize;
				while (num2 > 0)
				{
					lengthOfCryptoHeaderBytes = (int) (num2 > array.Length ? array.Length : num2);
					int num3 = countingStream.Read(array, 0, lengthOfCryptoHeaderBytes);
					outstream.Write(array, 0, num3);
					num2 -= num3;
					OnWriteBlock(countingStream.BytesRead, _CompressedSize);
					if (_ioOperationCanceled) break;
				}

				if ((_BitField & 8) == 8)
				{
					int num4 = 16;
					if (_InputUsesZip64) num4 += 8;
					byte[] buffer = new byte[num4];
					countingStream.Read(buffer, 0, num4);
					if (_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Default)
					{
						outstream.Write(buffer, 0, 8);
						if (_CompressedSize > 4294967295u) throw new InvalidOperationException("ZIP64 is required");
						outstream.Write(buffer, 8, 4);
						if (_UncompressedSize > 4294967295u) throw new InvalidOperationException("ZIP64 is required");
						outstream.Write(buffer, 16, 4);
						_LengthOfTrailer -= 8;
					}
					else if (!_InputUsesZip64 && _container.UseZip64WhenSaving == Zip64Option.Always)
					{
						byte[] buffer2 = new byte[4];
						outstream.Write(buffer, 0, 8);
						outstream.Write(buffer, 8, 4);
						outstream.Write(buffer2, 0, 4);
						outstream.Write(buffer, 12, 4);
						outstream.Write(buffer2, 0, 4);
						_LengthOfTrailer += 8;
					}
					else
					{
						outstream.Write(buffer, 0, num4);
					}
				}
			}

			_TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;
		}

		private void CopyThroughWithNoChange(Stream outstream)
		{
			byte[] array = new byte[BufferSize];
			CountingStream countingStream = new CountingStream(ArchiveStream);
			countingStream.Seek(_RelativeOffsetOfLocalHeader, SeekOrigin.Begin);
			if (_TotalEntrySize == 0) _TotalEntrySize = _LengthOfHeader + _CompressedFileDataSize + _LengthOfTrailer;
			CountingStream countingStream2 = outstream as CountingStream;
			_RelativeOffsetOfLocalHeader = countingStream2 != null ? countingStream2.ComputedPosition : outstream.Position;
			long num = _TotalEntrySize;
			while (num > 0)
			{
				int count = (int) (num > array.Length ? array.Length : num);
				int num2 = countingStream.Read(array, 0, count);
				outstream.Write(array, 0, num2);
				num -= num2;
				OnWriteBlock(countingStream.BytesRead, _TotalEntrySize);
				if (_ioOperationCanceled) break;
			}
		}

		[Conditional("Trace")]
		private void TraceWriteLine(string format, params object[] varParams)
		{
			lock (_outputLock)
			{
				int hashCode = Thread.CurrentThread.GetHashCode();
				Console.ForegroundColor = (ConsoleColor) (hashCode % 8 + 8);
				Console.Write("{0:000} ZipEntry.Write ", hashCode);
				Console.WriteLine(format, varParams);
				Console.ResetColor();
			}
		}

		private class CopyHelper
		{
			private static readonly Regex re = new Regex(" \\(copy (\\d+)\\)$");

			private static int callCount;

			internal static string AppendCopyToFileName(string f)
			{
				callCount++;
				if (callCount > 25) throw new OverflowException("overflow while creating filename");
				int num = 1;
				int num2 = f.LastIndexOf(".");
				if (num2 == -1)
				{
					Match match = re.Match(f);
					if (match.Success)
					{
						num = int.Parse(match.Groups[1].Value) + 1;
						string str = $" (copy {num})";
						f = f.Substring(0, match.Index) + str;
					}
					else
					{
						string str2 = $" (copy {num})";
						f += str2;
					}
				}
				else
				{
					Match match2 = re.Match(f.Substring(0, num2));
					if (match2.Success)
					{
						num = int.Parse(match2.Groups[1].Value) + 1;
						string str3 = $" (copy {num})";
						f = f.Substring(0, match2.Index) + str3 + f.Substring(num2);
					}
					else
					{
						string str4 = $" (copy {num})";
						f = f.Substring(0, num2) + str4 + f.Substring(num2);
					}
				}

				return f;
			}
		}

		private delegate T Func<T>();
	}
}