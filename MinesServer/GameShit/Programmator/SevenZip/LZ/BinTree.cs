using MinesServer.GameShit.GUI;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip.LZ
{
    public class BinTree : InWindow, IMatchFinder, IInWindowStream
    {
        // Token: 0x06000620 RID: 1568 RVA: 0x0003D544 File Offset: 0x0003B744
        public void SetType(int numHashBytes)
        {
            this.HASH_ARRAY = (numHashBytes > 2);
            if (this.HASH_ARRAY)
            {
                this.kNumHashDirectBytes = 0U;
                this.kMinMatchCheck = 4U;
                this.kFixHashSize = 66560U;
                return;
            }
            this.kNumHashDirectBytes = 2U;
            this.kMinMatchCheck = 3U;
            this.kFixHashSize = 0U;
        }

        // Token: 0x06000621 RID: 1569 RVA: 0x00008A1E File Offset: 0x00006C1E
        public new void SetStream(Stream stream)
        {
            base.SetStream(stream);
        }

        // Token: 0x06000622 RID: 1570 RVA: 0x00008A27 File Offset: 0x00006C27
        public new void ReleaseStream()
        {
            base.ReleaseStream();
        }

        // Token: 0x06000623 RID: 1571 RVA: 0x0003D594 File Offset: 0x0003B794
        public new void Init()
        {
            base.Init();
            for (uint num = 0U; num < this._hashSizeSum; num += 1U)
            {
                this._hash[(int)num] = 0U;
            }
            this._cyclicBufferPos = 0U;
            base.ReduceOffsets(-1);
        }

        // Token: 0x06000624 RID: 1572 RVA: 0x0003D5D0 File Offset: 0x0003B7D0
        public new void MovePos()
        {
            uint num = this._cyclicBufferPos + 1U;
            this._cyclicBufferPos = num;
            if (num >= this._cyclicBufferSize)
            {
                this._cyclicBufferPos = 0U;
            }
            base.MovePos();
            if (this._pos == 2147483647U)
            {
                this.Normalize();
            }
        }

        // Token: 0x06000625 RID: 1573 RVA: 0x00008A2F File Offset: 0x00006C2F
        public new byte GetIndexByte(int index)
        {
            return base.GetIndexByte(index);
        }

        // Token: 0x06000626 RID: 1574 RVA: 0x00008A38 File Offset: 0x00006C38
        public new uint GetMatchLen(int index, uint distance, uint limit)
        {
            return base.GetMatchLen(index, distance, limit);
        }

        // Token: 0x06000627 RID: 1575 RVA: 0x00008A43 File Offset: 0x00006C43
        public new uint GetNumAvailableBytes()
        {
            return base.GetNumAvailableBytes();
        }

        // Token: 0x06000628 RID: 1576 RVA: 0x0003D618 File Offset: 0x0003B818
        public void Create(uint historySize, uint keepAddBufferBefore, uint matchMaxLen, uint keepAddBufferAfter)
        {
            if (historySize > 2147483391U)
            {
                throw new Exception();
            }
            this._cutValue = 16U + (matchMaxLen >> 1);
            uint keepSizeReserv = (historySize + keepAddBufferBefore + matchMaxLen + keepAddBufferAfter) / 2U + 256U;
            base.Create(historySize + keepAddBufferBefore, matchMaxLen + keepAddBufferAfter, keepSizeReserv);
            this._matchMaxLen = matchMaxLen;
            uint num = historySize + 1U;
            if (this._cyclicBufferSize != num)
            {
                this._son = new uint[(this._cyclicBufferSize = num) * 2U];
            }
            uint num2 = 65536U;
            if (this.HASH_ARRAY)
            {
                num2 = historySize - 1U;
                num2 |= num2 >> 1;
                num2 |= num2 >> 2;
                num2 |= num2 >> 4;
                num2 |= num2 >> 8;
                num2 >>= 1;
                num2 |= 65535U;
                if (num2 > 16777216U)
                {
                    num2 >>= 1;
                }
                this._hashMask = num2;
                num2 += 1U;
                num2 += this.kFixHashSize;
            }
            if (num2 != this._hashSizeSum)
            {
                this._hash = new uint[this._hashSizeSum = num2];
            }
        }

        // Token: 0x06000629 RID: 1577 RVA: 0x0003D700 File Offset: 0x0003B900
        public UInt32 GetMatches(UInt32[] distances)
        {
            UInt32 lenLimit;
            if (_pos + _matchMaxLen <= _streamPos)
                lenLimit = _matchMaxLen;
            else
            {
                lenLimit = _streamPos - _pos;
                if (lenLimit < kMinMatchCheck)
                {
                    MovePos();
                    return 0;
                }
            }

            UInt32 offset = 0;
            UInt32 matchMinPos = (_pos > _cyclicBufferSize) ? (_pos - _cyclicBufferSize) : 0;
            UInt32 cur = _bufferOffset + _pos;
            UInt32 maxLen = kStartMaxLen; // to avoid items for len < hashSize;
            UInt32 hashValue, hash2Value = 0, hash3Value = 0;

            if (HASH_ARRAY)
            {
                UInt32 temp = CRC.Table[_bufferBase[cur]] ^ _bufferBase[cur + 1];
                hash2Value = temp & (kHash2Size - 1);
                temp ^= ((UInt32)(_bufferBase[cur + 2]) << 8);
                hash3Value = temp & (kHash3Size - 1);
                hashValue = (temp ^ (CRC.Table[_bufferBase[cur + 3]] << 5)) & _hashMask;
            }
            else
                hashValue = _bufferBase[cur] ^ ((UInt32)(_bufferBase[cur + 1]) << 8);

            UInt32 curMatch = _hash[kFixHashSize + hashValue];
            if (HASH_ARRAY)
            {
                UInt32 curMatch2 = _hash[hash2Value];
                UInt32 curMatch3 = _hash[kHash3Offset + hash3Value];
                _hash[hash2Value] = _pos;
                _hash[kHash3Offset + hash3Value] = _pos;
                if (curMatch2 > matchMinPos)
                    if (_bufferBase[_bufferOffset + curMatch2] == _bufferBase[cur])
                    {
                        distances[offset++] = maxLen = 2;
                        distances[offset++] = _pos - curMatch2 - 1;
                    }
                if (curMatch3 > matchMinPos)
                    if (_bufferBase[_bufferOffset + curMatch3] == _bufferBase[cur])
                    {
                        if (curMatch3 == curMatch2)
                            offset -= 2;
                        distances[offset++] = maxLen = 3;
                        distances[offset++] = _pos - curMatch3 - 1;
                        curMatch2 = curMatch3;
                    }
                if (offset != 0 && curMatch2 == curMatch)
                {
                    offset -= 2;
                    maxLen = kStartMaxLen;
                }
            }

            _hash[kFixHashSize + hashValue] = _pos;

            UInt32 ptr0 = (_cyclicBufferPos << 1) + 1;
            UInt32 ptr1 = (_cyclicBufferPos << 1);

            UInt32 len0, len1;
            len0 = len1 = kNumHashDirectBytes;

            if (kNumHashDirectBytes != 0)
            {
                if (curMatch > matchMinPos)
                {
                    if (_bufferBase[_bufferOffset + curMatch + kNumHashDirectBytes] !=
                            _bufferBase[cur + kNumHashDirectBytes])
                    {
                        distances[offset++] = maxLen = kNumHashDirectBytes;
                        distances[offset++] = _pos - curMatch - 1;
                    }
                }
            }

            UInt32 count = _cutValue;

            while (true)
            {
                if (curMatch <= matchMinPos || count-- == 0)
                {
                    _son[ptr0] = _son[ptr1] = kEmptyHashValue;
                    break;
                }
                UInt32 delta = _pos - curMatch;
                UInt32 cyclicPos = ((delta <= _cyclicBufferPos) ?
                            (_cyclicBufferPos - delta) :
                            (_cyclicBufferPos - delta + _cyclicBufferSize)) << 1;

                UInt32 pby1 = _bufferOffset + curMatch;
                UInt32 len = Math.Min(len0, len1);
                if (_bufferBase[pby1 + len] == _bufferBase[cur + len])
                {
                    while (++len != lenLimit)
                        if (_bufferBase[pby1 + len] != _bufferBase[cur + len])
                            break;
                    if (maxLen < len)
                    {
                        distances[offset++] = maxLen = len;
                        distances[offset++] = delta - 1;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                }
                if (_bufferBase[pby1 + len] < _bufferBase[cur + len])
                {
                    _son[ptr1] = curMatch;
                    ptr1 = cyclicPos + 1;
                    curMatch = _son[ptr1];
                    len1 = len;
                }
                else
                {
                    _son[ptr0] = curMatch;
                    ptr0 = cyclicPos;
                    curMatch = _son[ptr0];
                    len0 = len;
                }
            }
            MovePos();
            return offset;
        }

        // Token: 0x0600062A RID: 1578 RVA: 0x0003DAF4 File Offset: 0x0003BCF4
        public void Skip(UInt32 num)
        {
            do
            {
                UInt32 lenLimit;
                if (_pos + _matchMaxLen <= _streamPos)
                    lenLimit = _matchMaxLen;
                else
                {
                    lenLimit = _streamPos - _pos;
                    if (lenLimit < kMinMatchCheck)
                    {
                        MovePos();
                        continue;
                    }
                }

                UInt32 matchMinPos = (_pos > _cyclicBufferSize) ? (_pos - _cyclicBufferSize) : 0;
                UInt32 cur = _bufferOffset + _pos;

                UInt32 hashValue;

                if (HASH_ARRAY)
                {
                    UInt32 temp = CRC.Table[_bufferBase[cur]] ^ _bufferBase[cur + 1];
                    UInt32 hash2Value = temp & (kHash2Size - 1);
                    _hash[hash2Value] = _pos;
                    temp ^= ((UInt32)(_bufferBase[cur + 2]) << 8);
                    UInt32 hash3Value = temp & (kHash3Size - 1);
                    _hash[kHash3Offset + hash3Value] = _pos;
                    hashValue = (temp ^ (CRC.Table[_bufferBase[cur + 3]] << 5)) & _hashMask;
                }
                else
                    hashValue = _bufferBase[cur] ^ ((UInt32)(_bufferBase[cur + 1]) << 8);

                UInt32 curMatch = _hash[kFixHashSize + hashValue];
                _hash[kFixHashSize + hashValue] = _pos;

                UInt32 ptr0 = (_cyclicBufferPos << 1) + 1;
                UInt32 ptr1 = (_cyclicBufferPos << 1);

                UInt32 len0, len1;
                len0 = len1 = kNumHashDirectBytes;

                UInt32 count = _cutValue;
                while (true)
                {
                    if (curMatch <= matchMinPos || count-- == 0)
                    {
                        _son[ptr0] = _son[ptr1] = kEmptyHashValue;
                        break;
                    }

                    UInt32 delta = _pos - curMatch;
                    UInt32 cyclicPos = ((delta <= _cyclicBufferPos) ?
                                (_cyclicBufferPos - delta) :
                                (_cyclicBufferPos - delta + _cyclicBufferSize)) << 1;

                    UInt32 pby1 = _bufferOffset + curMatch;
                    UInt32 len = Math.Min(len0, len1);
                    if (_bufferBase[pby1 + len] == _bufferBase[cur + len])
                    {
                        while (++len != lenLimit)
                            if (_bufferBase[pby1 + len] != _bufferBase[cur + len])
                                break;
                        if (len == lenLimit)
                        {
                            _son[ptr1] = _son[cyclicPos];
                            _son[ptr0] = _son[cyclicPos + 1];
                            break;
                        }
                    }
                    if (_bufferBase[pby1 + len] < _bufferBase[cur + len])
                    {
                        _son[ptr1] = curMatch;
                        ptr1 = cyclicPos + 1;
                        curMatch = _son[ptr1];
                        len1 = len;
                    }
                    else
                    {
                        _son[ptr0] = curMatch;
                        ptr0 = cyclicPos;
                        curMatch = _son[ptr0];
                        len0 = len;
                    }
                }
                MovePos();
            }
            while (--num != 0);
        }

        // Token: 0x0600062B RID: 1579 RVA: 0x0003DDA4 File Offset: 0x0003BFA4
        private void NormalizeLinks(uint[] items, uint numItems, uint subValue)
        {
            for (uint num = 0U; num < numItems; num += 1U)
            {
                uint num2 = items[(int)num];
                uint num3 = items[(int)num] = ((num2 > subValue) ? (num2 - subValue) : 0U);
            }
        }

        // Token: 0x0600062C RID: 1580 RVA: 0x0003DDD4 File Offset: 0x0003BFD4
        private void Normalize()
        {
            uint subValue = this._pos - this._cyclicBufferSize;
            this.NormalizeLinks(this._son, this._cyclicBufferSize * 2U, subValue);
            this.NormalizeLinks(this._hash, this._hashSizeSum, subValue);
            base.ReduceOffsets((int)subValue);
        }

        // Token: 0x0600062D RID: 1581 RVA: 0x00008A4B File Offset: 0x00006C4B
        public void SetCutValue(uint cutValue)
        {
            this._cutValue = cutValue;
        }

        // Token: 0x040007B1 RID: 1969
        private uint _cyclicBufferPos;

        // Token: 0x040007B2 RID: 1970
        private uint _cyclicBufferSize;

        // Token: 0x040007B3 RID: 1971
        private uint _matchMaxLen;

        // Token: 0x040007B4 RID: 1972
        private uint[] _son;

        // Token: 0x040007B5 RID: 1973
        private uint[] _hash;

        // Token: 0x040007B6 RID: 1974
        private uint _cutValue = 255U;

        // Token: 0x040007B7 RID: 1975
        private uint _hashMask;

        // Token: 0x040007B8 RID: 1976
        private uint _hashSizeSum;

        // Token: 0x040007B9 RID: 1977
        private bool HASH_ARRAY = true;

        // Token: 0x040007BA RID: 1978
        private const uint kHash2Size = 1024U;

        // Token: 0x040007BB RID: 1979
        private const uint kHash3Size = 65536U;

        // Token: 0x040007BC RID: 1980
        private const uint kBT2HashSize = 65536U;

        // Token: 0x040007BD RID: 1981
        private const uint kStartMaxLen = 1U;

        // Token: 0x040007BE RID: 1982
        private const uint kHash3Offset = 1024U;

        // Token: 0x040007BF RID: 1983
        private const uint kEmptyHashValue = 0U;

        // Token: 0x040007C0 RID: 1984
        private const uint kMaxValForNormalize = 2147483647U;

        // Token: 0x040007C1 RID: 1985
        private uint kNumHashDirectBytes;

        // Token: 0x040007C2 RID: 1986
        private uint kMinMatchCheck = 4U;

        // Token: 0x040007C3 RID: 1987
        private uint kFixHashSize = 66560U;
    }
}
