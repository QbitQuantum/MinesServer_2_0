using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x020000CB RID: 203
	internal struct BitEncoder
	{
		// Token: 0x0600058F RID: 1423 RVA: 0x00008858 File Offset: 0x00006A58
		public void Init()
		{
			this.Prob = 1024U;
		}

		// Token: 0x06000590 RID: 1424 RVA: 0x00008865 File Offset: 0x00006A65
		public void UpdateModel(uint symbol)
		{
			if (symbol == 0U)
			{
				this.Prob += 2048U - this.Prob >> 5;
				return;
			}
			this.Prob -= this.Prob >> 5;
		}

		// Token: 0x06000591 RID: 1425 RVA: 0x00036EE4 File Offset: 0x000350E4
		public void Encode(Encoder encoder, uint symbol)
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

		// Token: 0x06000592 RID: 1426 RVA: 0x00036F7C File Offset: 0x0003517C
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

		// Token: 0x06000593 RID: 1427 RVA: 0x0000889B File Offset: 0x00006A9B
		public uint GetPrice(uint symbol)
		{
			return BitEncoder.ProbPrices[(int)(checked((IntPtr)((unchecked((ulong)(this.Prob - symbol) ^ (ulong)((long)(0U - symbol))) & 2047UL) >> 2)))];
		}

		// Token: 0x06000594 RID: 1428 RVA: 0x000088BB File Offset: 0x00006ABB
		public uint GetPrice0()
		{
			return BitEncoder.ProbPrices[(int)(this.Prob >> 2)];
		}

		// Token: 0x06000595 RID: 1429 RVA: 0x000088CB File Offset: 0x00006ACB
		public uint GetPrice1()
		{
			return BitEncoder.ProbPrices[(int)(2048U - this.Prob >> 2)];
		}

		// Token: 0x0400074D RID: 1869
		public const int kNumBitModelTotalBits = 11;

		// Token: 0x0400074E RID: 1870
		public const uint kBitModelTotal = 2048U;

		// Token: 0x0400074F RID: 1871
		private const int kNumMoveBits = 5;

		// Token: 0x04000750 RID: 1872
		private const int kNumMoveReducingBits = 2;

		// Token: 0x04000751 RID: 1873
		public const int kNumBitPriceShiftBits = 6;

		// Token: 0x04000752 RID: 1874
		private uint Prob;

		// Token: 0x04000753 RID: 1875
		private static uint[] ProbPrices = new uint[512];
	}
}
