using System;
using UnityEngine;

// Token: 0x02000004 RID: 4
public class BoomScript : MonoBehaviour
{
	// Token: 0x06000022 RID: 34 RVA: 0x000094FC File Offset: 0x000076FC
	public void Setup(int x, int y, int size, int color, int _index)
	{
		this.index = _index;
		this.x = x;
		this.y = y;
		this.size = size;
		this.color = color;
		this.BOOM_TIME = 0.12f * Mathf.Sqrt((float)this.size);
		this.startTime = Time.unscaledTime;
		if (!this.inited)
		{
			return;
		}
		base.transform.position = new Vector3((float)x + 0.5f, -(float)y - 0.5f, -2f);
		base.transform.localScale = new Vector3(0f, 0f, 1f);
		switch (color)
		{
		case 0:
			base.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.8f, 0f);
			return;
		case 1:
			base.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 1f);
			return;
		case 2:
			base.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000023 RID: 35 RVA: 0x00009628 File Offset: 0x00007828
	private void Start()
	{
		this.inited = true;
		this.startTime = Time.unscaledTime;
		base.transform.position = new Vector3((float)this.x + 0.5f, -(float)this.y - 0.5f, -2f);
		base.transform.localScale = new Vector3(0f, 0f, 1f);
		switch (this.color)
		{
		case 0:
			base.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.8f, 0f);
			return;
		case 1:
			base.gameObject.GetComponent<SpriteRenderer>().color = new Color(1f, 0.2f, 1f);
			return;
		case 2:
			base.gameObject.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 1f);
			return;
		default:
			return;
		}
	}

	// Token: 0x06000024 RID: 36 RVA: 0x00009720 File Offset: 0x00007920
	private void Update()
	{
		float num = Time.unscaledTime - this.startTime;
		float num2 = num / this.BOOM_TIME;
		num2 = Mathf.Sqrt(num2);
		if ((double)num2 > 0.5)
		{
			Color color = base.gameObject.GetComponent<SpriteRenderer>().color;
			color.a = 2f * (1f - num2);
			base.gameObject.GetComponent<SpriteRenderer>().color = color;
		}
		base.transform.localScale = new Vector3(num2 * (float)this.size / 3f, num2 * (float)this.size / 3f, 1f);
		if (num > this.BOOM_TIME)
		{
			ClientController.THIS.boomPool.Free(this.index);
		}
	}

	// Token: 0x04000010 RID: 16
	private int x = 4;

	// Token: 0x04000011 RID: 17
	private int y = 4;

	// Token: 0x04000012 RID: 18
	private int size = 4;

	// Token: 0x04000013 RID: 19
	private int color = 4;

	// Token: 0x04000014 RID: 20
	private int index = -1;

	// Token: 0x04000015 RID: 21
	private bool inited;

	// Token: 0x04000016 RID: 22
	private float startTime;

	// Token: 0x04000017 RID: 23
	private float BOOM_TIME = 0.5f;
}
