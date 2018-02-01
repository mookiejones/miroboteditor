#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:52 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	public class ZipOutputStream : Stream
	{
		private bool _anyEntriesUsedZip64;

		private string _comment;

		private ZipEntry _currentEntry;

		private Stream _deflater;

		private bool _directoryNeededZip64;

		private bool _disposed;

		private bool _DontIgnoreCase;
		private EncryptionAlgorithm _encryption;

		private Stream _encryptor;

		private Dictionary<string, ZipEntry> _entriesWritten;

		private int _entryCount;

		private CrcCalculatorStream _entryOutputStream;

		private bool _exceptionPending;

		private bool _leaveUnderlyingStreamOpen;

		private int _maxBufferPairs = 16;

		private bool _needToWriteEntryHeader;

		private CountingStream _outputCounter;

		private long _ParallelDeflateThreshold;

		internal string _password;

		private ZipEntryTimestamp _timestamp;

		internal Zip64Option _zip64;

		internal ParallelDeflateOutputStream ParallelDeflater;

		public ZipOutputStream(Stream stream)
			: this(stream, false)
		{
		}

		public ZipOutputStream(string fileName)
		{
			Stream stream = File.Open(fileName, FileMode.Create, FileAccess.ReadWrite, FileShare.None);
			_Init(stream, false, fileName);
		}

		public ZipOutputStream(Stream stream, bool leaveOpen)
		{
			_Init(stream, leaveOpen, null);
		}

		public string Password
		{
			set
			{
				if (_disposed)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("The stream has been closed.");
				}

				_password = value;
				if (_password == null)
					_encryption = EncryptionAlgorithm.None;
				else if (_encryption == EncryptionAlgorithm.None)
					_encryption = EncryptionAlgorithm.PkzipWeak;
			}
		}

		public EncryptionAlgorithm Encryption
		{
			get => _encryption;
			set
			{
				if (_disposed)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("The stream has been closed.");
				}

				if (value == EncryptionAlgorithm.Unsupported)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("You may not set Encryption to that value.");
				}

				_encryption = value;
			}
		}

		public int CodecBufferSize { get; set; }

		public CompressionStrategy Strategy { get; set; }

		public ZipEntryTimestamp Timestamp
		{
			get => _timestamp;
			set
			{
				if (_disposed)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("The stream has been closed.");
				}

				_timestamp = value;
			}
		}

		public CompressionLevel CompressionLevel { get; set; }

		public CompressionMethod CompressionMethod { get; set; }

		public string Comment
		{
			get => _comment;
			set
			{
				if (_disposed)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("The stream has been closed.");
				}

				_comment = value;
			}
		}

		public Zip64Option EnableZip64
		{
			get => _zip64;
			set
			{
				if (_disposed)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("The stream has been closed.");
				}

				_zip64 = value;
			}
		}

		public bool OutputUsedZip64
		{
			get
			{
				if (!_anyEntriesUsedZip64) return _directoryNeededZip64;
				return true;
			}
		}

		public bool IgnoreCase
		{
			get => !_DontIgnoreCase;
			set => _DontIgnoreCase = !value;
		}

		[Obsolete(
			"Beginning with v1.9.1.6 of DotNetZip, this property is obsolete. It will be removed in a future version of the library. Use AlternateEncoding and AlternateEncodingUsage instead.")]
		public bool UseUnicodeAsNecessary
		{
			get
			{
				if (AlternateEncoding == Encoding.UTF8) return AlternateEncodingUsage == ZipOption.AsNecessary;
				return false;
			}
			set
			{
				if (value)
				{
					AlternateEncoding = Encoding.UTF8;
					AlternateEncodingUsage = ZipOption.AsNecessary;
				}
				else
				{
					AlternateEncoding = DefaultEncoding;
					AlternateEncodingUsage = ZipOption.Default;
				}
			}
		}

		[Obsolete("use AlternateEncoding and AlternateEncodingUsage instead.")]
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

		public static Encoding DefaultEncoding => Encoding.GetEncoding("IBM437");

		public long ParallelDeflateThreshold
		{
			get => _ParallelDeflateThreshold;
			set
			{
				if (value != 0 && value != -1 && value < 65536)
					throw new ArgumentOutOfRangeException("value must be greater than 64k, or 0, or -1");
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

		internal Stream OutputStream { get; private set; }

		internal string Name { get; private set; }

		public override bool CanRead => false;

		public override bool CanSeek => false;

		public override bool CanWrite => true;

		public override long Length => throw new NotSupportedException();

		public override long Position
		{
			get => OutputStream.Position;
			set => throw new NotSupportedException();
		}

		private void _Init(Stream stream, bool leaveOpen, string name)
		{
			OutputStream = stream.CanRead ? stream : new CountingStream(stream);
			CompressionLevel = CompressionLevel.Default;
			CompressionMethod = CompressionMethod.Deflate;
			_encryption = EncryptionAlgorithm.None;
			_entriesWritten = new Dictionary<string, ZipEntry>(StringComparer.Ordinal);
			_zip64 = Zip64Option.Default;
			_leaveUnderlyingStreamOpen = leaveOpen;
			Strategy = CompressionStrategy.Default;
			Name = name ?? "(stream)";
			ParallelDeflateThreshold = -1L;
		}

		public override string ToString()
		{
			return $"ZipOutputStream::{Name}(leaveOpen({_leaveUnderlyingStreamOpen})))";
		}

		private void InsureUniqueEntry(ZipEntry ze1)
		{
			if (!_entriesWritten.ContainsKey(ze1.FileName)) return;
			_exceptionPending = true;
			throw new ArgumentException($"The entry '{ze1.FileName}' already exists in the zip archive.");
		}

		public bool ContainsEntry(string name)
		{
			return _entriesWritten.ContainsKey(SharedUtilities.NormalizePathForUseInZipFile(name));
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_disposed)
			{
				_exceptionPending = true;
				throw new InvalidOperationException("The stream has been closed.");
			}

			if (buffer == null)
			{
				_exceptionPending = true;
				throw new ArgumentNullException(nameof(buffer));
			}

			if (_currentEntry == null)
			{
				_exceptionPending = true;
				throw new InvalidOperationException("You must call PutNextEntry() before calling Write().");
			}

			if (_currentEntry.IsDirectory)
			{
				_exceptionPending = true;
				throw new InvalidOperationException("You cannot Write() data for an entry that is a directory.");
			}

			if (_needToWriteEntryHeader) _InitiateCurrentEntry(false);
			if (count != 0) _entryOutputStream.Write(buffer, offset, count);
		}

		public ZipEntry PutNextEntry(string entryName)
		{
			if (string.IsNullOrEmpty(entryName)) throw new ArgumentNullException(nameof(entryName));
			if (_disposed)
			{
				_exceptionPending = true;
				throw new InvalidOperationException("The stream has been closed.");
			}

			_FinishCurrentEntry();
			_currentEntry = ZipEntry.CreateForZipOutputStream(entryName);
			_currentEntry._container = new ZipContainer(this);
			_currentEntry._BitField |= 8;
			_currentEntry.SetEntryTimes(DateTime.Now, DateTime.Now, DateTime.Now);
			_currentEntry.CompressionLevel = CompressionLevel;
			_currentEntry.CompressionMethod = CompressionMethod;
			_currentEntry.Password = _password;
			_currentEntry.Encryption = Encryption;
			_currentEntry.AlternateEncoding = AlternateEncoding;
			_currentEntry.AlternateEncodingUsage = AlternateEncodingUsage;
			if (entryName.EndsWith("/")) _currentEntry.MarkAsDirectory();
			_currentEntry.EmitTimesInWindowsFormatWhenSaving =
				(_timestamp & ZipEntryTimestamp.Windows) != ZipEntryTimestamp.None;
			_currentEntry.EmitTimesInUnixFormatWhenSaving = (_timestamp & ZipEntryTimestamp.Unix) != ZipEntryTimestamp.None;
			InsureUniqueEntry(_currentEntry);
			_needToWriteEntryHeader = true;
			return _currentEntry;
		}

		private void _InitiateCurrentEntry(bool finishing)
		{
			_entriesWritten.Add(_currentEntry.FileName, _currentEntry);
			_entryCount++;
			if (_entryCount > 65534 && _zip64 == Zip64Option.Default)
			{
				_exceptionPending = true;
				throw new InvalidOperationException("Too many entries. Consider setting ZipOutputStream.EnableZip64.");
			}

			_currentEntry.WriteHeader(OutputStream, finishing ? 99 : 0);
			_currentEntry.StoreRelativeOffset();
			if (!_currentEntry.IsDirectory)
			{
				_currentEntry.WriteSecurityMetadata(OutputStream);
				_currentEntry.PrepOutputStream(OutputStream, !finishing ? -1 : 0, out _outputCounter, out _encryptor, out _deflater,
					out _entryOutputStream);
			}

			_needToWriteEntryHeader = false;
		}

		private void _FinishCurrentEntry()
		{
			if (_currentEntry != null)
			{
				if (_needToWriteEntryHeader) _InitiateCurrentEntry(true);
				_currentEntry.FinishOutputStream(OutputStream, _outputCounter, _encryptor, _deflater, _entryOutputStream);
				_currentEntry.PostProcessOutput(OutputStream);
				if (_currentEntry.OutputUsedZip64.HasValue) _anyEntriesUsedZip64 |= _currentEntry.OutputUsedZip64.Value;
				_outputCounter = null;
				_encryptor = _deflater = null;
				_entryOutputStream = null;
			}
		}

		protected override void Dispose(bool disposing)
		{
			if (!_disposed)
			{
				if (disposing && !_exceptionPending)
				{
					_FinishCurrentEntry();
					_directoryNeededZip64 = ZipOutput.WriteCentralDirectoryStructure(OutputStream, _entriesWritten.Values, 1u, _zip64,
						Comment, new ZipContainer(this));
					Stream stream = null;
					var countingStream = OutputStream as CountingStream;
					if (countingStream != null)
					{
						stream = countingStream.WrappedStream;
						countingStream.Dispose();
					}
					else
					{
						stream = OutputStream;
					}

					if (!_leaveUnderlyingStreamOpen) stream.Dispose();
					OutputStream = null;
				}

				_disposed = true;
			}
		}

		public override void Flush()
		{
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Read");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException("Seek");
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}
}