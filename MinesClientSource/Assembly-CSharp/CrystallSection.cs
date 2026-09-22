using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000018 RID: 24
public class CrystallSection : MonoBehaviour
{
	// Token: 0x060000BF RID: 191 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Start()
	{
	}

	// Token: 0x060000C0 RID: 192 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x060000C1 RID: 193 RVA: 0x0001322C File Offset: 0x0001142C
	public string GetValuesInString()
	{
		return string.Concat(new object[]
		{
			this.lines[0].GetComponent<CrystalScroller>().value,
			":",
			this.lines[1].GetComponent<CrystalScroller>().value,
			":",
			this.lines[2].GetComponent<CrystalScroller>().value,
			":",
			this.lines[3].GetComponent<CrystalScroller>().value,
			":",
			this.lines[4].GetComponent<CrystalScroller>().value,
			":",
			this.lines[5].GetComponent<CrystalScroller>().value
		});
	}

	// Token: 0x04000163 RID: 355
	public GameObject[] lines;

	// Token: 0x04000164 RID: 356
	public Text leftDesc;

	// Token: 0x04000165 RID: 357
	public Text rightDesc;
}
