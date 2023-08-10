using System;
using System.Collections.Generic;
using System.Text;

namespace Compress.Core
{
    public class BitWriter : IBitWriter
    {
        private List<byte> bytes = new List<byte>();

        private int offset = 0;//смещение от крайнего левого бита

        public void Writer(ulong code, int bitCount)
        {
            int i = 0;
            int cut = 0;

            if (this.offset != 0)//если байт не пустой
            {
                if (bitCount - 8 + this.offset > 0)
                {
                    this.bytes[this.bytes.Count - 1] = (byte)(this.bytes[this.bytes.Count - 1] | (byte)(code >> (bitCount - 8 + this.offset)));//вставляем недостающие биты
                    cut = 8 - this.offset;//вырезаем эти биты из кода
                }
                else
                {
                    this.bytes[this.bytes.Count - 1] = (byte)(this.bytes[this.bytes.Count - 1] | (byte)(code << (8 - bitCount - this.offset)));//вставляем недостающие биты
                    cut = bitCount;//вырезаем эти биты из кода
                }
            }

            if ((bitCount - cut)>=8)
            {
                while ((bitCount - cut - i) > 8)
                {
                    this.bytes.Add((byte)(code >> (bitCount - cut - 8 - i) & 0xFF));
                    i += 8;
                }
            }
            if (bitCount == cut)
            {
                this.offset += bitCount;
            }
            else
            {
                this.offset = bitCount - cut - i;
                if (this.offset != 0)//записываем остатки кода
                {
                    ulong mask = (ulong)(0xFF >> (8 - this.offset));
                    this.bytes.Add((byte)((code & mask) << 8 - this.offset));
                }
            }                                       
        }


        public byte[] GetBytes()
        {
            return this.bytes.ToArray();
        }
    }
}
