using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZMA
{
    internal abstract class Base
    {
        // Token: 0x060005C8 RID: 1480 RVA: 0x0000865B File Offset: 0x0000685B
        public static uint GetLenToPosState(uint len)
        {
            len -= 2U;
            if (len < 4U)
            {
                return len;
            }
            return 3U;
        }

        // Token: 0x0400071E RID: 1822
        public const uint kNumRepDistances = 4U;

        // Token: 0x0400071F RID: 1823
        public const uint kNumStates = 12U;

        // Token: 0x04000720 RID: 1824
        public const int kNumPosSlotBits = 6;

        // Token: 0x04000721 RID: 1825
        public const int kDicLogSizeMin = 0;

        // Token: 0x04000722 RID: 1826
        public const int kNumLenToPosStatesBits = 2;

        // Token: 0x04000723 RID: 1827
        public const uint kNumLenToPosStates = 4U;

        // Token: 0x04000724 RID: 1828
        public const uint kMatchMinLen = 2U;

        // Token: 0x04000725 RID: 1829
        public const int kNumAlignBits = 4;

        // Token: 0x04000726 RID: 1830
        public const uint kAlignTableSize = 16U;

        // Token: 0x04000727 RID: 1831
        public const uint kAlignMask = 15U;

        // Token: 0x04000728 RID: 1832
        public const uint kStartPosModelIndex = 4U;

        // Token: 0x04000729 RID: 1833
        public const uint kEndPosModelIndex = 14U;

        // Token: 0x0400072A RID: 1834
        public const uint kNumPosModels = 10U;

        // Token: 0x0400072B RID: 1835
        public const uint kNumFullDistances = 128U;

        // Token: 0x0400072C RID: 1836
        public const uint kNumLitPosStatesBitsEncodingMax = 4U;

        // Token: 0x0400072D RID: 1837
        public const uint kNumLitContextBitsMax = 8U;

        // Token: 0x0400072E RID: 1838
        public const int kNumPosStatesBitsMax = 4;

        // Token: 0x0400072F RID: 1839
        public const uint kNumPosStatesMax = 16U;

        // Token: 0x04000730 RID: 1840
        public const int kNumPosStatesBitsEncodingMax = 4;

        // Token: 0x04000731 RID: 1841
        public const uint kNumPosStatesEncodingMax = 16U;

        // Token: 0x04000732 RID: 1842
        public const int kNumLowLenBits = 3;

        // Token: 0x04000733 RID: 1843
        public const int kNumMidLenBits = 3;

        // Token: 0x04000734 RID: 1844
        public const int kNumHighLenBits = 8;

        // Token: 0x04000735 RID: 1845
        public const uint kNumLowLenSymbols = 8U;

        // Token: 0x04000736 RID: 1846
        public const uint kNumMidLenSymbols = 8U;

        // Token: 0x04000737 RID: 1847
        public const uint kNumLenSymbols = 272U;

        // Token: 0x04000738 RID: 1848
        public const uint kMatchMaxLen = 273U;

        // Token: 0x020000DB RID: 219
        public struct State
        {
            // Token: 0x060005CA RID: 1482 RVA: 0x00008669 File Offset: 0x00006869
            public void Init()
            {
                this.Index = 0U;
            }

            // Token: 0x060005CB RID: 1483 RVA: 0x00008672 File Offset: 0x00006872
            public void UpdateChar()
            {
                if (this.Index < 4U)
                {
                    this.Index = 0U;
                    return;
                }
                if (this.Index < 10U)
                {
                    this.Index -= 3U;
                    return;
                }
                this.Index -= 6U;
            }

            // Token: 0x060005CC RID: 1484 RVA: 0x000086AC File Offset: 0x000068AC
            public void UpdateMatch()
            {
                this.Index = ((this.Index < 7U) ? 7U : 10U);
            }

            // Token: 0x060005CD RID: 1485 RVA: 0x000086C2 File Offset: 0x000068C2
            public void UpdateRep()
            {
                this.Index = ((this.Index < 7U) ? 8U : 11U);
            }

            // Token: 0x060005CE RID: 1486 RVA: 0x000086D8 File Offset: 0x000068D8
            public void UpdateShortRep()
            {
                this.Index = ((this.Index < 7U) ? 9U : 11U);
            }

            // Token: 0x060005CF RID: 1487 RVA: 0x000086EF File Offset: 0x000068EF
            public bool IsCharState()
            {
                return this.Index < 7U;
            }

            // Token: 0x04000739 RID: 1849
            public uint Index;
        }
    }
}
