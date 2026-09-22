using System;
using System.Security.Cryptography;
using System.Text;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using UnityEngine.UI;

// Token: 0x02000003 RID: 3
public class AuthManager : MonoBehaviour
{
	// Token: 0x06000014 RID: 20 RVA: 0x00009118 File Offset: 0x00007318
	private void Start()
	{
		this.connectionManager = base.gameObject.GetComponent<ConnectionManager>();
		this.obvyazka = base.gameObject.GetComponent<Obvyazka>();
		this.connectionManager.onConnect.AddListener(new UnityAction(this.OnConnected));
		this.obvyazka.OnU("AE", new TypedCallback<string>(this.OnAE), true);
		this.obvyazka.OnU("AU", new TypedCallback<string>(this.OnAU), false);
		this.ListenAH();
		this.vkPanel.gameObject.SetActive(false);
	}

	// Token: 0x06000015 RID: 21 RVA: 0x00004B31 File Offset: 0x00002D31
	private void OnNV(ref string msg)
	{
		if (int.Parse(msg) > 28)
		{
			ConnectionManager.phrase = "\n\nЭТА ВЕРСИЯ НЕ ПОДДЕРЖИВАЕТСЯ.\nСКАЧАЙТЕ НОВЫЙ КЛИЕНТ ИЛИ ОБНОВИТЕ СТРАНИЦУ";
			this.connectionManager.dontReconnect = true;
			this.connectionManager.ForceDisconnect();
		}
	}

	// Token: 0x06000016 RID: 22 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void OnConnected()
	{
	}

	// Token: 0x06000017 RID: 23 RVA: 0x00004B61 File Offset: 0x00002D61
	private void ListenAH()
	{
		this.obvyazka.OnU("AH", new TypedCallback<string>(this.OnAH), true);
	}

	// Token: 0x06000018 RID: 24 RVA: 0x00004B80 File Offset: 0x00002D80
	private void OnAE(ref string msg)
	{
		ConnectionManager.THIS.connectionText.text = "ПОДКЛЮЧЕНИЕ ОТМЕНЕНО:\n\n" + msg;
	}

	// Token: 0x06000019 RID: 25 RVA: 0x000091B4 File Offset: 0x000073B4
	private void OnAU(ref string msg)
	{
		this._uniq = msg;
		this._haveUniq = true;
		if (AuthManager.debugGid != "")
		{
			this.obvyazka.SendU("AU", this._uniq + "_DEBUG_" + AuthManager.debugGid);
			return;
		}
		if ((ConnectionManager.METHOD == "SITE" || WorldInitScript.inited) && PlayerPrefs.HasKey("user_id") && PlayerPrefs.HasKey("user_hash"))
		{
			this._userId = PlayerPrefs.GetString("user_id");
			this._userHash = PlayerPrefs.GetString("user_hash");
			this.obvyazka.SendU("AU", string.Concat(new string[]
			{
				this._uniq,
				"_",
				this._userId,
				"_",
				this.CalculateMD5Hash(this._userHash + this._uniq)
			}));
			return;
		}
		if (this.obvyazka.vk_access_token != "")
		{
			this.obvyazka.SendU("AU", this._uniq + "_VK_" + this.obvyazka.vk_access_token);
			this.obvyazka.OnU("AV", new TypedCallback<string>(this.ShowVKButton), true);
			return;
		}
		this.obvyazka.SendU("AU", this._uniq + "_NO_AUTH");
		string text = "";
		this.ShowVKButton(ref text);
	}

	// Token: 0x0600001A RID: 26 RVA: 0x00009344 File Offset: 0x00007544
	private void ShowVKButton(ref string msg)
	{
		EventTrigger eventTrigger = this.vkButton.gameObject.AddComponent<EventTrigger>();
		EventTrigger.Entry entry = new EventTrigger.Entry
		{
			eventID = EventTriggerType.PointerDown
		};
		entry.callback.AddListener(new UnityAction<BaseEventData>(this.VKShow));
		eventTrigger.triggers.Add(entry);
		this.vkPanel.gameObject.SetActive(true);
	}

	// Token: 0x0600001B RID: 27 RVA: 0x00004B9D File Offset: 0x00002D9D
	private void VKShow(BaseEventData e)
	{
		this.OnVKButtonClick();
	}

	// Token: 0x0600001C RID: 28 RVA: 0x00004BA5 File Offset: 0x00002DA5
	private void OnVKButtonClick()
	{
		if (this._haveUniq)
		{
			Application.OpenURL("https://noy");
		}
	}

	// Token: 0x0600001D RID: 29 RVA: 0x000093A4 File Offset: 0x000075A4
	private void OnAH(ref string msg)
	{
		if (msg == "BAD")
		{
			PlayerPrefs.DeleteKey("user_id");
			PlayerPrefs.DeleteKey("user_hash");
			this.vkPanel.gameObject.SetActive(true);
			base.Invoke("ListenAH", 0.01f);
			return;
		}
		string[] array = msg.Split(new char[]
		{
			'_'
		});
		this._userId = array[0];
		this._userHash = array[1];
		PlayerPrefs.SetString("user_id", this._userId);
		PlayerPrefs.SetString("user_hash", this._userHash);
		this.obvyazka.SendU("AU", string.Concat(new string[]
		{
			this._uniq,
			"_",
			this._userId,
			"_",
			this.CalculateMD5Hash(this._userHash + this._uniq)
		}));
		this.vkPanel.gameObject.SetActive(false);
	}

	// Token: 0x0600001E RID: 30 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x0600001F RID: 31 RVA: 0x000094A4 File Offset: 0x000076A4
	private string CalculateMD5Hash(string input)
	{
		HashAlgorithm hashAlgorithm = MD5.Create();
		byte[] bytes = Encoding.ASCII.GetBytes(input);
		byte[] array = hashAlgorithm.ComputeHash(bytes);
		StringBuilder stringBuilder = new StringBuilder();
		for (int i = 0; i < array.Length; i++)
		{
			stringBuilder.Append(array[i].ToString("x2"));
		}
		return stringBuilder.ToString();
	}

	// Token: 0x04000007 RID: 7
	public GameObject vkPanel;

	// Token: 0x04000008 RID: 8
	public Button vkButton;

	// Token: 0x04000009 RID: 9
	public static string debugGid = "";

	// Token: 0x0400000A RID: 10
	private ConnectionManager connectionManager;

	// Token: 0x0400000B RID: 11
	private Obvyazka obvyazka;

	// Token: 0x0400000C RID: 12
	private bool _haveUniq;

	// Token: 0x0400000D RID: 13
	private string _uniq;

	// Token: 0x0400000E RID: 14
	private string _userHash;

	// Token: 0x0400000F RID: 15
	private string _userId;
}
