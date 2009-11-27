using System;
using System.Collections.Generic;

namespace Id3Tag
{
    /// <summary>
    /// Creates and Verifies CRC32 values.
    /// </summary>
    internal class Crc32
    {
        ///<summary>
        /// The CRC32 generator polynom.
        ///</summary>
        public const uint DefaultPolynom = 0x04C11DB7;

        private readonly uint m_GeneratorPolynom;

        /// <summary>
        /// Creates a new instance of Crc32.
        /// </summary>
        /// <param name="generatorPolynom">the generator polynom.</param>
        public Crc32(uint generatorPolynom)
        {
            m_GeneratorPolynom = generatorPolynom;
        }

        /// <summary>
        /// Calculates the CRC32 value with given generator polynom in the constructor!
        /// </summary>
        /// <param name="data">the bytes.</param>
        /// <returns>the CRC32 value in MSB!</returns>
        public byte[] Calculate(IEnumerable<byte> data)
        {
            // http://de.wikipedia.org/wiki/CRC32

            int[] bits = GetBits(data);
            uint crc32 = 0;

            foreach (int bit in bits)
            {
                int firstCrcBit;
                if ((crc32 & 0x80000000) == 0x80000000)
                {
                    firstCrcBit = 1;
                }
                else
                {
                    firstCrcBit = 0;
                }

                crc32 <<= 1;
                if (firstCrcBit != bit)
                {
                    crc32 = crc32 ^ m_GeneratorPolynom;
                }
            }

            var crcBytes = BitConverter.GetBytes(crc32);
            Array.Reverse(crcBytes);
            return crcBytes;
        }

		public bool Validate(IEnumerable<byte> data, IEnumerable<byte> crcBytes)
        {
            var bytes = new List<byte>();

            bytes.AddRange(data);
            bytes.AddRange(crcBytes);

            var crc = Calculate(bytes.ToArray());

            //
            //  Convert to LSB and compare.
            //
            Array.Reverse(crc);
            var result = BitConverter.ToUInt32(crc, 0);
            return result == 0;
        }

        #region Helper

        private static int[] GetBits(IEnumerable<byte> data)
        {
            var bits = new List<int>();

            //
            //  MSB Coding!!!
            //
            var patterns = new byte[]
                               {
                                   0x80,
                                   0x40,
                                   0x20,
                                   0x10,
                                   0x08,
                                   0x04,
                                   0x02,
                                   0x01
                               };

            foreach (var curByte in data)
            {
                foreach (var curPattern in patterns)
                {
                    if ((curByte & curPattern) == curPattern)
                    {
                        bits.Add(1);
                    }
                    else
                    {
                        bits.Add(0);
                    }
                }
            }

            return bits.ToArray();
        }

        #endregion
    }
}