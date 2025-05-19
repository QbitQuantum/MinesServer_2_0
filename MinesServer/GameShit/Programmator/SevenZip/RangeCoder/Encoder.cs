using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.RangeCoder
{
    internal class Encoder
    {
        // Token: 0x0600066F RID: 1647 RVA: 0x00008D7D File Offset: 0x00006F7D
        public void SetStream(Stream stream)
        {
            this.Stream = stream;
        }

        // Token: 0x06000670 RID: 1648 RVA: 0x00008D86 File Offset: 0x00006F86
        public void ReleaseStream()
        {
            this.Stream = null;
        }

        // Token: 0x06000671 RID: 1649 RVA: 0x00008D8F File Offset: 0x00006F8F
        public void Init()
        {
            this.StartPosition = this.Stream.Position;
            this.Low = 0UL;
            this.Range = uint.MaxValue;
            this._cacheSize = 1U;
            this._cache = 0;
        }

        // Token: 0x06000672 RID: 1650 RVA: 0x0003E824 File Offset: 0x0003CA24
        public void FlushData()
        {
            for (int i = 0; i < 5; i++)
            {
                this.ShiftLow();
            }
        }

        // Token: 0x06000673 RID: 1651 RVA: 0x00008DBF File Offset: 0x00006FBF
        public void FlushStream()
        {
            this.Stream.Flush();
        }

        // Token: 0x06000674 RID: 1652 RVA: 0x00008DCC File Offset: 0x00006FCC
        public void CloseStream()
        {
            this.Stream.Close();
        }

        // Token: 0x06000675 RID: 1653 RVA: 0x0003E844 File Offset: 0x0003CA44
        public void Encode(uint start, uint size, uint total)
        {
            this.Low += (ulong)(start * (this.Range /= total));
            this.Range *= size;
            while (this.Range < 16777216U)
            {
                this.Range <<= 8;
                this.ShiftLow();
            }
        }

        // Token: 0x06000676 RID: 1654 RVA: 0x0003E8A4 File Offset: 0x0003CAA4
        public void ShiftLow()
        {
            if ((uint)this.Low < 4278190080U || (int)(this.Low >> 32) == 1)
            {
                byte b = this._cache;
                uint num;
                do
                {
                    this.Stream.WriteByte((byte)((ulong)b + (this.Low >> 32)));
                    b = byte.MaxValue;
                    num = this._cacheSize - 1U;
                    this._cacheSize = num;
                }
                while (num != 0U);
                this._cache = (byte)((uint)this.Low >> 24);
            }
            this._cacheSize += 1U;
            this.Low = (ulong)((ulong)((uint)this.Low) << 8);
        }

        // Token: 0x06000677 RID: 1655 RVA: 0x0003E934 File Offset: 0x0003CB34
        public void EncodeDirectBits(uint v, int numTotalBits)
        {
            for (int i = numTotalBits - 1; i >= 0; i--)
            {
                this.Range >>= 1;
                if ((v >> i & 1U) == 1U)
                {
                    this.Low += (ulong)this.Range;
                }
                if (this.Range < 16777216U)
                {
                    this.Range <<= 8;
                    this.ShiftLow();
                }
            }
        }

        // Token: 0x06000678 RID: 1656 RVA: 0x0003E9A0 File Offset: 0x0003CBA0
        public void EncodeBit(uint size0, int numTotalBits, uint symbol)
        {
            uint num = (this.Range >> numTotalBits) * size0;
            if (symbol == 0U)
            {
                this.Range = num;
            }
            else
            {
                this.Low += (ulong)num;
                this.Range -= num;
            }
            while (this.Range < 16777216U)
            {
                this.Range <<= 8;
                this.ShiftLow();
            }
        }

        // Token: 0x06000679 RID: 1657 RVA: 0x00008DD9 File Offset: 0x00006FD9
        public long GetProcessedSizeAdd()
        {
            return (long)((ulong)this._cacheSize + (ulong)this.Stream.Position - (ulong)this.StartPosition + 4UL);
        }

        // Token: 0x040007E8 RID: 2024
        public const uint kTopValue = 16777216U;

        // Token: 0x040007E9 RID: 2025
        private Stream Stream;

        // Token: 0x040007EA RID: 2026
        public ulong Low;

        // Token: 0x040007EB RID: 2027
        public uint Range;

        // Token: 0x040007EC RID: 2028
        private uint _cacheSize;

        // Token: 0x040007ED RID: 2029
        private byte _cache;

        // Token: 0x040007EE RID: 2030
        private long StartPosition;
    }
}
