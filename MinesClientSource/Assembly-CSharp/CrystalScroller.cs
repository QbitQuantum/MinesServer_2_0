using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000017 RID: 23
public class CrystalScroller : MonoBehaviour
{
	// Token: 0x060000B5 RID: 181 RVA: 0x00012CE0 File Offset: 0x00010EE0
	public void UpdateFromModel()
	{
		switch (this.type)
		{
		case 0:
			this.crys.texture = this.crys_sprites[0].texture;
			this.left.color = new Color(0f, 1f, 0f);
			this.right.color = new Color(0f, 1f, 0f);
			break;
		case 1:
			this.crys.texture = this.crys_sprites[1].texture;
			this.left.color = new Color(0.4f, 0.4f, 1f);
			this.right.color = new Color(0.4f, 0.4f, 1f);
			break;
		case 2:
			this.crys.texture = this.crys_sprites[2].texture;
			this.left.color = new Color(1f, 0.3f, 0.3f);
			this.right.color = new Color(1f, 0.3f, 0.3f);
			break;
		case 3:
			this.crys.texture = this.crys_sprites[3].texture;
			this.left.color = new Color(1f, 0f, 1f);
			this.right.color = new Color(1f, 0f, 1f);
			break;
		case 4:
			this.crys.texture = this.crys_sprites[4].texture;
			this.left.color = new Color(1f, 1f, 1f);
			this.right.color = new Color(1f, 1f, 1f);
			break;
		default:
			this.crys.texture = this.crys_sprites[5].texture;
			this.left.color = new Color(0f, 1f, 1f);
			this.right.color = new Color(0f, 1f, 1f);
			break;
		}
		this.UpdateValues();
		this.desc.text = this.descText;
		this.needUpdate = false;
	}

	// Token: 0x060000B6 RID: 182 RVA: 0x00012F4C File Offset: 0x0001114C
	private void UpdateValues()
	{
		if (this.d != 0L)
		{
			if (CrystalScroller.BUY_LOGIC)
			{
				this.bar.value = (float)this.value / (float)this.d;
			}
			else
			{
				float b = (float)this.value / (float)this.d;
				if (this.d < 100L)
				{
					this.bar.value = b;
				}
				else if (this.d < 10000L)
				{
					this.bar.value = this.invsinch(b);
				}
				else
				{
					this.bar.value = this.invsinch(this.invsinch(b));
				}
			}
			this.bar.interactable = true;
			this.handle.SetActive(true);
			return;
		}
		this.bar.value = 0f;
		this.bar.interactable = false;
		this.handle.SetActive(false);
	}

	// Token: 0x060000B7 RID: 183 RVA: 0x00005198 File Offset: 0x00003398
	private void Start()
	{
		this.UpdateFromModel();
		this.bar.onValueChanged.AddListener(new UnityAction<float>(this.BarChange));
	}

	// Token: 0x060000B8 RID: 184 RVA: 0x000051BC File Offset: 0x000033BC
	private void BarChange(float v)
	{
		this.needUpdate = true;
	}

	// Token: 0x060000B9 RID: 185 RVA: 0x0001302C File Offset: 0x0001122C
	private string KKZer(long num)
	{
		if (num < 1000L)
		{
			return num.ToString("##0");
		}
		if (num < 100000L)
		{
			return num.ToString("## ##0");
		}
		if (num < 100000000L)
		{
			return (num / 1000L).ToString("## ##0K");
		}
		if (num < 10000000000L)
		{
			return (num / 1000000L).ToString("## ##0KK");
		}
		return (num / 1000000000L).ToString("## ##0KKK");
	}

	// Token: 0x060000BA RID: 186 RVA: 0x000051C5 File Offset: 0x000033C5
	private float sinch(float a)
	{
		return 0.5f - 0.5f * Mathf.Cos(a * 3.1415927f);
	}

	// Token: 0x060000BB RID: 187 RVA: 0x000051DF File Offset: 0x000033DF
	private float invsinch(float b)
	{
		return Mathf.Acos(-2f * (b - 0.5f)) / 3.1415927f;
	}

	// Token: 0x060000BC RID: 188 RVA: 0x000130BC File Offset: 0x000112BC
	private void Update()
	{
		if (CrystalScroller.BUY_LOGIC)
		{
			if (this.needUpdate)
			{
				this.value = (long)((float)this.d * this.bar.value * this.bar.value * this.bar.value);
			}
			this.left.text = this.KKZer(this.leftMin + this.value);
			this.right.text = this.KKZer(this.rightMin + this.value);
			return;
		}
		if (this.needUpdate)
		{
			if (this.d < 100L)
			{
				this.value = (long)((float)this.d * this.bar.value);
			}
			else if (this.d < 10000L)
			{
				this.value = (long)((float)this.d * this.sinch(this.bar.value));
			}
			else
			{
				this.value = (long)((float)this.d * this.sinch(this.sinch(this.bar.value)));
			}
		}
		if (this.value > this.d)
		{
			this.value = this.d;
		}
		this.left.text = this.KKZer(this.leftMin + this.d - this.value);
		this.right.text = this.KKZer(this.rightMin + this.value);
	}

	// Token: 0x04000154 RID: 340
	public Text left;

	// Token: 0x04000155 RID: 341
	public Text right;

	// Token: 0x04000156 RID: 342
	public Text desc;

	// Token: 0x04000157 RID: 343
	public Scrollbar bar;

	// Token: 0x04000158 RID: 344
	public GameObject handle;

	// Token: 0x04000159 RID: 345
	public RawImage crys;

	// Token: 0x0400015A RID: 346
	public Sprite[] crys_sprites;

	// Token: 0x0400015B RID: 347
	public string descText = "";

	// Token: 0x0400015C RID: 348
	public int type;

	// Token: 0x0400015D RID: 349
	public long leftMin = 100L;

	// Token: 0x0400015E RID: 350
	public long rightMin = 100L;

	// Token: 0x0400015F RID: 351
	public long d = 100L;

	// Token: 0x04000160 RID: 352
	public long value = 50L;

	// Token: 0x04000161 RID: 353
	private bool needUpdate;

	// Token: 0x04000162 RID: 354
	public static bool BUY_LOGIC = true;
}
