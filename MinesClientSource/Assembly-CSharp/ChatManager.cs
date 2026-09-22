using System;
using System.Collections.Generic;
using MyUI;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200000B RID: 11
public class ChatManager : MonoBehaviour
{
	// Token: 0x0600003B RID: 59 RVA: 0x0000BF48 File Offset: 0x0000A148
	public string getLasts()
	{
		string text = "";
		bool flag = true;
		foreach (KeyValuePair<string, int> keyValuePair in this.LastIDs)
		{
			if (!flag)
			{
				text += "#";
			}
			flag = false;
			text = string.Concat(new object[]
			{
				text,
				keyValuePair.Key,
				"#",
				keyValuePair.Value
			});
		}
		return text;
	}

	// Token: 0x0600003C RID: 60 RVA: 0x00004CDA File Offset: 0x00002EDA
	public string getCurrentChat()
	{
		return this.currentChat;
	}

	// Token: 0x0600003D RID: 61 RVA: 0x0000BFE0 File Offset: 0x0000A1E0
	private void Start()
	{
		ChatManager.THIS = this;
		this.UpdateChatMode();
		this.ChatToggle.onClick.AddListener(new UnityAction(this.ctog));
		this.RightChatToggle.onClick.AddListener(new UnityAction(this.rctog));
		this.ChatsButton.onClick.AddListener(new UnityAction(this.OnMenu));
		this.ChatSettings.onClick.AddListener(new UnityAction(this.OnSettings));
	}

	// Token: 0x0600003E RID: 62 RVA: 0x00004CE2 File Offset: 0x00002EE2
	private void ctog()
	{
		this.OnToggle();
		ClientController.CanGoto = false;
	}

	// Token: 0x0600003F RID: 63 RVA: 0x00004CE2 File Offset: 0x00002EE2
	private void rctog()
	{
		this.OnToggle();
		ClientController.CanGoto = false;
	}

	// Token: 0x06000040 RID: 64 RVA: 0x00004CF0 File Offset: 0x00002EF0
	private void OnSettings()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_CHATSET");
		ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Cset", 0, 0, "_");
	}

	// Token: 0x06000041 RID: 65 RVA: 0x00004D23 File Offset: 0x00002F23
	private void OnMenu()
	{
		ClientController.CanGoto = false;
		TutorialNavigation.CheckHide("_CHATMENU");
		ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Cmen", 0, 0, "_");
	}

	// Token: 0x06000042 RID: 66 RVA: 0x00004D56 File Offset: 0x00002F56
	public void OnToggle()
	{
		TutorialNavigation.CheckHide("_CHATTOGGLE");
		this.chatmode++;
		if (this.chatmode == 3)
		{
			this.chatmode = 0;
		}
		this.UpdateChatMode();
		base.Invoke("UpdateFocus", 0.01f);
	}

	// Token: 0x06000043 RID: 67 RVA: 0x00004D96 File Offset: 0x00002F96
	public void UpdateFocus()
	{
		if (this.chatmode == 1)
		{
			GUIManager.THIS.m_EventSystem.SetSelectedGameObject(this.ChatInput.gameObject, null);
			return;
		}
		GUIManager.THIS.ClearFocus();
	}

	// Token: 0x06000044 RID: 68 RVA: 0x0000C06C File Offset: 0x0000A26C
	private void SetPanelMode(int mode)
	{
		if (mode == 0)
		{
			this.RightChatToggle.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
			this.ChatsButton.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
		}
		if (mode == 1)
		{
			this.RightChatToggle.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 0.5f);
			this.ChatsButton.gameObject.GetComponent<Image>().color = new Color(1f, 1f, 1f, 1f);
		}
	}

	// Token: 0x06000045 RID: 69 RVA: 0x00004DC7 File Offset: 0x00002FC7
	public void mnHandler(ref string msg)
	{
		if (short.Parse(msg) == 0)
		{
			this.Notification.SetActive(false);
			return;
		}
		this.Notification.SetActive(true);
		this.Notification.GetComponentInChildren<Text>().text = msg;
	}

	// Token: 0x06000046 RID: 70 RVA: 0x0000C138 File Offset: 0x0000A338
	public void mlHandler(ref string msg)
	{
		if (msg == "")
		{
			return;
		}
		this.ChatInput.gameObject.SetActive(false);
		this.TitleTF.text = "СПИСОК ЧАТОВ";
		foreach (object obj in this.ChatContainer.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		string[] array = msg.Split(new char[]
		{
			'#'
		});
		for (int i = 0; i < array.Length; i++)
		{
			string[] subs = array[i].Split(new char[]
			{
				'±'
			});
			string text = "#dddd88";
			if (subs[0].StartsWith("_"))
			{
				subs[3] = subs[3].Substring(subs[3].IndexOf(':') + 1);
				text = "#88aa88";
			}
			GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ChatMenuLinePrefab);
			gameObject.transform.SetParent(this.ChatContainer.transform, false);
			gameObject.GetComponent<Text>().text = ((subs[1] == "1") ? "  " : "");
			gameObject.GetComponentInChildren<Image>().gameObject.SetActive(subs[1] == "1");
			if (subs[2].Length > 20)
			{
				subs[2] = subs[2].Substring(0, 18) + "...";
			}
			Text component = gameObject.GetComponent<Text>();
			component.text = string.Concat(new string[]
			{
				component.text,
				"<color=",
				text,
				">",
				subs[2],
				"</color>"
			});
			if (subs[3].Length > 22)
			{
				subs[3] = subs[3].Substring(0, 20) + "...";
			}
			Text component2 = gameObject.GetComponent<Text>();
			component2.text = component2.text + "\n<color=#888888>" + subs[3] + "</color>";
			gameObject.GetComponent<Button>().onClick.AddListener(delegate()
			{
				string str = subs[0];
				ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Choo", 0, 0, str);
			});
		}
	}

	// Token: 0x06000047 RID: 71 RVA: 0x0000C3D8 File Offset: 0x0000A5D8
	public void moHandler(ref string msg)
	{
		this.ChatInput.gameObject.SetActive(true);
		int num = msg.IndexOf(':');
		string text = msg.Substring(0, num);
		string text2 = msg.Substring(num + 1);
		if (text.StartsWith("_"))
		{
			if (text2.Length > 20)
			{
				text2 = text2.Substring(0, 18) + "...";
			}
			this.TitleTF.text = "ЛС – " + text2;
		}
		else
		{
			this.TitleTF.text = text + " – " + text2;
		}
		this.currentChat = text;
		this.SetPanelMode(0);
		if (!this.History.ContainsKey(text))
		{
			this.History.Add(text, new List<GCMessage>());
			this.LastIDs.Add(text, -1);
		}
		foreach (object obj in this.ChatContainer.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		this.lastAdded = default(GCMessage);
		List<GCMessage> list = this.History[text];
		for (int i = 0; i < list.Count; i++)
		{
			GCMessage message = list[i];
			this.AddLine(message);
		}
		this.UpdateMini();
		this.DelayedForcedScrollDown();
	}

	// Token: 0x06000048 RID: 72 RVA: 0x0000C550 File Offset: 0x0000A750
	public void UpdateChatStyle()
	{
		foreach (object obj in this.ChatContainer.transform)
		{
			ChatLineInfo component = ((Transform)obj).gameObject.GetComponent<ChatLineInfo>();
			if (component != null)
			{
				component.SetMessage(component.msg);
			}
		}
	}

	// Token: 0x06000049 RID: 73 RVA: 0x0000C5C8 File Offset: 0x0000A7C8
	private void UpdateMini()
	{
		foreach (object obj in this.DownChatContainer.transform)
		{
			UnityEngine.Object.Destroy(((Transform)obj).gameObject);
		}
		for (int i = Math.Max(0, this.History[this.currentChat].Count - 3); i < this.History[this.currentChat].Count; i++)
		{
			this.AddMiniLine(this.History[this.currentChat][i]);
		}
	}

	// Token: 0x0600004A RID: 74 RVA: 0x0000C684 File Offset: 0x0000A884
	private void AddMiniLine(GCMessage message)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ChatLineMiniPrefab);
		gameObject.transform.SetParent(this.DownChatContainer.transform, false);
		if (message.cid != 0)
		{
			gameObject.GetComponentInChildren<Image>().sprite = ClanSpriteScript.sprites[message.cid - 1];
			gameObject.GetComponent<Text>().text = "      " + message.nick + ": " + message.text;
		}
		else
		{
			gameObject.GetComponentInChildren<Image>().gameObject.SetActive(false);
			gameObject.GetComponent<Text>().text = message.nick + ": " + message.text;
		}
		gameObject.GetComponent<Text>().color = this.colorFromCode(message.color, true);
	}

	// Token: 0x0600004B RID: 75 RVA: 0x0000C748 File Offset: 0x0000A948
	private Color colorFromCode(int code, bool mini = false)
	{
		if (code != 50)
		{
			return Color.HSVToRGB((float)code / 20f, 0.3f, (code % 2 == 0) ? 1f : 0.86f);
		}
		if (mini)
		{
			return new Color(1f, 1f, 1f, 1f);
		}
		return new Color(0.5f, 0.5f, 0.5f, 1f);
	}

	// Token: 0x0600004C RID: 76 RVA: 0x0000C7B4 File Offset: 0x0000A9B4
	private void AddLine(GCMessage message)
	{
		string str = ", ";
		string text = "!?.,;:*()[]{}";
		GCMessage gcmessage = this.lastAdded;
		this.lastAdded = message;
		if (gcmessage.id >= message.id)
		{
			return;
		}
		if (this.History[this.currentChat].Count > 0)
		{
			int childCount = this.ChatContainer.transform.childCount;
			int length = this.ChatContainer.transform.GetChild(childCount - 1).gameObject.GetComponent<Text>().text.Length;
			if (gcmessage.gid > 0 && gcmessage.gid == message.gid && length < 100 && gcmessage.text.Length > 0)
			{
				if (text.IndexOf(gcmessage.text.Substring(gcmessage.text.Length - 1)) != -1)
				{
					str = " ";
				}
				gcmessage.text = gcmessage.text + str + message.text;
				string text2 = this.ChatContainer.transform.GetChild(childCount - 1).gameObject.GetComponent<Text>().text;
				this.ChatContainer.transform.GetChild(childCount - 1).gameObject.GetComponent<Text>().text = text2 + str + message.text;
				return;
			}
		}
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.ChatLinePrefab);
		gameObject.transform.SetParent(this.ChatContainer.transform, false);
		gameObject.GetComponent<ChatLineInfo>().SetMessage(message);
		gameObject.GetComponent<Button>().onClick.AddListener(delegate()
		{
			int gid = message.gid;
			ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Cpri", 0, 0, gid.ToString());
		});
		gameObject.GetComponent<Text>().color = this.colorFromCode(message.color, false);
		if (this.ChatContainer.transform.childCount > 30)
		{
			int num = this.ChatContainer.transform.childCount - 30;
			for (int i = 0; i < num; i++)
			{
				UnityEngine.Object.Destroy(this.ChatContainer.transform.GetChild(i).gameObject);
			}
		}
	}

	// Token: 0x0600004D RID: 77 RVA: 0x0000C9F8 File Offset: 0x0000ABF8
	public void SendChat()
	{
		if (!this.ChatInput.gameObject.activeSelf)
		{
			return;
		}
		if (this.ChatInput.text != "")
		{
			ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Chat", 0, 0, this.ChatInput.text);
		}
		this.ChatInput.text = "";
	}

	// Token: 0x0600004E RID: 78 RVA: 0x0000CA68 File Offset: 0x0000AC68
	public void mcHandler(ref string msg)
	{
		int num = (int)short.Parse(msg);
		this.ChatInput.GetComponentInChildren<Text>().color = Color.HSVToRGB((float)num / 20f, 0.3f, (num % 2 == 0) ? 1f : 0.86f);
	}

	// Token: 0x0600004F RID: 79 RVA: 0x0000CAB0 File Offset: 0x0000ACB0
	public void muHandler(ref string msg)
	{
		ChatManager.MuPacket muPacket = JsonUtility.FromJson<ChatManager.MuPacket>(msg);
		for (int i = 0; i < muPacket.h.Length; i++)
		{
			string[] array = muPacket.h[i].Split(new char[]
			{
				'±'
			});
			if (array.Length == 7)
			{
				GCMessage gcmessage = default(GCMessage);
				gcmessage.id = int.Parse(array[0]);
				gcmessage.color = int.Parse(array[1]);
				gcmessage.cid = int.Parse(array[2]);
				gcmessage.time = int.Parse(array[3]);
				gcmessage.nick = array[4];
				gcmessage.text = array[5];
				gcmessage.gid = int.Parse(array[6]);
				gcmessage.realtxt = "";
				if (muPacket.ch == this.currentChat)
				{
					this.AddLine(gcmessage);
				}
				if (!this.History.ContainsKey(muPacket.ch))
				{
					this.History.Add(muPacket.ch, new List<GCMessage>());
					this.LastIDs.Add(muPacket.ch, -1);
				}
				if (gcmessage.id > this.LastIDs[muPacket.ch])
				{
					this.History[muPacket.ch].Add(gcmessage);
					this.LastIDs[muPacket.ch] = gcmessage.id;
				}
			}
		}
		while (this.History[muPacket.ch].Count > 50)
		{
			this.History[muPacket.ch].RemoveAt(0);
		}
		if (muPacket.ch == this.currentChat)
		{
			this.UpdateMini();
		}
		if (muPacket.ch == this.currentChat)
		{
			this.ScrollDown();
		}
	}

	// Token: 0x06000050 RID: 80 RVA: 0x00004DFD File Offset: 0x00002FFD
	private void DelayedForcedScrollDown()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.DownChatContainer.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.ChatScroll.GetComponent<RectTransform>());
		base.Invoke("ForcedScrollDown", 0.2f);
	}

	// Token: 0x06000051 RID: 81 RVA: 0x00004E2F File Offset: 0x0000302F
	public void ScrollDown()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.DownChatContainer.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.ChatScroll.GetComponent<RectTransform>());
		this.ScrollDown2(false);
	}

	// Token: 0x06000052 RID: 82 RVA: 0x00004E58 File Offset: 0x00003058
	private void ForcedScrollDown()
	{
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.DownChatContainer.GetComponent<RectTransform>());
		LayoutRebuilder.ForceRebuildLayoutImmediate(this.ChatScroll.GetComponent<RectTransform>());
		this.ChatScroll.verticalNormalizedPosition = 0f;
	}

	// Token: 0x06000053 RID: 83 RVA: 0x00004E8A File Offset: 0x0000308A
	private void ScrollDown2(bool force = false)
	{
		if (force || this.ChatScroll.verticalNormalizedPosition < 0.2f)
		{
			this.ChatScroll.verticalNormalizedPosition = 0f;
		}
	}

	// Token: 0x06000054 RID: 84 RVA: 0x0000CC78 File Offset: 0x0000AE78
	private void UpdateChatMode()
	{
		switch (this.chatmode)
		{
		case 0:
			this.LeftGUI.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.ChatPanel.gameObject.SetActive(false);
			this.ChatToggleOff.gameObject.SetActive(false);
			this.ChatToggle.gameObject.SetActive(true);
			this.DownChatContainer.gameObject.SetActive(true);
			return;
		case 1:
			this.LeftGUI.transform.localPosition = new Vector3(213f, 0f, 0f);
			this.ChatPanel.gameObject.SetActive(true);
			this.ChatToggleOff.gameObject.SetActive(false);
			this.ChatToggle.gameObject.SetActive(false);
			this.DownChatContainer.gameObject.SetActive(false);
			this.ScrollDown();
			base.Invoke("ScrollDown", 0.1f);
			return;
		case 2:
			this.LeftGUI.transform.localPosition = new Vector3(0f, 0f, 0f);
			this.ChatPanel.gameObject.SetActive(false);
			this.ChatToggleOff.gameObject.SetActive(true);
			this.ChatToggle.gameObject.SetActive(true);
			this.DownChatContainer.gameObject.SetActive(false);
			this.ChatInput.text = "";
			return;
		default:
			return;
		}
	}

	// Token: 0x06000055 RID: 85 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000049 RID: 73
	public GameObject LeftGUI;

	// Token: 0x0400004A RID: 74
	public Button ChatToggle;

	// Token: 0x0400004B RID: 75
	public Image ChatToggleOff;

	// Token: 0x0400004C RID: 76
	public Button RightChatToggle;

	// Token: 0x0400004D RID: 77
	public Button ChatsButton;

	// Token: 0x0400004E RID: 78
	public Button ChatSettings;

	// Token: 0x0400004F RID: 79
	public GameObject ChatContainer;

	// Token: 0x04000050 RID: 80
	public GameObject ChatPanel;

	// Token: 0x04000051 RID: 81
	public GameObject Notification;

	// Token: 0x04000052 RID: 82
	public MyInputField ChatInput;

	// Token: 0x04000053 RID: 83
	public Text TitleTF;

	// Token: 0x04000054 RID: 84
	public GameObject ChatMenuLinePrefab;

	// Token: 0x04000055 RID: 85
	public GameObject ChatLinePrefab;

	// Token: 0x04000056 RID: 86
	public GameObject ChatLineMiniPrefab;

	// Token: 0x04000057 RID: 87
	public ScrollRect ChatScroll;

	// Token: 0x04000058 RID: 88
	public GameObject DownChatContainer;

	// Token: 0x04000059 RID: 89
	public static ChatManager THIS;

	// Token: 0x0400005A RID: 90
	private Dictionary<string, int> LastIDs = new Dictionary<string, int>();

	// Token: 0x0400005B RID: 91
	private Dictionary<string, List<GCMessage>> History = new Dictionary<string, List<GCMessage>>();

	// Token: 0x0400005C RID: 92
	private int chatmode;

	// Token: 0x0400005D RID: 93
	private string currentChat = "";

	// Token: 0x0400005E RID: 94
	private GCMessage lastAdded;

	// Token: 0x0200000C RID: 12
	public struct MuPacket
	{
		// Token: 0x0400005F RID: 95
		public string[] h;

		// Token: 0x04000060 RID: 96
		public string ch;
	}
}
