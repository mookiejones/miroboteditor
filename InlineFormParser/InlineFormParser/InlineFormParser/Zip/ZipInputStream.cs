#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:07 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	public class ZipInputStream : Stream
	{
		private bool _closed;

		private ZipContainer _container;

		private CrcCalculatorStream _crcStream;
		private ZipEntry _currentEntry;

		private long _endOfEntry;

		private bool _exceptionPending;

		private bool _findRequired;

		private bool _firstEntry;

		private bool _leaveUnderlyingStreamOpen;

		private long _LeftToRead;

		private string _name;

		private bool _needSetup;

		internal string _Password;

		public ZipInputStream(Stream stream)
			: this(stream, false)
		{
		}

		public ZipInputStream(string fileName)
		{
			Stream stream = File.Open(fileName, FileMode.Open, FileAccess.Read, FileShare.Read);
			_Init(stream, false, fileName);
		}

		public ZipInputStream(Stream stream, bool leaveOpen)
		{
			_Init(stream, leaveOpen, null);
		}

		public Encoding ProvisionalAlternateEncoding { get; set; }

		public int CodecBufferSize { get; set; }

		public string Password
		{
			set
			{
				if (_closed)
				{
					_exceptionPending = true;
					throw new InvalidOperationException("The stream has been closed.");
				}

				_Password = value;
			}
		}

		internal Stream ReadStream { get; private set; }

		public override bool CanRead => true;

		public override bool CanSeek => ReadStream.CanSeek;

		public override bool CanWrite => false;

		public override long Length => ReadStream.Length;

		public override long Position
		{
			get => ReadStream.Position;
			set => Seek(value, SeekOrigin.Begin);
		}

		private void _Init(Stream stream, bool leaveOpen, string name)
		{
			ReadStream = stream;
			if (!ReadStream.CanRead) throw new ZipException("The stream must be readable.");
			_container = new ZipContainer(this);
			ProvisionalAlternateEncoding = Encoding.GetEncoding("IBM437");
			_leaveUnderlyingStreamOpen = leaveOpen;
			_findRequired = true;
			_name = name ?? "(stream)";
		}

		public override string ToString()
		{
			return $"ZipInputStream::{_name}(leaveOpen({_leaveUnderlyingStreamOpen})))";
		}

		private void SetupStream()
		{
			_crcStream = _currentEntry.InternalOpenReader(_Password);
			_LeftToRead = _crcStream.Length;
			_needSetup = false;
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_closed)
			{
				_exceptionPending = true;
				throw new InvalidOperationException("The stream has been closed.");
			}

			if (_needSetup) SetupStream();
			if (_LeftToRead == 0) return 0;
			int count2 = (int) (_LeftToRead > count ? count : _LeftToRead);
			int num = _crcStream.Read(buffer, offset, count2);
			_LeftToRead -= num;
			if (_LeftToRead == 0)
			{
				int crc = _crcStream.Crc;
				_currentEntry.VerifyCrcAfterExtract(crc);
				ReadStream.Seek(_endOfEntry, SeekOrigin.Begin);
			}

			return num;
		}

		public ZipEntry GetNextEntry()
		{
			if (_findRequired)
			{
				long num = SharedUtilities.FindSignature(ReadStream, 67324752);
				if (num == -1) return null;
				ReadStream.Seek(-4L, SeekOrigin.Current);
			}
			else if (_firstEntry)
			{
				ReadStream.Seek(_endOfEntry, SeekOrigin.Begin);
			}

			_currentEntry = ZipEntry.ReadEntry(_container, !_firstEntry);
			_endOfEntry = ReadStream.Position;
			_firstEntry = true;
			_needSetup = true;
			_findRequired = false;
			return _currentEntry;
		}

		protected override void Dispose(bool disposing)
		{
			if (!_closed)
			{
				if (disposing)
				{
					if (_exceptionPending) return;
					if (!_leaveUnderlyingStreamOpen) ReadStream.Dispose();
				}

				_closed = true;
			}
		}

		public override void Flush()
		{
			throw new NotSupportedException("Flush");
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotSupportedException("Write");
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			_findRequired = true;
			return ReadStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}
}