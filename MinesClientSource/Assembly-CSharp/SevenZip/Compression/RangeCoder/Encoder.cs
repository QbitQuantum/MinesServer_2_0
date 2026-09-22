using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x020000CF RID: 207
	internal class Encoder
	{
		// Token: 0x060005AD RID: 1453 RVA: 0x000089C8 File Offset: 0x00006BC8
		public void SetStream(Stream stream)
		{
			this.Stream = stream;
		}

		// Token: 0x060005AE RID: 1454 RVA: 0x000089D1 File Offset: 0x00006BD1
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x060005AF RID: 1455 RVA: 0x000089DA File Offset: 0x00006BDA
		public void Init()
		{
			this.StartPosition = this.Stream.Position;
			this.Low = 0UL;
			this.Range = uint.MaxValue;
			this._cacheSize = 1U;
			this._cache = 0;
		}

		// Token: 0x060005B0 RID: 1456 RVA: 0x000373EC File Offset: 0x000355EC
		public void FlushData()
		{
			for (int i = 0; i < 5; i++)
			{
				this.ShiftLow();
			}
		}

		// Token: 0x060005B1 RID: 1457 RVA: 0x00008A0A File Offset: 0x00006C0A
		public void FlushStream()
		{
			this.Stream.Flush();
		}

		// Token: 0x060005B2 RID: 1458 RVA: 0x00008A17 File Offset: 0x00006C17
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x060005B3 RID: 1459 RVA: 0x0003740C File Offset: 0x0003560C
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

		// Token: 0x060005B4 RID: 1460 RVA: 0x0003746C File Offset: 0x0003566C
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

		// Token: 0x060005B5 RID: 1461 RVA: 0x000374FC File Offset: 0x000356FC
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

		// Token: 0x060005B6 RID: 1462 RVA: 0x00037568 File Offset: 0x00035768
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

		// Token: 0x060005B7 RID: 1463 RVA: 0x00008A24 File Offset: 0x00006C24
		public long GetProcessedSizeAdd()
		{
			return (long)((ulong)this._cacheSize + (ulong)this.Stream.Position - (ulong)this.StartPosition + 4UL);
		}

		// Token: 0x0400075C RID: 1884
		public const uint kTopValue = 16777216U;

		// Token: 0x0400075D RID: 1885
		private Stream Stream;

		// Token: 0x0400075E RID: 1886
		public ulong Low;

		// Token: 0x0400075F RID: 1887
		public uint Range;

		// Token: 0x04000760 RID: 1888
		private uint _cacheSize;

		// Token: 0x04000761 RID: 1889
		private byte _cache;

		// Token: 0x04000762 RID: 1890
		private long StartPosition;
	}
}
