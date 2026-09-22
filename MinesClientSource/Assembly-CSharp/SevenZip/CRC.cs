using System;

namespace SevenZip
{
	// Token: 0x020000A6 RID: 166
	internal class CRC
	{
		// Token: 0x060004DA RID: 1242 RVA: 0x000328F0 File Offset: 0x00030AF0
		static CRC()
		{
			for (uint num = 0U; num < 256U; num += 1U)
			{
				uint num2 = num;
				for (int i = 0; i < 8; i++)
				{
					num2 = (((num2 & 1U) == 0U) ? (num2 >> 1) : (num2 >> 1 ^ 3988292384U));
				}
				CRC.Table[(int)num] = num2;
			}
		}

		// Token: 0x060004DB RID: 1243 RVA: 0x00008083 File Offset: 0x00006283
		public void Init()
		{
			this._value = uint.MaxValue;
		}

		// Token: 0x060004DC RID: 1244 RVA: 0x0000808C File Offset: 0x0000628C
		public void UpdateByte(byte b)
		{
			this._value = (CRC.Table[(int)((byte)this._value ^ b)] ^ this._value >> 8);
		}

		// Token: 0x060004DD RID: 1245 RVA: 0x00032948 File Offset: 0x00030B48
		public void Update(byte[] data, uint offset, uint size)
		{
			for (uint num = 0U; num < size; num += 1U)
			{
				this._value = (CRC.Table[(int)((byte)this._value ^ data[(int)(offset + num)])] ^ this._value >> 8);
			}
		}

		// Token: 0x060004DE RID: 1246 RVA: 0x000080AC File Offset: 0x000062AC
		public uint GetDigest()
		{
			return this._value ^ uint.MaxValue;
		}

		// Token: 0x060004DF RID: 1247 RVA: 0x000080B6 File Offset: 0x000062B6
		private static uint CalculateDigest(byte[] data, uint offset, uint size)
		{
			CRC crc = new CRC();
			crc.Update(data, offset, size);
			return crc.GetDigest();
		}

		// Token: 0x060004E0 RID: 1248 RVA: 0x000080CB File Offset: 0x000062CB
		private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
		{
			return CRC.CalculateDigest(data, offset, size) == digest;
		}

		// Token: 0x0400065A RID: 1626
		public static readonly uint[] Table = new uint[256];

		// Token: 0x0400065B RID: 1627
		private uint _value = uint.MaxValue;
	}
}
