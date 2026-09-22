using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x020000B5 RID: 181
	public class InBuffer
	{
		// Token: 0x060004F6 RID: 1270 RVA: 0x000081BB File Offset: 0x000063BB
		public InBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060004F7 RID: 1271 RVA: 0x000081D6 File Offset: 0x000063D6
		public void Init(Stream stream)
		{
			this.m_Stream = stream;
			this.m_ProcessedSize = 0UL;
			this.m_Limit = 0U;
			this.m_Pos = 0U;
			this.m_StreamWasExhausted = false;
		}

		// Token: 0x060004F8 RID: 1272 RVA: 0x00032D74 File Offset: 0x00030F74
		public bool ReadBlock()
		{
			if (this.m_StreamWasExhausted)
			{
				return false;
			}
			this.m_ProcessedSize += (ulong)this.m_Pos;
			int num = this.m_Stream.Read(this.m_Buffer, 0, (int)this.m_BufferSize);
			this.m_Pos = 0U;
			this.m_Limit = (uint)num;
			this.m_StreamWasExhausted = (num == 0);
			return !this.m_StreamWasExhausted;
		}

		// Token: 0x060004F9 RID: 1273 RVA: 0x000081FC File Offset: 0x000063FC
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x060004FA RID: 1274 RVA: 0x00032DDC File Offset: 0x00030FDC
		public bool ReadByte(byte b)
		{
			if (this.m_Pos >= this.m_Limit && !this.ReadBlock())
			{
				return false;
			}
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			b = buffer[(int)pos];
			return true;
		}

		// Token: 0x060004FB RID: 1275 RVA: 0x00032E1C File Offset: 0x0003101C
		public byte ReadByte()
		{
			if (this.m_Pos >= this.m_Limit && !this.ReadBlock())
			{
				return byte.MaxValue;
			}
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			return buffer[(int)pos];
		}

		// Token: 0x060004FC RID: 1276 RVA: 0x00008205 File Offset: 0x00006405
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x04000686 RID: 1670
		private byte[] m_Buffer;

		// Token: 0x04000687 RID: 1671
		private uint m_Pos;

		// Token: 0x04000688 RID: 1672
		private uint m_Limit;

		// Token: 0x04000689 RID: 1673
		private uint m_BufferSize;

		// Token: 0x0400068A RID: 1674
		private Stream m_Stream;

		// Token: 0x0400068B RID: 1675
		private bool m_StreamWasExhausted;

		// Token: 0x0400068C RID: 1676
		private ulong m_ProcessedSize;
	}
}
