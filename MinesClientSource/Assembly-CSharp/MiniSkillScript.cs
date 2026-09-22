using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000035 RID: 53
public class MiniSkillScript : MonoBehaviour
{
	// Token: 0x06000174 RID: 372 RVA: 0x00005B11 File Offset: 0x00003D11
	private void Start()
	{
		this.InitGfx();
	}

	// Token: 0x06000175 RID: 373 RVA: 0x0001AC5C File Offset: 0x00018E5C
	public void SetModel(int progress, string code)
	{
		this.code = code;
		this.iconNum = SkillButtonScript.skillShorts[code];
		this.p = (float)progress / 100f;
		this.color = SkillButtonScript.colors[this.iconNum];
		if (progress >= 200)
		{
			this.p = 2f;
		}
	}

	// Token: 0x06000176 RID: 374 RVA: 0x0001ACB8 File Offset: 0x00018EB8
	public void InitGfx()
	{
		this.icon.sprite = SkillButtonScript.sprites[this.iconNum];
		if (this.p >= 1f)
		{
			this.up.gameObject.SetActive(true);
			this.up.color = this.color;
			this.bar.color = new Color(1f, 0f, 0f);
			this.ttl = 10000000f;
			return;
		}
		this.up.gameObject.SetActive(false);
		this.bar.gameObject.SetActive(true);
		this.bar.color = this.color;
		this.ttl = Time.time + 5f;
	}

	// Token: 0x06000177 RID: 375 RVA: 0x0001AD7C File Offset: 0x00018F7C
	private void Update()
	{
		if (this.p >= 1f)
		{
			Vector3 localPosition = this.up.transform.localPosition;
			localPosition.y = 14f + 1f * Mathf.Sin(10f * Time.time);
			this.up.transform.localPosition = localPosition;
			Vector2 sizeDelta = this.bar.rectTransform.sizeDelta;
			sizeDelta.y = 20f * (0.98f * (this.p - 1f) + 0.02f * (1f * Time.time - Mathf.Floor(1f * Time.time)));
			this.bar.rectTransform.sizeDelta = sizeDelta;
		}
		else
		{
			Vector2 sizeDelta2 = this.bar.rectTransform.sizeDelta;
			sizeDelta2.y = 20f * (0.8f * this.p + 0.2f * (1f * Time.time - Mathf.Floor(1f * Time.time)));
			this.bar.rectTransform.sizeDelta = sizeDelta2;
		}
		if (Time.time > this.ttl)
		{
			UnityEngine.Object.Destroy(base.gameObject);
			MiniSkillScript.minis.Remove(this.code);
		}
	}

	// Token: 0x04000291 RID: 657
	public static Dictionary<string, MiniSkillScript> minis = new Dictionary<string, MiniSkillScript>();

	// Token: 0x04000292 RID: 658
	public Image icon;

	// Token: 0x04000293 RID: 659
	public RawImage bar;

	// Token: 0x04000294 RID: 660
	public RawImage up;

	// Token: 0x04000295 RID: 661
	private float ttl;

	// Token: 0x04000296 RID: 662
	private float p;

	// Token: 0x04000297 RID: 663
	private int iconNum = -1;

	// Token: 0x04000298 RID: 664
	private string code;

	// Token: 0x04000299 RID: 665
	private Color color;
}
