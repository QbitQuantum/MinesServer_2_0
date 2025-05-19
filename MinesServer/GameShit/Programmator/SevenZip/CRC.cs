using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace MinesServer.GameShit.Programmator.SevenZip
{
    internal class CRC
    {
        // Token: 0x0600059C RID: 1436 RVA: 0x00039D28 File Offset: 0x00037F28
        static CRC()
        {
            for (uint num = 0U; num < 256U; num += 1U)
            {
                uint num2 = num;
                for (int i = 0; i < 8; i++)
                {
                    num2 = (((num2 & 1U) == 0U) ? (num2 >> 1) : (num2 >> 1 ^ 3988292384U));
                }
                CRC.Table[(int)num] = num2;
            }
        }

        // Token: 0x0600059D RID: 1437 RVA: 0x00008438 File Offset: 0x00006638
        public void Init()
        {
            this._value = uint.MaxValue;
        }

        // Token: 0x0600059E RID: 1438 RVA: 0x00008441 File Offset: 0x00006641
        public void UpdateByte(byte b)
        {
            this._value = (CRC.Table[(int)((byte)this._value ^ b)] ^ this._value >> 8);
        }

        // Token: 0x0600059F RID: 1439 RVA: 0x00039D80 File Offset: 0x00037F80
        public void Update(byte[] data, uint offset, uint size)
        {
            for (uint num = 0U; num < size; num += 1U)
            {
                this._value = (CRC.Table[(int)((byte)this._value ^ data[(int)(offset + num)])] ^ this._value >> 8);
            }
        }

        // Token: 0x060005A0 RID: 1440 RVA: 0x00008461 File Offset: 0x00006661
        public uint GetDigest()
        {
            return this._value ^ uint.MaxValue;
        }

        // Token: 0x060005A1 RID: 1441 RVA: 0x0000846B File Offset: 0x0000666B
        private static uint CalculateDigest(byte[] data, uint offset, uint size)
        {
            CRC crc = new CRC();
            crc.Update(data, offset, size);
            return crc.GetDigest();
        }

        // Token: 0x060005A2 RID: 1442 RVA: 0x00008480 File Offset: 0x00006680
        private static bool VerifyDigest(uint digest, byte[] data, uint offset, uint size)
        {
            return CRC.CalculateDigest(data, offset, size) == digest;
        }

        // Token: 0x040006E6 RID: 1766
        public static readonly uint[] Table = new uint[256];

        // Token: 0x040006E7 RID: 1767
        private uint _value = uint.MaxValue;
    }
}
