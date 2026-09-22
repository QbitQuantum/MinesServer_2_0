using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000013 RID: 19
public class ConnectionManager : MonoBehaviour
{
	// Token: 0x17000001 RID: 1
	// (get) Token: 0x06000095 RID: 149 RVA: 0x00005014 File Offset: 0x00003214
	public ConnectionStatusEvent onStatusChanged
	{
		get
		{
			return this._onStatusChanged;
		}
	}

	// Token: 0x17000002 RID: 2
	// (get) Token: 0x06000096 RID: 150 RVA: 0x0000501C File Offset: 0x0000321C
	public UnityEvent onConnect
	{
		get
		{
			return this._onConnect;
		}
	}

	// Token: 0x17000003 RID: 3
	// (get) Token: 0x06000097 RID: 151 RVA: 0x00005024 File Offset: 0x00003224
	public UnityEvent onDisconnect
	{
		get
		{
			return this._onDisconnect;
		}
	}

	// Token: 0x17000004 RID: 4
	// (get) Token: 0x06000098 RID: 152 RVA: 0x0000502C File Offset: 0x0000322C
	public UnityEvent onFinalDisconnect
	{
		get
		{
			return this._onFinalDisconnect;
		}
	}

	// Token: 0x17000005 RID: 5
	// (get) Token: 0x06000099 RID: 153 RVA: 0x00005034 File Offset: 0x00003234
	public UnityEvent onReconnect
	{
		get
		{
			return this._onReconnect;
		}
	}

	// Token: 0x0600009A RID: 154 RVA: 0x00011E98 File Offset: 0x00010098
	private void Start()
	{
		ConnectionManager.THIS = this;
		this.reconnectButton.gameObject.SetActive(false);
		this.InitEvents();
		this.obvyazka = base.gameObject.GetComponent<Obvyazka>();
		this.obvyazka.AddConnectHandler(new TypedCallback<string>(this.OnConnected));
		this.obvyazka.AddNoConnectHandler(new TypedCallback<string>(this.OnNoConnect));
		this.obvyazka.AddDisconnectHandler(new TypedCallback<string>(this.OnDisconnected));
		if (!this.DEBUG || this.DEBUG_WITH_AUTH)
		{
			this.playButton.gameObject.SetActive(true);
			this.playButton.onClick.AddListener(new UnityAction(this.FirstConnect));
			this.button.gameObject.SetActive(false);
			this.field.gameObject.SetActive(false);
		}
		else
		{
			this.playButton.gameObject.SetActive(false);
			this.button.gameObject.SetActive(true);
			this.field.gameObject.SetActive(true);
			this.button.onClick.AddListener(new UnityAction(this.DebugClick));
		}
		this.obvyazka.OnU("ST", new TypedCallback<string>(this.StatusLogger), false);
		this.obvyazka.OnU("RC", new TypedCallback<string>(this.ReconnectHandler), false);
		this.reconnectButton.onClick.AddListener(new UnityAction(this.ReconnectClick));
	}

	// Token: 0x0600009B RID: 155 RVA: 0x0000503C File Offset: 0x0000323C
	private void ReconnectHandler(ref string msg)
	{
		this.dontReconnect = true;
		ConnectionManager.phrase = "\n\nК вашему аккаунту подключились из другого места.";
	}

	// Token: 0x0600009C RID: 156 RVA: 0x0000504F File Offset: 0x0000324F
	public void StatusLogger(ref string msg)
	{
		this.connectionText.text = "Инициализируем сессию: " + msg;
	}

	// Token: 0x0600009D RID: 157 RVA: 0x00005068 File Offset: 0x00003268
	private void ReconnectClick()
	{
		if (ConnectionManager.disconnected && !this.tryingToConnect)
		{
			this.tryingToConnect = true;
			this.FirstConnect();
		}
	}

	// Token: 0x0600009E RID: 158 RVA: 0x00005086 File Offset: 0x00003286
	private void DebugClick()
	{
		AuthManager.debugGid = this.field.text;
		this.button.gameObject.SetActive(false);
		this.field.gameObject.SetActive(false);
		this.FirstConnect();
	}

	// Token: 0x0600009F RID: 159 RVA: 0x00012020 File Offset: 0x00010220
	private void InitEvents()
	{
		if (this._onConnect == null)
		{
			this._onConnect = new UnityEvent();
		}
		if (this._onDisconnect == null)
		{
			this._onDisconnect = new UnityEvent();
		}
		if (this._onFinalDisconnect == null)
		{
			this._onFinalDisconnect = new UnityEvent();
		}
		if (this._onReconnect == null)
		{
			this._onReconnect = new UnityEvent();
		}
		if (this._onStatusChanged == null)
		{
			this._onStatusChanged = new ConnectionStatusEvent();
		}
	}

	// Token: 0x060000A0 RID: 160 RVA: 0x000050C0 File Offset: 0x000032C0
	public void ForceDisconnectFull()
	{
		this.fadeIn = true;
		this.fadeOut = false;
		this.obvyazka.ForceDisconnect();
	}

	// Token: 0x060000A1 RID: 161 RVA: 0x000050DB File Offset: 0x000032DB
	public void ForceDisconnect()
	{
		this.fadeIn = true;
		this.fadeOut = false;
		this.obvyazka.ForceDisconnect();
		this.connectionText.gameObject.SetActive(true);
		base.Invoke("ReTry", 2f);
	}

	// Token: 0x060000A2 RID: 162 RVA: 0x0001208C File Offset: 0x0001028C
	private void ReTry()
	{
		this.fadeOut = false;
		this.fadeIn = true;
		ConnectionManager.disconnected = true;
		this.connectionText.text = ConnectionManager.phrase;
		if (!this.dontReconnect)
		{
			this.reconnectTries++;
			if (this.reconnectTries > 50)
			{
				this.dontReconnect = true;
			}
			this.reconnectButton.gameObject.SetActive(true);
			base.Invoke("ReconnectClick", 1f);
		}
	}

	// Token: 0x060000A3 RID: 163 RVA: 0x00012108 File Offset: 0x00010308
	private void FirstConnect()
	{
		if (this.DEBUG_PORTS && this.DEBUG)
		{
			this.tcpPort = 9094;
			this.ioPort = 9095;
		}
		else if (this.DEBUG_PORTS)
		{
			this.tcpPort = 8090;
			this.ioPort = 8091;
		}
		this.connectionText.text = "Подключаемся к серверу...";
		this.status = "connecting";
		this.obvyazka.Connect(this.tcpPort, this.ioPort, "mines3.firetype.ru");
		this._onStatusChanged.Invoke(this.status);
		this.playButton.gameObject.SetActive(false);
		this.button.gameObject.SetActive(false);
		this.field.gameObject.SetActive(false);
		this.reconnectButton.gameObject.SetActive(false);
		ServerTime.THIS.clientTimeStart = -1;
	}

	// Token: 0x060000A4 RID: 164 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void SecondConnect()
	{
	}

	// Token: 0x060000A5 RID: 165 RVA: 0x000121F4 File Offset: 0x000103F4
	private void OnDisconnected(ref string msg)
	{
		this._onDisconnect.Invoke();
		this.fadeIn = true;
		this.connectionText.gameObject.SetActive(true);
		this.reconnectionNum++;
		this.ReTry();
		this.prefix = "Потеря подключения";
	}

	// Token: 0x060000A6 RID: 166 RVA: 0x00005117 File Offset: 0x00003317
	private void OnNoConnect(ref string msg)
	{
		this.tryingToConnect = false;
		this.status = "no_connect";
		this._onFinalDisconnect.Invoke();
		this._onStatusChanged.Invoke(this.status);
		this.ReTry();
	}

	// Token: 0x060000A7 RID: 167 RVA: 0x00012244 File Offset: 0x00010444
	private void OnConnected(ref string msg)
	{
		this.tryingToConnect = false;
		this.status = "connected";
		this._onConnect.Invoke();
		this._onStatusChanged.Invoke(this.status);
		this.connectionText.text = "Подключение выполнено...";
		this.reconnectionNum = 2;
		this.fadeOut = true;
		ConnectionManager.disconnected = false;
	}

	// Token: 0x060000A8 RID: 168 RVA: 0x000122A4 File Offset: 0x000104A4
	private void Update()
	{
		if (this.reconnectTime > 0f)
		{
			int num = Mathf.FloorToInt(this.reconnectTime - Time.unscaledTime);
			if (num <= 0)
			{
				this.reconnectTime = -1f;
				this.connectionText.text = "Подключаемся к серверу...";
				return;
			}
			this.connectionText.text = this.prefix + "... Повторная попытка через " + num;
		}
	}

	// Token: 0x04000116 RID: 278
	public bool DEBUG;

	// Token: 0x04000117 RID: 279
	public bool DEBUG_PORTS;

	// Token: 0x04000118 RID: 280
	public bool DEBUG_WITH_AUTH = true;

	// Token: 0x04000119 RID: 281
	public static string METHOD = "SITE";

	// Token: 0x0400011A RID: 282
	private string status = "not_connected";

	// Token: 0x0400011B RID: 283
	public bool dontReconnect;

	// Token: 0x0400011C RID: 284
	private Obvyazka obvyazka;

	// Token: 0x0400011D RID: 285
	public Button button;

	// Token: 0x0400011E RID: 286
	public Button playButton;

	// Token: 0x0400011F RID: 287
	public InputField field;

	// Token: 0x04000120 RID: 288
	public Text connectionText;

	// Token: 0x04000121 RID: 289
	public Button reconnectButton;

	// Token: 0x04000122 RID: 290
	private ConnectionStatusEvent _onStatusChanged = new ConnectionStatusEvent();

	// Token: 0x04000123 RID: 291
	private UnityEvent _onConnect = new UnityEvent();

	// Token: 0x04000124 RID: 292
	private UnityEvent _onDisconnect = new UnityEvent();

	// Token: 0x04000125 RID: 293
	private UnityEvent _onFinalDisconnect = new UnityEvent();

	// Token: 0x04000126 RID: 294
	private UnityEvent _onReconnect = new UnityEvent();

	// Token: 0x04000127 RID: 295
	public static ConnectionManager THIS;

	// Token: 0x04000128 RID: 296
	private bool tryingToConnect;

	// Token: 0x04000129 RID: 297
	private int tries;

	// Token: 0x0400012A RID: 298
	public static bool disconnected;

	// Token: 0x0400012B RID: 299
	public static string phrase = "\n\nНет подключения к серверу.";

	// Token: 0x0400012C RID: 300
	private int reconnectTries;

	// Token: 0x0400012D RID: 301
	private int tcpPort = 8090;

	// Token: 0x0400012E RID: 302
	private int ioPort = 8082;

	// Token: 0x0400012F RID: 303
	private float reconnectTime = -1f;

	// Token: 0x04000130 RID: 304
	private int reconnectionNum = 2;

	// Token: 0x04000131 RID: 305
	private string prefix = "Сервер не отвечает";

	// Token: 0x04000132 RID: 306
	private bool fadeOut;

	// Token: 0x04000133 RID: 307
	private bool fadeIn;
}
