using System;
using UnityEngine;

// Token: 0x02000042 RID: 66
public class OverlayRenderer : MonoBehaviour
{
	// Token: 0x060001AD RID: 429 RVA: 0x00005E39 File Offset: 0x00004039
	private void Start()
	{
		OverlayRenderer.THIS = this;
	}

	// Token: 0x060001AE RID: 430 RVA: 0x0001B714 File Offset: 0x00019914
	public void HideGrid()
	{
		foreach (object obj in this.grid.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
	}

	// Token: 0x060001AF RID: 431 RVA: 0x0001B774 File Offset: 0x00019974
	public void AddGrid(int w, int h, int[] codes, int dx, int dy, int d)
	{
		this.dx = dx;
		this.dy = dy;
		this.d = d;
		this.HideGrid();
		for (int i = 0; i < h; i++)
		{
			for (int j = 0; j < w; j++)
			{
				if (codes[j + i * w] > 0)
				{
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.gridCellPrefab);
					gameObject.transform.SetParent(this.grid.transform);
					gameObject.transform.localPosition = new Vector3((float)j, -(float)i, 0f);
				}
			}
		}
	}

	// Token: 0x060001B0 RID: 432 RVA: 0x0001B7FC File Offset: 0x000199FC
	private void Update()
	{
		int num = 0;
		int num2 = 0;
		if (ClientController.THIS.myBot != null)
		{
			switch (ClientController.THIS.myBot.dir)
			{
			case 0:
				num2 = -this.d;
				break;
			case 1:
				num = -this.d;
				break;
			case 2:
				num2 = this.d;
				break;
			case 3:
				num = this.d;
				break;
			}
			this.grid.SetActive(ClientController.THIS.myBot.renderDistance < 0.1f);
		}
		this.grid.transform.position = new Vector3((float)(ClientController.THIS.view_x - this.dx) + 0.5f + (float)num, -(float)ClientController.THIS.view_y + (float)this.dy - 0.5f + (float)num2, -5f);
	}

	// Token: 0x040002C7 RID: 711
	public GameObject grid;

	// Token: 0x040002C8 RID: 712
	public GameObject gridCellPrefab;

	// Token: 0x040002C9 RID: 713
	public static OverlayRenderer THIS;

	// Token: 0x040002CA RID: 714
	private int dx = 1;

	// Token: 0x040002CB RID: 715
	private int dy = 1;

	// Token: 0x040002CC RID: 716
	private int d = 2;
}
