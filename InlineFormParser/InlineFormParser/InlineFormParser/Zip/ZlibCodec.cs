﻿#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:56 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;
using System.Runtime.InteropServices;

#endregion

namespace InlineFormParser.Zip
{
	[ClassInterface(ClassInterfaceType.AutoDispatch)]
	[ComVisible(true)]
	[Guid("ebc25cf6-9120-4283-b972-0e5520d0000D")]
	public sealed class ZlibCodec
	{
		internal uint _Adler32;

		public int AvailableBytesIn;

		public int AvailableBytesOut;

		public CompressionLevel CompressLevel = CompressionLevel.Default;

		internal DeflateManager dstate;
		public byte[] InputBuffer;

		internal InflateManager istate;

		public string Message;

		public int NextIn;

		public int NextOut;

		public byte[] OutputBuffer;

		public CompressionStrategy Strategy;

		public long TotalBytesIn;

		public long TotalBytesOut;

		public int WindowBits = 15;

		public ZlibCodec()
		{
		}

		public ZlibCodec(CompressionMode mode)
		{
			switch (mode)
			{
				case CompressionMode.Compress:
					if (InitializeDeflate() == 0) break;
					throw new ZlibException("Cannot initialize for deflate.");
				case CompressionMode.Decompress:
					if (InitializeInflate() == 0) break;
					throw new ZlibException("Cannot initialize for inflate.");
				default:
					throw new ZlibException("Invalid ZlibStreamFlavor.");
			}
		}

		public int Adler32 => (int) _Adler32;

		public int InitializeInflate()
		{
			return InitializeInflate(WindowBits);
		}

		public int InitializeInflate(bool expectRfc1950Header)
		{
			return InitializeInflate(WindowBits, expectRfc1950Header);
		}

		public int InitializeInflate(int windowBits)
		{
			WindowBits = windowBits;
			return InitializeInflate(windowBits, true);
		}

		public int InitializeInflate(int windowBits, bool expectRfc1950Header)
		{
			WindowBits = windowBits;
			if (dstate != null)
				throw new ZlibException("You may not call InitializeInflate() after calling InitializeDeflate().");
			istate = new InflateManager(expectRfc1950Header);
			return istate.Initialize(this, windowBits);
		}

		public int Inflate(FlushType flush)
		{
			if (istate == null) throw new ZlibException("No Inflate State!");
			return istate.Inflate(flush);
		}

		public int EndInflate()
		{
			if (istate == null) throw new ZlibException("No Inflate State!");
			int result = istate.End();
			istate = null;
			return result;
		}

		public int SyncInflate()
		{
			if (istate == null) throw new ZlibException("No Inflate State!");
			return istate.Sync();
		}

		public int InitializeDeflate()
		{
			return _InternalInitializeDeflate(true);
		}

		public int InitializeDeflate(CompressionLevel level)
		{
			CompressLevel = level;
			return _InternalInitializeDeflate(true);
		}

		public int InitializeDeflate(CompressionLevel level, bool wantRfc1950Header)
		{
			CompressLevel = level;
			return _InternalInitializeDeflate(wantRfc1950Header);
		}

		public int InitializeDeflate(CompressionLevel level, int bits)
		{
			CompressLevel = level;
			WindowBits = bits;
			return _InternalInitializeDeflate(true);
		}

		public int InitializeDeflate(CompressionLevel level, int bits, bool wantRfc1950Header)
		{
			CompressLevel = level;
			WindowBits = bits;
			return _InternalInitializeDeflate(wantRfc1950Header);
		}

		private int _InternalInitializeDeflate(bool wantRfc1950Header)
		{
			if (istate != null)
				throw new ZlibException("You may not call InitializeDeflate() after calling InitializeInflate().");
			dstate = new DeflateManager();
			dstate.WantRfc1950HeaderBytes = wantRfc1950Header;
			return dstate.Initialize(this, CompressLevel, WindowBits, Strategy);
		}

		public int Deflate(FlushType flush)
		{
			if (dstate == null) throw new ZlibException("No Deflate State!");
			return dstate.Deflate(flush);
		}

		public int EndDeflate()
		{
			if (dstate == null) throw new ZlibException("No Deflate State!");
			dstate = null;
			return 0;
		}

		public void ResetDeflate()
		{
			if (dstate == null) throw new ZlibException("No Deflate State!");
			dstate.Reset();
		}

		public int SetDeflateParams(CompressionLevel level, CompressionStrategy strategy)
		{
			if (dstate == null) throw new ZlibException("No Deflate State!");
			return dstate.SetParams(level, strategy);
		}

		public int SetDictionary(byte[] dictionary)
		{
			if (istate != null) return istate.SetDictionary(dictionary);
			if (dstate != null) return dstate.SetDictionary(dictionary);
			throw new ZlibException("No Inflate or Deflate state!");
		}

		internal void flush_pending()
		{
			int num = dstate.pendingCount;
			if (num > AvailableBytesOut) num = AvailableBytesOut;
			if (num != 0)
			{
				if (dstate.pending.Length <= dstate.nextPending || OutputBuffer.Length <= NextOut ||
				    dstate.pending.Length < dstate.nextPending + num || OutputBuffer.Length < NextOut + num)
					throw new ZlibException(
						$"Invalid State. (pending.Length={dstate.pending.Length}, pendingCount={dstate.pendingCount})");
				Array.Copy(dstate.pending, dstate.nextPending, OutputBuffer, NextOut, num);
				NextOut += num;
				dstate.nextPending += num;
				TotalBytesOut += num;
				AvailableBytesOut -= num;
				dstate.pendingCount -= num;
				if (dstate.pendingCount == 0) dstate.nextPending = 0;
			}
		}

		internal int read_buf(byte[] buf, int start, int size)
		{
			int num = AvailableBytesIn;
			if (num > size) num = size;
			if (num == 0) return 0;
			AvailableBytesIn -= num;
			if (dstate.WantRfc1950HeaderBytes) _Adler32 = Adler.Adler32(_Adler32, InputBuffer, NextIn, num);
			Array.Copy(InputBuffer, NextIn, buf, start, num);
			NextIn += num;
			TotalBytesIn += num;
			return num;
		}
	}
}