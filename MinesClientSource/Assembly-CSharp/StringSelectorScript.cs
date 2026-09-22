using System;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

// Token: 0x02000069 RID: 105
public class StringSelectorScript : MonoBehaviour
{
	// Token: 0x060002A5 RID: 677 RVA: 0x00006988 File Offset: 0x00004B88
	public void SetStrings(string[] strings)
	{
		if (strings == null)
		{
			throw new Exception("StringSelectorScript no strings");
		}
		if (strings.Length == 0)
		{
			throw new Exception("StringSelectorScript strings len = 0");
		}
		this.labels = strings;
		if (this.inited)
		{
			this.UpdateLabels();
		}
	}

	// Token: 0x060002A6 RID: 678 RVA: 0x00028784 File Offset: 0x00026984
	private void Start()
	{
		this.inited = true;
		if (this.labels != null)
		{
			this.UpdateLabels();
		}
		this.LessButton.onClick.AddListener(new UnityAction(this.OnLess));
		this.MoreButton.onClick.AddListener(new UnityAction(this.OnMore));
	}

	// Token: 0x060002A7 RID: 679 RVA: 0x000287E0 File Offset: 0x000269E0
	private void OnLess()
	{
		if (this.labels != null)
		{
			this.current--;
			if (this.current < 0)
			{
				this.current = this.labels.Length - 1;
			}
			this.UpdateCurrentLabel();
			this.ChangeEvent.Invoke();
		}
	}

	// Token: 0x060002A8 RID: 680 RVA: 0x00028830 File Offset: 0x00026A30
	private void OnMore()
	{
		if (this.labels != null)
		{
			this.current++;
			if (this.current > this.labels.Length - 1)
			{
				this.current = 0;
			}
			this.UpdateCurrentLabel();
			this.ChangeEvent.Invoke();
		}
	}

	// Token: 0x060002A9 RID: 681 RVA: 0x000069BC File Offset: 0x00004BBC
	private void UpdateLabels()
	{
		if (this.current > this.labels.Length)
		{
			this.current = 0;
		}
		this.UpdateCurrentLabel();
	}

	// Token: 0x060002AA RID: 682 RVA: 0x000069DB File Offset: 0x00004BDB
	private void UpdateCurrentLabel()
	{
		base.gameObject.GetComponent<Text>().text = this.labels[this.current];
	}

	// Token: 0x060002AB RID: 683 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x04000506 RID: 1286
	public Button LessButton;

	// Token: 0x04000507 RID: 1287
	public Button MoreButton;

	// Token: 0x04000508 RID: 1288
	public UnityEvent ChangeEvent = new UnityEvent();

	// Token: 0x04000509 RID: 1289
	public int current;

	// Token: 0x0400050A RID: 1290
	private string[] labels;

	// Token: 0x0400050B RID: 1291
	private bool inited;
}
