using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000033 RID: 51
public class MiniMapViewer : MonoBehaviour
{
	// Token: 0x0600016A RID: 362 RVA: 0x0001A9D0 File Offset: 0x00018BD0
	private void Start()
	{
		MiniMapViewer.THIS = this;
		this.texture = new Texture2D(this.width, this.height, TextureFormat.RGBA32, false);
		this.texture.filterMode = FilterMode.Bilinear;
		this.colors = new Color[this.width * this.height];
		Sprite sprite = Sprite.Create(this.texture, new Rect(0f, 0f, (float)this.width, (float)this.height), new Vector2(0.5f, 0.5f));
		this.mapImage.sprite = sprite;
		this.mapImage.SetNativeSize();
		if (!MapViewer.colorsInited)
		{
			MapViewer.InitColorTable();
		}
	}

	// Token: 0x0600016B RID: 363 RVA: 0x00005AD2 File Offset: 0x00003CD2
	public static Color RGBQ(int r, int g, int b)
	{
		return new Color((float)r / 256f, (float)g / 256f, (float)b / 256f);
	}

	// Token: 0x0600016C RID: 364 RVA: 0x0001AA7C File Offset: 0x00018C7C
	public void Show()
	{
		this.mapX = ClientController.THIS.view_x;
		this.mapY = ClientController.THIS.view_y;
		this.lastBotX = this.mapX;
		this.lastBotY = this.mapY;
		this.UpdateMap();
		base.gameObject.SetActive(true);
	}

	// Token: 0x0600016D RID: 365 RVA: 0x0001AAD4 File Offset: 0x00018CD4
	public void UpdateMap()
	{
		if (TerrainRendererScript.map == null)
		{
			return;
		}
		int num = this.mapX - this.width / 2;
		int num2 = this.mapY + this.height / 2;
		for (int i = 0; i < this.width; i++)
		{
			for (int j = 0; j < this.height; j++)
			{
				this.colors[i + j * this.width] = MapViewer.colorTable[TerrainRendererScript.map.GetCell(num + i, num2 - j)];
			}
		}
		this.texture.SetPixels(this.colors);
		this.texture.Apply();
	}

	// Token: 0x0600016E RID: 366 RVA: 0x0001AB78 File Offset: 0x00018D78
	private void Update()
	{
		if (base.gameObject.activeSelf && this.lastUpdateTime < Time.time - 0.05f)
		{
			this.UpdateMap();
			this.lastUpdateTime = Time.time;
		}
		this.mapX = ClientController.THIS.view_x;
		this.mapY = ClientController.THIS.view_y;
	}

	// Token: 0x04000284 RID: 644
	public Image mapImage;

	// Token: 0x04000285 RID: 645
	private Texture2D texture;

	// Token: 0x04000286 RID: 646
	private Color[] colors;

	// Token: 0x04000287 RID: 647
	private int width = 100;

	// Token: 0x04000288 RID: 648
	private int height = 100;

	// Token: 0x04000289 RID: 649
	public static MiniMapViewer THIS;

	// Token: 0x0400028A RID: 650
	private float lastUpdateTime;

	// Token: 0x0400028B RID: 651
	private int mapX;

	// Token: 0x0400028C RID: 652
	private int mapY;

	// Token: 0x0400028D RID: 653
	private int lastBotX;

	// Token: 0x0400028E RID: 654
	private int lastBotY;
}
