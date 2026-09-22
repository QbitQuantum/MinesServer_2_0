using System;

namespace SevenZip.CommandLineParser
{
	// Token: 0x020000B2 RID: 178
	public class SwitchForm
	{
		// Token: 0x060004F2 RID: 1266 RVA: 0x0000814D File Offset: 0x0000634D
		public SwitchForm(string idString, SwitchType type, bool multi, int minLen, int maxLen, string postCharSet)
		{
			this.IDString = idString;
			this.Type = type;
			this.Multi = multi;
			this.MinLen = minLen;
			this.MaxLen = maxLen;
			this.PostCharSet = postCharSet;
		}

		// Token: 0x060004F3 RID: 1267 RVA: 0x00008182 File Offset: 0x00006382
		public SwitchForm(string idString, SwitchType type, bool multi, int minLen) : this(idString, type, multi, minLen, 0, "")
		{
		}

		// Token: 0x060004F4 RID: 1268 RVA: 0x00008195 File Offset: 0x00006395
		public SwitchForm(string idString, SwitchType type, bool multi) : this(idString, type, multi, 0)
		{
		}

		// Token: 0x04000676 RID: 1654
		public string IDString;

		// Token: 0x04000677 RID: 1655
		public SwitchType Type;

		// Token: 0x04000678 RID: 1656
		public bool Multi;

		// Token: 0x04000679 RID: 1657
		public int MinLen;

		// Token: 0x0400067A RID: 1658
		public int MaxLen;

		// Token: 0x0400067B RID: 1659
		public string PostCharSet;
	}
}
