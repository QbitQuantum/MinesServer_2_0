using System;
using System.Runtime.InteropServices;
using UnityEngine;

namespace Obvyazka3
{
	// Token: 0x02000095 RID: 149
	public class SocketIOConnection : GameConnection
	{
		// Token: 0x060003FE RID: 1022
		[DllImport("__Internal")]
		private static extern void ConnectSocketIO(int port, string host);

		// Token: 0x060003FF RID: 1023
		[DllImport("__Internal")]
		private static extern void SendSocketIOJ(string name, string json);

		// Token: 0x06000400 RID: 1024
		[DllImport("__Internal")]
		private static extern void SendSocketIOU(string name, string str);

		// Token: 0x06000401 RID: 1025
		[DllImport("__Internal")]
		private static extern void SendSocketIOB(string name, string buffer);

		// Token: 0x06000402 RID: 1026
		[DllImport("__Internal")]
		private static extern void SocketIOPleaseDisconnect();

		// Token: 0x06000403 RID: 1027 RVA: 0x0000768A File Offset: 0x0000588A
		public SocketIOConnection()
		{
			this.jcbs = new EventCallbackSet<JSONNode>();
			this.bcbs = new EventCallbackSet<byte[]>();
			this.ucbs = new EventCallbackSet<string>();
		}

		// Token: 0x06000404 RID: 1028 RVA: 0x000076B3 File Offset: 0x000058B3
		public override void SendJ(string eventName, JSONNode message)
		{
			SocketIOConnection.SendSocketIOJ(eventName, message);
		}

		// Token: 0x06000405 RID: 1029 RVA: 0x000076C1 File Offset: 0x000058C1
		public override void SendB(string eventName, byte[] message)
		{
			SocketIOConnection.SendSocketIOB(eventName, Convert.ToBase64String(message));
		}

		// Token: 0x06000406 RID: 1030 RVA: 0x000076CF File Offset: 0x000058CF
		public override void SendU(string eventName, string message)
		{
			SocketIOConnection.SendSocketIOU(eventName, message);
		}

		// Token: 0x06000407 RID: 1031 RVA: 0x000076D8 File Offset: 0x000058D8
		public override void OnJ(string eventName, TypedCallback<JSONNode> cb, bool once)
		{
			this.jcbs.addEventCallback(eventName, cb, once);
		}

		// Token: 0x06000408 RID: 1032 RVA: 0x000076E8 File Offset: 0x000058E8
		public override void OnB(string eventName, TypedCallback<byte[]> cb, bool once)
		{
			this.bcbs.addEventCallback(eventName, cb, once);
		}

		// Token: 0x06000409 RID: 1033 RVA: 0x000076F8 File Offset: 0x000058F8
		public override void OnU(string eventName, TypedCallback<string> cb, bool once)
		{
			this.ucbs.addEventCallback(eventName, cb, once);
		}

		// Token: 0x0600040A RID: 1034 RVA: 0x0002F0B8 File Offset: 0x0002D2B8
		public override void Translate(string type, string eventName, string data)
		{
			if (type == "U")
			{
				this.ucbs.emitEvent(eventName, ref data);
				return;
			}
			if (type == "B")
			{
				byte[] array = Convert.FromBase64String(data);
				this.bcbs.emitEvent(eventName, ref array);
				return;
			}
			if (!(type == "J"))
			{
				return;
			}
			JSONNode jsonnode = JSON.Parse(data);
			this.jcbs.emitEvent(eventName, ref jsonnode);
		}

		// Token: 0x0600040B RID: 1035 RVA: 0x00007708 File Offset: 0x00005908
		public override void Connect(int port, string host)
		{
			SocketIOConnection.ConnectSocketIO(port, host);
		}

		// Token: 0x0600040C RID: 1036 RVA: 0x0002F128 File Offset: 0x0002D328
		public override void Update()
		{
			try
			{
				this.jcbs.emitFromQueue();
				this.ucbs.emitFromQueue();
				this.bcbs.emitFromQueue();
			}
			catch (Exception arg)
			{
				Debug.Log("TRANSLATION ERROR! " + arg);
			}
		}

		// Token: 0x0600040D RID: 1037 RVA: 0x00007711 File Offset: 0x00005911
		public override void ForceDisconnect()
		{
			SocketIOConnection.SocketIOPleaseDisconnect();
		}

		// Token: 0x0600040E RID: 1038 RVA: 0x00007718 File Offset: 0x00005918
		public override void Destroy()
		{
			SocketIOConnection.SocketIOPleaseDisconnect();
			this.jcbs.cbs.Clear();
			this.bcbs.cbs.Clear();
			this.ucbs.cbs.Clear();
		}

		// Token: 0x040005E3 RID: 1507
		public EventCallbackSet<JSONNode> jcbs;

		// Token: 0x040005E4 RID: 1508
		public EventCallbackSet<byte[]> bcbs;

		// Token: 0x040005E5 RID: 1509
		public EventCallbackSet<string> ucbs;

		// Token: 0x040005E6 RID: 1510
		public string _host;

		// Token: 0x040005E7 RID: 1511
		public int _port;
	}
}
