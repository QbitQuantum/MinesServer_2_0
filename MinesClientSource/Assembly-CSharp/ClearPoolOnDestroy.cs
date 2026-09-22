using System;
using UnityEngine;

// Token: 0x02000010 RID: 16
public class ClearPoolOnDestroy : MonoBehaviour
{
	// Token: 0x06000060 RID: 96 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Start()
	{
	}

	// Token: 0x06000061 RID: 97 RVA: 0x00004B5F File Offset: 0x00002D5F
	private void Update()
	{
	}

	// Token: 0x06000062 RID: 98 RVA: 0x00004F10 File Offset: 0x00003110
	private void OnDestroy()
	{
		this.pool.Free(this.index);
	}

	// Token: 0x04000066 RID: 102
	public GOPool pool;

	// Token: 0x04000067 RID: 103
	public int index;
}
