using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000013 RID: 19
public class ClientController : MonoBehaviour
{
	// Token: 0x06000080 RID: 128
	private void Start()
	{
		this.robotRenderer = this.mainRenderer.GetComponent<RobotRenderer>();
		this.terrainRenderer = this.mainRenderer.GetComponent<TerrainRendererScript>();
		this.serverTime = this.obvyazkaObject.GetComponent<ServerTime>();
		this.obvyazka = this.obvyazkaObject.GetComponent<Obvyazka>();
		ClientController.THIS = this;
		this.Cursor.SetActive(false);
		this.autoDiggButton.onClick.AddListener(new UnityAction(this.ToggleAutoDigg));
		this.ShowAutoDigg();
		this.NoGUIClickPad.onClick.AddListener(new UnityAction(this.NoGUIClick));
	}

	// Token: 0x06000081 RID: 129
	private void ToggleAutoDigg()
	{
		ClientController.CanGoto = false;
		ServerTime.THIS.SendTypicalMessage(-1, this.str1, 0, 0, "_");
	}

	// Token: 0x06000082 RID: 130
	public void ShowAutoDigg()
	{
		if (!ClientController.autoDigg)
		{
			this.autoDiggButtonText.text = "АВТОКОПА-";
			this.autoDiggButtonText.color = new Color(0.5f, 0.5f, 0.5f);
			this.autoDiggButton.GetComponent<Image>().color = new Color(0.2f, 0.2f, 0.2f, 0.7f);
			return;
		}
		this.autoDiggButtonText.text = "АВТОКОПА+";
		this.autoDiggButtonText.color = new Color(1f, 1f, 1f);
		this.autoDiggButton.GetComponent<Image>().color = new Color(0.7f, 0.2f, 0.2f, 0.7f);
	}

	// Token: 0x06000083 RID: 131
	public void AddFX(int x, int y, int fx)
	{
		if (Time.unscaledDeltaTime <= 0.5f)
		{
			switch (fx)
			{
				case 0:
					this.AddAnimation(1, x, y);
					return;
				case 1:
				case 9:
					break;
				case 2:
					this.AddAnimation(2, x, y);
					if (ClientConfig.SOUND_DEATH)
					{
						this.AddVolumedSound(x, y, 4, 200f, 1f);
						return;
					}
					break;
				case 3:
					if (ClientConfig.SOUND_BOMBTICK)
					{
						this.AddVolumedSound(x, y, 3, 49f, 1f);
						return;
					}
					break;
				case 4:
					if (ClientConfig.SOUND_BOMB)
					{
						this.AddVolumedSound(x, y, 2, 49f, 1f);
						return;
					}
					break;
				case 5:
					if (ClientConfig.SOUND_DESTROY)
					{
						this.AddVolumedSound(x, y, 5, 49f, 1f);
						return;
					}
					break;
				case 6:
					if (ClientConfig.SOUND_DIZZ)
					{
						this.AddVolumedSound(x, y, 11, 49f, 1f);
						return;
					}
					break;
				case 7:
					if (ClientConfig.SOUND_EMI)
					{
						this.AddVolumedSound(x, y, 6, 49f, 1f);
						return;
					}
					break;
				case 8:
					if (ClientConfig.SOUND_GEOLOGY)
					{
						this.AddVolumedSound(x, y, 7, 49f, 1f);
						return;
					}
					break;
				case 10:
					if (ClientConfig.SOUND_TP_IN)
					{
						this.AddVolumedSound(x, y, 12, 49f, 1f);
						return;
					}
					break;
				case 11:
					if (ClientConfig.SOUND_TP_OUT)
					{
						this.AddVolumedSound(x, y, 13, 49f, 1f);
						return;
					}
					break;
				case 12:
					{
						GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.nohpfxPrefab);
						gameObject.transform.SetParent(this.RenderWrapper.transform, false);
						gameObject.transform.position = new Vector3((float)x + 0.5f, -(float)y - 0.5f, -7f);
						return;
					}
				case 13:
					{
						GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.nohpfxSmallPrefab);
						gameObject2.transform.SetParent(this.RenderWrapper.transform, false);
						gameObject2.transform.position = new Vector3((float)x + 0.5f, -(float)y - 0.5f, -7f);
						return;
					}
				case 14:
					{
						GameObject gameObject3 = UnityEngine.Object.Instantiate<GameObject>(this.nohpfxSmallPrefab);
						gameObject3.transform.SetParent(this.RenderWrapper.transform, false);
						gameObject3.transform.position = new Vector3((float)x + 0.5f, -(float)y, -7f);
						return;
					}
				case 15:
					{
						GameObject gameObject4 = UnityEngine.Object.Instantiate<GameObject>(this.nohpfxSmallPrefab);
						gameObject4.transform.SetParent(this.RenderWrapper.transform, false);
						gameObject4.transform.position = new Vector3((float)x + 0.5f, -(float)y - 0.7f, -7f);
						gameObject4.transform.localScale = new Vector3(0.5f, 0.5f, 1f);
						return;
					}
				case 16:
				case 17:
				case 18:
				case 19:
				case 20:
				case 21:
				case 22:
				case 23:
					{
						GameObject gameObject5 = UnityEngine.Object.Instantiate<GameObject>(this.smokePrefab);
						gameObject5.transform.SetParent(this.RenderWrapper.transform, false);
						gameObject5.GetComponent<ParticleSystem>().startColor = this.smokeColors[fx - 16];
						gameObject5.transform.position = new Vector3((float)x + 0.5f, -(float)y - 0.5f, -3f);
						return;
					}
				case 24:
					{
						GameObject gameObject6 = UnityEngine.Object.Instantiate<GameObject>(this.volcanoPrefab);
						gameObject6.transform.SetParent(this.RenderWrapper.transform, false);
						gameObject6.transform.position = new Vector3((float)x + 0.5f, -(float)y - 0.5f, -3f);
						if (ClientConfig.SOUND_VOLC)
						{
							this.AddVolumedSound(x, y, 14, 49f, 1f);
							return;
						}
						break;
					}
				case 25:
					if (ClientConfig.SOUND_C190)
					{
						this.AddVolumedSound(x, y, 15, 49f, 0.2f);
						return;
					}
					break;
				default:
					return;
			}
		}
	}

	// Token: 0x06000084 RID: 132
	private void AddGunShot(int x, int y, int bid, int color)
	{
		int num = -1;
		GameObject free = this.gunShotPool.GetFree(out num);
		if (num != -1)
		{
			free.transform.SetParent(this.RenderWrapper.transform, false);
			free.GetComponent<GunShotScript>().Setup(x, y, bid, color, num);
		}
	}

	// Token: 0x06000085 RID: 133
	public void AddDirectedFX(int bid, int x, int y, int fx, int dir, int col)
	{
		switch (fx)
		{
			case -1:
			case 7:
				this.AddGunShot(x, y, bid, col);
				return;
			case 0:
				if (bid != this.myBotId || this.isProgrammator)
				{
					this.AddBz(x, y, dir, bid != this.myBotId || ClientController.ownSounds);
					return;
				}
				break;
			case 1:
				this.AddBoom(x, y, dir, col);
				return;
			case 2:
				if (dir > 230)
				{
					dir = 500 + (dir - 230) * 20;
				}
				else if (dir > 200)
				{
					dir = 200 + (dir - 200) * 10;
				}
				this.AddCrys(x, y, this.crysFromCode[col], dir, bid, 0);
				return;
			case 3:
				if (dir > 230)
				{
					dir = 500 + (dir - 230) * 20;
				}
				else if (dir > 200)
				{
					dir = 200 + (dir - 200) * 10;
				}
				this.AddCrys(x, y, this.crysFromCode[col], dir, bid, 150);
				return;
			case 4:
				if (dir > 230)
				{
					dir = 500 + (dir - 230) * 20;
				}
				else if (dir > 200)
				{
					dir = 200 + (dir - 200) * 10;
				}
				this.AddCrys2(x, y, this.crysFromCode[col], dir, bid);
				return;
			case 5:
				if (RobotRenderer.THIS.bots.ContainsKey(bid))
				{
					if (ClientConfig.SOUND_HEAL)
					{
						this.AddVolumedSound(x, y, 8, 49f, 1f);
					}
					GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.healfxPrefab);
					gameObject.transform.parent = RobotRenderer.THIS.bots[bid].transform;
					gameObject.transform.position = RobotRenderer.THIS.bots[bid].transform.position + new Vector3(0f, 0f, -0.5f);
					return;
				}
				break;
			case 6:
				if (RobotRenderer.THIS.bots.ContainsKey(bid))
				{
					if (ClientConfig.SOUND_HURT)
					{
						this.AddVolumedSound(x, y, 9, 49f, 1f);
					}
					RobotRenderer.THIS.bots[bid].GetComponent<RobotScript>().tremor = 0.5f;
					GameObject gameObject2 = UnityEngine.Object.Instantiate<GameObject>(this.hurtfxPrefab);
					gameObject2.GetComponent<ParticleSystem>().startColor = Color.Lerp(new Color(1f, 0f, 0f), new Color(0f, 2f, 1f), (float)col / 100f);
					gameObject2.GetComponent<ParticleSystem>().startSize = 0.65f + (float)col / 50f;
					gameObject2.transform.SetParent(RobotRenderer.THIS.bots[bid].transform, false);
					gameObject2.transform.position = RobotRenderer.THIS.bots[bid].transform.position + new Vector3(0f, 0f, -0.5f);
					return;
				}
				break;
			default:
				return;
		}
	}

	// Token: 0x06000086 RID: 134
	private void AddCrys2(int x, int y, string crys, int dx, int dy)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.crys2Prefab);
		gameObject.transform.SetParent(this.RenderWrapper.transform, false);
		gameObject.GetComponent<CrysAutScript>().SetCrys(x, y, crys, dx, dy);
	}

	// Token: 0x06000087 RID: 135
	private void AddCrys(int x, int y, string crys, int num, int bid, int delay)
	{
		int num2 = -1;
		GameObject free = this.crysPlusPool.GetFree(out num2);
		if (num2 != -1)
		{
			free.transform.SetParent(this.RenderWrapper.transform, false);
			free.GetComponent<CrysPlusScript>().SetCrys(x, y, crys, num, bid, delay, num2);
		}
	}

	// Token: 0x06000088 RID: 136
	private void AddBoom(int x, int y, int size, int color)
	{
		int num = -1;
		GameObject free = this.boomPool.GetFree(out num);
		if (num != -1)
		{
			free.transform.SetParent(this.RenderWrapper.transform, false);
			free.GetComponent<BoomScript>().Setup(x, y, size, color, num);
		}
	}

	// Token: 0x06000089 RID: 137
	public void stopAutoMove()
	{
		this.automove = false;
		this.Cursor.SetActive(false);
	}

	// Token: 0x0600008A RID: 138
	private void startAutoMove()
	{
		this.automove = true;
		this.Cursor.SetActive(true);
		this.Cursor.transform.position = new Vector3((float)this.GotoX + 0.5f, -(float)this.GotoY - 0.5f, -6f);
	}

	// Token: 0x0600008B RID: 139
	private float euristic(int x, int y)
	{
		return this.ROAD_PAUSE * (float)(Mathf.Abs(x - this.GotoX) + Mathf.Abs(y - this.GotoY)) + 0.01f * Mathf.Sqrt((float)((x - this.GotoX) * (x - this.GotoX) + (y - this.GotoY) * (y - this.GotoY)));
	}

	// Token: 0x0600008C RID: 140
	private bool CheckAStarFor(RoutePoint from, int x, int y)
	{
		int cell = ClientController.map.GetCell(x, y);
		if (x != this.GotoX || y != this.GotoY)
		{
			if (this.closedSet.Contains(x + y * ClientController.map.width))
			{
				return false;
			}
			if (Math.Abs(x - this.myBot.gx) + Math.Abs(y - this.myBot.gy) > ClientConfig.mouseR)
			{
				return false;
			}
			if (CellRender.UNBREAKABLE[cell])
			{
				return false;
			}
		}
		float num = this.BZ_PAUSE;
		if (CellModel.isEmpty[cell])
		{
			if (this.isRoad(cell))
			{
				num = this.ROAD_PAUSE;
			}
			else
			{
				num = this.XY_PAUSE;
			}
		}
		else
		{
			num = this.BZ_PAUSE * (float)CellRender.BZCOST[cell];
			if ((ClientConfig.mouseNoDig || !ClientController.autoDigg) && (x != this.GotoX || y != this.GotoY))
			{
				return false;
			}
		}
		if (cell == 37 && PackRenderer.THIS.IsPackOn(x, y) && (x != this.GotoX || y != this.GotoY))
		{
			num = 1000f;
		}
		float num2 = 1f / (1f + this.euristic(x, y));
		num = num2 * this.XY_PAUSE + (1f - num2) * num;
		float num3 = from.g + num;
		if (this.openedSet.Contains(x + y * ClientController.map.width))
		{
			if (num3 < this.points[x + y * ClientController.map.width].g)
			{
				RoutePoint routePoint = this.points[x + y * ClientController.map.width];
				routePoint.g = num3;
				routePoint.f = num3 + this.euristic(x, y);
				routePoint.px = from.x;
				routePoint.py = from.y;
			}
		}
		else
		{
			RoutePoint value = default(RoutePoint);
			value.x = x;
			value.y = y;
			value.cell = cell;
			value.g = num3;
			value.f = num3 + this.euristic(x, y);
			value.px = from.x;
			value.py = from.y;
			this.points[x + y * ClientController.map.width] = value;
			this.openedSet.Add(x + y * ClientController.map.width);
		}
		return false;
	}

	// Token: 0x0600008D RID: 141
	private void UpdateRoute()
	{
		if (this.automove)
		{
			if (this.myBot.gx == this.GotoX && this.myBot.gy == this.GotoY)
			{
				this.stopAutoMove();
				return;
			}
			int num = this.myBot.gx - this.GotoX;
			int num2 = this.myBot.gy - this.GotoY;
			if (num * num + num2 * num2 > ClientConfig.mouseR * ClientConfig.mouseR)
			{
				this.stopAutoMove();
				return;
			}
			this.route.Clear();
			this.points.Clear();
			this.openedSet.Clear();
			this.closedSet.Clear();
			RoutePoint routePoint = default(RoutePoint);
			routePoint.x = this.myBot.gx;
			routePoint.y = this.myBot.gy;
			routePoint.cell = 32;
			routePoint.h = this.euristic(this.myBot.gx, this.myBot.gy);
			routePoint.g = 0f;
			routePoint.f = routePoint.h + routePoint.g;
			this.points.Add(routePoint.x + routePoint.y * ClientController.map.width, routePoint);
			this.openedSet.Add(routePoint.x + routePoint.y * ClientController.map.width);
			int i = 0;
			while (i < ClientConfig.mouseMaxStack)
			{
				i++;
				float num3 = float.PositiveInfinity;
				int num4 = -1;
				foreach (int num5 in this.openedSet)
				{
					RoutePoint routePoint2 = this.points[num5];
					if (routePoint2.f < num3)
					{
						num3 = routePoint2.f;
						num4 = num5;
					}
				}
				if (num4 == -1)
				{
					this.stopAutoMove();
					return;
				}
				RoutePoint routePoint3 = this.points[num4];
				this.openedSet.Remove(num4);
				this.closedSet.Add(num4);
				this.CheckAStarFor(routePoint3, routePoint3.x + 1, routePoint3.y);
				this.CheckAStarFor(routePoint3, routePoint3.x - 1, routePoint3.y);
				this.CheckAStarFor(routePoint3, routePoint3.x, routePoint3.y + 1);
				this.CheckAStarFor(routePoint3, routePoint3.x, routePoint3.y - 1);
				if (this.openedSet.Contains(this.GotoX + ClientController.map.width * this.GotoY))
				{
					break;
				}
			}
			if (i == ClientConfig.mouseMaxStack)
			{
				this.stopAutoMove();
				return;
			}
			RoutePoint routePoint4 = this.points[this.GotoX + ClientController.map.width * this.GotoY];
			i = 0;
			while (i < ClientConfig.mouseMaxLen)
			{
				this.route.Add(routePoint4);
				routePoint4 = this.points[routePoint4.px + ClientController.map.width * routePoint4.py];
				if (routePoint4.x == this.myBot.gx && routePoint4.y == this.myBot.gy)
				{
					break;
				}
			}
			if (i == ClientConfig.mouseMaxLen)
			{
				this.stopAutoMove();
			}
		}
	}

	// Token: 0x0600008E RID: 142
	public void FromMapGoto(int x, int y)
	{
		if (!ProgrammatorView.active && (!this.isProgrammator || ProgPanel.handMode))
		{
			this.tryGotoX = x;
			this.tryGotoY = y;
			ClientController.CanGoto = true;
			this.TryToGoto(true);
		}
	}

	// Token: 0x0600008F RID: 143
	private void NoGUIClick()
	{
		if (this.myBot != null && !ConnectionManager.disconnected && !ChatManager.THIS.ChatInput.isFocused && !GUIManager.THIS.localChatInput.isFocused && !ProgrammatorView.active && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject == null || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name != "InputField") && !AYSWindowManager.THIS.gameObject.activeSelf && (!this.isProgrammator || ProgPanel.handMode) && !TutorialNavigation.THIS.lens.activeSelf)
		{
			Vector2 v = Input.mousePosition;
			Vector2 vector = Camera.main.ScreenToWorldPoint(v);
			int num = Mathf.FloorToInt(vector.x);
			int num2 = -Mathf.CeilToInt(vector.y);
			this.tryGotoX = num;
			this.tryGotoY = num2;
			base.Invoke("TryToGotoInvokable", 0.1f);
		}
	}

	// Token: 0x06000090 RID: 144
	private void TryToGotoInvokable()
	{
		this.TryToGoto(false);
	}

	// Token: 0x06000091 RID: 145
	private void TryToGoto(bool DisableCheck = false)
	{
		if (!(this.myBot == null) && ClientController.MouseControl)
		{
			if (!ClientController.CanGoto)
			{
				ClientController.CanGoto = true;
				return;
			}
			bool flag = false;
			if (GUIManager.THIS.m_EventSystem.currentSelectedGameObject != null && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "ChatField" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "LocalChat"))
			{
				flag = true;
			}
			if (!DisableCheck && (MapViewer.THIS.gameObject.activeSelf || OKWindowManager.THIS.gameObject.activeSelf || AYSWindowManager.THIS.gameObject.activeSelf || PopupManager.THIS.GUIWindow.activeSelf || flag))
			{
				ClientController.CanGoto = true;
				return;
			}
			int cell = ClientController.map.GetCell(this.tryGotoX, this.tryGotoY);
			if (cell != 83)
			{
				if ((CellRender.UNBREAKABLE[cell] || cell == 37) && !this.SmartPointing(this.tryGotoX + 1, this.tryGotoY) && !this.SmartPointing(this.tryGotoX - 1, this.tryGotoY) && !this.SmartPointing(this.tryGotoX, this.tryGotoY + 1) && !this.SmartPointing(this.tryGotoX, this.tryGotoY - 1) && !this.SmartPointing(this.tryGotoX + 1, this.tryGotoY + 1) && !this.SmartPointing(this.tryGotoX - 1, this.tryGotoY + 1) && !this.SmartPointing(this.tryGotoX + 1, this.tryGotoY - 1))
				{
					this.SmartPointing(this.tryGotoX - 1, this.tryGotoY - 1);
				}
				cell = ClientController.map.GetCell(this.tryGotoX, this.tryGotoY);
				if (CellRender.UNBREAKABLE[cell])
				{
					int num = -1;
					int num2 = -1;
					if (this.tryGotoX > this.myBot.gx)
					{
						num = 1;
					}
					if (this.tryGotoY > this.myBot.gy)
					{
						num2 = 1;
					}
					if (!this.SmartFreePointing(this.tryGotoX - num, this.tryGotoY) && !this.SmartFreePointing(this.tryGotoX, this.tryGotoY - num2) && !this.SmartFreePointing(this.tryGotoX - num, this.tryGotoY - num2) && !this.SmartFreePointing(this.tryGotoX - num, this.tryGotoY + num2) && !this.SmartFreePointing(this.tryGotoX + num, this.tryGotoY - num2) && !this.SmartFreePointing(this.tryGotoX + num, this.tryGotoY + num2) && !this.SmartFreePointing(this.tryGotoX + num, this.tryGotoY))
					{
						this.SmartFreePointing(this.tryGotoX, this.tryGotoY + num2);
					}
				}
				cell = ClientController.map.GetCell(this.tryGotoX, this.tryGotoY);
				if (CellRender.UNBREAKABLE[cell])
				{
					return;
				}
			}
			this.GotoX = this.tryGotoX;
			this.GotoY = this.tryGotoY;
			this.startAutoMove();
			this.UpdateRoute();
		}
	}

	// Token: 0x06000092 RID: 146
	private bool SmartFreePointing(int x, int y)
	{
		bool result;
		if (CellModel.isEmpty[ClientController.map.GetCell(x, y)])
		{
			this.tryGotoX = x;
			this.tryGotoY = y;
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000093 RID: 147
	private bool SmartPointing(int x, int y)
	{
		bool result;
		if (ClientController.map.GetCell(x, y) == 37 && PackRenderer.THIS.IsPackOn(x, y))
		{
			this.tryGotoX = x;
			this.tryGotoY = y;
			result = true;
		}
		else
		{
			result = false;
		}
		return result;
	}

	// Token: 0x06000094 RID: 148
	private void Update()
	{
		if (ClientController.inited)
		{
			TutorialNavigation.THIS.UpdateArrow();
			float num = 0f;
			if ((float)this.myBot.gy > this.DEPTH)
			{
				if (this.slowRedAlpha < 0.01f)
				{
					this.slowRedAlpha = 1f;
				}
				else
				{
					num = 0.01f + 0.5f * Mathf.Min(0.01f * ((float)this.myBot.gy - this.DEPTH), 1f);
				}
			}
			this.slowRedAlpha = 0.9f * this.slowRedAlpha + 0.1f * num;
			this.redPad.color = new Color(1f, 0f, 0f, this.slowRedAlpha);
			if (this.myBotId != -1)
			{
				Vector2 vector = new Vector2((float)this.view_x - this.tcx, (float)this.view_y - this.tcy);
				this.aveDCamera = 0.97f * this.aveDCamera + 0.03f * Vector2.ClampMagnitude(vector, 10f);
				this.aveDCamera2 = 0.97f * this.aveDCamera2 + 0.03f * this.aveDCamera;
				float num2 = 0.02f * this.aveDCamera2.magnitude + 3E-06f * vector.sqrMagnitude * vector.sqrMagnitude;
				if (num2 > 0.3f)
				{
					num2 = 0.3f;
				}
				this.tcx += num2 * vector.x;
				this.tcy += num2 * vector.y;
			}
			this.tremor *= 0.9f;
			this.terrainRenderer.cx = this.tcx;
			this.terrainRenderer.cy = this.tcy;
			if (this.tremor > 0.03f)
			{
				Vector3 position = this.myBot.transform.position;
				position.x += this.tremor * (UnityEngine.Random.value - 0.5f);
				position.y += this.tremor * (UnityEngine.Random.value - 0.5f);
				this.myBot.transform.position = position;
			}
			if (this.myBot != null)
			{
				this.view_x = this.myBot.gx;
				this.view_y = this.myBot.gy;
			}
			if (this.Cursor.activeSelf)
			{
				this.Cursor.GetComponent<SpriteRenderer>().color = new Color(0f, 1f, 0f, 0.5f + 0.5f * Mathf.Sin(10f * Time.time));
			}
			if (this.myBot != null && !ConnectionManager.disconnected && !ChatManager.THIS.ChatInput.isFocused && !GUIManager.THIS.localChatInput.isFocused && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject == null || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name != "InputField") && !AYSWindowManager.THIS.gameObject.activeSelf && !ProgrammatorView.active)
			{
				if (Input.GetKeyDown(ClientConfig.AUTOREM_KEY))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str2, 0, 0, "-");
				}
				if (Input.GetKeyDown(ClientConfig.AGR_KEY))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str3, 0, 0, "-");
				}
				if (Input.GetKeyDown(ClientConfig.AUTODIG_KEY))
				{
					this.ToggleAutoDigg();
				}
				if (Input.GetKeyDown(KeyCode.Alpha0))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 0, "0");
				}
				if (Input.GetKeyDown(KeyCode.Alpha1))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 1, "1");
				}
				if (Input.GetKeyDown(KeyCode.Alpha2))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 2, "2");
				}
				if (Input.GetKeyDown(KeyCode.Alpha3))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 3, "3");
				}
				if (Input.GetKeyDown(KeyCode.Alpha4))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 4, "4");
				}
				if (Input.GetKeyDown(KeyCode.Alpha5))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 5, "5");
				}
				if (Input.GetKeyDown(KeyCode.Alpha6))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 6, "6");
				}
				if (Input.GetKeyDown(KeyCode.Alpha7))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 7, "7");
				}
				if (Input.GetKeyDown(KeyCode.Alpha8))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 8, "8");
				}
				if (Input.GetKeyDown(KeyCode.Alpha9))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str5, 0, 9, "9");
				}
				if (Input.GetKeyDown(ClientConfig.PROG_KEY))
				{
					ProgPanel.THIS.OnPlayStop();
				}
				if (Input.GetKeyDown(ClientConfig.MAP_KEY) && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject == null || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name != "InputField"))
				{
					if (MapViewer.THIS.gameObject.activeSelf)
					{
						MapViewer.THIS.OnExit();
					}
					else
					{
						MapViewer.THIS.Show();
					}
				}
				if (Input.GetKeyDown(ClientConfig.INV_KEY) && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject == null || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name != "InputField"))
				{
					ServerTime.THIS.SendTypicalMessage(-1, this.str6, 0, 0, "_");
				}
			}
			if (this.myBot != null && !ConnectionManager.disconnected && !ChatManager.THIS.ChatInput.isFocused && !GUIManager.THIS.localChatInput.isFocused && !ProgrammatorView.active && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject == null || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name != "InputField") && !AYSWindowManager.THIS.gameObject.activeSelf && (!this.isProgrammator || ProgPanel.handMode))
			{
				if ((Input.GetKeyUp(KeyCode.LeftCommand) || Input.GetKeyUp(KeyCode.RightCommand) || Input.GetKeyUp(KeyCode.LeftControl) || Input.GetKeyUp(KeyCode.RightControl)) && ClientController.CtrlToggle)
				{
					this.isControl = !this.isControl;
				}
				if (Input.GetKeyDown("up") || Input.GetKeyDown(ClientConfig.MOVE_UP_KEY))
				{
					this.stopAutoMove();
					this.lastDirKey = "up";
				}
				if (Input.GetKeyDown("down") || Input.GetKeyDown(ClientConfig.MOVE_DOWN_KEY))
				{
					this.stopAutoMove();
					this.lastDirKey = "down";
				}
				if (Input.GetKeyDown("left") || Input.GetKeyDown(ClientConfig.MOVE_LEFT_KEY))
				{
					this.stopAutoMove();
					this.lastDirKey = "left";
				}
				if (Input.GetKeyDown("right") || Input.GetKeyDown(ClientConfig.MOVE_RIGHT_KEY))
				{
					this.stopAutoMove();
					this.lastDirKey = "right";
				}
				if (!ClientController.CtrlToggle)
				{
					this.isControl = (Input.GetKey(KeyCode.LeftCommand) || Input.GetKey(KeyCode.RightCommand) || Input.GetKey(KeyCode.LeftControl) || Input.GetKey(KeyCode.RightControl));
				}
				if (!this.notActive && Time.unscaledTime > this.TimeForNextOperation)
				{
					bool isShift = Input.GetKey(KeyCode.LeftShift) || Input.GetKey(KeyCode.RightShift);
					if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
					{
						bool flag = false;
						if (GUIManager.THIS.m_EventSystem.currentSelectedGameObject != null && (GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "ChatField" || GUIManager.THIS.m_EventSystem.currentSelectedGameObject.name == "LocalChat"))
						{
							flag = true;
						}
						if (!OKWindowManager.THIS.gameObject.activeSelf && !PopupManager.THIS.GUIWindow.activeSelf && !flag && GUIManager.THIS.inventoryItem != -1)
						{
							ServerTime.THIS.SendTypicalMessage(-1, this.str7, 0, 0, "_");
						}
					}
					else if (Input.GetKey(ClientConfig.GEO_KEY))
					{
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str8, this.myBot.gx, this.myBot.gy, "_");
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if (Input.GetKey(ClientConfig.DIGG_KEY))
					{
						Debug.Log(string.Concat(new object[]
						{
							"bz ",
							this.myBot.gx,
							":",
							this.myBot.gy
						}));
						this.AddBz(this.myBot.gx, this.myBot.gy, this.dir, ClientController.ownSounds);
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str10, this.myBot.gx, this.myBot.gy, this.dir.ToString());
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if (Input.GetKey(ClientConfig.WARBLOCK_KEY))
					{
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str11, this.myBot.gx, this.myBot.gy, this.dir.ToString() + "V");
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if (Input.GetKey(ClientConfig.BLOCK_KEY))
					{
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str11, this.myBot.gx, this.myBot.gy, this.dir.ToString() + "G");
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if (Input.GetKey(ClientConfig.ROAD_KEY))
					{
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str11, this.myBot.gx, this.myBot.gy, this.dir.ToString() + "R");
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if (Input.GetKey(ClientConfig.QUADRO_KEY))
					{
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str11, this.myBot.gx, this.myBot.gy, this.dir.ToString() + "O");
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if (Input.GetKey(ClientConfig.HEAL_KEY))
					{
						this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str13, this.myBot.gx, this.myBot.gy, "_");
						this.AddTime(this.BZ_PAUSE, false);
					}
					else if ((Input.GetKey("up") || Input.GetKey(ClientConfig.MOVE_UP_KEY)) && this.lastDirKey == "up")
					{
						this.MoveOrBz(0, -1, 2, isShift, this.isControl);
					}
					else if ((Input.GetKey("down") || Input.GetKey(ClientConfig.MOVE_DOWN_KEY)) && this.lastDirKey == "down")
					{
						this.MoveOrBz(0, 1, 0, isShift, this.isControl);
					}
					else if ((Input.GetKey("left") || Input.GetKey(ClientConfig.MOVE_LEFT_KEY)) && this.lastDirKey == "left")
					{
						this.MoveOrBz(-1, 0, 1, isShift, this.isControl);
					}
					else if ((Input.GetKey("right") || Input.GetKey(ClientConfig.MOVE_RIGHT_KEY)) && this.lastDirKey == "right")
					{
						this.MoveOrBz(1, 0, 3, isShift, this.isControl);
					}
					else if (this.automove)
					{
						if (this.route.Count == 0)
						{
							this.stopAutoMove();
						}
						RoutePoint routePoint = this.route[this.route.Count - 1];
						if (routePoint.x == this.myBot.gx && routePoint.y == this.myBot.gy)
						{
							this.route.RemoveAt(this.route.Count - 1);
							if (this.route.Count == 0)
							{
								this.stopAutoMove();
							}
							else
							{
								routePoint = this.route[this.route.Count - 1];
							}
						}
						if (routePoint.x > this.myBot.gx)
						{
							this.MoveOrBz(1, 0, 3, false, this.isControl);
						}
						else if (routePoint.x < this.myBot.gx)
						{
							this.MoveOrBz(-1, 0, 1, false, this.isControl);
						}
						else if (routePoint.y > this.myBot.gy)
						{
							this.MoveOrBz(0, 1, 0, false, this.isControl);
						}
						else if (routePoint.y < this.myBot.gy)
						{
							this.MoveOrBz(0, -1, 2, false, this.isControl);
						}
					}
					if (this.TimeForNextOperation < Time.unscaledTime)
					{
						this.TimeForNextOperation = Time.unscaledTime;
					}
					if (!this.wasMove)
					{
						this.slowingEffect = 0.6f + 0.7f * this.slowingEffect;
					}
				}
			}
			ClientController.serverTimeOfLastFrame = this.serverTime.NowTime();
			ClientController.clientTimeOfLastFrame = (int)(Time.unscaledTime * 1000f);
			if (UnityEngine.Random.value < 1f)
			{
				for (int i = 0; i < this.BZ_DEBUG_GEN; i++)
				{
				}
			}
		}
		if (ClientController.pongResponse != -1)
		{
			ServerTime.THIS.lastSendedTime = this.serverTime.NowTime();
			this.obvyazka.SendU("PO", ClientController.pongResponse.ToString() + ":" + ServerTime.THIS.lastSendedTime.ToString());
			ClientController.pongResponse = -1;
		}
	}

	// Token: 0x06000095 RID: 149
	public void Tremor()
	{
	}

	// Token: 0x06000096 RID: 150
	private void SelfHurt()
	{
		if (this.lastHurtTime > Time.unscaledTime - 0.6f)
		{
			this.lastHurtTime = Time.unscaledTime;
			this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str12, this.myBot.gx, this.myBot.gy, "_");
			this.AddTime(this.HURT_PAUSE, false);
		}
	}

	// Token: 0x06000097 RID: 151
	private GameObject AddAnimation(int type, int x, int y)
	{
		int num = -1;
		GameObject free = this.bzPool.GetFree(out num);
		if (num != -1)
		{
			free.transform.SetParent(this.RenderWrapper.transform, false);
			free.GetComponent<BzScript>().SetAnimation(type, num, new Vector3((float)x + 0.5f, -(float)y - 0.5f, -3f));
		}
		return free;
	}

	// Token: 0x06000098 RID: 152
	private void AddVolumedSound(int x, int y, int num, float dump = 49f, float mult = 1f)
	{
		float num2 = (float)((this.myBot.gx - x) * (this.myBot.gx - x) + (this.myBot.gy - y) * (this.myBot.gy - y));
		if (num2 > 1f || ClientController.ownSounds || SoundManager.THIS.isSlow(num))
		{
			num2 = dump + num2;
			SoundManager.THIS.PlaySound(num, mult * dump / num2);
		}
	}

	// Token: 0x06000099 RID: 153
	private void AddBz(int x, int y, int dir, bool playSound = true)
	{
		if (playSound && ClientConfig.SOUND_MINING)
		{
			this.AddVolumedSound(x, y, 10, 49f, 1f);
		}
		Vector3 pos;
		switch (dir)
		{
			case 0:
				pos = new Vector3((float)x + 0.5f, -(float)y - 1f, -0.5f);
				break;
			case 1:
				pos = new Vector3((float)x + 0f, -(float)y - 0.5f, -0.5f);
				break;
			case 2:
				pos = new Vector3((float)x + 0.5f, -(float)y, -0.5f);
				break;
			default:
				pos = new Vector3((float)x + 1f, -(float)y - 0.5f, -0.5f);
				break;
		}
		int num = -1;
		GameObject free = this.bzPool.GetFree(out num);
		if (num != -1)
		{
			free.transform.SetParent(this.RenderWrapper.transform, false);
			free.GetComponent<BzScript>().SetAnimation(0, num, pos);
			Quaternion rotation = free.transform.rotation;
			Vector3 eulerAngles = rotation.eulerAngles;
			eulerAngles.z = (float)(-90 * dir + 180);
			rotation.eulerAngles = eulerAngles;
			free.transform.rotation = rotation;
		}
	}

	// Token: 0x0600009A RID: 154
	public int TimeOfMove()
	{
		return ClientController.serverTimeOfLastFrame + (int)(this.TimeForNextOperation * 1000f) - ClientController.clientTimeOfLastFrame;
	}

	// Token: 0x0600009B RID: 155
	public void TimeSync()
	{
		ClientController.serverTimeOfLastFrame = this.serverTime.NowTime();
		ClientController.clientTimeOfLastFrame = (int)(Time.unscaledTime * 1000f);
		if (this.TimeForNextOperation < Time.unscaledTime)
		{
			this.TimeForNextOperation = Time.unscaledTime;
		}
		this.TimeForNextOperation += this.XY_PAUSE;
		this.TimeForNextOperation += this.XY_PAUSE;
	}

	// Token: 0x0600009C RID: 156
	public void AddTime(float time, bool isSlowing = false)
	{
		this.TimeForNextOperation += time;
		this.wasMove = true;
	}

	// Token: 0x0600009D RID: 157
	private bool isEmpty(int cell)
	{
		return cell > 30 && cell < 40;
	}

	// Token: 0x0600009E RID: 158
	private bool isRoad(int cell)
	{
		return cell == 35 || cell == 36 || cell == 39;
	}

	// Token: 0x0600009F RID: 159
	public bool MoveOrBz(int dx, int dy, int _dir, bool isShift, bool isCtrl)
	{
		if (isShift)
		{
			dx = 0;
			dy = 0;
		}
		this.dir = _dir;
		if (TerrainRendererScript.map.GetCell(this.myBot.gx, this.myBot.gy) == 30 && (this.myBotLastSyncX != this.myBot.gx || this.myBotLastSyncY != this.myBot.gy))
		{
			dx = 0;
			dy = 0;
		}
		if (Mathf.Abs(dx) + Mathf.Abs(dy) > 1)
		{
			throw new Exception("Bad move");
		}
		bool result;
		if (isShift || CellModel.isEmpty[TerrainRendererScript.map.GetCell(this.myBot.gx + dx, this.myBot.gy + dy)])
		{
			this.myBot.SetRotation(this.dir);
			bool flag = this.isRoad(TerrainRendererScript.map.GetCell(this.myBot.gx + dx, this.myBot.gy + dy)) && this.isRoad(TerrainRendererScript.map.GetCell(this.myBot.gx, this.myBot.gy));
			this.myBot.SetXY((float)(this.myBot.gx + dx), (float)(this.myBot.gy + dy));
			if (isCtrl)
			{
				this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str9, this.myBot.gx, this.myBot.gy, (this.dir + 10).ToString());
			}
			else
			{
				this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str9, this.myBot.gx, this.myBot.gy, this.dir.ToString());
			}
			if (TerrainRendererScript.map.GetCell(this.myBot.gx, this.myBot.gy) == 83)
			{
				this.AddTime(1f, true);
			}
			else if (isCtrl)
			{
				this.AddTime(flag ? (3f * this.ROAD_PAUSE) : (3f * this.XY_PAUSE), true);
			}
			else
			{
				this.AddTime(flag ? this.ROAD_PAUSE : this.XY_PAUSE, true);
			}
			result = true;
		}
		else if (ClientController.autoDigg)
		{
			this.AddBz(this.myBot.gx, this.myBot.gy, this.dir, ClientController.ownSounds);
			this.myBot.SetRotation(this.dir);
			this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str10, this.myBot.gx, this.myBot.gy, this.dir.ToString());
			this.AddTime(this.BZ_PAUSE, false);
			result = true;
		}
		else
		{
			this.myBot.SetRotation(this.dir);
			bool flag2 = this.isRoad(TerrainRendererScript.map.GetCell(this.myBot.gx + dx, this.myBot.gy + dy)) && this.isRoad(TerrainRendererScript.map.GetCell(this.myBot.gx, this.myBot.gy));
			this.serverTime.SendTypicalMessage(this.TimeOfMove(), this.str9, this.myBot.gx, this.myBot.gy, this.dir.ToString());
			this.AddTime(flag2 ? this.ROAD_PAUSE : this.XY_PAUSE, true);
			this.wasMove = true;
			result = true;
		}
		return result;
	}

	// Token: 0x060000A0 RID: 160
	public void SmoothTPMyBot(int x, int y)
	{
		this.stopAutoMove();
		this.myBot.SetXY((float)x, (float)y);
	}

	// Token: 0x060000A1 RID: 161
	public void TPMyBot(int x, int y)
	{
		this.stopAutoMove();
		this.myBot.SetXY((float)x, (float)y);
		this.myBot.SyncXY();
		this.myBot.HideTail();
	}

	// Token: 0x060000A2 RID: 162
	public void InitMyBot(int x, int y, int id, string name)
	{
		ClientController.inited = true;
		this.robotRenderer.RemoveAllBots();
		this.myBotId = id;
		this.robotRenderer.AddNewBotForMe(x, y, id, name, out this.myBot);
		this.view_x = x;
		this.view_y = y;
		this.tcx = this.terrainRenderer.cx;
		this.tcy = this.terrainRenderer.cy;
	}

	// Token: 0x060000A3 RID: 163
	public bool GetAutoMove()
	{
		return this.automove;
	}

	// Token: 0x060000A4 RID: 164
	public bool CanMoveNow()
	{
		return Time.unscaledTime > this.TimeForNextOperation;
	}

	// Token: 0x060000A5 RID: 165
	public ClientController()
	{
	}

	// Token: 0x060000A6 RID: 166
	static ClientController()
	{
	}

	// Token: 0x040000CE RID: 206
	private List<RoutePoint> route = new List<RoutePoint>();

	// Token: 0x040000CF RID: 207
	private Dictionary<int, RoutePoint> points = new Dictionary<int, RoutePoint>();

	// Token: 0x040000D0 RID: 208
	private HashSet<int> openedSet = new HashSet<int>();

	// Token: 0x040000D1 RID: 209
	private HashSet<int> closedSet = new HashSet<int>();

	// Token: 0x040000D2 RID: 210
	public GameObject mainRenderer;

	// Token: 0x040000D3 RID: 211
	public GameObject obvyazkaObject;

	// Token: 0x040000D4 RID: 212
	public GameObject bzPrefab;

	// Token: 0x040000D5 RID: 213
	public GameObject boomPrefab;

	// Token: 0x040000D6 RID: 214
	public GameObject shotPrefab;

	// Token: 0x040000D7 RID: 215
	public GameObject crysPrefab;

	// Token: 0x040000D8 RID: 216
	public GameObject crys2Prefab;

	// Token: 0x040000D9 RID: 217
	public GameObject smokePrefab;

	// Token: 0x040000DA RID: 218
	public GameObject volcanoPrefab;

	// Token: 0x040000DB RID: 219
	public GameObject nohpfxPrefab;

	// Token: 0x040000DC RID: 220
	public GameObject nohpfxSmallPrefab;

	// Token: 0x040000DD RID: 221
	public GameObject healfxPrefab;

	// Token: 0x040000DE RID: 222
	public GameObject hurtfxPrefab;

	// Token: 0x040000DF RID: 223
	public GOPool bzPool;

	// Token: 0x040000E0 RID: 224
	public GOPool gunShotPool;

	// Token: 0x040000E1 RID: 225
	public GOPool boomPool;

	// Token: 0x040000E2 RID: 226
	public GOPool crysPlusPool;

	// Token: 0x040000E3 RID: 227
	public GameObject RenderWrapper;

	// Token: 0x040000E4 RID: 228
	public static bool ownSounds = true;

	// Token: 0x040000E5 RID: 229
	public static bool autoDigg = true;

	// Token: 0x040000E6 RID: 230
	public Button autoDiggButton;

	// Token: 0x040000E7 RID: 231
	public Text autoDiggButtonText;

	// Token: 0x040000E8 RID: 232
	public InputField maxFXIF;

	// Token: 0x040000E9 RID: 233
	public Image redPad;

	// Token: 0x040000EA RID: 234
	public GameObject Cursor;

	// Token: 0x040000EB RID: 235
	public Button NoGUIClickPad;

	// Token: 0x040000EC RID: 236
	private Obvyazka obvyazka;

	// Token: 0x040000ED RID: 237
	private ServerTime serverTime;

	// Token: 0x040000EE RID: 238
	private RobotRenderer robotRenderer;

	// Token: 0x040000EF RID: 239
	public TerrainRendererScript terrainRenderer;

	// Token: 0x040000F0 RID: 240
	public static MapModel map;

	// Token: 0x040000F1 RID: 241
	public static bool CtrlToggle = false;

	// Token: 0x040000F2 RID: 242
	public int r_x;

	// Token: 0x040000F3 RID: 243
	public int r_y;

	// Token: 0x040000F4 RID: 244
	public int view_x;

	// Token: 0x040000F5 RID: 245
	public int view_y;

	// Token: 0x040000F6 RID: 246
	public int myBotId = -1;

	// Token: 0x040000F7 RID: 247
	public int myBotLastSyncX = -1;

	// Token: 0x040000F8 RID: 248
	public int myBotLastSyncY = -1;

	// Token: 0x040000F9 RID: 249
	public RobotScript myBot;

	// Token: 0x040000FA RID: 250
	public bool notActive;

	// Token: 0x040000FB RID: 251
	public static ClientController THIS;

	// Token: 0x040000FC RID: 252
	private bool isControl;

	// Token: 0x040000FD RID: 253
	private string str1 = "TADG";

	// Token: 0x040000FE RID: 254
	private string str2 = "TAUR";

	// Token: 0x040000FF RID: 255
	private string str3 = "TAGR";

	// Token: 0x04000100 RID: 256
	private string str5 = "FINV";

	// Token: 0x04000101 RID: 257
	private string str6 = "INVN";

	// Token: 0x04000102 RID: 258
	private string str7 = "INUS";

	// Token: 0x04000103 RID: 259
	private string str8 = "Xgeo";

	// Token: 0x04000104 RID: 260
	private string str9 = "Xmov";

	// Token: 0x04000105 RID: 261
	private string str10 = "Xdig";

	// Token: 0x04000106 RID: 262
	private string str11 = "Xbld";

	// Token: 0x04000107 RID: 263
	private string str12 = "Xhur";

	// Token: 0x04000108 RID: 264
	private string str13 = "Xhea";

	// Token: 0x04000109 RID: 265
	private float TimeForNextOperation;

	// Token: 0x0400010A RID: 266
	private string lastDirKey = "up";

	// Token: 0x0400010B RID: 267
	private float slowingEffect = 1f;

	// Token: 0x0400010C RID: 268
	private float HURT_PAUSE = 0.5f;

	// Token: 0x0400010D RID: 269
	public float XY_PAUSE = 0.8f;

	// Token: 0x0400010E RID: 270
	public float ROAD_PAUSE = 0.8f;

	// Token: 0x0400010F RID: 271
	public float DEPTH = 65000f;

	// Token: 0x04000110 RID: 272
	private float BZ_PAUSE = 0.3f;

	// Token: 0x04000111 RID: 273
	public static int serverTimeOfLastFrame = 0;

	// Token: 0x04000112 RID: 274
	public static int clientTimeOfLastFrame = 0;

	// Token: 0x04000113 RID: 275
	public static int pongResponse = -1;

	// Token: 0x04000114 RID: 276
	private Vector2 cameraSpeed;

	// Token: 0x04000115 RID: 277
	private float tcx;

	// Token: 0x04000116 RID: 278
	private float tcy;

	// Token: 0x04000117 RID: 279
	private float peck;

	// Token: 0x04000118 RID: 280
	private float tremor;

	// Token: 0x04000119 RID: 281
	private Color[] smokeColors = new Color[]
	{
		new Color(1f, 1f, 1f),
		new Color(0.5f, 0.5f, 1f),
		new Color(0f, 1f, 1f),
		new Color(0.1f, 1f, 0.3f),
		new Color(0.7f, 1f, 0f),
		new Color(1f, 1f, 0f),
		new Color(1f, 0.6f, 0f),
		new Color(1f, 0.3f, 0.3f),
		new Color(1f, 0f, 1f)
	};

	// Token: 0x0400011A RID: 282
	private string[] crysFromCode = new string[]
	{
		"g",
		"r",
		"v",
		"b",
		"w",
		"c",
		"z"
	};

	// Token: 0x0400011B RID: 283
	public bool isProgrammator;

	// Token: 0x0400011C RID: 284
	private int firstHurt;

	// Token: 0x0400011D RID: 285
	private float slowRedAlpha;

	// Token: 0x0400011E RID: 286
	private Vector2 aveDCamera;

	// Token: 0x0400011F RID: 287
	private Vector2 aveDCamera2;

	// Token: 0x04000120 RID: 288
	public static bool MouseControl = false;

	// Token: 0x04000121 RID: 289
	private int tryGotoX;

	// Token: 0x04000122 RID: 290
	private int tryGotoY;

	// Token: 0x04000123 RID: 291
	private int GotoX;

	// Token: 0x04000124 RID: 292
	private int GotoY;

	// Token: 0x04000125 RID: 293
	private bool automove;

	// Token: 0x04000126 RID: 294
	public static bool CanGoto = true;

	// Token: 0x04000127 RID: 295
	private int BZ_DEBUG_GEN;

	// Token: 0x04000128 RID: 296
	private float lastHurtTime;

	// Token: 0x04000129 RID: 297
	public static bool active = true;

	// Token: 0x0400012A RID: 298
	private bool wasMove;

	// Token: 0x0400012B RID: 299
	public int dir;

	// Token: 0x0400012C RID: 300
	public static bool inited = false;
}
