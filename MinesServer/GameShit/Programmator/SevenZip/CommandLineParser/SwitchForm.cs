using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.CommandLineParser
{
    public class SwitchForm
    {
        // Token: 0x060005B4 RID: 1460 RVA: 0x00008502 File Offset: 0x00006702
        public SwitchForm(string idString, SwitchType type, bool multi, int minLen, int maxLen, string postCharSet)
        {
            this.IDString = idString;
            this.Type = type;
            this.Multi = multi;
            this.MinLen = minLen;
            this.MaxLen = maxLen;
            this.PostCharSet = postCharSet;
        }

        // Token: 0x060005B5 RID: 1461 RVA: 0x00008537 File Offset: 0x00006737
        public SwitchForm(string idString, SwitchType type, bool multi, int minLen) : this(idString, type, multi, minLen, 0, "")
        {
        }

        // Token: 0x060005B6 RID: 1462 RVA: 0x0000854A File Offset: 0x0000674A
        public SwitchForm(string idString, SwitchType type, bool multi) : this(idString, type, multi, 0)
        {
        }

        // Token: 0x04000702 RID: 1794
        public string IDString;

        // Token: 0x04000703 RID: 1795
        public SwitchType Type;

        // Token: 0x04000704 RID: 1796
        public bool Multi;

        // Token: 0x04000705 RID: 1797
        public int MinLen;

        // Token: 0x04000706 RID: 1798
        public int MaxLen;

        // Token: 0x04000707 RID: 1799
        public string PostCharSet;
    }
}
