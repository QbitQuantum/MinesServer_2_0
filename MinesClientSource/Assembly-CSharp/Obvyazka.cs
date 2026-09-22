using System;
using System.Threading;
using Obvyazka3;
using UnityEngine;

// Token: 0x02000040 RID: 64
public class Obvyazka : MonoBehaviour
{
	// Token: 0x06000197 RID: 407 RVA: 0x0001B598 File Offset: 0x00019798
	public void Connect(int tcpPort, int socketIOPort, string host)
	{
		if (!this._inited)
		{
			this.Init();
		}
		this.client.OnU("_connected", new TypedCallback<string>(this._onTCPConnected), true);
		this.client.OnU("_connectError", new TypedCallback<string>(this._onTCPNoConnect), true);
		this.client.OnU("_disconnect", new TypedCallback<string>(this._onTCPDisconnect), true);
		this.client.Connect(tcpPort, host);
	}

	// Token: 0x06000198 RID: 408 RVA: 0x00005CD9 File Offset: 0x00003ED9
	private void _onTCPDisconnect(ref string msg)
	{
		this._needDisconnect = true;
	}

	// Token: 0x06000199 RID: 409 RVA: 0x00005CE2 File Offset: 0x00003EE2
	private void _onTCPNoConnect(ref string msg)
	{
		this._needNoConnect = true;
	}

	// Token: 0x0600019A RID: 410 RVA: 0x00005CEB File Offset: 0x00003EEB
	private void _onTCPConnected(ref string msg)
	{
		this._needConnected = true;
	}

	// Token: 0x0600019B RID: 411 RVA: 0x00005CF4 File Offset: 0x00003EF4
	private void Init()
	{
		if (!this._inited)
		{
			this._tcpOperationMutex = new Mutex();
			this.client = new TCPConnection(this._tcpOperationMutex);
		}
		this._inited = true;
	}

	// Token: 0x0600019C RID: 412 RVA: 0x0001B618 File Offset: 0x00019818
	private void Update()
	{
		if (!this._inited)
		{
			return;
		}
		if (this._tcpOperationMutex != null && this._tcpOperationMutex.WaitOne(10))
		{
			if (this._needConnected)
			{
				this._needConnected = false;
				this.Connected();
			}
			if (this._needNoConnect)
			{
				this._needNoConnect = false;
				this.NoConnect();
			}
			if (this._needDisconnect)
			{
				this._needDisconnect = false;
				this.Disconnected();
			}
			this.client.Update();
			this._tcpOperationMutex.ReleaseMutex();
		}
	}

	// Token: 0x0600019D RID: 413 RVA: 0x00005D21 File Offset: 0x00003F21
	private void Start()
	{
		this.Init();
	}

	// Token: 0x0600019E RID: 414 RVA: 0x00005D29 File Offset: 0x00003F29
	public void ForceDisconnect()
	{
		if (this.client != null)
		{
			this.client.ForceDisconnect();
		}
	}

	// Token: 0x0600019F RID: 415 RVA: 0x00005D3E File Offset: 0x00003F3E
	public void AddDisconnectHandler(TypedCallback<string> cb)
	{
		this._stringCallbackSet.addEventCallback("disconnect", cb, false);
	}

	// Token: 0x060001A0 RID: 416 RVA: 0x00005D52 File Offset: 0x00003F52
	public void AddNoConnectHandler(TypedCallback<string> cb)
	{
		this._stringCallbackSet.addEventCallback("noconnect", cb, false);
	}

	// Token: 0x060001A1 RID: 417 RVA: 0x00005D66 File Offset: 0x00003F66
	public void AddConnectHandler(TypedCallback<string> cb)
	{
		this._stringCallbackSet.addEventCallback("connect", cb, false);
	}

	// Token: 0x060001A2 RID: 418 RVA: 0x00005D7A File Offset: 0x00003F7A
	public void SendJ(string eventName, JSONNode message)
	{
		this.Init();
		this.client.SendJ(eventName, message);
	}

	// Token: 0x060001A3 RID: 419 RVA: 0x00005D8F File Offset: 0x00003F8F
	public void SendU(string eventName, string message)
	{
		this.Init();
		this.client.SendU(eventName, message);
	}

	// Token: 0x060001A4 RID: 420 RVA: 0x00005DA4 File Offset: 0x00003FA4
	public void SendB(string eventName, byte[] message)
	{
		this.Init();
		this.client.SendB(eventName, message);
	}

	// Token: 0x060001A5 RID: 421 RVA: 0x00005DB9 File Offset: 0x00003FB9
	public void OnJ(string eventName, TypedCallback<JSONNode> cb, bool once = false)
	{
		this.Init();
		this.client.OnJ(eventName, cb, once);
	}

	// Token: 0x060001A6 RID: 422 RVA: 0x00005DCF File Offset: 0x00003FCF
	public void OnU(string eventName, TypedCallback<string> cb, bool once = false)
	{
		this.Init();
		this.client.OnU(eventName, cb, once);
	}

	// Token: 0x060001A7 RID: 423 RVA: 0x00005DE5 File Offset: 0x00003FE5
	public void OnB(string eventName, TypedCallback<byte[]> cb, bool once = false)
	{
		this.Init();
		this.client.OnB(eventName, cb, once);
	}

	// Token: 0x060001A8 RID: 424 RVA: 0x0001B69C File Offset: 0x0001989C
	private void Connected()
	{
		string text = "connect";
		this._stringCallbackSet.emitEvent("connect", ref text);
	}

	// Token: 0x060001A9 RID: 425 RVA: 0x0001B6C4 File Offset: 0x000198C4
	private void Disconnected()
	{
		string text = "disconnect";
		this._stringCallbackSet.emitEvent("disconnect", ref text);
	}

	// Token: 0x060001AA RID: 426 RVA: 0x0001B6EC File Offset: 0x000198EC
	private void NoConnect()
	{
		string text = "noconnect";
		this._stringCallbackSet.emitEvent("noconnect", ref text);
	}

	// Token: 0x060001AB RID: 427 RVA: 0x00005DFB File Offset: 0x00003FFB
	private void OnApplicationQuit()
	{
		if (this.client != null)
		{
			this.client.Destroy();
		}
	}

	// Token: 0x040002BC RID: 700
	public GameConnection client;

	// Token: 0x040002BD RID: 701
	private bool _inited;

	// Token: 0x040002BE RID: 702
	private EventCallbackSet<string> _stringCallbackSet = new EventCallbackSet<string>();

	// Token: 0x040002BF RID: 703
	private Mutex _tcpOperationMutex;

	// Token: 0x040002C0 RID: 704
	private bool _needDisconnect;

	// Token: 0x040002C1 RID: 705
	private bool _needNoConnect;

	// Token: 0x040002C2 RID: 706
	private bool _needConnected;

	// Token: 0x040002C3 RID: 707
	public string vk_access_token = "";

	// Token: 0x040002C4 RID: 708
	public string vk_access_token2 = "";
}
