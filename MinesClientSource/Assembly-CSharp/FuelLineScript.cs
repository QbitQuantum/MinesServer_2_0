using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200001B RID: 27
public class FuelLineScript : MonoBehaviour
{
	// Token: 0x060000C8 RID: 200 RVA: 0x000141A0 File Offset: 0x000123A0
	public void Setup(int percentage, string left, string right, int crys_type, bool isFirstButton, bool isSecondButton, bool isThirdButton)
	{
		this.percentage = percentage;
		this.left = left;
		this.right = right;
		this.crys_type = crys_type;
		this.isFirstButton = isFirstButton;
		this.isSecondButton = isSecondButton;
		this.isThirdButton = isThirdButton;
		if (this.inited)
		{
			this.UpdateView();
		}
		this.isSetup = true;
	}

	// Token: 0x060000C9 RID: 201 RVA: 0x0000524E File Offset: 0x0000344E
	private void Start()
	{
		this.inited = true;
		if (this.isSetup)
		{
			this.UpdateView();
		}
	}

	// Token: 0x060000CA RID: 202 RVA: 0x000141F8 File Offset: 0x000123F8
	private void UpdateView()
	{
		Color color_crys_type = FuelLineScript.crysColors[this.crys_type];
        this.leftText.color = color_crys_type;
		this.allText.color = color_crys_type;
        this.lineImage.color = color_crys_type;
        this.leftText.text = this.left;
		this.allText.text = this.right;
		this.crysImage.sprite = this.crys_sprites[this.crys_type];
		base.gameObject.GetComponentsInChildren<Button>()[0].interactable = this.isFirstButton;
		base.gameObject.GetComponentsInChildren<Button>()[1].interactable = this.isSecondButton;
		base.gameObject.GetComponentsInChildren<Button>()[2].interactable = this.isThirdButton;
		this.lineImage.rectTransform.sizeDelta = new Vector2((float)(1 + 87 * this.percentage / 100), 6f);
		
	}

	// Token: 0x060000CB RID: 203 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x0400016C RID: 364
	public Image lineImage;

	// Token: 0x0400016D RID: 365
	public Image crysImage;

	// Token: 0x0400016E RID: 366
	public Text leftText;

	// Token: 0x0400016F RID: 367
	public Text allText;

	// Token: 0x04000170 RID: 368
	public Sprite[] crys_sprites;

	// Token: 0x04000171 RID: 369
	private int percentage;

	// Token: 0x04000172 RID: 370
	private string left;

	// Token: 0x04000173 RID: 371
	private string right;

	// Token: 0x04000174 RID: 372
	private int crys_type;

	// Token: 0x04000175 RID: 373
	private bool isFirstButton;

	// Token: 0x04000176 RID: 374
	private bool isSecondButton;

	// Token: 0x04000177 RID: 375
	private bool isThirdButton;

	// Token: 0x04000178 RID: 376
	private bool isSetup;

	// Token: 0x04000179 RID: 377
	private bool inited;

	// Token: 0x0400017A RID: 378
	public static Color[] crysColors = new Color[]
	{
		new Color(0f, 1f, 0f),
		new Color(0.4f, 0.4f, 1f),
		new Color(1f, 0.3f, 0.3f),
		new Color(1f, 0f, 1f),
		new Color(1f, 1f, 1f),
		new Color(0f, 1f, 1f)
	};
}
