using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZ
{
    internal interface IInWindowStream
    {
        // Token: 0x0600062F RID: 1583
        void SetStream(Stream inStream);

        // Token: 0x06000630 RID: 1584
        void Init();

        // Token: 0x06000631 RID: 1585
        void ReleaseStream();

        // Token: 0x06000632 RID: 1586
        byte GetIndexByte(int index);

        // Token: 0x06000633 RID: 1587
        uint GetMatchLen(int index, uint distance, uint limit);

        // Token: 0x06000634 RID: 1588
        uint GetNumAvailableBytes();
    }
}
