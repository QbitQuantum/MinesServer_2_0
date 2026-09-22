using System;
using System.Collections.Generic;
using System.Text;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000060 RID: 96
public class ServerController : MonoBehaviour
{
	// Token: 0x06000250 RID: 592 RVA: 0x00004F6F File Offset: 0x0000316F
	private void Start()
	{
	}

	// Token: 0x06000251 RID: 593 RVA: 0x0002579C File Offset: 0x0002399C
	public void Init()
	{
		ServerController.THIS = this;
		this.obvyazka = base.gameObject.GetComponent<Obvyazka>();
		this.robotRenderer = this.mainRenderer.GetComponent<RobotRenderer>();
		this.terrainRenderer = this.mainRenderer.GetComponent<TerrainRendererScript>();
		this.clientController = this.clientControllerObject.GetComponent<ClientController>();
		this._typeBuffer = new byte[1];
		this.obvyazka.OnB("HB", new TypedCallback<byte[]>(this.HubTranslator), false);
		this.obvyazka.OnU("BI", new TypedCallback<string>(this.BotInfoHandler), false);
		this.obvyazka.OnU("@T", new TypedCallback<string>(this.TPHandler), false);
		this.obvyazka.OnU("@t", new TypedCallback<string>(this.SmoothTPHandler), false);
		this.obvyazka.OnU("sp", new TypedCallback<string>(this.SpeedHandler), false);
		this.obvyazka.OnU("@L", new TypedCallback<string>(this.LiveHandler), false);
		this.obvyazka.OnU("@S", new TypedCallback<string>(this.SkillHandler), false);
		this.obvyazka.OnU("@B", new TypedCallback<string>(this.BasketHandler), false);
		this.obvyazka.OnU("NL", new TypedCallback<string>(this.NickListHandler), false);
		this.obvyazka.OnU("ON", new TypedCallback<string>(this.OnlineHandler), false);
		this.obvyazka.OnU("LV", new TypedCallback<string>(this.LevelHandler), false);
		this.obvyazka.OnU("GU", new TypedCallback<string>(this.PopupHandler), false);
		this.obvyazka.OnU("Gu", new TypedCallback<string>(this.PopupCloseHandler), false);
		this.obvyazka.OnU("GR", new TypedCallback<string>(this.OpenURLHandler), false);
		this.obvyazka.OnU("cS", new TypedCallback<string>(this.ClanShowHandler), false);
		this.obvyazka.OnU("cH", new TypedCallback<string>(this.ClanHideHandler), false);
		this.obvyazka.OnU("$$", new TypedCallback<string>(this.PurchaseHandler), false);
		this.obvyazka.OnU("P$", new TypedCallback<string>(this.MoneyHandler), false);
		this.obvyazka.OnU("PM", new TypedCallback<string>(this.ModulesHandler), false);
		this.obvyazka.OnU("@P", new TypedCallback<string>(this.ProgrammatorHandler), false);
		this.obvyazka.OnU("#P", new TypedCallback<string>(this.ProgrammatorOpenHandler), false);
		this.obvyazka.OnU("#p", new TypedCallback<string>(this.ProgrammatorUpdateHandler), false);
		this.obvyazka.OnU("OK", new TypedCallback<string>(this.OKHandler), false);
		this.obvyazka.OnU("IN", new TypedCallback<string>(this.InventoryHandler), false);
		this.obvyazka.OnU("BC", new TypedCallback<string>(this.BadCellsHandler), false);
		this.obvyazka.OnU("BA", new TypedCallback<string>(this.AgrHandler), false);
		this.obvyazka.OnU("BR", new TypedCallback<string>(this.AutoRemHandler), false);
		this.obvyazka.OnU("BD", new TypedCallback<string>(this.AutoDiggHandler), false);
		this.obvyazka.OnU("BH", new TypedCallback<string>(this.HandModeHandler), false);
		this.obvyazka.OnU("SP", new TypedCallback<string>(this.PanelHandler), false);
		this.obvyazka.OnU("GE", new TypedCallback<string>(this.GeoHandler), false);
		this.obvyazka.OnU("SU", new TypedCallback<string>(this.SuHandler), false);
		this.obvyazka.OnU("mN", new TypedCallback<string>(ChatManager.THIS.mnHandler), false);
		this.obvyazka.OnU("mL", new TypedCallback<string>(ChatManager.THIS.mlHandler), false);
		this.obvyazka.OnU("mO", new TypedCallback<string>(ChatManager.THIS.moHandler), false);
		this.obvyazka.OnU("mU", new TypedCallback<string>(ChatManager.THIS.muHandler), false);
		this.obvyazka.OnU("mC", new TypedCallback<string>(ChatManager.THIS.mcHandler), false);
		this.obvyazka.OnU("MM", new TypedCallback<string>(this.UMPHandler), false);
		this.obvyazka.OnU("MN", new TypedCallback<string>(this.MNHandler), false);
		this.obvyazka.OnU("MP", new TypedCallback<string>(this.MPHandler), false);
		this.obvyazka.OnU("#S", new TypedCallback<string>(this.SettingsHandler), false);
		this.obvyazka.OnU("#F", new TypedCallback<string>(this.ClientConfigHandler), false);
		this.obvyazka.OnU("BB", new TypedCallback<string>(this.BibikaHandler), false);
		this.obvyazka.OnU("@R", new TypedCallback<string>(this.respHandler), false);
		this.obvyazka.OnU("GO", new TypedCallback<string>(this.GoHandler), false);
		this.obvyazka.OnU("DR", new TypedCallback<string>(this.DailyRewardNotificationHandler), false);
	}

	// Token: 0x06000252 RID: 594 RVA: 0x00006605 File Offset: 0x00004805
	private void DailyRewardNotificationHandler(ref string msg)
	{
		GUIManager.THIS.DailyRewardToggle(msg == "1");
	}

	// Token: 0x06000253 RID: 595 RVA: 0x00025D78 File Offset: 0x00023F78
	private void GoHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		TutorialNavigation.THIS.SetNaviArrow(int.Parse(array[0]), int.Parse(array[1]));
	}

	// Token: 0x06000254 RID: 596 RVA: 0x0000661F File Offset: 0x0000481F
	private void respHandler(ref string msg)
	{
		ClientController.THIS.stopAutoMove();
	}

	// Token: 0x06000255 RID: 597 RVA: 0x00025DB4 File Offset: 0x00023FB4
	private void BibikaHandler(ref string msg)
	{
		bool sound_SIGNAL = ClientConfig.SOUND_SIGNAL;
		if (sound_SIGNAL)
		{
			SoundManager.THIS.PlayBibika();
		}
	}

	// Token: 0x06000256 RID: 598 RVA: 0x00025DD8 File Offset: 0x00023FD8
	private void ClientConfigHandler(ref string msg)
	{
		bool ignoreConfig = WorldInitScript.ignoreConfig;
		if (!ignoreConfig)
		{
			ClientConfig.SetDefaults();
			msg = msg.Replace('\t', ' ');
			msg = msg.Replace("    ", " ");
			msg = msg.Replace("   ", " ");
			msg = msg.Replace("  ", " ");
			msg = msg.Replace('\r', '\n');
			msg = msg.Replace(':', ';');
			List<string> list = new List<string>();
			string[] array = msg.Split(new char[]
			{
				'\n'
			});
			for (int i = 0; i < array.Length; i++)
			{
				int num = array[i].IndexOf("//");
				bool flag = num >= 0;
				if (flag)
				{
					array[i] = array[i].Substring(0, num);
				}
				string[] array2 = array[i].Split(new char[]
				{
					';'
				});
				for (int j = 0; j < array2.Length; j++)
				{
					array2[j] = array2[j].Trim();
					array2[j] = array2[j].Replace("  ", " ");
					list.Add(array2[j]);
				}
			}
			foreach (string text in list)
			{
				bool flag2 = text.Length != 0;
				if (flag2)
				{
					bool flag3 = text.EndsWith("+") || text.EndsWith("-");
					if (flag3)
					{
						ClientConfig.ParseBoolean(text.Substring(0, text.Length - 1), text.EndsWith("+"));
					}
					else
					{
						bool flag4 = text.Contains("=");
						if (flag4)
						{
							string[] array3 = text.Replace(" ", "").Split(new char[]
							{
								'='
							});
							ClientConfig.ParseEquals(array3[0], array3[1]);
						}
						else
						{
							ClientConfig.ParseOperator(text.Replace(" ", ""));
						}
					}
				}
			}
			ClientConfig.EndConfig();
		}
	}

	// Token: 0x06000257 RID: 599 RVA: 0x0002602C File Offset: 0x0002422C
	private void SettingsHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			'#'
		});
		for (int i = 1; i < array.Length; i += 2)
		{
			string a = array[i];
			int num = 0;
			string text = array[i + 1];
			try
			{
				num = int.Parse(array[i + 1]);
			}
			catch (Exception)
			{
			}
			bool flag = a == "snd";
			if (flag)
			{
				bool flag2 = num == 1;
				if (flag2)
				{
					SoundManager.SoundOn = true;
				}
				else
				{
					SoundManager.SoundOn = false;
				}
				GUIManager.THIS.SetSound();
			}
			bool flag3 = a == "mus";
			if (flag3)
			{
				bool flag4 = num == 1;
				if (flag4)
				{
					SoundManager.MusicOn = true;
				}
				else
				{
					SoundManager.MusicOn = false;
				}
				GUIManager.THIS.SetMusic();
			}
			bool flag5 = a == "mof";
			if (flag5)
			{
				bool flag6 = num == 0;
				if (flag6)
				{
					ClientController.ownSounds = true;
				}
				else
				{
					ClientController.ownSounds = false;
				}
			}
			bool flag7 = a == "pot";
			if (flag7)
			{
				bool flag8 = num == 0;
				if (flag8)
				{
					this.terrainRenderer.ChangeQualityFor(0);
				}
				else
				{
					this.terrainRenderer.ChangeQualityFor(2);
				}
			}
			bool flag9 = a == "frc";
			if (flag9)
			{
				bool flag10 = num == 0;
				if (flag10)
				{
					TerrainRendererScript.alwaysUpdate = false;
				}
				else
				{
					TerrainRendererScript.alwaysUpdate = true;
				}
			}
			bool flag11 = a == "ctrl";
			if (flag11)
			{
				bool flag12 = num == 0;
				if (flag12)
				{
					ClientController.CtrlToggle = false;
				}
				else
				{
					ClientController.CtrlToggle = true;
				}
			}
			bool flag13 = a == "tsca";
			if (flag13)
			{
				bool flag14 = num == 1;
				if (flag14)
				{
					TerrainRendererScript.unitSize = 24f;
					TerrainRendererScript.needUpdate = true;
					this.terrainRenderer.RecreateMeshes();
				}
				else
				{
					TerrainRendererScript.unitSize = 16f;
					TerrainRendererScript.needUpdate = true;
					this.terrainRenderer.RecreateMeshes();
				}
			}
			bool flag15 = a == "isca";
			if (flag15)
			{
				bool flag16 = num == 1;
				if (flag16)
				{
					this.MainCanvasScaler.scaleFactor = 1.4f;
				}
				else
				{
					this.MainCanvasScaler.scaleFactor = 1f;
				}
			}
			bool flag17 = a == "mous";
			if (flag17)
			{
				bool flag18 = num == 0;
				if (flag18)
				{
					ClientController.MouseControl = false;
				}
				else
				{
					ClientController.MouseControl = true;
				}
			}
		}
	}

	// Token: 0x06000258 RID: 600 RVA: 0x000262A8 File Offset: 0x000244A8
	private void MPHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		int exp = (int)short.Parse(array[0]);
		int max = (int)short.Parse(array[1]);
		MissionPad.THIS.UpdateMissionProgress(exp, max);
	}

	// Token: 0x06000259 RID: 601 RVA: 0x000262E8 File Offset: 0x000244E8
	private void MNHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			'#'
		});
		string text = array[0];
		int dx = (int)short.Parse(array[1]);
		int dy = (int)short.Parse(array[2]);
		int anchorType = (int)short.Parse(array[3]);
		string hideReason = array[4];
		TutorialNavigation.THIS.SetNavi(text, dx, dy, anchorType, hideReason);
	}

	// Token: 0x0600025A RID: 602 RVA: 0x00026340 File Offset: 0x00024540
	public void OpenURLHandler(ref string msg)
	{
		bool flag = Application.platform == RuntimePlatform.WebGLPlayer;
		if (flag)
		{
			Application.ExternalEval("window.open(\"" + msg + "\",\"_blank\")");
		}
		else
		{
			Application.OpenURL(msg);
		}
	}

	// Token: 0x0600025B RID: 603 RVA: 0x00026380 File Offset: 0x00024580
	private void PanelHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			'#'
		});
		string text = array[0];
		string text2 = array[1];
		string s = array[2];
		bool flag = text2 == "";
		if (flag)
		{
			bool flag2 = text == "CLEAR";
			if (flag2)
			{
				GUIManager.THIS.statePanel.RemoveAll();
			}
			else
			{
				GUIManager.THIS.statePanel.RemoveLine(text);
			}
		}
		else
		{
			int num = (int)short.Parse(s);
			string[] text3 = text2.Split(new char[]
			{
				'~'
			});
			bool flag3 = num == 0;
			if (flag3)
			{
				GUIManager.THIS.statePanel.AddLine(text, text3, true, new Color(0.5f, 0.25f, 0.25f));
			}
			else
			{
				bool flag4 = num == 1;
				if (flag4)
				{
					GUIManager.THIS.statePanel.AddLine(text, text3, false, new Color(0.15f, 0.4f, 0.15f));
				}
				else
				{
					bool flag5 = num == 2;
					if (flag5)
					{
						GUIManager.THIS.statePanel.AddLine(text, text3, false, new Color(0.15f, 0.15f, 0.6f));
					}
				}
			}
		}
	}

	// Token: 0x0600025C RID: 604 RVA: 0x0000662D File Offset: 0x0000482D
	private void SuHandler(ref string msg)
	{
		GUIManager.THIS.banHammer.gameObject.SetActive(msg == "1");
	}

	// Token: 0x0600025D RID: 605 RVA: 0x00006651 File Offset: 0x00004851
	private void AgrHandler(ref string msg)
	{
		GUIManager.THIS.agrShow.SetActive(msg == "1");
	}

	// Token: 0x0600025E RID: 606 RVA: 0x00006670 File Offset: 0x00004870
	private void AutoDiggHandler(ref string msg)
	{
		ClientController.autoDigg = (msg == "1");
		ClientController.THIS.ShowAutoDigg();
	}

	// Token: 0x0600025F RID: 607 RVA: 0x0000668F File Offset: 0x0000488F
	private void ClanHideHandler(ref string msg)
	{
		GUIManager.THIS.HideClanIcon();
	}

	// Token: 0x06000260 RID: 608 RVA: 0x0000669D File Offset: 0x0000489D
	private void ClanShowHandler(ref string msg)
	{
		GUIManager.THIS.ShowClanIcon(int.Parse(msg));
	}

	// Token: 0x06000261 RID: 609 RVA: 0x000264BC File Offset: 0x000246BC
	private void BadCellsHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		for (int i = 0; i < array.Length; i += 3)
		{
			ClientController.THIS.AddFX(int.Parse(array[i]), int.Parse(array[i + 1]), 0);
		}
	}

	// Token: 0x06000262 RID: 610 RVA: 0x00026510 File Offset: 0x00024710
	private void InventoryHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		string a = array[0];
		bool flag = a == "show";
		if (flag)
		{
			InventoryPanel.THIS.ShowInventory(array[3], int.Parse(array[2]), int.Parse(array[1]));
		}
		else
		{
			bool flag2 = a == "full";
			if (flag2)
			{
				InventoryPanel.THIS.ShowFullGrid(array[2], int.Parse(array[1]));
			}
			else
			{
				bool flag3 = !(a == "choose");
				if (flag3)
				{
					bool flag4 = a == "close";
					if (flag4)
					{
						GUIManager.THIS.CloseInventoryItem();
					}
				}
				else
				{
					string hint = array[1];
					int d = int.Parse(array[2]);
					int dx = int.Parse(array[3]);
					int dy = int.Parse(array[4]);
					int num = int.Parse(array[5]);
					int h = int.Parse(array[6]);
					string mapStr = array[7];
					GUIManager.THIS.ChooseInventoryItem(0, 0, hint);
					bool flag5 = num > 0;
					if (flag5)
					{
						GUIManager.THIS.ShowInventoryGrid(d, dx, dy, num, h, mapStr);
					}
					else
					{
						OverlayRenderer.THIS.HideGrid();
					}
				}
			}
		}
	}

	// Token: 0x06000263 RID: 611 RVA: 0x00026644 File Offset: 0x00024844
	private void SpeedHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		int num = int.Parse(array[0]);
		int num2 = int.Parse(array[1]);
		int num3 = int.Parse(array[2]);
		ClientController.THIS.XY_PAUSE = (float)num * 0.001f;
		ClientController.THIS.ROAD_PAUSE = (float)num2 * 0.001f;
		ClientController.THIS.DEPTH = (float)num3;
	}

	// Token: 0x06000264 RID: 612 RVA: 0x000266B0 File Offset: 0x000248B0
	private void OKHandler(ref string msg)
	{
		int num = msg.IndexOf('#');
		bool flag = num != -1;
		if (flag)
		{
			OKMessage msg2 = default(OKMessage);
			msg2.title = msg.Substring(0, num);
			msg2.message = msg.Substring(num + 1);
			OKWindowManager.THIS.AddMessage(msg2);
		}
	}

	// Token: 0x06000265 RID: 613 RVA: 0x0002670C File Offset: 0x0002490C
	private void ProgrammatorOpenHandler(ref string msg)
	{
		ProgrammWrapper programmWrapper = JsonUtility.FromJson<ProgrammWrapper>(msg);
		GUIManager.THIS.OpenProgramm(programmWrapper.id, programmWrapper.title, programmWrapper.source);
	}

	// Token: 0x06000266 RID: 614 RVA: 0x00026740 File Offset: 0x00024940
	private void ProgrammatorUpdateHandler(ref string msg)
	{
		ProgrammWrapper programmWrapper = JsonUtility.FromJson<ProgrammWrapper>(msg);
		GUIManager.THIS.UpdateProgramm(programmWrapper.id, programmWrapper.title, programmWrapper.source);
	}

	// Token: 0x06000267 RID: 615 RVA: 0x00026774 File Offset: 0x00024974
	private void ProgrammatorHandler(ref string msg)
	{
		bool flag = msg == "1";
		if (flag)
		{
			GUIManager.THIS.ChangeProgTo(true);
			this.robotRenderer.isProgrammator = true;
			this.clientController.isProgrammator = true;
			ProgPanel.playing = true;
			this.clientController.stopAutoMove();
		}
		else
		{
			GUIManager.THIS.ChangeProgTo(false);
			this.robotRenderer.isProgrammator = false;
			this.clientController.isProgrammator = false;
			this.clientController.TimeSync();
			ProgPanel.playing = false;
		}
	}

	// Token: 0x06000268 RID: 616 RVA: 0x00004F6F File Offset: 0x0000316F
	private void PurchaseHandler(ref string msg)
	{
	}

	// Token: 0x06000269 RID: 617 RVA: 0x00026804 File Offset: 0x00024A04
	private void MoneyHandler(ref string msg)
	{
		MoneyWrapper moneyWrapper = JsonUtility.FromJson<MoneyWrapper>(msg);
		GUIManager.THIS.SetMoney(moneyWrapper.money, moneyWrapper.creds);
	}

	// Token: 0x0600026A RID: 618 RVA: 0x000066B2 File Offset: 0x000048B2
	private void LevelHandler(ref string msg)
	{
		Debug.Log(msg);
		GUIManager.THIS.SetLevel(int.Parse(msg));
	}

	// Token: 0x0600026B RID: 619 RVA: 0x000066CF File Offset: 0x000048CF
	private void ModulesHandler(ref string msg)
	{
		JsonUtility.FromJson<ModsWrapper>(msg);
	}

	// Token: 0x0600026C RID: 620 RVA: 0x00026834 File Offset: 0x00024A34
	private void PopupHandler(ref string msg)
	{
		bool debug = ConnectionManager.THIS.DEBUG;
		if (debug)
		{
			this.unsafePopupHandler(ref msg);
		}
		else
		{
			try
			{
				this.unsafePopupHandler(ref msg);
			}
			catch (Exception)
			{
			}
		}
	}

	// Token: 0x0600026D RID: 621 RVA: 0x00026880 File Offset: 0x00024A80
	private void unsafePopupHandler(ref string msg)
	{
		Debug.Log("PopupHandler " + msg);
		string a = msg.Substring(0, msg.IndexOf(':'));
		bool flag = a == "horb";
		if (flag)
		{
			HORBConfig cfg = JsonUtility.FromJson<HORBConfig>(msg.Substring(msg.IndexOf(':') + 1));
			PopupManager.THIS.ShowHORB(cfg);
		}
		else
		{
			bool flag2 = a == "up";
			if (flag2)
			{
				UPConfig upconfig = default(UPConfig);
				SkillsWrapper skillsWrapper = JsonUtility.FromJson<SkillsWrapper>(msg.Substring(msg.IndexOf(':') + 1));
				upconfig.admin = skillsWrapper.admin;
				upconfig.tabs = skillsWrapper.tabs;
				upconfig.skills = new SkillConfig[skillsWrapper.s];
				for (int i = 0; i < upconfig.skills.Length; i++)
				{
					upconfig.skills[i].type = -1;
				}
				string[] array = skillsWrapper.k.Split(new char[]
				{
					'#'
				});
				for (int j = 0; j < array.Length - 1; j++)
				{
					bool flag3 = !(array[j] == "");
					if (flag3)
					{
						string[] array2 = array[j].Split(new char[]
						{
							':'
						});
						int num = int.Parse(array2[2]);
						bool flag4 = num < skillsWrapper.s;
						if (flag4)
						{
							upconfig.skills[num].type = SkillButtonScript.skillShorts[array2[0]];
							upconfig.skills[num].level = int.Parse(array2[1]);
							upconfig.skills[num].isUp = (array2[3] != "0");
							upconfig.skills[num].isLocked = (array2[3] == "2");
						}
					}
				}
				upconfig.title = skillsWrapper.title;
				upconfig.text = skillsWrapper.txt;
				upconfig.button = skillsWrapper.b;
				upconfig.buttonAction = skillsWrapper.ba;
				bool flag5 = SkillButtonScript.skillShorts.ContainsKey(skillsWrapper.si);
				if (flag5)
				{
					upconfig.skillIcon = SkillButtonScript.skillShorts[skillsWrapper.si];
				}
				else
				{
					upconfig.skillIcon = -1;
				}
				upconfig.toInstall = null;
				bool flag6 = skillsWrapper.i.Length > 0;
				if (flag6)
				{
					upconfig.toInstall = skillsWrapper.i.Split(new char[]
					{
						':'
					});
				}
				upconfig.slot = skillsWrapper.sl;
				upconfig.canDelete = (skillsWrapper.del > 0);
				upconfig.lockState = ((skillsWrapper.del == 2) ? 1 : 0);
				PopupManager.THIS.ShowUP(upconfig);
			}
		}
	}

	// Token: 0x0600026E RID: 622 RVA: 0x000066DA File Offset: 0x000048DA
	private void PopupCloseHandler(ref string msg)
	{
		this.clientController.stopAutoMove();
		PopupManager.THIS.CloseWindow();
	}

	// Token: 0x0600026F RID: 623 RVA: 0x00026B88 File Offset: 0x00024D88
	private void OnlineHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		GUIManager.THIS.SetOnline(array[0], array[1]);
	}

	// Token: 0x06000270 RID: 624 RVA: 0x00026BBC File Offset: 0x00024DBC
	private void BasketHandler(ref string msg)
	{
		bool flag = ClientController.ownSounds && ClientConfig.SOUND_BASKET;
		if (flag)
		{
			SoundManager.THIS.PlaySound(0, 1f);
		}
		string[] array = msg.Split(new char[]
		{
			':'
		});
		GUIManager.THIS.SetBasket(long.Parse(array[0]), long.Parse(array[1]), long.Parse(array[2]), long.Parse(array[3]), long.Parse(array[4]), long.Parse(array[5]), long.Parse(array[6]));
	}

	// Token: 0x06000271 RID: 625 RVA: 0x00026C48 File Offset: 0x00024E48
	private void NickListHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			','
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				':'
			});
			this.robotRenderer.AddNick(int.Parse(array2[0]), array2[1]);
		}
	}

	// Token: 0x06000272 RID: 626 RVA: 0x00026CA8 File Offset: 0x00024EA8
	private void SkillHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			'#'
		});
		for (int i = 0; i < array.Length - 1; i++)
		{
			string[] array2 = array[i].Split(new char[]
			{
				':'
			});
			string code = array2[0];
			int progress = int.Parse(array2[1]);
			MiniSkillManager.THIS.AddIcon(progress, code);
		}
	}

	// Token: 0x06000273 RID: 627 RVA: 0x00026D14 File Offset: 0x00024F14
	private void GeoHandler(ref string msg)
	{
		GUIManager.THIS.GeoTF.text = " " + msg + " ";
		bool flag = msg == " ";
		if (flag)
		{
			GUIManager.THIS.GeoTF.gameObject.SetActive(false);
		}
		else
		{
			GUIManager.THIS.GeoTF.gameObject.SetActive(true);
		}
	}

	// Token: 0x06000274 RID: 628 RVA: 0x00026D84 File Offset: 0x00024F84
	private void LiveHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		this.clientController.Tremor();
		GUIManager.THIS.SetHP(int.Parse(array[0]), int.Parse(array[1]));
	}

	// Token: 0x06000275 RID: 629 RVA: 0x00026DCC File Offset: 0x00024FCC
	private void SmoothTPHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		this.clientController.SmoothTPMyBot(int.Parse(array[0]), int.Parse(array[1]));
	}

	// Token: 0x06000276 RID: 630 RVA: 0x00026E0C File Offset: 0x0002500C
	private void TPHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		this.clientController.TPMyBot(int.Parse(array[0]), int.Parse(array[1]));
	}

	// Token: 0x06000277 RID: 631 RVA: 0x00026E4C File Offset: 0x0002504C
	private void BotInfoHandler(ref string msg)
	{
		BotInfo botInfo = JsonUtility.FromJson<BotInfo>(msg);
		this.robotRenderer.Init();
		this.clientController.InitMyBot(botInfo.x, botInfo.y, botInfo.id, botInfo.name);
	}

	// Token: 0x06000278 RID: 632 RVA: 0x00026E94 File Offset: 0x00025094
	private void HubTranslator(ref byte[] buffer)
	{
		int i = 0;
		int num = 0;
		while (i < buffer.Length)
		{
			num++;
			char c = Convert.ToChar(buffer[i]);
			i++;
			bool flag = c <= 'O';
			if (flag)
			{
				switch (c)
				{
				case 'B':
				{
					int num2 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i));
					i += 2;
					int[] array = new int[num2];
					for (int j = 0; j < num2; j++)
					{
						array[j] = Convert.ToInt32(BitConverter.ToUInt16(buffer, i));
						i += 2;
					}
					bool inited = RobotRenderer.inited;
					if (inited)
					{
						this.robotRenderer.CheckAliveBots(num2, array);
						continue;
					}
					continue;
				}
				case 'C':
				{
					int bid = Convert.ToInt32(BitConverter.ToUInt16(buffer, i));
					int x = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 2));
					int y = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 4));
					int num3 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 6));
					i += 8;
					string @string = Encoding.UTF8.GetString(buffer, i, num3);
					i += num3;
					LocalChatMessages.THIS.AddLocalMessage(bid, x, y, @string);
					continue;
				}
				case 'D':
				{
					int fx = Convert.ToInt32(buffer[i]);
					int dir = Convert.ToInt32(buffer[i + 1]);
					int col = Convert.ToInt32(buffer[i + 2]);
					int x2 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 3));
					int y2 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 5));
					int bid2 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 7));
					i += 9;
					bool inited2 = ClientController.inited;
					if (inited2)
					{
						this.clientController.AddDirectedFX(bid2, x2, y2, fx, dir, col);
						continue;
					}
					continue;
				}
				case 'E':
					break;
				case 'F':
				{
					int fx2 = Convert.ToInt32(buffer[i]);
					int x3 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 1));
					int y3 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 3));
					i += 5;
					bool inited3 = ClientController.inited;
					if (inited3)
					{
						this.clientController.AddFX(x3, y3, fx2);
						continue;
					}
					continue;
				}
				default:
					switch (c)
					{
					case 'L':
					{
						int id = Convert.ToInt32(BitConverter.ToUInt16(buffer, i));
						i += 2;
						bool inited4 = RobotRenderer.inited;
						if (inited4)
						{
							this.robotRenderer.RemoveBot(id);
							continue;
						}
						continue;
					}
					case 'M':
					{
						int num4 = Convert.ToInt32(buffer[i]);
						int num5 = Convert.ToInt32(buffer[i + 1]);
						int num6 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 2));
						int num7 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 4));
						i += 6;
						for (int k = 0; k < num5; k++)
						{
							for (int l = 0; l < num4; l++)
							{
								byte cell = buffer[i];
								i++;
								bool inited5 = TerrainRendererScript.inited;
								if (inited5)
								{
									TerrainRendererScript.map.SetCell(num6 + l, num7 + k, cell);
								}
							}
						}
						TerrainRendererScript.needUpdate = true;
						continue;
					}
					case 'O':
					{
						int blockId = Convert.ToInt32(BitConverter.ToInt32(buffer, i));
						int num8 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 4));
						i += 6;
						PackRenderer.THIS.RemoveObjectInBlock(blockId);
						for (int m = 0; m < num8; m++)
						{
							char type = Convert.ToChar(buffer[i]);
							int x4 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 1));
							int y4 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 3));
							int cid = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 5));
							int off = Convert.ToInt32(buffer[i + 7]);
							i += 8;
							ObjectModel objectModel = new ObjectModel();
							objectModel.x = x4;
							objectModel.y = y4;
							objectModel.cid = cid;
							objectModel.off = off;
							objectModel.type = type;
							PackRenderer.THIS.AddObject(objectModel, blockId);
						}
						continue;
					}
					}
					break;
				}
			}
			else
			{
				bool flag2 = c != 'S';
				if (flag2)
				{
					bool flag3 = c != 'X';
					if (flag3)
					{
						bool flag4 = c == 'Z';
						if (flag4)
						{
							int num9 = Convert.ToInt32(buffer[i]);
							int col2 = Convert.ToInt32(buffer[i + 1]);
							int x5 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 2));
							int y5 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 4));
							i += 6;
							int[] array2 = new int[num9];
							for (int n = 0; n < num9; n++)
							{
								array2[n] = Convert.ToInt32(BitConverter.ToUInt16(buffer, i));
								i += 2;
								bool inited6 = ClientController.inited;
								if (inited6)
								{
									this.clientController.AddDirectedFX(array2[n], x5, y5, -1, 0, col2);
								}
							}
							continue;
						}
					}
					else
					{
						int dir2 = Convert.ToInt32(buffer[i]);
						int skin = Convert.ToInt32(buffer[i + 1]);
						int tail = Convert.ToInt32(buffer[i + 2]);
						int id2 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 3));
						int x6 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 5));
						int y6 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 7));
						int cid2 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i + 9));
						i += 11;
						bool inited7 = RobotRenderer.inited;
						if (inited7)
						{
							this.robotRenderer.XYBot(id2, x6, y6, dir2, cid2, skin, tail);
							continue;
						}
						continue;
					}
				}
				else
				{
					int id3 = Convert.ToInt32(BitConverter.ToUInt16(buffer, i));
					int block = Convert.ToInt32(BitConverter.ToInt32(buffer, i + 2));
					i += 6;
					bool inited8 = RobotRenderer.inited;
					if (inited8)
					{
						this.robotRenderer.RemoveBotFromBlock(id3, block);
						continue;
					}
					continue;
				}
			}
			throw new Exception("Corrupted HB type - " + c.ToString());
		}
	}

	// Token: 0x06000279 RID: 633 RVA: 0x000066F4 File Offset: 0x000048F4
	private void AutoRemHandler(ref string msg)
	{
		GUIManager.THIS.autoRemShow.SetActive(msg != "0");
		GUIManager.THIS.autoRemShow.GetComponentInChildren<Text>().text = "[B] " + msg;
	}

	// Token: 0x0600027A RID: 634 RVA: 0x00006734 File Offset: 0x00004934
	private void HandModeHandler(ref string msg)
	{
		Debug.Log("BH >" + msg);
		ProgPanel.handMode = (msg == "1");
	}

	// Token: 0x0600027B RID: 635 RVA: 0x000274A0 File Offset: 0x000256A0
	private void UMPHandler(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			'#'
		});
		string url = array[0];
		int imgx = (int)short.Parse(array[1]);
		int imgy = (int)short.Parse(array[2]);
		int progress = (int)short.Parse(array[3]);
		string text = array[4];
		MissionPad.THIS.UpdateMissionPanel(url, imgx, imgy, progress, text);
	}

	// Token: 0x0600027C RID: 636 RVA: 0x00004F6F File Offset: 0x0000316F
	private void Update()
	{
	}

	// Token: 0x0400046A RID: 1130
	private Obvyazka obvyazka;

	// Token: 0x0400046B RID: 1131
	public GameObject mainRenderer;

	// Token: 0x0400046C RID: 1132
	public GameObject clientControllerObject;

	// Token: 0x0400046D RID: 1133
	private ClientController clientController;

	// Token: 0x0400046E RID: 1134
	private RobotRenderer robotRenderer;

	// Token: 0x0400046F RID: 1135
	private TerrainRendererScript terrainRenderer;

	// Token: 0x04000470 RID: 1136
	private byte[] _typeBuffer;

	// Token: 0x04000471 RID: 1137
	public CanvasScaler MainCanvasScaler;

	// Token: 0x04000472 RID: 1138
	public static ServerController THIS;

	// Token: 0x04000473 RID: 1139
	public static string onlineString;
}
