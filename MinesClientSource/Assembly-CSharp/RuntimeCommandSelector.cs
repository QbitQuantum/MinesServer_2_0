using System;
using System.Collections.Generic;
using UnityEngine;

// Token: 0x02000130 RID: 304
public class RuntimeCommandSelector : MonoBehaviour
{
	// Token: 0x06000603 RID: 1539 RVA: 0x0003A1DC File Offset: 0x000383DC
	private void CreateCommandDatabase()
	{
		this.AddCommand(0, "По умолчанию", false);
		this.AddCommand(1, "Переход", false);
		this.AddCommand(2, "Старт", false);
		this.AddCommand(3, "Стоп", false);
		this.AddCommand(4, "Вверх (W)", false);
		this.AddCommand(5, "Влево (A)", false);
		this.AddCommand(6, "Вниз (S)", false);
		this.AddCommand(7, "Вправо (D)", false);
		this.AddCommand(8, "Копать", false);
		this.AddCommand(9, "Поворот вверх (W x2)", false);
		this.AddCommand(10, "Поворот влево (S)", false);
		this.AddCommand(11, "Поворот вниз (D)", false);
		this.AddCommand(12, "Поворот вправо", false);
		this.AddCommand(13, "Повторить действие", false);
		this.AddCommand(14, "Шаг в направлении", false);
		this.AddCommand(15, "Поворот влево на 90", false);
		this.AddCommand(16, "Поворот вправо на 90", false);
		this.AddCommand(17, "Строить", false);
		this.AddCommand(18, "Геология", false);
		this.AddCommand(19, "Дорога", false);
		this.AddCommand(20, "Лечить", false);
		this.AddCommand(21, "Квадро", false);
		this.AddCommand(22, "Поворот в случайном направлении", false);
		this.AddCommand(23, "Бибика", false);
		this.AddCommand(24, "перейти к метке", false);
		this.AddCommand(25, "вызвать подпрограмму", false);
		this.AddCommand(26, "вызвать подпрограмму с передачей последнего аргумента", false);
		this.AddCommand(27, "возврат в место вызова подпрограммы", false);
		this.AddCommand(28, "возврат в место вызова с передачей последнего аргумента", false);
		this.AddCommand(29, "выбор просматриваемой клетки", false);
		this.AddCommand(30, "выбор просматриваемой клетки", false);
		this.AddCommand(31, "выбор просматриваемой клетки", false);
		this.AddCommand(32, "выбор просматриваемой клетки", false);
		this.AddCommand(33, "выбор просматриваемой клетки", false);
		this.AddCommand(35, "выбор просматриваемой клетки", false);
		this.AddCommand(36, "выбор просматриваемой клетки", false);
		this.AddCommand(37, "выбор просматриваемой клетки", false);
		this.AddCommand(38, "ИЛИ (OR)", false);
		this.AddCommand(39, "И (AND)", false);
		this.AddCommand(40, "Метка", false);
		this.AddCommand(41, "Заглушка (41)", true);
		this.AddCommand(42, "Заглушка (42)", true);
		this.AddCommand(43, "Не пусто", false);
		this.AddCommand(44, "Пусто", false);
		this.AddCommand(45, "падает?", false);
		this.AddCommand(46, "Кристалл?", false);
		this.AddCommand(47, "Живка?", false);
		this.AddCommand(48, "Валун?", false);
		this.AddCommand(49, "Песок?", false);
		this.AddCommand(50, "Камень?", false);
		this.AddCommand(51, "Разрушаем?", false);
		this.AddCommand(52, "Красноскал?", false);
		this.AddCommand(53, "Черноскал?", false);
		this.AddCommand(54, "Слизь?", false);
		this.AddCommand(57, "Квадро?", false);
		this.AddCommand(58, "Дорога?", false);
		this.AddCommand(59, "Заглушка (59)", true);
		this.AddCommand(60, "Заглушка (60)", true);
		this.AddCommand(61, "Заглушка (61)", true);
		this.AddCommand(62, "Заглушка (62)", true);
		this.AddCommand(63, "Заглушка (63)", true);
		this.AddCommand(64, "Заглушка (64)", true);
		this.AddCommand(65, "Заглушка (65)", true);
		this.AddCommand(66, "Заглушка (66)", true);
		this.AddCommand(67, "Заглушка (67)", true);
		this.AddCommand(68, "Заглушка (68)", true);
		this.AddCommand(69, "Заглушка (69)", true);
		this.AddCommand(70, "Заглушка (70)", true);
		this.AddCommand(71, "Заглушка (71)", true);
		this.AddCommand(72, "Заглушка (72)", true);
		this.AddCommand(73, "Заглушка (73)", true);
		this.AddCommand(74, "Бокс?", false);
		this.AddCommand(75, "Заглушка (75)", true);
		this.AddCommand(76, "Заглушка (76)", true);
		this.AddCommand(77, "Заглушка (77)", true);
		this.AddCommand(78, "Заглушка (78)", true);
		this.AddCommand(79, "Заглушка (79)", true);
		this.AddCommand(80, "Заглушка (80)", true);
		this.AddCommand(81, "Заглушка (81)", true);
		this.AddCommand(82, "Заглушка (82)", true);
		this.AddCommand(83, "Заглушка (83)", true);
		this.AddCommand(84, "Заглушка (84)", true);
		this.AddCommand(85, "Заглушка (85)", true);
		this.AddCommand(86, "Заглушка (86)", true);
		this.AddCommand(87, "Заглушка (87)", true);
		this.AddCommand(88, "Заглушка (88)", true);
		this.AddCommand(89, "Заглушка (89)", true);
		this.AddCommand(90, "Заглушка (90)", true);
		this.AddCommand(91, "Заглушка (91)", true);
		this.AddCommand(92, "Заглушка (92)", true);
		this.AddCommand(93, "Заглушка (93)", true);
		this.AddCommand(94, "Заглушка (94)", true);
		this.AddCommand(95, "Заглушка (95)", true);
		this.AddCommand(96, "Заглушка (96)", true);
		this.AddCommand(97, "Заглушка (97)", true);
		this.AddCommand(98, "Заглушка (98)", true);
		this.AddCommand(99, "Заглушка (99)", true);
		this.AddCommand(100, "Заглушка (100)", true);
		this.AddCommand(101, "Заглушка (101)", true);
		this.AddCommand(102, "Заглушка (102)", true);
		this.AddCommand(103, "Заглушка (103)", true);
		this.AddCommand(104, "Заглушка (104)", true);
		this.AddCommand(105, "Заглушка (105)", true);
		this.AddCommand(106, "Заглушка (106)", true);
		this.AddCommand(107, "Заглушка (107)", true);
		this.AddCommand(108, "Заглушка (108)", true);
		this.AddCommand(109, "Заглушка (109)", true);
		this.AddCommand(110, "Заглушка (110)", true);
		this.AddCommand(111, "Заглушка (111)", true);
		this.AddCommand(112, "Заглушка (112)", true);
		this.AddCommand(113, "Заглушка (113)", true);
		this.AddCommand(114, "Заглушка (114)", true);
		this.AddCommand(115, "Заглушка (115)", true);
		this.AddCommand(116, "Заглушка (116)", true);
		this.AddCommand(117, "Заглушка (117)", true);
		this.AddCommand(118, "Заглушка (118)", true);
		this.AddCommand(119, "Больше (>)", false);
		this.AddCommand(120, "Меньше (<)", false);
		this.AddCommand(121, "Заглушка (121)", true);
		this.AddCommand(122, "Заглушка (122)", true);
		this.AddCommand(123, "Равно (=)", false);
		this.AddCommand(124, "Заглушка (124)", true);
		this.AddCommand(125, "Заглушка (125)", true);
		this.AddCommand(126, "Заглушка (126)", true);
		this.AddCommand(127, "Заглушка (127)", true);
		this.AddCommand(128, "Заглушка (128)", true);
		this.AddCommand(129, "Заглушка (129)", true);
		this.AddCommand(130, "Заглушка (130)", true);
		this.AddCommand(131, "Ячейка WW", false);
		this.AddCommand(132, "Ячейка AA", false);
		this.AddCommand(133, "Ячейка SS", false);
		this.AddCommand(134, "Ячейка DD", false);
		this.AddCommand(135, "Ячейка F", false);
		this.AddCommand(136, "Ячейка FF", false);
		this.AddCommand(137, "Вызов (функция)", false);
		this.AddCommand(138, "Возврат (функция)", false);
		this.AddCommand(139, "Если не - GOTO", false);
		this.AddCommand(140, "Если - GOTO", false);
		this.AddCommand(141, "Копать (стандарт)", false);
		this.AddCommand(142, "Строить (стандарт)", false);
		this.AddCommand(143, "Лечить (стандарт)", false);
		this.AddCommand(144, "Flip", false);
		this.AddCommand(145, "Добыча (стандарт)", false);
		this.AddCommand(146, "Пушка", false);
		this.AddCommand(147, "Зарядить пушку", false);
		this.AddCommand(148, "HP", false);
		this.AddCommand(149, "HP50", false);
		this.AddCommand(150, "Заглушка (150)", true);
		this.AddCommand(151, "Заглушка (151)", true);
		this.AddCommand(152, "Заглушка (152)", true);
		this.AddCommand(153, "Заглушка (153)", true);
		this.AddCommand(154, "Заглушка (154)", true);
		this.AddCommand(155, "Заглушка (155)", true);
		this.AddCommand(156, "Правая рука", false);
		this.AddCommand(157, "Левая рука", false);
		this.AddCommand(158, "Автокопка ВКЛ", false);
		this.AddCommand(159, "Автокопка ВЫКЛ", false);
		this.AddCommand(160, "Агро ВКЛ", false);
		this.AddCommand(161, "Агро ВЫКЛ", false);
		this.AddCommand(162, "B1", false);
		this.AddCommand(163, "B3", false);
		this.AddCommand(164, "B2", false);
		this.AddCommand(165, "WB", false);
		this.AddCommand(166, "On Resp", false);
		this.AddCommand(167, "GeoPack", false);
		this.AddCommand(168, "ZM", false);
		this.AddCommand(169, "C190", false);
		this.AddCommand(170, "Poly", false);
		this.AddCommand(171, "Up", false);
		this.AddCommand(172, "Craft", false);
		this.AddCommand(173, "Nano", false);
		this.AddCommand(174, "RemBot", false);
		this.AddCommand(175, "Движение W (инв.)", false);
		this.AddCommand(176, "Движение A (инв.)", false);
		this.AddCommand(177, "Движение S (инв.)", false);
		this.AddCommand(178, "Движение D (инв.)", false);
		this.AddCommand(179, "Ручной режим ВКЛ", false);
		this.AddCommand(180, "Ручной режим ВЫКЛ", false);
		this.AddCommand(181, "Debug Break", false);
		this.AddCommand(182, "Debug Set", false);
		this.GroupCommandsByCategory();
	}

	// Token: 0x06000604 RID: 1540 RVA: 0x0003AC64 File Offset: 0x00038E64
	public void Show(int cellIndex)
	{
		this.targetCellIndex = cellIndex;
		this.isVisible = true;
		base.enabled = true;
		float num = 500f;
		float num2 = 600f;
		this.windowRect = new Rect(((float)Screen.width - num) / 2f, ((float)Screen.height - num2) / 2f, num, num2);
		this.scrollPosition = Vector2.zero;
	}

	// Token: 0x06000605 RID: 1541 RVA: 0x0003ACC8 File Offset: 0x00038EC8
	private void SelectCommand(int commandId)
	{
		RuntimeCommandSelector.CommandInfo commandInfo;
		if (this.commandMap.TryGetValue(commandId, out commandInfo))
		{
			Debug.Log(string.Format("Выбрана команда: {0} (ID: {1}) для ячейки {2}", commandInfo.name, commandId, this.targetCellIndex));
			if (this.programmatorView != null && this.targetCellIndex >= 0 && this.targetCellIndex < ProgrammatorView.actions.Length)
			{
				ProgrammatorView.actions[this.targetCellIndex].ChangeTo(commandId);
				ProgrammatorView.unsaved = true;
				if (this.programmatorView.titleTF != null)
				{
					this.programmatorView.titleTF.text = ProgrammatorView.title + " [*]";
				}
			}
			this.Close();
		}
	}

	// Token: 0x06000606 RID: 1542 RVA: 0x00008D5B File Offset: 0x00006F5B
	public void Close()
	{
		this.isVisible = false;
		base.enabled = false;
	}

	// Token: 0x06000608 RID: 1544 RVA: 0x0003AD84 File Offset: 0x00038F84
	private void Start()
	{
		this.commandMap = new Dictionary<int, RuntimeCommandSelector.CommandInfo>();
		this.CreateCommandDatabase();
		this.programmatorView = base.GetComponent<ProgrammatorView>();
		base.enabled = false;
		if (!RuntimeCommandSelector.inited)
		{
			RuntimeCommandSelector.sprites = ResourcesManager.LoadAll<Sprite>("programmator");
			RuntimeCommandSelector.inited = true;
		}
	}

	// Token: 0x06000609 RID: 1545 RVA: 0x0003ADD4 File Offset: 0x00038FD4
	private void GroupCommandsByCategory()
	{
		this.categorizedCommands = new Dictionary<string, List<RuntimeCommandSelector.CommandInfo>>();
		foreach (RuntimeCommandSelector.CommandInfo commandInfo in this.commandMap.Values)
		{
			string commandCategory = this.GetCommandCategory(commandInfo.id);
			if (!this.categorizedCommands.ContainsKey(commandCategory))
			{
				this.categorizedCommands[commandCategory] = new List<RuntimeCommandSelector.CommandInfo>();
			}
			this.categorizedCommands[commandCategory].Add(commandInfo);
		}
	}

	// Token: 0x0600060A RID: 1546 RVA: 0x0003AE70 File Offset: 0x00039070
	private void OnGUI()
	{
		if (!this.isVisible)
		{
			return;
		}
		GUI.skin = null;
		Color color = GUI.color;
		GUI.color = new Color(0f, 0f, 0f, 0.7f);
		GUI.DrawTexture(new Rect(0f, 0f, (float)Screen.width, (float)Screen.height), Texture2D.whiteTexture);
		GUI.color = color;
		this.windowRect = GUI.Window(1234567, this.windowRect, new GUI.WindowFunction(this.DrawCommandWindow), "ВЫБОР КОМАНДЫ");
	}

	// Token: 0x0600060B RID: 1547 RVA: 0x0003AF00 File Offset: 0x00039100
	private void DrawCommandWindow(int windowID)
	{
		GUILayout.Space(5f);
		Event current = Event.current;
		this.scrollPosition = GUILayout.BeginScrollView(this.scrollPosition, false, true, new GUILayoutOption[]
		{
			GUILayout.ExpandWidth(true),
			GUILayout.ExpandHeight(true)
		});
		List<string> list = new List<string>(this.categorizedCommands.Keys);
		list.Sort();
		foreach (string text in list)
		{
			List<RuntimeCommandSelector.CommandInfo> list2 = this.categorizedCommands[text];
			GUILayout.Label(text, new GUIStyle(GUI.skin.label)
			{
				fontStyle = FontStyle.Bold,
				normal = 
				{
					textColor = Color.yellow
				},
				fontSize = 14,
				alignment = TextAnchor.MiddleLeft,
				padding = new RectOffset(10, 0, 5, 5)
			}, Array.Empty<GUILayoutOption>());
			int num = Mathf.Max(4, Mathf.FloorToInt(this.windowRect.width / 95f));
			int count = list2.Count;
			int num2 = Mathf.CeilToInt((float)count / (float)num);
			for (int i = 0; i < num2; i++)
			{
				GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
				for (int j = 0; j < num; j++)
				{
					int num3 = i * num + j;
					if (num3 < count)
					{
						RuntimeCommandSelector.CommandInfo commandInfo = list2[num3];
						GUIStyle guistyle = new GUIStyle(GUI.skin.button);
						guistyle.fontSize = 10;
						guistyle.wordWrap = true;
						guistyle.margin = new RectOffset(2, 2, 2, 2);
						Sprite spriteById = this.GetSpriteById(commandInfo.id);
						if (spriteById != null)
						{
							Rect rect = GUILayoutUtility.GetRect(35f, 35f, new GUILayoutOption[]
							{
								GUILayout.Width(35f),
								GUILayout.Height(35f)
							});
							if (GUI.Button(rect, GUIContent.none, GUI.skin.button))
							{
								this.SelectCommand(commandInfo.id);
							}
							Rect rect2 = spriteById.rect;
							Texture2D texture = spriteById.texture;
							Rect texCoords = new Rect(rect2.x / (float)texture.width, rect2.y / (float)texture.height, rect2.width / (float)texture.width, rect2.height / (float)texture.height);
							GUI.DrawTextureWithTexCoords(rect, texture, texCoords);
							if (rect.Contains(current.mousePosition))
							{
								this.hoveredCommandId = commandInfo.id;
							}
							if (rect.Contains(current.mousePosition))
							{
								this.DrawTooltip(commandInfo.name, current.mousePosition);
							}
						}
						else if (GUILayout.Button(commandInfo.name, guistyle, new GUILayoutOption[]
						{
							GUILayout.Width(90f),
							GUILayout.Height(35f)
						}))
						{
							this.SelectCommand(commandInfo.id);
						}
					}
					else
					{
						GUILayout.Space(94f);
					}
				}
				GUILayout.EndHorizontal();
				GUILayout.Space(2f);
			}
			GUILayout.Space(10f);
		}
		GUILayout.EndScrollView();
		GUILayout.Space(5f);
		GUILayout.BeginHorizontal(Array.Empty<GUILayoutOption>());
		GUILayout.FlexibleSpace();
		if (GUILayout.Button("Закрыть", new GUIStyle(GUI.skin.button)
		{
			fontSize = 12
		}, new GUILayoutOption[]
		{
			GUILayout.Width(100f),
			GUILayout.Height(30f)
		}))
		{
			this.Close();
		}
		GUILayout.FlexibleSpace();
		GUILayout.EndHorizontal();
		GUILayout.Space(5f);
		GUI.DragWindow(new Rect(0f, 0f, this.windowRect.width, 30f));
	}

	// Token: 0x0600060C RID: 1548 RVA: 0x00008D7A File Offset: 0x00006F7A
	private Sprite GetSpriteById(int id)
	{
		if (RuntimeCommandSelector.sprites != null && id >= 0 && id < RuntimeCommandSelector.sprites.Length)
		{
			return RuntimeCommandSelector.sprites[id];
		}
		return null;
	}

	// Token: 0x0600060D RID: 1549 RVA: 0x0003B2D8 File Offset: 0x000394D8
	private Texture2D MakeTexture(int width, int height, Color color)
	{
		Texture2D texture2D = new Texture2D(width, height);
		for (int i = 0; i < width; i++)
		{
			for (int j = 0; j < height; j++)
			{
				texture2D.SetPixel(i, j, color);
			}
		}
		texture2D.Apply();
		return texture2D;
	}

	// Token: 0x0600060E RID: 1550 RVA: 0x0003B318 File Offset: 0x00039518
	private string GetCommandCategory(int id)
	{
		if ((id >= 4 && id <= 14) || (id >= 175 && id <= 178))
		{
			return "ДВИЖЕНИЕ";
		}
		if (id == 8 || id == 17 || id == 18 || id == 19 || id == 20 || id == 21 || id == 22 || id == 23 || id == 15 || id == 16)
		{
			return "ДЕЙСТВИЯ";
		}
		if (id == 141 || id == 142 || id == 143 || id == 145)
		{
			return "СТАНДАРТНЫЕ";
		}
		if (id == 40 || id == 24 || id == 25 || id == 26 || id == 137 || id == 27 || id == 28 || id == 138 || id == 200)
		{
			return "УПРАВЛЕНИЕ";
		}
		if ((id >= 43 && id <= 54) || id == 57 || id == 58 || id == 74)
		{
			return "УСЛОВИЯ (КЛЕТКИ)";
		}
		if (id == 123 || id == 120 || id == 119 || id == 139 || id == 140 || id == 38 || id == 39)
		{
			return "УСЛОВИЯ (СРАВНЕНИЯ)";
		}
		if (id == 158 || id == 159 || id == 160 || id == 161 || id == 179 || id == 180)
		{
			return "РЕЖИМЫ";
		}
		if (id == 181 || id == 182)
		{
			return "ОТЛАДКА";
		}
		if (id == 148 || id == 149 || id == 144 || id == 166)
		{
			return "СОСТОЯНИЯ";
		}
		if (id >= 162 && id <= 174)
		{
			return "ДОП. ДЕЙСТВИЯ";
		}
		if ((id >= 29 && id <= 37) || (id >= 131 && id <= 136) || id == 156 || id == 157)
		{
			return "НАПРАВЛЕНИЯ";
		}
		if (id == 146 || id == 147)
		{
			return "ОРУЖИЕ";
		}
		if ((id >= 41 && id <= 42) || (id >= 59 && id <= 73) || (id >= 75 && id <= 118) || (id >= 121 && id <= 122) || (id >= 124 && id <= 130) || (id >= 150 && id <= 155))
		{
			return "ЗАГЛУШКИ";
		}
		return "ПРОЧЕЕ";
	}

	// Token: 0x0600060F RID: 1551 RVA: 0x00008D9A File Offset: 0x00006F9A
	private void AddCommand(int id, string name, bool isPlaceholder = false)
	{
		this.commandMap[id] = new RuntimeCommandSelector.CommandInfo
		{
			id = id,
			name = name,
			isPlaceholder = isPlaceholder
		};
	}

	// Token: 0x06000610 RID: 1552 RVA: 0x0003B54C File Offset: 0x0003974C
	private void DrawTooltip(string text, Vector2 mousePosition)
	{
		GUIStyle guistyle = new GUIStyle(GUI.skin.box);
		guistyle.normal.background = this.MakeTexture(2, 2, new Color(0.1f, 0.1f, 0.1f, 0.95f));
		guistyle.normal.textColor = Color.white;
		guistyle.fontSize = 12;
		guistyle.alignment = TextAnchor.MiddleCenter;
		guistyle.padding = new RectOffset(8, 8, 4, 4);
		Vector2 vector = guistyle.CalcSize(new GUIContent(text));
		Rect position = new Rect(mousePosition.x + 15f, mousePosition.y - 15f, vector.x + 10f, vector.y + 6f);
		if (position.xMax > (float)Screen.width)
		{
			position.x = (float)Screen.width - position.width - 10f;
		}
		if (position.yMin < 0f)
		{
			position.y = mousePosition.y + 20f;
		}
		GUI.Box(position, text, guistyle);
	}

	// Token: 0x040008B3 RID: 2227
	private int targetCellIndex;

	// Token: 0x040008B4 RID: 2228
	private Dictionary<int, RuntimeCommandSelector.CommandInfo> commandMap;

	// Token: 0x040008B5 RID: 2229
	private Dictionary<string, List<RuntimeCommandSelector.CommandInfo>> categorizedCommands;

	// Token: 0x040008B6 RID: 2230
	private ProgrammatorView programmatorView;

	// Token: 0x040008B7 RID: 2231
	private bool isVisible;

	// Token: 0x040008B8 RID: 2232
	private Vector2 scrollPosition;

	// Token: 0x040008B9 RID: 2233
	private Rect windowRect;

	// Token: 0x040008BA RID: 2234
	private static Sprite[] sprites;

	// Token: 0x040008BB RID: 2235
	private static bool inited;

	// Token: 0x040008BC RID: 2236
	private int hoveredCommandId = -1;

	// Token: 0x02000131 RID: 305
	public class CommandInfo
	{
		// Token: 0x040008BD RID: 2237
		public int id;

		// Token: 0x040008BE RID: 2238
		public string name;

		// Token: 0x040008BF RID: 2239
		public bool isPlaceholder;
	}
}
