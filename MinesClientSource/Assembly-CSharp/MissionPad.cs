using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000036 RID: 54
public class MissionPad : MonoBehaviour
{
	// Token: 0x0600017A RID: 378 RVA: 0x0001AECC File Offset: 0x000190CC
	private void Start()
	{
		MissionPad.THIS = this;
		base.GetComponent<Button>().onClick.AddListener(delegate()
		{
			ServerTime.THIS.SendTypicalMessage(-1, "Miso", 0, 0, "0");
			TutorialNavigation.CheckHide("_MISO");
		});
		base.gameObject.SetActive(false);
	}

	// Token: 0x0600017B RID: 379 RVA: 0x0001AF1C File Offset: 0x0001911C
	public void UpdateMissionProgress(int exp, int max)
	{
		float x = this.bar.gameObject.transform.parent.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
		Vector2 sizeDelta = this.bar.rectTransform.sizeDelta;
		sizeDelta.x = x * (float)exp / (float)max;
		this.bar.rectTransform.sizeDelta = sizeDelta;
		this.progressText = exp.ToString() + " / " + max.ToString();
		if (max == 1)
		{
			this.progressText = "";
		}
		this.tf.text = this.macroReplacedText();
	}

	// Token: 0x0600017C RID: 380 RVA: 0x0001AFC8 File Offset: 0x000191C8
	public void UpdateMissionPanel(string url, int imgx, int imgy, int progress, string text)
	{
		if (text == "")
		{
			base.gameObject.SetActive(false);
			return;
		}
		base.gameObject.SetActive(true);
		this.macroText = text;
		if (text.Contains("%T%"))
		{
			string s = text.Substring(text.IndexOf("%T%") + 3, text.LastIndexOf('%') - text.IndexOf("%T%") - 3);
			this.endTime = Time.unscaledTime + (float)int.Parse(s);
			this.macroText = text.Substring(0, text.IndexOf("%T%") + 3) + text.Substring(text.LastIndexOf('%') + 1);
			this.needUpdateTimer = true;
		}
		else
		{
			this.needUpdateTimer = false;
		}
		this.progressText = "? / ?";
		this.tf.text = this.macroReplacedText();
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.GetComponent<RectTransform>());
		if (url != "")
		{
			this.webImage.gameObject.SetActive(true);
			this.webImage.SetSizeAndUrl(imgx, imgy, url);
		}
		else
		{
			this.webImage.gameObject.SetActive(false);
		}
		float x = this.bar.gameObject.transform.parent.parent.gameObject.GetComponent<RectTransform>().sizeDelta.x;
		Vector2 sizeDelta = this.bar.rectTransform.sizeDelta;
		sizeDelta.x = x * (float)progress / 100f;
		this.bar.rectTransform.sizeDelta = sizeDelta;
	}

	// Token: 0x0600017D RID: 381 RVA: 0x00005B34 File Offset: 0x00003D34
	private string macroReplacedText()
	{
		return this.macroText.Replace("%T%", this.timeLeft()).Replace("%P%", this.progressText);
	}

	// Token: 0x0600017E RID: 382 RVA: 0x00005B5C File Offset: 0x00003D5C
	private void Update()
	{
		if (this.needUpdateTimer)
		{
			this.tf.text = this.macroReplacedText();
		}
	}

	// Token: 0x0600017F RID: 383 RVA: 0x0001B160 File Offset: 0x00019360
	private string timeLeft()
	{
		int num = Mathf.CeilToInt(this.endTime - Time.unscaledTime);
		if (num < 0)
		{
			num = 0;
		}
		int num2 = Mathf.FloorToInt((float)(num / 3600));
		int s = Mathf.FloorToInt((float)(num / 60)) % 60;
		int s2 = num % 60;
		if (num2 == 0)
		{
			return this.pad(s) + ":" + this.pad(s2);
		}
		return string.Concat(new string[]
		{
			this.pad(num2),
			":",
			this.pad(s),
			":",
			this.pad(s2)
		});
	}

	// Token: 0x06000180 RID: 384 RVA: 0x00005B77 File Offset: 0x00003D77
	private string pad(int s)
	{
		if (s < 10)
		{
			return "0" + s;
		}
		return s.ToString();
	}

	// Token: 0x0400029A RID: 666
	public WebImage webImage;

	// Token: 0x0400029B RID: 667
	public Text tf;

	// Token: 0x0400029C RID: 668
	public RawImage bar;

	// Token: 0x0400029D RID: 669
	public static MissionPad THIS;

	// Token: 0x0400029E RID: 670
	private bool needUpdateTimer;

	// Token: 0x0400029F RID: 671
	private string macroText = "";

	// Token: 0x040002A0 RID: 672
	private float endTime;

	// Token: 0x040002A1 RID: 673
	private string progressText = "? / ?";
}
