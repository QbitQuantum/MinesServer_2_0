using System;
using UnityEngine;

// Token: 0x02000006 RID: 6
public class BzScript : MonoBehaviour
{
	// Token: 0x06000026 RID: 38 RVA: 0x000097DC File Offset: 0x000079DC
	public void SetAnimation(int type, int _index, Vector3 pos)
	{
		base.transform.position = pos;
		this.frame = 0;
		this.index = _index;
		switch (type)
		{
		case 0:
			this.spriteCollection = BzScript.bzSprites;
			this.framesInAnimation = 56;
			this.updatesPerFrame = 4;
			return;
		case 1:
			this.spriteCollection = BzScript.hurtSprites;
			this.framesInAnimation = 16;
			this.updatesPerFrame = 3;
			return;
		case 2:
			this.spriteCollection = BzScript.expSprites;
			this.framesInAnimation = 40;
			this.updatesPerFrame = 3;
			return;
		default:
			return;
		}
	}

	// Token: 0x06000027 RID: 39 RVA: 0x00004BFB File Offset: 0x00002DFB
	public static void Init()
	{
		if (!BzScript.inited)
		{
			BzScript.bzSprites = ResourcesManager.LoadAll<Sprite>("fx");
			BzScript.hurtSprites = ResourcesManager.LoadAll<Sprite>("hurt");
			BzScript.expSprites = ResourcesManager.LoadAll<Sprite>("explosion");
			BzScript.inited = true;
		}
	}

	// Token: 0x06000028 RID: 40 RVA: 0x00004C37 File Offset: 0x00002E37
	private void Start()
	{
		this.sr = base.GetComponent<SpriteRenderer>();
		this.sr.color = new Color(1f, 1f, 1f);
	}

	// Token: 0x06000029 RID: 41 RVA: 0x00009868 File Offset: 0x00007A68
	private void Update()
	{
		this.frame++;
		if (this.sr != null)
		{
			this.sr.sprite = this.spriteCollection[this.frame / this.updatesPerFrame];
		}
		if (this.frame >= this.framesInAnimation + this.updatesPerFrame - 1)
		{
			this.spriteCollection = BzScript.bzSprites;
			this.framesInAnimation = 56;
			this.updatesPerFrame = 4;
			this.sr.sprite = this.spriteCollection[0];
			ClientController.THIS.bzPool.Free(this.index);
		}
	}

	// Token: 0x0400001C RID: 28
	public static Sprite[] bzSprites;

	// Token: 0x0400001D RID: 29
	public static Sprite[] hurtSprites;

	// Token: 0x0400001E RID: 30
	public static Sprite[] expSprites;

	// Token: 0x0400001F RID: 31
	public static bool inited;

	// Token: 0x04000020 RID: 32
	public static int all_fx_num;

	// Token: 0x04000021 RID: 33
	private int index;

	// Token: 0x04000022 RID: 34
	private int num;

	// Token: 0x04000023 RID: 35
	private int framesInAnimation = 1;

	// Token: 0x04000024 RID: 36
	private int updatesPerFrame = 4;

	// Token: 0x04000025 RID: 37
	private SpriteRenderer sr;

	// Token: 0x04000026 RID: 38
	private Sprite[] spriteCollection;

	// Token: 0x04000027 RID: 39
	private int frame;
}
