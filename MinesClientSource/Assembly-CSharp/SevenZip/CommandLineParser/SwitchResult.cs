using System;
using System.Collections;

namespace SevenZip.CommandLineParser
{
	// Token: 0x020000B3 RID: 179
	public class SwitchResult
	{
		// Token: 0x060004F5 RID: 1269 RVA: 0x000081A1 File Offset: 0x000063A1
		public SwitchResult()
		{
			this.ThereIs = false;
		}

		// Token: 0x0400067C RID: 1660
		public bool ThereIs;

		// Token: 0x0400067D RID: 1661
		public bool WithMinus;

		// Token: 0x0400067E RID: 1662
		public ArrayList PostStrings = new ArrayList();

		// Token: 0x0400067F RID: 1663
		public int PostCharIndex;
	}
}
