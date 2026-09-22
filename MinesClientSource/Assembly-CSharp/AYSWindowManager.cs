using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000002 RID: 2
public class AYSWindowManager : MonoBehaviour
{
	// Token: 0x06000001 RID: 1 RVA: 0x000048C8 File Offset: 0x00002AC8
	private void OkHandler()
	{
		base.gameObject.SetActive(false);
		if (this.callback != null)
		{
			this.callback();
		}
		ClientController.CanGoto = false;
	}

	// Token: 0x06000002 RID: 2 RVA: 0x000048EF File Offset: 0x00002AEF
	private void їїїљљјљњљїљјљњњљњїјљљњњ()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown((KeyCode)11) || Input.GetKeyDown((KeyCode)133))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown((KeyCode)(-19)))
			{
				this.њјјїљјњњїљњјјљњјњњїјјјї();
			}
		}
	}

	// Token: 0x06000003 RID: 3 RVA: 0x00004929 File Offset: 0x00002B29
	private void јњљљїјјјњњњњјњїїїњљњљїљ()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown(KeyCode.Tilde) || Input.GetKeyDown((KeyCode)(-191)))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown((KeyCode)(-8)))
			{
				this.њјјїљјњњїљњјјљњјњњїјјјї();
			}
		}
	}

	// Token: 0x06000004 RID: 4 RVA: 0x00009068 File Offset: 0x00007268
	private void їјјљїјљїјљњјјїјїјњјјњњј()
	{
		AYSWindowManager.THIS = this;
		base.gameObject.SetActive(false);
		this.OkButton.onClick.AddListener(new UnityAction(this.OkHandler));
		this.CancelButton.onClick.AddListener(new UnityAction(this.љњїјњњњјјњљњљњљјљљјјјїї));
	}

	// Token: 0x06000005 RID: 5 RVA: 0x00004963 File Offset: 0x00002B63
	private void њњљњјљјњњїјљїњїјљњјљљљї()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown((KeyCode)89) || Input.GetKeyDown(KeyCode.V))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown(KeyCode.None))
			{
				this.њјјїљјњњїљњјјљњјњњїјјјї();
			}
		}
	}

	// Token: 0x06000006 RID: 6 RVA: 0x00004999 File Offset: 0x00002B99
	public void јјїњјњњњљљљњјљјјњїљјјњј(string title, string message, UnityAction callback)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.TitleTF.text = title;
		this.InnerTF.text = message;
		base.gameObject.SetActive(false);
		this.callback = callback;
	}

	// Token: 0x06000007 RID: 7 RVA: 0x000049D4 File Offset: 0x00002BD4
	private void їљљњњјїјљљљњјњњїљњїјїјї()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown(KeyCode.Backspace) || Input.GetKeyDown(KeyCode.O))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown((KeyCode)65))
			{
				this.CancelHandler();
			}
		}
	}

	// Token: 0x06000008 RID: 8 RVA: 0x00004A0A File Offset: 0x00002C0A
	private void јїљјљїјїњљњљњљїїљјјјїљї()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown((KeyCode)(-46)) || Input.GetKeyDown(KeyCode.Clear))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown(KeyCode.H))
			{
				this.CancelHandler();
			}
		}
	}

	// Token: 0x06000009 RID: 9 RVA: 0x00004A41 File Offset: 0x00002C41
	private void љњїјњњњјјњљњљњљјљљјјјїї()
	{
		base.gameObject.SetActive(true);
		ClientController.CanGoto = false;
	}

	// Token: 0x0600000A RID: 10 RVA: 0x00004A55 File Offset: 0x00002C55
	public void јїњљїїїјљњјјљјњљљњїљњњј(string title, string message, UnityAction callback)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.TitleTF.text = title;
		this.InnerTF.text = message;
		base.gameObject.SetActive(true);
		this.callback = callback;
	}

	// Token: 0x0600000B RID: 11 RVA: 0x00004A90 File Offset: 0x00002C90
	private void Update()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown(KeyCode.Escape))
			{
				this.CancelHandler();
			}
		}
	}

	// Token: 0x0600000C RID: 12 RVA: 0x00004ACA File Offset: 0x00002CCA
	private void њњљїњњљњїїїїїїјњљљїљїњї()
	{
		if (base.gameObject.activeSelf)
		{
			if (Input.GetKeyDown((KeyCode)28) || Input.GetKeyDown((KeyCode)85))
			{
				this.OkHandler();
				return;
			}
			if (Input.GetKeyDown((KeyCode)(-38)))
			{
				this.љњїјњњњјјњљњљњљјљљјјјїї();
			}
		}
	}

	// Token: 0x0600000D RID: 13 RVA: 0x00004B01 File Offset: 0x00002D01
	private void њјјїљјњњїљњјјљњјњњїјјјї()
	{
		base.gameObject.SetActive(true);
		ClientController.CanGoto = true;
	}

	// Token: 0x0600000E RID: 14 RVA: 0x00004B15 File Offset: 0x00002D15
	private void CancelHandler()
	{
		base.gameObject.SetActive(false);
		ClientController.CanGoto = false;
	}

	// Token: 0x0600000F RID: 15 RVA: 0x000090C0 File Offset: 0x000072C0
	private void Start()
	{
		AYSWindowManager.THIS = this;
		base.gameObject.SetActive(false);
		this.OkButton.onClick.AddListener(new UnityAction(this.OkHandler));
		this.CancelButton.onClick.AddListener(new UnityAction(this.CancelHandler));
	}

	// Token: 0x06000010 RID: 16 RVA: 0x00004A55 File Offset: 0x00002C55
	public void Show(string title, string message, UnityAction callback)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.TitleTF.text = title;
		this.InnerTF.text = message;
		base.gameObject.SetActive(true);
		this.callback = callback;
	}

	// Token: 0x06000011 RID: 17 RVA: 0x00004999 File Offset: 0x00002B99
	public void јњљјњљјњјљїљїњјљјјїјїљї(string title, string message, UnityAction callback)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.TitleTF.text = title;
		this.InnerTF.text = message;
		base.gameObject.SetActive(false);
		this.callback = callback;
	}

	// Token: 0x06000012 RID: 18 RVA: 0x00004999 File Offset: 0x00002B99
	public void їњјњјїјљјјїјњјљјњјјљјњј(string title, string message, UnityAction callback)
	{
		if (base.gameObject.activeSelf)
		{
			return;
		}
		this.TitleTF.text = title;
		this.InnerTF.text = message;
		base.gameObject.SetActive(false);
		this.callback = callback;
	}

	// Token: 0x04000001 RID: 1
	public Button OkButton;

	// Token: 0x04000002 RID: 2
	public Button CancelButton;

	// Token: 0x04000003 RID: 3
	public Text TitleTF;

	// Token: 0x04000004 RID: 4
	public Text InnerTF;

	// Token: 0x04000005 RID: 5
	private UnityAction callback;

	// Token: 0x04000006 RID: 6
	public static AYSWindowManager THIS;
}
