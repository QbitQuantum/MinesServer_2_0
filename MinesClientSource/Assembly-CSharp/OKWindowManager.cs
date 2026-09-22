using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200003D RID: 61
public class OKWindowManager : MonoBehaviour
{
	// Token: 0x0600018A RID: 394 RVA: 0x00005C11 File Offset: 0x00003E11
	private void Start()
	{
		OKWindowManager.THIS = this;
		base.gameObject.SetActive(false);
		this.ExitButton.onClick.AddListener(new UnityAction(this.ExitHandler));
	}

	// Token: 0x0600018B RID: 395 RVA: 0x00005C41 File Offset: 0x00003E41
	private void ExitHandler()
	{
		base.gameObject.SetActive(false);
		this.CheckQueue();
		ClientController.CanGoto = false;
	}

	// Token: 0x0600018C RID: 396 RVA: 0x00005C5B File Offset: 0x00003E5B
	public void AddMessage(OKMessage msg)
	{
		this.msgQueue.Enqueue(msg);
		this.CheckQueue();
	}

	// Token: 0x0600018D RID: 397 RVA: 0x0001B414 File Offset: 0x00019614
	public void CheckQueue()
	{
		if (this.msgQueue.Count > 0 && !base.gameObject.activeSelf)
		{
			OKMessage okmessage = this.msgQueue.Dequeue();
			this.TitleTF.text = okmessage.title;
			this.InnerTF.text = okmessage.message;
			base.gameObject.SetActive(true);
		}
	}

	// Token: 0x0600018E RID: 398 RVA: 0x00005C6F File Offset: 0x00003E6F
	private void Update()
	{
		if (base.gameObject.activeSelf && !AYSWindowManager.THIS.gameObject.activeSelf && (Input.GetKeyDown(KeyCode.Return) || Input.GetKeyDown(KeyCode.KeypadEnter)))
		{
			this.ExitHandler();
		}
	}

	// Token: 0x040002AE RID: 686
	public Button ExitButton;

	// Token: 0x040002AF RID: 687
	public Text TitleTF;

	// Token: 0x040002B0 RID: 688
	public Text InnerTF;

	// Token: 0x040002B1 RID: 689
	private Queue<OKMessage> msgQueue = new Queue<OKMessage>();

	// Token: 0x040002B2 RID: 690
	public static OKWindowManager THIS;
}
