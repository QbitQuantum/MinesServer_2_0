using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.RangeCoder
{
    internal struct BitTreeDecoder
    {
        // Token: 0x06000658 RID: 1624 RVA: 0x00008C96 File Offset: 0x00006E96
        public BitTreeDecoder(int numBitLevels)
        {
            this.NumBitLevels = numBitLevels;
            this.Models = new BitDecoder[1 << numBitLevels];
        }

        // Token: 0x06000659 RID: 1625 RVA: 0x0003E418 File Offset: 0x0003C618
        public void Init()
        {
            uint num = 1U;
            while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
            {
                this.Models[(int)num].Init();
                num += 1U;
            }
        }

        // Token: 0x0600065A RID: 1626 RVA: 0x0003E450 File Offset: 0x0003C650
        public uint Decode(Decoder rangeDecoder)
        {
            uint num = 1U;
            for (int i = this.NumBitLevels; i > 0; i--)
            {
                num = (num << 1) + this.Models[(int)num].Decode(rangeDecoder);
            }
            return num - (1U << this.NumBitLevels);
        }

        // Token: 0x0600065B RID: 1627 RVA: 0x0003E494 File Offset: 0x0003C694
        public uint ReverseDecode(Decoder rangeDecoder)
        {
            uint num = 1U;
            uint num2 = 0U;
            for (int i = 0; i < this.NumBitLevels; i++)
            {
                uint num3 = this.Models[(int)num].Decode(rangeDecoder);
                num <<= 1;
                num += num3;
                num2 |= num3 << i;
            }
            return num2;
        }

        // Token: 0x0600065C RID: 1628 RVA: 0x0003E4DC File Offset: 0x0003C6DC
        public static uint ReverseDecode(BitDecoder[] Models, uint startIndex, Decoder rangeDecoder, int NumBitLevels)
        {
            uint num = 1U;
            uint num2 = 0U;
            for (int i = 0; i < NumBitLevels; i++)
            {
                uint num3 = Models[(int)(startIndex + num)].Decode(rangeDecoder);
                num <<= 1;
                num += num3;
                num2 |= num3 << i;
            }
            return num2;
        }

        // Token: 0x040007E0 RID: 2016
        private BitDecoder[] Models;

        // Token: 0x040007E1 RID: 2017
        private int NumBitLevels;
    }
}
