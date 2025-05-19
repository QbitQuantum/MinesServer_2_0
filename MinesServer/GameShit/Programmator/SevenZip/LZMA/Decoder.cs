using MinesServer.GameShit.Programmator.SevenZip.LZ;
using MinesServer.GameShit.Programmator.SevenZip.RangeCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;


namespace MinesServer.GameShit.Programmator.SevenZip.LZMA
{
    public class Decoder : ICoder, ISetDecoderProperties
    {
        // Token: 0x060005D0 RID: 1488 RVA: 0x0003A2D4 File Offset: 0x000384D4
        public Decoder()
        {
            this.m_DictionarySize = uint.MaxValue;
            int num = 0;
            while ((long)num < 4L)
            {
                this.m_PosSlotDecoder[num] = new BitTreeDecoder(6);
                num++;
            }
        }

        // Token: 0x060005D1 RID: 1489 RVA: 0x0003A3C0 File Offset: 0x000385C0
        private void SetDictionarySize(uint dictionarySize)
        {
            if (this.m_DictionarySize != dictionarySize)
            {
                this.m_DictionarySize = dictionarySize;
                this.m_DictionarySizeCheck = Math.Max(this.m_DictionarySize, 1U);
                uint windowSize = Math.Max(this.m_DictionarySizeCheck, 4096U);
                this.m_OutWindow.Create(windowSize);
            }
        }

        // Token: 0x060005D2 RID: 1490 RVA: 0x000086FA File Offset: 0x000068FA
        private void SetLiteralProperties(int lp, int lc)
        {
            if (lp > 8)
            {
                throw new InvalidParamException();
            }
            if (lc > 8)
            {
                throw new InvalidParamException();
            }
            this.m_LiteralDecoder.Create(lp, lc);
        }

        // Token: 0x060005D3 RID: 1491 RVA: 0x0003A40C File Offset: 0x0003860C
        private void SetPosBitsProperties(int pb)
        {
            if (pb > 4)
            {
                throw new InvalidParamException();
            }
            uint num = 1U << pb;
            this.m_LenDecoder.Create(num);
            this.m_RepLenDecoder.Create(num);
            this.m_PosStateMask = num - 1U;
        }

        // Token: 0x060005D4 RID: 1492 RVA: 0x0003A44C File Offset: 0x0003864C
        private void Init(Stream inStream, Stream outStream)
        {
            this.m_RangeDecoder.Init(inStream);
            this.m_OutWindow.Init(outStream, this._solid);
            for (uint num = 0U; num < 12U; num += 1U)
            {
                for (uint num2 = 0U; num2 <= this.m_PosStateMask; num2 += 1U)
                {
                    uint num3 = (num << 4) + num2;
                    this.m_IsMatchDecoders[(int)num3].Init();
                    this.m_IsRep0LongDecoders[(int)num3].Init();
                }
                this.m_IsRepDecoders[(int)num].Init();
                this.m_IsRepG0Decoders[(int)num].Init();
                this.m_IsRepG1Decoders[(int)num].Init();
                this.m_IsRepG2Decoders[(int)num].Init();
            }
            this.m_LiteralDecoder.Init();
            for (uint num4 = 0U; num4 < 4U; num4 += 1U)
            {
                this.m_PosSlotDecoder[(int)num4].Init();
            }
            for (uint num5 = 0U; num5 < 114U; num5 += 1U)
            {
                this.m_PosDecoders[(int)num5].Init();
            }
            this.m_LenDecoder.Init();
            this.m_RepLenDecoder.Init();
            this.m_PosAlignDecoder.Init();
        }

        // Token: 0x060005D5 RID: 1493 RVA: 0x0003A574 File Offset: 0x00038774
        public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
        {
            this.Init(inStream, outStream);
            Base.State state = default(Base.State);
            state.Init();
            uint num = 0U;
            uint num2 = 0U;
            uint num3 = 0U;
            uint num4 = 0U;
            ulong num5 = 0UL;
            if (num5 < (ulong)outSize)
            {
                if (this.m_IsMatchDecoders[(int)((int)state.Index << 4)].Decode(this.m_RangeDecoder) != 0U)
                {
                    throw new DataErrorException();
                }
                state.UpdateChar();
                byte b = this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, 0U, 0);
                this.m_OutWindow.PutByte(b);
                num5 += 1UL;
            }
            while (num5 < (ulong)outSize)
            {
                uint num6 = (uint)((int)num5 & (int)this.m_PosStateMask);
                if (this.m_IsMatchDecoders[(int)((state.Index << 4) + num6)].Decode(this.m_RangeDecoder) == 0U)
                {
                    byte @byte = this.m_OutWindow.GetByte(0U);
                    byte b2 = state.IsCharState() ? this.m_LiteralDecoder.DecodeNormal(this.m_RangeDecoder, (uint)num5, @byte) : this.m_LiteralDecoder.DecodeWithMatchByte(this.m_RangeDecoder, (uint)num5, @byte, this.m_OutWindow.GetByte(num));
                    this.m_OutWindow.PutByte(b2);
                    state.UpdateChar();
                    num5 += 1UL;
                }
                else
                {
                    uint num8;
                    if (this.m_IsRepDecoders[(int)state.Index].Decode(this.m_RangeDecoder) == 1U)
                    {
                        if (this.m_IsRepG0Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
                        {
                            if (this.m_IsRep0LongDecoders[(int)((state.Index << 4) + num6)].Decode(this.m_RangeDecoder) == 0U)
                            {
                                state.UpdateShortRep();
                                this.m_OutWindow.PutByte(this.m_OutWindow.GetByte(num));
                                num5 += 1UL;
                                continue;
                            }
                        }
                        else
                        {
                            uint num7;
                            if (this.m_IsRepG1Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
                            {
                                num7 = num2;
                            }
                            else
                            {
                                if (this.m_IsRepG2Decoders[(int)state.Index].Decode(this.m_RangeDecoder) == 0U)
                                {
                                    num7 = num3;
                                }
                                else
                                {
                                    num7 = num4;
                                    num4 = num3;
                                }
                                num3 = num2;
                            }
                            num2 = num;
                            num = num7;
                        }
                        num8 = this.m_RepLenDecoder.Decode(this.m_RangeDecoder, num6) + 2U;
                        state.UpdateRep();
                    }
                    else
                    {
                        num4 = num3;
                        num3 = num2;
                        num2 = num;
                        num8 = 2U + this.m_LenDecoder.Decode(this.m_RangeDecoder, num6);
                        state.UpdateMatch();
                        uint num9 = this.m_PosSlotDecoder[(int)Base.GetLenToPosState(num8)].Decode(this.m_RangeDecoder);
                        if (num9 >= 4U)
                        {
                            int num10 = (int)((num9 >> 1) - 1U);
                            num = (2U | (num9 & 1U)) << num10;
                            if (num9 < 14U)
                            {
                                num += BitTreeDecoder.ReverseDecode(this.m_PosDecoders, num - num9 - 1U, this.m_RangeDecoder, num10);
                            }
                            else
                            {
                                num += this.m_RangeDecoder.DecodeDirectBits(num10 - 4) << 4;
                                num += this.m_PosAlignDecoder.ReverseDecode(this.m_RangeDecoder);
                            }
                        }
                        else
                        {
                            num = num9;
                        }
                    }
                    if ((ulong)num >= (ulong)this.m_OutWindow.TrainSize + num5 || num >= this.m_DictionarySizeCheck)
                    {
                        if (num != 4294967295U)
                        {
                            throw new DataErrorException();
                        }
                        break;
                    }
                    else
                    {
                        this.m_OutWindow.CopyBlock(num, num8);
                        num5 += (ulong)num8;
                    }
                }
            }
            this.m_OutWindow.Flush();
            this.m_OutWindow.ReleaseStream();
            this.m_RangeDecoder.ReleaseStream();
        }

        // Token: 0x060005D6 RID: 1494 RVA: 0x0003A8C4 File Offset: 0x00038AC4
        public void SetDecoderProperties(byte[] properties)
        {
            if (properties.Length < 5)
            {
                throw new InvalidParamException();
            }
            int lc = (int)(properties[0] % 9);
            int b = properties[0] / 9;
            int lp = (int)(b % 5);
            int num = (int)(b / 5);
            if (num > 4)
            {
                throw new InvalidParamException();
            }
            uint num2 = 0U;
            for (int i = 0; i < 4; i++)
            {
                num2 += (uint)((uint)properties[1 + i] << i * 8);
            }
            this.SetDictionarySize(num2);
            this.SetLiteralProperties(lp, lc);
            this.SetPosBitsProperties(num);
        }

        // Token: 0x060005D7 RID: 1495 RVA: 0x0000871D File Offset: 0x0000691D
        public bool Train(Stream stream)
        {
            this._solid = true;
            return this.m_OutWindow.Train(stream);
        }

        // Token: 0x0400073A RID: 1850
        private OutWindow m_OutWindow = new OutWindow();

        // Token: 0x0400073B RID: 1851
        private RangeCoder.Decoder m_RangeDecoder = new RangeCoder.Decoder();

        // Token: 0x0400073C RID: 1852
        private BitDecoder[] m_IsMatchDecoders = new BitDecoder[192];

        // Token: 0x0400073D RID: 1853
        private BitDecoder[] m_IsRepDecoders = new BitDecoder[12];

        // Token: 0x0400073E RID: 1854
        private BitDecoder[] m_IsRepG0Decoders = new BitDecoder[12];

        // Token: 0x0400073F RID: 1855
        private BitDecoder[] m_IsRepG1Decoders = new BitDecoder[12];

        // Token: 0x04000740 RID: 1856
        private BitDecoder[] m_IsRepG2Decoders = new BitDecoder[12];

        // Token: 0x04000741 RID: 1857
        private BitDecoder[] m_IsRep0LongDecoders = new BitDecoder[192];

        // Token: 0x04000742 RID: 1858
        private BitTreeDecoder[] m_PosSlotDecoder = new BitTreeDecoder[4];

        // Token: 0x04000743 RID: 1859
        private BitDecoder[] m_PosDecoders = new BitDecoder[114];

        // Token: 0x04000744 RID: 1860
        private BitTreeDecoder m_PosAlignDecoder = new BitTreeDecoder(4);

        // Token: 0x04000745 RID: 1861
        private Decoder.LenDecoder m_LenDecoder = new Decoder.LenDecoder();

        // Token: 0x04000746 RID: 1862
        private Decoder.LenDecoder m_RepLenDecoder = new Decoder.LenDecoder();

        // Token: 0x04000747 RID: 1863
        private Decoder.LiteralDecoder m_LiteralDecoder = new Decoder.LiteralDecoder();

        // Token: 0x04000748 RID: 1864
        private uint m_DictionarySize;

        // Token: 0x04000749 RID: 1865
        private uint m_DictionarySizeCheck;

        // Token: 0x0400074A RID: 1866
        private uint m_PosStateMask;

        // Token: 0x0400074B RID: 1867
        private bool _solid;

        // Token: 0x020000DD RID: 221
        private class LenDecoder
        {
            // Token: 0x060005D8 RID: 1496 RVA: 0x0003A934 File Offset: 0x00038B34
            public void Create(uint numPosStates)
            {
                for (uint num = this.m_NumPosStates; num < numPosStates; num += 1U)
                {
                    this.m_LowCoder[(int)num] = new BitTreeDecoder(3);
                    this.m_MidCoder[(int)num] = new BitTreeDecoder(3);
                }
                this.m_NumPosStates = numPosStates;
            }

            // Token: 0x060005D9 RID: 1497 RVA: 0x0003A980 File Offset: 0x00038B80
            public void Init()
            {
                this.m_Choice.Init();
                for (uint num = 0U; num < this.m_NumPosStates; num += 1U)
                {
                    this.m_LowCoder[(int)num].Init();
                    this.m_MidCoder[(int)num].Init();
                }
                this.m_Choice2.Init();
                this.m_HighCoder.Init();
            }

            // Token: 0x060005DA RID: 1498 RVA: 0x0003A9E4 File Offset: 0x00038BE4
            public uint Decode(RangeCoder.Decoder rangeDecoder, uint posState)
            {
                if (this.m_Choice.Decode(rangeDecoder) == 0U)
                {
                    return this.m_LowCoder[(int)posState].Decode(rangeDecoder);
                }
                uint num = 8U;
                if (this.m_Choice2.Decode(rangeDecoder) == 0U)
                {
                    return num + this.m_MidCoder[(int)posState].Decode(rangeDecoder);
                }
                num += 8U;
                return num + this.m_HighCoder.Decode(rangeDecoder);
            }

            // Token: 0x0400074C RID: 1868
            private BitDecoder m_Choice;

            // Token: 0x0400074D RID: 1869
            private BitDecoder m_Choice2;

            // Token: 0x0400074E RID: 1870
            private BitTreeDecoder[] m_LowCoder = new BitTreeDecoder[16];

            // Token: 0x0400074F RID: 1871
            private BitTreeDecoder[] m_MidCoder = new BitTreeDecoder[16];

            // Token: 0x04000750 RID: 1872
            private BitTreeDecoder m_HighCoder = new BitTreeDecoder(8);

            // Token: 0x04000751 RID: 1873
            private uint m_NumPosStates;
        }

        // Token: 0x020000DE RID: 222
        private class LiteralDecoder
        {
            // Token: 0x060005DC RID: 1500 RVA: 0x0003AA4C File Offset: 0x00038C4C
            public void Create(int numPosBits, int numPrevBits)
            {
                if (this.m_Coders == null || this.m_NumPrevBits != numPrevBits || this.m_NumPosBits != numPosBits)
                {
                    this.m_NumPosBits = numPosBits;
                    this.m_PosMask = (1U << numPosBits) - 1U;
                    this.m_NumPrevBits = numPrevBits;
                    uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
                    this.m_Coders = new Decoder.LiteralDecoder.Decoder2[num];
                    for (uint num2 = 0U; num2 < num; num2 += 1U)
                    {
                        this.m_Coders[(int)num2].Create();
                    }
                }
            }

            // Token: 0x060005DD RID: 1501 RVA: 0x0003AACC File Offset: 0x00038CCC
            public void Init()
            {
                uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
                for (uint num2 = 0U; num2 < num; num2 += 1U)
                {
                    this.m_Coders[(int)num2].Init();
                }
            }

            // Token: 0x060005DE RID: 1502 RVA: 0x00008760 File Offset: 0x00006960
            private uint GetState(uint pos, byte prevByte)
            {
                return ((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits);
            }

            // Token: 0x060005DF RID: 1503 RVA: 0x00008782 File Offset: 0x00006982
            public byte DecodeNormal(RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte)
            {
                return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeNormal(rangeDecoder);
            }

            // Token: 0x060005E0 RID: 1504 RVA: 0x0000879D File Offset: 0x0000699D
            public byte DecodeWithMatchByte(RangeCoder.Decoder rangeDecoder, uint pos, byte prevByte, byte matchByte)
            {
                return this.m_Coders[(int)this.GetState(pos, prevByte)].DecodeWithMatchByte(rangeDecoder, matchByte);
            }

            // Token: 0x04000752 RID: 1874
            private Decoder.LiteralDecoder.Decoder2[] m_Coders;

            // Token: 0x04000753 RID: 1875
            private int m_NumPrevBits;

            // Token: 0x04000754 RID: 1876
            private int m_NumPosBits;

            // Token: 0x04000755 RID: 1877
            private uint m_PosMask;

            // Token: 0x020000DF RID: 223
            private struct Decoder2
            {
                // Token: 0x060005E2 RID: 1506 RVA: 0x000087BA File Offset: 0x000069BA
                public void Create()
                {
                    this.m_Decoders = new BitDecoder[768];
                }

                // Token: 0x060005E3 RID: 1507 RVA: 0x0003AB0C File Offset: 0x00038D0C
                public void Init()
                {
                    for (int i = 0; i < 768; i++)
                    {
                        this.m_Decoders[i].Init();
                    }
                }

                // Token: 0x060005E4 RID: 1508 RVA: 0x0003AB3C File Offset: 0x00038D3C
                public byte DecodeNormal(RangeCoder.Decoder rangeDecoder)
                {
                    uint num = 1U;
                    do
                    {
                        num = (num << 1 | this.m_Decoders[(int)num].Decode(rangeDecoder));
                    }
                    while (num < 256U);
                    return (byte)num;
                }

                // Token: 0x060005E5 RID: 1509 RVA: 0x0003AB6C File Offset: 0x00038D6C
                public byte DecodeWithMatchByte(RangeCoder.Decoder rangeDecoder, byte matchByte)
                {
                    uint num = 1U;
                    for (; ; )
                    {
                        uint num2 = (uint)(matchByte >> 7 & 1);
                        matchByte = (byte)(matchByte << 1);
                        uint num3 = this.m_Decoders[(int)((1U + num2 << 8) + num)].Decode(rangeDecoder);
                        num = (num << 1 | num3);
                        if (num2 != num3)
                        {
                            break;
                        }
                        if (num >= 256U)
                        {
                            goto IL_5C;
                        }
                    }
                    while (num < 256U)
                    {
                        num = (num << 1 | this.m_Decoders[(int)num].Decode(rangeDecoder));
                    }
                IL_5C:
                    return (byte)num;
                }

                // Token: 0x04000756 RID: 1878
                private BitDecoder[] m_Decoders;
            }
        }
    }
}
