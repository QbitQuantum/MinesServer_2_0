using System;
using System.IO;

namespace SevenZip
{
	// Token: 0x020000AA RID: 170
	public interface ICoder
	{
		// Token: 0x060004E4 RID: 1252
		void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress);
	}
}
