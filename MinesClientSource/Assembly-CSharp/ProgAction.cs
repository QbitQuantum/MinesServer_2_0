using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000057 RID: 87
public class ProgAction : MonoBehaviour
{
	// Token: 0x060001FB RID: 507 RVA: 0x00006211 File Offset: 0x00004411
	public static void InitSprites()
	{
		if (!ProgAction.inited)
		{
			ProgAction.sprites = ResourcesManager.LoadAll<Sprite>("programmator");
			ProgAction.inited = true;
		}
	}

	// Token: 0x060001FC RID: 508 RVA: 0x0000622F File Offset: 0x0000442F
	private void Start()
	{
		ProgAction.InitSprites();
	}

	// Token: 0x060001FD RID: 509 RVA: 0x000203F0 File Offset: 0x0001E5F0
	public void ChangeTo(int _id)
	{
		ProgAction.InitSprites();
		this.id = _id;
		base.GetComponent<Image>().sprite = ProgAction.sprites[this.id];
		base.GetComponent<Image>().SetNativeSize();
		if (this.input != null)
		{
			this.updateInput();
			return;
		}
		this.inputInited = false;
	}

	// Token: 0x060001FE RID: 510 RVA: 0x00020448 File Offset: 0x0001E648
	private void updateInput()
	{
		this.input.gameObject.SetActive(false);
		this.numInput.gameObject.SetActive(false);
		if (this.id == 40)
		{
			this.input.gameObject.SetActive(true);
			this.input.gameObject.transform.localPosition = new Vector3(-5f, 0f);
		}
		else if (this.id == 140 || this.id == 139 || this.id == 166)
		{
			this.input.gameObject.SetActive(true);
			this.input.gameObject.transform.localPosition = new Vector3(-1f, -9f);
		}
		else if (this.id == 25 || this.id == 26 || this.id == 137)
		{
			this.input.gameObject.SetActive(true);
			this.input.gameObject.transform.localPosition = new Vector3(1f, 0f);
		}
		else if (this.id == 24)
		{
			this.input.gameObject.SetActive(true);
			this.input.gameObject.transform.localPosition = new Vector3(6f, 0f);
		}
		else if (this.id == 123 || this.id == 119 || this.id == 120)
		{
			this.input.gameObject.SetActive(true);
			this.input.gameObject.transform.localPosition = new Vector3(-3f, 4f);
			this.numInput.gameObject.SetActive(true);
			this.numInput.gameObject.transform.localPosition = new Vector3(-1f, -10f);
		}
		else if (this.id == 182 || this.id == 181)
		{
			this.input.gameObject.SetActive(true);
			this.input.gameObject.transform.localPosition = new Vector3(0f, -2f);
		}
		this.inputInited = true;
	}

	// Token: 0x060001FF RID: 511 RVA: 0x00006236 File Offset: 0x00004436
	private void Update()
	{
		if (!this.inputInited)
		{
			this.updateInput();
		}
	}

	// Token: 0x06000200 RID: 512 RVA: 0x00006246 File Offset: 0x00004446
	public string getString()
	{
		return this.input.text;
	}

	// Token: 0x06000201 RID: 513 RVA: 0x00006253 File Offset: 0x00004453
	public void setString(string label)
	{
		this.input.text = label;
	}

	// Token: 0x06000202 RID: 514 RVA: 0x0002069C File Offset: 0x0001E89C
	public int getNum()
	{
		int num;
		if (!int.TryParse(this.numInput.text, out num))
		{
			return 0;
		}
		return int.Parse(this.numInput.text);
	}

	// Token: 0x06000203 RID: 515 RVA: 0x00006261 File Offset: 0x00004461
	public void setNum(int label)
	{
		this.numInput.text = label.ToString();
	}

	// Token: 0x04000361 RID: 865
	public static Sprite[] sprites;

	// Token: 0x04000362 RID: 866
	public static bool inited;

	// Token: 0x04000363 RID: 867
	public InputField numInput;

	// Token: 0x04000364 RID: 868
	public InputField input;

	// Token: 0x04000365 RID: 869
	public int id;

	// Token: 0x04000366 RID: 870
	public bool inputInited;
}
