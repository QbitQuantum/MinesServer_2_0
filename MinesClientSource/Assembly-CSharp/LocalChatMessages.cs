using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200002B RID: 43
public class LocalChatMessages : MonoBehaviour
{
	// Token: 0x06000136 RID: 310 RVA: 0x00005906 File Offset: 0x00003B06
	private void Start()
	{
		LocalChatMessages.THIS = this;
	}

	// Token: 0x06000137 RID: 311 RVA: 0x00016F34 File Offset: 0x00015134
	public void AddLocalMessage(int bid, int x, int y, string str)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.messagePrefab);
		gameObject.transform.SetParent(this.canvas.transform);
		gameObject.GetComponentInChildren<Text>().text = str;
		LocalMessage localMessage = default(LocalMessage);
		localMessage.go = gameObject;
		localMessage.x = (float)x;
		localMessage.y = (float)y;
		localMessage.timeExpired = Time.unscaledTime + 3f;
		if (RobotRenderer.THIS.bots.ContainsKey(bid))
		{
			localMessage.rt = RobotRenderer.THIS.bots[bid].transform;
			Vector3 position = Camera.main.WorldToScreenPoint(new Vector3(localMessage.rt.position.x, localMessage.rt.position.y));
			localMessage.go.transform.position = position;
		}
		else
		{
			localMessage.rt = null;
			Vector3 position2 = Camera.main.WorldToScreenPoint(new Vector3((float)x, -(float)y));
			localMessage.go.transform.position = position2;
		}
		if (this.messages.ContainsKey(bid))
		{
			this.RemoveMessage(bid);
		}
		this.messages[bid] = localMessage;
	}

	// Token: 0x06000138 RID: 312 RVA: 0x0000590E File Offset: 0x00003B0E
	private void RemoveMessage(int id)
	{
		UnityEngine.Object.Destroy(this.messages[id].go);
		this.messages.Remove(id);
	}

	// Token: 0x06000139 RID: 313 RVA: 0x00017068 File Offset: 0x00015268
	private void Update()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, LocalMessage> keyValuePair in this.messages)
		{
			if (keyValuePair.Value.rt != null)
			{
				Vector3 a = Camera.main.WorldToScreenPoint(new Vector3(keyValuePair.Value.rt.position.x - 0.5f, keyValuePair.Value.rt.position.y));
				Vector3 vector = keyValuePair.Value.go.transform.position;
				vector = 0.3f * a + 0.7f * vector;
				keyValuePair.Value.go.transform.position = vector;
			}
			else
			{
				Vector3 a2 = Camera.main.WorldToScreenPoint(new Vector3(keyValuePair.Value.x - 0.5f, -keyValuePair.Value.y));
				Vector3 vector2 = keyValuePair.Value.go.transform.position;
				vector2 = 0.3f * a2 + 0.7f * vector2;
				keyValuePair.Value.go.transform.position = vector2;
			}
			if (Time.unscaledTime > keyValuePair.Value.timeExpired)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (int id in list)
		{
			this.RemoveMessage(id);
		}
	}

	// Token: 0x0400022C RID: 556
	public GameObject messagePrefab;

	// Token: 0x0400022D RID: 557
	public GameObject canvas;

	// Token: 0x0400022E RID: 558
	public static LocalChatMessages THIS;

	// Token: 0x0400022F RID: 559
	private Dictionary<int, LocalMessage> messages = new Dictionary<int, LocalMessage>();
}
