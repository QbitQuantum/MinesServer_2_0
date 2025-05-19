using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZ
{
    public class InWindow
    {
        // Token: 0x06000638 RID: 1592 RVA: 0x0003DE20 File Offset: 0x0003C020
        public void MoveBlock()
        {
            uint num = this._bufferOffset + this._pos - this._keepSizeBefore;
            if (num != 0U)
            {
                num -= 1U;
            }
            uint num2 = this._bufferOffset + this._streamPos - num;
            for (uint num3 = 0U; num3 < num2; num3 += 1U)
            {
                this._bufferBase[(int)num3] = this._bufferBase[(int)(num + num3)];
            }
            this._bufferOffset -= num;
        }

        // Token: 0x06000639 RID: 1593 RVA: 0x0003DE88 File Offset: 0x0003C088
        public virtual void ReadBlock()
        {
            if (this._streamEndWasReached)
            {
                return;
            }
            for (; ; )
            {
                int num = (int)(0U - this._bufferOffset + this._blockSize - this._streamPos);
                if (num == 0)
                {
                    break;
                }
                int num2 = this._stream.Read(this._bufferBase, (int)(this._bufferOffset + this._streamPos), num);
                if (num2 == 0)
                {
                    goto IL_7F;
                }
                this._streamPos += (uint)num2;
                if (this._streamPos >= this._pos + this._keepSizeAfter)
                {
                    this._posLimit = this._streamPos - this._keepSizeAfter;
                }
            }
            return;
        IL_7F:
            this._posLimit = this._streamPos;
            if (this._bufferOffset + this._posLimit > this._pointerToLastSafePosition)
            {
                this._posLimit = this._pointerToLastSafePosition - this._bufferOffset;
            }
            this._streamEndWasReached = true;
        }

        // Token: 0x0600063A RID: 1594 RVA: 0x00008A80 File Offset: 0x00006C80
        private void Free()
        {
            this._bufferBase = null;
        }

        // Token: 0x0600063B RID: 1595 RVA: 0x0003DF50 File Offset: 0x0003C150
        public void Create(uint keepSizeBefore, uint keepSizeAfter, uint keepSizeReserv)
        {
            this._keepSizeBefore = keepSizeBefore;
            this._keepSizeAfter = keepSizeAfter;
            uint num = keepSizeBefore + keepSizeAfter + keepSizeReserv;
            if (this._bufferBase == null || this._blockSize != num)
            {
                this.Free();
                this._blockSize = num;
                this._bufferBase = new byte[this._blockSize];
            }
            this._pointerToLastSafePosition = this._blockSize - keepSizeAfter;
        }

        // Token: 0x0600063C RID: 1596 RVA: 0x00008A89 File Offset: 0x00006C89
        public void SetStream(Stream stream)
        {
            this._stream = stream;
        }

        // Token: 0x0600063D RID: 1597 RVA: 0x00008A92 File Offset: 0x00006C92
        public void ReleaseStream()
        {
            this._stream = null;
        }

        // Token: 0x0600063E RID: 1598 RVA: 0x00008A9B File Offset: 0x00006C9B
        public void Init()
        {
            this._bufferOffset = 0U;
            this._pos = 0U;
            this._streamPos = 0U;
            this._streamEndWasReached = false;
            this.ReadBlock();
        }

        // Token: 0x0600063F RID: 1599 RVA: 0x00008ABF File Offset: 0x00006CBF
        public void MovePos()
        {
            this._pos += 1U;
            if (this._pos > this._posLimit)
            {
                if (this._bufferOffset + this._pos > this._pointerToLastSafePosition)
                {
                    this.MoveBlock();
                }
                this.ReadBlock();
            }
        }

        // Token: 0x06000640 RID: 1600 RVA: 0x00008AFE File Offset: 0x00006CFE
        public byte GetIndexByte(int index)
        {
            return this._bufferBase[(int)(checked((IntPtr)(unchecked((ulong)(this._bufferOffset + this._pos) + (ulong)((long)index)))))];
        }

        // Token: 0x06000641 RID: 1601 RVA: 0x0003DFB0 File Offset: 0x0003C1B0
        public uint GetMatchLen(int index, uint distance, uint limit)
        {
            if (this._streamEndWasReached && (ulong)this._pos + (ulong)((long)index) + (ulong)limit > (ulong)this._streamPos)
            {
                limit = this._streamPos - (uint)((int)((ulong)this._pos + (ulong)((long)index)));
            }
            distance += 1U;
            uint num = this._bufferOffset + this._pos + (uint)index;
            uint num2 = 0U;
            while (num2 < limit && this._bufferBase[(int)(num + num2)] == this._bufferBase[(int)(num + num2 - distance)])
            {
                num2 += 1U;
            }
            return num2;
        }

        // Token: 0x06000642 RID: 1602 RVA: 0x00008B19 File Offset: 0x00006D19
        public uint GetNumAvailableBytes()
        {
            return this._streamPos - this._pos;
        }

        // Token: 0x06000643 RID: 1603 RVA: 0x00008B28 File Offset: 0x00006D28
        public void ReduceOffsets(int subValue)
        {
            this._bufferOffset += (uint)subValue;
            this._posLimit -= (uint)subValue;
            this._pos -= (uint)subValue;
            this._streamPos -= (uint)subValue;
        }

        // Token: 0x040007C4 RID: 1988
        public byte[] _bufferBase;

        // Token: 0x040007C5 RID: 1989
        private Stream _stream;

        // Token: 0x040007C6 RID: 1990
        private uint _posLimit;

        // Token: 0x040007C7 RID: 1991
        private bool _streamEndWasReached;

        // Token: 0x040007C8 RID: 1992
        private uint _pointerToLastSafePosition;

        // Token: 0x040007C9 RID: 1993
        public uint _bufferOffset;

        // Token: 0x040007CA RID: 1994
        public uint _blockSize;

        // Token: 0x040007CB RID: 1995
        public uint _pos;

        // Token: 0x040007CC RID: 1996
        private uint _keepSizeBefore;

        // Token: 0x040007CD RID: 1997
        private uint _keepSizeAfter;

        // Token: 0x040007CE RID: 1998
        public uint _streamPos;
    }
}
