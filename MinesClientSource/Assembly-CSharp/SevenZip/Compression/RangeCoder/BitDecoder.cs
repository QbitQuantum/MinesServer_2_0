using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x020000CA RID: 202
	internal struct BitDecoder
	{
		// Token: 0x0600058C RID: 1420 RVA: 0x0000880F File Offset: 0x00006A0F
		public void UpdateModel(int numMoveBits, uint symbol)
		{
			if (symbol == 0U)
			{
				this.Prob += 2048U - this.Prob >> numMoveBits;
				return;
			}
			this.Prob -= this.Prob >> numMoveBits;
		}

		// Token: 0x0600058D RID: 1421 RVA: 0x0000884B File Offset: 0x00006A4B
		public void Init()
		{
			this.Prob = 1024U;
		}

		// Token: 0x0600058E RID: 1422 RVA: 0x00036DF8 File Offset: 0x00034FF8
		public uint Decode(Decoder rangeDecoder)
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

		// Token: 0x04000749 RID: 1865
		public const int kNumBitModelTotalBits = 11;

		// Token: 0x0400074A RID: 1866
		public const uint kBitModelTotal = 2048U;

		// Token: 0x0400074B RID: 1867
		private const int kNumMoveBits = 5;

		// Token: 0x0400074C RID: 1868
		private uint Prob;
	}
}
