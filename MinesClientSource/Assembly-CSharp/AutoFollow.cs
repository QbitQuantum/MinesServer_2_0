using System;
using System.Collections.Generic;
using System.Reflection;
using MyUI;
using UnityEngine;

// Token: 0x020000E6 RID: 230
public class AutoFollow : MonoBehaviour
{
	// Token: 0x060005F0 RID: 1520 RVA: 0x00008CAD File Offset: 0x00006EAD
	public void Start()
	{
		this.clientController = ClientController.THIS;
		this.robotRenderer = RobotRenderer.THIS;
		if (this.autoStart)
		{
			this.ToggleFollow();
		}
	}

	// Token: 0x060005F1 RID: 1521 RVA: 0x00008CD3 File Offset: 0x00006ED3
	public void Update()
	{
		if (Input.GetKeyDown(this.toggleKey))
		{
			this.ToggleFollow();
		}
		if (this.isFollowing && !this.IsAnyUIBlocking())
		{
			this.ProcessFollow();
		}
	}

	// Token: 0x060005F2 RID: 1522 RVA: 0x00039818 File Offset: 0x00037A18
	public void ToggleFollow()
	{
		this.isFollowing = !this.isFollowing;
		if (this.isFollowing)
		{
			if (this.clientController.GetAutoMove())
			{
				this.clientController.stopAutoMove();
			}
			this.FindBestTarget();
			this.SetFollowTarget();
			Debug.Log("[AutoFollow] Включен. Цель: " + ((this.currentTargetId != -1) ? this.currentTargetId.ToString() : "нет"));
			return;
		}
		if (this.clientController.GetAutoMove())
		{
			this.clientController.stopAutoMove();
		}
		Debug.Log("[AutoFollow] Выключен");
	}

	// Token: 0x060005F3 RID: 1523 RVA: 0x000398B0 File Offset: 0x00037AB0
	public void ProcessFollow()
	{
		if (!this.IsTargetValid())
		{
			this.FindBestTarget();
			if (this.currentTargetId == -1)
			{
				if (this.clientController.GetAutoMove())
				{
					this.clientController.stopAutoMove();
				}
				return;
			}
			this.SetFollowTarget();
		}
		Vector2Int targetPosition = this.GetTargetPosition();
		if (Vector2Int.Distance(new Vector2Int(this.clientController.myBot.gx, this.clientController.myBot.gy), targetPosition) <= this.minDistanceToTarget)
		{
			if (this.clientController.GetAutoMove())
			{
				this.clientController.stopAutoMove();
				return;
			}
		}
		else
		{
			if (Time.time - this.lastPathRecalcTime >= this.pathRecalcInterval)
			{
				this.SetFollowTarget();
				this.lastPathRecalcTime = Time.time;
			}
			if (!this.clientController.GetAutoMove() && this.wasTargetReachable)
			{
				this.SetFollowTarget();
			}
		}
	}

	// Token: 0x060005F4 RID: 1524 RVA: 0x00039998 File Offset: 0x00037B98
	public void FindBestTarget()
	{
		if (this.targetBotId != -1 && this.robotRenderer.bots.ContainsKey(this.targetBotId))
		{
			this.currentTargetId = this.targetBotId;
			return;
		}
		float num = float.MaxValue;
		int num2 = -1;
		Vector2Int a = new Vector2Int(this.clientController.myBot.gx, this.clientController.myBot.gy);
		foreach (KeyValuePair<int, GameObject> keyValuePair in this.robotRenderer.bots)
		{
			if (keyValuePair.Key != this.clientController.myBotId)
			{
				RobotScript component = keyValuePair.Value.GetComponent<RobotScript>();
				if (component != null)
				{
					float num3 = Vector2Int.Distance(a, new Vector2Int(component.gx, component.gy));
					if (num3 < num && num3 < (float)ClientConfig.mouseR)
					{
						num = num3;
						num2 = keyValuePair.Key;
					}
				}
			}
		}
		this.currentTargetId = num2;
	}

	// Token: 0x060005F5 RID: 1525 RVA: 0x00039ABC File Offset: 0x00037CBC
	public bool IsTargetValid()
	{
		bool result;
		if (this.currentTargetId == -1)
		{
			result = false;
		}
		else if (!this.robotRenderer.bots.ContainsKey(this.currentTargetId))
		{
			result = false;
		}
		else
		{
			GameObject gameObject = this.robotRenderer.bots[this.currentTargetId];
			if (gameObject == null)
			{
				result = false;
			}
			else
			{
				RobotScript component = gameObject.GetComponent<RobotScript>();
				if (component == null)
				{
					result = false;
				}
				else
				{
					Vector2Int a = new Vector2Int(this.clientController.myBot.gx, this.clientController.myBot.gy);
					Vector2Int b = new Vector2Int(component.gx, component.gy);
					result = (Vector2Int.Distance(a, b) <= (float)ClientConfig.mouseR * 1.5f);
				}
			}
		}
		return result;
	}

	// Token: 0x060005F6 RID: 1526 RVA: 0x00039B88 File Offset: 0x00037D88
	public Vector2Int GetTargetPosition()
	{
		Vector2Int zero;
		if (!this.IsTargetValid())
		{
			zero = Vector2Int.zero;
		}
		else
		{
			RobotScript component = this.robotRenderer.bots[this.currentTargetId].GetComponent<RobotScript>();
			if (component == null)
			{
				zero = Vector2Int.zero;
			}
			else
			{
				zero = new Vector2Int(component.gx, component.gy);
			}
		}
		return zero;
	}

	// Token: 0x060005F7 RID: 1527 RVA: 0x00039BEC File Offset: 0x00037DEC
	public bool IsAnyUIBlocking()
	{
		ChatManager this2 = ChatManager.THIS;
		bool flag;
		if (this2 == null)
		{
			flag = false;
		}
		else
		{
			MyInputField chatInput = this2.ChatInput;
			bool? flag2 = (chatInput != null) ? new bool?(chatInput.isFocused) : null;
			bool flag3 = true;
			flag = (flag2.GetValueOrDefault() == flag3 & flag2 != null);
		}
		bool result;
		if (flag)
		{
			result = true;
		}
		else
		{
			GUIManager this3 = GUIManager.THIS;
			bool flag4;
			if (this3 == null)
			{
				flag4 = false;
			}
			else
			{
				MyInputField localChatInput = this3.localChatInput;
				bool? flag5 = (localChatInput != null) ? new bool?(localChatInput.isFocused) : null;
				bool flag6 = true;
				flag4 = (flag5.GetValueOrDefault() == flag6 & flag5 != null);
			}
			if (flag4)
			{
				result = true;
			}
			else if (ProgrammatorView.active)
			{
				result = true;
			}
			else
			{
				MapViewer this4 = MapViewer.THIS;
				if (this4 != null && this4.gameObject.activeSelf)
				{
					result = true;
				}
				else
				{
					AYSWindowManager this5 = AYSWindowManager.THIS;
					if (this5 != null && this5.gameObject.activeSelf)
					{
						result = true;
					}
					else
					{
						PopupManager this6 = PopupManager.THIS;
						bool flag7;
						if (this6 == null)
						{
							flag7 = false;
						}
						else
						{
							GameObject guiwindow = this6.GUIWindow;
							bool? flag8 = (guiwindow != null) ? new bool?(guiwindow.activeSelf) : null;
							bool flag9 = true;
							flag7 = (flag8.GetValueOrDefault() == flag9 & flag8 != null);
						}
						result = flag7;
					}
				}
			}
		}
		return result;
	}

	// Token: 0x060005F8 RID: 1528 RVA: 0x00039D78 File Offset: 0x00037F78
	private void OnGUI()
	{
		if (this.isFollowing && this.currentTargetId != -1)
		{
			GUI.color = Color.green;
			GUI.Label(new Rect(10f, 50f, 350f, 30f), string.Format("[AUTOFOLLOW] Следую за ботом ID: {0}", this.currentTargetId));
			if (this.robotRenderer.bots.ContainsKey(this.currentTargetId))
			{
				RobotScript component = this.robotRenderer.bots[this.currentTargetId].GetComponent<RobotScript>();
				if (component != null)
				{
					Vector2Int a = new Vector2Int(this.clientController.myBot.gx, this.clientController.myBot.gy);
					Vector2Int b = new Vector2Int(component.gx, component.gy);
					float num = Vector2Int.Distance(a, b);
					GUI.Label(new Rect(10f, 75f, 350f, 30f), string.Format("Дистанция: {0:F1} клеток", num));
					GUI.Label(new Rect(10f, 100f, 350f, 30f), "Автодвижение: " + (this.clientController.GetAutoMove() ? "активно" : "неактивно"));
					if (!this.wasTargetReachable)
					{
						GUI.color = Color.yellow;
						GUI.Label(new Rect(10f, 125f, 350f, 30f), "Цель слишком далеко!");
					}
				}
			}
		}
	}

	// Token: 0x060005F9 RID: 1529 RVA: 0x00039F08 File Offset: 0x00038108
	public AutoFollow()
	{
		this.isFollowing = false;
		this.currentTargetId = -1;
		this.wasTargetReachable = true;
	}

	// Token: 0x060005FA RID: 1530 RVA: 0x00008D04 File Offset: 0x00006F04
	public void SetTargetBotId(int botId)
	{
		this.targetBotId = botId;
		this.currentTargetId = botId;
		if (this.isFollowing)
		{
			if (this.clientController.GetAutoMove())
			{
				this.clientController.stopAutoMove();
			}
			this.SetFollowTarget();
		}
	}

	// Token: 0x060005FB RID: 1531 RVA: 0x00008D3A File Offset: 0x00006F3A
	public bool IsFollowing()
	{
		return this.isFollowing;
	}

	// Token: 0x060005FC RID: 1532 RVA: 0x00039F60 File Offset: 0x00038160
	private void SetFollowTarget()
	{
		if (!(this.clientController.myBot == null))
		{
			Vector2Int targetPosition = this.GetTargetPosition();
			if (!(targetPosition == Vector2Int.zero))
			{
				if (Vector2Int.Distance(new Vector2Int(this.clientController.myBot.gx, this.clientController.myBot.gy), targetPosition) > (float)ClientConfig.mouseR)
				{
					this.wasTargetReachable = false;
					if (this.clientController.GetAutoMove())
					{
						this.clientController.stopAutoMove();
						return;
					}
				}
				else
				{
					this.wasTargetReachable = true;
					this.lastTargetPosition = targetPosition;
					if (this.useSmartPointing)
					{
						this.clientController.FromMapGoto(targetPosition.x, targetPosition.y);
						return;
					}
					this.SetTargetDirect(targetPosition.x, targetPosition.y);
				}
			}
		}
	}

	// Token: 0x060005FD RID: 1533 RVA: 0x0003A034 File Offset: 0x00038234
	private void SetTargetDirect(int x, int y)
	{
		Type type = this.clientController.GetType();
		FieldInfo field = type.GetField("GotoX", BindingFlags.Instance | BindingFlags.NonPublic);
		FieldInfo field2 = type.GetField("GotoY", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method = type.GetMethod("startAutoMove", BindingFlags.Instance | BindingFlags.NonPublic);
		MethodInfo method2 = type.GetMethod("UpdateRoute", BindingFlags.Instance | BindingFlags.NonPublic);
		if (field != null && field2 != null && method != null)
		{
			field.SetValue(this.clientController, x);
			field2.SetValue(this.clientController, y);
			method.Invoke(this.clientController, null);
			if (method2 != null)
			{
				method2.Invoke(this.clientController, null);
			}
		}
	}

	// Token: 0x060005FE RID: 1534 RVA: 0x00008D42 File Offset: 0x00006F42
	public int GetCurrentTargetId()
	{
		return this.currentTargetId;
	}

	// Token: 0x040007FA RID: 2042
	[Header("Настройки преследования")]
	public int targetBotId = -1;

	// Token: 0x040007FB RID: 2043
	public float minDistanceToTarget = 1f;

	// Token: 0x040007FC RID: 2044
	[Header("Управление")]
	public KeyCode toggleKey = KeyCode.F9;

	// Token: 0x040007FD RID: 2045
	public bool autoStart;

	// Token: 0x040007FE RID: 2046
	[Header("Настройки задержек")]
	public float pathRecalcInterval = 0.5f;

	// Token: 0x040007FF RID: 2047
	private bool isFollowing;

	// Token: 0x04000800 RID: 2048
	private ClientController clientController;

	// Token: 0x04000801 RID: 2049
	private RobotRenderer robotRenderer;

	// Token: 0x04000802 RID: 2050
	public int currentTargetId;

	// Token: 0x04000803 RID: 2051
	private float lastPathRecalcTime;

	// Token: 0x04000804 RID: 2052
	[Header("Настройки движения")]
	public bool useSmartPointing = true;

	// Token: 0x04000805 RID: 2053
	private Vector2Int lastTargetPosition;

	// Token: 0x04000806 RID: 2054
	private bool wasTargetReachable;
}
