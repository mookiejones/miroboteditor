#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:54 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.IO;

#endregion

namespace InlineFormParser.Zip
{
	public class CrcCalculatorStream : Stream, IDisposable
	{
		private static readonly long UnsetLengthLimit = -99L;

		private readonly CRC32 _Crc32;

		internal Stream _innerStream;

		private readonly long _lengthLimit = -99L;

		public CrcCalculatorStream(Stream stream)
			: this(true, UnsetLengthLimit, stream, null)
		{
		}

		public CrcCalculatorStream(Stream stream, bool leaveOpen)
			: this(leaveOpen, UnsetLengthLimit, stream, null)
		{
		}

		public CrcCalculatorStream(Stream stream, long length)
			: this(true, length, stream, null)
		{
			if (length >= 0) return;
			throw new ArgumentException("length");
		}

		public CrcCalculatorStream(Stream stream, long length, bool leaveOpen)
			: this(leaveOpen, length, stream, null)
		{
			if (length >= 0) return;
			throw new ArgumentException("length");
		}

		public CrcCalculatorStream(Stream stream, long length, bool leaveOpen, CRC32 crc32)
			: this(leaveOpen, length, stream, crc32)
		{
			if (length >= 0) return;
			throw new ArgumentException("length");
		}

		private CrcCalculatorStream(bool leaveOpen, long length, Stream stream, CRC32 crc32)
		{
			_innerStream = stream;
			_Crc32 = crc32 ?? new CRC32();
			_lengthLimit = length;
			LeaveOpen = leaveOpen;
		}

		public long TotalBytesSlurped => _Crc32.TotalBytesRead;

		public int Crc => _Crc32.Crc32Result;

		public bool LeaveOpen { get; set; }

		public override bool CanRead => _innerStream.CanRead;

		public override bool CanSeek => false;

		public override bool CanWrite => _innerStream.CanWrite;

		public override long Length
		{
			get
			{
				if (_lengthLimit == UnsetLengthLimit) return _innerStream.Length;
				return _lengthLimit;
			}
		}

		public override long Position
		{
			get => _Crc32.TotalBytesRead;
			set => throw new NotSupportedException();
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int count2 = count;
			if (_lengthLimit != UnsetLengthLimit)
			{
				if (_Crc32.TotalBytesRead >= _lengthLimit) return 0;
				long num = _lengthLimit - _Crc32.TotalBytesRead;
				if (num < count) count2 = (int) num;
			}

			int num2 = _innerStream.Read(buffer, offset, count2);
			if (num2 > 0) _Crc32.SlurpBlock(buffer, offset, num2);
			return num2;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count > 0) _Crc32.SlurpBlock(buffer, offset, count);
			_innerStream.Write(buffer, offset, count);
		}

		public override void Flush()
		{
			_innerStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}

		public override void Close()
		{
			base.Close();
			if (!LeaveOpen) _innerStream.Close();
		}
	}
}