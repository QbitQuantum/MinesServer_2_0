using System;
using UnityEngine;

// Token: 0x0200001A RID: 26
public class FakeGenerator : MonoBehaviour
{
	// Token: 0x060000C6 RID: 198 RVA: 0x00013370 File Offset: 0x00011570
	private void FakeMapGeneration()
	{
		this.map = new MapModel(1600, 1600, "test");
		for (int i = 0; i < 1600; i++)
		{
			for (int j = 0; j < 1600; j++)
			{
				if (
					Mathf.Cos(0.1f * (float)i + 0.25634f * (float)j) + 
					Mathf.Cos(0.1234f * (float)i + 0.43262f * (float)j) + 
					Mathf.Cos(0.0253f * (float)i + 0.0732f * (float)j) + 
					Mathf.Cos(0.0436f * (float)i + 0.026543f * (float)j) + 
					Mathf.Cos(0.1557f * (float)i + 0.2362f * (float)j) + 
					Mathf.Cos(0.235f * (float)i + 0.2624f * (float)j) < 0f)
				{
					this.map.SetCell(i, j, 117);
					if (j < 320)
					{
						this.map.SetCell(i, j, 114);
					}
					if (j < 280)
					{
						this.map.SetCell(i, j, 119);
					}
					if (j < 240)
					{
						this.map.SetCell(i, j, 118);
					}
					if (j < 200)
					{
						this.map.SetCell(i, j, 122);
					}
					if (j < 160)
					{
						this.map.SetCell(i, j, 121);
					}
					if (j < 120)
					{
						this.map.SetCell(i, j, 120);
					}
					if (j < 80)
					{
						this.map.SetCell(i, j, 113);
					}
					if (j < 40)
					{
						this.map.SetCell(i, j, 103);
					}
					if ((double)UnityEngine.Random.value < 0.3)
					{
						this.map.SetCell(i, j, 75);
						if (j < 600)
						{
							this.map.SetCell(i, j, 55);
						}
						if (j < 600)
						{
							this.map.SetCell(i, j, 54);
						}
						if (j < 560)
						{
							this.map.SetCell(i, j, 53);
						}
						if (j < 520)
						{
							this.map.SetCell(i, j, 52);
						}
						if (j < 480)
						{
							this.map.SetCell(i, j, 51);
						}
						if (j < 440)
						{
							this.map.SetCell(i, j, 50);
						}
						if (j < 400)
						{
							this.map.SetCell(i, j, 74);
						}
						if (j < 360)
						{
							this.map.SetCell(i, j, 73);
						}
						if (j < 320)
						{
							this.map.SetCell(i, j, 72);
						}
						if (j < 280)
						{
							this.map.SetCell(i, j, 71);
						}
						if (j < 240)
						{
							this.map.SetCell(i, j, 112);
						}
						if (j < 200)
						{
							this.map.SetCell(i, j, 111);
						}
						if (j < 160)
						{
							this.map.SetCell(i, j, 110);
						}
						if (j < 120)
						{
							this.map.SetCell(i, j, 108);
						}
						if (j < 80)
						{
							this.map.SetCell(i, j, 109);
						}
						if (j < 40)
						{
							this.map.SetCell(i, j, 107);
						}
					}
				}
				else if ((double)UnityEngine.Random.value < 0.8)
				{
					this.map.SetCell(i, j, 32);
					if ((double)UnityEngine.Random.value < 0.66)
					{
						this.map.SetCell(i, j, 33);
					}
					if ((double)UnityEngine.Random.value < 0.33)
					{
						this.map.SetCell(i, j, 34);
					}
				}
				else if (j < 20)
				{
					this.map.SetCell(i, j, 99);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 100);
					}
				}
				else if (j < 40)
				{
					this.map.SetCell(i, j, 97);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 98);
					}
				}
				else if (j < 50)
				{
					this.map.SetCell(i, j, 60);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 61);
					}
				}
				else if (j < 60)
				{
					this.map.SetCell(i, j, 62);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 63);
					}
				}
				else if (j < 100)
				{
					this.map.SetCell(i, j, 91);
				}
				else if (j < 130)
				{
					this.map.SetCell(i, j, 86);
				}
				else if (j < 160)
				{
					this.map.SetCell(i, j, 66);
				}
				else if (j < 200)
				{
					this.map.SetCell(i, j, 67);
				}
				else if (j < 240)
				{
					this.map.SetCell(i, j, 92);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 93);
					}
					if ((double)UnityEngine.Random.value < 0.25)
					{
						this.map.SetCell(i, j, 94);
					}
				}
				else if (j < 280)
				{
					this.map.SetCell(i, j, 40);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 41);
					}
					if ((double)UnityEngine.Random.value < 0.25)
					{
						this.map.SetCell(i, j, 42);
					}
				}
				else if (j < 320)
				{
					this.map.SetCell(i, j, 43);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 44);
					}
					if ((double)UnityEngine.Random.value < 0.25)
					{
						this.map.SetCell(i, j, 45);
					}
				}
				else if (j < 360)
				{
					this.map.SetCell(i, j, 68);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 69);
					}
				}
				else
				{
					this.map.SetCell(i, j, 64);
					if ((double)UnityEngine.Random.value < 0.5)
					{
						this.map.SetCell(i, j, 65);
					}
				}
				if ((double)UnityEngine.Random.value < 0.005)
				{
					this.map.SetCell(i, j, 90);
				}
				if ((double)UnityEngine.Random.value < 0.005)
				{
					this.map.SetCell(i, j, 48);
				}
				if ((double)UnityEngine.Random.value < 0.005)
				{
					this.map.SetCell(i, j, 49);
				}
				if ((double)UnityEngine.Random.value < 0.005)
				{
					this.map.SetCell(i, j, 104);
				}
				if (i % 66 == 0)
				{
					this.map.SetCell(i, j, 35);
				}
				if (i % 68 == 0)
				{
					this.map.SetCell(i, j, 101);
				}
				if (i % 69 == 0)
				{
					this.map.SetCell(i, j, 102);
				}
				if (i % 70 == 0)
				{
					this.map.SetCell(i, j, 105);
				}
				if (i > 80 && i < 120)
				{
					this.map.SetCell(i, j, 36);
				}
			}
		}
		this.map.SetCell(90, 10, 106);
		this.map.SetCell(91, 10, 106);
		this.map.SetCell(92, 10, 106);
		this.map.SetCell(90, 11, 106);
		this.map.SetCell(91, 11, 37);
		this.map.SetCell(92, 11, 106);
		this.map.SetCell(90, 19, 38);
		this.map.SetCell(91, 19, 106);
		this.map.SetCell(92, 19, 38);
		this.map.SetCell(90, 20, 106);
		this.map.SetCell(91, 20, 106);
		this.map.SetCell(92, 20, 106);
		this.map.SetCell(90, 21, 106);
		this.map.SetCell(91, 21, 37);
		this.map.SetCell(92, 21, 106);
		this.map.SetCell(95, 10, 106);
		this.map.SetCell(95, 11, 106);
		this.map.SetCell(95, 12, 106);
		this.map.SetCell(96, 10, 106);
		this.map.SetCell(96, 11, 37);
		this.map.SetCell(96, 12, 106);
		this.map.SetCell(97, 10, 106);
		this.map.SetCell(97, 11, 37);
		this.map.SetCell(97, 12, 106);
		this.map.SetCell(95, 20, 106);
		this.map.SetCell(95, 21, 106);
		this.map.SetCell(95, 22, 106);
		this.map.SetCell(96, 20, 106);
		this.map.SetCell(96, 21, 37);
		this.map.SetCell(96, 22, 106);
		this.map.SetCell(97, 20, 106);
		this.map.SetCell(97, 21, 37);
		this.map.SetCell(97, 22, 106);
		this.map.SetCell(95, 23, 106);
		this.map.SetCell(96, 23, 37);
		this.map.SetCell(97, 23, 106);
		this.map.SetCell(100, 10, 106);
		this.map.SetCell(100, 11, 106);
		this.map.SetCell(100, 12, 106);
		this.map.SetCell(101, 10, 106);
		this.map.SetCell(101, 11, 37);
		this.map.SetCell(101, 12, 37);
		this.map.SetCell(102, 10, 106);
		this.map.SetCell(102, 11, 106);
		this.map.SetCell(102, 12, 106);
		this.map.SetCell(100, 9, 38);
		this.map.SetCell(101, 9, 106);
		this.map.SetCell(102, 9, 38);
		this.map.SetCell(100, 10, 106);
		this.map.SetCell(100, 11, 106);
		this.map.SetCell(100, 12, 106);
		this.map.SetCell(101, 10, 106);
		this.map.SetCell(101, 11, 37);
		this.map.SetCell(101, 12, 37);
		this.map.SetCell(102, 10, 106);
		this.map.SetCell(102, 11, 106);
		this.map.SetCell(102, 12, 106);
		this.map.SetCell(110, 9, 38);
		this.map.SetCell(111, 9, 106);
		this.map.SetCell(112, 9, 37);
		this.map.SetCell(113, 9, 106);
		this.map.SetCell(114, 9, 38);
		this.map.SetCell(110, 10, 106);
		this.map.SetCell(111, 10, 106);
		this.map.SetCell(112, 10, 37);
		this.map.SetCell(113, 10, 106);
		this.map.SetCell(114, 10, 106);
		this.map.SetCell(110, 11, 37);
		this.map.SetCell(111, 11, 37);
		this.map.SetCell(112, 11, 37);
		this.map.SetCell(113, 11, 37);
		this.map.SetCell(114, 11, 37);
		this.map.SetCell(110, 12, 106);
		this.map.SetCell(111, 12, 106);
		this.map.SetCell(112, 12, 37);
		this.map.SetCell(113, 12, 106);
		this.map.SetCell(114, 12, 106);
		this.map.SetCell(110, 13, 38);
		this.map.SetCell(111, 13, 106);
		this.map.SetCell(112, 13, 37);
		this.map.SetCell(113, 13, 106);
		this.map.SetCell(114, 13, 38);
		this.map.SetCell(110, 19, 38);
		this.map.SetCell(111, 19, 106);
		this.map.SetCell(112, 19, 106);
		this.map.SetCell(113, 19, 106);
		this.map.SetCell(114, 19, 38);
		this.map.SetCell(110, 20, 106);
		this.map.SetCell(111, 20, 106);
		this.map.SetCell(112, 20, 37);
		this.map.SetCell(113, 20, 106);
		this.map.SetCell(114, 20, 106);
		this.map.SetCell(110, 21, 106);
		this.map.SetCell(111, 21, 106);
		this.map.SetCell(112, 21, 37);
		this.map.SetCell(113, 21, 106);
		this.map.SetCell(114, 21, 106);
		this.map.SetCell(110, 22, 38);
		this.map.SetCell(111, 22, 106);
		this.map.SetCell(112, 22, 37);
		this.map.SetCell(113, 22, 106);
		this.map.SetCell(114, 22, 38);
	}

	// Token: 0x0400016B RID: 363
	private MapModel map;
}
