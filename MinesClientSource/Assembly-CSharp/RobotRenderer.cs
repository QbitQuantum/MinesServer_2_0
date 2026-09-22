using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x0200005D RID: 93
public class RobotRenderer : MonoBehaviour
{
	// Token: 0x06000232 RID: 562 RVA: 0x000064C6 File Offset: 0x000046C6
	private void Start()
	{
		RobotRenderer.THIS = this;
		this.terrainRenderer = base.gameObject.GetComponent<TerrainRendererScript>();
		this.clientController = this.clientControllerObject.GetComponent<ClientController>();
	}

	// Token: 0x06000233 RID: 563 RVA: 0x000064F0 File Offset: 0x000046F0
	public void Init()
	{
		if (!RobotRenderer.inited)
		{
			RobotRenderer.inited = true;
		}
	}

	// Token: 0x06000234 RID: 564 RVA: 0x0002415C File Offset: 0x0002235C
	public void XYBot(int id, int x, int y, int dir, int cid, int skin, int tail)
	{
		if (this.bots.ContainsKey(id))
		{
			if (id != this.clientController.myBotId || tail == 1)
			{
				this.bots[id].GetComponent<RobotScript>().SetXY((float)x, (float)y);
				this.bots[id].GetComponent<RobotScript>().SetRotation(dir);
				if (id == this.clientController.myBotId)
				{
					this.clientController.dir = dir;
				}
			}
			else
			{
				this.clientController.myBotLastSyncX = x;
				this.clientController.myBotLastSyncY = y;
			}
		}
		else
		{
			this.AddNewBot(x, y, id);
			this.bots[id].GetComponent<RobotScript>().SetRotation(dir);
		}
		this.bots[id].GetComponent<RobotScript>().lastPingTime = Time.unscaledTime;
		this.bots[id].GetComponent<RobotScript>().SetSkin(skin);
		this.bots[id].GetComponent<RobotScript>().SetClan(cid);
		this.bots[id].GetComponent<RobotScript>().deathPingTime = -1f;
		this.tails[id] = tail;
	}

	// Token: 0x06000235 RID: 565 RVA: 0x0002428C File Offset: 0x0002248C
	public void RemoveBotFromBlock(int id, int block)
	{
		if (this.bots.ContainsKey(id) && id != this.clientController.myBotId)
		{
			RobotScript component = this.bots[id].GetComponent<RobotScript>();
			if ((component.gx >> 5) + (component.gy >> 5) * MapModel._blocksW == block)
			{
				component.deathPingTime = Time.unscaledTime;
			}
		}
	}

	// Token: 0x06000236 RID: 566 RVA: 0x000242EC File Offset: 0x000224EC
	public void RemoveAll()
	{
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.bots)
		{
			UnityEngine.Object.Destroy(keyValuePair.Value);
		}
		this.bots.Clear();
	}

	// Token: 0x06000237 RID: 567 RVA: 0x00024350 File Offset: 0x00022550
	public void RemoveBot(int id)
	{
		bool flag = this.bots.ContainsKey(id) && id != this.clientController.myBotId;
		bool flag2 = flag;
		bool flag3 = flag2;
		if (flag3)
		{
			UnityEngine.Object.Destroy(this.bots[id]);
			this.bots.Remove(id);
			UnityEngine.Object.Destroy(this.nickTFs[id].gameObject);
			this.tails.Remove(id);
			this.nickTFs.Remove(id);
			bool flag4 = this.nicks.ContainsKey(id);
			bool flag5 = flag4;
			bool flag6 = flag5;
			if (flag6)
			{
				this.nicks.Remove(id);
			}
		}
		GUIManager.THIS.RemoveBot(id);
	}

	// Token: 0x06000238 RID: 568 RVA: 0x00024410 File Offset: 0x00022610
	public RobotScript AddNewBot(int x, int y, int id)
	{
		GameObject gameObject = UnityEngine.Object.Instantiate<GameObject>(this.robotPrefab);
		gameObject.transform.SetParent(this.RenderWrapper.transform, false);
		RobotScript component = gameObject.GetComponent<RobotScript>();
		component.SetXY((float)x, (float)y);
		component.SyncXY();
		component.id = id;
		if (this.bots.ContainsKey(id))
		{
			this.RemoveBot(id);
			this.bots.Add(id, gameObject);
		}
		this.bots.Add(id, gameObject);
		Text text = UnityEngine.Object.Instantiate<Text>(this.nickPrefab);
		text.text = "";
		text.transform.SetParent(this.canvas.transform, false);
		if (this.nickTFs.ContainsKey(id))
		{
			UnityEngine.Object.Destroy(this.nickTFs[id].gameObject);
			this.nickTFs.Remove(id);
		}
		this.nickTFs[id] = text;
		this.tails[id] = 0;
		return component;
	}

	// Token: 0x06000239 RID: 569 RVA: 0x00024508 File Offset: 0x00022708
	public void RemoveAllBots()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.bots)
		{
			list.Add(keyValuePair.Key);
		}
		foreach (int id in list)
		{
			this.RemoveBot(id);
		}
	}

	// Token: 0x0600023A RID: 570 RVA: 0x000064FF File Offset: 0x000046FF
	public void AddNick(int id, string nick)
	{
		this.nicks[id] = nick;
		GUIManager.THIS.AddBot(id, nick);
	}

	// Token: 0x0600023B RID: 571 RVA: 0x0000651D File Offset: 0x0000471D
	public void AddNewBotForMe(int x, int y, int id, string name, out RobotScript rs)
	{
		this.nicks[id] = name;
		rs = this.AddNewBot(x, y, id);
	}

	// Token: 0x0600023C RID: 572 RVA: 0x000245A8 File Offset: 0x000227A8
	public void CheckAliveBots(int num, int[] bids)
	{
		if (num != 0)
		{
			HashSet<int> hashSet = new HashSet<int>();
			foreach (KeyValuePair<int, GameObject> keyValuePair in this.bots)
			{
				hashSet.Add(keyValuePair.Key);
			}
			for (int i = 0; i < num; i++)
			{
				hashSet.Remove(bids[i]);
			}
			foreach (int id in hashSet)
			{
				this.RemoveBot(id);
			}
		}
	}

	// Token: 0x0600023D RID: 573 RVA: 0x00024664 File Offset: 0x00022864
	public void RobotsGarbageCollector()
	{
		List<int> list = new List<int>();
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.bots)
		{
			float deathPingTime = keyValuePair.Value.GetComponent<RobotScript>().deathPingTime;
			if ((Vector2.Distance(keyValuePair.Value.transform.position, new Vector2((float)this.clientController.view_x, -(float)this.clientController.view_y)) > 205f || keyValuePair.Value.GetComponent<RobotScript>().lastPingTime < Time.unscaledTime - 6f || (deathPingTime > 0f && deathPingTime < Time.unscaledTime - 0.5f)) && keyValuePair.Key != this.clientController.myBotId)
			{
				list.Add(keyValuePair.Key);
			}
		}
		foreach (int id in list)
		{
			this.RemoveBot(id);
		}
	}

	// Token: 0x0600023E RID: 574 RVA: 0x000247A4 File Offset: 0x000229A4
	private void Update()
	{
		if (!RobotRenderer.inited)
		{
			return;
		}
		if (Time.unscaledTime > this.lastRobotGCTime + 4f)
		{
			this.RobotsGarbageCollector();
			this.lastRobotGCTime = Time.unscaledTime;
		}
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.bots)
		{
			Vector3 a = Camera.main.WorldToScreenPoint(keyValuePair.Value.transform.position);
			this.nickTFs[keyValuePair.Key].transform.position = a + new Vector3(89f, 2f, 0f);
			if (this.nicks.ContainsKey(keyValuePair.Key))
			{
				this.nickTFs[keyValuePair.Key].text = this.nicks[keyValuePair.Key];
				if (!ClientConfig.SHOW_MY_NICK && keyValuePair.Key == ClientController.THIS.myBotId)
				{
					this.nickTFs[keyValuePair.Key].text = "";
				}
				if (this.tails.ContainsKey(keyValuePair.Key))
				{
					if (this.tails[keyValuePair.Key] == 1)
					{
						this.nickTFs[keyValuePair.Key].color = Color.yellow;
					}
					else if (this.tails[keyValuePair.Key] == 2)
					{
						this.nickTFs[keyValuePair.Key].color = Color.green;
					}
					else
					{
						this.nickTFs[keyValuePair.Key].color = Color.white;
					}
				}
			}
			else
			{
				this.bidsToKnow.Add(keyValuePair.Key);
			}
		}
		if (this.clientController.myBotId != -1)
		{
			if (Time.unscaledTime > this.lastMyRobotNamingTime + 2f)
			{
				this.nickTF.text = this.nicks[this.clientController.myBotId];
				this.lastMyRobotNamingTime = Time.unscaledTime;
			}
			GUIManager.THIS.SetCoord(this.clientController.myBot.gx, this.clientController.myBot.gy);
		}
		if (Time.unscaledTime > this.lastRobotNamingTime + 1f)
		{
			string text = "";
			foreach (int num in this.bidsToKnow)
			{
				if (text != "")
				{
					text += ",";
				}
				text += num;
			}
			this.bidsToKnow.Clear();
			if (text != "")
			{
				ServerTime.THIS.SendTypicalMessage(ClientController.THIS.TimeOfMove(), "Whoi", 0, 0, text);
			}
			this.lastRobotNamingTime = Time.unscaledTime;
		}
	}

	// Token: 0x0400042B RID: 1067
	public GameObject robotPrefab;

	// Token: 0x0400042C RID: 1068
	public GameObject clientControllerObject;

	// Token: 0x0400042D RID: 1069
	private ClientController clientController;

	// Token: 0x0400042E RID: 1070
	public GameObject canvas;

	// Token: 0x0400042F RID: 1071
	public Text nickPrefab;

	// Token: 0x04000430 RID: 1072
	public Text nickTF;

	// Token: 0x04000431 RID: 1073
	private Dictionary<int, int> tails = new Dictionary<int, int>();

	// Token: 0x04000432 RID: 1074
	private Dictionary<int, string> nicks = new Dictionary<int, string>();

	// Token: 0x04000433 RID: 1075
	private Dictionary<int, Text> nickTFs = new Dictionary<int, Text>();

	// Token: 0x04000434 RID: 1076
	public GameObject RenderWrapper;

	// Token: 0x04000435 RID: 1077
	public Dictionary<int, GameObject> bots = new Dictionary<int, GameObject>();

	// Token: 0x04000436 RID: 1078
	private TerrainRendererScript terrainRenderer;

	// Token: 0x04000437 RID: 1079
	public static RobotRenderer THIS;

	// Token: 0x04000438 RID: 1080
	public static bool inited;

	// Token: 0x04000439 RID: 1081
	public bool isProgrammator;

	// Token: 0x0400043A RID: 1082
	private float lastRobotGCTime;

	// Token: 0x0400043B RID: 1083
	private float lastRobotNamingTime;

	// Token: 0x0400043C RID: 1084
	private float lastMyRobotNamingTime;

	// Token: 0x0400043D RID: 1085
	public HashSet<int> bidsToKnow = new HashSet<int>();
}
