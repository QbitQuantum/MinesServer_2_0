using System;
using System.Text;
using UnityEngine;

// Token: 0x02000061 RID: 97
public class ServerTime : MonoBehaviour
{
	// Token: 0x0600027E RID: 638 RVA: 0x00006763 File Offset: 0x00004963
	private void Start()
	{
		this.obvyazka = base.gameObject.GetComponent<Obvyazka>();
		this.obvyazka.OnU("PI", new TypedCallback<string>(this.OnPI), false);
		ServerTime.THIS = this;
	}

	// Token: 0x0600027F RID: 639 RVA: 0x00006799 File Offset: 0x00004999
	public int NowTime()
	{
		return this.clientTimeStart + (int)(Time.unscaledTime * 1000f) - this.clientTimeLocal;
	}

	// Token: 0x06000280 RID: 640 RVA: 0x000274F8 File Offset: 0x000256F8
	public bool SendTypicalMessage(int time, string type, int x, int y, string str)
	{
		byte[] bytes = Encoding.UTF8.GetBytes(str);
		return this.SendTypicalMessage(time, type, x, y, bytes);
	}

	// Token: 0x06000281 RID: 641 RVA: 0x00027520 File Offset: 0x00025720
	public bool SendTypicalMessage(int time, string type, int x, int y, byte[] buffer)
	{
		if (time == -1)
		{
			time = this.lastSendedTime;
		}
		if (!this.ready)
		{
			throw new Exception("SendTypicalMessage - PIP is not ready &!");
		}
		if (type.Length != 4)
		{
			throw new Exception("SendTypicalMessage - bad type styring not 4 chars");
		}
		if (time < this.lastSendedTime)
		{
			time = this.lastSendedTime;
		}
		this.lastSendedTime = time;
		int num = 0;
		if (buffer != null)
		{
			num = buffer.Length;
		}
		byte[] array = new byte[16 + num];
		Array bytes = Encoding.UTF8.GetBytes(type);
		byte[] bytes2 = BitConverter.GetBytes((uint)time);
		byte[] bytes3 = BitConverter.GetBytes((uint)x);
		byte[] bytes4 = BitConverter.GetBytes((uint)y);
		Buffer.BlockCopy(bytes, 0, array, 0, 4);
		Buffer.BlockCopy(bytes2, 0, array, 4, 4);
		Buffer.BlockCopy(bytes3, 0, array, 8, 4);
		Buffer.BlockCopy(bytes4, 0, array, 12, 4);
		if (buffer == null)
		{
			this.obvyazka.SendB("TY", array);
		}
		else
		{
			Buffer.BlockCopy(buffer, 0, array, 16, num);
			this.obvyazka.SendB("TY", array);
		}
		return true;
	}

	// Token: 0x06000282 RID: 642 RVA: 0x00027610 File Offset: 0x00025810
	private void OnPI(ref string msg)
	{
		string[] array = msg.Split(new char[]
		{
			':'
		});
		if (this.clientTimeStart == -1)
		{
			this.clientTimeStart = int.Parse(array[1]);
			this.clientTimeLocal = (int)(Time.unscaledTime * 1000f);
			this.lastSendedTime = this.clientTimeStart;
			ClientController.serverTimeOfLastFrame = this.clientTimeStart;
			ClientController.clientTimeOfLastFrame = (int)(Time.unscaledTime * 1000f);
			this.ready = true;
		}
		this.lastPITime = int.Parse(array[1]);
		this.pingStr = array[2];
		ClientController.pongResponse = int.Parse(array[0]);
	}

	// Token: 0x06000283 RID: 643 RVA: 0x000276AC File Offset: 0x000258AC
	private void Update()
	{
		if (this.lastPITime < this.NowTime() - 40500)
		{
			FPSCountScript.PING_MESSAGE = " OFFLINE";
		}
		else if (this.lastPITime < this.NowTime() - 1500)
		{
			FPSCountScript.PING_MESSAGE = " FREEZE " + ((float)(this.NowTime() - this.lastPITime) / 1000f).ToString("0.0") + " sec";
		}
		else
		{
			FPSCountScript.PING_MESSAGE = " PING " + this.pingStr + "ms";
		}
		if (this.clientTimeStart != -1)
		{
			this.NowTime();
			int num = this.lastPITime;
			return;
		}
	}

	// Token: 0x04000474 RID: 1140
	private Obvyazka obvyazka;

	// Token: 0x04000475 RID: 1141
	public int clientTimeStart = -1;

	// Token: 0x04000476 RID: 1142
	private int clientTimeLocal = -1;

	// Token: 0x04000477 RID: 1143
	public static ServerTime THIS;

	// Token: 0x04000478 RID: 1144
	public int lastSendedTime;

	// Token: 0x04000479 RID: 1145
	public int lastPITime;

	// Token: 0x0400047A RID: 1146
	private string pingStr;

	// Token: 0x0400047B RID: 1147
	public bool ready;
}
