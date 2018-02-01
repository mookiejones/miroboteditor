#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:48 AM
// Modified:2018:02:01:9:49 AM:

#endregion


#region Usings

using System;
using System.IO;

#endregion

namespace InlineFormParser.Zip
{
	public class CountingStream : Stream
	{
		private readonly long _initialOffset;

		public CountingStream(Stream stream)
		{
			WrappedStream = stream;
			try
			{
				_initialOffset = WrappedStream.Position;
			}
			catch
			{
				_initialOffset = 0L;
			}
		}

		public Stream WrappedStream { get; }

		public long BytesWritten { get; private set; }

		public long BytesRead { get; private set; }

		public override bool CanRead => WrappedStream.CanRead;

		public override bool CanSeek => WrappedStream.CanSeek;

		public override bool CanWrite => WrappedStream.CanWrite;

		public override long Length => WrappedStream.Length;

		public long ComputedPosition => _initialOffset + BytesWritten;

		public override long Position
		{
			get => WrappedStream.Position;
			set => WrappedStream.Seek(value, SeekOrigin.Begin);
		}

		public void Adjust(long delta)
		{
			BytesWritten -= delta;
			if (BytesWritten < 0) throw new InvalidOperationException();
			if (WrappedStream is CountingStream) ((CountingStream) WrappedStream).Adjust(delta);
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			int num = WrappedStream.Read(buffer, offset, count);
			BytesRead += num;
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (count != 0)
			{
				WrappedStream.Write(buffer, offset, count);
				BytesWritten += count;
			}
		}

		public override void Flush()
		{
			WrappedStream.Flush();
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			return WrappedStream.Seek(offset, origin);
		}

		public override void SetLength(long value)
		{
			WrappedStream.SetLength(value);
		}
	}
}