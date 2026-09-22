using System;
using System.IO;

namespace SevenZip.Buffer
{
	// Token: 0x020000B6 RID: 182
	public class OutBuffer
	{
		// Token: 0x060004FD RID: 1277 RVA: 0x00008215 File Offset: 0x00006415
		public OutBuffer(uint bufferSize)
		{
			this.m_Buffer = new byte[bufferSize];
			this.m_BufferSize = bufferSize;
		}

		// Token: 0x060004FE RID: 1278 RVA: 0x00008230 File Offset: 0x00006430
		public void SetStream(Stream stream)
		{
			this.m_Stream = stream;
		}

		// Token: 0x060004FF RID: 1279 RVA: 0x00008239 File Offset: 0x00006439
		public void FlushStream()
		{
			this.m_Stream.Flush();
		}

		// Token: 0x06000500 RID: 1280 RVA: 0x00008246 File Offset: 0x00006446
		public void CloseStream()
		{
			this.m_Stream.Close();
		}

		// Token: 0x06000501 RID: 1281 RVA: 0x00008253 File Offset: 0x00006453
		public void ReleaseStream()
		{
			this.m_Stream = null;
		}

		// Token: 0x06000502 RID: 1282 RVA: 0x0000825C File Offset: 0x0000645C
		public void Init()
		{
			this.m_ProcessedSize = 0UL;
			this.m_Pos = 0U;
		}

		// Token: 0x06000503 RID: 1283 RVA: 0x00032E60 File Offset: 0x00031060
		public void WriteByte(byte b)
		{
			byte[] buffer = this.m_Buffer;
			uint pos = this.m_Pos;
			this.m_Pos = pos + 1U;
			buffer[(int)pos] = b;
			if (this.m_Pos >= this.m_BufferSize)
			{
				this.FlushData();
			}
		}

		// Token: 0x06000504 RID: 1284 RVA: 0x0000826D File Offset: 0x0000646D
		public void FlushData()
		{
			if (this.m_Pos != 0U)
			{
				this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
				this.m_Pos = 0U;
			}
		}

		// Token: 0x06000505 RID: 1285 RVA: 0x00008296 File Offset: 0x00006496
		public ulong GetProcessedSize()
		{
			return this.m_ProcessedSize + (ulong)this.m_Pos;
		}

		// Token: 0x0400068D RID: 1677
		private byte[] m_Buffer;

		// Token: 0x0400068E RID: 1678
		private uint m_Pos;

		// Token: 0x0400068F RID: 1679
		private uint m_BufferSize;

		// Token: 0x04000690 RID: 1680
		private Stream m_Stream;

		// Token: 0x04000691 RID: 1681
		private ulong m_ProcessedSize;
	}
}
