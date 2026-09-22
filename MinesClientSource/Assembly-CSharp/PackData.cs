using System;

// Token: 0x020000EA RID: 234
[Serializable]
public class PackData
{
	// Token: 0x060005FF RID: 1535 RVA: 0x0003A0EC File Offset: 0x000382EC
	public PackData()
	{
		this.id = Guid.NewGuid().ToString();
		this.name = "";
		this.type = PackType.Weapon;
		this.currentHp = 100;
		this.maxHp = 100;
		this.currentCharge = -1;
		this.maxCharge = -1;
		this.isEnabled = true;
		this.hpThreshold = 100;
		this.chargeThreshold = -1;
	}

	// Token: 0x04000819 RID: 2073
	public string id;

	// Token: 0x0400081A RID: 2074
	public string name;

	// Token: 0x0400081B RID: 2075
	public PackType type;

	// Token: 0x0400081C RID: 2076
	public int currentHp;

	// Token: 0x0400081D RID: 2077
	public int maxHp;

	// Token: 0x0400081E RID: 2078
	public int currentCharge;

	// Token: 0x0400081F RID: 2079
	public int maxCharge;

	// Token: 0x04000820 RID: 2080
	public bool isEnabled;

	// Token: 0x04000821 RID: 2081
	public int hpThreshold;

	// Token: 0x04000822 RID: 2082
	public int chargeThreshold;
}
