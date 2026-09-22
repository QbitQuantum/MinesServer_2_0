using System;
using System.Collections;

// Token: 0x0200006C RID: 108
public class SwitchResult
{
	// Token: 0x060002B4 RID: 692 RVA: 0x00006A74 File Offset: 0x00004C74
	public SwitchResult()
	{
		this.ThereIs = false;
	}

	// Token: 0x04000521 RID: 1313
	public bool ThereIs;

	// Token: 0x04000522 RID: 1314
	public bool WithMinus;

	// Token: 0x04000523 RID: 1315
	public ArrayList PostStrings = new ArrayList();

	// Token: 0x04000524 RID: 1316
	public int PostCharIndex;
}
