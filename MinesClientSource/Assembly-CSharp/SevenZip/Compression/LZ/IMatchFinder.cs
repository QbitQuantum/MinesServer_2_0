using System;

namespace SevenZip.Compression.LZ
{
	// Token: 0x020000C7 RID: 199
	internal interface IMatchFinder : IInWindowStream
	{
		// Token: 0x06000573 RID: 1395
		void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter);

		// Token: 0x06000574 RID: 1396
		uint GetMatches(uint[] distances);

		// Token: 0x06000575 RID: 1397
		void Skip(uint num);
	}
}
