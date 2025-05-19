using MinesServer.GameShit.Programmator.SevenZip.LZ;
using MinesServer.GameShit.Programmator.SevenZip.RangeCoder;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZMA
{
    public class Encoder : ICoder, ISetCoderProperties, IWriteCoderProperties
    {
        // Token: 0x060005E6 RID: 1510 RVA: 0x0003ABD8 File Offset: 0x00038DD8
        static Encoder()
        {
            int num = 2;
            Encoder.g_FastPos[0] = 0;
            Encoder.g_FastPos[1] = 1;
            for (byte b = 2; b < 22; b += 1)
            {
                uint num2 = 1U << (b >> 1) - 1;
                uint num3 = 0U;
                while (num3 < num2)
                {
                    Encoder.g_FastPos[num] = b;
                    num3 += 1U;
                    num++;
                }
            }
        }

        // Token: 0x060005E7 RID: 1511 RVA: 0x000087CC File Offset: 0x000069CC
        private static uint GetPosSlot(uint pos)
        {
            if (pos < 2048U)
            {
                return (uint)Encoder.g_FastPos[(int)pos];
            }
            if (pos < 2097152U)
            {
                return (uint)(Encoder.g_FastPos[(int)(pos >> 10)] + 20);
            }
            return (uint)(Encoder.g_FastPos[(int)(pos >> 20)] + 40);
        }

        // Token: 0x060005E8 RID: 1512 RVA: 0x00008801 File Offset: 0x00006A01
        private static uint GetPosSlot2(uint pos)
        {
            if (pos < 131072U)
            {
                return (uint)(Encoder.g_FastPos[(int)(pos >> 6)] + 12);
            }
            if (pos < 134217728U)
            {
                return (uint)(Encoder.g_FastPos[(int)(pos >> 16)] + 32);
            }
            return (uint)(Encoder.g_FastPos[(int)(pos >> 26)] + 52);
        }

        // Token: 0x060005E9 RID: 1513 RVA: 0x0003AC54 File Offset: 0x00038E54
        private void BaseInit()
        {
            this._state.Init();
            this._previousByte = 0;
            for (uint num = 0U; num < 4U; num += 1U)
            {
                this._repDistances[(int)num] = 0U;
            }
        }

        // Token: 0x060005EA RID: 1514 RVA: 0x0003AC88 File Offset: 0x00038E88
        private void Create()
        {
            if (this._matchFinder == null)
            {
                BinTree binTree = new BinTree();
                int type = 4;
                if (this._matchFinderType == Encoder.EMatchFinderType.BT2)
                {
                    type = 2;
                }
                binTree.SetType(type);
                this._matchFinder = binTree;
            }
            this._literalEncoder.Create(this._numLiteralPosStateBits, this._numLiteralContextBits);
            if (this._dictionarySize != this._dictionarySizePrev || this._numFastBytesPrev != this._numFastBytes)
            {
                this._matchFinder.Create(this._dictionarySize, 4096U, this._numFastBytes, 274U);
                this._dictionarySizePrev = this._dictionarySize;
                this._numFastBytesPrev = this._numFastBytes;
            }
        }

        // Token: 0x060005EB RID: 1515 RVA: 0x0003AD2C File Offset: 0x00038F2C
        public Encoder()
        {
            int num = 0;
            while ((long)num < 4096L)
            {
                this._optimum[num] = new Encoder.Optimal();
                num++;
            }
            int num2 = 0;
            while ((long)num2 < 4L)
            {
                this._posSlotEncoder[num2] = new BitTreeEncoder(6);
                num2++;
            }
        }

        // Token: 0x060005EC RID: 1516 RVA: 0x0000883B File Offset: 0x00006A3B
        private void SetWriteEndMarkerMode(bool writeEndMarker)
        {
            this._writeEndMark = writeEndMarker;
        }

        // Token: 0x060005ED RID: 1517 RVA: 0x0003AEF8 File Offset: 0x000390F8
        private void Init()
        {
            this.BaseInit();
            this._rangeEncoder.Init();
            for (uint num = 0U; num < 12U; num += 1U)
            {
                for (uint num2 = 0U; num2 <= this._posStateMask; num2 += 1U)
                {
                    uint num3 = (num << 4) + num2;
                    this._isMatch[(int)num3].Init();
                    this._isRep0Long[(int)num3].Init();
                }
                this._isRep[(int)num].Init();
                this._isRepG0[(int)num].Init();
                this._isRepG1[(int)num].Init();
                this._isRepG2[(int)num].Init();
            }
            this._literalEncoder.Init();
            for (uint num4 = 0U; num4 < 4U; num4 += 1U)
            {
                this._posSlotEncoder[(int)num4].Init();
            }
            for (uint num5 = 0U; num5 < 114U; num5 += 1U)
            {
                this._posEncoders[(int)num5].Init();
            }
            this._lenEncoder.Init(1U << this._posStateBits);
            this._repMatchLenEncoder.Init(1U << this._posStateBits);
            this._posAlignEncoder.Init();
            this._longestMatchWasFound = false;
            this._optimumEndIndex = 0U;
            this._optimumCurrentIndex = 0U;
            this._additionalOffset = 0U;
        }

        // Token: 0x060005EE RID: 1518 RVA: 0x0003B044 File Offset: 0x00039244
        private void ReadMatchDistances(out uint lenRes, out uint numDistancePairs)
        {
            lenRes = 0U;
            numDistancePairs = this._matchFinder.GetMatches(this._matchDistances);
            if (numDistancePairs != 0U)
            {
                lenRes = this._matchDistances[(int)(numDistancePairs - 2U)];
                if (lenRes == this._numFastBytes)
                {
                    lenRes += this._matchFinder.GetMatchLen((int)(lenRes - 1U), this._matchDistances[(int)(numDistancePairs - 1U)], 273U - lenRes);
                }
            }
            this._additionalOffset += 1U;
        }

        // Token: 0x060005EF RID: 1519 RVA: 0x00008844 File Offset: 0x00006A44
        private void MovePos(uint num)
        {
            if (num != 0U)
            {
                this._matchFinder.Skip(num);
                this._additionalOffset += num;
            }
        }

        // Token: 0x060005F0 RID: 1520 RVA: 0x00008863 File Offset: 0x00006A63
        private uint GetRepLen1Price(Base.State state, uint posState)
        {
            return this._isRepG0[(int)state.Index].GetPrice0() + this._isRep0Long[(int)((state.Index << 4) + posState)].GetPrice0();
        }

        // Token: 0x060005F1 RID: 1521 RVA: 0x0003B0B8 File Offset: 0x000392B8
        private uint GetPureRepPrice(uint repIndex, Base.State state, uint posState)
        {
            uint num;
            if (repIndex == 0U)
            {
                num = this._isRepG0[(int)state.Index].GetPrice0();
                return num + this._isRep0Long[(int)((state.Index << 4) + posState)].GetPrice1();
            }
            num = this._isRepG0[(int)state.Index].GetPrice1();
            if (repIndex == 1U)
            {
                return num + this._isRepG1[(int)state.Index].GetPrice0();
            }
            num += this._isRepG1[(int)state.Index].GetPrice1();
            return num + this._isRepG2[(int)state.Index].GetPrice(repIndex - 2U);
        }

        // Token: 0x060005F2 RID: 1522 RVA: 0x00008896 File Offset: 0x00006A96
        private uint GetRepPrice(uint repIndex, uint len, Base.State state, uint posState)
        {
            return this._repMatchLenEncoder.GetPrice(len - 2U, posState) + this.GetPureRepPrice(repIndex, state, posState);
        }

        // Token: 0x060005F3 RID: 1523 RVA: 0x0003B164 File Offset: 0x00039364
        private uint GetPosLenPrice(uint pos, uint len, uint posState)
        {
            uint lenToPosState = Base.GetLenToPosState(len);
            return ((pos >= 128U) ? (this._posSlotPrices[(int)((lenToPosState << 6) + Encoder.GetPosSlot2(pos))] + this._alignPrices[(int)(pos & 15U)]) : this._distancesPrices[(int)(lenToPosState * 128U + pos)]) + this._lenEncoder.GetPrice(len - 2U, posState);
        }

        // Token: 0x060005F4 RID: 1524 RVA: 0x0003B1C0 File Offset: 0x000393C0
        private uint Backward(out uint backRes, uint cur)
        {
            this._optimumEndIndex = cur;
            uint posPrev = this._optimum[(int)cur].PosPrev;
            uint backPrev = this._optimum[(int)cur].BackPrev;
            do
            {
                if (this._optimum[(int)cur].Prev1IsChar)
                {
                    this._optimum[(int)posPrev].MakeAsChar();
                    this._optimum[(int)posPrev].PosPrev = posPrev - 1U;
                    if (this._optimum[(int)cur].Prev2)
                    {
                        this._optimum[(int)(posPrev - 1U)].Prev1IsChar = false;
                        this._optimum[(int)(posPrev - 1U)].PosPrev = this._optimum[(int)cur].PosPrev2;
                        this._optimum[(int)(posPrev - 1U)].BackPrev = this._optimum[(int)cur].BackPrev2;
                    }
                }
                uint num = posPrev;
                uint backPrev2 = backPrev;
                backPrev = this._optimum[(int)num].BackPrev;
                posPrev = this._optimum[(int)num].PosPrev;
                this._optimum[(int)num].BackPrev = backPrev2;
                this._optimum[(int)num].PosPrev = cur;
                cur = num;
            }
            while (cur != 0U);
            backRes = this._optimum[0].BackPrev;
            this._optimumCurrentIndex = this._optimum[0].PosPrev;
            return this._optimumCurrentIndex;
        }

        // Token: 0x060005F5 RID: 1525 RVA: 0x0003B2E0 File Offset: 0x000394E0
        private uint GetOptimum(uint position, out uint backRes)
        {
            if (this._optimumEndIndex != this._optimumCurrentIndex)
            {
                uint result = this._optimum[(int)this._optimumCurrentIndex].PosPrev - this._optimumCurrentIndex;
                backRes = this._optimum[(int)this._optimumCurrentIndex].BackPrev;
                this._optimumCurrentIndex = this._optimum[(int)this._optimumCurrentIndex].PosPrev;
                return result;
            }
            this._optimumCurrentIndex = (this._optimumEndIndex = 0U);
            uint longestMatchLength;
            uint num;
            if (!this._longestMatchWasFound)
            {
                this.ReadMatchDistances(out longestMatchLength, out num);
            }
            else
            {
                longestMatchLength = this._longestMatchLength;
                num = this._numDistancePairs;
                this._longestMatchWasFound = false;
            }
            uint num2 = this._matchFinder.GetNumAvailableBytes() + 1U;
            if (num2 < 2U)
            {
                backRes = uint.MaxValue;
                return 1U;
            }
            if (num2 > 273U)
            {
            }
            uint num3 = 0U;
            for (uint num4 = 0U; num4 < 4U; num4 += 1U)
            {
                this.reps[(int)num4] = this._repDistances[(int)num4];
                this.repLens[(int)num4] = this._matchFinder.GetMatchLen(-1, this.reps[(int)num4], 273U);
                if (this.repLens[(int)num4] > this.repLens[(int)num3])
                {
                    num3 = num4;
                }
            }
            if (this.repLens[(int)num3] >= this._numFastBytes)
            {
                backRes = num3;
                uint num5 = this.repLens[(int)num3];
                this.MovePos(num5 - 1U);
                return num5;
            }
            if (longestMatchLength >= this._numFastBytes)
            {
                backRes = this._matchDistances[(int)(num - 1U)] + 4U;
                this.MovePos(longestMatchLength - 1U);
                return longestMatchLength;
            }
            byte indexByte = this._matchFinder.GetIndexByte(-1);
            byte indexByte2 = this._matchFinder.GetIndexByte((int)(0U - this._repDistances[0] - 1U - 1U));
            if (longestMatchLength < 2U && indexByte != indexByte2 && this.repLens[(int)num3] < 2U)
            {
                backRes = uint.MaxValue;
                return 1U;
            }
            this._optimum[0].State = this._state;
            uint num6 = position & this._posStateMask;
            this._optimum[1].Price = this._isMatch[(int)((this._state.Index << 4) + num6)].GetPrice0() + this._literalEncoder.GetSubCoder(position, this._previousByte).GetPrice(!this._state.IsCharState(), indexByte2, indexByte);
            this._optimum[1].MakeAsChar();
            uint num7 = this._isMatch[(int)((this._state.Index << 4) + num6)].GetPrice1();
            uint num8 = num7 + this._isRep[(int)this._state.Index].GetPrice1();
            if (indexByte2 == indexByte)
            {
                uint num9 = num8 + this.GetRepLen1Price(this._state, num6);
                if (num9 < this._optimum[1].Price)
                {
                    this._optimum[1].Price = num9;
                    this._optimum[1].MakeAsShortRep();
                }
            }
            uint num10 = (longestMatchLength >= this.repLens[(int)num3]) ? longestMatchLength : this.repLens[(int)num3];
            if (num10 < 2U)
            {
                backRes = this._optimum[1].BackPrev;
                return 1U;
            }
            this._optimum[1].PosPrev = 0U;
            this._optimum[0].Backs0 = this.reps[0];
            this._optimum[0].Backs1 = this.reps[1];
            this._optimum[0].Backs2 = this.reps[2];
            this._optimum[0].Backs3 = this.reps[3];
            uint num11 = num10;
            do
            {
                this._optimum[(int)num11--].Price = 268435455U;
            }
            while (num11 >= 2U);
            for (uint num12 = 0U; num12 < 4U; num12 += 1U)
            {
                uint num13 = this.repLens[(int)num12];
                if (num13 >= 2U)
                {
                    uint num14 = num8 + this.GetPureRepPrice(num12, this._state, num6);
                    do
                    {
                        uint num15 = num14 + this._repMatchLenEncoder.GetPrice(num13 - 2U, num6);
                        Encoder.Optimal optimal = this._optimum[(int)num13];
                        if (num15 < optimal.Price)
                        {
                            optimal.Price = num15;
                            optimal.PosPrev = 0U;
                            optimal.BackPrev = num12;
                            optimal.Prev1IsChar = false;
                        }
                    }
                    while ((num13 -= 1U) >= 2U);
                }
            }
            uint num16 = num7 + this._isRep[(int)this._state.Index].GetPrice0();
            num11 = ((this.repLens[0] >= 2U) ? (this.repLens[0] + 1U) : 2U);
            if (num11 <= longestMatchLength)
            {
                uint num17 = 0U;
                while (num11 > this._matchDistances[(int)num17])
                {
                    num17 += 2U;
                }
                for (; ; )
                {
                    uint num18 = this._matchDistances[(int)(num17 + 1U)];
                    uint num19 = num16 + this.GetPosLenPrice(num18, num11, num6);
                    Encoder.Optimal optimal2 = this._optimum[(int)num11];
                    if (num19 < optimal2.Price)
                    {
                        optimal2.Price = num19;
                        optimal2.PosPrev = 0U;
                        optimal2.BackPrev = num18 + 4U;
                        optimal2.Prev1IsChar = false;
                    }
                    if (num11 == this._matchDistances[(int)num17])
                    {
                        num17 += 2U;
                        if (num17 == num)
                        {
                            break;
                        }
                    }
                    num11 += 1U;
                }
            }
            uint num20 = 0U;
            uint num21;
            for (; ; )
            {
                num20 += 1U;
                if (num20 == num10)
                {
                    break;
                }
                this.ReadMatchDistances(out num21, out num);
                if (num21 >= this._numFastBytes)
                {
                    goto IL_FD3;
                }
                position += 1U;
                uint num22 = this._optimum[(int)num20].PosPrev;
                Base.State state;
                if (this._optimum[(int)num20].Prev1IsChar)
                {
                    num22 -= 1U;
                    if (this._optimum[(int)num20].Prev2)
                    {
                        state = this._optimum[(int)this._optimum[(int)num20].PosPrev2].State;
                        if (this._optimum[(int)num20].BackPrev2 < 4U)
                        {
                            state.UpdateRep();
                        }
                        else
                        {
                            state.UpdateMatch();
                        }
                    }
                    else
                    {
                        state = this._optimum[(int)num22].State;
                    }
                    state.UpdateChar();
                }
                else
                {
                    state = this._optimum[(int)num22].State;
                }
                if (num22 == num20 - 1U)
                {
                    if (this._optimum[(int)num20].IsShortRep())
                    {
                        state.UpdateShortRep();
                    }
                    else
                    {
                        state.UpdateChar();
                    }
                }
                else
                {
                    uint num23;
                    if (this._optimum[(int)num20].Prev1IsChar && this._optimum[(int)num20].Prev2)
                    {
                        num22 = this._optimum[(int)num20].PosPrev2;
                        num23 = this._optimum[(int)num20].BackPrev2;
                        state.UpdateRep();
                    }
                    else
                    {
                        num23 = this._optimum[(int)num20].BackPrev;
                        if (num23 < 4U)
                        {
                            state.UpdateRep();
                        }
                        else
                        {
                            state.UpdateMatch();
                        }
                    }
                    Encoder.Optimal optimal3 = this._optimum[(int)num22];
                    switch (num23)
                    {
                        case 0U:
                            this.reps[0] = optimal3.Backs0;
                            this.reps[1] = optimal3.Backs1;
                            this.reps[2] = optimal3.Backs2;
                            this.reps[3] = optimal3.Backs3;
                            break;
                        case 1U:
                            this.reps[0] = optimal3.Backs1;
                            this.reps[1] = optimal3.Backs0;
                            this.reps[2] = optimal3.Backs2;
                            this.reps[3] = optimal3.Backs3;
                            break;
                        case 2U:
                            this.reps[0] = optimal3.Backs2;
                            this.reps[1] = optimal3.Backs0;
                            this.reps[2] = optimal3.Backs1;
                            this.reps[3] = optimal3.Backs3;
                            break;
                        case 3U:
                            this.reps[0] = optimal3.Backs3;
                            this.reps[1] = optimal3.Backs0;
                            this.reps[2] = optimal3.Backs1;
                            this.reps[3] = optimal3.Backs2;
                            break;
                        default:
                            this.reps[0] = num23 - 4U;
                            this.reps[1] = optimal3.Backs0;
                            this.reps[2] = optimal3.Backs1;
                            this.reps[3] = optimal3.Backs2;
                            break;
                    }
                }
                this._optimum[(int)num20].State = state;
                this._optimum[(int)num20].Backs0 = this.reps[0];
                this._optimum[(int)num20].Backs1 = this.reps[1];
                this._optimum[(int)num20].Backs2 = this.reps[2];
                this._optimum[(int)num20].Backs3 = this.reps[3];
                uint price = this._optimum[(int)num20].Price;
                indexByte = this._matchFinder.GetIndexByte(-1);
                indexByte2 = this._matchFinder.GetIndexByte((int)(0U - this.reps[0] - 1U - 1U));
                num6 = (position & this._posStateMask);
                uint num24 = price + this._isMatch[(int)((state.Index << 4) + num6)].GetPrice0() + this._literalEncoder.GetSubCoder(position, this._matchFinder.GetIndexByte(-2)).GetPrice(!state.IsCharState(), indexByte2, indexByte);
                Encoder.Optimal optimal4 = this._optimum[(int)(num20 + 1U)];
                bool flag = false;
                if (num24 < optimal4.Price)
                {
                    optimal4.Price = num24;
                    optimal4.PosPrev = num20;
                    optimal4.MakeAsChar();
                    flag = true;
                }
                num7 = price + this._isMatch[(int)((state.Index << 4) + num6)].GetPrice1();
                num8 = num7 + this._isRep[(int)state.Index].GetPrice1();
                if (indexByte2 == indexByte && (optimal4.PosPrev >= num20 || optimal4.BackPrev != 0U))
                {
                    uint num25 = num8 + this.GetRepLen1Price(state, num6);
                    if (num25 <= optimal4.Price)
                    {
                        optimal4.Price = num25;
                        optimal4.PosPrev = num20;
                        optimal4.MakeAsShortRep();
                        flag = true;
                    }
                }
                uint num26 = this._matchFinder.GetNumAvailableBytes() + 1U;
                num26 = Math.Min(4095U - num20, num26);
                num2 = num26;
                if (num2 >= 2U)
                {
                    if (num2 > this._numFastBytes)
                    {
                        num2 = this._numFastBytes;
                    }
                    if (!flag && indexByte2 != indexByte)
                    {
                        uint limit = Math.Min(num26 - 1U, this._numFastBytes);
                        uint matchLen = this._matchFinder.GetMatchLen(0, this.reps[0], limit);
                        if (matchLen >= 2U)
                        {
                            Base.State state2 = state;
                            state2.UpdateChar();
                            uint num27 = position + 1U & this._posStateMask;
                            uint num28 = num24 + this._isMatch[(int)((state2.Index << 4) + num27)].GetPrice1() + this._isRep[(int)state2.Index].GetPrice1();
                            uint num29 = num20 + 1U + matchLen;
                            while (num10 < num29)
                            {
                                this._optimum[(int)(num10 += 1U)].Price = 268435455U;
                            }
                            uint num30 = num28 + this.GetRepPrice(0U, matchLen, state2, num27);
                            Encoder.Optimal optimal5 = this._optimum[(int)num29];
                            if (num30 < optimal5.Price)
                            {
                                optimal5.Price = num30;
                                optimal5.PosPrev = num20 + 1U;
                                optimal5.BackPrev = 0U;
                                optimal5.Prev1IsChar = true;
                                optimal5.Prev2 = false;
                            }
                        }
                    }
                    uint num31 = 2U;
                    for (uint num32 = 0U; num32 < 4U; num32 += 1U)
                    {
                        uint num33 = this._matchFinder.GetMatchLen(-1, this.reps[(int)num32], num2);
                        if (num33 >= 2U)
                        {
                            uint num34 = num33;
                            for (; ; )
                            {
                                if (num10 < num20 + num33)
                                {
                                    this._optimum[(int)(num10 += 1U)].Price = 268435455U;
                                }
                                else
                                {
                                    uint num35 = num8 + this.GetRepPrice(num32, num33, state, num6);
                                    Encoder.Optimal optimal6 = this._optimum[(int)(num20 + num33)];
                                    if (num35 < optimal6.Price)
                                    {
                                        optimal6.Price = num35;
                                        optimal6.PosPrev = num20;
                                        optimal6.BackPrev = num32;
                                        optimal6.Prev1IsChar = false;
                                    }
                                    if ((num33 -= 1U) < 2U)
                                    {
                                        break;
                                    }
                                }
                            }
                            num33 = num34;
                            if (num32 == 0U)
                            {
                                num31 = num33 + 1U;
                            }
                            if (num33 < num26)
                            {
                                uint limit2 = Math.Min(num26 - 1U - num33, this._numFastBytes);
                                uint matchLen2 = this._matchFinder.GetMatchLen((int)num33, this.reps[(int)num32], limit2);
                                if (matchLen2 >= 2U)
                                {
                                    Base.State state3 = state;
                                    state3.UpdateRep();
                                    uint num36 = position + num33 & this._posStateMask;
                                    uint num37 = num8 + this.GetRepPrice(num32, num33, state, num6) + this._isMatch[(int)((state3.Index << 4) + num36)].GetPrice0() + this._literalEncoder.GetSubCoder(position + num33, this._matchFinder.GetIndexByte((int)(num33 - 1U - 1U))).GetPrice(true, this._matchFinder.GetIndexByte((int)(num33 - 1U - (this.reps[(int)num32] + 1U))), this._matchFinder.GetIndexByte((int)(num33 - 1U)));
                                    state3.UpdateChar();
                                    num36 = (position + num33 + 1U & this._posStateMask);
                                    uint num38 = num37 + this._isMatch[(int)((state3.Index << 4) + num36)].GetPrice1() + this._isRep[(int)state3.Index].GetPrice1();
                                    uint num39 = num33 + 1U + matchLen2;
                                    while (num10 < num20 + num39)
                                    {
                                        this._optimum[(int)(num10 += 1U)].Price = 268435455U;
                                    }
                                    uint num40 = num38 + this.GetRepPrice(0U, matchLen2, state3, num36);
                                    Encoder.Optimal optimal7 = this._optimum[(int)(num20 + num39)];
                                    if (num40 < optimal7.Price)
                                    {
                                        optimal7.Price = num40;
                                        optimal7.PosPrev = num20 + num33 + 1U;
                                        optimal7.BackPrev = 0U;
                                        optimal7.Prev1IsChar = true;
                                        optimal7.Prev2 = true;
                                        optimal7.PosPrev2 = num20;
                                        optimal7.BackPrev2 = num32;
                                    }
                                }
                            }
                        }
                    }
                    if (num21 > num2)
                    {
                        num21 = num2;
                        num = 0U;
                        while (num21 > this._matchDistances[(int)num])
                        {
                            num += 2U;
                        }
                        this._matchDistances[(int)num] = num21;
                        num += 2U;
                    }
                    if (num21 >= num31)
                    {
                        num16 = num7 + this._isRep[(int)state.Index].GetPrice0();
                        while (num10 < num20 + num21)
                        {
                            this._optimum[(int)(num10 += 1U)].Price = 268435455U;
                        }
                        uint num41 = 0U;
                        while (num31 > this._matchDistances[(int)num41])
                        {
                            num41 += 2U;
                        }
                        uint num42 = num31;
                        for (; ; )
                        {
                            uint num43 = this._matchDistances[(int)(num41 + 1U)];
                            uint num44 = num16 + this.GetPosLenPrice(num43, num42, num6);
                            Encoder.Optimal optimal8 = this._optimum[(int)(num20 + num42)];
                            if (num44 < optimal8.Price)
                            {
                                optimal8.Price = num44;
                                optimal8.PosPrev = num20;
                                optimal8.BackPrev = num43 + 4U;
                                optimal8.Prev1IsChar = false;
                            }
                            if (num42 == this._matchDistances[(int)num41])
                            {
                                if (num42 < num26)
                                {
                                    uint limit3 = Math.Min(num26 - 1U - num42, this._numFastBytes);
                                    uint matchLen3 = this._matchFinder.GetMatchLen((int)num42, num43, limit3);
                                    if (matchLen3 >= 2U)
                                    {
                                        Base.State state4 = state;
                                        state4.UpdateMatch();
                                        uint num45 = position + num42 & this._posStateMask;
                                        uint num46 = num44 + this._isMatch[(int)((state4.Index << 4) + num45)].GetPrice0() + this._literalEncoder.GetSubCoder(position + num42, this._matchFinder.GetIndexByte((int)(num42 - 1U - 1U))).GetPrice(true, this._matchFinder.GetIndexByte((int)(num42 - (num43 + 1U) - 1U)), this._matchFinder.GetIndexByte((int)(num42 - 1U)));
                                        state4.UpdateChar();
                                        num45 = (position + num42 + 1U & this._posStateMask);
                                        uint num47 = num46 + this._isMatch[(int)((state4.Index << 4) + num45)].GetPrice1() + this._isRep[(int)state4.Index].GetPrice1();
                                        uint num48 = num42 + 1U + matchLen3;
                                        while (num10 < num20 + num48)
                                        {
                                            this._optimum[(int)(num10 += 1U)].Price = 268435455U;
                                        }
                                        num44 = num47 + this.GetRepPrice(0U, matchLen3, state4, num45);
                                        optimal8 = this._optimum[(int)(num20 + num48)];
                                        if (num44 < optimal8.Price)
                                        {
                                            optimal8.Price = num44;
                                            optimal8.PosPrev = num20 + num42 + 1U;
                                            optimal8.BackPrev = 0U;
                                            optimal8.Prev1IsChar = true;
                                            optimal8.Prev2 = true;
                                            optimal8.PosPrev2 = num20;
                                            optimal8.BackPrev2 = num43 + 4U;
                                        }
                                    }
                                }
                                num41 += 2U;
                                if (num41 == num)
                                {
                                    break;
                                }
                            }
                            num42 += 1U;
                        }
                    }
                }
            }
            return this.Backward(out backRes, num20);
        IL_FD3:
            this._numDistancePairs = num;
            this._longestMatchLength = num21;
            this._longestMatchWasFound = true;
            return this.Backward(out backRes, num20);
        }

        // Token: 0x060005F6 RID: 1526 RVA: 0x000088B3 File Offset: 0x00006AB3
        private bool ChangePair(uint smallDist, uint bigDist)
        {
            return smallDist < 33554432U && bigDist >= smallDist << 7;
        }

        // Token: 0x060005F7 RID: 1527 RVA: 0x0003C2E0 File Offset: 0x0003A4E0
        private void WriteEndMarker(uint posState)
        {
            if (this._writeEndMark)
            {
                this._isMatch[(int)((this._state.Index << 4) + posState)].Encode(this._rangeEncoder, 1U);
                this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
                this._state.UpdateMatch();
                uint num = 2U;
                this._lenEncoder.Encode(this._rangeEncoder, num - 2U, posState);
                uint symbol = 63U;
                uint lenToPosState = Base.GetLenToPosState(num);
                this._posSlotEncoder[(int)lenToPosState].Encode(this._rangeEncoder, symbol);
                int num2 = 30;
                uint num3 = (1U << num2) - 1U;
                this._rangeEncoder.EncodeDirectBits(num3 >> 4, num2 - 4);
                this._posAlignEncoder.ReverseEncode(this._rangeEncoder, num3 & 15U);
            }
        }

        // Token: 0x060005F8 RID: 1528 RVA: 0x000088C8 File Offset: 0x00006AC8
        private void Flush(uint nowPos)
        {
            this.ReleaseMFStream();
            this.WriteEndMarker(nowPos & this._posStateMask);
            this._rangeEncoder.FlushData();
            this._rangeEncoder.FlushStream();
        }

        // Token: 0x060005F9 RID: 1529 RVA: 0x0003C3BC File Offset: 0x0003A5BC
        public void CodeOneBlock(out long inSize, out long outSize, out bool finished)
        {
            inSize = 0L;
            outSize = 0L;
            finished = true;
            if (this._inStream != null)
            {
                this._matchFinder.SetStream(this._inStream);
                this._matchFinder.Init();
                this._needReleaseMFStream = true;
                this._inStream = null;
                if (this._trainSize != 0U)
                {
                    this._matchFinder.Skip(this._trainSize);
                }
            }
            if (this._finished)
            {
                return;
            }
            this._finished = true;
            long num = this.nowPos64;
            if (this.nowPos64 == 0L)
            {
                if (this._matchFinder.GetNumAvailableBytes() == 0U)
                {
                    this.Flush((uint)this.nowPos64);
                    return;
                }
                uint num2;
                uint num3;
                this.ReadMatchDistances(out num2, out num3);
                uint num4 = (uint)((int)this.nowPos64 & (int)this._posStateMask);
                this._isMatch[(int)((this._state.Index << 4) + num4)].Encode(this._rangeEncoder, 0U);
                this._state.UpdateChar();
                byte indexByte = this._matchFinder.GetIndexByte((int)(0U - this._additionalOffset));
                this._literalEncoder.GetSubCoder((uint)this.nowPos64, this._previousByte).Encode(this._rangeEncoder, indexByte);
                this._previousByte = indexByte;
                this._additionalOffset -= 1U;
                this.nowPos64 += 1L;
            }
            if (this._matchFinder.GetNumAvailableBytes() == 0U)
            {
                this.Flush((uint)this.nowPos64);
                return;
            }
            for (; ; )
            {
                uint num5;
                uint optimum = this.GetOptimum((uint)this.nowPos64, out num5);
                uint num6 = (uint)((int)this.nowPos64 & (int)this._posStateMask);
                uint num7 = (this._state.Index << 4) + num6;
                if (optimum == 1U && num5 == 4294967295U)
                {
                    this._isMatch[(int)num7].Encode(this._rangeEncoder, 0U);
                    byte indexByte2 = this._matchFinder.GetIndexByte((int)(0U - this._additionalOffset));
                    Encoder.LiteralEncoder.Encoder2 subCoder = this._literalEncoder.GetSubCoder((uint)this.nowPos64, this._previousByte);
                    if (!this._state.IsCharState())
                    {
                        byte indexByte3 = this._matchFinder.GetIndexByte((int)(0U - this._repDistances[0] - 1U - this._additionalOffset));
                        subCoder.EncodeMatched(this._rangeEncoder, indexByte3, indexByte2);
                    }
                    else
                    {
                        subCoder.Encode(this._rangeEncoder, indexByte2);
                    }
                    this._previousByte = indexByte2;
                    this._state.UpdateChar();
                }
                else
                {
                    this._isMatch[(int)num7].Encode(this._rangeEncoder, 1U);
                    if (num5 < 4U)
                    {
                        this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
                        if (num5 == 0U)
                        {
                            this._isRepG0[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
                            if (optimum == 1U)
                            {
                                this._isRep0Long[(int)num7].Encode(this._rangeEncoder, 0U);
                            }
                            else
                            {
                                this._isRep0Long[(int)num7].Encode(this._rangeEncoder, 1U);
                            }
                        }
                        else
                        {
                            this._isRepG0[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
                            if (num5 == 1U)
                            {
                                this._isRepG1[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
                            }
                            else
                            {
                                this._isRepG1[(int)this._state.Index].Encode(this._rangeEncoder, 1U);
                                this._isRepG2[(int)this._state.Index].Encode(this._rangeEncoder, num5 - 2U);
                            }
                        }
                        if (optimum == 1U)
                        {
                            this._state.UpdateShortRep();
                        }
                        else
                        {
                            this._repMatchLenEncoder.Encode(this._rangeEncoder, optimum - 2U, num6);
                            this._state.UpdateRep();
                        }
                        uint num8 = this._repDistances[(int)num5];
                        if (num5 != 0U)
                        {
                            for (uint num9 = num5; num9 >= 1U; num9 -= 1U)
                            {
                                this._repDistances[(int)num9] = this._repDistances[(int)(num9 - 1U)];
                            }
                            this._repDistances[0] = num8;
                        }
                    }
                    else
                    {
                        this._isRep[(int)this._state.Index].Encode(this._rangeEncoder, 0U);
                        this._state.UpdateMatch();
                        this._lenEncoder.Encode(this._rangeEncoder, optimum - 2U, num6);
                        num5 -= 4U;
                        uint posSlot = Encoder.GetPosSlot(num5);
                        uint lenToPosState = Base.GetLenToPosState(optimum);
                        this._posSlotEncoder[(int)lenToPosState].Encode(this._rangeEncoder, posSlot);
                        if (posSlot >= 4U)
                        {
                            int num10 = (int)((posSlot >> 1) - 1U);
                            uint num11 = (2U | (posSlot & 1U)) << num10;
                            uint num12 = num5 - num11;
                            if (posSlot < 14U)
                            {
                                BitTreeEncoder.ReverseEncode(this._posEncoders, num11 - posSlot - 1U, this._rangeEncoder, num10, num12);
                            }
                            else
                            {
                                this._rangeEncoder.EncodeDirectBits(num12 >> 4, num10 - 4);
                                this._posAlignEncoder.ReverseEncode(this._rangeEncoder, num12 & 15U);
                                this._alignPriceCount += 1U;
                            }
                        }
                        uint num13 = num5;
                        for (uint num14 = 3U; num14 >= 1U; num14 -= 1U)
                        {
                            this._repDistances[(int)num14] = this._repDistances[(int)(num14 - 1U)];
                        }
                        this._repDistances[0] = num13;
                        this._matchPriceCount += 1U;
                    }
                    this._previousByte = this._matchFinder.GetIndexByte((int)(optimum - 1U - this._additionalOffset));
                }
                this._additionalOffset -= optimum;
                this.nowPos64 += (long)((ulong)optimum);
                if (this._additionalOffset == 0U)
                {
                    if (this._matchPriceCount >= 128U)
                    {
                        this.FillDistancesPrices();
                    }
                    if (this._alignPriceCount >= 16U)
                    {
                        this.FillAlignPrices();
                    }
                    inSize = this.nowPos64;
                    outSize = this._rangeEncoder.GetProcessedSizeAdd();
                    if (this._matchFinder.GetNumAvailableBytes() == 0U)
                    {
                        break;
                    }
                    if (this.nowPos64 - num >= 4096L)
                    {
                        goto Block_24;
                    }
                }
            }
            this.Flush((uint)this.nowPos64);
            return;
        Block_24:
            this._finished = false;
            finished = false;
        }

        // Token: 0x060005FA RID: 1530 RVA: 0x000088F4 File Offset: 0x00006AF4
        private void ReleaseMFStream()
        {
            if (this._matchFinder != null && this._needReleaseMFStream)
            {
                this._matchFinder.ReleaseStream();
                this._needReleaseMFStream = false;
            }
        }

        // Token: 0x060005FB RID: 1531 RVA: 0x00008918 File Offset: 0x00006B18
        private void SetOutStream(Stream outStream)
        {
            this._rangeEncoder.SetStream(outStream);
        }

        // Token: 0x060005FC RID: 1532 RVA: 0x00008926 File Offset: 0x00006B26
        private void ReleaseOutStream()
        {
            this._rangeEncoder.ReleaseStream();
        }

        // Token: 0x060005FD RID: 1533 RVA: 0x00008933 File Offset: 0x00006B33
        private void ReleaseStreams()
        {
            this.ReleaseMFStream();
            this.ReleaseOutStream();
        }

        // Token: 0x060005FE RID: 1534 RVA: 0x0003C9B4 File Offset: 0x0003ABB4
        private void SetStreams(Stream inStream, Stream outStream, long inSize, long outSize)
        {
            this._inStream = inStream;
            this._finished = false;
            this.Create();
            this.SetOutStream(outStream);
            this.Init();
            this.FillDistancesPrices();
            this.FillAlignPrices();
            this._lenEncoder.SetTableSize(this._numFastBytes + 1U - 2U);
            this._lenEncoder.UpdateTables(1U << this._posStateBits);
            this._repMatchLenEncoder.SetTableSize(this._numFastBytes + 1U - 2U);
            this._repMatchLenEncoder.UpdateTables(1U << this._posStateBits);
            this.nowPos64 = 0L;
        }

        // Token: 0x060005FF RID: 1535 RVA: 0x0003CA4C File Offset: 0x0003AC4C
        public void Code(Stream inStream, Stream outStream, long inSize, long outSize, ICodeProgress progress)
        {
            this._needReleaseMFStream = false;
            try
            {
                this.SetStreams(inStream, outStream, inSize, outSize);
                for (; ; )
                {
                    long inSize2;
                    long outSize2;
                    bool flag;
                    this.CodeOneBlock(out inSize2, out outSize2, out flag);
                    if (flag)
                    {
                        break;
                    }
                    if (progress != null)
                    {
                        progress.SetProgress(inSize2, outSize2);
                    }
                }
            }
            finally
            {
                this.ReleaseStreams();
            }
        }

        // Token: 0x06000600 RID: 1536 RVA: 0x0003CAA4 File Offset: 0x0003ACA4
        public void WriteCoderProperties(Stream outStream)
        {
            this.properties[0] = (byte)((this._posStateBits * 5 + this._numLiteralPosStateBits) * 9 + this._numLiteralContextBits);
            for (int i = 0; i < 4; i++)
            {
                this.properties[1 + i] = (byte)(this._dictionarySize >> 8 * i & 255U);
            }
            outStream.Write(this.properties, 0, 5);
        }

        // Token: 0x06000601 RID: 1537 RVA: 0x0003CB0C File Offset: 0x0003AD0C
        private void FillDistancesPrices()
        {
            for (uint num = 4U; num < 128U; num += 1U)
            {
                uint posSlot = Encoder.GetPosSlot(num);
                int num2 = (int)((posSlot >> 1) - 1U);
                uint num3 = (2U | (posSlot & 1U)) << num2;
                this.tempPrices[(int)num] = BitTreeEncoder.ReverseGetPrice(this._posEncoders, num3 - posSlot - 1U, num2, num - num3);
            }
            for (uint num4 = 0U; num4 < 4U; num4 += 1U)
            {
                BitTreeEncoder bitTreeEncoder = this._posSlotEncoder[(int)num4];
                uint num5 = num4 << 6;
                for (uint num6 = 0U; num6 < this._distTableSize; num6 += 1U)
                {
                    this._posSlotPrices[(int)(num5 + num6)] = bitTreeEncoder.GetPrice(num6);
                }
                for (uint num7 = 14U; num7 < this._distTableSize; num7 += 1U)
                {
                    this._posSlotPrices[(int)(num5 + num7)] += (num7 >> 1) - 1U - 4U << 6;
                }
                uint num8 = num4 * 128U;
                uint num9;
                for (num9 = 0U; num9 < 4U; num9 += 1U)
                {
                    this._distancesPrices[(int)(num8 + num9)] = this._posSlotPrices[(int)(num5 + num9)];
                }
                while (num9 < 128U)
                {
                    this._distancesPrices[(int)(num8 + num9)] = this._posSlotPrices[(int)(num5 + Encoder.GetPosSlot(num9))] + this.tempPrices[(int)num9];
                    num9 += 1U;
                }
            }
            this._matchPriceCount = 0U;
        }

        // Token: 0x06000602 RID: 1538 RVA: 0x0003CC58 File Offset: 0x0003AE58
        private void FillAlignPrices()
        {
            for (uint num = 0U; num < 16U; num += 1U)
            {
                this._alignPrices[(int)num] = this._posAlignEncoder.ReverseGetPrice(num);
            }
            this._alignPriceCount = 0U;
        }

        // Token: 0x06000603 RID: 1539 RVA: 0x0003CC90 File Offset: 0x0003AE90
        private static int FindMatchFinder(string s)
        {
            for (int i = 0; i < Encoder.kMatchFinderIDs.Length; i++)
            {
                if (s == Encoder.kMatchFinderIDs[i])
                {
                    return i;
                }
            }
            return -1;
        }

        // Token: 0x06000604 RID: 1540 RVA: 0x0003CCC4 File Offset: 0x0003AEC4
        public void SetCoderProperties(CoderPropID[] propIDs, object[] properties)
        {
            uint num = 0U;
            while ((ulong)num < (ulong)((long)properties.Length))
            {
                object obj = properties[(int)num];
                switch (propIDs[(int)num])
                {
                    case CoderPropID.DictionarySize:
                        {
                            if (!(obj is int))
                            {
                                throw new InvalidParamException();
                            }
                            int num2 = (int)obj;
                            if ((long)num2 < 1L || (long)num2 > 1073741824L)
                            {
                                throw new InvalidParamException();
                            }
                            this._dictionarySize = (uint)num2;
                            int num3 = 0;
                            while ((long)num3 < 30L && (long)num2 > (long)(1UL << (num3 & 31)))
                            {
                                num3++;
                            }
                            this._distTableSize = (uint)(num3 * 2);
                            break;
                        }
                    case CoderPropID.UsedMemorySize:
                    case CoderPropID.Order:
                    case CoderPropID.BlockSize:
                    case CoderPropID.MatchFinderCycles:
                    case CoderPropID.NumPasses:
                    case CoderPropID.NumThreads:
                        goto IL_21C;
                    case CoderPropID.PosStateBits:
                        {
                            if (!(obj is int))
                            {
                                throw new InvalidParamException();
                            }
                            int num4 = (int)obj;
                            if (num4 < 0 || (long)num4 > 4L)
                            {
                                throw new InvalidParamException();
                            }
                            this._posStateBits = num4;
                            this._posStateMask = (1U << this._posStateBits) - 1U;
                            break;
                        }
                    case CoderPropID.LitContextBits:
                        {
                            if (!(obj is int))
                            {
                                throw new InvalidParamException();
                            }
                            int num5 = (int)obj;
                            if (num5 < 0 || (long)num5 > 8L)
                            {
                                throw new InvalidParamException();
                            }
                            this._numLiteralContextBits = num5;
                            break;
                        }
                    case CoderPropID.LitPosBits:
                        {
                            if (!(obj is int))
                            {
                                throw new InvalidParamException();
                            }
                            int num6 = (int)obj;
                            if (num6 < 0 || (long)num6 > 4L)
                            {
                                throw new InvalidParamException();
                            }
                            this._numLiteralPosStateBits = num6;
                            break;
                        }
                    case CoderPropID.NumFastBytes:
                        {
                            if (!(obj is int))
                            {
                                throw new InvalidParamException();
                            }
                            int num7 = (int)obj;
                            if (num7 < 5 || (long)num7 > 273L)
                            {
                                throw new InvalidParamException();
                            }
                            this._numFastBytes = (uint)num7;
                            break;
                        }
                    case CoderPropID.MatchFinder:
                        {
                            if (!(obj is string))
                            {
                                throw new InvalidParamException();
                            }
                            Encoder.EMatchFinderType matchFinderType = this._matchFinderType;
                            int num8 = Encoder.FindMatchFinder(((string)obj).ToUpper());
                            if (num8 < 0)
                            {
                                throw new InvalidParamException();
                            }
                            this._matchFinderType = (Encoder.EMatchFinderType)num8;
                            if (this._matchFinder != null && matchFinderType != this._matchFinderType)
                            {
                                this._dictionarySizePrev = uint.MaxValue;
                                this._matchFinder = null;
                            }
                            break;
                        }
                    case CoderPropID.Algorithm:
                        break;
                    case CoderPropID.EndMarker:
                        if (!(obj is bool))
                        {
                            throw new InvalidParamException();
                        }
                        this.SetWriteEndMarkerMode((bool)obj);
                        break;
                    default:
                        goto IL_21C;
                }
                num += 1U;
                continue;
            IL_21C:
                throw new InvalidParamException();
            }
        }

        // Token: 0x06000605 RID: 1541 RVA: 0x00008941 File Offset: 0x00006B41
        public void SetTrainSize(uint trainSize)
        {
            this._trainSize = trainSize;
        }

        // Token: 0x04000757 RID: 1879
        private const uint kIfinityPrice = 268435455U;

        // Token: 0x04000758 RID: 1880
        private static byte[] g_FastPos = new byte[2048];

        // Token: 0x04000759 RID: 1881
        private Base.State _state;

        // Token: 0x0400075A RID: 1882
        private byte _previousByte;

        // Token: 0x0400075B RID: 1883
        private uint[] _repDistances = new uint[4];

        // Token: 0x0400075C RID: 1884
        private const int kDefaultDictionaryLogSize = 22;

        // Token: 0x0400075D RID: 1885
        private const uint kNumFastBytesDefault = 32U;

        // Token: 0x0400075E RID: 1886
        private const uint kNumLenSpecSymbols = 16U;

        // Token: 0x0400075F RID: 1887
        private const uint kNumOpts = 4096U;

        // Token: 0x04000760 RID: 1888
        private Encoder.Optimal[] _optimum = new Encoder.Optimal[4096];

        // Token: 0x04000761 RID: 1889
        private IMatchFinder _matchFinder;

        // Token: 0x04000762 RID: 1890
        private RangeCoder.Encoder _rangeEncoder = new RangeCoder.Encoder();

        // Token: 0x04000763 RID: 1891
        private BitEncoder[] _isMatch = new BitEncoder[192];

        // Token: 0x04000764 RID: 1892
        private BitEncoder[] _isRep = new BitEncoder[12];

        // Token: 0x04000765 RID: 1893
        private BitEncoder[] _isRepG0 = new BitEncoder[12];

        // Token: 0x04000766 RID: 1894
        private BitEncoder[] _isRepG1 = new BitEncoder[12];

        // Token: 0x04000767 RID: 1895
        private BitEncoder[] _isRepG2 = new BitEncoder[12];

        // Token: 0x04000768 RID: 1896
        private BitEncoder[] _isRep0Long = new BitEncoder[192];

        // Token: 0x04000769 RID: 1897
        private BitTreeEncoder[] _posSlotEncoder = new BitTreeEncoder[4];

        // Token: 0x0400076A RID: 1898
        private BitEncoder[] _posEncoders = new BitEncoder[114];

        // Token: 0x0400076B RID: 1899
        private BitTreeEncoder _posAlignEncoder = new BitTreeEncoder(4);

        // Token: 0x0400076C RID: 1900
        private Encoder.LenPriceTableEncoder _lenEncoder = new Encoder.LenPriceTableEncoder();

        // Token: 0x0400076D RID: 1901
        private Encoder.LenPriceTableEncoder _repMatchLenEncoder = new Encoder.LenPriceTableEncoder();

        // Token: 0x0400076E RID: 1902
        private Encoder.LiteralEncoder _literalEncoder = new Encoder.LiteralEncoder();

        // Token: 0x0400076F RID: 1903
        private uint[] _matchDistances = new uint[548];

        // Token: 0x04000770 RID: 1904
        private uint _numFastBytes = 32U;

        // Token: 0x04000771 RID: 1905
        private uint _longestMatchLength;

        // Token: 0x04000772 RID: 1906
        private uint _numDistancePairs;

        // Token: 0x04000773 RID: 1907
        private uint _additionalOffset;

        // Token: 0x04000774 RID: 1908
        private uint _optimumEndIndex;

        // Token: 0x04000775 RID: 1909
        private uint _optimumCurrentIndex;

        // Token: 0x04000776 RID: 1910
        private bool _longestMatchWasFound;

        // Token: 0x04000777 RID: 1911
        private uint[] _posSlotPrices = new uint[256];

        // Token: 0x04000778 RID: 1912
        private uint[] _distancesPrices = new uint[512];

        // Token: 0x04000779 RID: 1913
        private uint[] _alignPrices = new uint[16];

        // Token: 0x0400077A RID: 1914
        private uint _alignPriceCount;

        // Token: 0x0400077B RID: 1915
        private uint _distTableSize = 44U;

        // Token: 0x0400077C RID: 1916
        private int _posStateBits = 2;

        // Token: 0x0400077D RID: 1917
        private uint _posStateMask = 3U;

        // Token: 0x0400077E RID: 1918
        private int _numLiteralPosStateBits;

        // Token: 0x0400077F RID: 1919
        private int _numLiteralContextBits = 3;

        // Token: 0x04000780 RID: 1920
        private uint _dictionarySize = 4194304U;

        // Token: 0x04000781 RID: 1921
        private uint _dictionarySizePrev = uint.MaxValue;

        // Token: 0x04000782 RID: 1922
        private uint _numFastBytesPrev = uint.MaxValue;

        // Token: 0x04000783 RID: 1923
        private long nowPos64;

        // Token: 0x04000784 RID: 1924
        private bool _finished;

        // Token: 0x04000785 RID: 1925
        private Stream _inStream;

        // Token: 0x04000786 RID: 1926
        private Encoder.EMatchFinderType _matchFinderType = Encoder.EMatchFinderType.BT4;

        // Token: 0x04000787 RID: 1927
        private bool _writeEndMark;

        // Token: 0x04000788 RID: 1928
        private bool _needReleaseMFStream;

        // Token: 0x04000789 RID: 1929
        private uint[] reps = new uint[4];

        // Token: 0x0400078A RID: 1930
        private uint[] repLens = new uint[4];

        // Token: 0x0400078B RID: 1931
        private const int kPropSize = 5;

        // Token: 0x0400078C RID: 1932
        private byte[] properties = new byte[5];

        // Token: 0x0400078D RID: 1933
        private uint[] tempPrices = new uint[128];

        // Token: 0x0400078E RID: 1934
        private uint _matchPriceCount;

        // Token: 0x0400078F RID: 1935
        private static string[] kMatchFinderIDs = new string[]
        {
            "BT2",
            "BT4"
        };

        // Token: 0x04000790 RID: 1936
        private uint _trainSize;

        // Token: 0x020000E1 RID: 225
        private enum EMatchFinderType
        {
            // Token: 0x04000792 RID: 1938
            BT2,
            // Token: 0x04000793 RID: 1939
            BT4
        }

        // Token: 0x020000E2 RID: 226
        private class LiteralEncoder
        {
            // Token: 0x06000606 RID: 1542 RVA: 0x0003CF04 File Offset: 0x0003B104
            public void Create(int numPosBits, int numPrevBits)
            {
                if (this.m_Coders == null || this.m_NumPrevBits != numPrevBits || this.m_NumPosBits != numPosBits)
                {
                    this.m_NumPosBits = numPosBits;
                    this.m_PosMask = (1U << numPosBits) - 1U;
                    this.m_NumPrevBits = numPrevBits;
                    uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
                    this.m_Coders = new Encoder.LiteralEncoder.Encoder2[num];
                    for (uint num2 = 0U; num2 < num; num2 += 1U)
                    {
                        this.m_Coders[(int)num2].Create();
                    }
                }
            }

            // Token: 0x06000607 RID: 1543 RVA: 0x0003CF84 File Offset: 0x0003B184
            public void Init()
            {
                uint num = 1U << this.m_NumPrevBits + this.m_NumPosBits;
                for (uint num2 = 0U; num2 < num; num2 += 1U)
                {
                    this.m_Coders[(int)num2].Init();
                }
            }

            // Token: 0x06000608 RID: 1544 RVA: 0x0000894A File Offset: 0x00006B4A
            public Encoder.LiteralEncoder.Encoder2 GetSubCoder(uint pos, byte prevByte)
            {
                return this.m_Coders[(int)(((pos & this.m_PosMask) << this.m_NumPrevBits) + (uint)(prevByte >> 8 - this.m_NumPrevBits))];
            }

            // Token: 0x04000794 RID: 1940
            private Encoder.LiteralEncoder.Encoder2[] m_Coders;

            // Token: 0x04000795 RID: 1941
            private int m_NumPrevBits;

            // Token: 0x04000796 RID: 1942
            private int m_NumPosBits;

            // Token: 0x04000797 RID: 1943
            private uint m_PosMask;

            // Token: 0x020000E3 RID: 227
            public struct Encoder2
            {
                // Token: 0x0600060A RID: 1546 RVA: 0x00008977 File Offset: 0x00006B77
                public void Create()
                {
                    this.m_Encoders = new BitEncoder[768];
                }

                // Token: 0x0600060B RID: 1547 RVA: 0x0003CFC4 File Offset: 0x0003B1C4
                public void Init()
                {
                    for (int i = 0; i < 768; i++)
                    {
                        this.m_Encoders[i].Init();
                    }
                }

                // Token: 0x0600060C RID: 1548 RVA: 0x0003CFF4 File Offset: 0x0003B1F4
                public void Encode(RangeCoder.Encoder rangeEncoder, byte symbol)
                {
                    uint num = 1U;
                    for (int i = 7; i >= 0; i--)
                    {
                        uint num2 = (uint)(symbol >> i & 1);
                        this.m_Encoders[(int)num].Encode(rangeEncoder, num2);
                        num = (num << 1 | num2);
                    }
                }

                // Token: 0x0600060D RID: 1549 RVA: 0x0003D034 File Offset: 0x0003B234
                public void EncodeMatched(RangeCoder.Encoder rangeEncoder, byte matchByte, byte symbol)
                {
                    uint num = 1U;
                    bool flag = true;
                    for (int i = 7; i >= 0; i--)
                    {
                        uint num2 = (uint)(symbol >> i & 1);
                        uint num3 = num;
                        if (flag)
                        {
                            uint num4 = (uint)(matchByte >> i & 1);
                            num3 += 1U + num4 << 8;
                            flag = (num4 == num2);
                        }
                        this.m_Encoders[(int)num3].Encode(rangeEncoder, num2);
                        num = (num << 1 | num2);
                    }
                }

                // Token: 0x0600060E RID: 1550 RVA: 0x0003D098 File Offset: 0x0003B298
                public uint GetPrice(bool matchMode, byte matchByte, byte symbol)
                {
                    uint num = 0U;
                    uint num2 = 1U;
                    int i = 7;
                    if (matchMode)
                    {
                        while (i >= 0)
                        {
                            uint num3 = (uint)(matchByte >> i & 1);
                            uint num4 = (uint)(symbol >> i & 1);
                            num += this.m_Encoders[(int)((1U + num3 << 8) + num2)].GetPrice(num4);
                            num2 = (num2 << 1 | num4);
                            if (num3 != num4)
                            {
                                i--;
                                break;
                            }
                            i--;
                        }
                    }
                    while (i >= 0)
                    {
                        uint num5 = (uint)(symbol >> i & 1);
                        num += this.m_Encoders[(int)num2].GetPrice(num5);
                        num2 = (num2 << 1 | num5);
                        i--;
                    }
                    return num;
                }

                // Token: 0x04000798 RID: 1944
                private BitEncoder[] m_Encoders;
            }
        }

        // Token: 0x020000E4 RID: 228
        private class LenEncoder
        {
            // Token: 0x0600060F RID: 1551 RVA: 0x0003D12C File Offset: 0x0003B32C
            public LenEncoder()
            {
                for (uint num = 0U; num < 16U; num += 1U)
                {
                    this._lowCoder[(int)num] = new BitTreeEncoder(3);
                    this._midCoder[(int)num] = new BitTreeEncoder(3);
                }
            }

            // Token: 0x06000610 RID: 1552 RVA: 0x0003D198 File Offset: 0x0003B398
            public void Init(uint numPosStates)
            {
                this._choice.Init();
                this._choice2.Init();
                for (uint num = 0U; num < numPosStates; num += 1U)
                {
                    this._lowCoder[(int)num].Init();
                    this._midCoder[(int)num].Init();
                }
                this._highCoder.Init();
            }

            // Token: 0x06000611 RID: 1553 RVA: 0x0003D1F4 File Offset: 0x0003B3F4
            public void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                if (symbol < 8U)
                {
                    this._choice.Encode(rangeEncoder, 0U);
                    this._lowCoder[(int)posState].Encode(rangeEncoder, symbol);
                    return;
                }
                symbol -= 8U;
                this._choice.Encode(rangeEncoder, 1U);
                if (symbol < 8U)
                {
                    this._choice2.Encode(rangeEncoder, 0U);
                    this._midCoder[(int)posState].Encode(rangeEncoder, symbol);
                    return;
                }
                this._choice2.Encode(rangeEncoder, 1U);
                this._highCoder.Encode(rangeEncoder, symbol - 8U);
            }

            // Token: 0x06000612 RID: 1554 RVA: 0x0003D27C File Offset: 0x0003B47C
            public void SetPrices(uint posState, uint numSymbols, uint[] prices, uint st)
            {
                uint price = this._choice.GetPrice0();
                uint price2 = this._choice.GetPrice1();
                uint num = price2 + this._choice2.GetPrice0();
                uint num2 = price2 + this._choice2.GetPrice1();
                uint num3;
                for (num3 = 0U; num3 < 8U; num3 += 1U)
                {
                    if (num3 >= numSymbols)
                    {
                        return;
                    }
                    prices[(int)(st + num3)] = price + this._lowCoder[(int)posState].GetPrice(num3);
                }
                while (num3 < 16U)
                {
                    if (num3 >= numSymbols)
                    {
                        return;
                    }
                    prices[(int)(st + num3)] = num + this._midCoder[(int)posState].GetPrice(num3 - 8U);
                    num3 += 1U;
                }
                while (num3 < numSymbols)
                {
                    prices[(int)(st + num3)] = num2 + this._highCoder.GetPrice(num3 - 8U - 8U);
                    num3 += 1U;
                }
            }

            // Token: 0x04000799 RID: 1945
            private BitEncoder _choice;

            // Token: 0x0400079A RID: 1946
            private BitEncoder _choice2;

            // Token: 0x0400079B RID: 1947
            private BitTreeEncoder[] _lowCoder = new BitTreeEncoder[16];

            // Token: 0x0400079C RID: 1948
            private BitTreeEncoder[] _midCoder = new BitTreeEncoder[16];

            // Token: 0x0400079D RID: 1949
            private BitTreeEncoder _highCoder = new BitTreeEncoder(8);
        }

        // Token: 0x020000E5 RID: 229
        private class LenPriceTableEncoder : Encoder.LenEncoder
        {
            // Token: 0x06000613 RID: 1555 RVA: 0x00008989 File Offset: 0x00006B89
            public void SetTableSize(uint tableSize)
            {
                this._tableSize = tableSize;
            }

            // Token: 0x06000614 RID: 1556 RVA: 0x00008992 File Offset: 0x00006B92
            public uint GetPrice(uint symbol, uint posState)
            {
                return this._prices[(int)(posState * 272U + symbol)];
            }

            // Token: 0x06000615 RID: 1557 RVA: 0x000089A4 File Offset: 0x00006BA4
            private void UpdateTable(uint posState)
            {
                base.SetPrices(posState, this._tableSize, this._prices, posState * 272U);
                this._counters[(int)posState] = this._tableSize;
            }

            // Token: 0x06000616 RID: 1558 RVA: 0x0003D338 File Offset: 0x0003B538
            public void UpdateTables(uint numPosStates)
            {
                for (uint num = 0U; num < numPosStates; num += 1U)
                {
                    this.UpdateTable(num);
                }
            }

            // Token: 0x06000617 RID: 1559 RVA: 0x0003D358 File Offset: 0x0003B558
            public new void Encode(RangeCoder.Encoder rangeEncoder, uint symbol, uint posState)
            {
                base.Encode(rangeEncoder, symbol, posState);
                uint[] counters = this._counters;
                uint num = counters[(int)posState] - 1U;
                counters[(int)posState] = num;
                if (num == 0U)
                {
                    this.UpdateTable(posState);
                }
            }

            // Token: 0x0400079E RID: 1950
            private uint[] _prices = new uint[4352];

            // Token: 0x0400079F RID: 1951
            private uint _tableSize;

            // Token: 0x040007A0 RID: 1952
            private uint[] _counters = new uint[16];
        }

        // Token: 0x020000E6 RID: 230
        private class Optimal
        {
            // Token: 0x06000619 RID: 1561 RVA: 0x000089F3 File Offset: 0x00006BF3
            public void MakeAsChar()
            {
                this.BackPrev = uint.MaxValue;
                this.Prev1IsChar = false;
            }

            // Token: 0x0600061A RID: 1562 RVA: 0x00008A03 File Offset: 0x00006C03
            public void MakeAsShortRep()
            {
                this.BackPrev = 0U;
                this.Prev1IsChar = false;
            }

            // Token: 0x0600061B RID: 1563 RVA: 0x00008A13 File Offset: 0x00006C13
            public bool IsShortRep()
            {
                return this.BackPrev == 0U;
            }

            // Token: 0x040007A1 RID: 1953
            public Base.State State;

            // Token: 0x040007A2 RID: 1954
            public bool Prev1IsChar;

            // Token: 0x040007A3 RID: 1955
            public bool Prev2;

            // Token: 0x040007A4 RID: 1956
            public uint PosPrev2;

            // Token: 0x040007A5 RID: 1957
            public uint BackPrev2;

            // Token: 0x040007A6 RID: 1958
            public uint Price;

            // Token: 0x040007A7 RID: 1959
            public uint PosPrev;

            // Token: 0x040007A8 RID: 1960
            public uint BackPrev;

            // Token: 0x040007A9 RID: 1961
            public uint Backs0;

            // Token: 0x040007AA RID: 1962
            public uint Backs1;

            // Token: 0x040007AB RID: 1963
            public uint Backs2;

            // Token: 0x040007AC RID: 1964
            public uint Backs3;
        }
    }
}
