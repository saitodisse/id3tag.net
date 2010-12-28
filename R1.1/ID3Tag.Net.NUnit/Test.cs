using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using Id3Tag.HighLevel;
using Id3Tag.LowLevel;

namespace Id3Tag.Net.NUnit
{
    public abstract class Test
    {
        protected byte[] m_AudioData;
        protected IIOController m_Controller;
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

            for (int curPos = 0; curPos < bits; curPos++)
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
            int bitPos = 0;
            long curValue = payload;

            while (curValue != 0)
            {
                long remainder = curValue%2;
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

            byte b4 = CalculateFromBitSet(binCoding, 0, 6);
            byte b3 = CalculateFromBitSet(binCoding, 7, 13);
            byte b2 = CalculateFromBitSet(binCoding, 14, 20);
            byte b1 = CalculateFromBitSet(binCoding, 21, 27);

            sizeBytes[3] = b4;
            sizeBytes[2] = b3;
            sizeBytes[1] = b2;
            sizeBytes[0] = b1;

            return sizeBytes;
        }

        private static byte CalculateFromBitSet(BitArray binCoding, int startIndex, int stopIndex)
        {
            int bitPos = 0;
            byte curValue = 0;
            for (int curIndex = startIndex; curIndex <= stopIndex; curIndex++)
            {
                bool level = binCoding.Get(curIndex);
                if (level)
                {
                    curValue += Convert.ToByte(Math.Pow(2, bitPos));
                }

                bitPos++;
            }

            return curValue;
        }

        protected static byte[] GetCompleteV3Tag(byte[] frameBytes)
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x03, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            // Update the size of the tag.
            byte[] size = CalculateSize(frameBytes.Length);
            Array.Copy(size, 0, headerBytes, 6, 4);

            int completeLength = headerBytes.Length + frameBytes.Length;
            var completeBytes = new byte[completeLength];

            Array.Copy(headerBytes, 0, completeBytes, 0, headerBytes.Length);
            Array.Copy(frameBytes, 0, completeBytes, headerBytes.Length, frameBytes.Length);

            return completeBytes;
        }

        protected static byte[] GetCompleteV4Tag(byte[] frameBytes)
        {
            var headerBytes = new byte[] {0x49, 0x44, 0x33, 0x04, 0x00, 0x00, 0x00, 0x00, 0x00, 0x00};

            // Update the size of the tag.
            byte[] size = CalculateSize(frameBytes.Length);
            Array.Copy(size, 0, headerBytes, 6, 4);

            int completeLength = headerBytes.Length + frameBytes.Length;
            var completeBytes = new byte[completeLength];

            Array.Copy(headerBytes, 0, completeBytes, 0, headerBytes.Length);
            Array.Copy(frameBytes, 0, completeBytes, headerBytes.Length, frameBytes.Length);

            return completeBytes;
        }

        protected static bool ComparePayload(IList<byte> bytes, IList<byte> refBytes)
        {
            bool ok = true;

            if (bytes == null && refBytes == null)
            {
                return true;
            }

            if (bytes.Count != refBytes.Count)
            {
                ok = false;
            }
            else
            {
                for (int i = 0; i < bytes.Count; i++)
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

        protected static void FillData(byte[] array)
        {
            FillData(array, 0);
        }

        protected static void FillData(byte[] array, byte startValue)
        {
            byte curValue = startValue;

            for (int i = 0; i < array.Length; i++)
            {
                array[i] = curValue;

                if (curValue < 0xFF)
                {
                    curValue++;
                }
                else
                {
                    curValue = 0;
                }
            }
        }
    }
}