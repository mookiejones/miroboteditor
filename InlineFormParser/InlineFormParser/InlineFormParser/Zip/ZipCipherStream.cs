#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:15 AM
// Modified:2018:02:01:9:50 AM:

#endregion

#region Usings

using System;
using System.IO;

#endregion

namespace InlineFormParser.Zip
{
	internal class ZipCipherStream : Stream
	{
		private readonly ZipCrypto _cipher;

		private readonly CryptoMode _mode;

		private readonly Stream _s;

		public ZipCipherStream(Stream s, ZipCrypto cipher, CryptoMode mode)
		{
			_cipher = cipher;
			_s = s;
			_mode = mode;
		}

		public override bool CanRead => _mode == CryptoMode.Decrypt;

		public override bool CanSeek => false;

		public override bool CanWrite => _mode == CryptoMode.Encrypt;

		public override long Length => throw new NotSupportedException();

		public override long Position
		{
			get => throw new NotSupportedException();
			set => throw new NotSupportedException();
		}

		public override int Read(byte[] buffer, int offset, int count)
		{
			if (_mode == CryptoMode.Encrypt) throw new NotSupportedException("This stream does not encrypt via Read()");
			if (buffer == null) throw new ArgumentNullException(nameof(buffer));
			var array = new byte[count];
			var num = _s.Read(array, 0, count);
			var array2 = _cipher.DecryptMessage(array, num);
			for (var i = 0; i < num; i++) buffer[offset + i] = array2[i];
			return num;
		}

		public override void Write(byte[] buffer, int offset, int count)
		{
			if (_mode == CryptoMode.Decrypt) throw new NotSupportedException("This stream does not Decrypt via Write()");
			if (buffer == null) throw new ArgumentNullException(nameof(buffer));
			if (count != 0)
			{
				byte[] array = null;
				if (offset != 0)
				{
					array = new byte[count];
					for (var i = 0; i < count; i++) array[i] = buffer[offset + i];
				}
				else
				{
					array = buffer;
				}

				var array2 = _cipher.EncryptMessage(array, count);
				_s.Write(array2, 0, array2.Length);
			}
		}

		public override void Flush()
		{
		}

		public override long Seek(long offset, SeekOrigin origin)
		{
			throw new NotSupportedException();
		}

		public override void SetLength(long value)
		{
			throw new NotSupportedException();
		}
	}
}