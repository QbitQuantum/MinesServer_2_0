using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x020000CC RID: 204
	internal struct BitTreeDecoder
	{
		// Token: 0x06000596 RID: 1430 RVA: 0x000088E1 File Offset: 0x00006AE1
		public BitTreeDecoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitDecoder[1 << numBitLevels];
		}

		// Token: 0x06000597 RID: 1431 RVA: 0x00036FE0 File Offset: 0x000351E0
		public void Init()
		{
			uint num = 1U;
			while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)num].Init();
				num += 1U;
			}
		}

		// Token: 0x06000598 RID: 1432 RVA: 0x00037018 File Offset: 0x00035218
		public uint Decode(Decoder rangeDecoder)
		{
			uint num = 1U;
			for (int i = this.NumBitLevels; i > 0; i--)
			{
				num = (num << 1) + this.Models[(int)num].Decode(rangeDecoder);
			}
			return num - (1U << this.NumBitLevels);
		}

		// Token: 0x06000599 RID: 1433 RVA: 0x0003705C File Offset: 0x0003525C
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

		// Token: 0x0600059A RID: 1434 RVA: 0x000370A4 File Offset: 0x000352A4
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

		// Token: 0x04000754 RID: 1876
		private BitDecoder[] Models;

		// Token: 0x04000755 RID: 1877
		private int NumBitLevels;
	}
}
