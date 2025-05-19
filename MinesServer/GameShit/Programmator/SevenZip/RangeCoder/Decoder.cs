using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.RangeCoder
{
    internal class Decoder
    {
        // Token: 0x06000665 RID: 1637 RVA: 0x0003E6E8 File Offset: 0x0003C8E8
        public void Init(Stream stream)
        {
            this.Stream = stream;
            this.Code = 0U;
            this.Range = uint.MaxValue;
            for (int i = 0; i < 5; i++)
            {
                this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
            }
        }

        // Token: 0x06000666 RID: 1638 RVA: 0x00008CCA File Offset: 0x00006ECA
        public void ReleaseStream()
        {
            this.Stream = null;
        }

        // Token: 0x06000667 RID: 1639 RVA: 0x00008CD3 File Offset: 0x00006ED3
        public void CloseStream()
        {
            this.Stream.Close();
        }

        // Token: 0x06000668 RID: 1640 RVA: 0x00008CE0 File Offset: 0x00006EE0
        public void Normalize()
        {
            while (this.Range < 16777216U)
            {
                this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
                this.Range <<= 8;
            }
        }

        // Token: 0x06000669 RID: 1641 RVA: 0x00008D1A File Offset: 0x00006F1A
        public void Normalize2()
        {
            if (this.Range < 16777216U)
            {
                this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
                this.Range <<= 8;
            }
        }

        // Token: 0x0600066A RID: 1642 RVA: 0x0003E734 File Offset: 0x0003C934
        public uint GetThreshold(uint total)
        {
            return this.Code / (this.Range /= total);
        }

        // Token: 0x0600066B RID: 1643 RVA: 0x00008D52 File Offset: 0x00006F52
        public void Decode(uint start, uint size, uint total)
        {
            this.Code -= start * this.Range;
            this.Range *= size;
            this.Normalize();
        }

        // Token: 0x0600066C RID: 1644 RVA: 0x0003E75C File Offset: 0x0003C95C
        public uint DecodeDirectBits(int numTotalBits)
        {
            uint num = this.Range;
            uint num2 = this.Code;
            uint num3 = 0U;
            for (int i = numTotalBits; i > 0; i--)
            {
                num >>= 1;
                uint num4 = num2 - num >> 31;
                num2 -= (num & num4 - 1U);
                num3 = (num3 << 1 | 1U - num4);
                if (num < 16777216U)
                {
                    num2 = (num2 << 8 | (uint)((byte)this.Stream.ReadByte()));
                    num <<= 8;
                }
            }
            this.Range = num;
            this.Code = num2;
            return num3;
        }

        // Token: 0x0600066D RID: 1645 RVA: 0x0003E7D0 File Offset: 0x0003C9D0
        public uint DecodeBit(uint size0, int numTotalBits)
        {
            uint num = (this.Range >> numTotalBits) * size0;
            uint result;
            if (this.Code < num)
            {
                result = 0U;
                this.Range = num;
            }
            else
            {
                result = 1U;
                this.Code -= num;
                this.Range -= num;
            }
            this.Normalize();
            return result;
        }

        // Token: 0x040007E4 RID: 2020
        public const uint kTopValue = 16777216U;

        // Token: 0x040007E5 RID: 2021
        public uint Range;

        // Token: 0x040007E6 RID: 2022
        public uint Code;

        // Token: 0x040007E7 RID: 2023
        public Stream Stream;
    }
}
