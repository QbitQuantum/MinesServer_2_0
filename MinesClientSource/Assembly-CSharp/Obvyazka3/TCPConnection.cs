using System;
using System.Net.Sockets;
using System.Text;
using System.Threading;
using UnityEngine;

namespace Obvyazka3
{
	// Token: 0x02000097 RID: 151
	public class TCPConnection : GameConnection
	{
		// Token: 0x06000410 RID: 1040 RVA: 0x0000774F File Offset: 0x0000594F
		private void recreateSocket()
		{
			this._client = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
			this._client.NoDelay = true;
			this.canConnect = true;
		}

		// Token: 0x06000411 RID: 1041 RVA: 0x0002F17C File Offset: 0x0002D37C
		public TCPConnection(Mutex tcpOperationMutex)
		{
			this._tcpOperationMutex = tcpOperationMutex;
			this.recreateSocket();
			this._segmentBuffer = new byte[this._maxSegmentLen];
			this._mergedBuffer = new byte[this._maxBufferLen];
			this._typeBuffer = new byte[1];
			this._eventNameBuffer = new byte[2];
			this._jsonCallbackSet = new EventCallbackSet<JSONNode>();
			this._bufferCallbackSet = new EventCallbackSet<byte[]>();
			this._stringCallbackSet = new EventCallbackSet<string>();
		}

		// Token: 0x06000412 RID: 1042 RVA: 0x0002F214 File Offset: 0x0002D414
		public override void Connect(int port, string host)
		{
			this._port = port;
			this._host = host;
			SocketState state = new SocketState();
			this._client.BeginConnect(this._host, this._port, new AsyncCallback(this._connectHandler), state);
			this.canConnect = false;
		}

		// Token: 0x06000413 RID: 1043 RVA: 0x0002F264 File Offset: 0x0002D464
		private void _connectHandler(IAsyncResult asyncResult)
		{
			this._tcpOperationMutex.WaitOne();
			try
			{
				SocketState state = (SocketState)asyncResult.AsyncState;
				this._client.EndConnect(asyncResult);
				string text = "";
				this._stringCallbackSet.emitEvent("_connected", ref text);
				this._client.BeginReceive(this._segmentBuffer, 0, this._segmentBuffer.Length, SocketFlags.None, new AsyncCallback(this._recieveHandler), state);
			}
			catch (Exception)
			{
				string text2 = "";
				this._stringCallbackSet.emitEvent("_connectError", ref text2);
				this.recreateSocket();
			}
			this._tcpOperationMutex.ReleaseMutex();
		}

		// Token: 0x06000414 RID: 1044 RVA: 0x0002F314 File Offset: 0x0002D514
		public override void SendJ(string eventName, JSONNode message)
		{
			if (!this.canConnect)
			{
				string s = message.ToString();
				byte[] bytes = Encoding.UTF8.GetBytes("J" + eventName);
				byte[] bytes2 = Encoding.UTF8.GetBytes(s);
				int num = 4 + bytes.Length + bytes2.Length;
				byte[] bytes3 = BitConverter.GetBytes((uint)num);
				byte[] array = new byte[num];
				Buffer.BlockCopy(bytes3, 0, array, 0, bytes3.Length);
				Buffer.BlockCopy(bytes, 0, array, bytes3.Length, bytes.Length);
				Buffer.BlockCopy(bytes2, 0, array, bytes3.Length + bytes.Length, bytes2.Length);
				try
				{
					this._client.Send(array);
				}
				catch (Exception)
				{
					string text = "";
					this._stringCallbackSet.emitEvent("_disconnect", ref text);
					this.recreateSocket();
				}
			}
		}

		// Token: 0x06000415 RID: 1045 RVA: 0x0002F3E0 File Offset: 0x0002D5E0
		public override void SendB(string eventName, byte[] message)
		{
			if (!this.canConnect)
			{
				byte[] bytes = Encoding.UTF8.GetBytes("B" + eventName);
				int num = 4 + bytes.Length + message.Length;
				byte[] bytes2 = BitConverter.GetBytes((uint)num);
				byte[] array = new byte[num];
				Buffer.BlockCopy(bytes2, 0, array, 0, bytes2.Length);
				Buffer.BlockCopy(bytes, 0, array, bytes2.Length, bytes.Length);
				Buffer.BlockCopy(message, 0, array, bytes2.Length + bytes.Length, message.Length);
				try
				{
					this._client.Send(array);
				}
				catch (Exception)
				{
					string text = "";
					this._stringCallbackSet.emitEvent("_disconnect", ref text);
					this.recreateSocket();
				}
			}
		}

		// Token: 0x06000416 RID: 1046 RVA: 0x0002F490 File Offset: 0x0002D690
		public override void SendU(string eventName, string message)
		{
			if (!this.canConnect)
			{
				byte[] bytes = Encoding.UTF8.GetBytes("U" + eventName);
				byte[] bytes2 = Encoding.UTF8.GetBytes(message);
				int num = 4 + bytes.Length + bytes2.Length;
				byte[] bytes3 = BitConverter.GetBytes((uint)num);
				byte[] array = new byte[num];
				Buffer.BlockCopy(bytes3, 0, array, 0, bytes3.Length);
				Buffer.BlockCopy(bytes, 0, array, bytes3.Length, bytes.Length);
				Buffer.BlockCopy(bytes2, 0, array, bytes3.Length + bytes.Length, bytes2.Length);
				try
				{
					this._client.Send(array);
				}
				catch (Exception)
				{
					string text = "";
					this._stringCallbackSet.emitEvent("_disconnect", ref text);
					this.recreateSocket();
				}
			}
		}

		// Token: 0x06000417 RID: 1047 RVA: 0x00007772 File Offset: 0x00005972
		public override void OnJ(string eventName, TypedCallback<JSONNode> cb, bool once = false)
		{
			this._jsonCallbackSet.addEventCallback(eventName, cb, once);
		}

		// Token: 0x06000418 RID: 1048 RVA: 0x00007782 File Offset: 0x00005982
		public override void OnB(string eventName, TypedCallback<byte[]> cb, bool once = false)
		{
			this._bufferCallbackSet.addEventCallback(eventName, cb, once);
		}

		// Token: 0x06000419 RID: 1049 RVA: 0x00007792 File Offset: 0x00005992
		public override void OnU(string eventName, TypedCallback<string> cb, bool once = false)
		{
			this._stringCallbackSet.addEventCallback(eventName, cb, once);
		}

		// Token: 0x0600041A RID: 1050 RVA: 0x0002F550 File Offset: 0x0002D750
		private void _recieveHandler(IAsyncResult asyncResult)
		{
			this._tcpOperationMutex.WaitOne();
			SocketState socketState = (SocketState)asyncResult.AsyncState;
			socketState.counter++;
			int counter = socketState.counter;
			int num = this._client.EndReceive(asyncResult);
			if (num > 0)
			{
				if (this._mergedLen + num > this._maxBufferLen)
				{
					this._tcpOperationMutex.ReleaseMutex();
					return;
				}
				Buffer.BlockCopy(this._segmentBuffer, 0, this._mergedBuffer, this._mergedLen, num);
				this._mergedLen += num;
				int num2 = 0;
				for (;;)
				{
					num2++;
					if (this._mergedLen < 4)
					{
						goto IL_F2;
					}
					int num3 = Convert.ToInt32(BitConverter.ToUInt32(this._mergedBuffer, 0));
					if (num3 > this._mergedLen)
					{
						goto IL_F2;
					}
					this._translateMessage(num3);
					if (num3 >= this._mergedLen)
					{
						break;
					}
					Buffer.BlockCopy(this._mergedBuffer, num3, this._mergedBuffer, 0, this._mergedLen - num3);
					this._mergedLen -= num3;
				}
				this._mergedLen = 0;
				IL_F2:
				this._client.BeginReceive(this._segmentBuffer, 0, this._segmentBuffer.Length, SocketFlags.None, new AsyncCallback(this._recieveHandler), socketState);
			}
			else
			{
				string text = "";
				this._stringCallbackSet.emitEvent("_disconnect", ref text);
				this.recreateSocket();
			}
			this._tcpOperationMutex.ReleaseMutex();
		}

		// Token: 0x0600041B RID: 1051 RVA: 0x00004B5F File Offset: 0x00002D5F
		public override void Translate(string type, string eventName, string data)
		{
		}

		// Token: 0x0600041C RID: 1052 RVA: 0x0002F6A4 File Offset: 0x0002D8A4
		public override void ForceDisconnect()
		{
			try
			{
				this._client.DisconnectAsync(new SocketAsyncEventArgs());
			}
			catch (Exception)
			{
			}
			this.recreateSocket();
		}

		// Token: 0x0600041D RID: 1053 RVA: 0x0002F6E0 File Offset: 0x0002D8E0
		public override void Update()
		{
			try
			{
				this._jsonCallbackSet.emitFromQueue();
				this._stringCallbackSet.emitFromQueue();
				this._bufferCallbackSet.emitFromQueue();
			}
			catch (Exception arg)
			{
				Debug.Log("TRANSLATION ERROR! " + arg);
			}
		}

		// Token: 0x0600041E RID: 1054 RVA: 0x0002F734 File Offset: 0x0002D934
		private void _translateMessage(int messageLen)
		{
			Buffer.BlockCopy(this._mergedBuffer, 4, this._typeBuffer, 0, 1);
			Buffer.BlockCopy(this._mergedBuffer, 5, this._eventNameBuffer, 0, 2);
			string @string = Encoding.Default.GetString(this._typeBuffer);
			string string2 = Encoding.Default.GetString(this._eventNameBuffer);
			if (@string == "J")
			{
				JSONNode jsonnode = JSON.Parse(Encoding.Default.GetString(this._mergedBuffer, 7, messageLen - 7));
				this._jsonCallbackSet.delayedEmitEvent(string2, ref jsonnode);
				return;
			}
			if (@string == "U")
			{
				string string3 = Encoding.UTF8.GetString(this._mergedBuffer, 7, messageLen - 7);
				this._stringCallbackSet.delayedEmitEvent(string2, ref string3);
				return;
			}
			if (@string == "B")
			{
				byte[] dst = new byte[messageLen - 7];
				Buffer.BlockCopy(this._mergedBuffer, 7, dst, 0, messageLen - 7);
				this._bufferCallbackSet.delayedEmitEvent(string2, ref dst);
			}
		}

		// Token: 0x0600041F RID: 1055 RVA: 0x0002F82C File Offset: 0x0002DA2C
		public override void Destroy()
		{
			try
			{
				this._client.DisconnectAsync(new SocketAsyncEventArgs());
			}
			catch (Exception)
			{
			}
			this._jsonCallbackSet.cbs.Clear();
			this._bufferCallbackSet.cbs.Clear();
			this._stringCallbackSet.cbs.Clear();
		}

		// Token: 0x040005E9 RID: 1513
		private Socket _client;

		// Token: 0x040005EA RID: 1514
		private byte[] _segmentBuffer;

		// Token: 0x040005EB RID: 1515
		private byte[] _mergedBuffer;

		// Token: 0x040005EC RID: 1516
		private byte[] _typeBuffer;

		// Token: 0x040005ED RID: 1517
		private byte[] _eventNameBuffer;

		// Token: 0x040005EE RID: 1518
		private EventCallbackSet<JSONNode> _jsonCallbackSet;

		// Token: 0x040005EF RID: 1519
		private EventCallbackSet<byte[]> _bufferCallbackSet;

		// Token: 0x040005F0 RID: 1520
		private EventCallbackSet<string> _stringCallbackSet;

		// Token: 0x040005F1 RID: 1521
		private int _mergedLen;

		// Token: 0x040005F2 RID: 1522
		private int _maxSegmentLen = 68000;

		// Token: 0x040005F3 RID: 1523
		private int _maxBufferLen = 1000000;

		// Token: 0x040005F4 RID: 1524
		private const int LEN_PREFIX_LENGTH = 4;

		// Token: 0x040005F5 RID: 1525
		private const int TYPE_PREFIX_LENGTH = 1;

		// Token: 0x040005F6 RID: 1526
		private const int EVENTNAME_PREFIX_LENGTH = 2;

		// Token: 0x040005F7 RID: 1527
		private const int PREFIX_LENGTH = 7;

		// Token: 0x040005F8 RID: 1528
		private string _host;

		// Token: 0x040005F9 RID: 1529
		private int _port;

		// Token: 0x040005FA RID: 1530
		private Mutex _tcpOperationMutex;

		// Token: 0x040005FB RID: 1531
		private bool canConnect = true;
	}
}
