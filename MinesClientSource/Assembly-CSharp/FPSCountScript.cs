using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000019 RID: 25
public class FPSCountScript : MonoBehaviour
{
	// Token: 0x060000C3 RID: 195 RVA: 0x0001330C File Offset: 0x0001150C
	private void Update()
	{
		this.deltaTime += (Time.deltaTime - this.deltaTime) * 0.1f;
		float num = 1f / this.deltaTime;
		this.fpsText.text = "FPS " + Mathf.Ceil(num).ToString() + FPSCountScript.PING_MESSAGE;
	}

	// Token: 0x04000166 RID: 358
	public Text fpsText;

	// Token: 0x04000167 RID: 359
	public static string txt;

	// Token: 0x04000168 RID: 360
	public float deltaTime;

	// Token: 0x04000169 RID: 361
	public static string PING_MESSAGE = "";

	// Token: 0x0400016A RID: 362
	public static float f = 0f;
}
