using System;
using UnityEngine;

// Token: 0x0200001D RID: 29
public class GOPool : MonoBehaviour
{
	// Token: 0x060000CE RID: 206 RVA: 0x00005265 File Offset: 0x00003465
	private void Start()
	{
		this.gos = new GameObject[this.use_size];
		this.isFree = new bool[this.use_size];
		this.max_size1 = 0;
	}

	// Token: 0x060000CF RID: 207 RVA: 0x000143C0 File Offset: 0x000125C0
	public GameObject GetFree(out int index)
	{
		int i;
		for (i = 0; i < this.max_size1; i++)
		{
			if (this.isFree[i] && i < this.max_size)
			{
				index = i;
				this.isFree[index] = false;
				this.gos[i].SetActive(true);
				return this.gos[i];
			}
		}
		if (i >= this.max_size)
		{
			index = -1;
			return null;
		}
		index = i;
		this.isFree[index] = false;
		this.max_size1 = index + 1;
		this.gos[i] = UnityEngine.Object.Instantiate<GameObject>(this.prefab);
		return this.gos[i];
	}

	// Token: 0x060000D0 RID: 208 RVA: 0x00005290 File Offset: 0x00003490
	public void Free(int index)
	{
		this.isFree[index] = true;
		this.gos[index].SetActive(false);
	}

	// Token: 0x04000183 RID: 387
	public GameObject prefab;

	// Token: 0x04000184 RID: 388
	private GameObject[] gos;

	// Token: 0x04000185 RID: 389
	private bool[] isFree;

	// Token: 0x04000186 RID: 390
	public int max_size;

	// Token: 0x04000187 RID: 391
	private int max_size1;

	// Token: 0x04000188 RID: 392
	private int use_size = 4096;
}
