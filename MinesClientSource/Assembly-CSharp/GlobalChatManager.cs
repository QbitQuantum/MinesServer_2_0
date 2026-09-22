using System;
using MyUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000021 RID: 33
public class GlobalChatManager : MonoBehaviour
{
	// Token: 0x06000108 RID: 264 RVA: 0x00015594 File Offset: 0x00013794
	private GlobalChatLineModel DefaultLineModel()
	{
		return new GlobalChatLineModel
		{
			cid = 0,
			id = 0,
			name = "",
			text = "",
			color = 0
		};
	}

	// Token: 0x06000109 RID: 265 RVA: 0x000155DC File Offset: 0x000137DC
	private void Start()
	{
		GlobalChatManager.THIS = this;
		this.chatModeButton.onClick.AddListener(new UnityAction(this.OnModeButton));
		this.chatColors = new uint[]
		{
			16121843U,
			13390926U,
			9211599U,
			15061507U,
			3201644U,
			10071813U,
			7198161U,
			14973696U,
			15721871U,
			14124754U,
			5090557U,
			13421772U,
			16681832U,
			4100608U,
			9400432U
		};
		this.chatInput.gameObject.name = "ChatField";
		this.colorPickButton.onClick.AddListener(new UnityAction(this.OnColorButton));
		this.ChangeChatColor();
		this.chatColor = 2;
		this.chatLines = new GameObject[this.ALL_MESSAGES];
		this.fed_models = new GlobalChatLineModel[this.ALL_MESSAGES];
		this.tor_models = new GlobalChatLineModel[this.ALL_MESSAGES];
		this.dno_models = new GlobalChatLineModel[this.ALL_MESSAGES];
		this.models = this.fed_models;
		for (int i = 0; i < this.ALL_MESSAGES; i++)
		{
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.chatLinePrefab);
			this.chatLines[i] = gameObject;
			gameObject.transform.SetParent(this.chatLinesContainer.transform);
			int num = i;
			gameObject.GetComponentInChildren<Button>().onClick.AddListener(delegate()
			{
				this.CommonNickListener(num);
			});
			this.fed_models[i] = this.DefaultLineModel();
			this.tor_models[i] = this.DefaultLineModel();
			this.dno_models[i] = this.DefaultLineModel();
		}
		this.UpdateView();
		this.OnModeButton();
		this.fedButton.onClick.AddListener(new UnityAction(this.Fedbt));
		this.torButton.onClick.AddListener(new UnityAction(this.Torbt));
		this.dnoButton.onClick.AddListener(new UnityAction(this.Dnobt));
	}

	// Token: 0x0600010A RID: 266 RVA: 0x0000573C File Offset: 0x0000393C
	private void Fedbt()
	{
		this.ChangeChannelTo(0, "FED");
	}

	// Token: 0x0600010B RID: 267 RVA: 0x0000574A File Offset: 0x0000394A
	private void Torbt()
	{
		this.ChangeChannelTo(1, "TOR");
	}

	// Token: 0x0600010C RID: 268 RVA: 0x00005758 File Offset: 0x00003958
	private void Dnobt()
	{
		this.ChangeChannelTo(2, "DNO");
	}

	// Token: 0x0600010D RID: 269 RVA: 0x000157BC File Offset: 0x000139BC
	public void ChangeChannelTo(int num, string name)
	{
		this.channel = name;
		Vector3 position = this.chatArrow.transform.position;
		position.y = 120f - 20f * (float)num;
		this.chatArrow.transform.position = position;
		switch (num)
		{
		case 0:
			this.models = this.fed_models;
			break;
		case 1:
			this.models = this.tor_models;
			break;
		case 2:
			this.models = this.dno_models;
			break;
		}
		this.UpdateView();
	}

	// Token: 0x0600010E RID: 270 RVA: 0x0001584C File Offset: 0x00013A4C
	public void ChatHandler(ref string msg)
	{
		if (msg.IndexOf('#') != -1)
		{
			string[] array = msg.Substring(0, msg.IndexOf('#')).Split(new char[]
			{
				':'
			});
			string text = msg.Substring(msg.IndexOf('#') + 1);
			string[] array2 = text.Split(new char[]
			{
				':'
			});
			array2[0] = text.Substring(0, text.IndexOf(':'));
			array2[1] = text.Substring(text.IndexOf(':') + 1);
			string a = array[3];
			GlobalChatLineModel[] array3;
			if (a == "FED")
			{
				array3 = this.fed_models;
			}
			else if (a == "DNO")
			{
				array3 = this.dno_models;
			}
			else
			{
				array3 = this.tor_models;
			}
			if (array3[this.ALL_MESSAGES - 1].id == int.Parse(array[2]) && array3[this.ALL_MESSAGES - 1].text.Length + array2[1].Length < 130)
			{
				string str = ", ";
				if ("!?.,;:*()[]{}".IndexOf(array3[this.ALL_MESSAGES - 1].text.Substring(array3[this.ALL_MESSAGES - 1].text.Length - 1)) != -1)
				{
					str = " ";
				}
				GlobalChatLineModel[] array4 = array3;
				int num = this.ALL_MESSAGES - 1;
				array4[num].text = array4[num].text + str + array2[1];
			}
			else
			{
				for (int i = 0; i < this.ALL_MESSAGES - 1; i++)
				{
					array3[i] = array3[i + 1];
				}
				array3[this.ALL_MESSAGES - 1].name = array2[0];
				array3[this.ALL_MESSAGES - 1].text = array2[1];
				array3[this.ALL_MESSAGES - 1].color = int.Parse(array[0]);
				array3[this.ALL_MESSAGES - 1].cid = int.Parse(array[1]);
				array3[this.ALL_MESSAGES - 1].id = int.Parse(array[2]);
			}
			if (array3 == this.models)
			{
				this.UpdateView();
			}
		}
	}

	// Token: 0x0600010F RID: 271 RVA: 0x00015AA0 File Offset: 0x00013CA0
	private void UpdateView()
	{
		for (int i = 0; i < this.ALL_MESSAGES; i++)
		{
			Text[] componentsInChildren = this.chatLines[i].GetComponentsInChildren<Text>();
			componentsInChildren[0].color = this.UIntToColor(this.chatColors[this.models[i].color]);
			componentsInChildren[0].text = " " + this.models[i].name + " : ";
			componentsInChildren[1].color = this.UIntToColor(this.chatColors[this.models[i].color]);
			componentsInChildren[1].text = this.models[i].text;
			Image componentInChildren = this.chatLines[i].GetComponentInChildren<Image>();
			if (this.models[i].cid == 0)
			{
				componentInChildren.sprite = this.emptySprite;
			}
			else
			{
				componentInChildren.sprite = ClanSpriteScript.sprites[this.models[i].cid - 1];
			}
		}
	}

	// Token: 0x06000110 RID: 272 RVA: 0x00015BAC File Offset: 0x00013DAC
	private Color UIntToColor(uint color)
	{
		byte b = (byte)(color >> 16);
		byte b2 = (byte)(color >> 8);
		byte b3 = (byte)color;
		return new Color
		{
			r = (float)b / 255f,
			g = (float)b2 / 255f,
			b = (float)b3 / 255f,
			a = 1f
		};
	}

	// Token: 0x06000111 RID: 273 RVA: 0x00015C0C File Offset: 0x00013E0C
	public void SendChat()
	{
		ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Chat", 0, 0, string.Concat(new object[]
		{
			this.chatColor,
			":",
			this.channel,
			"#",
			this.chatInput.text
		}));
		this.chatInput.text = "";
	}

	// Token: 0x06000112 RID: 274 RVA: 0x00005766 File Offset: 0x00003966
	private void OnColorButton()
	{
		this.chatColor++;
		this.chatColor %= this.chatColors.Length;
		this.ChangeChatColor();
	}

	// Token: 0x06000113 RID: 275 RVA: 0x00015C88 File Offset: 0x00013E88
	public void ChangeChatColor()
	{
		this.colorPickButton.image.color = this.UIntToColor(this.chatColors[this.chatColor]);
		this.chatInput.textComponent.color = this.UIntToColor(this.chatColors[this.chatColor]);
		this.chatInput.image.color = this.UIntToColor(this.chatColors[this.chatColor]);
	}

	// Token: 0x06000114 RID: 276 RVA: 0x00015D00 File Offset: 0x00013F00
	public void SetLinesNum(int num)
	{
		for (int i = 0; i < this.ALL_MESSAGES; i++)
		{
			this.chatLines[i].gameObject.SetActive(i > this.ALL_MESSAGES - 1 - num);
		}
	}

	// Token: 0x06000115 RID: 277 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void CommonNickListener(int јњљјїњјњњњїїјјљљњїњјљјј)
	{
	}

	// Token: 0x06000116 RID: 278 RVA: 0x00015D40 File Offset: 0x00013F40
	private void OnModeButton()
	{
		this.chatMode++;
		this.chatMode %= 3;
		if (this.chatMode == 0)
		{
			this.chatFull.gameObject.SetActive(false);
			this.chatNorm.gameObject.SetActive(true);
			this.chatMin.gameObject.SetActive(false);
			Vector2 sizeDelta = this.chatBack.rectTransform.sizeDelta;
			sizeDelta.y = 155f;
			this.chatBack.rectTransform.sizeDelta = sizeDelta;
			this.chatBack.gameObject.SetActive(true);
			this.SetLinesNum(11);
			return;
		}
		if (this.chatMode == 1)
		{
			this.chatFull.gameObject.SetActive(false);
			this.chatNorm.gameObject.SetActive(false);
			this.chatMin.gameObject.SetActive(true);
			this.chatBack.gameObject.SetActive(false);
			this.SetLinesNum(3);
			return;
		}
		this.chatFull.gameObject.SetActive(true);
		this.chatNorm.gameObject.SetActive(false);
		this.chatMin.gameObject.SetActive(false);
		this.chatBack.gameObject.SetActive(true);
		Vector2 sizeDelta2 = this.chatBack.rectTransform.sizeDelta;
		sizeDelta2.y = 600f;
		this.chatBack.rectTransform.sizeDelta = sizeDelta2;
		this.SetLinesNum(28);
	}

	// Token: 0x06000117 RID: 279 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x040001D0 RID: 464
	public static GlobalChatManager THIS;

	// Token: 0x040001D1 RID: 465
	public Button chatModeButton;

	// Token: 0x040001D2 RID: 466
	public Image chatFull;

	// Token: 0x040001D3 RID: 467
	public Image chatNorm;

	// Token: 0x040001D4 RID: 468
	public Image chatMin;

	// Token: 0x040001D5 RID: 469
	public Sprite emptySprite;

	// Token: 0x040001D6 RID: 470
	public Image chatBack;

	// Token: 0x040001D7 RID: 471
	public Image chatArrow;

	// Token: 0x040001D8 RID: 472
	public GameObject chatLinesContainer;

	// Token: 0x040001D9 RID: 473
	public GameObject[] chatLines;

	// Token: 0x040001DA RID: 474
	public GameObject chatLinePrefab;

	// Token: 0x040001DB RID: 475
	private GlobalChatLineModel[] fed_models;

	// Token: 0x040001DC RID: 476
	private GlobalChatLineModel[] tor_models;

	// Token: 0x040001DD RID: 477
	private GlobalChatLineModel[] dno_models;

	// Token: 0x040001DE RID: 478
	private GlobalChatLineModel[] models;

	// Token: 0x040001DF RID: 479
	public MyInputField chatInput;

	// Token: 0x040001E0 RID: 480
	public Button colorPickButton;

	// Token: 0x040001E1 RID: 481
	public int chatColor = 2;

	// Token: 0x040001E2 RID: 482
	public uint[] chatColors;

	// Token: 0x040001E3 RID: 483
	public Button fedButton;

	// Token: 0x040001E4 RID: 484
	public Button torButton;

	// Token: 0x040001E5 RID: 485
	public Button dnoButton;

	// Token: 0x040001E6 RID: 486
	private int chatMode;

	// Token: 0x040001E7 RID: 487
	private int ALL_MESSAGES = 28;

	// Token: 0x040001E8 RID: 488
	private string channel = "FED";
}
