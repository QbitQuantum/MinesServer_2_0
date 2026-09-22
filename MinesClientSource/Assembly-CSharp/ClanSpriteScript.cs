using System;
using UnityEngine;

// Token: 0x0200000F RID: 15
public class ClanSpriteScript : MonoBehaviour
{
	// Token: 0x0600005B RID: 91 RVA: 0x00004EDA File Offset: 0x000030DA
	public static void Init()
	{
		if (!ClanSpriteScript.inited)
		{
			ClanSpriteScript.sprites = ResourcesManager.LoadAll<Sprite>("clans");
			ClanSpriteScript.inited = true;
		}
	}

	// Token: 0x0600005C RID: 92 RVA: 0x00004EF8 File Offset: 0x000030F8
	private void Start()
	{
		this.changeClanToId(-1);
	}

	// Token: 0x0600005D RID: 93 RVA: 0x0000CE70 File Offset: 0x0000B070
	public void changeClanToId(int id)
	{
		if (id == -1)
		{
			if (this._id >= 0)
			{
				id = this._id;
			}
			else
			{
				id = 0;
			}
		}
		else if (this._id == id)
		{
			return;
		}
		this._id = id;
		if (id == 0)
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		base.GetComponent<SpriteRenderer>().sprite = ClanSpriteScript.sprites[id - 1];
	}

	// Token: 0x0600005E RID: 94 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000063 RID: 99
	public static Sprite[] sprites;

	// Token: 0x04000064 RID: 100
	public static bool inited;

	// Token: 0x04000065 RID: 101
	private int _id = -1;
}
