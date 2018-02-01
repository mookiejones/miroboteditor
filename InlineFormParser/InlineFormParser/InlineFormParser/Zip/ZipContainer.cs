#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:51 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.IO;
using System.Text;

#endregion

namespace InlineFormParser.Zip
{
	internal class ZipContainer
	{
		private readonly ZipInputStream _zis;

		public ZipContainer(object o)
		{
			ZipFile = o as ZipFile;
			ZipOutputStream = o as ZipOutputStream;
			_zis = o as ZipInputStream;
		}

		public ZipFile ZipFile { get; }

		public ZipOutputStream ZipOutputStream { get; }

		public string Name
		{
			get
			{
				if (ZipFile != null) return ZipFile.Name;
				if (_zis != null) throw new NotSupportedException();
				return ZipOutputStream.Name;
			}
		}

		public string Password
		{
			get
			{
				if (ZipFile != null) return ZipFile._Password;
				if (_zis != null) return _zis._Password;
				return ZipOutputStream._password;
			}
		}

		public Zip64Option Zip64
		{
			get
			{
				if (ZipFile != null) return ZipFile._zip64;
				if (_zis != null) throw new NotSupportedException();
				return ZipOutputStream._zip64;
			}
		}

		public int BufferSize
		{
			get
			{
				if (ZipFile != null) return ZipFile.BufferSize;
				if (_zis != null) throw new NotSupportedException();
				return 0;
			}
		}

		public ParallelDeflateOutputStream ParallelDeflater
		{
			get
			{
				if (ZipFile != null) return ZipFile.ParallelDeflater;
				if (_zis != null) return null;
				return ZipOutputStream.ParallelDeflater;
			}
			set
			{
				if (ZipFile != null)
					ZipFile.ParallelDeflater = value;
				else if (ZipOutputStream != null)
					ZipOutputStream.ParallelDeflater = value;
			}
		}

		public long ParallelDeflateThreshold
		{
			get
			{
				if (ZipFile != null) return ZipFile.ParallelDeflateThreshold;
				return ZipOutputStream.ParallelDeflateThreshold;
			}
		}

		public int ParallelDeflateMaxBufferPairs
		{
			get
			{
				if (ZipFile != null) return ZipFile.ParallelDeflateMaxBufferPairs;
				return ZipOutputStream.ParallelDeflateMaxBufferPairs;
			}
		}

		public int CodecBufferSize
		{
			get
			{
				if (ZipFile != null) return ZipFile.CodecBufferSize;
				if (_zis != null) return _zis.CodecBufferSize;
				return ZipOutputStream.CodecBufferSize;
			}
		}

		public CompressionStrategy Strategy
		{
			get
			{
				if (ZipFile != null) return ZipFile.Strategy;
				return ZipOutputStream.Strategy;
			}
		}

		public Zip64Option UseZip64WhenSaving
		{
			get
			{
				if (ZipFile != null) return ZipFile.UseZip64WhenSaving;
				return ZipOutputStream.EnableZip64;
			}
		}

		public Encoding AlternateEncoding
		{
			get
			{
				if (ZipFile != null) return ZipFile.AlternateEncoding;
				if (ZipOutputStream != null) return ZipOutputStream.AlternateEncoding;
				return null;
			}
		}

		public Encoding DefaultEncoding
		{
			get
			{
				if (ZipFile != null) return ZipFile.DefaultEncoding;
				if (ZipOutputStream != null) return ZipOutputStream.DefaultEncoding;
				return null;
			}
		}

		public ZipOption AlternateEncodingUsage
		{
			get
			{
				if (ZipFile != null) return ZipFile.AlternateEncodingUsage;
				if (ZipOutputStream != null) return ZipOutputStream.AlternateEncodingUsage;
				return ZipOption.Default;
			}
		}

		public Stream ReadStream
		{
			get
			{
				if (ZipFile != null) return ZipFile.ReadStream;
				return _zis.ReadStream;
			}
		}
	}
}