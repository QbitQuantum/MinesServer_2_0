using System;
using System.IO;

namespace SevenZip.Compression.LZMA
{
	// Token: 0x020000C4 RID: 196
	public static class SevenZipHelper
	{
		// Token: 0x0600055B RID: 1371 RVA: 0x00035F54 File Offset: 0x00034154
		public static byte[] Compress(byte[] inputBytes)
		{
			MemoryStream memoryStream = new MemoryStream(inputBytes);
			MemoryStream memoryStream2 = new MemoryStream();
			Encoder encoder = new Encoder();
			encoder.SetCoderProperties(SevenZipHelper.propIDs, SevenZipHelper.properties);
			encoder.WriteCoderProperties(memoryStream2);
			long length = memoryStream.Length;
			for (int i = 0; i < 8; i++)
			{
				memoryStream2.WriteByte((byte)(length >> 8 * i));
			}
			encoder.Code(memoryStream, memoryStream2, -1L, -1L, null);
			return memoryStream2.ToArray();
		}

		// Token: 0x0600055C RID: 1372 RVA: 0x00035FC8 File Offset: 0x000341C8
		public static byte[] Decompress(byte[] inputBytes)
		{
			MemoryStream memoryStream = new MemoryStream(inputBytes);
			Decoder decoder = new Decoder();
			memoryStream.Seek(0L, SeekOrigin.Begin);
			MemoryStream memoryStream2 = new MemoryStream();
			byte[] array = new byte[5];
			if (memoryStream.Read(array, 0, 5) != 5)
			{
				throw new Exception("input .lzma is too short");
			}
			long num = 0L;
			for (int i = 0; i < 8; i++)
			{
				int num2 = memoryStream.ReadByte();
				if (num2 < 0)
				{
					throw new Exception("Can't Read 1");
				}
				num |= (long)((long)((ulong)((byte)num2)) << 8 * i);
			}
			decoder.SetDecoderProperties(array);
			long inSize = memoryStream.Length - memoryStream.Position;
			decoder.Code(memoryStream, memoryStream2, inSize, num, null);
			return memoryStream2.ToArray();
		}

		// Token: 0x04000721 RID: 1825
		private static int dictionary = 8388608;

		// Token: 0x04000722 RID: 1826
		private static bool eos = false;

		// Token: 0x04000723 RID: 1827
		private static CoderPropID[] propIDs = new CoderPropID[]
		{
			CoderPropID.DictionarySize,
			CoderPropID.PosStateBits,
			CoderPropID.LitContextBits,
			CoderPropID.LitPosBits,
			CoderPropID.Algorithm,
			CoderPropID.NumFastBytes,
			CoderPropID.MatchFinder,
			CoderPropID.EndMarker
		};

		// Token: 0x04000724 RID: 1828
		private static object[] properties = new object[]
		{
			SevenZipHelper.dictionary,
			2,
			3,
			0,
			2,
			128,
			"bt4",
			SevenZipHelper.eos
		};
	}
}
