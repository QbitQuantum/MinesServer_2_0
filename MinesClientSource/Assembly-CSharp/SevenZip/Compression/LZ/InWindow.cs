using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x020000C8 RID: 200
	public class InWindow
	{
		// Token: 0x06000576 RID: 1398 RVA: 0x000369E8 File Offset: 0x00034BE8
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

		// Token: 0x06000577 RID: 1399 RVA: 0x00036A50 File Offset: 0x00034C50
		public virtual void ReadBlock()
		{
			if (this._streamEndWasReached)
			{
				return;
			}
			for (;;)
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

		// Token: 0x06000578 RID: 1400 RVA: 0x000086CB File Offset: 0x000068CB
		private void Free()
		{
			this._bufferBase = null;
		}

		// Token: 0x06000579 RID: 1401 RVA: 0x00036B18 File Offset: 0x00034D18
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

		// Token: 0x0600057A RID: 1402 RVA: 0x000086D4 File Offset: 0x000068D4
		public void SetStream(Stream stream)
		{
			this._stream = stream;
		}

		// Token: 0x0600057B RID: 1403 RVA: 0x000086DD File Offset: 0x000068DD
		public void ReleaseStream()
		{
			this._stream = null;
		}

		// Token: 0x0600057C RID: 1404 RVA: 0x000086E6 File Offset: 0x000068E6
		public void Init()
		{
			this._bufferOffset = 0U;
			this._pos = 0U;
			this._streamPos = 0U;
			this._streamEndWasReached = false;
			this.ReadBlock();
		}

		// Token: 0x0600057D RID: 1405 RVA: 0x0000870A File Offset: 0x0000690A
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

		// Token: 0x0600057E RID: 1406 RVA: 0x00008749 File Offset: 0x00006949
		public byte GetIndexByte(int index)
		{
			return this._bufferBase[(int)(checked((IntPtr)(unchecked((ulong)(this._bufferOffset + this._pos) + (ulong)((long)index)))))];
		}

		// Token: 0x0600057F RID: 1407 RVA: 0x00036B78 File Offset: 0x00034D78
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

		// Token: 0x06000580 RID: 1408 RVA: 0x00008764 File Offset: 0x00006964
		public uint GetNumAvailableBytes()
		{
			return this._streamPos - this._pos;
		}

		// Token: 0x06000581 RID: 1409 RVA: 0x00008773 File Offset: 0x00006973
		public void ReduceOffsets(int subValue)
		{
			this._bufferOffset += (uint)subValue;
			this._posLimit -= (uint)subValue;
			this._pos -= (uint)subValue;
			this._streamPos -= (uint)subValue;
		}

		// Token: 0x04000738 RID: 1848
		public byte[] _bufferBase;

		// Token: 0x04000739 RID: 1849
		private Stream _stream;

		// Token: 0x0400073A RID: 1850
		private uint _posLimit;

		// Token: 0x0400073B RID: 1851
		private bool _streamEndWasReached;

		// Token: 0x0400073C RID: 1852
		private uint _pointerToLastSafePosition;

		// Token: 0x0400073D RID: 1853
		public uint _bufferOffset;

		// Token: 0x0400073E RID: 1854
		public uint _blockSize;

		// Token: 0x0400073F RID: 1855
		public uint _pos;

		// Token: 0x04000740 RID: 1856
		private uint _keepSizeBefore;

		// Token: 0x04000741 RID: 1857
		private uint _keepSizeAfter;

		// Token: 0x04000742 RID: 1858
		public uint _streamPos;
	}
}
