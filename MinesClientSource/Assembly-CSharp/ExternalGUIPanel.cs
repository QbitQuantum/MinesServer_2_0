using System;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

// Token: 0x020000E0 RID: 224
public class ExternalGUIPanel : MonoBehaviour
{
	// Token: 0x060005BA RID: 1466 RVA: 0x00008A43 File Offset: 0x00006C43
	public void Show()
	{
		this.isVisible = true;
	}

	// Token: 0x060005BB RID: 1467 RVA: 0x00008A4C File Offset: 0x00006C4C
	public void Hide()
	{
		this.isVisible = false;
	}

	// Token: 0x060005BC RID: 1468 RVA: 0x00037608 File Offset: 0x00035808
	private void OnDestroy()
	{
		this.DestroyTexture(this.solidBackground);
		this.DestroyStyleTexture(this.boxStyle);
		this.DestroyStyleTexture(this.infoBoxStyle);
		this.DestroyStyleTexture(this.cachedSelectedStyle);
		this.DestroyStyleTexture(this.cachedAlternateStyle);
		this.DestroyStyleTexture(this.cachedSeparatorStyle);
		if (this.autoFollow != null && this.autoFollow.IsFollowing())
		{
			this.autoFollow.ToggleFollow();
		}
	}

	// Token: 0x060005BE RID: 1470 RVA: 0x00008A55 File Offset: 0x00006C55
	public void RemoveBot(int id)
	{
		if (this.bots.ContainsKey(id))
		{
			this.bots.Remove(id);
			if (this.selectedBotId == id)
			{
				this.selectedBotId = -1;
			}
		}
	}

	// Token: 0x060005BF RID: 1471 RVA: 0x00037744 File Offset: 0x00035944
	public List<int> GetSelectedBotIds()
	{
		List<int> list = new List<int>();
		if (this.selectedBotId != -1 && this.bots.ContainsKey(this.selectedBotId))
		{
			list.Add(this.selectedBotId);
		}
		return list;
	}

	// Token: 0x060005C0 RID: 1472 RVA: 0x00037784 File Offset: 0x00035984
	public List<string> GetSelectedBotNicks()
	{
		List<string> list = new List<string>();
		if (this.selectedBotId != -1 && this.bots.ContainsKey(this.selectedBotId))
		{
			list.Add(this.bots[this.selectedBotId]);
		}
		return list;
	}

	// Token: 0x060005C1 RID: 1473 RVA: 0x00008A84 File Offset: 0x00006C84
	public void ClearAllBots()
	{
		this.bots.Clear();
		this.selectedBotId = -1;
	}

	// Token: 0x060005C2 RID: 1474 RVA: 0x00008A98 File Offset: 0x00006C98
	public void AddBot(int id, string nick)
	{
		if (!this.bots.ContainsKey(id))
		{
			this.bots[id] = nick;
		}
	}

	// Token: 0x060005C3 RID: 1475 RVA: 0x000377D0 File Offset: 0x000359D0
	private void Start()
	{
		if (ClientController.THIS != null)
		{
			this.autoFollow = ClientController.THIS.gameObject.GetComponent<AutoFollow>();
			if (this.autoFollow == null)
			{
				this.autoFollow = ClientController.THIS.gameObject.AddComponent<AutoFollow>();
			}
		}
		this.InitializeStyles();
		this.lastCheckTimeValue = Time.time;
	}

	// Token: 0x060005C4 RID: 1476 RVA: 0x00037834 File Offset: 0x00035A34
	private void OnGUI()
	{
		if (this.isVisible)
		{
			if (Event.current.type == EventType.Layout)
			{
				this.windowRect.x = Mathf.Clamp(this.windowRect.x, 0f, (float)Screen.width - this.windowRect.width);
				this.windowRect.y = Mathf.Clamp(this.windowRect.y, 0f, (float)Screen.height - this.windowRect.height);
			}
			this.windowRect = GUI.Window(123456, this.windowRect, new GUI.WindowFunction(this.DrawWindow), "ASPIRE MOD v3.1");
		}
	}

	// Token: 0x060005C5 RID: 1477 RVA: 0x000378EC File Offset: 0x00035AEC
	private void DrawWindow(int windowID)
	{
		GUI.DrawTexture(new Rect(0f, 0f, this.windowRect.width, this.windowRect.height), this.GetSolidBackground());
		GUILayout.Space(5f);
		this.currentPanel = GUILayout.Toolbar(this.currentPanel, this.panelNames, new GUILayoutOption[]
		{
			GUILayout.Height(40f)
		});
		GUILayout.Space(10f);
		this.DrawSeparator();
		GUILayout.Space(10f);
		switch (this.currentPanel)
		{
		case 0:
			this.DrawBotManagerPanel();
			break;
		case 1:
			this.DrawProgramsPanel();
			break;
		case 2:
			this.DrawCheckerPanel();
			break;
		}
		GUI.DragWindow(new Rect(0f, 0f, this.windowRect.width, 35f));
	}

	// Token: 0x060005C6 RID: 1478 RVA: 0x00008AB8 File Offset: 0x00006CB8
	private void DrawSeparator()
	{
		GUI.Box(GUILayoutUtility.GetRect(350f, 3f), "", this.GetSeparatorStyle());
	}

	// Token: 0x060005C7 RID: 1479 RVA: 0x000379CC File Offset: 0x00035BCC
	private GUIStyle GetSelectedStyle()
	{
		if (this.cachedSelectedStyle == null)
		{
			this.cachedSelectedStyle = new GUIStyle(GUI.skin.box);
			this.cachedSelectedStyle.normal.background = this.CreateSolidTexture(new Color(0.25f, 0.45f, 0.8f, 1f));
		}
		return this.cachedSelectedStyle;
	}

	// Token: 0x060005C8 RID: 1480 RVA: 0x00037A30 File Offset: 0x00035C30
	private GUIStyle GetAlternateStyle()
	{
		if (this.cachedAlternateStyle == null)
		{
			this.cachedAlternateStyle = new GUIStyle(GUI.skin.box);
			this.cachedAlternateStyle.normal.background = this.CreateSolidTexture(new Color(0.15f, 0.15f, 0.2f, 1f));
		}
		return this.cachedAlternateStyle;
	}

	// Token: 0x060005C9 RID: 1481 RVA: 0x00037A94 File Offset: 0x00035C94
	private GUIStyle GetSeparatorStyle()
	{
		if (this.cachedSeparatorStyle == null)
		{
			this.cachedSeparatorStyle = new GUIStyle(GUI.skin.box);
			this.cachedSeparatorStyle.normal.background = this.CreateSolidTexture(new Color(1f, 0.5f, 0f, 1f));
		}
		return this.cachedSeparatorStyle;
	}

	// Token: 0x060005CA RID: 1482 RVA: 0x00008AD9 File Offset: 0x00006CD9
	private void FollowSelectedBot()
	{
		if (this.selectedBotId != -1 && this.autoFollow != null)
		{
			this.autoFollow.SetTargetBotId(this.selectedBotId);
			this.autoFollow.ToggleFollow();
		}
	}

	// Token: 0x060005CB RID: 1483 RVA: 0x00008B11 File Offset: 0x00006D11
	private void StopFollow()
	{
		if (this.autoFollow != null && this.autoFollow.IsFollowing())
		{
			this.autoFollow.ToggleFollow();
		}
	}

	// Token: 0x060005CC RID: 1484 RVA: 0x00037AF8 File Offset: 0x00035CF8
	private void DrawBotManagerPanel()
	{
		GUIStyle style = this.GetHeaderStyle();
		GUILayout.Label("СПИСОК БОТОВ:", style, Array.Empty<GUILayoutOption>());
		GUILayout.Space(5f);
		Rect rect = GUILayoutUtility.GetRect(350f, 450f);
		GUI.Box(rect, GUIContent.none, this.GetBoxStyle());
		this.DrawBotsList(rect);
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUI.enabled = (this.selectedBotId != -1);
		if (GUILayout.Button("СЛЕДОВАТЬ", new GUILayoutOption[]
		{
			GUILayout.Height(40f),
			GUILayout.Width(110f)
		}))
		{
			this.FollowSelectedBot();
		}
		GUI.enabled = (this.autoFollow != null && this.autoFollow.IsFollowing());
		if (GUILayout.Button("СТОП", new GUILayoutOption[]
		{
			GUILayout.Height(40f),
			GUILayout.Width(110f)
		}))
		{
			this.StopFollow();
		}
		GUI.enabled = true;
		if (GUILayout.Button("ОЧИСТИТЬ", new GUILayoutOption[]
		{
			GUILayout.Height(40f),
			GUILayout.Width(110f)
		}))
		{
			this.ClearAllBots();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		this.DrawStatusPanel();
	}

	// Token: 0x060005CD RID: 1485 RVA: 0x00037C48 File Offset: 0x00035E48
	private void DrawProgramsPanel()
	{
		GUIStyle labelStyle = this.GetLabelStyle();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Toggle(false, "ТРАМВАЙ", new GUILayoutOption[]
		{
			GUILayout.Width(130f),
			GUILayout.Height(35f)
		});
		GUILayout.Label("Автоматическое движение по маршруту", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Toggle(false, "ВБ-ШЕР", new GUILayoutOption[]
		{
			GUILayout.Width(130f),
			GUILayout.Height(35f)
		});
		GUILayout.Label("Сбор выброшенных предметов", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Toggle(false, "РАЗРЯДКА ТП", new GUILayoutOption[]
		{
			GUILayout.Width(130f),
			GUILayout.Height(35f)
		});
		GUILayout.Label("Автоматическая разрядка ТП", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Toggle(false, "СБОР ЖИВНОСТИ", new GUILayoutOption[]
		{
			GUILayout.Width(130f),
			GUILayout.Height(35f)
		});
		GUILayout.Label("Автоматический сбор ресурсов", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Space(25f);
		GUI.Box(GUILayoutUtility.GetRect(350f, 100f), GUIContent.none, this.GetInfoBoxStyle());
		GUILayout.Label("Активные программы:", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Label("• Нет активных программ", labelStyle, Array.Empty<GUILayoutOption>());
		GUILayout.Space(15f);
		if (GUILayout.Button("ЗАПУСТИТЬ ВСЕ ПРОГРАММЫ", new GUILayoutOption[]
		{
			GUILayout.Height(45f)
		}))
		{
			this.StartAllPrograms();
		}
	}

	// Token: 0x060005CE RID: 1486 RVA: 0x00037E28 File Offset: 0x00036028
	private void DrawBotsList(Rect listRect)
	{
		GUIStyle labelStyle = this.GetLabelStyle();
		labelStyle.padding = new RectOffset(5, 5, 5, 5);
		GUIStyle selectedStyle = this.GetSelectedStyle();
		GUIStyle alternateStyle = this.GetAlternateStyle();
		float num = 40f;
		float height = Mathf.Max(listRect.height, (float)this.bots.Count * num);
		this.scrollPosition = GUI.BeginScrollView(listRect, this.scrollPosition, new Rect(0f, 0f, listRect.width - 20f, height));
		int num2 = 0;
		foreach (KeyValuePair<int, string> keyValuePair in this.bots)
		{
			float num3 = (float)num2 * num;
			Rect position = new Rect(5f, num3, listRect.width - 25f, 35f);
			if (this.selectedBotId == keyValuePair.Key)
			{
				GUI.Box(position, "", selectedStyle);
			}
			else if (num2 % 2 == 0)
			{
				GUI.Box(position, "", alternateStyle);
			}
			if (GUI.Toggle(new Rect(12f, num3 + 7f, 22f, 22f), this.selectedBotId == keyValuePair.Key, "") && this.selectedBotId != keyValuePair.Key)
			{
				this.selectedBotId = keyValuePair.Key;
			}
			GUI.Label(new Rect(42f, num3 + 7f, position.width - 50f, 25f), string.Format("{0}: {1}", keyValuePair.Key, keyValuePair.Value), labelStyle);
			num2++;
		}
		GUI.EndScrollView();
	}

	// Token: 0x060005CF RID: 1487 RVA: 0x0003800C File Offset: 0x0003620C
	private void DrawStatusPanel()
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.label)
		{
			fontSize = 12,
			alignment = TextAnchor.MiddleCenter
		};
		GUI.Box(GUILayoutUtility.GetRect(350f, 50f), GUIContent.none, this.GetInfoBoxStyle());
		if (this.autoFollow != null && this.autoFollow.IsFollowing())
		{
			guistyle.normal.textColor = Color.green;
			GUILayout.Label("▶ СЛЕДОВАНИЕ АКТИВНО", guistyle, Array.Empty<GUILayoutOption>());
			return;
		}
		if (this.selectedBotId != -1 && this.bots.ContainsKey(this.selectedBotId))
		{
			guistyle.normal.textColor = Color.yellow;
			GUILayout.Label(string.Format("✓ ВЫБРАН: {0}: {1}", this.selectedBotId, this.bots[this.selectedBotId]), guistyle, Array.Empty<GUILayoutOption>());
			return;
		}
		guistyle.normal.textColor = Color.gray;
		GUILayout.Label("○ НЕТ ВЫБРАННЫХ БОТОВ", guistyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x060005D0 RID: 1488 RVA: 0x00008B3C File Offset: 0x00006D3C
	private Texture2D GetSolidBackground()
	{
		if (this.solidBackground == null)
		{
			this.solidBackground = this.CreateSolidTexture(new Color(0.08f, 0.08f, 0.12f, 1f));
		}
		return this.solidBackground;
	}

	// Token: 0x060005D1 RID: 1489 RVA: 0x00038118 File Offset: 0x00036318
	private GUIStyle GetBoxStyle()
	{
		if (this.boxStyle == null)
		{
			this.boxStyle = new GUIStyle(GUI.skin.box);
			this.boxStyle.normal.background = this.CreateSolidTexture(new Color(0.12f, 0.12f, 0.18f, 1f));
			this.boxStyle.normal.textColor = Color.white;
		}
		return this.boxStyle;
	}

	// Token: 0x060005D2 RID: 1490 RVA: 0x00038190 File Offset: 0x00036390
	private GUIStyle GetInfoBoxStyle()
	{
		if (this.infoBoxStyle == null)
		{
			this.infoBoxStyle = new GUIStyle(GUI.skin.box);
			this.infoBoxStyle.normal.background = this.CreateSolidTexture(new Color(0.05f, 0.05f, 0.08f, 1f));
			this.infoBoxStyle.padding = new RectOffset(10, 10, 10, 10);
		}
		return this.infoBoxStyle;
	}

	// Token: 0x060005D3 RID: 1491 RVA: 0x00008B77 File Offset: 0x00006D77
	private void StartAllPrograms()
	{
		Debug.Log("[Programs] Все программы запущены");
	}

	// Token: 0x060005D4 RID: 1492 RVA: 0x0003820C File Offset: 0x0003640C
	private void Awake()
	{
		if (!this.windowRectInitialized)
		{
			this.windowRect = new Rect((float)Screen.width - 400f, (float)(Screen.height / 2) - 350f, 380f, 700f);
			this.windowRectInitialized = true;
		}
	}

	// Token: 0x060005D5 RID: 1493 RVA: 0x00008B83 File Offset: 0x00006D83
	private void InitializeStyles()
	{
		if (!this.stylesInitialized)
		{
			this.GetBoxStyle();
			this.GetInfoBoxStyle();
			this.GetLabelStyle();
			this.GetHeaderStyle();
			this.stylesInitialized = true;
		}
	}

	// Token: 0x060005D6 RID: 1494 RVA: 0x00008BB3 File Offset: 0x00006DB3
	private Texture2D CreateSolidTexture(Color color)
	{
		Texture2D texture2D = new Texture2D(1, 1);
		texture2D.SetPixel(0, 0, color);
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x060005D7 RID: 1495 RVA: 0x00008BCB File Offset: 0x00006DCB
	private void DestroyTexture(Texture2D texture)
	{
		if (texture != null)
		{
			UnityEngine.Object.Destroy(texture);
		}
	}

	// Token: 0x060005D8 RID: 1496 RVA: 0x00008BDC File Offset: 0x00006DDC
	private void DestroyStyleTexture(GUIStyle style)
	{
		if (((style != null) ? style.normal.background : null) != null)
		{
			UnityEngine.Object.Destroy(style.normal.background);
			style.normal.background = null;
		}
	}

	// Token: 0x060005D9 RID: 1497 RVA: 0x0003825C File Offset: 0x0003645C
	private GUIStyle GetLabelStyle()
	{
		if (this.cachedLabelStyle == null)
		{
			this.cachedLabelStyle = new GUIStyle(GUI.skin.label);
			this.cachedLabelStyle.fontSize = 13;
			this.cachedLabelStyle.normal.textColor = Color.white;
		}
		return this.cachedLabelStyle;
	}

	// Token: 0x060005DA RID: 1498 RVA: 0x000382B4 File Offset: 0x000364B4
	private void DrawCheckerPanel()
	{
		GUIStyle labelStyle = this.GetLabelStyle();
		GUIStyle style = this.GetHeaderStyle();
		GUIStyle guistyle = new GUIStyle(labelStyle)
		{
			normal = 
			{
				textColor = Color.red
			}
		};
		GUIStyle guistyle2 = new GUIStyle(labelStyle)
		{
			normal = 
			{
				textColor = Color.green
			}
		};
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		this.checkerConfig.isEnabled = GUILayout.Toggle(this.checkerConfig.isEnabled, " ЧЕКЕР АКТИВЕН", new GUILayoutOption[]
		{
			GUILayout.Width(150f),
			GUILayout.Height(35f)
		});
		GUILayout.Label("Статус: " + (this.checkerConfig.isEnabled ? "ВКЛ" : "ВЫКЛ"), this.checkerConfig.isEnabled ? guistyle2 : guistyle, Array.Empty<GUILayoutOption>());
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		this.showSettingsPanel = GUILayout.Toggle(this.showSettingsPanel, " НАСТРОЙКИ ПОРОГОВ", new GUILayoutOption[]
		{
			GUILayout.Height(30f)
		});
		if (this.showSettingsPanel)
		{
			GUILayout.Space(10f);
			GUILayout.Label("=== ТП ===", style, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Порог заряда:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.tpChargeThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.tpChargeThreshold);
			GUILayout.Label("Порог ХП:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.tpHealthThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.tpHealthThreshold);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Label("=== ПУШКИ ===", style, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Порог заряда:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.weaponChargeThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.weaponChargeThreshold);
			GUILayout.Label("Порог ХП:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.weaponHealthThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.weaponHealthThreshold);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Label("=== КРАФТ ===", style, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Порог ХП:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.craftHealthThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.craftHealthThreshold);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Label("=== РЕСПАВН ===", style, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Порог ХП:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.respawnHealthThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.respawnHealthThreshold);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
			GUILayout.Label("=== МАРКЕТ ===", style, Array.Empty<GUILayoutOption>());
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Порог ХП:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			int.TryParse(GUILayout.TextField(this.checkerConfig.marketHealthThreshold.ToString(), new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			}), out this.checkerConfig.marketHealthThreshold);
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Интервал проверки (сек):", labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(150f)
		});
		float.TryParse(GUILayout.TextField(this.checkInterval.ToString(), new GUILayoutOption[]
		{
			GUILayout.Width(60f)
		}), out this.checkInterval);
		if (this.checkInterval < 2f)
		{
			this.checkInterval = 2f;
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		string[] texts = new string[]
		{
			"СПИСОК ПАКОВ",
			"ДОБАВИТЬ ПАК",
			"ИСТОРИЯ"
		};
		int num = GUILayout.Toolbar(0, texts, new GUILayoutOption[]
		{
			GUILayout.Height(35f)
		});
		GUILayout.Space(10f);
		switch (num)
		{
		case 0:
			this.DrawPacksList();
			break;
		case 1:
			this.DrawAddPackPanel();
			break;
		case 2:
			this.DrawHistoryPanel();
			break;
		}
		GUILayout.Space(15f);
		if (GUILayout.Button("ПРОВЕРИТЬ СЕЙЧАС", new GUILayoutOption[]
		{
			GUILayout.Height(45f)
		}))
		{
			this.RunChecker();
		}
		GUILayout.Space(10f);
		GUI.Box(GUILayoutUtility.GetRect(350f, 90f), GUIContent.none, this.GetInfoBoxStyle());
		if (!string.IsNullOrEmpty(this.lastCheckStatus))
		{
			GUILayout.Label(this.lastCheckStatus, this.lastCheckHasError ? guistyle : guistyle2, Array.Empty<GUILayoutOption>());
			return;
		}
		GUILayout.Label("Ожидание проверки...", labelStyle, Array.Empty<GUILayoutOption>());
	}

	// Token: 0x060005DB RID: 1499 RVA: 0x00038894 File Offset: 0x00036A94
	private void SaveConfig()
	{
		try
		{
			File.WriteAllText(this.configPath, JsonUtility.ToJson(this.checkerConfig, true));
			Debug.Log("[Чекер] Конфигурация сохранена");
		}
		catch (Exception ex)
		{
			Debug.LogError("[Чекер] Ошибка сохранения: " + ex.Message);
		}
	}

	// Token: 0x060005DC RID: 1500 RVA: 0x000388EC File Offset: 0x00036AEC
	private void LoadConfig()
	{
		try
		{
			if (File.Exists(this.configPath))
			{
				this.checkerConfig = JsonUtility.FromJson<CheckerConfig>(File.ReadAllText(this.configPath));
				Debug.Log("[Чекер] Конфигурация загружена");
			}
			else
			{
				this.checkerConfig = new CheckerConfig();
				this.SaveConfig();
			}
		}
		catch (Exception ex)
		{
			Debug.LogError("[Чекер] Ошибка загрузки: " + ex.Message);
			this.checkerConfig = new CheckerConfig();
		}
	}

	// Token: 0x060005DD RID: 1501 RVA: 0x00008C13 File Offset: 0x00006E13
	public void ClearNotificationHistory()
	{
		this.checkerConfig.notificationHistory.Clear();
		this.SaveConfig();
	}

	// Token: 0x060005DE RID: 1502 RVA: 0x00038970 File Offset: 0x00036B70
	private void AddToHistory(string message)
	{
		this.checkerConfig.notificationHistory.Insert(0, string.Format("[{0:HH:mm:ss}] {1}", DateTime.Now, message));
		while (this.checkerConfig.notificationHistory.Count > 100)
		{
			this.checkerConfig.notificationHistory.RemoveAt(this.checkerConfig.notificationHistory.Count - 1);
		}
		this.SaveConfig();
	}

	// Token: 0x060005DF RID: 1503 RVA: 0x000389E4 File Offset: 0x00036BE4
	public void AddPack(string name, PackType type, int maxHp, int maxCharge = -1)
	{
		PackData packData = this.checkerConfig.packs.Find((PackData p) => p.name.Equals(name, StringComparison.OrdinalIgnoreCase));
		if (packData != null)
		{
			this.AddToHistory("Проверка пака '" + name + "' при попытке добавления...");
			if (packData.maxHp > 0)
			{
				if (packData.currentHp <= 0)
				{
					string name2 = packData.name;
					this.checkerConfig.packs.Remove(packData);
					this.SaveConfig();
					this.AddToHistory("\ud83d\uddd1️ Пак '" + name2 + "' УДАЛЕН (разрушен, ХП = 0)");
					this.SendNotification("Пак '" + name2 + "' удален из-за разрушения");
					return;
				}
				if (packData.currentHp < packData.hpThreshold)
				{
					this.SendNotification(string.Format("{0}: ХП упало до {1} (порог {2})", packData.name, packData.currentHp, packData.hpThreshold));
					this.AddToHistory(string.Format("{0}: ХП={1} (ниже порога {2})", packData.name, packData.currentHp, packData.hpThreshold));
					if (packData.currentHp <= 0)
					{
						this.SendNotification(packData.name + ": ПОЛНОСТЬЮ УНИЧТОЖЕН!");
						this.AddToHistory(packData.name + ": ПОЛНОСТЬЮ УНИЧТОЖЕН!");
					}
				}
			}
			if (packData.maxCharge >= 0 && packData.currentCharge < packData.chargeThreshold)
			{
				this.SendNotification(string.Format("{0}: Заряд упал до {1} (порог {2})", packData.name, packData.currentCharge, packData.chargeThreshold));
				this.AddToHistory(string.Format("{0}: Заряд={1} (ниже порога {2})", packData.name, packData.currentCharge, packData.chargeThreshold));
				if (packData.type == PackType.TP && packData.currentCharge <= 0 && this.checkerConfig.tpZeroNotify)
				{
					this.SendNotification("ТП: ПОЛНОСТЬЮ РАЗРЯЖЕНА!");
					this.AddToHistory("ТП: ПОЛНОСТЬЮ РАЗРЯЖЕНА!");
				}
			}
			this.AddToHistory("Проверка пака '" + name + "' завершена");
			return;
		}
		PackData packData2 = new PackData
		{
			name = name,
			type = type,
			maxHp = maxHp,
			currentHp = maxHp,
			maxCharge = maxCharge,
			currentCharge = maxCharge
		};
		switch (type)
		{
		case PackType.TP:
			packData2.hpThreshold = this.checkerConfig.tpHealthThreshold;
			packData2.chargeThreshold = this.checkerConfig.tpChargeThreshold;
			break;
		case PackType.Craft:
			packData2.hpThreshold = this.checkerConfig.craftHealthThreshold;
			break;
		case PackType.Weapon:
			packData2.hpThreshold = this.checkerConfig.weaponHealthThreshold;
			packData2.chargeThreshold = this.checkerConfig.weaponChargeThreshold;
			break;
		case PackType.Respawn:
			packData2.hpThreshold = this.checkerConfig.respawnHealthThreshold;
			break;
		case PackType.Market:
			packData2.hpThreshold = this.checkerConfig.marketHealthThreshold;
			break;
		}
		this.checkerConfig.packs.Add(packData2);
		this.SaveConfig();
		this.AddToHistory(string.Format("✓ Пак '{0}' успешно добавлен (Тип: {1}, ХП: {2}, Заряд: {3})", new object[]
		{
			name,
			type,
			maxHp,
			(maxCharge >= 0) ? maxCharge.ToString() : "Нет"
		}));
		bool flag = maxHp > 0 && maxHp < packData2.hpThreshold;
		bool flag2 = maxCharge >= 0 && maxCharge < packData2.chargeThreshold;
		if (flag || flag2)
		{
			string text = "";
			if (flag)
			{
				text += string.Format("ХП={0} (ниже порога {1})", maxHp, packData2.hpThreshold);
			}
			if (flag2)
			{
				if (!string.IsNullOrEmpty(text))
				{
					text += ", ";
				}
				text += string.Format("Заряд={0} (ниже порога {1})", maxCharge, packData2.chargeThreshold);
			}
			this.SendNotification(name + ": ВНИМАНИЕ! " + text);
			this.AddToHistory(name + ": добавлен с критическими значениями - " + text);
		}
		Debug.Log(string.Format("[Чекер] Добавлен пак: {0} (Тип: {1}, ХП: {2}, Заряд: {3})", new object[]
		{
			name,
			type,
			maxHp,
			(maxCharge >= 0) ? maxCharge.ToString() : "Нет"
		}));
	}

	// Token: 0x060005E0 RID: 1504 RVA: 0x00038E60 File Offset: 0x00037060
	public void UpdatePackData(string packId, int currentHp, int currentCharge = -1)
	{
		PackData packData = this.checkerConfig.packs.Find((PackData p) => p.id == packId);
		if (packData != null)
		{
			packData.currentHp = currentHp;
			if (currentCharge >= 0 && packData.maxCharge >= 0)
			{
				packData.currentCharge = currentCharge;
			}
		}
	}

	// Token: 0x060005E1 RID: 1505 RVA: 0x00038EC0 File Offset: 0x000370C0
	public void RemovePack(string packId)
	{
		this.checkerConfig.packs.RemoveAll((PackData p) => p.id == packId);
		this.SaveConfig();
	}

	// Token: 0x060005E2 RID: 1506 RVA: 0x00038F00 File Offset: 0x00037100
	private void DrawPacksList()
	{
		GUIStyle labelStyle = this.GetLabelStyle();
		GUIStyle selectedStyle = this.GetSelectedStyle();
		GUIStyle alternateStyle = this.GetAlternateStyle();
		GUILayout.Label(string.Format("ВСЕГО ПАКОВ: {0}", this.checkerConfig.packs.Count), this.GetHeaderStyle(), Array.Empty<GUILayoutOption>());
		GUILayout.Space(5f);
		if (this.checkerConfig.packs.Count == 0)
		{
			GUILayout.Label("Нет добавленных паков", labelStyle, Array.Empty<GUILayoutOption>());
			return;
		}
		float height = Mathf.Min(320f, (float)this.checkerConfig.packs.Count * 70f);
		Rect rect = GUILayoutUtility.GetRect(350f, height);
		float height2 = (float)this.checkerConfig.packs.Count * 70f;
		this.packsScrollPosition = GUI.BeginScrollView(rect, this.packsScrollPosition, new Rect(0f, 0f, rect.width - 20f, height2));
		for (int i = 0; i < this.checkerConfig.packs.Count; i++)
		{
			PackData packData = this.checkerConfig.packs[i];
			float num = (float)i * 70f;
			Rect position = new Rect(5f, num, rect.width - 25f, 65f);
			if (this.selectedPackIndex == i)
			{
				GUI.Box(position, "", selectedStyle);
			}
			else if (i % 2 == 0)
			{
				GUI.Box(position, "", alternateStyle);
			}
			GUILayout.BeginArea(new Rect(12f, num + 5f, position.width - 15f, 60f));
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label(packData.name, this.GetHeaderStyle(), new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			string str = packData.type.ToString();
			GUILayout.Label("[" + str + "]", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(60f)
			});
			if (packData.maxHp > 0)
			{
				GUILayout.Label(string.Format("ХП: {0}/{1}", packData.currentHp, packData.maxHp), labelStyle, new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
			}
			if (packData.maxCharge >= 0)
			{
				GUILayout.Label(string.Format("Заряд: {0}/{1}", packData.currentCharge, packData.maxCharge), labelStyle, new GUILayoutOption[]
				{
					GUILayout.Width(100f)
				});
			}
			GUILayout.EndHorizontal();
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			packData.isEnabled = GUILayout.Toggle(packData.isEnabled, " Проверять", new GUILayoutOption[]
			{
				GUILayout.Width(100f)
			});
			if (packData.maxHp > 0)
			{
				GUILayout.Label("Порог ХП:", labelStyle, new GUILayoutOption[]
				{
					GUILayout.Width(70f)
				});
				int.TryParse(GUILayout.TextField(packData.hpThreshold.ToString(), new GUILayoutOption[]
				{
					GUILayout.Width(50f)
				}), out packData.hpThreshold);
			}
			if (packData.maxCharge >= 0)
			{
				GUILayout.Label("Порог заряда:", labelStyle, new GUILayoutOption[]
				{
					GUILayout.Width(90f)
				});
				int.TryParse(GUILayout.TextField(packData.chargeThreshold.ToString(), new GUILayoutOption[]
				{
					GUILayout.Width(50f)
				}), out packData.chargeThreshold);
			}
			if (GUILayout.Button("Удалить", new GUILayoutOption[]
			{
				GUILayout.Width(70f)
			}))
			{
				this.RemovePack(packData.id);
				if (this.selectedPackIndex >= i)
				{
					this.selectedPackIndex--;
				}
				return;
			}
			GUILayout.EndHorizontal();
			GUILayout.EndArea();
		}
		GUI.EndScrollView();
	}

	// Token: 0x060005E3 RID: 1507 RVA: 0x00039304 File Offset: 0x00037504
	private void DrawAddPackPanel()
	{
		GUIStyle labelStyle = this.GetLabelStyle();
		GUILayout.Label("ДОБАВЛЕНИЕ НОВОГО ПАКА", this.GetHeaderStyle(), Array.Empty<GUILayoutOption>());
		GUILayout.Space(15f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Название:", labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(80f)
		});
		this.newPackName = GUILayout.TextField(this.newPackName, new GUILayoutOption[]
		{
			GUILayout.Width(250f)
		});
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label("Тип пака:", labelStyle, new GUILayoutOption[]
		{
			GUILayout.Width(80f)
		});
		this.newPackType = (PackType)GUILayout.SelectionGrid((int)this.newPackType, new string[]
		{
			"ТП",
			"Крафт",
			"Пушка",
			"Респ",
			"Маркет"
		}, 5, new GUILayoutOption[]
		{
			GUILayout.Height(30f)
		});
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if (this.newPackType > PackType.TP)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Макс. ХП:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			this.newPackHp = GUILayout.TextField(this.newPackHp, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
		if (this.newPackType == PackType.TP || this.newPackType == PackType.Weapon)
		{
			GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
			GUILayout.Label("Макс. заряд:", labelStyle, new GUILayoutOption[]
			{
				GUILayout.Width(80f)
			});
			this.newPackCharge = GUILayout.TextField(this.newPackCharge, new GUILayoutOption[]
			{
				GUILayout.Width(120f)
			});
			GUILayout.EndHorizontal();
			GUILayout.Space(10f);
		}
		GUI.enabled = !string.IsNullOrEmpty(this.newPackName);
		if (GUILayout.Button("ДОБАВИТЬ ПАК", new GUILayoutOption[]
		{
			GUILayout.Height(45f)
		}))
		{
			int maxHp = 100;
			int num = 100;
			if (this.newPackType > PackType.TP)
			{
				int.TryParse(this.newPackHp, out maxHp);
			}
			if (this.newPackType == PackType.TP || this.newPackType == PackType.Weapon)
			{
				int.TryParse(this.newPackCharge, out num);
			}
			this.AddPack(this.newPackName, this.newPackType, maxHp, (this.newPackType == PackType.TP || this.newPackType == PackType.Weapon) ? num : -1);
			this.newPackName = "";
			this.newPackHp = "100";
			this.newPackCharge = "100";
			this.SaveConfig();
		}
		GUI.enabled = true;
	}

	// Token: 0x060005E4 RID: 1508 RVA: 0x000395C0 File Offset: 0x000377C0
	private void DrawHistoryPanel()
	{
		GUIStyle labelStyle = this.GetLabelStyle();
		GUIStyle style = new GUIStyle(labelStyle)
		{
			normal = 
			{
				textColor = Color.yellow
			}
		};
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.Label(string.Format("ИСТОРИЯ УВЕДОМЛЕНИЙ ({0})", this.checkerConfig.notificationHistory.Count), this.GetHeaderStyle(), Array.Empty<GUILayoutOption>());
		if (GUILayout.Button("ОЧИСТИТЬ", new GUILayoutOption[]
		{
			GUILayout.Width(100f),
			GUILayout.Height(30f)
		}))
		{
			this.ClearNotificationHistory();
		}
		GUILayout.EndHorizontal();
		GUILayout.Space(10f);
		if (this.checkerConfig.notificationHistory.Count == 0)
		{
			GUILayout.Label("Нет уведомлений", labelStyle, Array.Empty<GUILayoutOption>());
			return;
		}
		float height = Mathf.Min(320f, (float)this.checkerConfig.notificationHistory.Count * 28f);
		Rect rect = GUILayoutUtility.GetRect(350f, height);
		float height2 = (float)this.checkerConfig.notificationHistory.Count * 28f;
		this.historyScrollPosition = GUI.BeginScrollView(rect, this.historyScrollPosition, new Rect(0f, 0f, rect.width - 20f, height2));
		for (int i = 0; i < this.checkerConfig.notificationHistory.Count; i++)
		{
			GUI.Label(new Rect(8f, (float)i * 28f, rect.width - 30f, 25f), this.checkerConfig.notificationHistory[i], style);
		}
		GUI.EndScrollView();
	}

	// Token: 0x060005E5 RID: 1509 RVA: 0x00008C2B File Offset: 0x00006E2B
	private void RunChecker()
	{
		if (!this.checkerConfig.isEnabled)
		{
			this.lastCheckStatus = "Чекер отключен";
			this.lastCheckHasError = false;
		}
		int shag = AMCheker.Shag;
	}

	// Token: 0x060005E6 RID: 1510 RVA: 0x00008C54 File Offset: 0x00006E54
	private void SendNotification(string message)
	{
		Debug.Log("[ЧЕКЕР УВЕДОМЛЕНИЕ] " + message);
	}

	// Token: 0x060005E7 RID: 1511 RVA: 0x00039764 File Offset: 0x00037964
	private void Update()
	{
		if (this.isVisible && this.checkerConfig.isEnabled && Time.time - this.lastCheckTimeValue >= this.checkInterval)
		{
			this.lastCheckTimeValue = Time.time;
			this.RunChecker();
		}
	}

	// Token: 0x060005E8 RID: 1512 RVA: 0x000397B4 File Offset: 0x000379B4
	private GUIStyle GetHeaderStyle()
	{
		if (this.headerStyle == null)
		{
			this.headerStyle = new GUIStyle(GUI.skin.label);
			this.headerStyle.fontSize = 14;
			this.headerStyle.fontStyle = FontStyle.Bold;
			this.headerStyle.normal.textColor = Color.white;
		}
		return this.headerStyle;
	}

	// Token: 0x060005E9 RID: 1513 RVA: 0x00008C66 File Offset: 0x00006E66
	public bool GetCheckerState()
	{
		return this.checkerConfig.isEnabled;
	}

	// Token: 0x040007CB RID: 1995
	private bool isVisible;

	// Token: 0x040007CC RID: 1996
	private Dictionary<int, string> bots = new Dictionary<int, string>();

	// Token: 0x040007CD RID: 1997
	private int selectedBotId = -1;

	// Token: 0x040007CE RID: 1998
	private AutoFollow autoFollow;

	// Token: 0x040007CF RID: 1999
	private Vector2 scrollPosition = Vector2.zero;

	// Token: 0x040007D0 RID: 2000
	private Rect windowRect;

	// Token: 0x040007D1 RID: 2001
	private int currentPanel;

	// Token: 0x040007D2 RID: 2002
	private readonly string[] panelNames = new string[]
	{
		"BOT MANAGER",
		"ПРОГИ",
		"ЧЕКЕР"
	};

	// Token: 0x040007D3 RID: 2003
	private Texture2D solidBackground;

	// Token: 0x040007D4 RID: 2004
	private GUIStyle boxStyle;

	// Token: 0x040007D5 RID: 2005
	private GUIStyle infoBoxStyle;

	// Token: 0x040007D6 RID: 2006
	private GUIStyle cachedSelectedStyle;

	// Token: 0x040007D7 RID: 2007
	private GUIStyle cachedAlternateStyle;

	// Token: 0x040007D8 RID: 2008
	private GUIStyle cachedSeparatorStyle;

	// Token: 0x040007D9 RID: 2009
	private GUIStyle cachedLabelStyle;

	// Token: 0x040007DA RID: 2010
	private bool stylesInitialized;

	// Token: 0x040007DB RID: 2011
	private bool windowRectInitialized;

	// Token: 0x040007DC RID: 2012
	private CheckerConfig checkerConfig = new CheckerConfig();

	// Token: 0x040007DD RID: 2013
	private string configPath;

	// Token: 0x040007DE RID: 2014
	private float lastCheckTimeValue;

	// Token: 0x040007DF RID: 2015
	private string lastCheckStatus = "";

	// Token: 0x040007E0 RID: 2016
	private bool lastCheckHasError;

	// Token: 0x040007E1 RID: 2017
	private Vector2 packsScrollPosition = Vector2.zero;

	// Token: 0x040007E2 RID: 2018
	private Vector2 historyScrollPosition = Vector2.zero;

	// Token: 0x040007E3 RID: 2019
	private int selectedPackIndex = -1;

	// Token: 0x040007E4 RID: 2020
	private string newPackName = "";

	// Token: 0x040007E5 RID: 2021
	private PackType newPackType = PackType.Weapon;

	// Token: 0x040007E6 RID: 2022
	private string newPackHp = "100";

	// Token: 0x040007E7 RID: 2023
	private string newPackCharge = "100";

	// Token: 0x040007E8 RID: 2024
	private float checkInterval = 20f;

	// Token: 0x040007E9 RID: 2025
	private GUIStyle headerStyle;

	// Token: 0x040007EA RID: 2026
	private bool showSettingsPanel;
}
