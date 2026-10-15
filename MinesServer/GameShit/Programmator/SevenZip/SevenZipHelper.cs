using System;
using System.IO;

namespace SevenZip.Compression.LZMA
{
    public static class SevenZipHelper
    {
        private static int dictionary = 1 << 23;
        private static bool eos = false;

        private static CoderPropID[] propIDs =
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

        private static object[] properties =
        {
            dictionary,
            2,          // PosStateBits
            3,          // LitContextBits
            0,          // LitPosBits
            2,          // Algorithm
            32,         // NumFastBytes  ← ВАЖНО: 32, а не 128
            "bt4",      // MatchFinder
            eos         // EndMarker
        };

        public static byte[] Compress(byte[] inputBytes)
        {
            MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            Encoder encoder = new Encoder();
            encoder.SetCoderProperties(propIDs, properties);
            encoder.WriteCoderProperties(outStream);
            long fileSize = inStream.Length;
            for (int i = 0; i < 8; i++)
                outStream.WriteByte((byte)(fileSize >> (8 * i)));
            encoder.Code(inStream, outStream, -1, -1, null);
            return outStream.ToArray();
        }

        public static byte[] Decompress(byte[] inputBytes)
        {
            MemoryStream inStream = new MemoryStream(inputBytes);
            MemoryStream outStream = new MemoryStream();
            byte[] properties = new byte[5];
            if (inStream.Read(properties, 0, 5) != 5)
                throw new Exception("input .lzma is too short");

            Decoder decoder = new Decoder();
            decoder.SetDecoderProperties(properties);

            long outSize = 0;
            for (int i = 0; i < 8; i++)
            {
                int b = inStream.ReadByte();
                if (b < 0)
                    throw new Exception("Can't Read 1");
                outSize |= ((long)(byte)b) << (8 * i);
            }

            long compressedSize = inStream.Length - inStream.Position;
            decoder.Code(inStream, outStream, compressedSize, outSize, null);
            return outStream.ToArray();
        }
    }
}