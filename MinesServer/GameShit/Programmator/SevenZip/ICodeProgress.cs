using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip
{
    public interface ICodeProgress
    {
        // Token: 0x060005A5 RID: 1445
        void SetProgress(long inSize, long outSize);
    }
}
