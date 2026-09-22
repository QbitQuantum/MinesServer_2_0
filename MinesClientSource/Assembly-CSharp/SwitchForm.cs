using System;

// Token: 0x0200006B RID: 107
public class SwitchForm
{
	// Token: 0x060002B1 RID: 689 RVA: 0x00006A20 File Offset: 0x00004C20
	public SwitchForm(string idString, SwitchType type, bool multi) : this(idString, type, multi, 0)
	{
	}

	// Token: 0x060002B2 RID: 690 RVA: 0x00006A2C File Offset: 0x00004C2C
	public SwitchForm(string idString, SwitchType type, bool multi, int minLen, int maxLen, string postCharSet)
	{
		this.IDString = idString;
		this.Type = type;
		this.Multi = multi;
		this.MinLen = minLen;
		this.MaxLen = maxLen;
		this.PostCharSet = postCharSet;
	}

	// Token: 0x060002B3 RID: 691 RVA: 0x00006A61 File Offset: 0x00004C61
	public SwitchForm(string idString, SwitchType type, bool multi, int minLen) : this(idString, type, multi, minLen, 0, "")
	{
	}

	// Token: 0x0400051B RID: 1307
	public string IDString;

	// Token: 0x0400051C RID: 1308
	public SwitchType Type;

	// Token: 0x0400051D RID: 1309
	public bool Multi;

	// Token: 0x0400051E RID: 1310
	public int MinLen;

	// Token: 0x0400051F RID: 1311
	public int MaxLen;

	// Token: 0x04000520 RID: 1312
	public string PostCharSet;
}
