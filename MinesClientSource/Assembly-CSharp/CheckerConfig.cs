using System;
using System.Collections.Generic;

// Token: 0x020000EB RID: 235
[Serializable]
public class CheckerConfig
{
	// Token: 0x06000600 RID: 1536 RVA: 0x0003A160 File Offset: 0x00038360
	public CheckerConfig()
	{
		this.tpZeroNotify = true;
		this.packs = new List<PackData>();
		this.notificationHistory = new List<string>();
	}

	// Token: 0x04000823 RID: 2083
	public bool isEnabled = true;

	// Token: 0x04000824 RID: 2084
	public List<PackData> packs;

	// Token: 0x04000825 RID: 2085
	public List<string> notificationHistory;

	// Token: 0x04000826 RID: 2086
	public bool tpZeroNotify;

	// Token: 0x04000827 RID: 2087
	public int tpChargeThreshold = 20;

	// Token: 0x04000828 RID: 2088
	public int tpHealthThreshold = 0;

	// Token: 0x04000829 RID: 2089
	public int weaponChargeThreshold = 30;

	// Token: 0x0400082A RID: 2090
	public int weaponHealthThreshold = 100;

	// Token: 0x0400082B RID: 2091
	public int craftHealthThreshold = 100;

	// Token: 0x0400082C RID: 2092
	public int respawnHealthThreshold = 100;

	// Token: 0x0400082D RID: 2093
	public int marketHealthThreshold = 100;

	// Token: 0x0400082E RID: 2094
	public float notificationCooldown = 5f;
}
