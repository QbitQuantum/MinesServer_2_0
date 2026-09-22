using System;

namespace SevenZip.CommandLineParser
{
	// Token: 0x020000AF RID: 175
	public class CommandForm
	{
		// Token: 0x060004E9 RID: 1257 RVA: 0x00008101 File Offset: 0x00006301
		public CommandForm(string idString, bool postStringMode)
		{
			this.IDString = idString;
			this.PostStringMode = postStringMode;
		}

		// Token: 0x0400066C RID: 1644
		public string IDString = "";

		// Token: 0x0400066D RID: 1645
		public bool PostStringMode;
	}
}
