using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Compress.Core
{
    public class BitReader : IBitReader
    {
        private List<byte> bytes = new List<byte>();

        private int offset = 0;

        public void PutBytes(byte[] data)
        {
            this.bytes.AddRange(data);
        }

        public bool TryRead(int bitCount, out ulong code)
        {
            int i = 0;
            int cut = 0;
            code = 0;

            if (bitCount > 8 * this.bytes.Count)
                return false;

            if (this.offset != 0)
            {
                if (bitCount - 8 + this.offset >= 0)
                {
                    cut = 8 - this.offset;// количество требуемых битов
                    ulong mask = (ulong)(0xFF >> this.offset);
                    code = this.bytes[0] & mask;//получаем нужные биты
                    code <<= (bitCount - cut);//сдвигаем влево на кол-во оставшихся значимых битов
                    this.bytes.RemoveAt(0);//удаляем данный байт из массива
                }
                else
                {
                    cut = bitCount;// количество требуемых битов
                    ulong mask = (ulong)((0xFF >> (8 - bitCount)) << (8 - bitCount - this.offset));
                    code = (this.bytes[0] & mask) >> (8 - bitCount - this.offset);//получаем нужные биты                  
                }
            }

            if ((bitCount - cut) >= 8)
            {
                while ((bitCount - cut - i) >= 8)
                {
                    if ((bitCount - cut - i) > 8 * this.bytes.Count)
                        return false;
                    int currOffset;
                    if (this.offset != 0)
                    {
                        currOffset = bitCount - 8 - i - cut;
                    }
                    else
                    {
                        currOffset = bitCount - 8 - i;
                    }
                    ulong addByte = this.bytes[0];
                    addByte <<= currOffset;
                    code |= addByte;
                    bytes.RemoveAt(0);
                    i += 8;
                }
            }
            if (bitCount == cut)
            {
                this.offset += bitCount;
            }
            else
            {
                if ((bitCount - cut - i) > 8 * this.bytes.Count)
                    return false;

                this.offset = bitCount - cut - i;
                if (this.offset != 0)//записываем остатки кода
                {
                    ulong lastBits = (ulong)(this.bytes[0] >> (8 - this.offset));
                    code |= lastBits;
                }
            }

            return true;
        }
    }
}
