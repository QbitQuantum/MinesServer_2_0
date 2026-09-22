using System;
using UnityEngine;

// Token: 0x02000076 RID: 118
public class WindowActivator : MonoBehaviour
{
	// Token: 0x060002EB RID: 747 RVA: 0x0002D928 File Offset: 0x0002BB28
	private void Start()
	{
		this.programmatorWindow.gameObject.SetActive(true);
		this.mapWindow.gameObject.SetActive(true);
		this.okWindow.gameObject.SetActive(true);
		this.aysWindow.gameObject.SetActive(true);
	}

	// Token: 0x060002EC RID: 748 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x0400057E RID: 1406
	public GameObject programmatorWindow;

	// Token: 0x0400057F RID: 1407
	public GameObject mapWindow;

	// Token: 0x04000580 RID: 1408
	public GameObject okWindow;

	// Token: 0x04000581 RID: 1409
	public GameObject aysWindow;
}
