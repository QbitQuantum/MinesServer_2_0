using System;
using UnityEngine;

// Token: 0x02000016 RID: 22
public class CrysPlusScript : MonoBehaviour
{
	// Token: 0x060000B0 RID: 176 RVA: 0x0001266C File Offset: 0x0001086C
	public void SetCrys(int x, int y, string crys, int num, int bid, int delay, int _index)
	{
		this.crys = crys;
		this.num = num;
		this.x = x;
		this.y = y;
		this.bid = bid;
		this.delay = delay;
		this.index = _index;
		if (this.instanceInited)
		{
			this.UpdateModel();
		}
	}

	// Token: 0x060000B1 RID: 177 RVA: 0x000126BC File Offset: 0x000108BC
	private void UpdateModel()
	{
		int num = 0;
		string a = this.crys;
		if (!(a == "g"))
		{
			if (!(a == "r"))
			{
				if (!(a == "v"))
				{
					if (!(a == "b"))
					{
						if (!(a == "w"))
						{
							if (a == "c")
							{
								num = 5;
							}
						}
						else
						{
							num = 4;
						}
					}
					else
					{
						num = 3;
					}
				}
				else
				{
					num = 2;
				}
			}
			else
			{
				num = 1;
			}
		}
		else
		{
			num = 0;
		}
		base.gameObject.transform.position = new Vector3((float)this.x + 0.5f, -(float)this.y - 0.5f, -2f);
		float num2 = 0f;
		this.crysImage.transform.SetParent(base.gameObject.transform);
		this.crysImage.transform.localPosition = new Vector3(num2, 0f, 0f);
		this.crysImage.GetComponent<SpriteRenderer>().sprite = CrysPlusScript.sprites[num];
		num2 += 0.75f;
		if (this.num >= 100)
		{
			this.hundImage.SetActive(true);
			int num3 = Mathf.FloorToInt((float)(this.num / 100));
			this.hundImage.transform.SetParent(base.gameObject.transform);
			this.hundImage.transform.localPosition = new Vector3(num2, 0f, 0f);
			if (num3 == 0)
			{
				num3 = 10;
			}
			this.hundImage.GetComponent<SpriteRenderer>().sprite = CrysPlusScript.sprites[5 + num3];
			this.hundImage.GetComponent<SpriteRenderer>().color = this.colors[num];
			num2 += 0.5f;
		}
		else
		{
			this.hundImage.SetActive(false);
		}
		if (this.num >= 10)
		{
			this.tenImage.SetActive(true);
			int num4 = Mathf.FloorToInt((float)(this.num % 100 / 10));
			this.tenImage.transform.SetParent(base.gameObject.transform);
			this.tenImage.transform.localPosition = new Vector3(num2, 0f, 0f);
			if (num4 == 0)
			{
				num4 = 10;
			}
			this.tenImage.GetComponent<SpriteRenderer>().sprite = CrysPlusScript.sprites[5 + num4];
			this.tenImage.GetComponent<SpriteRenderer>().color = this.colors[num];
			num2 += 0.5f;
		}
		else
		{
			this.tenImage.SetActive(false);
		}
		if (this.num > 1)
		{
			this.singImage.SetActive(true);
			int num5 = Mathf.FloorToInt((float)(this.num % 10));
			this.singImage.transform.SetParent(base.gameObject.transform);
			this.singImage.transform.localPosition = new Vector3(num2, 0f, 0f);
			if (num5 == 0)
			{
				num5 = 10;
			}
			this.singImage.GetComponent<SpriteRenderer>().sprite = CrysPlusScript.sprites[5 + num5];
			this.singImage.GetComponent<SpriteRenderer>().color = this.colors[num];
			num2 += 0.5f;
		}
		else
		{
			this.singImage.SetActive(false);
		}
		this.start = Time.unscaledTime + 0.001f * (float)this.delay;
		this.delayStart = Time.unscaledTime;
	}

	// Token: 0x060000B2 RID: 178 RVA: 0x00012A1C File Offset: 0x00010C1C
	private void Start()
	{
		if (!CrysPlusScript.inited)
		{
			CrysPlusScript.sprites = ResourcesManager.LoadAll<Sprite>("crysfont");
			CrysPlusScript.inited = true;
		}
		this.crysImage = UnityEngine.Object.Instantiate<GameObject>(this.cryPrefab);
		this.hundImage = UnityEngine.Object.Instantiate<GameObject>(this.cryPrefab);
		this.tenImage = UnityEngine.Object.Instantiate<GameObject>(this.cryPrefab);
		this.singImage = UnityEngine.Object.Instantiate<GameObject>(this.cryPrefab);
		this.instanceInited = true;
		this.UpdateModel();
	}

	// Token: 0x060000B3 RID: 179 RVA: 0x00012A98 File Offset: 0x00010C98
	private void Update()
	{
		if (Time.unscaledTime - this.delayStart > 0.001f * (float)this.delay)
		{
			Vector3 position = base.gameObject.transform.position;
			position.z = -2f;
			base.gameObject.transform.position = position;
			Vector3 vector = base.gameObject.transform.position;
			vector.y += 8f * Time.unscaledDeltaTime;
			this.lerpLevel = 1f - 0.5f * (Time.unscaledTime - this.start);
			if (RobotRenderer.THIS.bots.ContainsKey(this.bid))
			{
				vector = this.lerpLevel * vector + (1f - this.lerpLevel) * RobotRenderer.THIS.bots[this.bid].transform.position;
			}
			base.gameObject.transform.position = vector;
			if (Time.unscaledTime > this.start + 0.5f)
			{
				ClientController.THIS.crysPlusPool.Free(this.index);
			}
			return;
		}
		Vector3 position2 = base.gameObject.transform.position;
		position2.z = 2f;
		base.gameObject.transform.position = position2;
	}

	// Token: 0x04000141 RID: 321
	public static bool inited;

	// Token: 0x04000142 RID: 322
	public static Sprite[] sprites;

	// Token: 0x04000143 RID: 323
	public GameObject cryPrefab;

	// Token: 0x04000144 RID: 324
	private string crys = "g";

	// Token: 0x04000145 RID: 325
	private int num;

	// Token: 0x04000146 RID: 326
	private int x;

	// Token: 0x04000147 RID: 327
	private int y;

	// Token: 0x04000148 RID: 328
	private int bid = -1;

	// Token: 0x04000149 RID: 329
	private int delay;

	// Token: 0x0400014A RID: 330
	private int index = -1;

	// Token: 0x0400014B RID: 331
	private bool instanceInited;

	// Token: 0x0400014C RID: 332
	private Color[] colors = new Color[]
	{
		new Color(0.5f, 1f, 0.5f),
		new Color(1f, 0.5f, 0.5f),
		new Color(1f, 0.1f, 1f),
		new Color(0.5f, 0.5f, 1f),
		new Color(1f, 1f, 1f),
		new Color(0.1f, 1f, 1f)
	};

	// Token: 0x0400014D RID: 333
	private GameObject crysImage;

	// Token: 0x0400014E RID: 334
	private GameObject hundImage;

	// Token: 0x0400014F RID: 335
	private GameObject tenImage;

	// Token: 0x04000150 RID: 336
	private GameObject singImage;

	// Token: 0x04000151 RID: 337
	private float start;

	// Token: 0x04000152 RID: 338
	private float delayStart;

	// Token: 0x04000153 RID: 339
	private float lerpLevel = 1f;
}
