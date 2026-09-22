using System;
using System.IO;

namespace SevenZip.Compression.RangeCoder
{
	// Token: 0x020000CE RID: 206
	internal class Decoder
	{
		// Token: 0x060005A3 RID: 1443 RVA: 0x000372B0 File Offset: 0x000354B0
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

		// Token: 0x060005A4 RID: 1444 RVA: 0x00008915 File Offset: 0x00006B15
		public void ReleaseStream()
		{
			this.Stream = null;
		}

		// Token: 0x060005A5 RID: 1445 RVA: 0x0000891E File Offset: 0x00006B1E
		public void CloseStream()
		{
			this.Stream.Close();
		}

		// Token: 0x060005A6 RID: 1446 RVA: 0x0000892B File Offset: 0x00006B2B
		public void Normalize()
		{
			while (this.Range < 16777216U)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x060005A7 RID: 1447 RVA: 0x00008965 File Offset: 0x00006B65
		public void Normalize2()
		{
			if (this.Range < 16777216U)
			{
				this.Code = (this.Code << 8 | (uint)((byte)this.Stream.ReadByte()));
				this.Range <<= 8;
			}
		}

		// Token: 0x060005A8 RID: 1448 RVA: 0x000372FC File Offset: 0x000354FC
		public uint GetThreshold(uint total)
		{
			return this.Code / (this.Range /= total);
		}

		// Token: 0x060005A9 RID: 1449 RVA: 0x0000899D File Offset: 0x00006B9D
		public void Decode(uint start, uint size, uint total)
		{
			this.Code -= start * this.Range;
			this.Range *= size;
			this.Normalize();
		}

		// Token: 0x060005AA RID: 1450 RVA: 0x00037324 File Offset: 0x00035524
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

		// Token: 0x060005AB RID: 1451 RVA: 0x00037398 File Offset: 0x00035598
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

		// Token: 0x04000758 RID: 1880
		public const uint kTopValue = 16777216U;

		// Token: 0x04000759 RID: 1881
		public uint Range;

		// Token: 0x0400075A RID: 1882
		public uint Code;

		// Token: 0x0400075B RID: 1883
		public Stream Stream;
	}
}
