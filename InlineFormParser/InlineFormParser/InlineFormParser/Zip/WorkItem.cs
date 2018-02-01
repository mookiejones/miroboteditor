#region Project Info

// Created By Charles.Berman
// UserName: Berman, Charles
// 
// Solution: miRobotEditor
// 
// Project: InlineFormParser
// Created: 2018:02:01:8:56 AM
// Modified:2018:02:01:9:49 AM:

#endregion

namespace InlineFormParser.Zip
{
	internal class WorkItem
	{
		public byte[] buffer;

		public byte[] compressed;

		public int compressedBytesAvailable;

		public ZlibCodec compressor;

		public int crc;

		public int index;

		public int inputBytesAvailable;

		public int ordinal;

		public WorkItem(int size, CompressionLevel compressLevel, CompressionStrategy strategy, int ix)
		{
			buffer = new byte[size];
			int num = size + (size / 32768 + 1) * 5 * 2;
			compressed = new byte[num];
			compressor = new ZlibCodec();
			compressor.InitializeDeflate(compressLevel, false);
			compressor.OutputBuffer = compressed;
			compressor.InputBuffer = buffer;
			index = ix;
		}
	}
}