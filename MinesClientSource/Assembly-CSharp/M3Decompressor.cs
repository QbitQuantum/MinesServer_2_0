using System;
using UnityEngine;

// Token: 0x0200002D RID: 45
public class M3Decompressor : MonoBehaviour
{
	// Token: 0x0600013B RID: 315 RVA: 0x00017260 File Offset: 0x00015460
	public static Texture2D M3Decompress(byte[] source)
	{
		if (!M3Decompressor.inited)
		{
			M3Decompressor.inited = true;
			M3Decompressor.inBuffer = new SmartBuffer(2500000);
			M3Decompressor.outBuffer = new SmartBuffer(2500000);
		}
		if (source.Length < 15)
		{
			return null;
		}
		int num = Convert.ToInt32(BitConverter.ToUInt16(source, 0));
		int num2 = Convert.ToInt32(BitConverter.ToUInt16(source, 2));
		for (int i = 0; i < 10; i++)
		{
			M3Decompressor.operations[i] = source[4 + i];
		}
		M3Decompressor.inBuffer.copyFromArray(source, 14);
		Texture2D texture2D = new Texture2D(num, num2, TextureFormat.RGBA32, false);
		for (int j = 0; j < 10; j++)
		{
			switch (M3Decompressor.operations[j])
			{
			case 1:
				M3Decompressor.UnDelta();
				M3Decompressor.SwapBuffers();
				break;
			case 2:
				M3Decompressor.UnNgramm();
				M3Decompressor.SwapBuffers();
				break;
			case 3:
				M3Decompressor.UnRLE();
				M3Decompressor.SwapBuffers();
				break;
			case 4:
				for (int k = 0; k < num2; k++)
				{
					for (int l = 0; l < num; l++)
					{
						texture2D.SetPixel(l, num2 - 1 - k, new Color((float)M3Decompressor.inBuffer.get(4 * (k * num + l)) / 255f, (float)M3Decompressor.inBuffer.get(4 * (k * num + l) + 1) / 255f, (float)M3Decompressor.inBuffer.get(4 * (k * num + l) + 2) / 255f, (float)M3Decompressor.inBuffer.get(4 * (k * num + l) + 3) / 255f));
					}
				}
				texture2D.Apply();
				break;
			case 5:
				for (int m = 0; m < num2; m++)
				{
					for (int n = 0; n < num; n++)
					{
						texture2D.SetPixel(n, num2 - 1 - m, new Color((float)M3Decompressor.inBuffer.get(m * num + n) / 255f, (float)M3Decompressor.inBuffer.get(num * num2 + (m * num + n)) / 255f, (float)M3Decompressor.inBuffer.get(2 * (num * num2) + (m * num + n)) / 255f, (float)M3Decompressor.inBuffer.get(3 * (num * num2) + (m * num + n)) / 255f));
					}
				}
				texture2D.Apply();
				break;
			}
		}
		return texture2D;
	}

	// Token: 0x0600013C RID: 316 RVA: 0x000174CC File Offset: 0x000156CC
	public static void UnNgramm()
	{
		M3Decompressor.outBuffer.clear();
		int i = 0;
		byte b = M3Decompressor.inBuffer.get(i);
		i++;
		for (int j = 0; j < 256; j++)
		{
			M3Decompressor.dictionary[j] = 0;
		}
		byte b2 = M3Decompressor.inBuffer.get(i);
		i++;
		while (i < 1560 && b2 != b)
		{
			M3Decompressor.dictionary[(int)b2] = i;
			while (i < 1560 && b2 != b)
			{
				b2 = M3Decompressor.inBuffer.get(i);
				i++;
			}
			b2 = M3Decompressor.inBuffer.get(i);
			i++;
		}
		if (i >= 1560)
		{
			throw new Exception("M3G NGRAMM CORRUPTED?!");
		}
		int num = 0;
		while (i < M3Decompressor.inBuffer.getLength())
		{
			num++;
			b2 = M3Decompressor.inBuffer.get(i);
			i++;
			if (M3Decompressor.dictionary[(int)b2] > 0)
			{
				int num2 = M3Decompressor.dictionary[(int)b2];
				byte b3 = M3Decompressor.inBuffer.get(num2);
				num2++;
				while (b3 != b)
				{
					M3Decompressor.outBuffer.push(b3);
					b3 = M3Decompressor.inBuffer.get(num2);
					num2++;
				}
			}
			else if (b2 == b)
			{
				b2 = M3Decompressor.inBuffer.get(i);
				i++;
				M3Decompressor.outBuffer.push(b2);
			}
			else
			{
				M3Decompressor.outBuffer.push(b2);
			}
		}
	}

	// Token: 0x0600013D RID: 317 RVA: 0x00017620 File Offset: 0x00015820
	public static void UnRLE()
	{
		byte b = M3Decompressor.inBuffer.get(0);
		byte b2 = M3Decompressor.inBuffer.get(1);
		M3Decompressor.outBuffer.clear();
		int i = 2;
		while (i < M3Decompressor.inBuffer.getLength())
		{
			byte b3 = M3Decompressor.inBuffer.get(i);
			i++;
			if (b3 == b)
			{
				int num = (int)M3Decompressor.inBuffer.get(i);
				i++;
				byte b4 = M3Decompressor.inBuffer.get(i);
				i++;
				for (int j = 0; j < num; j++)
				{
					M3Decompressor.outBuffer.push(b4);
				}
			}
			else if (b3 == b2)
			{
				int num2 = (int)M3Decompressor.inBuffer.get(i);
				i++;
				int num3 = (int)M3Decompressor.inBuffer.get(i);
				i++;
				byte b5 = M3Decompressor.inBuffer.get(i);
				i++;
				for (int k = 0; k < num2 + 256 * num3; k++)
				{
					M3Decompressor.outBuffer.push(b5);
				}
			}
			else
			{
				M3Decompressor.outBuffer.push(b3);
			}
		}
	}

	// Token: 0x0600013E RID: 318 RVA: 0x00017728 File Offset: 0x00015928
	public static void UnDelta()
	{
		M3Decompressor.outBuffer.clear();
		int num = 0;
		for (int i = 0; i < M3Decompressor.inBuffer.getLength(); i++)
		{
			int num2 = num + (int)M3Decompressor.inBuffer.get(i);
			if (num2 > 255)
			{
				num2 -= 256;
			}
			num = num2;
			M3Decompressor.outBuffer.push((byte)num2);
		}
	}

	// Token: 0x0600013F RID: 319 RVA: 0x00005946 File Offset: 0x00003B46
	public static void SwapBuffers()
	{
		SmartBuffer smartBuffer = M3Decompressor.inBuffer;
		M3Decompressor.inBuffer = M3Decompressor.outBuffer;
		M3Decompressor.outBuffer = smartBuffer;
	}

	// Token: 0x06000140 RID: 320 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Start()
	{
	}

	// Token: 0x06000141 RID: 321 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000235 RID: 565
	public static SmartBuffer inBuffer;

	// Token: 0x04000236 RID: 566
	public static SmartBuffer outBuffer;

	// Token: 0x04000237 RID: 567
	public static byte[] operations = new byte[10];

	// Token: 0x04000238 RID: 568
	public static int[] dictionary = new int[256];

	// Token: 0x04000239 RID: 569
	public static bool inited = false;
}
