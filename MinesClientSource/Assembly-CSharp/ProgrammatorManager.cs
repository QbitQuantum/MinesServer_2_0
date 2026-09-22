using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x0200005A RID: 90
public class ProgrammatorManager : MonoBehaviour
{
	// Token: 0x0600020B RID: 523 RVA: 0x000207E0 File Offset: 0x0001E9E0
	private void Start()
	{
		this.StartButton.onClick.AddListener(new UnityAction(this.OnStartButton));
		this.ExitButton.onClick.AddListener(new UnityAction(this.OnExitButton));
		this.ClearButton.onClick.AddListener(new UnityAction(this.OnClearButton));
		this.MenuButton.onClick.AddListener(new UnityAction(this.OnMenuButton));
		this.CopyButton.onClick.AddListener(new UnityAction(this.OnCopyButton));
		this.RenameButton.onClick.AddListener(new UnityAction(this.OnRenameButton));
		this.OpenHelpButton.onClick.AddListener(new UnityAction(this.OnOpenHelp));
		this.ExitHelpButton.onClick.AddListener(new UnityAction(this.OnExitHelp));
		this.OpenWikiButton.onClick.AddListener(delegate()
		{
			string text = "http://minesgame.ru/wiki";
			ServerController.THIS.OpenURLHandler(ref text);
		});
		base.Invoke("OnExitButton", 0.01f);
	}

	// Token: 0x0600020C RID: 524 RVA: 0x000062B8 File Offset: 0x000044B8
	private void OnOpenHelp()
	{
		this.HelpPanel.SetActive(!this.HelpPanel.activeSelf);
	}

	// Token: 0x0600020D RID: 525 RVA: 0x000062D3 File Offset: 0x000044D3
	private void OnExitHelp()
	{
		this.HelpPanel.SetActive(false);
	}

	// Token: 0x0600020E RID: 526 RVA: 0x000062E1 File Offset: 0x000044E1
	private void OnClearButton()
	{
		ClientController.CanGoto = false;
		AYSWindowManager.THIS.Show("УДАЛЕНИЕ ПРОГРАММЫ", "<color=#ff8888ff>Программа будет удалена навсегда.</color>\n\nУдалить программу?", new UnityAction(this.ClearButtonEnable));
	}

	// Token: 0x0600020F RID: 527 RVA: 0x00006309 File Offset: 0x00004509
	private void ClearButtonEnable()
	{
		base.gameObject.SetActive(false);
		ProgrammatorView.active = false;
		ServerTime.THIS.SendTypicalMessage(-1, "PDEL", 0, 0, ProgrammatorView.programId.ToString());
	}

	// Token: 0x06000210 RID: 528 RVA: 0x0000633A File Offset: 0x0000453A
	private void OnStartButton()
	{
		ClientController.CanGoto = false;
		base.gameObject.SetActive(false);
		ProgrammatorView.active = false;
		ProgrammatorView.unsaved = false;
		ProgrammatorView.THIS.titleTF.text = ProgrammatorView.title;
		ProgrammatorView.THIS.SendAndStartProgram();
	}

	// Token: 0x06000211 RID: 529 RVA: 0x00006378 File Offset: 0x00004578
	private void OnRenameButton()
	{
		ClientController.CanGoto = false;
		base.gameObject.SetActive(false);
		ProgrammatorView.active = false;
		ServerTime.THIS.SendTypicalMessage(-1, "PREN", 0, 0, ProgrammatorView.programId.ToString());
	}

	// Token: 0x06000212 RID: 530 RVA: 0x000063AF File Offset: 0x000045AF
	private void OnMenuButton()
	{
		ClientController.CanGoto = false;
		if (ProgrammatorView.unsaved)
		{
			AYSWindowManager.THIS.Show("НЕСОХРАНЕННЫЕ ИЗМЕНЕНИЯ", "Вы собираетесь выйти в меню.\nНесохраненные изменения потеряются.\nЧтобы сохранить программу, запустите ее.\n\nВыйти и потерять изменения?", delegate
			{
				this.ExitToMenu();
				ProgrammatorView.unsaved = false;
			});
		}
		this.ExitToMenu();
	}

	// Token: 0x06000213 RID: 531 RVA: 0x000063E4 File Offset: 0x000045E4
	private void ExitToMenu()
	{
		ClientController.CanGoto = false;
		base.gameObject.SetActive(false);
		ProgrammatorView.active = false;
		ProgrammatorView.opened = false;
		ServerTime.THIS.SendTypicalMessage(-1, "Pope", 0, 0, "=");
	}

	// Token: 0x06000214 RID: 532 RVA: 0x0002090C File Offset: 0x0001EB0C
	private void OnCopyButton()
	{
		ClientController.CanGoto = false;
		string message = "Вы собираетесь создать копию программы\nОна появится в общем списке программ\n\nСоздать копию?";
		if (ProgrammatorView.unsaved)
		{
			message = "Вы собираетесь создать копию программы\nОна появится в общем списке программ\n\n<color=#ff8888ff>ПРОГРАММА НЕ СОХРАНЕНА\nИЗМЕНЕНИЯ ПОТЕРЯЮТСЯ</color>\n\nСоздать копию?";
		}
		AYSWindowManager.THIS.Show("СОЗДАНИЕ КОПИИ ПРОГРАММЫ", message, new UnityAction(this.OnCopyProgramm));
	}

	// Token: 0x06000215 RID: 533 RVA: 0x0000641C File Offset: 0x0000461C
	private void OnCopyProgramm()
	{
		base.gameObject.SetActive(false);
		ProgrammatorView.active = false;
		ServerTime.THIS.SendTypicalMessage(-1, "PCOP", 0, 0, ProgrammatorView.programId.ToString());
	}

	// Token: 0x06000216 RID: 534 RVA: 0x0000644D File Offset: 0x0000464D
	private void OnExitButton()
	{
		ClientController.CanGoto = false;
		base.gameObject.SetActive(false);
		ProgrammatorView.active = false;
	}

	// Token: 0x04000376 RID: 886
	public Button StartButton;

	// Token: 0x04000377 RID: 887
	public Button ExitButton;

	// Token: 0x04000378 RID: 888
	public Button ClearButton;

	// Token: 0x04000379 RID: 889
	public Button MenuButton;

	// Token: 0x0400037A RID: 890
	public Button CopyButton;

	// Token: 0x0400037B RID: 891
	public Button RenameButton;

	// Token: 0x0400037C RID: 892
	public Button OpenHelpButton;

	// Token: 0x0400037D RID: 893
	public GameObject HelpPanel;

	// Token: 0x0400037E RID: 894
	public Button OpenWikiButton;

	// Token: 0x0400037F RID: 895
	public Button ExitHelpButton;
}
