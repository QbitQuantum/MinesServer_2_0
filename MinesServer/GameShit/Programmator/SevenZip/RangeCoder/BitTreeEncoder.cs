using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.RangeCoder
{
    internal struct BitTreeEncoder
    {
        // Token: 0x0600065D RID: 1629 RVA: 0x00008CB0 File Offset: 0x00006EB0
        public BitTreeEncoder(int numBitLevels)
        {
            this.NumBitLevels = numBitLevels;
            this.Models = new BitEncoder[1 << numBitLevels];
        }

        // Token: 0x0600065E RID: 1630 RVA: 0x0003E51C File Offset: 0x0003C71C
        public void Init()
        {
            uint num = 1U;
            while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
            {
                this.Models[(int)num].Init();
                num += 1U;
            }
        }

        // Token: 0x0600065F RID: 1631 RVA: 0x0003E554 File Offset: 0x0003C754
        public void Encode(Encoder rangeEncoder, uint symbol)
        {
            uint num = 1U;
            int i = this.NumBitLevels;
            while (i > 0)
            {
                i--;
                uint num2 = symbol >> i & 1U;
                this.Models[(int)num].Encode(rangeEncoder, num2);
                num = (num << 1 | num2);
            }
        }

        // Token: 0x06000660 RID: 1632 RVA: 0x0003E598 File Offset: 0x0003C798
        public void ReverseEncode(Encoder rangeEncoder, uint symbol)
        {
            uint num = 1U;
            uint num2 = 0U;
            while ((ulong)num2 < (ulong)((long)this.NumBitLevels))
            {
                uint num3 = symbol & 1U;
                this.Models[(int)num].Encode(rangeEncoder, num3);
                num = (num << 1 | num3);
                symbol >>= 1;
                num2 += 1U;
            }
        }

        // Token: 0x06000661 RID: 1633 RVA: 0x0003E5DC File Offset: 0x0003C7DC
        public uint GetPrice(uint symbol)
        {
            uint num = 0U;
            uint num2 = 1U;
            int i = this.NumBitLevels;
            while (i > 0)
            {
                i--;
                uint num3 = symbol >> i & 1U;
                num += this.Models[(int)num2].GetPrice(num3);
                num2 = (num2 << 1) + num3;
            }
            return num;
        }

        // Token: 0x06000662 RID: 1634 RVA: 0x0003E624 File Offset: 0x0003C824
        public uint ReverseGetPrice(uint symbol)
        {
            uint num = 0U;
            uint num2 = 1U;
            for (int i = this.NumBitLevels; i > 0; i--)
            {
                uint num3 = symbol & 1U;
                symbol >>= 1;
                num += this.Models[(int)num2].GetPrice(num3);
                num2 = (num2 << 1 | num3);
            }
            return num;
        }

        // Token: 0x06000663 RID: 1635 RVA: 0x0003E66C File Offset: 0x0003C86C
        public static uint ReverseGetPrice(BitEncoder[] Models, uint startIndex, int NumBitLevels, uint symbol)
        {
            uint num = 0U;
            uint num2 = 1U;
            for (int i = NumBitLevels; i > 0; i--)
            {
                uint num3 = symbol & 1U;
                symbol >>= 1;
                num += Models[(int)(startIndex + num2)].GetPrice(num3);
                num2 = (num2 << 1 | num3);
            }
            return num;
        }

        // Token: 0x06000664 RID: 1636 RVA: 0x0003E6AC File Offset: 0x0003C8AC
        public static void ReverseEncode(BitEncoder[] Models, uint startIndex, Encoder rangeEncoder, int NumBitLevels, uint symbol)
        {
            uint num = 1U;
            for (int i = 0; i < NumBitLevels; i++)
            {
                uint num2 = symbol & 1U;
                Models[(int)(startIndex + num)].Encode(rangeEncoder, num2);
                num = (num << 1 | num2);
                symbol >>= 1;
            }
        }

        // Token: 0x040007E2 RID: 2018
        private BitEncoder[] Models;

        // Token: 0x040007E3 RID: 2019
        private int NumBitLevels;
    }
}
