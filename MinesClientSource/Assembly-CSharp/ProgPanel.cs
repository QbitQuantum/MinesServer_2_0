using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000058 RID: 88
public class ProgPanel : MonoBehaviour
{
	// Token: 0x06000205 RID: 517 RVA: 0x00006275 File Offset: 0x00004475
	private void Start()
	{
		ProgPanel.THIS = this;
		this.handModeImage.gameObject.SetActive(false);
		this.playStopButton.onClick.AddListener(delegate()
		{
			ClientController.CanGoto = false;
			this.OnPlayStop();
		});
	}

	// Token: 0x06000206 RID: 518 RVA: 0x000062AA File Offset: 0x000044AA
	private void onPlayStopBut()
	{
		ClientController.CanGoto = false;
		this.OnPlayStop();
	}

	// Token: 0x06000207 RID: 519 RVA: 0x000206D0 File Offset: 0x0001E8D0
	public void OnPlayStop()
	{
		if (ProgPanel.playing || ProgPanel.handMode)
		{
			GUIManager.THIS.OnProgCloseButton();
			return;
		}
		if (GUIManager.programToSend.StartsWith("@"))
		{
			ServerTime.THIS.SendTypicalMessage(-1, "Pope", 0, 0, "@" + GUIManager.programToSend);
			Debug.Log("START!!!");
			return;
		}
		ProgrammatorView.THIS.SendAndStartProgram();
		Debug.Log("START!!!");
	}

	// Token: 0x06000208 RID: 520 RVA: 0x00020748 File Offset: 0x0001E948
	private void Update()
	{
		this.handModeImage.gameObject.SetActive(ProgPanel.handMode);
		if (ProgPanel.playing || ProgPanel.handMode)
		{
			this.frame++;
			this.progImage.sprite = this.progs[this.frame / 5 % this.progs.Length];
			this.playStopImage.sprite = this.stop;
			return;
		}
		this.progImage.sprite = this.progStable;
		this.playStopImage.sprite = this.play;
	}

	// Token: 0x04000367 RID: 871
	public static bool playing;

	// Token: 0x04000368 RID: 872
	public static bool handMode;

	// Token: 0x04000369 RID: 873
	public Sprite stop;

	// Token: 0x0400036A RID: 874
	public Sprite play;

	// Token: 0x0400036B RID: 875
	public Sprite progStable;

	// Token: 0x0400036C RID: 876
	public Button playStopButton;

	// Token: 0x0400036D RID: 877
	public Sprite[] progs;

	// Token: 0x0400036E RID: 878
	public Image progImage;

	// Token: 0x0400036F RID: 879
	public Image playStopImage;

	// Token: 0x04000370 RID: 880
	public Image handModeImage;

	// Token: 0x04000371 RID: 881
	public static ProgPanel THIS;

	// Token: 0x04000372 RID: 882
	private int frame;
}
