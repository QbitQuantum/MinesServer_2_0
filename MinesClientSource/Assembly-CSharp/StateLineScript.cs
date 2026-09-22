using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000067 RID: 103
public class StateLineScript : MonoBehaviour
{
	// Token: 0x0600029A RID: 666 RVA: 0x00006904 File Offset: 0x00004B04
	public void SetLine(string[] text, bool blinking, Color color)
	{
		this.texts = text;
		this.blinking = blinking;
		this.color = color;
	}

	// Token: 0x0600029B RID: 667 RVA: 0x0000691B File Offset: 0x00004B1B
	private void Start()
	{
		this.inited = true;
	}

	// Token: 0x0600029C RID: 668 RVA: 0x00028534 File Offset: 0x00026734
	private void Update()
	{
		int num = Mathf.FloorToInt(Time.unscaledTime / 1.5f) % this.texts.Length;
		if (num != this.prevIndex)
		{
			this.prevIndex = num;
			base.gameObject.GetComponentInChildren<Text>().text = this.texts[num];
		}
		if (this.blinking)
		{
			this.color.a = 0.5f + 0.3f * Mathf.Sin(4f * Time.unscaledTime);
		}
		else
		{
			this.color.a = 0.7f;
		}
		base.gameObject.GetComponent<Image>().color = this.color;
	}

	// Token: 0x040004FE RID: 1278
	public string[] texts;

	// Token: 0x040004FF RID: 1279
	public bool blinking;

	// Token: 0x04000500 RID: 1280
	public Color color;

	// Token: 0x04000501 RID: 1281
	public bool inited;

	// Token: 0x04000502 RID: 1282
	private int prevIndex = -1;
}
