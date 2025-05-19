using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.RangeCoder
{
    internal struct BitEncoder
    {
        // Token: 0x06000651 RID: 1617 RVA: 0x00008C0D File Offset: 0x00006E0D
        public void Init()
        {
            this.Prob = 1024U;
        }

        // Token: 0x06000652 RID: 1618 RVA: 0x00008C1A File Offset: 0x00006E1A
        public void UpdateModel(uint symbol)
        {
            if (symbol == 0U)
            {
                this.Prob += 2048U - this.Prob >> 5;
                return;
            }
            this.Prob -= this.Prob >> 5;
        }

        // Token: 0x06000653 RID: 1619 RVA: 0x0003E31C File Offset: 0x0003C51C
        public void Encode(RangeCoder.Encoder encoder, uint symbol)
        {
            uint num = (encoder.Range >> 11) * this.Prob;
            if (symbol == 0U)
            {
                encoder.Range = num;
                this.Prob += 2048U - this.Prob >> 5;
            }
            else
            {
                encoder.Low += (ulong)num;
                encoder.Range -= num;
                this.Prob -= this.Prob >> 5;
            }
            if (encoder.Range < 16777216U)
            {
                encoder.Range <<= 8;
                encoder.ShiftLow();
            }
        }

        // Token: 0x06000654 RID: 1620 RVA: 0x0003E3B4 File Offset: 0x0003C5B4
        static BitEncoder()
        {
            for (int i = 8; i >= 0; i--)
            {
                uint num = 1U << 9 - i - 1;
                uint num2 = 1U << 9 - i;
                for (uint num3 = num; num3 < num2; num3 += 1U)
                {
                    BitEncoder.ProbPrices[(int)num3] = (uint)((i << 6) + (int)(num2 - num3 << 6 >> 9 - i - 1));
                }
            }
        }

        // Token: 0x06000655 RID: 1621 RVA: 0x00008C50 File Offset: 0x00006E50
        public uint GetPrice(uint symbol)
        {
            return BitEncoder.ProbPrices[(int)(checked((IntPtr)((unchecked((ulong)(this.Prob - symbol) ^ (ulong)((long)(0U - symbol))) & 2047UL) >> 2)))];
        }

        // Token: 0x06000656 RID: 1622 RVA: 0x00008C70 File Offset: 0x00006E70
        public uint GetPrice0()
        {
            return BitEncoder.ProbPrices[(int)(this.Prob >> 2)];
        }

        // Token: 0x06000657 RID: 1623 RVA: 0x00008C80 File Offset: 0x00006E80
        public uint GetPrice1()
        {
            return BitEncoder.ProbPrices[(int)(2048U - this.Prob >> 2)];
        }

        // Token: 0x040007D9 RID: 2009
        public const int kNumBitModelTotalBits = 11;

        // Token: 0x040007DA RID: 2010
        public const uint kBitModelTotal = 2048U;

        // Token: 0x040007DB RID: 2011
        private const int kNumMoveBits = 5;

        // Token: 0x040007DC RID: 2012
        private const int kNumMoveReducingBits = 2;

        // Token: 0x040007DD RID: 2013
        public const int kNumBitPriceShiftBits = 6;

        // Token: 0x040007DE RID: 2014
        private uint Prob;

        // Token: 0x040007DF RID: 2015
        private static uint[] ProbPrices = new uint[512];
    }
}
