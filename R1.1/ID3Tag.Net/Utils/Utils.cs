using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Id3Tag
{
    internal static class Utils
    {
        /// <summary>
        /// Calculates the size that is used in various parts of the Frame Header.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        internal static int CalculateSize(byte[] data)
        {
            uint size = Convert8BitEncodedToInt32(data);

            // Make sure that size does not exceed max ID3 Tag value and that we could cast it to Int32
            if (size > 268435455)
            {
                throw new Id3TagException(
                    String.Format(CultureInfo.InvariantCulture,
                                  "Size value of {0} exceeds max ID3 tag size value of 268435455).", size));
            }

            return (int) size;
        }

        /// <summary>
        /// Converts 8-bit encoded data to 32-bit integer value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static uint Convert8BitEncodedToInt32(byte[] data)
        {
            #region Params Check

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Length != 4)
            {
                throw new ArgumentException("Array length must be 4", "data");
            }

            #endregion

            uint value = 0;

            for (int i = 0; i < 4; i++)
            {
                value <<= 8;
                value += data[i];
            }

            return value;
        }

        /// <summary>
        /// Converts 7-bit encoded data to 32-bit integer value.
        /// </summary>
        /// <param name="data">The data.</param>
        /// <returns></returns>
        public static int Convert7BitEncodedToInt32(byte[] data)
        {
            #region Params Check

            if (data == null)
            {
                throw new ArgumentNullException("data");
            }

            if (data.Length != 4)
            {
                throw new ArgumentException("Array length must be 4", "data");
            }

            #endregion

            uint value = 0;

            for (int i = 0; i < 4; i++)
            {
                byte b = data[i];
                if (b > 0x7F)
                {
                    throw new Id3TagException("Byte value exceeds 127.");
                }

                value <<= 7;
                value += b;
            }

            return (int) value;
        }

        /// <summary>
        /// Converts byte array to a string of printable characters.
        /// </summary>
        /// <param name="bytes">The bytes.</param>
        /// <returns></returns>
        public static string BytesToString(IList<byte> bytes)
        {
            if (bytes == null || bytes.Count == 0)
            {
                return null;
            }

            var sb = new StringBuilder();

            foreach (byte byteValue in bytes)
            {
                sb.AppendFormat("{0:X2} ", byteValue);
            }

            return sb.ToString();
        }

        internal static void WriteAudioStream(Stream output, Stream input, long length)
        {
            var buffer = new byte[64000];
            while (input.Position < length)
            {
                long diff = length - input.Position;
                if (diff < buffer.Length)
                {
                    // Read the rest.
                    int count = Convert.ToInt32(diff);
                    input.Read(buffer, 0, count);
                    output.Write(buffer, 0, count);
                }
                else
                {
                    // Read the next 64K Byte buffer.
                    input.Read(buffer, 0, buffer.Length);
                    output.Write(buffer, 0, buffer.Length);
                }
            }
        }
    }
}