using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.RangeCoder
{
    internal struct BitDecoder
    {
        // Token: 0x0600064E RID: 1614 RVA: 0x00008BC4 File Offset: 0x00006DC4
        public void UpdateModel(int numMoveBits, uint symbol)
        {
            if (symbol == 0U)
            {
                this.Prob += 2048U - this.Prob >> numMoveBits;
                return;
            }
            this.Prob -= this.Prob >> numMoveBits;
        }

        // Token: 0x0600064F RID: 1615 RVA: 0x00008C00 File Offset: 0x00006E00
        public void Init()
        {
            this.Prob = 1024U;
        }

        // Token: 0x06000650 RID: 1616 RVA: 0x0003E230 File Offset: 0x0003C430
        public uint Decode(RangeCoder.Decoder rangeDecoder)
        {
            uint num = (rangeDecoder.Range >> 11) * this.Prob;
            if (rangeDecoder.Code < num)
            {
                rangeDecoder.Range = num;
                this.Prob += 2048U - this.Prob >> 5;
                if (rangeDecoder.Range < 16777216U)
                {
                    rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
                    rangeDecoder.Range <<= 8;
                }
                return 0U;
            }
            rangeDecoder.Range -= num;
            rangeDecoder.Code -= num;
            this.Prob -= this.Prob >> 5;
            if (rangeDecoder.Range < 16777216U)
            {
                rangeDecoder.Code = (rangeDecoder.Code << 8 | (uint)((byte)rangeDecoder.Stream.ReadByte()));
                rangeDecoder.Range <<= 8;
            }
            return 1U;
        }

        // Token: 0x040007D5 RID: 2005
        public const int kNumBitModelTotalBits = 11;

        // Token: 0x040007D6 RID: 2006
        public const uint kBitModelTotal = 2048U;

        // Token: 0x040007D7 RID: 2007
        private const int kNumMoveBits = 5;

        // Token: 0x040007D8 RID: 2008
        private uint Prob;
    }
}
