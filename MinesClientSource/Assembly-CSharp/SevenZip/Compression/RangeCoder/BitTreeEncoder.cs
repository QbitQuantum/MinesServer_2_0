using System;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x020000CD RID: 205
	internal struct BitTreeEncoder
	{
		// Token: 0x0600059B RID: 1435 RVA: 0x000088FB File Offset: 0x00006AFB
		public BitTreeEncoder(int numBitLevels)
		{
			this.NumBitLevels = numBitLevels;
			this.Models = new BitEncoder[1 << numBitLevels];
		}

		// Token: 0x0600059C RID: 1436 RVA: 0x000370E4 File Offset: 0x000352E4
		public void Init()
		{
			uint num = 1U;
			while ((ulong)num < (ulong)(1L << (this.NumBitLevels & 31)))
			{
				this.Models[(int)num].Init();
				num += 1U;
			}
		}

		// Token: 0x0600059D RID: 1437 RVA: 0x0003711C File Offset: 0x0003531C
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

		// Token: 0x0600059E RID: 1438 RVA: 0x00037160 File Offset: 0x00035360
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

		// Token: 0x0600059F RID: 1439 RVA: 0x000371A4 File Offset: 0x000353A4
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

		// Token: 0x060005A0 RID: 1440 RVA: 0x000371EC File Offset: 0x000353EC
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

		// Token: 0x060005A1 RID: 1441 RVA: 0x00037234 File Offset: 0x00035434
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

		// Token: 0x060005A2 RID: 1442 RVA: 0x00037274 File Offset: 0x00035474
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

		// Token: 0x04000756 RID: 1878
		private BitEncoder[] Models;

		// Token: 0x04000757 RID: 1879
		private int NumBitLevels;
	}
}
