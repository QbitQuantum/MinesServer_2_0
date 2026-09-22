using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x020000C9 RID: 201
	public class OutWindow
	{
		// Token: 0x06000583 RID: 1411 RVA: 0x000087AD File Offset: 0x000069AD
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

		// Token: 0x06000584 RID: 1412 RVA: 0x000087D9 File Offset: 0x000069D9
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

		// Token: 0x06000585 RID: 1413 RVA: 0x00036BF4 File Offset: 0x00034DF4
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

		// Token: 0x06000586 RID: 1414 RVA: 0x00008800 File Offset: 0x00006A00
		public void ReleaseStream()
		{
			this.Flush();
			this._stream = null;
		}

		// Token: 0x06000587 RID: 1415 RVA: 0x00036CB8 File Offset: 0x00034EB8
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

		// Token: 0x06000588 RID: 1416 RVA: 0x00036D10 File Offset: 0x00034F10
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

		// Token: 0x06000589 RID: 1417 RVA: 0x00036D88 File Offset: 0x00034F88
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

		// Token: 0x0600058A RID: 1418 RVA: 0x00036DC4 File Offset: 0x00034FC4
		public byte GetByte(uint distance)
		{
			uint num = this._pos - distance - 1U;
			if (num >= this._windowSize)
			{
				num += this._windowSize;
			}
			return this._buffer[(int)num];
		}

		// Token: 0x04000743 RID: 1859
		private byte[] _buffer;

		// Token: 0x04000744 RID: 1860
		private uint _pos;

		// Token: 0x04000745 RID: 1861
		private uint _windowSize;

		// Token: 0x04000746 RID: 1862
		private uint _streamPos;

		// Token: 0x04000747 RID: 1863
		private Stream _stream;

		// Token: 0x04000748 RID: 1864
		public uint TrainSize;
	}
}
