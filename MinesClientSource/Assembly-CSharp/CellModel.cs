using System;

// Token: 0x02000007 RID: 7
public class CellModel
{
	// Token: 0x0600002B RID: 43 RVA: 0x0000990C File Offset: 0x00007B0C
	public static void Init()
	{
		CellModel.isEmpty = new bool[127];
		for (int i = 0; i < 127; i++)
		{
			CellModel.isEmpty[i] = false;
		}
		for (int j = 0; j < CellModel.empty.Length; j++)
		{
			CellModel.isEmpty[CellModel.empty[j]] = true;
		}
	}

	// Token: 0x04000028 RID: 40
	public static int[] empty = new int[]
	{
		30,
		31,
		32,
		33,
		34,
		35,
		36,
		37,
		39,
		83
	};

	// Token: 0x04000029 RID: 41
	public static bool[] isEmpty;
}
