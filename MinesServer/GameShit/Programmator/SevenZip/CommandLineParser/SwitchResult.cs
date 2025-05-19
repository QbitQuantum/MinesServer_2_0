using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.CommandLineParser
{
    public class SwitchResult
    {
        // Token: 0x060005B7 RID: 1463 RVA: 0x00008556 File Offset: 0x00006756
        public SwitchResult()
        {
            this.ThereIs = false;
        }

        // Token: 0x04000708 RID: 1800
        public bool ThereIs;

        // Token: 0x04000709 RID: 1801
        public bool WithMinus;

        // Token: 0x0400070A RID: 1802
        public ArrayList PostStrings = new ArrayList();

        // Token: 0x0400070B RID: 1803
        public int PostCharIndex;
    }
}
