using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.CommandLineParser
{
    public class CommandForm
    {
        // Token: 0x060005AB RID: 1451 RVA: 0x000084B6 File Offset: 0x000066B6
        public CommandForm(string idString, bool postStringMode)
        {
            this.IDString = idString;
            this.PostStringMode = postStringMode;
        }

        // Token: 0x040006F8 RID: 1784
        public string IDString = "";

        // Token: 0x040006F9 RID: 1785
        public bool PostStringMode;
    }
}
