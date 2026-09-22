using System;

namespace SevenZip.Compression.LZMA
{
	// Token: 0x020000B7 RID: 183
	internal abstract class Base
	{
		// Token: 0x06000506 RID: 1286 RVA: 0x000082A6 File Offset: 0x000064A6
		public static uint GetLenToPosState(uint len)
		{
			len -= 2U;
			if (len < 4U)
			{
				return len;
			}
			return 3U;
		}

		// Token: 0x04000692 RID: 1682
		public const uint kNumRepDistances = 4U;

		// Token: 0x04000693 RID: 1683
		public const uint kNumStates = 12U;

		// Token: 0x04000694 RID: 1684
		public const int kNumPosSlotBits = 6;

		// Token: 0x04000695 RID: 1685
		public const int kDicLogSizeMin = 0;

		// Token: 0x04000696 RID: 1686
		public const int kNumLenToPosStatesBits = 2;

		// Token: 0x04000697 RID: 1687
		public const uint kNumLenToPosStates = 4U;

		// Token: 0x04000698 RID: 1688
		public const uint kMatchMinLen = 2U;

		// Token: 0x04000699 RID: 1689
		public const int kNumAlignBits = 4;

		// Token: 0x0400069A RID: 1690
		public const uint kAlignTableSize = 16U;

		// Token: 0x0400069B RID: 1691
		public const uint kAlignMask = 15U;

		// Token: 0x0400069C RID: 1692
		public const uint kStartPosModelIndex = 4U;

		// Token: 0x0400069D RID: 1693
		public const uint kEndPosModelIndex = 14U;

		// Token: 0x0400069E RID: 1694
		public const uint kNumPosModels = 10U;

		// Token: 0x0400069F RID: 1695
		public const uint kNumFullDistances = 128U;

		// Token: 0x040006A0 RID: 1696
		public const uint kNumLitPosStatesBitsEncodingMax = 4U;

		// Token: 0x040006A1 RID: 1697
		public const uint kNumLitContextBitsMax = 8U;

		// Token: 0x040006A2 RID: 1698
		public const int kNumPosStatesBitsMax = 4;

		// Token: 0x040006A3 RID: 1699
		public const uint kNumPosStatesMax = 16U;

		// Token: 0x040006A4 RID: 1700
		public const int kNumPosStatesBitsEncodingMax = 4;

		// Token: 0x040006A5 RID: 1701
		public const uint kNumPosStatesEncodingMax = 16U;

		// Token: 0x040006A6 RID: 1702
		public const int kNumLowLenBits = 3;

		// Token: 0x040006A7 RID: 1703
		public const int kNumMidLenBits = 3;

		// Token: 0x040006A8 RID: 1704
		public const int kNumHighLenBits = 8;

		// Token: 0x040006A9 RID: 1705
		public const uint kNumLowLenSymbols = 8U;

		// Token: 0x040006AA RID: 1706
		public const uint kNumMidLenSymbols = 8U;

		// Token: 0x040006AB RID: 1707
		public const uint kNumLenSymbols = 272U;

		// Token: 0x040006AC RID: 1708
		public const uint kMatchMaxLen = 273U;

		// Token: 0x020000B8 RID: 184
		public struct State
		{
			// Token: 0x06000508 RID: 1288 RVA: 0x000082B4 File Offset: 0x000064B4
			public void Init()
			{
				this.Index = 0U;
			}

			// Token: 0x06000509 RID: 1289 RVA: 0x000082BD File Offset: 0x000064BD
			public void UpdateChar()
			{
				if (this.Index < 4U)
				{
					this.Index = 0U;
					return;
				}
				if (this.Index < 10U)
				{
					this.Index -= 3U;
					return;
				}
				this.Index -= 6U;
			}

			// Token: 0x0600050A RID: 1290 RVA: 0x000082F7 File Offset: 0x000064F7
			public void UpdateMatch()
			{
				this.Index = ((this.Index < 7U) ? 7U : 10U);
			}

			// Token: 0x0600050B RID: 1291 RVA: 0x0000830D File Offset: 0x0000650D
			public void UpdateRep()
			{
				this.Index = ((this.Index < 7U) ? 8U : 11U);
			}

			// Token: 0x0600050C RID: 1292 RVA: 0x00008323 File Offset: 0x00006523
			public void UpdateShortRep()
			{
				this.Index = ((this.Index < 7U) ? 9U : 11U);
			}

			// Token: 0x0600050D RID: 1293 RVA: 0x0000833A File Offset: 0x0000653A
			public bool IsCharState()
			{
				return this.Index < 7U;
			}

			// Token: 0x040006AD RID: 1709
			public uint Index;
		}
	}
}
