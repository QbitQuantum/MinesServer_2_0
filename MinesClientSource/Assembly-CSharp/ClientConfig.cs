using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000011 RID: 17
public class ClientConfig
{
	// Token: 0x06000064 RID: 100 RVA: 0x0000CEDC File Offset: 0x0000B0DC
	public static void Toggle(int button)
	{
		if (!ClientConfig.ToggleStates.ContainsKey(button))
		{
			ClientConfig.ToggleStates.Add(button, -1);
		}
		Dictionary<int, int> toggleStates = ClientConfig.ToggleStates;
		int num = toggleStates[button];
		toggleStates[button] = num + 1;
		if (ClientConfig.ToggleStates[button] >= ClientConfig.toggleLists[button].Count)
		{
			ClientConfig.ToggleStates[button] = 0;
		}
		Debug.Log(string.Concat(new object[]
		{
			"toggle ",
			button,
			" > ",
			ClientConfig.ToggleStates[button],
			"/",
			ClientConfig.toggleLists[button].Count,
			"  ?",
			ClientConfig.toggleList1.Count
		}));
		if (ClientConfig.toggleLists[button].Count > 0)
		{
			List<string> list = ClientConfig.toggleLists[button][ClientConfig.ToggleStates[button]];
			for (int i = 0; i < list.Count; i += 3)
			{
				string a = list[i];
				string name = list[i + 1];
				string text = list[i + 2];
				if (!(a == "O"))
				{
					if (!(a == "="))
					{
						if (a == "+")
						{
							ClientConfig.ParseBoolean(name, text == "+");
						}
					}
					else
					{
						ClientConfig.ParseEquals(name, text);
					}
				}
				else
				{
					ClientConfig.ParseOperator(name);
				}
			}
		}
	}

	// Token: 0x06000065 RID: 101 RVA: 0x00004F23 File Offset: 0x00003123
	public static void EndConfig()
	{
		ClientConfig.TOGGLE_UPDATING = false;
	}

	// Token: 0x06000066 RID: 102 RVA: 0x0000D058 File Offset: 0x0000B258
	public static void SetDefaults()
	{
		GUIManager.THIS.InitExternalPanel();
		GUIManager.THIS.mapButton.gameObject.SetActive(true);
		GUIManager.THIS.AccountPanel.SetActive(true);
		MiniSkillManager.THIS.gameObject.SetActive(true);
		MapViewer.ResetCustomTable();
		ClientConfig.CHAT_SHOW_ID = false;
		ClientConfig.CHAT_SHOW_TIME = false;
		ClientConfig.CHAT_SHOW_NICK = true;
		ClientConfig.SHOW_MY_NICK = false;
		ClientConfig.OLD_PROGRAM_FORMAT = false;
		ClientConfig.noDistortion = false;
		ClientConfig.toggleList1.Clear();
		ClientConfig.toggleList2.Clear();
		ClientConfig.toggleList3.Clear();
		ClientConfig.toggleList4.Clear();
		ClientConfig.toggleList5.Clear();
		ClientConfig.toggleList6.Clear();
		ClientConfig.toggleList7.Clear();
		ClientConfig.toggleList8.Clear();
		ClientConfig.toggleList9.Clear();
		ClientConfig.toggleList0.Clear();
		ClientConfig.TOGGLE_UPDATING = false;
		ClientConfig.mouseDefMaxLen = 5000;
		ClientConfig.mouseDefMaxStack = 2000;
		ClientConfig.mouseDefR = 50;
		ClientConfig.mouseR = ClientConfig.mouseDefR;
		ClientConfig.mouseMaxLen = ClientConfig.mouseDefMaxLen;
		ClientConfig.mouseMaxStack = ClientConfig.mouseDefMaxStack;
		ClientConfig.mouseMapMaxLen = 5000;
		ClientConfig.mouseMapMaxStack = 2000;
		ClientConfig.mouseMapR = 50;
		ClientConfig.mouseNoDig = false;
		ClientConfig.gunRadius = true;
		ClientConfig.MOVE_UP_KEY = KeyCode.W;
		ClientConfig.MOVE_DOWN_KEY = KeyCode.S;
		ClientConfig.MOVE_LEFT_KEY = KeyCode.A;
		ClientConfig.MOVE_RIGHT_KEY = KeyCode.D;
		ClientConfig.DIGG_KEY = KeyCode.Z;
		ClientConfig.WARBLOCK_KEY = KeyCode.Y;
		ClientConfig.BLOCK_KEY = KeyCode.F;
		ClientConfig.ROAD_KEY = KeyCode.H;
		ClientConfig.QUADRO_KEY = KeyCode.J;
		ClientConfig.HEAL_KEY = KeyCode.V;
		ClientConfig.GEO_KEY = KeyCode.G;
		ClientConfig.AGR_KEY = KeyCode.L;
		ClientConfig.AUTODIG_KEY = KeyCode.E;
		ClientConfig.MAP_KEY = KeyCode.M;
		ClientConfig.LOCALCHAT_KEY = KeyCode.T;
		ClientConfig.PROG_KEY = KeyCode.R;
		ClientConfig.INV_KEY = KeyCode.I;
		ClientConfig.SOUND_BASKET = true;
		ClientConfig.SOUND_SIGNAL = true;
		ClientConfig.SOUND_BOMB = true;
		ClientConfig.SOUND_BOMBTICK = true;
		ClientConfig.SOUND_DEATH = true;
		ClientConfig.SOUND_DESTROY = true;
		ClientConfig.SOUND_EMI = true;
		ClientConfig.SOUND_GEOLOGY = true;
		ClientConfig.SOUND_HEAL = true;
		ClientConfig.SOUND_HURT = true;
		ClientConfig.SOUND_MINING = true;
		ClientConfig.SOUND_DIZZ = true;
		ClientConfig.SOUND_TP_IN = true;
		ClientConfig.SOUND_TP_OUT = true;
		ClientConfig.SOUND_VOLC = true;
		ClientConfig.SOUND_C190 = true;
		ClientController.THIS.boomPool.max_size = 30;
		ClientController.THIS.gunShotPool.max_size = 50;
		ClientController.THIS.crysPlusPool.max_size = 20;
		ClientController.THIS.bzPool.max_size = 20;
	}

	// Token: 0x06000067 RID: 103 RVA: 0x0000D2B4 File Offset: 0x0000B4B4
	public static void ParseBoolean(string name, bool value)
	{
		if (ClientConfig.TOGGLE_UPDATING)
		{
			ClientConfig.currentToggleList.Add("+");
			ClientConfig.currentToggleList.Add(name);
			ClientConfig.currentToggleList.Add(value ? "+" : "-");
			Debug.Log("currentToggleList.Count=" + ClientConfig.currentToggleList.Count);
			return;
		}
		Debug.Log(" ParseBoolean " + name + " " + value.ToString());
		string a = name.ToLower();
		if (a == "sfxdizz")
		{
			ClientConfig.SOUND_DIZZ = value;
			return;
		}
		if (!(a == "radius"))
		{
			if (a == "chatnick")
			{
				ClientConfig.CHAT_SHOW_NICK = value;
				ChatManager.THIS.UpdateChatStyle();
				return;
			}
			if (a == "sfxbasket")
			{
				ClientConfig.SOUND_BASKET = value;
				return;
			}
			if (a == "chatid")
			{
				ClientConfig.CHAT_SHOW_ID = value;
				ChatManager.THIS.UpdateChatStyle();
				return;
			}
			if (a == "sfxtpout")
			{
				ClientConfig.SOUND_TP_OUT = value;
				return;
			}
			if (a == "showmynick")
			{
				ClientConfig.SHOW_MY_NICK = value;
				return;
			}
			if (a == "gunradius")
			{
				ClientConfig.gunRadius = value;
				return;
			}
			if (a == "mousenodig")
			{
				ClientConfig.mouseNoDig = value;
				return;
			}
			if (!(a == "gunr"))
			{
				if (a == "acc")
				{
					GUIManager.THIS.AccountPanel.SetActive(value);
					return;
				}
				if (a == "oldprogramformat")
				{
					ClientConfig.OLD_PROGRAM_FORMAT = value;
					return;
				}
				if (a == "skills")
				{
					MiniSkillManager.THIS.gameObject.SetActive(value);
					return;
				}
				if (a == "sfxdeath")
				{
					ClientConfig.SOUND_DEATH = value;
					return;
				}
				if (a == "sfxbomb")
				{
					ClientConfig.SOUND_BOMB = value;
					return;
				}
				if (!(a == "mousenodigg"))
				{
					if (a == "sfxheal")
					{
						ClientConfig.SOUND_HEAL = value;
						return;
					}
					if (a == "sfxvolc")
					{
						ClientConfig.SOUND_VOLC = value;
						return;
					}
					if (a == "sfxgeo")
					{
						ClientConfig.SOUND_GEOLOGY = value;
						return;
					}
					if (a == "sfxtick")
					{
						ClientConfig.SOUND_BOMBTICK = value;
						return;
					}
					if (a == "sfxdestroy")
					{
						ClientConfig.SOUND_DESTROY = value;
						return;
					}
					if (a == "sfxemi")
					{
						ClientConfig.SOUND_EMI = value;
						return;
					}
					if (a == "plain")
					{
						ClientConfig.noDistortion = value;
						return;
					}
					if (a == "sfxhurt")
					{
						ClientConfig.SOUND_HURT = value;
						return;
					}
					if (a == "map")
					{
						GUIManager.THIS.mapButton.gameObject.SetActive(value);
						return;
					}
					if (a == "sfxmine")
					{
						ClientConfig.SOUND_MINING = value;
						return;
					}
					if (a == "sfxsignal")
					{
						ClientConfig.SOUND_SIGNAL = value;
						return;
					}
					if (a == "sfxc190")
					{
						ClientConfig.SOUND_C190 = value;
						return;
					}
					if (a == "sfxtpin")
					{
						ClientConfig.SOUND_TP_IN = value;
						return;
					}
					if (a == "chattime")
					{
						ClientConfig.CHAT_SHOW_TIME = value;
						ChatManager.THIS.UpdateChatStyle();
					}
				}
			}
		}
	}

	// Token: 0x06000068 RID: 104 RVA: 0x0000D5CC File Offset: 0x0000B7CC
	public static KeyCode TranslateCode(string input)
	{
		string a = input.ToLower();
		if (a == "f8")
		{
			return KeyCode.F8;
		}
		if (a == "f9")
		{
			return KeyCode.F9;
		}
		if (a == "f2")
		{
			return KeyCode.F2;
		}
		if (a == "f3")
		{
			return KeyCode.F3;
		}
		if (a == "f1")
		{
			return KeyCode.F1;
		}
		if (a == "f7")
		{
			return KeyCode.F7;
		}
		if (a == "f4")
		{
			return KeyCode.F4;
		}
		if (a == "f5")
		{
			return KeyCode.F5;
		}
		if (a == "f6")
		{
			return KeyCode.F6;
		}
		if (a == "+")
		{
			return KeyCode.Plus;
		}
		if (a == "-")
		{
			return KeyCode.Minus;
		}
		if (a == ">")
		{
			return KeyCode.Greater;
		}
		if (a == "<")
		{
			return KeyCode.Less;
		}
		if (a == "g")
		{
			return KeyCode.G;
		}
		if (a == "d")
		{
			return KeyCode.D;
		}
		if (a == "e")
		{
			return KeyCode.E;
		}
		if (a == "c")
		{
			return KeyCode.C;
		}
		if (a == "a")
		{
			return KeyCode.A;
		}
		if (a == "f")
		{
			return KeyCode.F;
		}
		if (a == "b")
		{
			return KeyCode.B;
		}
		if (a == "l")
		{
			return KeyCode.L;
		}
		if (a == "m")
		{
			return KeyCode.M;
		}
		if (a == "n")
		{
			return KeyCode.N;
		}
		if (a == "o")
		{
			return KeyCode.O;
		}
		if (a == "k")
		{
			return KeyCode.K;
		}
		if (a == "h")
		{
			return KeyCode.H;
		}
		if (a == "i")
		{
			return KeyCode.I;
		}
		if (a == "t")
		{
			return KeyCode.T;
		}
		if (a == "u")
		{
			return KeyCode.U;
		}
		if (a == "j")
		{
			return KeyCode.J;
		}
		if (a == "q")
		{
			return KeyCode.Q;
		}
		if (a == "v")
		{
			return KeyCode.V;
		}
		if (a == "w")
		{
			return KeyCode.W;
		}
		if (a == "s")
		{
			return KeyCode.S;
		}
		if (a == "p")
		{
			return KeyCode.P;
		}
		if (a == "r")
		{
			return KeyCode.R;
		}
		if (a == "f10")
		{
			return KeyCode.F10;
		}
		if (a == "}")
		{
			return KeyCode.RightCurlyBracket;
		}
		if (a == "f11")
		{
			return KeyCode.F11;
		}
		if (a == "f12")
		{
			return KeyCode.F12;
		}
		if (a == "y")
		{
			return KeyCode.Y;
		}
		if (a == "z")
		{
			return KeyCode.Z;
		}
		if (a == "{")
		{
			return KeyCode.LeftCurlyBracket;
		}
		if (a == "x")
		{
			return KeyCode.X;
		}
		return KeyCode.None;
	}

	// Token: 0x06000069 RID: 105 RVA: 0x0000D8C8 File Offset: 0x0000BAC8
	public static int fromHex(string c)
	{
		if (c == "4")
		{
			return 4;
		}
		if (c == "5")
		{
			return 5;
		}
		if (c == "6")
		{
			return 6;
		}
		if (c == "7")
		{
			return 7;
		}
		if (c == "0")
		{
			return 0;
		}
		if (c == "1")
		{
			return 1;
		}
		if (c == "2")
		{
			return 2;
		}
		if (c == "3")
		{
			return 3;
		}
		if (c == "8")
		{
			return 8;
		}
		if (c == "9")
		{
			return 9;
		}
		if (c == "d")
		{
			return 13;
		}
		if (c == "e")
		{
			return 14;
		}
		if (c == "a")
		{
			return 10;
		}
		if (c == "f")
		{
			return 15;
		}
		if (c == "b")
		{
			return 11;
		}
		if (c == "c")
		{
			return 12;
		}
		return 0;
	}

	// Token: 0x0600006A RID: 106 RVA: 0x0000D9D0 File Offset: 0x0000BBD0
	public static void ParseOperator(string name)
	{
		name = name.ToUpper();
		if ((name.Length < 6 || name.Substring(0, 6) != "TOGGLE") && ClientConfig.TOGGLE_UPDATING)
		{
			ClientConfig.currentToggleList.Add("O");
			ClientConfig.currentToggleList.Add(name);
			ClientConfig.currentToggleList.Add("");
			return;
		}
		if (name == "MAPMODE2")
		{
			MapViewer.THIS.modeDropdown.value = 2;
			return;
		}
		if (name == "MAPMODE4")
		{
			MapViewer.THIS.modeDropdown.value = 4;
			return;
		}
		if (name == "MAPMODE3")
		{
			MapViewer.THIS.modeDropdown.value = 3;
			return;
		}
		if (name == "MAPMODE1")
		{
			MapViewer.THIS.modeDropdown.value = 1;
			return;
		}
		if (name == "MAPMODE0")
		{
			MapViewer.THIS.modeDropdown.value = 0;
			return;
		}
		if (name == "CMAP")
		{
			MapViewer.ResetCustomTable();
			MapViewer.THIS.modeDropdown.value = 4;
			if (!MapViewer.THIS.gameObject.activeSelf)
			{
				MapViewer.THIS.Show();
				return;
			}
		}
		else
		{
			if (name == "CMAPRESET")
			{
				MapViewer.ResetCustomTable();
				return;
			}
			if (name == "CMR")
			{
				MapViewer.ResetCustomTable();
				return;
			}
			if (!(name == "CMC"))
			{
				if (name == "SHOWMAP")
				{
					if (!MapViewer.THIS.gameObject.activeSelf)
					{
						MapViewer.THIS.Show();
						return;
					}
				}
				else if (name == "HIDEMAP")
				{
					if (MapViewer.THIS.gameObject.activeSelf)
					{
						MapViewer.THIS.OnExit();
						return;
					}
				}
				else
				{
					if (name == "TOGGLE7")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item = new List<string>();
						ClientConfig.toggleList7.Add(item);
						ClientConfig.currentToggleList = item;
						return;
					}
					if (name == "TOGGLE4")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item2 = new List<string>();
						ClientConfig.toggleList4.Add(item2);
						ClientConfig.currentToggleList = item2;
						return;
					}
					if (name == "TOGGLE5")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item3 = new List<string>();
						ClientConfig.toggleList5.Add(item3);
						ClientConfig.currentToggleList = item3;
						return;
					}
					if (name == "TOGGLE0")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item4 = new List<string>();
						ClientConfig.toggleList0.Add(item4);
						ClientConfig.currentToggleList = item4;
						return;
					}
					if (name == "TOGGLE1")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item5 = new List<string>();
						ClientConfig.toggleList1.Add(item5);
						ClientConfig.currentToggleList = item5;
						return;
					}
					if (name == "TOGGLE6")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item6 = new List<string>();
						ClientConfig.toggleList6.Add(item6);
						ClientConfig.currentToggleList = item6;
						return;
					}
					if (name == "TOGGLE9")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item7 = new List<string>();
						ClientConfig.toggleList9.Add(item7);
						ClientConfig.currentToggleList = item7;
						return;
					}
					if (name == "TOGGLE2")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item8 = new List<string>();
						ClientConfig.toggleList2.Add(item8);
						ClientConfig.currentToggleList = item8;
						return;
					}
					if (name == "TOGGLE3")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item9 = new List<string>();
						ClientConfig.toggleList3.Add(item9);
						ClientConfig.currentToggleList = item9;
						return;
					}
					if (name == "TOGGLE8")
					{
						ClientConfig.TOGGLE_UPDATING = true;
						List<string> item10 = new List<string>();
						ClientConfig.toggleList8.Add(item10);
						ClientConfig.currentToggleList = item10;
						return;
					}
					if (name == "TOGGLEEND")
					{
						ClientConfig.TOGGLE_UPDATING = false;
						return;
					}
					if (name == "CMAPCHOOSE")
					{
						MapViewer.THIS.modeDropdown.value = 4;
					}
				}
			}
		}
	}

	// Token: 0x0600006B RID: 107 RVA: 0x0000DD8C File Offset: 0x0000BF8C
	public static void ParseEquals(string name, string value)
	{
		if (ClientConfig.TOGGLE_UPDATING)
		{
			ClientConfig.currentToggleList.Add("=");
			ClientConfig.currentToggleList.Add(name);
			ClientConfig.currentToggleList.Add(value);
			return;
		}
		short num = 0;
		float num2 = 0f;
		bool flag = short.TryParse(value, out num);
		bool flag2 = float.TryParse(value, out num2);
		name = name.ToLower();
		value = value.ToLower();
		if (name.Substring(0, 4) == "cmap" && short.TryParse(name.Substring(4), out num) && num > 0 && num < 255 && value.Length >= 3)
		{
			int num3 = ClientConfig.fromHex(value.Substring(0, 1));
			int num4 = ClientConfig.fromHex(value.Substring(1, 1));
			int num5 = ClientConfig.fromHex(value.Substring(2, 1));
			if (value.Length >= 5 && value.Substring(3, 1) == "!")
			{
				int num6 = ClientConfig.fromHex(value.Substring(4, 1));
				MapViewer.customBlinkRateTable[(int)num] = 2.5f * Mathf.Pow(1.3f, (float)num6);
			}
			MapViewer.customTable[(int)num] = new Color((float)num3 / 16f, (float)num4 / 16f, (float)num5 / 16f);
		}
		string a = name.ToLower();
		if (a == "mouser")
		{
			if (flag && num > 10 && num < 30000)
			{
				ClientConfig.mouseDefR = (int)num;
				ClientConfig.mouseR = ClientConfig.mouseDefR;
				return;
			}
		}
		else if (a == "mousemaxstack")
		{
			if (flag && num > 100)
			{
				ClientConfig.mouseDefMaxStack = (int)num;
				ClientConfig.mouseMaxStack = ClientConfig.mouseDefMaxStack;
				return;
			}
		}
		else if (a == "crysanimations")
		{
			if (flag && num >= 0 && num < 4096)
			{
				ClientConfig.animationNumCrysPlus = (int)num;
				ClientController.THIS.crysPlusPool.max_size = ClientConfig.animationNumCrysPlus;
				return;
			}
		}
		else
		{
			if (a == "key_quadro")
			{
				ClientConfig.QUADRO_KEY = ClientConfig.TranslateCode(value);
				return;
			}
			if (a == "key_agr")
			{
				ClientConfig.AGR_KEY = ClientConfig.TranslateCode(value);
				return;
			}
			if (a == "mousemapmaxstack")
			{
				if (flag && num > 100)
				{
					ClientConfig.mouseMapMaxStack = (int)num;
					return;
				}
			}
			else if (a == "diganimations")
			{
				if (flag && num >= 0 && num < 4096)
				{
					ClientConfig.animationNumBz = (int)num;
					ClientController.THIS.bzPool.max_size = ClientConfig.animationNumBz;
					return;
				}
			}
			else
			{
				if (a == "key_toggle8")
				{
					ClientConfig.TOGGLE8_KEY = ClientConfig.TranslateCode(value);
					return;
				}
				if (a == "key_toggle9")
				{
					ClientConfig.TOGGLE9_KEY = ClientConfig.TranslateCode(value);
					return;
				}
				if (a == "boomanimations")
				{
					if (flag && num >= 0 && num < 4096)
					{
						ClientConfig.animationNumBoom = (int)num;
						ClientController.THIS.boomPool.max_size = ClientConfig.animationNumBoom;
						return;
					}
				}
				else if (a == "mousemapmaxlen")
				{
					if (flag && num > 100)
					{
						ClientConfig.mouseMapMaxLen = (int)num;
						return;
					}
				}
				else
				{
					if (a == "key_toggle3")
					{
						ClientConfig.TOGGLE3_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle0")
					{
						ClientConfig.TOGGLE0_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle1")
					{
						ClientConfig.TOGGLE1_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle2")
					{
						ClientConfig.TOGGLE2_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle5")
					{
						ClientConfig.TOGGLE5_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle4")
					{
						ClientConfig.TOGGLE4_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle6")
					{
						ClientConfig.TOGGLE6_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_toggle7")
					{
						ClientConfig.TOGGLE7_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_local")
					{
						ClientConfig.LOCALCHAT_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "key_dig")
					{
						ClientConfig.DIGG_KEY = ClientConfig.TranslateCode(value);
						return;
					}
					if (a == "mousemaxlen")
					{
						if (flag && num > 100)
						{
							ClientConfig.mouseDefMaxLen = (int)num;
							ClientConfig.mouseMaxLen = ClientConfig.mouseDefMaxLen;
							return;
						}
					}
					else
					{
						if (a == "key_map")
						{
							ClientConfig.MAP_KEY = ClientConfig.TranslateCode(value);
							return;
						}
						if (a == "mousemapr")
						{
							if (flag && num > 10 && num < 30000)
							{
								ClientConfig.mouseMapR = (int)num;
								return;
							}
						}
						else
						{
							if (a == "key_up")
							{
								ClientConfig.MOVE_UP_KEY = ClientConfig.TranslateCode(value);
								return;
							}
							if (a == "gunanimations")
							{
								if (flag && num >= 0 && num < 4096)
								{
									ClientConfig.animationNumGunShot = (int)num;
									ClientController.THIS.gunShotPool.max_size = ClientConfig.animationNumGunShot;
									return;
								}
							}
							else
							{
								if (a == "key_autodig")
								{
									ClientConfig.AUTODIG_KEY = ClientConfig.TranslateCode(value);
									return;
								}
								if (a == "guizoom")
								{
									if (flag2 && num2 > 0.01f && num2 < 5f)
									{
										ServerController.THIS.MainCanvasScaler.scaleFactor = num2;
										return;
									}
								}
								else
								{
									if (a == "key_wb")
									{
										ClientConfig.WARBLOCK_KEY = ClientConfig.TranslateCode(value);
										return;
									}
									if (a == "key_geo")
									{
										ClientConfig.GEO_KEY = ClientConfig.TranslateCode(value);
										return;
									}
									if (a == "key_inv")
									{
										ClientConfig.INV_KEY = ClientConfig.TranslateCode(value);
										return;
									}
									if (a == "key_right")
									{
										ClientConfig.MOVE_RIGHT_KEY = ClientConfig.TranslateCode(value);
										return;
									}
									if (a == "zoom")
									{
										if (flag2 && num2 >= 0.2f && num2 < 5f)
										{
											TerrainRendererScript.unitSize = 16f * num2;
											TerrainRendererScript.needUpdate = true;
											ClientController.THIS.terrainRenderer.RecreateMeshes();
											return;
										}
									}
									else
									{
										if (a == "key_prog")
										{
											ClientConfig.PROG_KEY = ClientConfig.TranslateCode(value);
											return;
										}
										if (!(a == "key_autorem"))
										{
											if (a == "key_left")
											{
												ClientConfig.MOVE_LEFT_KEY = ClientConfig.TranslateCode(value);
												return;
											}
											if (a == "key_down")
											{
												ClientConfig.MOVE_DOWN_KEY = ClientConfig.TranslateCode(value);
												return;
											}
											if (a == "key_heal")
											{
												ClientConfig.HEAL_KEY = ClientConfig.TranslateCode(value);
												return;
											}
											if (a == "key_block")
											{
												ClientConfig.BLOCK_KEY = ClientConfig.TranslateCode(value);
											}
										}
									}
								}
							}
						}
					}
				}
			}
		}
	}

	// Token: 0x04000068 RID: 104
	public static int mouseMaxLen = 5000;

	// Token: 0x04000069 RID: 105
	public static int mouseMaxStack = 2000;

	// Token: 0x0400006A RID: 106
	public static int mouseR = 50;

	// Token: 0x0400006B RID: 107
	public static int mouseDefMaxLen = 5000;

	// Token: 0x0400006C RID: 108
	public static int mouseDefMaxStack = 2000;

	// Token: 0x0400006D RID: 109
	public static int mouseDefR = 50;

	// Token: 0x0400006E RID: 110
	public static bool mouseNoDig = false;

	// Token: 0x0400006F RID: 111
	public static bool gunRadius = true;

	// Token: 0x04000070 RID: 112
	public static bool noDistortion = false;

	// Token: 0x04000071 RID: 113
	public static int mouseMapMaxLen = 5000;

	// Token: 0x04000072 RID: 114
	public static int mouseMapMaxStack = 20000;

	// Token: 0x04000073 RID: 115
	public static int mouseMapR = 4000;

	// Token: 0x04000074 RID: 116
	public static int animationNumBoom = 30;

	// Token: 0x04000075 RID: 117
	public static int animationNumGunShot = 50;

	// Token: 0x04000076 RID: 118
	public static int animationNumCrysPlus = 20;

	// Token: 0x04000077 RID: 119
	public static int animationNumBz = 20;

	// Token: 0x04000078 RID: 120
	public static KeyCode MOVE_UP_KEY = KeyCode.W;

	// Token: 0x04000079 RID: 121
	public static KeyCode MOVE_DOWN_KEY = KeyCode.S;

	// Token: 0x0400007A RID: 122
	public static KeyCode MOVE_LEFT_KEY = KeyCode.A;

	// Token: 0x0400007B RID: 123
	public static KeyCode MOVE_RIGHT_KEY = KeyCode.D;

	// Token: 0x0400007C RID: 124
	public static KeyCode DIGG_KEY = KeyCode.Z;

	// Token: 0x0400007D RID: 125
	public static KeyCode WARBLOCK_KEY = KeyCode.Y;

	// Token: 0x0400007E RID: 126
	public static KeyCode BLOCK_KEY = KeyCode.F;

	// Token: 0x0400007F RID: 127
	public static KeyCode ROAD_KEY = KeyCode.H;

	// Token: 0x04000080 RID: 128
	public static KeyCode QUADRO_KEY = KeyCode.J;

	// Token: 0x04000081 RID: 129
	public static KeyCode HEAL_KEY = KeyCode.V;

	// Token: 0x04000082 RID: 130
	public static KeyCode GEO_KEY = KeyCode.G;

	// Token: 0x04000083 RID: 131
	public static KeyCode AUTOREM_KEY = KeyCode.B;

	// Token: 0x04000084 RID: 132
	public static KeyCode AGR_KEY = KeyCode.L;

	// Token: 0x04000085 RID: 133
	public static KeyCode AUTODIG_KEY = KeyCode.E;

	// Token: 0x04000086 RID: 134
	public static KeyCode MAP_KEY = KeyCode.M;

	// Token: 0x04000087 RID: 135
	public static KeyCode LOCALCHAT_KEY = KeyCode.T;

	// Token: 0x04000088 RID: 136
	public static KeyCode PROG_KEY = KeyCode.R;

	// Token: 0x04000089 RID: 137
	public static KeyCode INV_KEY = KeyCode.I;

	// Token: 0x0400008A RID: 138
	public static KeyCode TOGGLE1_KEY = KeyCode.None;

	// Token: 0x0400008B RID: 139
	public static KeyCode TOGGLE2_KEY = KeyCode.None;

	// Token: 0x0400008C RID: 140
	public static KeyCode TOGGLE3_KEY = KeyCode.None;

	// Token: 0x0400008D RID: 141
	public static KeyCode TOGGLE4_KEY = KeyCode.None;

	// Token: 0x0400008E RID: 142
	public static KeyCode TOGGLE5_KEY = KeyCode.None;

	// Token: 0x0400008F RID: 143
	public static KeyCode TOGGLE6_KEY = KeyCode.None;

	// Token: 0x04000090 RID: 144
	public static KeyCode TOGGLE7_KEY = KeyCode.None;

	// Token: 0x04000091 RID: 145
	public static KeyCode TOGGLE8_KEY = KeyCode.None;

	// Token: 0x04000092 RID: 146
	public static KeyCode TOGGLE9_KEY = KeyCode.None;

	// Token: 0x04000093 RID: 147
	public static KeyCode TOGGLE0_KEY = KeyCode.None;

	// Token: 0x04000094 RID: 148
	public static bool SOUND_BASKET = true;

	// Token: 0x04000095 RID: 149
	public static bool SOUND_SIGNAL = true;

	// Token: 0x04000096 RID: 150
	public static bool SOUND_BOMB = true;

	// Token: 0x04000097 RID: 151
	public static bool SOUND_BOMBTICK = true;

	// Token: 0x04000098 RID: 152
	public static bool SOUND_DEATH = true;

	// Token: 0x04000099 RID: 153
	public static bool SOUND_DESTROY = true;

	// Token: 0x0400009A RID: 154
	public static bool SOUND_EMI = true;

	// Token: 0x0400009B RID: 155
	public static bool SOUND_GEOLOGY = true;

	// Token: 0x0400009C RID: 156
	public static bool SOUND_HEAL = true;

	// Token: 0x0400009D RID: 157
	public static bool SOUND_HURT = true;

	// Token: 0x0400009E RID: 158
	public static bool SOUND_MINING = true;

	// Token: 0x0400009F RID: 159
	public static bool SOUND_DIZZ = true;

	// Token: 0x040000A0 RID: 160
	public static bool SOUND_TP_IN = true;

	// Token: 0x040000A1 RID: 161
	public static bool SOUND_TP_OUT = true;

	// Token: 0x040000A2 RID: 162
	public static bool SOUND_VOLC = true;

	// Token: 0x040000A3 RID: 163
	public static bool SOUND_C190 = true;

	// Token: 0x040000A4 RID: 164
	public static bool TOGGLE_UPDATING = false;

	// Token: 0x040000A5 RID: 165
	public static bool OLD_PROGRAM_FORMAT = false;

	// Token: 0x040000A6 RID: 166
	public static bool SHOW_MY_NICK = false;

	// Token: 0x040000A7 RID: 167
	public static List<List<string>> toggleList1 = new List<List<string>>();

	// Token: 0x040000A8 RID: 168
	public static List<List<string>> toggleList2 = new List<List<string>>();

	// Token: 0x040000A9 RID: 169
	public static List<List<string>> toggleList3 = new List<List<string>>();

	// Token: 0x040000AA RID: 170
	public static List<List<string>> toggleList4 = new List<List<string>>();

	// Token: 0x040000AB RID: 171
	public static List<List<string>> toggleList5 = new List<List<string>>();

	// Token: 0x040000AC RID: 172
	public static List<List<string>> toggleList6 = new List<List<string>>();

	// Token: 0x040000AD RID: 173
	public static List<List<string>> toggleList7 = new List<List<string>>();

	// Token: 0x040000AE RID: 174
	public static List<List<string>> toggleList8 = new List<List<string>>();

	// Token: 0x040000AF RID: 175
	public static List<List<string>> toggleList9 = new List<List<string>>();

	// Token: 0x040000B0 RID: 176
	public static List<List<string>> toggleList0 = new List<List<string>>();

	// Token: 0x040000B1 RID: 177
	public static List<string> currentToggleList;

	// Token: 0x040000B2 RID: 178
	public static Dictionary<int, int> ToggleStates = new Dictionary<int, int>();

	// Token: 0x040000B3 RID: 179
	public static List<List<string>>[] toggleLists = new List<List<string>>[]
	{
		ClientConfig.toggleList0,
		ClientConfig.toggleList1,
		ClientConfig.toggleList2,
		ClientConfig.toggleList3,
		ClientConfig.toggleList4,
		ClientConfig.toggleList5,
		ClientConfig.toggleList6,
		ClientConfig.toggleList7,
		ClientConfig.toggleList8,
		ClientConfig.toggleList9
	};

	// Token: 0x040000B4 RID: 180
	public static bool CHAT_SHOW_ID = false;

	// Token: 0x040000B5 RID: 181
	public static bool CHAT_SHOW_TIME = false;

	// Token: 0x040000B6 RID: 182
	public static bool CHAT_SHOW_NICK = true;
}
