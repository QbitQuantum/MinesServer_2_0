using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.Buffer
{
    public class OutBuffer
    {
        // Token: 0x060005BF RID: 1471 RVA: 0x000085CA File Offset: 0x000067CA
        public OutBuffer(uint bufferSize)
        {
            this.m_Buffer = new byte[bufferSize];
            this.m_BufferSize = bufferSize;
        }

        // Token: 0x060005C0 RID: 1472 RVA: 0x000085E5 File Offset: 0x000067E5
        public void SetStream(Stream stream)
        {
            this.m_Stream = stream;
        }

        // Token: 0x060005C1 RID: 1473 RVA: 0x000085EE File Offset: 0x000067EE
        public void FlushStream()
        {
            this.m_Stream.Flush();
        }

        // Token: 0x060005C2 RID: 1474 RVA: 0x000085FB File Offset: 0x000067FB
        public void CloseStream()
        {
            this.m_Stream.Close();
        }

        // Token: 0x060005C3 RID: 1475 RVA: 0x00008608 File Offset: 0x00006808
        public void ReleaseStream()
        {
            this.m_Stream = null;
        }

        // Token: 0x060005C4 RID: 1476 RVA: 0x00008611 File Offset: 0x00006811
        public void Init()
        {
            this.m_ProcessedSize = 0UL;
            this.m_Pos = 0U;
        }

        // Token: 0x060005C5 RID: 1477 RVA: 0x0003A298 File Offset: 0x00038498
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

        // Token: 0x060005C6 RID: 1478 RVA: 0x00008622 File Offset: 0x00006822
        public void FlushData()
        {
            if (this.m_Pos != 0U)
            {
                this.m_Stream.Write(this.m_Buffer, 0, (int)this.m_Pos);
                this.m_Pos = 0U;
            }
        }

        // Token: 0x060005C7 RID: 1479 RVA: 0x0000864B File Offset: 0x0000684B
        public ulong GetProcessedSize()
        {
            return this.m_ProcessedSize + (ulong)this.m_Pos;
        }

        // Token: 0x04000719 RID: 1817
        private byte[] m_Buffer;

        // Token: 0x0400071A RID: 1818
        private uint m_Pos;

        // Token: 0x0400071B RID: 1819
        private uint m_BufferSize;

        // Token: 0x0400071C RID: 1820
        private Stream m_Stream;

        // Token: 0x0400071D RID: 1821
        private ulong m_ProcessedSize;
    }
}
