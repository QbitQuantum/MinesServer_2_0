using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZMA
{
    public static class SevenZipHelper
    {
        // Token: 0x0600061D RID: 1565 RVA: 0x0003D38C File Offset: 0x0003B58C
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

        // Token: 0x0600061E RID: 1566 RVA: 0x0003D400 File Offset: 0x0003B600
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

        // Token: 0x040007AD RID: 1965
        private static int dictionary = 8388608;

        // Token: 0x040007AE RID: 1966
        private static bool eos = false;

        // Token: 0x040007AF RID: 1967
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

        // Token: 0x040007B0 RID: 1968
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
