using System;
using UnityEngine;
using UnityEngine.UI;

// Token: 0x02000070 RID: 112
public class TutorialNavigation : MonoBehaviour
{
	// Token: 0x060002CD RID: 717 RVA: 0x00006B4E File Offset: 0x00004D4E
	private void Start()
	{
		TutorialNavigation.THIS = this;
		base.gameObject.SetActive(false);
		this.lens.SetActive(false);
		this.pad = base.GetComponent<Image>();
		this.NaviArrowRT = this.NaviArrow.GetComponent<RectTransform>();
	}

	// Token: 0x060002CE RID: 718 RVA: 0x00006B8B File Offset: 0x00004D8B
	public void SetNaviArrow(int dx, int dy)
	{
		this.NaviArrow.SetActive(true);
		this.arrowX = dx;
		this.arrowY = dy;
		this.showArrow = true;
	}

	// Token: 0x060002CF RID: 719 RVA: 0x0002CE10 File Offset: 0x0002B010
	public void SetNavi(string text, int dx, int dy, int anchorType, string hideReason)
	{
		TutorialNavigation.hideReason = hideReason;
		if (TutorialNavigation.hideReason.StartsWith(":"))
		{
			int num = int.Parse(TutorialNavigation.hideReason.Split(new char[]
			{
				':'
			})[1]);
			this.hideTime = Time.unscaledTime + (float)num;
		}
		if (anchorType < 10)
		{
			ClientController.THIS.stopAutoMove();
		}
		if (text == "")
		{
			base.gameObject.SetActive(false);
			this.lens.SetActive(false);
			TutorialNavigation.hideReason = "";
			this.hideTime = 0f;
			return;
		}
		base.gameObject.SetActive(true);
		this.lens.SetActive(anchorType < 10);
		this.tf.text = text;
		RectTransform component = base.GetComponent<RectTransform>();
		this.pad = base.GetComponent<Image>();
		switch (anchorType % 10)
		{
		case 0:
			this.pad.sprite = this.rightUpSprite;
			component.anchorMin = new Vector2(1f, 1f);
			component.anchorMax = new Vector2(1f, 1f);
			component.pivot = new Vector2(1f, 1f);
			component.anchoredPosition = new Vector3(-(float)dx, -(float)dy);
			break;
		case 1:
			this.pad.sprite = this.rightDownSprite;
			component.anchorMin = new Vector2(1f, 0f);
			component.anchorMax = new Vector2(1f, 0f);
			component.pivot = new Vector2(1f, 0f);
			component.anchoredPosition = new Vector3(-(float)dx, (float)dy);
			break;
		case 2:
			this.pad.sprite = this.leftDownSprite;
			component.anchorMin = new Vector2(0f, 0f);
			component.anchorMax = new Vector2(0f, 0f);
			component.pivot = new Vector2(0f, 0f);
			component.anchoredPosition = new Vector3((float)dx, (float)dy);
			break;
		case 3:
			this.pad.sprite = this.leftUpSprite;
			component.anchorMin = new Vector2(0f, 1f);
			component.anchorMax = new Vector2(0f, 1f);
			component.pivot = new Vector2(0f, 1f);
			component.anchoredPosition = new Vector3((float)dx, -(float)dy);
			break;
		case 4:
			this.pad.sprite = this.rightUpSprite;
			component.anchorMin = new Vector2(0.5f, 0.5f);
			component.anchorMax = new Vector2(0.5f, 0.5f);
			component.pivot = new Vector2(1f, 1f);
			component.anchoredPosition = new Vector3((float)dx, (float)dy);
			break;
		case 5:
			this.pad.sprite = this.rightDownSprite;
			component.anchorMin = new Vector2(0.5f, 0.5f);
			component.anchorMax = new Vector2(0.5f, 0.5f);
			component.pivot = new Vector2(1f, 0f);
			component.anchoredPosition = new Vector3((float)dx, -(float)dy);
			break;
		case 6:
			this.pad.sprite = this.leftDownSprite;
			component.anchorMin = new Vector2(0.5f, 0.5f);
			component.anchorMax = new Vector2(0.5f, 0.5f);
			component.pivot = new Vector2(0f, 0f);
			component.anchoredPosition = new Vector3(-(float)dx, -(float)dy);
			break;
		case 7:
			this.pad.sprite = this.leftUpSprite;
			component.anchorMin = new Vector2(0.5f, 0.5f);
			component.anchorMax = new Vector2(0.5f, 0.5f);
			component.pivot = new Vector2(0f, 1f);
			component.anchoredPosition = new Vector3(-(float)dx, (float)dy);
			break;
		}
		RectTransform component2 = this.lens.GetComponent<RectTransform>();
		component2.anchorMax = component.anchorMax;
		component2.anchorMin = component.anchorMin;
		component2.anchoredPosition = component.anchoredPosition;
		this.lens.transform.position = base.gameObject.transform.position;
		LayoutRebuilder.ForceRebuildLayoutImmediate(base.gameObject.GetComponent<RectTransform>());
	}

	// Token: 0x060002D0 RID: 720 RVA: 0x00006BAE File Offset: 0x00004DAE
	private void hide()
	{
		base.gameObject.SetActive(false);
		this.lens.SetActive(false);
		TutorialNavigation.hideReason = "";
		this.hideTime = 0f;
		this.HideArrow();
	}

	// Token: 0x060002D1 RID: 721 RVA: 0x00006BE3 File Offset: 0x00004DE3
	public static void CheckHide(string marker)
	{
		if (TutorialNavigation.hideReason.Contains(marker))
		{
			ServerTime.THIS.SendTypicalMessage(-1, "THID", 0, 0, marker);
			TutorialNavigation.THIS.hide();
		}
	}

	// Token: 0x060002D2 RID: 722 RVA: 0x0002D2C8 File Offset: 0x0002B4C8
	private void Update()
	{
		if (this.hideTime != 0f && Time.unscaledTime > this.hideTime)
		{
			ServerTime.THIS.SendTypicalMessage(-1, "THID", 0, 0, "TIMEOVER");
			this.hide();
		}
		if (base.gameObject.activeSelf)
		{
			this.pad.color = new Color(1f, 1f, 1f, 0.9f + 0.1f * Mathf.Sin(12f * Time.time));
		}
	}

	// Token: 0x060002D3 RID: 723 RVA: 0x0002D358 File Offset: 0x0002B558
	public void UpdateArrow()
	{
		if (this.showArrow)
		{
			float num = Camera.main.transform.position.x - 0.5f;
			float num2 = -Camera.main.transform.position.y - 0.5f;
			float num3 = Mathf.Atan2((float)this.arrowY - num2, (float)this.arrowX - num);
			float num4 = Mathf.Sqrt(((float)this.arrowY - num2) * ((float)this.arrowY - num2) + ((float)this.arrowX - num) * ((float)this.arrowX - num));
			float num5 = Mathf.Min(num4 - 2f, 10f) - 0.5f * Mathf.Sin(9f * Time.time);
			this.NaviArrowRT.rotation = Quaternion.Euler(0f, 0f, -180f * num3 / 3.1415927f);
			this.NaviArrowRT.localPosition = new Vector3(16f * num5 * Mathf.Cos(num3), -16f * num5 * Mathf.Sin(num3), 0f);
			if (num4 < 4f)
			{
				this.HideArrow();
			}
		}
	}

	// Token: 0x060002D4 RID: 724 RVA: 0x00006C10 File Offset: 0x00004E10
	private void HideArrow()
	{
		this.NaviArrow.SetActive(false);
		this.showArrow = false;
	}

	// Token: 0x04000554 RID: 1364
	public Sprite leftUpSprite;

	// Token: 0x04000555 RID: 1365
	public Sprite rightUpSprite;

	// Token: 0x04000556 RID: 1366
	public Sprite leftDownSprite;

	// Token: 0x04000557 RID: 1367
	public Sprite rightDownSprite;

	// Token: 0x04000558 RID: 1368
	public GameObject lens;

	// Token: 0x04000559 RID: 1369
	public Text tf;

	// Token: 0x0400055A RID: 1370
	public static TutorialNavigation THIS;

	// Token: 0x0400055B RID: 1371
	public GameObject NaviArrow;

	// Token: 0x0400055C RID: 1372
	private RectTransform NaviArrowRT;

	// Token: 0x0400055D RID: 1373
	public static string hideReason = "";

	// Token: 0x0400055E RID: 1374
	private bool showArrow;

	// Token: 0x0400055F RID: 1375
	private int arrowX;

	// Token: 0x04000560 RID: 1376
	private int arrowY;

	// Token: 0x04000561 RID: 1377
	private float hideTime;

	// Token: 0x04000562 RID: 1378
	private Image pad;
}
