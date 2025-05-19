using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZ
{
    internal interface IMatchFinder : IInWindowStream
    {
        // Token: 0x06000635 RID: 1589
        void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter);

        // Token: 0x06000636 RID: 1590
        uint GetMatches(uint[] distances);

        // Token: 0x06000637 RID: 1591
        void Skip(uint num);
    }
}
