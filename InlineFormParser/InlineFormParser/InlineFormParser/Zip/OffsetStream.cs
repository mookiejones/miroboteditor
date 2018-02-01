#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:27 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.IO;

#endregion

namespace InlineFormParser.Zip
{
	internal class OffsetStream : Stream, IDisposable
	{
		private readonly Stream _innerStream;
		private readonly long _originalPosition;

		public OffsetStream(Stream s)
		{
			_originalPosition = s.Position;
			_innerStream = s;
		}

		public override bool CanRead => _innerStream.CanRead;

		public override bool CanSeek => _innerStream.CanSeek;

		public override bool CanWrite => false;

		public override long Length => _innerStream.Length;

		public override long Position
		{
			get => _innerStream.Position - _originalPosition;
			set => _innerStream.Position = _originalPosition + value;
		}

		void IDisposable.Dispose()
		{
			Close();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			return _innerStream.Read(buffer, offset, count);
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			throw new NotImplementedException();
		}

		public override void Flush()
		{
			_innerStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return _innerStream.Seek(_originalPosition + offset, origin) - _originalPosition;
		}

		public override void SetLength(long value)
		{
			throw new NotImplementedException();
		}

		public override void Close()
		{
			base.Close();
		}
	}
}