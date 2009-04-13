using System;
using System.Collections;
using System.IO;
using ID3Tag.HighLevel;
using ID3Tag.LowLevel;

namespace ID3Tag.Net.NUnit
{
    public abstract class Test
    {
        protected IIoController m_Controller;
        protected ITagController m_TagController;
        protected Id3TagInfo m_TagInfo;

        protected void Read(byte[] tagBytes)
        {
            using (var ms = new MemoryStream(tagBytes))
            {
                m_TagInfo = m_Controller.Read(ms);
            }
        }

        protected static long CalculateBitValue(int bits)
        {
            long curValue = 0;

            for (var curPos = 0; curPos < bits; curPos++)
            {
                curValue += Convert.ToInt64(Math.Pow(2, curPos));
            }

            return curValue;
        }

        protected static byte[] CalculateSize(long payload)
        {
            //
            // Transform to binary.
            //
            var binCoding = new BitArray(28);
            var bitPos = 0;
            var curValue = payload;

            while (curValue != 0)
            {
                var remainder = curValue%2;
                curValue = curValue/2;

                if (remainder == 1)
                {
                    binCoding.Set(bitPos, true);
                }
                else
                {
                    binCoding.Set(bitPos, false);
                }

                bitPos++;
            }

            //
            //  Convert to byte[] structure
            //
            var sizeBytes = new byte[4];

            var b4 = CalculateFromBitSet(binCoding, 0, 6);
            var b3 = CalculateFromBitSet(binCoding, 7, 13);
            var b2 = CalculateFromBitSet(binCoding, 14, 20);
            var b1 = CalculateFromBitSet(binCoding, 21, 27);

            sizeBytes[3] = b4;
            sizeBytes[2] = b3;
            sizeBytes[1] = b2;
            sizeBytes[0] = b1;

            return sizeBytes;
        }

        private static byte CalculateFromBitSet(BitArray binCoding, int startIndex, int stopIndex)
        {
            var bitPos = 0;
            byte curValue = 0;
            for (var curIndex = startIndex; curIndex <= stopIndex; curIndex++)
            {
                var level = binCoding.Get(curIndex);
                if (level)
                {
                    curValue += Convert.ToByte(Math.Pow(2, bitPos));
                }

                bitPos++;
            }

            return curValue;
        }

        protected static byte[] GetCompleteTag(byte[] frameBytes)
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            // Update the size of the tag.
            var size = CalculateSize(frameBytes.Length);
            Array.Copy(size, 0, headerBytes, 6, 4);

            var completeLength = headerBytes.Length + frameBytes.Length;
            var completeBytes = new byte[completeLength];

            Array.Copy(headerBytes, 0, completeBytes, 0, headerBytes.Length);
            Array.Copy(frameBytes, 0, completeBytes, headerBytes.Length, frameBytes.Length);

            return completeBytes;
        }

        protected static bool ComparePayload(byte[] bytes, byte[] refBytes)
        {
            var ok = true;

            if (bytes == null && refBytes == null)
            {
                return true;
            }

            if (bytes.Length != refBytes.Length)
            {
                ok = false;
            }
            else
            {
                for (var i = 0; i < bytes.Length; i++)
                {
                    if (bytes[i] != refBytes[i])
                    {
                        ok = false;
                        break;
                    }
                }
            }

            return ok;
        }
    }
}