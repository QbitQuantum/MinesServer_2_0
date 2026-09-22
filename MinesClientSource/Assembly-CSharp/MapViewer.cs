using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000030 RID: 48
public class MapViewer : MonoBehaviour
{
	// Token: 0x06000152 RID: 338 RVA: 0x00017FD8 File Offset: 0x000161D8
	public static void ResetCustomTable()
	{
		for (int i = 0; i < 255; i++)
		{
			int num = i + 50;
			MapViewer.customTable[i] = new Color((float)num / 312f, (float)num / 312f, (float)num / 312f);
			MapViewer.customBlinkRateTable[i] = 0f;
			bool flag = i <= 39;
			if (flag)
			{
				MapViewer.customTable[i] = new Color(0.04f, 0.045f, 0.045f, 1f);
			}
		}
	}

	// Token: 0x06000153 RID: 339 RVA: 0x00018068 File Offset: 0x00016268
	public static void InitColorTable()
	{
		MapViewer.ResetCustomTable();
		for (int i = 0; i < 255; i++)
		{
			MapViewer.colorTable[i] = new Color((float)i / 512f, (float)i / 256f, 0.01f);
			MapViewer.aliveColorTable[i] = new Color((float)i / 512f, (float)i / 256f, 0.01f);
			MapViewer.transparentTable[i] = new Color(0f, 0f, 0f, 1f);
			bool flag = i > 39;
			if (flag)
			{
				MapViewer.transparentTable[i] = new Color(0.4f, 0.45f, 0.45f, 1f);
			}
			bool flag2 = i < 120 && CellRender.isSandCache[i];
			if (flag2)
			{
				MapViewer.transparentTable[i] = new Color(0.3f, 0.35f, 0.35f, 1f);
			}
		}
		MapViewer.transparentTable[35] = new Color(0.4f, 0.01f, 0.1f, 1f);
		MapViewer.transparentTable[36] = new Color(0.4f, 0.1f, 0.01f, 1f);
		MapViewer.transparentTable[39] = new Color(0.4f, 0.01f, 0.01f, 1f);
		MapViewer.transparentTable[30] = new Color(1f, 1f, 0f, 1f);
		MapViewer.transparentTable[114] = new Color(0.6f, 0.6f, 0.6f, 1f);
		MapViewer.transparentTable[115] = new Color(0.6f, 0.6f, 0.6f, 1f);
		MapViewer.transparentTable[117] = new Color(1f, 1f, 1f, 1f);
		MapViewer.transparentTable[106] = new Color(1f, 1f, 1f, 1f);
		MapViewer.transparentTable[119] = new Color(0.8f, 1f, 1f, 1f);
		MapViewer.transparentTable[80] = new Color(0f, 1f, 1f, 1f);
		MapViewer.transparentTable[81] = new Color(0f, 0.7f, 0.7f, 1f);
		MapViewer.colorTable[0] = new Color(0f, 0f, 0f, 0.5f);
		MapViewer.colorTable[1] = new Color(0f, 0f, 0f, 0.5f);
		MapViewer.colorTable[32] = MapViewer.RGBQ(0, 0, 0);
		MapViewer.colorTable[33] = MapViewer.RGBQ(15, 11, 3);
		MapViewer.colorTable[34] = MapViewer.RGBQ(29, 25, 18);
		MapViewer.colorTable[35] = MapViewer.RGBQ(68, 68, 68);
		MapViewer.colorTable[36] = MapViewer.RGBQ(85, 68, 34);
		MapViewer.colorTable[37] = MapViewer.RGBQ(68, 0, 0);
		MapViewer.colorTable[38] = MapViewer.RGBQ(51, 68, 0);
		MapViewer.colorTable[40] = MapViewer.RGBQ(255, 97, 107);
		MapViewer.colorTable[41] = MapViewer.RGBQ(255, 107, 97);
		MapViewer.colorTable[42] = MapViewer.RGBQ(255, 107, 107);
		MapViewer.colorTable[43] = MapViewer.RGBQ(255, 187, 251);
		MapViewer.colorTable[44] = MapViewer.RGBQ(191, 241, 251);
		MapViewer.colorTable[45] = MapViewer.RGBQ(207, 203, 241);
		MapViewer.colorTable[48] = MapViewer.RGBQ(255, 255, 255);
		MapViewer.colorTable[49] = MapViewer.RGBQ(101, 150, 126);
		MapViewer.colorTable[50] = MapViewer.RGBQ(101, 255, 255);
		MapViewer.aliveColorTable[50] = MapViewer.RGBQ(101, 255, 255);
		MapViewer.colorTable[51] = MapViewer.RGBQ(255, 51, 51);
		MapViewer.aliveColorTable[51] = MapViewer.RGBQ(255, 51, 51);
		MapViewer.colorTable[52] = MapViewer.RGBQ(255, 101, 255);
		MapViewer.aliveColorTable[52] = MapViewer.RGBQ(255, 101, 255);
		MapViewer.colorTable[53] = MapViewer.RGBQ(34, 101, 255);
		MapViewer.aliveColorTable[53] = MapViewer.RGBQ(255, 138, 255);
		MapViewer.colorTable[54] = MapViewer.RGBQ(238, 254, 255);
		MapViewer.aliveColorTable[54] = MapViewer.RGBQ(238, 254, 255);
		MapViewer.colorTable[55] = MapViewer.RGBQ(238, 254, 255);
		MapViewer.aliveColorTable[55] = MapViewer.RGBQ(238, 254, 255);
		MapViewer.colorTable[56] = MapViewer.RGBQ(225, 254, 255);
		MapViewer.colorTable[57] = MapViewer.RGBQ(226, 254, 255);
		MapViewer.colorTable[58] = MapViewer.RGBQ(227, 254, 255);
		MapViewer.colorTable[59] = MapViewer.RGBQ(228, 254, 255);
		MapViewer.colorTable[60] = MapViewer.RGBQ(204, 204, 204);
		MapViewer.colorTable[61] = MapViewer.RGBQ(221, 221, 221);
		MapViewer.colorTable[62] = MapViewer.RGBQ(255, 204, 204);
		MapViewer.colorTable[63] = MapViewer.RGBQ(255, 221, 221);
		MapViewer.colorTable[64] = MapViewer.RGBQ(170, 170, 170);
		MapViewer.colorTable[65] = MapViewer.RGBQ(187, 187, 187);
		MapViewer.colorTable[66] = MapViewer.RGBQ(184, 153, 51);
		MapViewer.colorTable[67] = MapViewer.RGBQ(184, 136, 187);
		MapViewer.colorTable[68] = MapViewer.RGBQ(119, 68, 68);
		MapViewer.colorTable[69] = MapViewer.RGBQ(34, 68, 153);
		MapViewer.colorTable[70] = MapViewer.RGBQ(243, 241, 152);
		MapViewer.colorTable[71] = MapViewer.RGBQ(71, 215, 100);
		MapViewer.colorTable[72] = MapViewer.RGBQ(101, 134, 247);
		MapViewer.colorTable[73] = MapViewer.RGBQ(247, 82, 67);
		MapViewer.colorTable[74] = MapViewer.RGBQ(132, 238, 247);
		MapViewer.colorTable[75] = MapViewer.RGBQ(255, 135, 231);
		MapViewer.colorTable[82] = MapViewer.RGBQ(17, 102, 102);
		MapViewer.colorTable[83] = MapViewer.RGBQ(50, 135, 152);
		MapViewer.colorTable[86] = MapViewer.RGBQ(184, 255, 17);
		MapViewer.colorTable[90] = MapViewer.RGBQ(238, 238, 238);
		MapViewer.colorTable[91] = MapViewer.RGBQ(255, 90, 0);
		MapViewer.colorTable[92] = MapViewer.RGBQ(193, 187, 187);
		MapViewer.colorTable[93] = MapViewer.RGBQ(187, 193, 187);
		MapViewer.colorTable[94] = MapViewer.RGBQ(187, 187, 193);
		MapViewer.colorTable[95] = MapViewer.RGBQ(184, 255, 34);
		MapViewer.colorTable[96] = MapViewer.RGBQ(184, 255, 68);
		MapViewer.colorTable[97] = MapViewer.RGBQ(112, 160, 183);
		MapViewer.colorTable[98] = MapViewer.RGBQ(112, 187, 207);
		MapViewer.colorTable[99] = MapViewer.RGBQ(219, 209, 125);
		MapViewer.colorTable[100] = MapViewer.RGBQ(181, 168, 57);
		MapViewer.colorTable[101] = MapViewer.RGBQ(76, 191, 0);
		MapViewer.colorTable[102] = MapViewer.RGBQ(208, 206, 0);
		MapViewer.colorTable[103] = MapViewer.RGBQ(133, 81, 166);
		MapViewer.colorTable[104] = MapViewer.RGBQ(153, 153, 136);
		MapViewer.colorTable[105] = MapViewer.RGBQ(198, 0, 0);
		MapViewer.colorTable[106] = MapViewer.RGBQ(136, 136, 136);
		MapViewer.colorTable[107] = MapViewer.RGBQ(8, 215, 100);
		MapViewer.colorTable[108] = MapViewer.RGBQ(255, 0, 0);
		MapViewer.colorTable[109] = MapViewer.RGBQ(0, 0, 255);
		MapViewer.colorTable[110] = MapViewer.RGBQ(255, 0, 255);
		MapViewer.colorTable[111] = MapViewer.RGBQ(238, 238, 255);
		MapViewer.colorTable[112] = MapViewer.RGBQ(0, 255, 255);
		MapViewer.colorTable[113] = MapViewer.RGBQ(211, 159, 166);
		MapViewer.colorTable[114] = MapViewer.RGBQ(119, 119, 119);
		MapViewer.colorTable[115] = MapViewer.RGBQ(56, 118, 65);
		MapViewer.colorTable[116] = MapViewer.RGBQ(17, 17, 255);
		MapViewer.aliveColorTable[116] = MapViewer.RGBQ(161, 162, 255);
		MapViewer.colorTable[117] = MapViewer.RGBQ(170, 119, 119);
		MapViewer.colorTable[118] = MapViewer.RGBQ(100, 98, 21);
		MapViewer.colorTable[119] = MapViewer.RGBQ(170, 255, 255);
		MapViewer.aliveColorTable[119] = MapViewer.RGBQ(170, 255, 255);
		MapViewer.colorTable[120] = MapViewer.RGBQ(227, 191, 120);
		MapViewer.colorTable[121] = MapViewer.RGBQ(163, 136, 72);
		MapViewer.colorTable[122] = MapViewer.RGBQ(51, 153, 120);
		MapViewer.colorsInited = true;
	}

	// Token: 0x06000154 RID: 340 RVA: 0x00018CC8 File Offset: 0x00016EC8
	private void Start()
	{
		MapViewer.THIS = this;
		this.texture = new Texture2D(this.width, this.height, TextureFormat.RGBA32, false);
		this.texture.filterMode = FilterMode.Point;
		this.colors = new Color[this.width * this.height];
		this.texture_x2 = new Texture2D(2 * this.width, 2 * this.height, TextureFormat.RGBA32, false);
		this.texture_x2.filterMode = FilterMode.Point;
		this.colors_x2 = new Color[4 * this.width * this.height];
		this.mapSprite_x2 = Sprite.Create(this.texture_x2, new Rect(0f, 0f, (float)(2 * this.width), (float)(2 * this.height)), new Vector2(0.5f, 0.5f));
		this.mapSprite = Sprite.Create(this.texture, new Rect(0f, 0f, (float)this.width, (float)this.height), new Vector2(0.5f, 0.5f));
		this.mapImage.sprite = this.mapSprite;
		this.mapImage.SetNativeSize();
		this.mapImage.rectTransform.sizeDelta = new Vector2((float)(this.width * 2), (float)(this.height * 2));
		base.gameObject.SetActive(false);
		this.exitButton.onClick.AddListener(new UnityAction(this.OnExit));
		bool flag = !MapViewer.colorsInited;
		if (flag)
		{
			MapViewer.InitColorTable();
		}
		this.allmapButton.onClick.AddListener(new UnityAction(this.OnClick));
		this.LoadNotes();
	}

	// Token: 0x06000155 RID: 341 RVA: 0x00018E84 File Offset: 0x00017084
	public static Color RGBQ(int r, int g, int b)
	{
		return new Color((float)r / 256f, (float)g / 256f, (float)b / 256f);
	}

	// Token: 0x06000156 RID: 342 RVA: 0x00005A22 File Offset: 0x00003C22
	public void OnExit()
	{
		TutorialNavigation.CheckHide("_CMAP");
		ClientController.CanGoto = false;
		base.gameObject.SetActive(false);
		this.SaveNotes();
	}

	// Token: 0x06000157 RID: 343 RVA: 0x00018EB4 File Offset: 0x000170B4
	public void OnClick()
	{
		bool flag = this.lastClickTime + 0.5f < Time.unscaledTime;
		if (flag)
		{
			this.lastClickTime = Time.unscaledTime;
		}
		else
		{
			bool flag2 = this.cursDx > 0 && this.cursDy > 0 && this.lastdrag < 5f;
			if (flag2)
			{
				ClientConfig.mouseNoDig = true;
				ClientConfig.mouseR = ClientConfig.mouseMapR;
				ClientConfig.mouseMaxLen = ClientConfig.mouseMapMaxLen;
				ClientConfig.mouseMaxStack = ClientConfig.mouseMapMaxStack;
				ClientController.THIS.FromMapGoto(this.cursDx, this.cursDy);
				this.mapX = ClientController.THIS.view_x;
				this.mapY = ClientController.THIS.view_y;
				ClientConfig.mouseNoDig = false;
				ClientConfig.mouseR = ClientConfig.mouseDefR;
				ClientConfig.mouseMaxLen = ClientConfig.mouseDefMaxLen;
				ClientConfig.mouseMaxStack = ClientConfig.mouseDefMaxStack;
			}
		}
	}

	// Token: 0x06000158 RID: 344 RVA: 0x00018F94 File Offset: 0x00017194
	public void Show()
	{
		bool flag = !base.gameObject.activeSelf;
		if (flag)
		{
			this.isDragging = false;
			this.mapX = ClientController.THIS.view_x;
			this.mapY = ClientController.THIS.view_y;
			this.lastBotX = this.mapX;
			this.lastBotY = this.mapY;
			this.UpdateMap();
			base.gameObject.SetActive(true);
		}
		else
		{
			this.OnExit();
		}
	}

	// Token: 0x06000159 RID: 345 RVA: 0x00019014 File Offset: 0x00017214
	public void UpdateMap()
	{
		int num = this.width;
		int num2 = this.height;
		Color[] array = this.colors;
		bool flag = this.modeDropdown.value == 1;
		if (flag)
		{
			num *= 2;
			num2 *= 2;
			this.mapImage.sprite = this.mapSprite_x2;
			array = this.colors_x2;
		}
		else
		{
			this.mapImage.sprite = this.mapSprite;
		}
		int num3 = this.mapX - num / 2;
		int num4 = this.mapY + num2 / 2;
		for (int i = 0; i < num; i++)
		{
			for (int j = 0; j < num2; j++)
			{
				bool flag2 = num4 - j < 0;
				if (flag2)
				{
					float num5 = 0.3f * Mathf.Max(1f - (float)(j - num4) / 100f, 0f);
					bool flag3 = this.modeDropdown.value == 1;
					if (flag3)
					{
						array[i + j * num] = new Color(num5 * 0.5f, num5, 0f);
					}
					else
					{
						this.colors[i + j * num] = new Color(num5 * 0.5f, num5, 0f);
					}
				}
				else
				{
					bool flag4 = this.modeDropdown.value == 2;
					if (flag4)
					{
						this.colors[i + j * num] = MapViewer.aliveColorTable[TerrainRendererScript.map.GetCell(num3 + i, num4 - j)];
						bool flag5 = ((num3 + i) / 32 + (num4 - j) / 32) % 2 == 0;
						if (flag5)
						{
							Color[] array2 = this.colors;
							int num6 = i + j * num;
							array2[num6].b = array2[num6].b + 0.3f;
						}
					}
					else
					{
						bool flag6 = this.modeDropdown.value == 3;
						if (flag6)
						{
							int num7 = this.lastBotX;
							int num8 = this.lastBotY;
							this.colors[i + j * num] = MapViewer.transparentTable[TerrainRendererScript.map.GetCell(num3 + i, num4 - j)];
						}
						else
						{
							bool flag7 = this.modeDropdown.value < 2;
							if (flag7)
							{
								array[i + j * num] = MapViewer.colorTable[TerrainRendererScript.map.GetCell(num3 + i, num4 - j)];
							}
							else
							{
								bool flag8 = this.modeDropdown.value == 4;
								if (flag8)
								{
									int cell = TerrainRendererScript.map.GetCell(num3 + i, num4 - j);
									bool flag9 = MapViewer.customBlinkRateTable[cell] == 0f;
									if (flag9)
									{
										array[i + j * num] = MapViewer.customTable[cell];
									}
									else
									{
										array[i + j * num] = MapViewer.customTable[cell] * (0.5f + 0.5f * Mathf.Sin(Time.time * MapViewer.customBlinkRateTable[cell]));
									}
								}
							}
						}
					}
				}
			}
		}
		this.DrawNotes(array, num, num2, num3, num4);
		bool flag10 = this.modeDropdown.value == 1;
		if (flag10)
		{
			this.texture_x2.SetPixels(array);
			this.texture_x2.Apply();
		}
		else
		{
			this.texture.SetPixels(this.colors);
			this.texture.Apply();
		}
	}

	// Token: 0x0600015A RID: 346 RVA: 0x000193AC File Offset: 0x000175AC
	private void Update()
	{
		bool flag = !WorldInitScript.inited;
		if (!flag)
		{
			bool flag2 = (this.isNotePlacementMode || this.showNotesList) && (Input.mousePosition.x > (float)(Screen.width - 320) || (this.isNotePlacementMode && Input.mousePosition.x > (float)(Screen.width / 2 - 150) && Input.mousePosition.x < (float)(Screen.width / 2 + 150) && Input.mousePosition.y > (float)(Screen.height / 2 - 100) && Input.mousePosition.y < (float)(Screen.height / 2 + 100)));
			bool flag3 = this.modeDropdown.value == 1;
			if (flag3)
			{
				this.marker.transform.localPosition = new Vector3(-1f * (float)(this.mapX - ClientController.THIS.view_x), 1f * (float)(this.mapY - ClientController.THIS.view_y));
			}
			else
			{
				this.marker.transform.localPosition = new Vector3(-2f * (float)(this.mapX - ClientController.THIS.view_x), 2f * (float)(this.mapY - ClientController.THIS.view_y));
			}
			this.marker.color = new Color(1f, 0f, 0f, 0.5f + 0.5f * Mathf.Sin(15f * Time.time));
			this.UpdateTrackingNotes();
			bool flag4 = base.gameObject.activeSelf && this.lastUpdateTime < Time.time - 0.05f;
			if (flag4)
			{
				this.UpdateMap();
				this.lastUpdateTime = Time.time;
				bool flag5 = !flag2;
				if (flag5)
				{
					int num = (int)(Input.mousePosition.x - this.mapImage.transform.position.x);
					int num2 = (int)(Input.mousePosition.y - this.mapImage.transform.position.y);
					bool flag6 = this.modeDropdown.value == 3;
					if (flag6)
					{
						this.marker.gameObject.SetActive(false);
						this.mapImage.color = new Color(1f, 1f, 1f, 1f);
						base.gameObject.GetComponent<Image>().color = new Color(0f, 0f, 0f, 0f);
					}
					else
					{
						this.marker.gameObject.SetActive(true);
						this.mapImage.color = new Color(1f, 1f, 1f, 1f);
						base.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
					}
					bool flag7 = this.modeDropdown.value != 1;
					if (flag7)
					{
						num /= 2;
						num2 /= 2;
					}
					num += this.mapX;
					num2 = this.mapY - num2;
					this.CoordText.text = num + ":" + num2;
					this.cursDx = num;
					this.cursDy = num2;
				}
			}
			bool activeSelf = base.gameObject.activeSelf;
			if (activeSelf)
			{
				bool flag8 = Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter);
				if (flag8)
				{
					bool flag9 = this.isNotePlacementMode;
					if (flag9)
					{
						this.PlaceNoteAtCurrentPosition();
					}
				}
				bool keyDown = Input.GetKeyDown(KeyCode.Escape);
				if (keyDown)
				{
					bool flag10 = this.isNotePlacementMode;
					if (flag10)
					{
						this.CancelNotePlacement();
					}
					else
					{
						this.OnExit();
					}
				}
				bool keyDown2 = Input.GetKeyDown(KeyCode.N);
				if (keyDown2)
				{
					bool flag11 = !this.isNotePlacementMode;
					if (flag11)
					{
						this.EnableNotePlacementMode();
					}
					else
					{
						this.CancelNotePlacement();
					}
				}
				bool keyDown3 = Input.GetKeyDown(KeyCode.M);
				if (keyDown3)
				{
					this.showNotesList = !this.showNotesList;
				}
				bool flag12 = !flag2;
				if (flag12)
				{
					bool flag13 = Input.GetMouseButtonDown(0) && !this.isDragging;
					if (flag13)
					{
						this.isDragging = true;
						this.startMapX = this.mapX;
						this.startMapY = this.mapY;
						this.startMouseX = (int)Input.mousePosition.x;
						this.startMouseY = (int)Input.mousePosition.y;
						this.lastdrag = 0f;
					}
					bool mouseButtonUp = Input.GetMouseButtonUp(0);
					if (mouseButtonUp)
					{
						this.isDragging = false;
						this.lastdrag = Mathf.Abs(Input.mousePosition.x - (float)this.startMouseX) + Mathf.Abs(Input.mousePosition.y - (float)this.startMouseY);
					}
					bool flag14 = Input.GetMouseButton(1) && this.cursDx != 0 && this.cursDy != 0;
					if (flag14)
					{
						ClientController.THIS.FromMapGoto(this.cursDx, this.cursDy);
					}
					bool flag15 = this.isDragging;
					if (flag15)
					{
						bool flag16 = this.modeDropdown.value == 1;
						if (flag16)
						{
							this.mapX = this.startMapX - (int)((Input.mousePosition.x - (float)this.startMouseX) / 1f);
							this.mapY = this.startMapY + (int)((Input.mousePosition.y - (float)this.startMouseY) / 1f);
						}
						else
						{
							this.mapX = this.startMapX - (int)((Input.mousePosition.x - (float)this.startMouseX) / 2f);
							this.mapY = this.startMapY + (int)((Input.mousePosition.y - (float)this.startMouseY) / 2f);
						}
						this.lastdrag = Mathf.Abs(Input.mousePosition.x - (float)this.startMouseX) + Mathf.Abs(Input.mousePosition.y - (float)this.startMouseY);
					}
				}
				bool flag17 = this.lastBotX != ClientController.THIS.myBot.gx || this.lastBotY != ClientController.THIS.myBot.gy;
				if (flag17)
				{
					this.mapX += ClientController.THIS.myBot.gx - this.lastBotX;
					this.mapY += ClientController.THIS.myBot.gy - this.lastBotY;
					this.lastBotX = ClientController.THIS.myBot.gx;
					this.lastBotY = ClientController.THIS.myBot.gy;
				}
			}
		}
	}

	// Token: 0x0600015D RID: 349 RVA: 0x00019B50 File Offset: 0x00017D50
	private void DrawNotes(Color[] colorsArray, int width, int height, int startX, int startY)
	{
		foreach (MapViewer.MapNote mapNote in this.notes)
		{
			int num = mapNote.x - startX;
			int num2 = startY - mapNote.y;
			bool flag = num >= 0 && num < width && num2 >= 0 && num2 < height;
			if (flag)
			{
				int num3 = num + num2 * width;
				bool flag2 = num3 >= 0 && num3 < colorsArray.Length;
				if (flag2)
				{
					Color noteColor = this.GetNoteColor(mapNote);
					bool flag3 = this.modeDropdown.value == 1;
					if (flag3)
					{
						for (int i = 0; i < 2; i++)
						{
							for (int j = 0; j < 2; j++)
							{
								int num4 = num * 2 + i + (num2 * 2 + j) * width;
								bool flag4 = num4 < colorsArray.Length;
								if (flag4)
								{
									colorsArray[num4] = noteColor;
								}
							}
						}
					}
					else
					{
						colorsArray[num3] = noteColor;
						bool flag5 = num > 0;
						if (flag5)
						{
							colorsArray[num3 - 1] = Color.Lerp(colorsArray[num3 - 1], noteColor, 0.5f);
						}
						bool flag6 = num < width - 1;
						if (flag6)
						{
							colorsArray[num3 + 1] = Color.Lerp(colorsArray[num3 + 1], noteColor, 0.5f);
						}
						bool flag7 = num2 > 0;
						if (flag7)
						{
							colorsArray[num3 - width] = Color.Lerp(colorsArray[num3 - width], noteColor, 0.5f);
						}
						bool flag8 = num2 < height - 1;
						if (flag8)
						{
							colorsArray[num3 + width] = Color.Lerp(colorsArray[num3 + width], noteColor, 0.5f);
						}
					}
				}
			}
		}
	}

	// Token: 0x0600015E RID: 350 RVA: 0x00019D3C File Offset: 0x00017F3C
	private Color GetNoteColor(MapViewer.MapNote note)
	{
		bool isTracking = note.isTracking;
		Color result;
		if (isTracking)
		{
			result = Color.cyan;
		}
		else
		{
			bool flag = note.type == MapViewer.NoteType.Danger;
			if (flag)
			{
				result = Color.red;
			}
			else
			{
				bool flag2 = note.type == MapViewer.NoteType.Resource;
				if (flag2)
				{
					result = Color.green;
				}
				else
				{
					bool flag3 = note.type == MapViewer.NoteType.Base;
					if (flag3)
					{
						result = Color.blue;
					}
					else
					{
						result = Color.yellow;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x0600015F RID: 351 RVA: 0x00019DA8 File Offset: 0x00017FA8
	private void OnGUI()
	{
		bool flag = !base.gameObject.activeSelf;
		if (!flag)
		{
			bool flag2 = this.isNotePlacementMode;
			if (flag2)
			{
				int num = (ClientController.THIS.myBot != null) ? ClientController.THIS.myBot.gx : this.cursDx;
				int num2 = (ClientController.THIS.myBot != null) ? ClientController.THIS.myBot.gy : this.cursDy;
				GUI.Box(new Rect((float)(Screen.width / 2 - 150), (float)(Screen.height / 2 - 100), 300f, 200f), "Add Note");
				GUI.Label(new Rect((float)(Screen.width / 2 - 130), (float)(Screen.height / 2 - 70), 260f, 25f), string.Format("Position (Bot): {0}, {1}", num, num2));
				GUI.Label(new Rect((float)(Screen.width / 2 - 130), (float)(Screen.height / 2 - 40), 100f, 25f), "Note text:");
				this.pendingNoteText = GUI.TextField(new Rect((float)(Screen.width / 2 - 30), (float)(Screen.height / 2 - 40), 160f, 25f), this.pendingNoteText);
				this.trackSelf = GUI.Toggle(new Rect((float)(Screen.width / 2 - 130), (float)(Screen.height / 2 - 5), 150f, 25f), this.trackSelf, "Track my position");
				GUI.Label(new Rect((float)(Screen.width / 2 - 130), (float)(Screen.height / 2 + 20), 80f, 25f), "Type:");
				string[] texts = new string[]
				{
					"General",
					"Danger",
					"Resource",
					"Base"
				};
				this.selectedNoteType = GUI.Toolbar(new Rect((float)(Screen.width / 2 - 50), (float)(Screen.height / 2 + 20), 180f, 25f), this.selectedNoteType, texts);
				bool flag3 = GUI.Button(new Rect((float)(Screen.width / 2 - 130), (float)(Screen.height / 2 + 55), 120f, 30f), "Place Note");
				if (flag3)
				{
					this.PlaceNoteAtCurrentPosition();
				}
				bool flag4 = GUI.Button(new Rect((float)(Screen.width / 2 + 10), (float)(Screen.height / 2 + 55), 120f, 30f), "Cancel");
				if (flag4)
				{
					this.CancelNotePlacement();
				}
			}
			bool flag5 = this.showNotesList;
			if (flag5)
			{
				Rect position = new Rect((float)(Screen.width - 320), 10f, 310f, (float)Mathf.Min(this.notes.Count * 25 + 50, 400));
				GUI.Box(position, "Notes List (Press M to close)");
				bool flag6 = this.notes.Count == 0;
				if (flag6)
				{
					GUI.Label(new Rect((float)(Screen.width - 300), 40f, 280f, 25f), "No notes yet. Press N to add note.");
				}
				else
				{
					this.scrollPosition = GUI.BeginScrollView(new Rect((float)(Screen.width - 310), 40f, 300f, position.height - 50f), this.scrollPosition, new Rect(0f, 0f, 280f, (float)(this.notes.Count * 25)));
					for (int i = 0; i < this.notes.Count; i++)
					{
						MapViewer.MapNote mapNote = this.notes[i];
						Color color = GUI.color;
						GUI.color = this.GetNoteColor(mapNote);
						Rect position2 = new Rect(5f, (float)(i * 25), 200f, 22f);
						string text = "";
						bool flag7 = mapNote.type == MapViewer.NoteType.Danger;
						if (flag7)
						{
							text = "⚠️ ";
						}
						else
						{
							bool flag8 = mapNote.type == MapViewer.NoteType.Resource;
							if (flag8)
							{
								text = "\ud83d\udcb0 ";
							}
							else
							{
								bool flag9 = mapNote.type == MapViewer.NoteType.Base;
								if (flag9)
								{
									text = "\ud83c\udfe0 ";
								}
							}
						}
						string text2 = mapNote.isTracking ? "\ud83d\udccd " : "";
						bool flag10 = GUI.Button(position2, string.Format("{0}{1}[{2},{3}] {4}", new object[]
						{
							text,
							text2,
							mapNote.x,
							mapNote.y,
							mapNote.text
						}));
						if (flag10)
						{
							ClientController.THIS.FromMapGoto(mapNote.x, mapNote.y);
							this.mapX = mapNote.x;
							this.mapY = mapNote.y;
							this.UpdateMap();
						}
						GUI.color = Color.red;
						bool flag11 = GUI.Button(new Rect(210f, (float)(i * 25), 30f, 22f), "X");
						if (flag11)
						{
							this.notes.RemoveAt(i);
							this.UpdateMap();
							this.SaveNotes();
						}
						GUI.color = color;
					}
					GUI.EndScrollView();
				}
				bool flag12 = GUI.Button(new Rect((float)(Screen.width - 300), position.height - 35f, 280f, 25f), "Clear All Notes");
				if (flag12)
				{
					this.notes.Clear();
					this.UpdateMap();
					this.SaveNotes();
				}
			}
			GUI.Label(new Rect(10f, (float)(Screen.height - 45), 300f, 20f), "N - Add note | M - Show notes");
			bool flag13 = this.isNotePlacementMode;
			if (flag13)
			{
				GUI.Label(new Rect(10f, (float)(Screen.height - 25), 300f, 20f), "Note mode ACTIVE - Press Enter to place on bot position");
			}
		}
	}

	// Token: 0x06000160 RID: 352 RVA: 0x00005A4A File Offset: 0x00003C4A
	public void EnableNotePlacementMode()
	{
		this.isNotePlacementMode = true;
		this.pendingNoteText = "";
		this.trackSelf = false;
		this.selectedNoteType = 0;
	}

	// Token: 0x06000161 RID: 353 RVA: 0x00005A6D File Offset: 0x00003C6D
	public void CancelNotePlacement()
	{
		this.isNotePlacementMode = false;
	}

	// Token: 0x06000162 RID: 354 RVA: 0x0001A3C8 File Offset: 0x000185C8
	public void PlaceNoteAtCurrentPosition()
	{
		bool flag = !this.isNotePlacementMode;
		if (!flag)
		{
			string text = this.pendingNoteText.Trim();
			bool flag2 = string.IsNullOrEmpty(text);
			if (flag2)
			{
				text = "Note";
			}
			MapViewer.NoteType type = (MapViewer.NoteType)this.selectedNoteType;
			int x = (ClientController.THIS.myBot != null) ? ClientController.THIS.myBot.gx : 0;
			int y = (ClientController.THIS.myBot != null) ? ClientController.THIS.myBot.gy : 0;
			bool flag3 = this.trackSelf;
			if (flag3)
			{
				this.notes.Add(new MapViewer.MapNote(true, text, type));
			}
			else
			{
				this.notes.Add(new MapViewer.MapNote(x, y, text, type));
			}
			this.CancelNotePlacement();
			this.UpdateMap();
			this.SaveNotes();
		}
	}

	// Token: 0x06000163 RID: 355 RVA: 0x0001A4AC File Offset: 0x000186AC
	private void UpdateTrackingNotes()
	{
		bool flag = false;
		foreach (MapViewer.MapNote mapNote in this.notes)
		{
			bool isTracking = mapNote.isTracking;
			if (isTracking)
			{
				bool flag2 = ClientController.THIS.myBot != null;
				if (flag2)
				{
					int gx = ClientController.THIS.myBot.gx;
					int gy = ClientController.THIS.myBot.gy;
					bool flag3 = mapNote.x != gx || mapNote.y != gy;
					if (flag3)
					{
						mapNote.x = gx;
						mapNote.y = gy;
						flag = true;
					}
				}
			}
		}
		bool flag4 = flag;
		if (flag4)
		{
			this.UpdateMap();
		}
	}

	// Token: 0x06000164 RID: 356 RVA: 0x0001A590 File Offset: 0x00018790
	private void SaveNotes()
	{
		try
		{
			string text = "{\"notes\":[";
			for (int i = 0; i < this.notes.Count; i++)
			{
				bool flag = i > 0;
				if (flag)
				{
					text += ",";
				}
				text += this.notes[i].ToJson();
			}
			text += "]}";
			PlayerPrefs.SetString("MapNotes", text);
			PlayerPrefs.Save();
		}
		catch (Exception ex)
		{
			Debug.LogError("Error saving notes: " + ex.Message);
		}
	}

	// Token: 0x06000165 RID: 357 RVA: 0x0001A63C File Offset: 0x0001883C
	private void LoadNotes()
	{
		this.notes.Clear();
		bool flag = PlayerPrefs.HasKey("MapNotes");
		if (flag)
		{
			try
			{
				string @string = PlayerPrefs.GetString("MapNotes");
				int num = @string.IndexOf("[");
				int num2 = @string.LastIndexOf("]");
				bool flag2 = num >= 0 && num2 > num;
				if (flag2)
				{
					string text = @string.Substring(num + 1, num2 - num - 1);
					string[] array = text.Split(new string[]
					{
						"},{",
						"},{"
					}, StringSplitOptions.None);
					foreach (string str in array)
					{
						string json = "{" + str + "}";
						MapViewer.MapNote mapNote = MapViewer.MapNote.FromJson(json);
						bool flag3 = mapNote != null;
						if (flag3)
						{
							this.notes.Add(mapNote);
						}
					}
				}
			}
			catch (Exception ex)
			{
				Debug.LogError("Error loading notes: " + ex.Message);
			}
		}
	}

	// Token: 0x04000250 RID: 592
	public Image mapImage;

	// Token: 0x04000251 RID: 593
	public Image marker;

	// Token: 0x04000252 RID: 594
	private Texture2D texture;

	// Token: 0x04000253 RID: 595
	private Texture2D texture_x2;

	// Token: 0x04000254 RID: 596
	private Color[] colors;

	// Token: 0x04000255 RID: 597
	private Color[] colors_x2;

	// Token: 0x04000256 RID: 598
	public static Color[] aliveColorTable = new Color[255];

	// Token: 0x04000257 RID: 599
	public static Color[] colorTable = new Color[255];

	// Token: 0x04000258 RID: 600
	public static Color[] transparentTable = new Color[255];

	// Token: 0x04000259 RID: 601
	public static Color[] customTable = new Color[255];

	// Token: 0x0400025A RID: 602
	public static float[] customBlinkRateTable = new float[255];

	// Token: 0x0400025B RID: 603
	public static bool colorsInited = false;

	// Token: 0x0400025C RID: 604
	public Button allmapButton;

	// Token: 0x0400025D RID: 605
	public Button exitButton;

	// Token: 0x0400025E RID: 606
	public Dropdown modeDropdown;

	// Token: 0x0400025F RID: 607
	public Text CoordText;

	// Token: 0x04000260 RID: 608
	private int width = 380;

	// Token: 0x04000261 RID: 609
	private int height = 230;

	// Token: 0x04000262 RID: 610
	public static MapViewer THIS;

	// Token: 0x04000263 RID: 611
	private Sprite mapSprite;

	// Token: 0x04000264 RID: 612
	private Sprite mapSprite_x2;

	// Token: 0x04000265 RID: 613
	private float lastClickTime;

	// Token: 0x04000266 RID: 614
	private float lastUpdateTime;

	// Token: 0x04000267 RID: 615
	private bool isDragging;

	// Token: 0x04000268 RID: 616
	private int startMouseX;

	// Token: 0x04000269 RID: 617
	private int startMouseY;

	// Token: 0x0400026A RID: 618
	private int startMapX;

	// Token: 0x0400026B RID: 619
	private int startMapY;

	// Token: 0x0400026C RID: 620
	private int mapX;

	// Token: 0x0400026D RID: 621
	private int mapY;

	// Token: 0x0400026E RID: 622
	private int lastBotX;

	// Token: 0x0400026F RID: 623
	private int lastBotY;

	// Token: 0x04000270 RID: 624
	private int cursDx;

	// Token: 0x04000271 RID: 625
	private int cursDy;

	// Token: 0x04000272 RID: 626
	private float lastdrag;

	// Token: 0x04000273 RID: 627
	private List<MapViewer.MapNote> notes = new List<MapViewer.MapNote>();

	// Token: 0x04000274 RID: 628
	private bool isNotePlacementMode = false;

	// Token: 0x04000275 RID: 629
	private string pendingNoteText = "";

	// Token: 0x04000276 RID: 630
	private bool trackSelf = false;

	// Token: 0x04000277 RID: 631
	private int selectedNoteType = 0;

	// Token: 0x04000278 RID: 632
	private bool showNotesList = false;

	// Token: 0x04000279 RID: 633
	private Vector2 scrollPosition;

	// Token: 0x02000031 RID: 49
	[Serializable]
	public class MapNote
	{
		// Token: 0x06000166 RID: 358 RVA: 0x00005A77 File Offset: 0x00003C77
		public MapNote(int x, int y, string text, MapViewer.NoteType type = MapViewer.NoteType.General)
		{
			this.x = x;
			this.y = y;
			this.text = text;
			this.type = type;
			this.isTracking = false;
		}

		// Token: 0x06000167 RID: 359 RVA: 0x00005AA5 File Offset: 0x00003CA5
		public MapNote(bool trackSelf, string text, MapViewer.NoteType type = MapViewer.NoteType.General)
		{
			this.isTracking = true;
			this.type = type;
			this.text = text;
			this.x = 0;
			this.y = 0;
		}

		// Token: 0x06000168 RID: 360 RVA: 0x0001A75C File Offset: 0x0001895C
		public string ToJson()
		{
			return string.Format("{{\"x\":{0},\"y\":{1},\"text\":\"{2}\",\"type\":{3},\"isTracking\":{4}}}", new object[]
			{
				this.x,
				this.y,
				this.text,
				(int)this.type,
				this.isTracking ? "true" : "false"
			});
		}

		// Token: 0x06000169 RID: 361 RVA: 0x0001A7C8 File Offset: 0x000189C8
		public static MapViewer.MapNote FromJson(string json)
		{
			MapViewer.MapNote result;
			try
			{
				int num = 0;
				int num2 = 0;
				int num3 = 0;
				string text = "";
				bool flag = false;
				int num4 = json.IndexOf("\"x\":");
				bool flag2 = num4 >= 0;
				if (flag2)
				{
					int num5 = num4 + 4;
					int num6 = json.IndexOf(",", num5);
					bool flag3 = num6 < 0;
					if (flag3)
					{
						num6 = json.IndexOf("}", num5);
					}
					int.TryParse(json.Substring(num5, num6 - num5), out num);
				}
				int num7 = json.IndexOf("\"y\":");
				bool flag4 = num7 >= 0;
				if (flag4)
				{
					int num8 = num7 + 4;
					int num9 = json.IndexOf(",", num8);
					bool flag5 = num9 < 0;
					if (flag5)
					{
						num9 = json.IndexOf("}", num8);
					}
					int.TryParse(json.Substring(num8, num9 - num8), out num2);
				}
				int num10 = json.IndexOf("\"text\":\"");
				bool flag6 = num10 >= 0;
				if (flag6)
				{
					int num11 = num10 + 8;
					int num12 = json.IndexOf("\"", num11);
					text = json.Substring(num11, num12 - num11);
				}
				int num13 = json.IndexOf("\"type\":");
				bool flag7 = num13 >= 0;
				if (flag7)
				{
					int num14 = num13 + 7;
					int num15 = json.IndexOf(",", num14);
					bool flag8 = num15 < 0;
					if (flag8)
					{
						num15 = json.IndexOf("}", num14);
					}
					int.TryParse(json.Substring(num14, num15 - num14), out num3);
				}
				int num16 = json.IndexOf("\"isTracking\":");
				bool flag9 = num16 >= 0;
				if (flag9)
				{
					int startIndex = num16 + 13;
					flag = (json.Substring(startIndex, 4) == "true");
				}
				bool flag10 = flag;
				MapViewer.MapNote mapNote;
				if (flag10)
				{
					mapNote = new MapViewer.MapNote(true, text, (MapViewer.NoteType)num3);
				}
				else
				{
					mapNote = new MapViewer.MapNote(num, num2, text, (MapViewer.NoteType)num3);
				}
				result = mapNote;
			}
			catch
			{
				result = null;
			}
			return result;
		}

		// Token: 0x0400027A RID: 634
		public int x;

		// Token: 0x0400027B RID: 635
		public int y;

		// Token: 0x0400027C RID: 636
		public string text;

		// Token: 0x0400027D RID: 637
		public MapViewer.NoteType type;

		// Token: 0x0400027E RID: 638
		public bool isTracking;
	}

	// Token: 0x02000032 RID: 50
	public enum NoteType
	{
		// Token: 0x04000280 RID: 640
		General,
		// Token: 0x04000281 RID: 641
		Danger,
		// Token: 0x04000282 RID: 642
		Resource,
		// Token: 0x04000283 RID: 643
		Base
	}
}
