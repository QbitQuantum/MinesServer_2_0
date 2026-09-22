using System;

// Token: 0x02000065 RID: 101
public class SmartBuffer
{
	// Token: 0x0600028B RID: 651 RVA: 0x00006800 File Offset: 0x00004A00
	public SmartBuffer(int maxLen)
	{
		this.buffer = new byte[maxLen];
		this.len = 0;
	}

	// Token: 0x0600028C RID: 652 RVA: 0x000281C4 File Offset: 0x000263C4
	public int copyFromArray(byte[] copyFrom, int offset = 0)
	{
		if (copyFrom.Length - offset > this.buffer.Length)
		{
			throw new Exception("SmartBuffer cant copy - too long");
		}
		for (int i = offset; i < copyFrom.Length; i++)
		{
			this.buffer[i - offset] = copyFrom[i];
		}
		this.len = copyFrom.Length - offset;
		return this.len;
	}

	// Token: 0x0600028D RID: 653 RVA: 0x0000681B File Offset: 0x00004A1B
	public int getLength()
	{
		return this.len;
	}

	// Token: 0x0600028E RID: 654 RVA: 0x00006823 File Offset: 0x00004A23
	public void clear()
	{
		this.len = 0;
	}

	// Token: 0x0600028F RID: 655 RVA: 0x0000682C File Offset: 0x00004A2C
	public byte get(int i)
	{
		if (i > this.len)
		{
			throw new Exception("SmartBuffer index out of range");
		}
		return this.buffer[i];
	}

	// Token: 0x06000290 RID: 656 RVA: 0x0000684A File Offset: 0x00004A4A
	public void push(byte b)
	{
		if (this.len >= this.buffer.Length)
		{
			throw new Exception("SmartBuffer cant push - buffer is full");
		}
		this.buffer[this.len] = b;
		this.len++;
	}

	// Token: 0x040004E0 RID: 1248
	private byte[] buffer;

	// Token: 0x040004E1 RID: 1249
	private int len;
}
