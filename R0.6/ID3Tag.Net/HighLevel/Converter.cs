using System;
using System.Collections.Generic;
using System.Text;

namespace ID3Tag.HighLevel
{
    internal static class Converter
    {
		public static char[] Extract(TextEncodingType type, int codePage, byte[] content)
        {
            return PerformExtract(type, codePage, content, false);
        }

		public static char[] Extract(TextEncodingType type, int codePage, List<byte> content)
        {
            var contentBytes = content.ToArray();
            return PerformExtract(type, codePage, contentBytes, false);
        }

		/// <summary>
		/// Converts <paramref name="content"/> into an array of characters.
		/// </summary>
		/// <param name="type">The encoding type.</param>
		/// <param name="codePage">The code page for Ansi encoding.</param>
		/// <param name="content">The content.</param>
		/// <param name="abortAfterTermination">if set to <c>true</c> [abort after termination].</param>
		/// <returns></returns>
        public static char[] Extract(TextEncodingType type, int codePage, byte[] content, bool abortAfterTermination)
        {
            return PerformExtract(type, codePage, content, abortAfterTermination);
        }

        public static List<byte[]> SplitByteArray(byte[] data)
        {
            var byteArrays = new List<byte[]>();

            var bytes = new List<byte>();
            for (var i = 0; i < data.Length; i++)
            {
                if (data[i] == 0)
                {
                    byteArrays.Add(bytes.ToArray());
                    bytes = new List<byte>();
                }
                else
                {
                    bytes.Add(data[i]);
                }
            }

            if (bytes.Count != 0)
            {
                byteArrays.Add(bytes.ToArray());
            }

            return byteArrays;
        }

        public static int GetTerminationCharLength(TextEncodingType type)
        {
            int length;
            switch (type)
            {
                case TextEncodingType.Ansi:
                    length = 1;
                    break;
                case TextEncodingType.UTF16:
                    length = 2;
                    break;
                case TextEncodingType.UTF16_BE:
                    length = 2;
                    break;
                case TextEncodingType.UTF8:
                    length = 1;
                    break;
                default:
                    throw new ID3TagException("Could not extract TextEncoding (" + type + ").");
            }

            return length;
        }

        /// <summary>
        /// Splits an array of chars in a description/value pair. 
        /// </summary>
        /// <param name="data"></param>
        /// <param name="description"></param>
        /// <param name="value"></param>
        public static void DecodeDescriptionValuePairs(char[] data, out string description, out string value)
        {
            var descBuilder = new StringBuilder();
            var valueBuilder = new StringBuilder();
            var terminationFound = false;

            foreach (var curValue in data)
            {
                if (terminationFound)
                {
                    valueBuilder.Append(curValue);
                }
                else
                {
                    if (curValue == 0)
                    {
                        terminationFound = true;
                    }
                    else
                    {
                        descBuilder.Append(curValue);
                    }
                }
            }

            description = descBuilder.ToString();
            value = valueBuilder.ToString();
        }

        public static bool DetermineTerminateSymbol(byte[] data, int pos, int increment)
        {
            var isNull = true;
            for (var i = 0; i < increment; i++)
            {
                if (data[pos + i] == 0x00)
                {
                    isNull &= true;
                }
                else
                {
                    isNull &= false;
                }
            }

            return isNull;
        }

        internal static byte[] GetContentBytes(TextEncodingType encodingType, int codePage, string content)
        {
            byte[] contentBytes;
            switch (encodingType)
            {
                case TextEncodingType.Ansi:
					var asciiEncoding = codePage == 0 ? Encoding.Default : Encoding.GetEncoding(codePage);
					if (!asciiEncoding.IsSingleByte)
					{
						throw new InvalidOperationException(String.Format("Code page {0} cannot be used for single-byte Ansi encoding.", codePage));
					}
                    contentBytes = asciiEncoding.GetBytes(content);
                    break;
                case TextEncodingType.UTF16:
                    var utf16Encoding = new UnicodeEncoding(false, true);
                    var bom = utf16Encoding.GetPreamble();
                    var dataBytes = utf16Encoding.GetBytes(content);

                    // Copy BOM and Content together in an array
                    contentBytes = new byte[bom.Length + dataBytes.Length];
                    Array.Copy(bom, 0, contentBytes, 0, bom.Length);
                    Array.Copy(dataBytes, 0, contentBytes, bom.Length, dataBytes.Length);
                    break;
                case TextEncodingType.UTF16_BE:
                    var utf16BEEncoding = new UnicodeEncoding(true, true);
                    contentBytes = utf16BEEncoding.GetBytes(content);
                    break;
                case TextEncodingType.UTF8:
                    var utf8Encoding = new UTF8Encoding();
                    contentBytes = utf8Encoding.GetBytes(content);
                    break;
                default:
                    throw new ID3TagException("Unknown Encoding Type found!");
            }

            return contentBytes;
        }

        #region Private Helper

        private static char[] ExtractAnsi(int codePage, byte[] content, bool abortAfterTermination)
        {
        	var encoding = Encoding.GetEncoding(codePage);

			if (!encoding.IsSingleByte)
			{
				throw new InvalidOperationException(String.Format("Code page {0} cannot be used for single-byte Ansi encoding.", codePage));
			}

        	var count = content.Length;
            if (abortAfterTermination)
            {
            	for (var index = 0; index < content.Length; index++)
            	{
					if (content[index] == 0x00)
					{
						count = index;
						break;
					}
            	}
            }

        	return encoding.GetChars(content, 0, count);
        }

        private static char[] PerformExtract(TextEncodingType type, int codePage, byte[] content, bool abortAfterTermination)
        {
            char[] chars;
            switch (type)
            {
                case TextEncodingType.Ansi:
                    chars = ExtractAnsi(codePage, content, abortAfterTermination);
                    break;
                case TextEncodingType.UTF16:
                    chars = ExtractUTF16(content, abortAfterTermination);
                    break;
                case TextEncodingType.UTF16_BE:
                    // Ohne BOM
                    chars = ExtractUTF16_BigEndian(content, abortAfterTermination);
                    break;
                case TextEncodingType.UTF8:
                    chars = ExtractUTF8(content, abortAfterTermination);
                    break;
                default:
                    throw new ID3TagException("Could not extract TextEncoding (" + type + ").");
            }

            return chars;
        }

        private static char[] ExtractUTF8(byte[] content, bool abortAfterTermination)
        {
            // $03   UTF-8 [UTF-8] encoded Unicode [UNICODE]. Terminated with $00.

            var data = new List<char>();
            var encoder = new UTF8Encoding();

            var chars = encoder.GetChars(content, 0, content.Length);
            foreach (var curChar in chars)
            {
                if (abortAfterTermination && curChar == '\u0000')
                {
                    break;
                }

                data.Add(curChar);
            }

            return data.ToArray();
        }

        private static char[] ExtractUTF16_BigEndian(byte[] content, bool abortAfterTermination)
        {
            var data = new List<char>();

            // Create a Big Endian UTF 16 Encoder
            var encoder = new UnicodeEncoding(true, true);
            var chars = encoder.GetChars(content, 0, content.Length);
            foreach (var curChar in chars)
            {
                if (abortAfterTermination && curChar == '\u0000')
                {
                    break;
                }

                data.Add(curChar);
            }

            return data.ToArray();
        }

        private static char[] ExtractUTF16(byte[] content, bool abortAfterTermination)
        {
            var data = new List<char>();

            if (content.Length > 4)
            {
                var bomBytes = new byte[2];
                Array.Copy(content, 0, bomBytes, 0, 2);
                var bigEndianEnabled = false;

                //
                //  See Byte Order Mask (BOM) UTF 16 for details
                //
                if (bomBytes[0] == 0xFE && bomBytes[1] == 0xFF)
                {
                    bigEndianEnabled = true;
                }

                var encoder = new UnicodeEncoding(bigEndianEnabled, true);
                var inputBytes = new byte[content.Length - 2];
                Array.Copy(content, 2, inputBytes, 0, inputBytes.Length);

                var chars = encoder.GetChars(inputBytes, 0, inputBytes.Length);
                foreach (var curChar in chars)
                {
                    if (abortAfterTermination && curChar == '\u0000')
                    {
                        break;
                    }

                    data.Add(curChar);
                }
            }

            return data.ToArray();
        }

        #endregion
    }
}