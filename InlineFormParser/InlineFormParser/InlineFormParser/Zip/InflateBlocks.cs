#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:9:03 AM
// Modified:2018:02:01:9:49 AM:

#endregion

#region Usings

using System;

#endregion

namespace InlineFormParser.Zip
{
	internal sealed class InflateBlocks
	{
		private const int MANY = 1440;

		internal static readonly int[] border = new int[19]
		{
			16,
			17,
			18,
			0,
			8,
			7,
			9,
			6,
			10,
			5,
			11,
			4,
			12,
			3,
			13,
			2,
			14,
			1,
			15
		};

		internal ZlibCodec _codec;

		internal int[] bb = new int[1];

		internal int bitb;

		internal int bitk;

		internal int[] blens;

		internal uint check;

		internal object checkfn;

		internal InflateCodes codes = new InflateCodes();

		internal int end;

		internal int[] hufts;

		internal int index;

		internal InfTree inftree = new InfTree();

		internal int last;

		internal int left;

		private InflateBlockMode mode;

		internal int readAt;

		internal int table;

		internal int[] tb = new int[1];

		internal byte[] window;

		internal int writeAt;

		internal InflateBlocks(ZlibCodec codec, object checkfn, int w)
		{
			_codec = codec;
			hufts = new int[4320];
			window = new byte[w];
			end = w;
			this.checkfn = checkfn;
			mode = InflateBlockMode.TYPE;
			Reset();
		}

		internal uint Reset()
		{
			uint result = check;
			mode = InflateBlockMode.TYPE;
			bitk = 0;
			bitb = 0;
			readAt = writeAt = 0;
			if (checkfn != null) _codec._Adler32 = check = Adler.Adler32(0u, null, 0, 0);
			return result;
		}

		internal int Process(int r)
		{
			int num = _codec.NextIn;
			int num2 = _codec.AvailableBytesIn;
			int num3 = bitb;
			int num4 = bitk;
			int num5 = writeAt;
			int num6 = num5 < readAt ? readAt - num5 - 1 : end - num5;
			while (true)
				switch (mode)
				{
					case InflateBlockMode.TYPE:
					{
						while (num4 < 3)
						{
							if (num2 != 0)
							{
								r = 0;
								num2--;
								num3 |= (_codec.InputBuffer[num++] & 0xFF) << num4;
								num4 += 8;
								continue;
							}

							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						int num8 = num3 & 7;
						last = num8 & 1;
						switch ((uint) num8 >> 1)
						{
							case 0u:
								num3 >>= 3;
								num4 -= 3;
								num8 = num4 & 7;
								num3 >>= num8;
								num4 -= num8;
								mode = InflateBlockMode.LENS;
								break;
							case 1u:
							{
								int[] array = new int[1];
								int[] array2 = new int[1];
								int[][] array3 = new int[1][];
								int[][] array4 = new int[1][];
								InfTree.inflate_trees_fixed(array, array2, array3, array4, _codec);
								codes.Init(array[0], array2[0], array3[0], 0, array4[0], 0);
								num3 >>= 3;
								num4 -= 3;
								mode = InflateBlockMode.CODES;
								break;
							}
							case 2u:
								num3 >>= 3;
								num4 -= 3;
								mode = InflateBlockMode.TABLE;
								break;
							case 3u:
								num3 >>= 3;
								num4 -= 3;
								mode = InflateBlockMode.BAD;
								_codec.Message = "invalid block type";
								r = -3;
								bitb = num3;
								bitk = num4;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num5;
								return Flush(r);
						}

						break;
					}
					case InflateBlockMode.LENS:
						while (num4 < 32)
						{
							if (num2 != 0)
							{
								r = 0;
								num2--;
								num3 |= (_codec.InputBuffer[num++] & 0xFF) << num4;
								num4 += 8;
								continue;
							}

							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						if (((~num3 >> 16) & 0xFFFF) != (num3 & 0xFFFF))
						{
							mode = InflateBlockMode.BAD;
							_codec.Message = "invalid stored block lengths";
							r = -3;
							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						left = num3 & 0xFFFF;
						num3 = num4 = 0;
						mode = (InflateBlockMode) (left != 0 ? 2 : (last != 0 ? 7 : 0));
						break;
					case InflateBlockMode.STORED:
					{
						if (num2 == 0)
						{
							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						if (num6 == 0)
						{
							if (num5 == end && readAt != 0)
							{
								num5 = 0;
								num6 = num5 < readAt ? readAt - num5 - 1 : end - num5;
							}

							if (num6 == 0)
							{
								writeAt = num5;
								r = Flush(r);
								num5 = writeAt;
								num6 = num5 < readAt ? readAt - num5 - 1 : end - num5;
								if (num5 == end && readAt != 0)
								{
									num5 = 0;
									num6 = num5 < readAt ? readAt - num5 - 1 : end - num5;
								}

								if (num6 == 0)
								{
									bitb = num3;
									bitk = num4;
									_codec.AvailableBytesIn = num2;
									_codec.TotalBytesIn += num - _codec.NextIn;
									_codec.NextIn = num;
									writeAt = num5;
									return Flush(r);
								}
							}
						}

						r = 0;
						int num8 = left;
						if (num8 > num2) num8 = num2;
						if (num8 > num6) num8 = num6;
						Array.Copy(_codec.InputBuffer, num, window, num5, num8);
						num += num8;
						num2 -= num8;
						num5 += num8;
						num6 -= num8;
						if ((left -= num8) == 0) mode = (InflateBlockMode) (last != 0 ? 7 : 0);
						break;
					}
					case InflateBlockMode.TABLE:
					{
						while (num4 < 14)
						{
							if (num2 != 0)
							{
								r = 0;
								num2--;
								num3 |= (_codec.InputBuffer[num++] & 0xFF) << num4;
								num4 += 8;
								continue;
							}

							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						int num8 = table = num3 & 0x3FFF;
						if ((num8 & 0x1F) <= 29 && ((num8 >> 5) & 0x1F) <= 29)
						{
							num8 = 258 + (num8 & 0x1F) + ((num8 >> 5) & 0x1F);
							if (blens == null || blens.Length < num8)
								blens = new int[num8];
							else
								Array.Clear(blens, 0, num8);
							num3 >>= 14;
							num4 -= 14;
							index = 0;
							mode = InflateBlockMode.BTREE;
							goto case InflateBlockMode.BTREE;
						}

						mode = InflateBlockMode.BAD;
						_codec.Message = "too many length or distance symbols";
						r = -3;
						bitb = num3;
						bitk = num4;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num5;
						return Flush(r);
					}
					case InflateBlockMode.BTREE:
					{
						while (index < 4 + (table >> 10))
						{
							while (num4 < 3)
							{
								if (num2 != 0)
								{
									r = 0;
									num2--;
									num3 |= (_codec.InputBuffer[num++] & 0xFF) << num4;
									num4 += 8;
									continue;
								}

								bitb = num3;
								bitk = num4;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num5;
								return Flush(r);
							}

							blens[border[index++]] = num3 & 7;
							num3 >>= 3;
							num4 -= 3;
						}

						while (index < 19) blens[border[index++]] = 0;
						bb[0] = 7;
						int num8 = inftree.inflate_trees_bits(blens, bb, tb, hufts, _codec);
						if (num8 != 0)
						{
							r = num8;
							if (r == -3)
							{
								blens = null;
								mode = InflateBlockMode.BAD;
							}

							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						index = 0;
						mode = InflateBlockMode.DTREE;
						goto case InflateBlockMode.DTREE;
					}
					case InflateBlockMode.DTREE:
					{
						int num8;
						while (true)
						{
							num8 = table;
							if (index >= 258 + (num8 & 0x1F) + ((num8 >> 5) & 0x1F)) break;
							num8 = bb[0];
							while (num4 < num8)
							{
								if (num2 != 0)
								{
									r = 0;
									num2--;
									num3 |= (_codec.InputBuffer[num++] & 0xFF) << num4;
									num4 += 8;
									continue;
								}

								bitb = num3;
								bitk = num4;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num5;
								return Flush(r);
							}

							num8 = hufts[(tb[0] + (num3 & InternalInflateConstants.InflateMask[num8])) * 3 + 1];
							int num12 = hufts[(tb[0] + (num3 & InternalInflateConstants.InflateMask[num8])) * 3 + 2];
							if (num12 < 16)
							{
								num3 >>= num8;
								num4 -= num8;
								blens[index++] = num12;
								continue;
							}

							int num13 = num12 == 18 ? 7 : num12 - 14;
							int num14 = num12 == 18 ? 11 : 3;
							while (num4 < num8 + num13)
							{
								if (num2 != 0)
								{
									r = 0;
									num2--;
									num3 |= (_codec.InputBuffer[num++] & 0xFF) << num4;
									num4 += 8;
									continue;
								}

								bitb = num3;
								bitk = num4;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num5;
								return Flush(r);
							}

							num3 >>= num8;
							num4 -= num8;
							num14 += num3 & InternalInflateConstants.InflateMask[num13];
							num3 >>= num13;
							num4 -= num13;
							num13 = index;
							num8 = table;
							if (num13 + num14 <= 258 + (num8 & 0x1F) + ((num8 >> 5) & 0x1F) && (num12 != 16 || num13 >= 1))
							{
								num12 = num12 == 16 ? blens[num13 - 1] : 0;
								do
								{
									blens[num13++] = num12;
								} while (--num14 != 0);

								index = num13;
								continue;
							}

							blens = null;
							mode = InflateBlockMode.BAD;
							_codec.Message = "invalid bit length repeat";
							r = -3;
							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						tb[0] = -1;
						int[] array5 = new int[1]
						{
							9
						};
						int[] array6 = new int[1]
						{
							6
						};
						int[] array7 = new int[1];
						int[] array8 = new int[1];
						num8 = table;
						num8 = inftree.inflate_trees_dynamic(257 + (num8 & 0x1F), 1 + ((num8 >> 5) & 0x1F), blens, array5, array6, array7,
							array8, hufts, _codec);
						switch (num8)
						{
							case -3:
								blens = null;
								mode = InflateBlockMode.BAD;
								goto default;
							default:
								r = num8;
								bitb = num3;
								bitk = num4;
								_codec.AvailableBytesIn = num2;
								_codec.TotalBytesIn += num - _codec.NextIn;
								_codec.NextIn = num;
								writeAt = num5;
								return Flush(r);
							case 0:
								break;
						}

						codes.Init(array5[0], array6[0], hufts, array7[0], hufts, array8[0]);
						mode = InflateBlockMode.CODES;
						goto case InflateBlockMode.CODES;
					}
					case InflateBlockMode.CODES:
						bitb = num3;
						bitk = num4;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num5;
						r = codes.Process(this, r);
						if (r != 1) return Flush(r);
						r = 0;
						num = _codec.NextIn;
						num2 = _codec.AvailableBytesIn;
						num3 = bitb;
						num4 = bitk;
						num5 = writeAt;
						num6 = num5 < readAt ? readAt - num5 - 1 : end - num5;
						if (last == 0)
						{
							mode = InflateBlockMode.TYPE;
							break;
						}

						mode = InflateBlockMode.DRY;
						goto case InflateBlockMode.DRY;
					case InflateBlockMode.DRY:
						writeAt = num5;
						r = Flush(r);
						num5 = writeAt;
						num6 = num5 < readAt ? readAt - num5 - 1 : end - num5;
						if (readAt != writeAt)
						{
							bitb = num3;
							bitk = num4;
							_codec.AvailableBytesIn = num2;
							_codec.TotalBytesIn += num - _codec.NextIn;
							_codec.NextIn = num;
							writeAt = num5;
							return Flush(r);
						}

						mode = InflateBlockMode.DONE;
						goto case InflateBlockMode.DONE;
					case InflateBlockMode.DONE:
						r = 1;
						bitb = num3;
						bitk = num4;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num5;
						return Flush(r);
					case InflateBlockMode.BAD:
						r = -3;
						bitb = num3;
						bitk = num4;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num5;
						return Flush(r);
					default:
						r = -2;
						bitb = num3;
						bitk = num4;
						_codec.AvailableBytesIn = num2;
						_codec.TotalBytesIn += num - _codec.NextIn;
						_codec.NextIn = num;
						writeAt = num5;
						return Flush(r);
				}
		}

		internal void Free()
		{
			Reset();
			window = null;
			hufts = null;
		}

		internal void SetDictionary(byte[] d, int start, int n)
		{
			Array.Copy(d, start, window, 0, n);
			readAt = writeAt = n;
		}

		internal int SyncPoint()
		{
			if (mode != InflateBlockMode.LENS) return 0;
			return 1;
		}

		internal int Flush(int r)
		{
			for (int i = 0; i < 2; i++)
			{
				int num = i != 0 ? writeAt - readAt : (readAt <= writeAt ? writeAt : end) - readAt;
				if (num == 0)
				{
					if (r == -5) r = 0;
					return r;
				}

				if (num > _codec.AvailableBytesOut) num = _codec.AvailableBytesOut;
				if (num != 0 && r == -5) r = 0;
				_codec.AvailableBytesOut -= num;
				_codec.TotalBytesOut += num;
				if (checkfn != null) _codec._Adler32 = check = Adler.Adler32(check, window, readAt, num);
				Array.Copy(window, readAt, _codec.OutputBuffer, _codec.NextOut, num);
				_codec.NextOut += num;
				readAt += num;
				if (readAt == end && i == 0)
				{
					readAt = 0;
					if (writeAt == end) writeAt = 0;
				}
				else
				{
					i++;
				}
			}

			return r;
		}

		private enum InflateBlockMode
		{
			TYPE,
			LENS,
			STORED,
			TABLE,
			BTREE,
			DTREE,
			CODES,
			DRY,
			DONE,
			BAD
		}
	}
}