using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000027 RID: 39
public class InventoryItem : MonoBehaviour
{
	// Token: 0x06000124 RID: 292 RVA: 0x000057D5 File Offset: 0x000039D5
	public static void InitSprites()
	{
		if (InventoryItem.spritesLoaded)
		{
			return;
		}
		InventoryItem.sprites = ResourcesManager.LoadAll<Sprite>("inventory");
		InventoryItem.spritesLoaded = true;
	}

	// Token: 0x06000125 RID: 293 RVA: 0x000057F4 File Offset: 0x000039F4
	private void Start()
	{
		if (!InventoryItem.spritesLoaded)
		{
			InventoryItem.InitSprites();
		}
		this.inited = true;
		this.UpdateItemView();
	}

	// Token: 0x06000126 RID: 294 RVA: 0x0000580F File Offset: 0x00003A0F
	public void Setup(int id, int num, bool frame = false, string upstr = "", string downstr = "")
	{
		this.id = id;
		this.num = num;
		this.frame = frame;
		this.upString = upstr;
		this.downString = downstr;
		if (this.inited)
		{
			this.UpdateItemView();
		}
	}

	// Token: 0x06000127 RID: 295 RVA: 0x000168F4 File Offset: 0x00014AF4
	private void UpdateItemView()
	{
		this.frameImage.gameObject.SetActive(this.frame);
		if (this.id == -1)
		{
			this.itemImage.gameObject.SetActive(false);
			this.numText.gameObject.SetActive(false);
			return;
		}
		this.itemImage.gameObject.SetActive(true);
		this.numText.gameObject.SetActive(true);
		if (this.id >= 2000)
		{
			this.itemImage.sprite = SkillButtonScript.sprites[this.id - 2000];
			this.itemImage.SetNativeSize();
			this.itemImage.rectTransform.sizeDelta = 0.5f * this.itemImage.rectTransform.sizeDelta;
		}
		else if (this.id > 200)
		{
			this.itemImage.sprite = ClanSpriteScript.sprites[this.id - 200 - 1];
			this.itemImage.SetNativeSize();
			this.itemImage.rectTransform.sizeDelta = 3f * this.itemImage.rectTransform.sizeDelta;
		}
		else
		{
			this.itemImage.sprite = InventoryItem.sprites[this.id];
			this.itemImage.SetNativeSize();
		}
		if (this.num > 0)
		{
			this.numText.text = this.num.ToString();
		}
		else if (this.num == 0)
		{
			this.numText.text = "";
		}
		else
		{
			this.numText.text = (-this.num).ToString();
			this.itemImage.color = new Color(1f, 1f, 1f, 0.4f);
			this.numText.color = new Color(0.8f, 0.4f, 0.4f, 0.9f);
		}
		if (this.upString.StartsWith("@"))
		{
			this.numText.text = "";
			this.itemImage.color = new Color(1f, 1f, 1f, 0.4f);
			this.numText.color = new Color(0.8f, 0.4f, 0.4f, 0.9f);
			this.upString = this.upString.Substring(1);
		}
		if (this.upString.StartsWith("^"))
		{
			this.upString = this.upString.Substring(1);
			this.upText.color = new Color(0.6f, 0.5f, 0.5f, 1f);
		}
		if (this.upString.StartsWith("!"))
		{
			this.upText.color = new Color(0.3f, 0.4f, 0.3f, 1f);
			this.upString = this.upString.Substring(1);
		}
		if (this.downString.StartsWith("^"))
		{
			this.downString = this.downString.Substring(1);
			this.numText.color = new Color(0.6f, 0.5f, 0.5f, 1f);
		}
		if (this.downString.StartsWith("!"))
		{
			this.downString = this.downString.Substring(1);
			this.numText.color = new Color(0.3f, 0.4f, 0.3f, 1f);
		}
		if (this.upString != "")
		{
			this.upText.gameObject.SetActive(true);
			this.upText.text = this.upString;
		}
		if (this.downString != "")
		{
			this.numText.gameObject.SetActive(true);
			this.numText.text = this.downString;
		}
	}

	// Token: 0x06000128 RID: 296 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000217 RID: 535
	public static Sprite[] sprites;

	// Token: 0x04000218 RID: 536
	public Image itemImage;

	// Token: 0x04000219 RID: 537
	public Text numText;

	// Token: 0x0400021A RID: 538
	public Text upText;

	// Token: 0x0400021B RID: 539
	private int id = -1;

	// Token: 0x0400021C RID: 540
	private int num;

	// Token: 0x0400021D RID: 541
	private string upString = "";

	// Token: 0x0400021E RID: 542
	private string downString = "";

	// Token: 0x0400021F RID: 543
	public Image frameImage;

	// Token: 0x04000220 RID: 544
	private static bool spritesLoaded;

	// Token: 0x04000221 RID: 545
	private bool inited;

	// Token: 0x04000222 RID: 546
	private bool frame;
}
