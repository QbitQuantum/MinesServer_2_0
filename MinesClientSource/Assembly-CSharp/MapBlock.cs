using System;

// Token: 0x0200002E RID: 46
public class MapBlock
{
	// Token: 0x06000144 RID: 324 RVA: 0x0000597F File Offset: 0x00003B7F
	public MapBlock()
	{
		this.isLoaded = false;
		this.notSaved = false;
	}

	// Token: 0x06000145 RID: 325 RVA: 0x00005995 File Offset: 0x00003B95
	public void Init()
	{
		this.isLoaded = true;
		this.notSaved = true;
		this.data = new byte[1024];
	}

	// Token: 0x0400023A RID: 570
	public byte[] data;

	// Token: 0x0400023B RID: 571
	public bool notSaved;

	// Token: 0x0400023C RID: 572
	public bool isLoaded;
}
