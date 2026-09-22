using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200000A RID: 10
public class ChatLineInfo : MonoBehaviour
{
	// Token: 0x06000039 RID: 57 RVA: 0x0000BD98 File Offset: 0x00009F98
	public void SetMessage(GCMessage message)
	{
		if (this.img == null)
		{
			this.img = base.gameObject.GetComponentInChildren<Image>();
		}
		this.msg = message;
		string text = "";
		if (ClientConfig.CHAT_SHOW_TIME && message.gid > 0)
		{
			long ticks = (long)message.time * 60000L * 10000L;
			DateTime dateTime = new DateTime(ticks);
			text += dateTime.ToLocalTime().ToString("HH:mm");
			if (ClientConfig.CHAT_SHOW_NICK || ClientConfig.CHAT_SHOW_ID)
			{
				text += " ";
			}
		}
		if (ClientConfig.CHAT_SHOW_ID && message.gid > 0)
		{
			text = string.Concat(new object[]
			{
				text,
				"%",
				message.gid,
				"%"
			});
			if (ClientConfig.CHAT_SHOW_NICK)
			{
				text += " ";
			}
		}
		if (ClientConfig.CHAT_SHOW_NICK)
		{
			text += message.nick;
		}
		if (message.cid != 0)
		{
			this.img.gameObject.SetActive(true);
			this.img.sprite = ClanSpriteScript.sprites[message.cid - 1];
			base.gameObject.GetComponent<Text>().text = "     " + text + ": " + message.text;
		}
		else
		{
			this.img.gameObject.SetActive(false);
			base.gameObject.GetComponent<Text>().text = " " + text + ": " + message.text;
		}
		if (message.gid <= 0)
		{
			base.gameObject.GetComponent<Text>().fontSize = 10;
		}
	}

	// Token: 0x04000047 RID: 71
	public GCMessage msg;

	// Token: 0x04000048 RID: 72
	private Image img;
}
