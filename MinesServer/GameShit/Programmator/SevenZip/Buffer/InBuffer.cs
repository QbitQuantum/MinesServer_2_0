using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.Buffer
{
    public class InBuffer
    {
        // Token: 0x060005B8 RID: 1464 RVA: 0x00008570 File Offset: 0x00006770
        public InBuffer(uint bufferSize)
        {
            this.m_Buffer = new byte[bufferSize];
            this.m_BufferSize = bufferSize;
        }

        // Token: 0x060005B9 RID: 1465 RVA: 0x0000858B File Offset: 0x0000678B
        public void Init(Stream stream)
        {
            this.m_Stream = stream;
            this.m_ProcessedSize = 0UL;
            this.m_Limit = 0U;
            this.m_Pos = 0U;
            this.m_StreamWasExhausted = false;
        }

        // Token: 0x060005BA RID: 1466 RVA: 0x0003A1AC File Offset: 0x000383AC
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

        // Token: 0x060005BB RID: 1467 RVA: 0x000085B1 File Offset: 0x000067B1
        public void ReleaseStream()
        {
            this.m_Stream = null;
        }

        // Token: 0x060005BC RID: 1468 RVA: 0x0003A214 File Offset: 0x00038414
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

        // Token: 0x060005BD RID: 1469 RVA: 0x0003A254 File Offset: 0x00038454
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

        // Token: 0x060005BE RID: 1470 RVA: 0x000085BA File Offset: 0x000067BA
        public ulong GetProcessedSize()
        {
            return this.m_ProcessedSize + (ulong)this.m_Pos;
        }

        // Token: 0x04000712 RID: 1810
        private byte[] m_Buffer;

        // Token: 0x04000713 RID: 1811
        private uint m_Pos;

        // Token: 0x04000714 RID: 1812
        private uint m_Limit;

        // Token: 0x04000715 RID: 1813
        private uint m_BufferSize;

        // Token: 0x04000716 RID: 1814
        private Stream m_Stream;

        // Token: 0x04000717 RID: 1815
        private bool m_StreamWasExhausted;

        // Token: 0x04000718 RID: 1816
        private ulong m_ProcessedSize;
    }
}
