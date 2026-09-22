using System;
using MyUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x0200001E RID: 30
public class GUIManager : MonoBehaviour
{
	// Token: 0x060000D2 RID: 210 RVA: 0x000052BC File Offset: 0x000034BC
	public void DailyRewardToggle(bool showReward)
	{
		if (showReward)
		{
			this.blinkDonate = true;
			return;
		}
		this.blinkDonate = false;
		this.DonatePlus.gameObject.SetActive(true);
	}

	// Token: 0x060000D3 RID: 211 RVA: 0x00014458 File Offset: 0x00012658
	public void ShowInventoryGrid(int d, int dx, int dy, int w, int h, string mapStr)
	{
		char[] array = mapStr.ToCharArray();
		int[] array2 = new int[w * h];
		for (int i = 0; i < w * h; i++)
		{
			if (array[i] == '0')
			{
				array2[i] = 0;
			}
			else
			{
				array2[i] = 1;
			}
		}
		OverlayRenderer.THIS.AddGrid(w, h, array2, dx, dy, d);
	}

	// Token: 0x060000D4 RID: 212 RVA: 0x000052E1 File Offset: 0x000034E1
	public void ChooseInventoryItem(int item, int num, string hint)
	{
		this.inventoryItem = item;
		this.InventoryHint.text = hint;
	}

	// Token: 0x060000D5 RID: 213 RVA: 0x000052F6 File Offset: 0x000034F6
	public void HideClanIcon()
	{
		this.clanButton.gameObject.SetActive(false);
	}

	// Token: 0x060000D6 RID: 214 RVA: 0x00005309 File Offset: 0x00003509
	public void ShowClanIcon(int id)
	{
		if (id == 0)
		{
			this.HideClanIcon();
			return;
		}
		this.clanButton.gameObject.SetActive(true);
		this.clanIcon.sprite = ClanSpriteScript.sprites[id - 1];
	}

	// Token: 0x060000D7 RID: 215 RVA: 0x0000533A File Offset: 0x0000353A
	public void CloseInventoryItem()
	{
		this.inventoryItem = -1;
		this.InventoryHint.text = "";
		OverlayRenderer.THIS.HideGrid();
	}

	// Token: 0x060000D8 RID: 216 RVA: 0x00004B5F File Offset: 0x00002D5F
	public void ChangeProgTo(bool state)
	{
	}

	// Token: 0x060000D9 RID: 217 RVA: 0x000144AC File Offset: 0x000126AC
	private void Start()
	{
		this.ChangeProgTo(false);
		this.connectionManager = this.network.GetComponent<ConnectionManager>();
		this.connectionManager.onStatusChanged.AddListener(new UnityAction<string>(this.updateConnectionTF));
		if (GUIManager.THIS != null)
		{
			throw new Exception("Singletone!");
		}
		GUIManager.THIS = this;
		//this.myCoolWindow = base.gameObject.AddComponent<MyCoolWindow>();
		this.m_EventSystem = EventSystem.current;
		this.m_EventSystem.sendNavigationEvents = false;
		this.fontLoader.gameObject.SetActive(false);
		this.agrShow.gameObject.SetActive(false);
		this.autoRemShow.gameObject.SetActive(false);
		this.progOpenButton.onClick.AddListener(new UnityAction(this.OnProgButton));
		this.returnButton.onClick.AddListener(new UnityAction(this.OnReturnButton));
		this.soundButton.onClick.AddListener(new UnityAction(this.OnSound));
		this.musicButton.onClick.AddListener(new UnityAction(this.OnMusic));
		this.mapButton.onClick.AddListener(new UnityAction(this.OnMap));
		this.settingsButton.onClick.AddListener(new UnityAction(this.OnSettings));
		this.DonateButton.onClick.AddListener(new UnityAction(this.OnDonate));
		this.InventoryHint.text = "";
		this.dropboxButton.onClick.AddListener(new UnityAction(this.OnDropbox));
		this.ConsoleButton.onClick.AddListener(new UnityAction(this.OnConsole));
		this.HelpButton.onClick.AddListener(new UnityAction(this.OnHelp));
		this.clanButton.onClick.AddListener(new UnityAction(this.OpenClan));
		this.buildingsButton.onClick.AddListener(new UnityAction(this.OpenBuildings));
	}

	// Token: 0x060000DA RID: 218 RVA: 0x0000535D File Offset: 0x0000355D
	private void OnConsole()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_CONS");
		ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Locl", 0, 0, "console");
	}

	// Token: 0x060000DB RID: 219 RVA: 0x00005390 File Offset: 0x00003590
	private void OnHelp()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_HELP");
		ServerTime.THIS.SendTypicalMessage(-1, "Help", 0, 0, "_");
	}

	// Token: 0x060000DC RID: 220 RVA: 0x000053BA File Offset: 0x000035BA
	private void OpenBuildings()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_BLDS");
		ServerTime.THIS.SendTypicalMessage(-1, "Blds", 0, 0, "_");
	}

	// Token: 0x060000DD RID: 221 RVA: 0x000053E4 File Offset: 0x000035E4
	private void OnSettings()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_SETT");
		ServerTime.THIS.SendTypicalMessage(-1, "Sett", 0, 0, "_");
	}

	// Token: 0x060000DE RID: 222 RVA: 0x0000540E File Offset: 0x0000360E
	private void OpenClan()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_CLAN");
		ServerTime.THIS.SendTypicalMessage(-1, "Clan", 0, 0, "_");
	}

	// Token: 0x060000DF RID: 223 RVA: 0x00005438 File Offset: 0x00003638
	private void OnDropbox()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_DROP");
		ServerTime.THIS.SendTypicalMessage(-1, "DPBX", 0, 0, "_");
	}

	// Token: 0x060000E0 RID: 224 RVA: 0x00005462 File Offset: 0x00003662
	private void OnInventory()
	{
		if (this.inventoryItem == -1)
		{
			ServerTime.THIS.SendTypicalMessage(-1, "INVN", 0, 0, "_");
			return;
		}
		ServerTime.THIS.SendTypicalMessage(-1, "INCL", 0, 0, "_");
	}

	// Token: 0x060000E1 RID: 225 RVA: 0x0000549E File Offset: 0x0000369E
	private void OnDonate()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_DONATE");
		ServerTime.THIS.SendTypicalMessage(-1, "GDon", 0, 0, ConnectionManager.METHOD);
	}

	// Token: 0x060000E2 RID: 226 RVA: 0x000054C8 File Offset: 0x000036C8
	private void OnMap()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_MAP");
		MapViewer.THIS.Show();
	}

	// Token: 0x060000E3 RID: 227 RVA: 0x000054E4 File Offset: 0x000036E4
	public void SetMusic()
	{
		if (SoundManager.MusicOn)
		{
			this.musicOff.gameObject.SetActive(false);
		}
		else
		{
			this.musicOff.gameObject.SetActive(true);
		}
		SoundManager.THIS.UpdateMusic();
	}

	// Token: 0x060000E4 RID: 228 RVA: 0x0000551B File Offset: 0x0000371B
	public void SetSound()
	{
		if (SoundManager.SoundOn)
		{
			this.soundOff.gameObject.SetActive(false);
			return;
		}
		this.soundOff.gameObject.SetActive(true);
	}

	// Token: 0x060000E5 RID: 229 RVA: 0x000146C8 File Offset: 0x000128C8
	private void OnMusic()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_Music");
		SoundManager.MusicOn = !SoundManager.MusicOn;
		this.SetMusic();
		ServerTime.THIS.SendTypicalMessage(-1, "Sett", 0, 0, "mus:" + (SoundManager.MusicOn ? "1" : "0"));
	}

	// Token: 0x060000E6 RID: 230 RVA: 0x00014728 File Offset: 0x00012928
	private void OnSound()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_SOUND");
		SoundManager.SoundOn = !SoundManager.SoundOn;
		this.SetSound();
		ServerTime.THIS.SendTypicalMessage(-1, "Sett", 0, 0, "snd:" + (SoundManager.SoundOn ? "1" : "0"));
	}

	// Token: 0x060000E7 RID: 231 RVA: 0x00014788 File Offset: 0x00012988
	private void OnReturnButton()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_RESP");
		AYSWindowManager.THIS.Show("ВОЗВРАЩЕНИЕ НА РЕСП", "Вы собираетесь вернуться на респаун.\nВы потеряете 10% груза, остальной груз упакуется в бокс.\nУверены?", delegate
		{
			ServerTime.THIS.SendTypicalMessage(-1, "RESP", 0, 0, "_");
		});
	}

	// Token: 0x060000E8 RID: 232 RVA: 0x00005547 File Offset: 0x00003747
	public void OnProgCloseButton()
	{
		ClientController.CanGoto = false;
		ServerTime.THIS.SendTypicalMessage(-1, "pRST", 0, 0, "_");
	}

	// Token: 0x060000E9 RID: 233 RVA: 0x000147D8 File Offset: 0x000129D8
	private void OnProgButton()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_PROG");
		if (!this.programmator.activeSelf)
		{
			ServerTime.THIS.SendTypicalMessage(-1, "pRST", 0, 0, "_");
			if (!ProgrammatorView.opened)
			{
				ServerTime.THIS.SendTypicalMessage(-1, "Pope", 0, 0, GUIManager.programToSend);
				return;
			}
			this.programmator.SetActive(true);
			ProgrammatorView.active = true;
			ProgrammatorView.THIS.Show();
		}
	}

	// Token: 0x060000EA RID: 234 RVA: 0x00014858 File Offset: 0x00012A58
	public void UpdateProgramm(int id, string title, string source)
	{
		if (id == -1)
		{
			this.programmatorTitlePanel.text = title;
			this.programmatorTitle.text = title;
			GUIManager.programToSend = source;
			return;
		}
		this.programmator.SetActive(true);
		ProgrammatorView.active = true;
		ProgrammatorView.programId = id;
		ProgrammatorView.title = title;
		this.programmatorTitlePanel.text = title;
		this.programmatorTitle.text = title;
		GUIManager.programToSend = "_";
		if (source.Length > 0)
		{
			ProgrammatorView.THIS.LoadFromString(source);
		}
		else
		{
			ProgrammatorView.THIS.ClearSource();
			ProgrammatorView.THIS.UpdateIconsWithoutSaving();
		}
		ProgrammatorView.THIS.Show();
		this.programmator.SetActive(false);
		ProgrammatorView.active = false;
	}

	// Token: 0x060000EB RID: 235 RVA: 0x00014910 File Offset: 0x00012B10
	public void OpenProgramm(int id, string title, string source)
	{
		if (id == -1)
		{
			this.programmatorTitlePanel.text = title;
			this.programmatorTitle.text = title;
			GUIManager.programToSend = source;
			return;
		}
		this.programmator.SetActive(true);
		ProgrammatorView.active = true;
		ProgrammatorView.programId = id;
		ProgrammatorView.title = title;
		this.programmatorTitlePanel.text = title;
		this.programmatorTitle.text = title;
		GUIManager.programToSend = "_";
		if (source.Length > 0)
		{
			ProgrammatorView.THIS.LoadFromString(source);
			Debug.Log("DA");
		}
		else
		{
			ProgrammatorView.THIS.ClearSource();
			ProgrammatorView.THIS.UpdateIconsWithoutSaving();
			Debug.Log("Net");
		}
		ProgrammatorView.THIS.Show();
	}

	// Token: 0x060000EC RID: 236 RVA: 0x00004B5F File Offset: 0x00002D5F
	public void ChatHandler(ref string msg)
	{
	}

	// Token: 0x060000ED RID: 237 RVA: 0x00005567 File Offset: 0x00003767
	public void ClearFocus()
	{
		this.m_EventSystem.SetSelectedGameObject(null);
	}

	// Token: 0x060000EE RID: 238 RVA: 0x000149C8 File Offset: 0x00012BC8
	private void OnLocalChatButton()
	{
		ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Locl", 0, 0, this.localChatInput.text);
		this.localChatInput.text = "";
		this.ClearFocus();
		this.localChatInput.gameObject.SetActive(false);
	}

	// Token: 0x060000EF RID: 239 RVA: 0x00005575 File Offset: 0x00003775
	private void OnChatButton()
	{
		ChatManager.THIS.SendChat();
		ChatManager.THIS.UpdateFocus();
	}

	// Token: 0x060000F0 RID: 240 RVA: 0x0000558B File Offset: 0x0000378B
	public void SetCoord(int x, int y)
	{
		this.CoordTF.text = x.ToString() + ":" + y.ToString();
	}

	// Token: 0x060000F1 RID: 241 RVA: 0x000055B0 File Offset: 0x000037B0
	public void SetMoney(long money, long creds)
	{
		this.MoneyTF.text = " $" + money.ToString("### ### ### ##0");
		this.CredTF.text = creds.ToString("### ### ### ##0");
	}

	// Token: 0x060000F2 RID: 242 RVA: 0x000055EA File Offset: 0x000037EA
	public void SetCreds(int creds)
	{
		this.CredTF.text = creds.ToString("### ### ### ##0");
	}

	// Token: 0x060000F3 RID: 243 RVA: 0x00005603 File Offset: 0x00003803
	public void SetOnline(string online, string onprog)
	{
		this.OnlineTF.text = string.Concat(new string[]
		{
			"ОНЛАЙН ",
			online,
			" <color=yellow>(",
			onprog,
			")</color>"
		});
	}

	// Token: 0x060000F4 RID: 244 RVA: 0x00014A24 File Offset: 0x00012C24
	public void SetMods(string[] mods)
	{
		this.modsTF.text = "Модули:\n";
		for (int i = 0; i < mods.Length; i++)
		{
			Text text = this.modsTF;
			text.text = text.text + mods[i] + "\n";
		}
	}

	// Token: 0x060000F5 RID: 245 RVA: 0x0000563B File Offset: 0x0000383B
	public void SetLevel(int level)
	{
		this.LevelTF.text = " ур." + level;
	}

	// Token: 0x060000F6 RID: 246 RVA: 0x00014A70 File Offset: 0x00012C70
	public void SetHP(int hp, int hpmax)
	{
		this.HPTF.text = hp + "HP";
		float num = (float)hp / (float)hpmax;
		this.HPTF.color = new Color(Mathf.Sqrt(Mathf.Sqrt(1f - num)), Mathf.Sqrt(Mathf.Sqrt(num)), num * (1f - num));
		this.HpLine.color = new Color(Mathf.Sqrt(Mathf.Sqrt(1f - num)), Mathf.Sqrt(Mathf.Sqrt(num)), num * (1f - num), 0.7f);
		Vector2 sizeDelta = this.HpLine.rectTransform.sizeDelta;
		sizeDelta.x = this.HpWrapper.rectTransform.sizeDelta.x * num;
		this.HpLine.rectTransform.sizeDelta = sizeDelta;
	}

	// Token: 0x060000F7 RID: 247 RVA: 0x00014B50 File Offset: 0x00012D50
	private void MakeAlpha(Text TF, float alpha)
	{
		Color color = TF.color;
		color.a = alpha;
		TF.color = color;
	}

	// Token: 0x060000F8 RID: 248 RVA: 0x00014B74 File Offset: 0x00012D74
	private void SetBasketCrys(GameObject TF, long crys)
	{
		Text componentInChildren = TF.GetComponentInChildren<Text>();
		if (crys < 1000L)
		{
			componentInChildren.text = " " + crys.ToString("##0");
		}
		else if (crys < 1000000L)
		{
			componentInChildren.text = " " + crys.ToString("### ##0");
		}
		else if (crys < 10000000000L)
		{
			crys = (long)Mathf.FloorToInt((float)crys / 1000f);
			componentInChildren.text = " " + crys.ToString("### ##0") + " K";
		}
		else
		{
			crys = (long)Mathf.FloorToInt((float)crys / 1000000f);
			componentInChildren.text = " " + crys.ToString("### ##0") + " KK";
		}
		if (crys == 0L)
		{
			this.MakeAlpha(componentInChildren, 0.25f);
			TF.SetActive(false);
			return;
		}
		this.MakeAlpha(componentInChildren, 1f);
		TF.SetActive(true);
	}

	// Token: 0x060000F9 RID: 249 RVA: 0x00014C74 File Offset: 0x00012E74
	public void SetBasket(long g, long b, long r, long v, long w, long c, long capacity)
	{
		this.SetBasketCrys(this.BasketGTF, g);
		this.SetBasketCrys(this.BasketBTF, b);
		this.SetBasketCrys(this.BasketRTF, r);
		this.SetBasketCrys(this.BasketVTF, v);
		this.SetBasketCrys(this.BasketWTF, w);
		this.SetBasketCrys(this.BasketCTF, c);
		if (g + b + r + v + w + c == 0L)
		{
			this.BasketPanel.SetActive(false);
		}
		else
		{
			this.BasketPanel.SetActive(true);
		}
		float num = (float)capacity / 100f;
		if (num > 1f)
		{
			num = 1f;
		}
		float num2 = (float)(capacity - 100L) / 100f;
		if (num2 > 1f)
		{
			num2 = 1f;
		}
		string str;
		if (capacity < 10L)
		{
			str = " Груз  <color=#777>" + capacity.ToString() + "%";
		}
		else if (capacity < 50L)
		{
			str = " Груз <color=#777>" + capacity.ToString() + "%";
		}
		else if (capacity < 100L)
		{
			str = " Груз <color=#7f7>" + capacity.ToString() + "%";
		}
		else if (capacity < 115L)
		{
			str = "Груз <color=#ff5>" + capacity.ToString() + "%";
		}
		else if (capacity < 200L)
		{
			str = "Груз <color=#f75>" + capacity.ToString() + "%";
		}
		else if (capacity < 1000L)
		{
			str = "Груз <color=#f55>" + capacity.ToString() + "%";
		}
		else if (capacity < 2000L)
		{
			str = " Груз <color=#f57>1k%";
		}
		else if (capacity < 3000L)
		{
			str = " Груз <color=#f5f>2k%";
		}
		else
		{
			str = "Груз <color=#5ff>3k+%";
		}
		this.CapacityTF.text = str + "</color>";
		if (capacity < 50L)
		{
			this.CapacityBar.color = new Color(0.75f, 0.75f, 0.75f);
			this.overload = false;
			Vector2 sizeDelta = this.CapacityBar.rectTransform.sizeDelta;
			sizeDelta.x = 95f * num;
			this.CapacityBar.rectTransform.sizeDelta = sizeDelta;
			sizeDelta = this.CapacityOverloadBar.rectTransform.sizeDelta;
			sizeDelta.x = 0f;
			this.CapacityOverloadBar.rectTransform.sizeDelta = sizeDelta;
			return;
		}
		if (capacity < 100L)
		{
			this.CapacityBar.color = new Color(0.25f, 0.99f, 0.25f);
			this.overload = false;
			Vector2 sizeDelta2 = this.CapacityBar.rectTransform.sizeDelta;
			sizeDelta2.x = 95f * num;
			this.CapacityBar.rectTransform.sizeDelta = sizeDelta2;
			sizeDelta2 = this.CapacityOverloadBar.rectTransform.sizeDelta;
			sizeDelta2.x = 0f;
			this.CapacityOverloadBar.rectTransform.sizeDelta = sizeDelta2;
			return;
		}
		if (capacity < 115L)
		{
			this.CapacityBar.color = new Color(0.95f, 0.95f, 0.25f);
			this.overload = true;
			Vector2 sizeDelta3 = this.CapacityBar.rectTransform.sizeDelta;
			sizeDelta3.x = 95f;
			this.CapacityBar.rectTransform.sizeDelta = sizeDelta3;
			this.CapacityOverloadBar.color = new Color(0.7f, 0.5f, 0.15f);
			sizeDelta3 = this.CapacityOverloadBar.rectTransform.sizeDelta;
			sizeDelta3.x = 95f * num2;
			this.CapacityOverloadBar.rectTransform.sizeDelta = sizeDelta3;
			return;
		}
		if (capacity < 200L)
		{
			this.CapacityBar.color = new Color(0.99f, 0.65f, 0.25f);
			this.overload = true;
			Vector2 sizeDelta4 = this.CapacityBar.rectTransform.sizeDelta;
			sizeDelta4.x = 95f;
			this.CapacityBar.rectTransform.sizeDelta = sizeDelta4;
			this.CapacityOverloadBar.color = new Color(0.7f, 0f, 0.15f);
			sizeDelta4 = this.CapacityOverloadBar.rectTransform.sizeDelta;
			sizeDelta4.x = 95f * num2;
			this.CapacityOverloadBar.rectTransform.sizeDelta = sizeDelta4;
			return;
		}
		this.CapacityBar.color = new Color(0.9f, 0.35f, 0.35f);
		this.overload = true;
		Vector2 sizeDelta5 = this.CapacityBar.rectTransform.sizeDelta;
		sizeDelta5.x = 95f;
		this.CapacityBar.rectTransform.sizeDelta = sizeDelta5;
		this.CapacityOverloadBar.color = new Color(0.7f, 0f, 0.15f);
		sizeDelta5 = this.CapacityOverloadBar.rectTransform.sizeDelta;
		sizeDelta5.x = 95f * num2;
		this.CapacityOverloadBar.rectTransform.sizeDelta = sizeDelta5;
	}

	// Token: 0x060000FA RID: 250 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void updateConnectionTF(string msg)
	{
	}

	// Token: 0x060000FB RID: 251 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void њљјїјїїјїљљїњњїљјљљњјјњ()
	{
	}

	// Token: 0x060000FC RID: 252 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void њјљјњјљљљїњљњїјїјњњїїїљ()
	{
	}

	// Token: 0x060000FD RID: 253 RVA: 0x00005658 File Offset: 0x00003858
	public bool GlobalChatIsActive()
	{
		return this.m_EventSystem.currentSelectedGameObject != null && this.m_EventSystem.currentSelectedGameObject.name == "ChatField";
	}

	// Token: 0x060000FE RID: 254 RVA: 0x00015170 File Offset: 0x00013370
	private void Update()
	{
		if (this.blinkDonate)
		{
			this.DonatePlus.gameObject.SetActive(Mathf.Sin(15f * Time.time) > 0f);
		}
		if (this.overload)
		{
			Color color = this.CapacityOverloadBar.color;
			color.a = 0.7f + 0.3f * Mathf.Sin(19f * Time.time);
			this.CapacityOverloadBar.color = color;
		}
		if (Input.GetMouseButtonDown(0))
		{
			if ((this.m_EventSystem.currentSelectedGameObject != null && this.m_EventSystem.currentSelectedGameObject.name == "LocalChat") || this.localChatInput.gameObject.activeSelf)
			{
				ClientController.CanGoto = false;
				this.localChatInput.text = "";
				this.ClearFocus();
				this.localChatInput.gameObject.SetActive(false);
			}
			if (this.fullscreenButton != null)
			{
				Vector3 position = this.fullscreenButton.transform.position;
				if (Vector3.Distance(Input.mousePosition, position) < 23f)
				{
					ClientController.CanGoto = false;
					Screen.fullScreen = !Screen.fullScreen;
					TerrainRendererScript.needUpdate = true;
					TutorialNavigation.CheckHide("_FULL");
				}
			}
		}
		if (!ProgrammatorView.active && Input.GetKeyDown(ClientConfig.LOCALCHAT_KEY) && (this.m_EventSystem.currentSelectedGameObject == null || (this.m_EventSystem.currentSelectedGameObject.name != "LocalChat" && this.m_EventSystem.currentSelectedGameObject.name != "InputField" && this.m_EventSystem.currentSelectedGameObject.name != "ChatField")))
		{
			this.localChatInput.gameObject.SetActive(true);
			this.m_EventSystem.SetSelectedGameObject(this.localChatInput.gameObject, null);
		}
		if (Input.GetKeyDown(KeyCode.Tab))
		{
			ChatManager.THIS.OnToggle();
		}
		if (Input.GetKeyDown(KeyCode.Escape))
		{
			if (this.inventoryItem != -1)
			{
				ServerTime.THIS.SendTypicalMessage(-1, "INCL", 0, 0, "-1");
			}
			if (this.m_EventSystem.currentSelectedGameObject != null && this.m_EventSystem.currentSelectedGameObject.name == "LocalChat")
			{
				this.localChatInput.text = "";
				this.ClearFocus();
				this.localChatInput.gameObject.SetActive(false);
			}
		}
		if ((Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)) && !ConnectionManager.disconnected && this.m_EventSystem.currentSelectedGameObject != null)
		{
			if (this.m_EventSystem.currentSelectedGameObject.name == "ChatField")
			{
				this.OnChatButton();
			}
			else if (this.m_EventSystem.currentSelectedGameObject.name == "LocalChat")
			{
				this.OnLocalChatButton();
			}
		}
		if (this.m_EventSystem.currentSelectedGameObject == null || (this.m_EventSystem.currentSelectedGameObject.name != "LocalChat" && this.m_EventSystem.currentSelectedGameObject.name != "InputField" && this.m_EventSystem.currentSelectedGameObject.name != "ChatField"))
		{
			if (Input.GetKeyDown(ClientConfig.TOGGLE0_KEY))
			{
				ClientConfig.Toggle(0);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE1_KEY))
			{
				ClientConfig.Toggle(1);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE2_KEY))
			{
				ClientConfig.Toggle(2);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE3_KEY))
			{
				ClientConfig.Toggle(3);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE4_KEY))
			{
				ClientConfig.Toggle(4);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE5_KEY))
			{
				ClientConfig.Toggle(5);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE6_KEY))
			{
				ClientConfig.Toggle(6);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE7_KEY))
			{
				ClientConfig.Toggle(7);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE8_KEY))
			{
				ClientConfig.Toggle(8);
			}
			if (Input.GetKeyDown(ClientConfig.TOGGLE9_KEY))
			{
				ClientConfig.Toggle(9);
			}
		}
	}

	// Token: 0x06000101 RID: 257 RVA: 0x000056A4 File Offset: 0x000038A4
	public void InitExternalPanel()
	{
		if (this.externalPanel == null)
		{
			this.externalPanel = base.gameObject.AddComponent<ExternalGUIPanel>();
			this.externalPanel.Show();
		}
	}

	// Token: 0x06000102 RID: 258 RVA: 0x000056D0 File Offset: 0x000038D0
	public void AddBot(int ID, string nick)
	{
		if (!(this.externalPanel == null))
		{
			this.externalPanel.AddBot(ID, nick);
		}
	}

	// Token: 0x06000103 RID: 259 RVA: 0x000056ED File Offset: 0x000038ED
	public void RemoveBot(int ID)
	{
		if (!(this.externalPanel == null))
		{
			this.externalPanel.RemoveBot(ID);
		}
	}

	// Token: 0x06000104 RID: 260 RVA: 0x00005709 File Offset: 0x00003909
	public bool GetCheckerStateExternalPanel()
	{
		return this.externalPanel.GetCheckerState();
	}

	// Token: 0x04000189 RID: 393
	public GameObject network;

	// Token: 0x0400018A RID: 394
	public Text statusTF;

	// Token: 0x0400018B RID: 395
	public Button fullscreenButton;

	// Token: 0x0400018C RID: 396
	public Button returnButton;

	// Token: 0x0400018D RID: 397
	public Button mapButton;

	// Token: 0x0400018E RID: 398
	public Button buildingsButton;

	// Token: 0x0400018F RID: 399
	public Text InventoryHint;

	// Token: 0x04000190 RID: 400
	public Button inventoryButton;

	// Token: 0x04000191 RID: 401
	public GameObject currentItemImage;

	// Token: 0x04000192 RID: 402
	public int inventoryItem = -1;

	// Token: 0x04000193 RID: 403
	public Button banHammer;

	// Token: 0x04000194 RID: 404
	public Button dropboxButton;

	// Token: 0x04000195 RID: 405
	public Button settingsButton;

	// Token: 0x04000196 RID: 406
	public Button progOpenButton;

	// Token: 0x04000197 RID: 407
	public Button clanButton;

	// Token: 0x04000198 RID: 408
	public Image clanIcon;

	// Token: 0x04000199 RID: 409
	public Button soundButton;

	// Token: 0x0400019A RID: 410
	public Image soundOff;

	// Token: 0x0400019B RID: 411
	public Button musicButton;

	// Token: 0x0400019C RID: 412
	public Image musicOff;

	// Token: 0x0400019D RID: 413
	public GameObject programmator;

	// Token: 0x0400019E RID: 414
	public Text programmatorTitle;

	// Token: 0x0400019F RID: 415
	public Text programmatorTitlePanel;

	// Token: 0x040001A0 RID: 416
	public GameObject agrShow;

	// Token: 0x040001A1 RID: 417
	public GameObject autoRemShow;

	// Token: 0x040001A2 RID: 418
	public Text GeoTF;

	// Token: 0x040001A3 RID: 419
	public Text HPTF;

	// Token: 0x040001A4 RID: 420
	public Text LevelTF;

	// Token: 0x040001A5 RID: 421
	public RawImage HpWrapper;

	// Token: 0x040001A6 RID: 422
	public RawImage HpLine;

	// Token: 0x040001A7 RID: 423
	public GameObject BasketPanel;

	// Token: 0x040001A8 RID: 424
	public GameObject BasketGTF;

	// Token: 0x040001A9 RID: 425
	public GameObject BasketBTF;

	// Token: 0x040001AA RID: 426
	public GameObject BasketRTF;

	// Token: 0x040001AB RID: 427
	public GameObject BasketVTF;

	// Token: 0x040001AC RID: 428
	public GameObject BasketWTF;

	// Token: 0x040001AD RID: 429
	public GameObject BasketCTF;

	// Token: 0x040001AE RID: 430
	public Text CapacityTF;

	// Token: 0x040001AF RID: 431
	public RawImage CapacityBar;

	// Token: 0x040001B0 RID: 432
	public RawImage CapacityOverloadBar;

	// Token: 0x040001B1 RID: 433
	public Text MoneyTF;

	// Token: 0x040001B2 RID: 434
	public Text CredTF;

	// Token: 0x040001B3 RID: 435
	public InputField CoordTF;

	// Token: 0x040001B4 RID: 436
	public Text OnlineTF;

	// Token: 0x040001B5 RID: 437
	public Button DonateButton;

	// Token: 0x040001B6 RID: 438
	public Button ConsoleButton;

	// Token: 0x040001B7 RID: 439
	public Button HelpButton;

	// Token: 0x040001B8 RID: 440
	public StatePanel statePanel;

	// Token: 0x040001B9 RID: 441
	public Image DonatePlus;

	// Token: 0x040001BA RID: 442
	public GameObject AccountPanel;

	// Token: 0x040001BB RID: 443
	public GameObject PayloadPanel;

	// Token: 0x040001BC RID: 444
	private bool blinkDonate;

	// Token: 0x040001BD RID: 445
	public static string programToSend = "_";

	// Token: 0x040001BE RID: 446
	private ConnectionManager connectionManager;

	// Token: 0x040001BF RID: 447
	public MyInputField localChatInput;

	// Token: 0x040001C0 RID: 448
	public Text chatTF;

	// Token: 0x040001C1 RID: 449
	public Text modsTF;

	// Token: 0x040001C2 RID: 450
	public Image RightPanel;

	// Token: 0x040001C3 RID: 451
	public GameObject fontLoader;

	// Token: 0x040001C4 RID: 452
	public EventSystem m_EventSystem;

	// Token: 0x040001C5 RID: 453
	public static GUIManager THIS;

	// Token: 0x040001C6 RID: 454
	private bool overload;

	// Token: 0x040001C7 RID: 455
	private ExternalGUIPanel externalPanel;
}
