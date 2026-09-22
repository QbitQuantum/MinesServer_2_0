using System;
using System.ComponentModel;
using System.Diagnostics;
using System.Net;
using System.Threading;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000077 RID: 119
public class WorldInitScript : MonoBehaviour
{
	// Token: 0x060002EE RID: 750 RVA: 0x0002D97C File Offset: 0x0002BB7C
	private void Start()
	{
		WorldInitScript.THIS = this;
		this.pad.gameObject.SetActive(true);
		this.obvyazka = base.gameObject.GetComponent<Obvyazka>();
		this.obvyazka.OnU("cf", new TypedCallback<string>(this.OnWorldConfig), false);
		/*
		string[] commandLineArgs = Environment.GetCommandLineArgs();
		for (int i = 0; i < commandLineArgs.Length; i++)
		{
			if (commandLineArgs[i] == "-ignoreConfig")
			{
				WorldInitScript.ignoreConfig = true;
			}
		}
		*/

	}

	// Token: 0x060002EF RID: 751 RVA: 0x00006CFD File Offset: 0x00004EFD
	public static void SetupExceptionHandling()
	{
		if (!WorldInitScript.isExceptionHandlingSetup)
		{
			WorldInitScript.isExceptionHandlingSetup = true;
			Application.logMessageReceived += WorldInitScript.HandleException;
		}
	}

	// Token: 0x060002F0 RID: 752 RVA: 0x00006D1D File Offset: 0x00004F1D
	private static void HandleException(string condition, string stackTrace, LogType type)
	{
		if (type == LogType.Exception)
		{
			UnityEngine.Debug.Log(condition + "\n" + stackTrace);
		}
	}

	// Token: 0x060002F1 RID: 753 RVA: 0x00006D34 File Offset: 0x00004F34
	private void wb_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
	{
		this.UpdatingMutex.WaitOne();
		this.downloadedPercent = e.ProgressPercentage;
		this.updateProgress = true;
		this.UpdatingMutex.ReleaseMutex();
	}

	// Token: 0x060002F2 RID: 754 RVA: 0x00006D60 File Offset: 0x00004F60
	private void wb_DownloadFileCompleted(object sender, AsyncCompletedEventArgs e)
	{
		this.UpdatingMutex.WaitOne();
		this.launchUpdater = true;
		this.UpdatingMutex.ReleaseMutex();
	}

	// Token: 0x060002F3 RID: 755 RVA: 0x0002D9F8 File Offset: 0x0002BBF8
	private void OnWorldConfig(ref string msg)
	{
		WorldSize worldSize = JsonUtility.FromJson<WorldSize>(msg);
		if (worldSize.name == null)
		{
			worldSize.name = "barsoom";
		}
		UnityEngine.Debug.Log(string.Concat(new object[]
		{
			worldSize.width,
			"x",
			worldSize.height,
			" - in ",
			worldSize.name
		}));
		ConnectionManager.THIS.connectionText.text = "";
		UnityEngine.Debug.Log(this.VERSION);
		if (!WorldInitScript.inited)
		{
			string text = this.obvyazka.vk_access_token2;
			if (text == "")
			{
				text = SystemInfo.deviceUniqueIdentifier;
			}
			ServerTime.THIS.SendTypicalMessage(-1, "Rndm", 0, 0, "hash=" + text);
			if (!ConnectionManager.THIS.DEBUG)
			{
				WorldInitScript.SetupExceptionHandling();
			}
			SoundManager.THIS.PlayMusic();
			CellModel.Init();
			BzScript.Init();
			SkillButtonScript.InitColors();
			ClanSpriteScript.Init();
			WorldInitScript.inited = true;
			MapModel map = new MapModel(worldSize.width, worldSize.height, worldSize.name);
			InventoryItem.InitSprites();
			ProgAction.InitSprites();
			PackSpriteScript.InitSprites();
			this.mainRenderer.GetComponent<TerrainRendererScript>().SetMaps(map);
			ClientController.map = map;
			this.FPSText.gameObject.SetActive(true);
			this.obvyazka.GetComponent<ServerController>().Init();
			ServerTime.THIS.SendTypicalMessage(-1, "Miss", 0, 0, "0");
			ServerTime.THIS.SendTypicalMessage(-1, "Chin", 0, 0, "_");
		}
		else
		{
			PopupManager.THIS.CloseWindow();
			this.mainRenderer.GetComponent<RobotRenderer>().RemoveAll();
			GUIManager.THIS.CloseInventoryItem();
			ServerTime.THIS.SendTypicalMessage(-1, "Miss", 0, 0, "1");
			ServerTime.THIS.SendTypicalMessage(-1, "Chin", 0, 0, "1:" + ChatManager.THIS.getCurrentChat() + ":" + ChatManager.THIS.getLasts());
		}
		this.pad.gameObject.SetActive(false);
	}

	// Token: 0x060002F4 RID: 756 RVA: 0x0002DC30 File Offset: 0x0002BE30
	private void Update()
	{
		if (this.updatingStarted)
		{
			this.UpdatingMutex.WaitOne();
			if (this.updateProgress)
			{
				this.updateProgress = false;
				this.UpdateText.text = "СКАЧИВАЕМ ОБНОВЛЕНИЕ - " + this.downloadedPercent + "%";
			}
			if (this.launchUpdater)
			{
				this.launchUpdater = false;
				this.UpdateText.text = "ОБНОВЛЯЕМ!                      ";
				this.wb.Dispose();
				WorldInitScript.saved = true;
				WorldInitScript.saving = true;
				Process.Start(this.downloadedFilename, "/SP- /SILENT /NOICONS");
				this.UpdatingMutex.ReleaseMutex();
				Application.Quit();
				return;
			}
			this.UpdatingMutex.ReleaseMutex();
		}
		if (WorldInitScript.saving || this.updatingStarted)
		{
			return;
		}
		if (Time.unscaledTime > this.lastLoadingMapTime + 1f)
		{
			if (ClientController.map != null)
			{
				ClientController.map.LoadBlocks();
			}
			this.lastLoadingMapTime = Time.unscaledTime;
		}
		if (Time.unscaledTime > this.lastSavingMapTime + 60f)
		{
			if (ClientController.map != null)
			{
				ClientController.map.SaveMapV2();
			}
			this.lastSavingMapTime = Time.unscaledTime;
		}
	}

	// Token: 0x060002F5 RID: 757 RVA: 0x00006D80 File Offset: 0x00004F80
	private void SaveOnQuit()
	{
		if (ClientController.map != null)
		{
			ClientController.map.SaveMapV2();
			ClientController.map.Destroy();
		}
		WorldInitScript.THIS.Invoke("QuitAfterSaving", 0.5f);
	}

	// Token: 0x060002F6 RID: 758 RVA: 0x0002DD5C File Offset: 0x0002BF5C
	private static bool WantsToQuit()
	{
		if (!WorldInitScript.saved)
		{
			string text = "СОХРАНЕНИЕ КАРТЫ.\nДОЖДИТЕСЬ ЗАВЕРШЕНИЯ ПРИЛОЖЕНИЯ";
			ConnectionManager.THIS.connectionText.text = text;
			WorldInitScript.THIS.Invoke("SaveOnQuit", 0.2f);
			WorldInitScript.saving = true;
			return false;
		}
		return !WorldInitScript.saving || WorldInitScript.saved;
	}

	// Token: 0x060002F7 RID: 759 RVA: 0x00006DB1 File Offset: 0x00004FB1
	[RuntimeInitializeOnLoadMethod]
	private static void RunOnStart()
	{
		Application.wantsToQuit += WorldInitScript.WantsToQuit;
	}

	// Token: 0x060002F8 RID: 760 RVA: 0x00006DC4 File Offset: 0x00004FC4
	private void QuitAfterSaving()
	{
		WorldInitScript.saved = true;
		Application.Quit();
	}

	// Token: 0x04000582 RID: 1410
	private Obvyazka obvyazka;

	// Token: 0x04000583 RID: 1411
	public GameObject mainRenderer;

	// Token: 0x04000584 RID: 1412
	public Text FPSText;

	// Token: 0x04000585 RID: 1413
	public Image pad;

	// Token: 0x04000586 RID: 1414
	public Canvas canvas;

	// Token: 0x04000587 RID: 1415
	public GameObject UpdateWindow;

	// Token: 0x04000588 RID: 1416
	public Text UpdateText;

	// Token: 0x04000589 RID: 1417
	public Button UpdateButton;

	// Token: 0x0400058A RID: 1418
	public static WorldInitScript THIS;

	// Token: 0x0400058B RID: 1419
	public float lastSavingMapTime;

	// Token: 0x0400058C RID: 1420
	public float lastLoadingMapTime;

	// Token: 0x0400058D RID: 1421
	public static bool ignoreConfig;

	// Token: 0x0400058E RID: 1422
	public static bool inited;

	// Token: 0x0400058F RID: 1423
	private static bool isExceptionHandlingSetup;

	// Token: 0x04000590 RID: 1424
	private int VERSION = 3403;

	// Token: 0x04000591 RID: 1425
	private bool updatingStarted;

	// Token: 0x04000592 RID: 1426
	private WebClient wb = new WebClient();

	// Token: 0x04000593 RID: 1427
	private int downloadedPercent;

	// Token: 0x04000594 RID: 1428
	private bool updateProgress;

	// Token: 0x04000595 RID: 1429
	private bool launchUpdater;

	// Token: 0x04000596 RID: 1430
	private Mutex UpdatingMutex = new Mutex();

	// Token: 0x04000597 RID: 1431
	private string downloadedFilename;

	// Token: 0x04000598 RID: 1432
	private static bool saving;

	// Token: 0x04000599 RID: 1433
	private static bool saved;
}
