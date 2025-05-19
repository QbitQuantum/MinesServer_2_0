using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZ
{
    public class OutWindow
    {
        // Token: 0x06000645 RID: 1605 RVA: 0x00008B62 File Offset: 0x00006D62
        public void Create(uint windowSize)
        {
            if (this._windowSize != windowSize)
            {
                this._buffer = new byte[windowSize];
            }
            this._windowSize = windowSize;
            this._pos = 0U;
            this._streamPos = 0U;
        }

        // Token: 0x06000646 RID: 1606 RVA: 0x00008B8E File Offset: 0x00006D8E
        public void Init(Stream stream, bool solid)
        {
            this.ReleaseStream();
            this._stream = stream;
            if (!solid)
            {
                this._streamPos = 0U;
                this._pos = 0U;
                this.TrainSize = 0U;
            }
        }

        // Token: 0x06000647 RID: 1607 RVA: 0x0003E02C File Offset: 0x0003C22C
        public bool Train(Stream stream)
        {
            long length = stream.Length;
            uint num = this.TrainSize = (uint)((length < (long)((ulong)this._windowSize)) ? length : ((long)((ulong)this._windowSize)));
            stream.Position = length - (long)((ulong)num);
            this._streamPos = (this._pos = 0U);
            while (num != 0U)
            {
                uint num2 = this._windowSize - this._pos;
                if (num < num2)
                {
                    num2 = num;
                }
                int num3 = stream.Read(this._buffer, (int)this._pos, (int)num2);
                if (num3 == 0)
                {
                    return false;
                }
                num -= (uint)num3;
                this._pos += (uint)num3;
                this._streamPos += (uint)num3;
                if (this._pos == this._windowSize)
                {
                    this._streamPos = (this._pos = 0U);
                }
            }
            return true;
        }

        // Token: 0x06000648 RID: 1608 RVA: 0x00008BB5 File Offset: 0x00006DB5
        public void ReleaseStream()
        {
            this.Flush();
            this._stream = null;
        }

        // Token: 0x06000649 RID: 1609 RVA: 0x0003E0F0 File Offset: 0x0003C2F0
        public void Flush()
        {
            uint num = this._pos - this._streamPos;
            if (num != 0U)
            {
                this._stream.Write(this._buffer, (int)this._streamPos, (int)num);
                if (this._pos >= this._windowSize)
                {
                    this._pos = 0U;
                }
                this._streamPos = this._pos;
            }
        }

        // Token: 0x0600064A RID: 1610 RVA: 0x0003E148 File Offset: 0x0003C348
        public void CopyBlock(uint distance, uint len)
        {
            uint num = this._pos - distance - 1U;
            if (num >= this._windowSize)
            {
                num += this._windowSize;
            }
            while (len != 0U)
            {
                if (num >= this._windowSize)
                {
                    num = 0U;
                }
                byte[] buffer = this._buffer;
                uint pos = this._pos;
                this._pos = pos + 1U;
                buffer[(int)pos] = this._buffer[(int)num++];
                if (this._pos >= this._windowSize)
                {
                    this.Flush();
                }
                len -= 1U;
            }
        }

        // Token: 0x0600064B RID: 1611 RVA: 0x0003E1C0 File Offset: 0x0003C3C0
        public void PutByte(byte b)
        {
            byte[] buffer = this._buffer;
            uint pos = this._pos;
            this._pos = pos + 1U;
            buffer[(int)pos] = b;
            if (this._pos >= this._windowSize)
            {
                this.Flush();
            }
        }

        // Token: 0x0600064C RID: 1612 RVA: 0x0003E1FC File Offset: 0x0003C3FC
        public byte GetByte(uint distance)
        {
            uint num = this._pos - distance - 1U;
            if (num >= this._windowSize)
            {
                num += this._windowSize;
            }
            return this._buffer[(int)num];
        }

        // Token: 0x040007CF RID: 1999
        private byte[] _buffer;

        // Token: 0x040007D0 RID: 2000
        private uint _pos;

        // Token: 0x040007D1 RID: 2001
        private uint _windowSize;

        // Token: 0x040007D2 RID: 2002
        private uint _streamPos;

        // Token: 0x040007D3 RID: 2003
        private Stream _stream;

        // Token: 0x040007D4 RID: 2004
        public uint TrainSize;
    }
}
