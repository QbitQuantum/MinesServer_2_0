using System;
using System.IO;

namespace SevenZip.Compression.LZ
{
	// Token: 0x020000C6 RID: 198
	internal interface IInWindowStream
	{
		// Token: 0x0600056D RID: 1389
		void SetStream(Stream inStream);

		// Token: 0x0600056E RID: 1390
		void Init();

		// Token: 0x0600056F RID: 1391
		void ReleaseStream();

		// Token: 0x06000570 RID: 1392
		byte GetIndexByte(int index);

		// Token: 0x06000571 RID: 1393
		uint GetMatchLen(int index, uint distance, uint limit);

		// Token: 0x06000572 RID: 1394
		uint GetNumAvailableBytes();
	}
}
