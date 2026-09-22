using System;
using UnityEngine;

// Token: 0x02000015 RID: 21
public class CrysAutScript : MonoBehaviour
{
	// Token: 0x060000AC RID: 172 RVA: 0x0000516B File Offset: 0x0000336B
	public void SetCrys(int x, int y, string crys, int dx, int dy)
	{
		this.crys = crys;
		this.x = x;
		this.y = y;
		this.dy = dx - 50;
		this.dx = dy - 50;
	}

	// Token: 0x060000AD RID: 173 RVA: 0x000123A4 File Offset: 0x000105A4
	private void Start()
	{
		if (!CrysAutScript.inited)
		{
			CrysAutScript.sprites = ResourcesManager.LoadAll<Sprite>("crysfont");
			CrysAutScript.inited = true;
		}
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
		base.gameObject.transform.position = new Vector3((float)(this.x + this.dx) + 0.5f, (float)(-(float)this.y - this.dy) - 0.5f, -2f);
		float num2 = 0f;
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.cryPrefab);
		gameObject.transform.SetParent(base.gameObject.transform);
		gameObject.transform.localPosition = new Vector3(num2, 0f, 0f);
		gameObject.GetComponent<SpriteRenderer>().sprite = CrysAutScript.sprites[num];
		num2 += 0.75f;
		this.start = Time.unscaledTime;
		this.delayStart = Time.unscaledTime;
	}

	// Token: 0x060000AE RID: 174 RVA: 0x000124F0 File Offset: 0x000106F0
	private void Update()
	{
		Vector3 vector = base.gameObject.transform.position;
		this.lerpLevel = 0.9f;
		vector = this.lerpLevel * vector + (1f - this.lerpLevel) * new Vector3((float)this.x + 0.5f, -(float)this.y - 0.5f, -2f);
		base.gameObject.transform.position = vector;
		if (Time.unscaledTime > this.start + 0.5f)
		{
			UnityEngine.Object.Destroy(base.gameObject);
		}
	}

	// Token: 0x04000134 RID: 308
	public static bool inited;

	// Token: 0x04000135 RID: 309
	public static Sprite[] sprites;

	// Token: 0x04000136 RID: 310
	public GameObject cryPrefab;

	// Token: 0x04000137 RID: 311
	private string crys = "g";

	// Token: 0x04000138 RID: 312
	private int num;

	// Token: 0x04000139 RID: 313
	private int x;

	// Token: 0x0400013A RID: 314
	private int y;

	// Token: 0x0400013B RID: 315
	private int dx;

	// Token: 0x0400013C RID: 316
	private int dy;

	// Token: 0x0400013D RID: 317
	private Color[] colors = new Color[]
	{
		new Color(0.5f, 1f, 0.5f),
		new Color(1f, 0.5f, 0.5f),
		new Color(1f, 0.1f, 1f),
		new Color(0.5f, 0.5f, 1f),
		new Color(1f, 1f, 1f),
		new Color(0.1f, 1f, 1f)
	};

	// Token: 0x0400013E RID: 318
	private float start;

	// Token: 0x0400013F RID: 319
	private float delayStart;

	// Token: 0x04000140 RID: 320
	private float lerpLevel = 1f;
}
