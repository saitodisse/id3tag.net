using System;

namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Represents the extended tag header of the ID3 tag.
    /// </summary>
    public class ExtendedTagHeaderV3
    {
        private ExtendedTagHeaderV3()
        {
        }

        /// <summary>
        /// True if CRC data is present.
        /// </summary>
        public bool CRCDataPresent { get; private set; }

        /// <summary>
        /// The padding size.
        /// </summary>
        public int PaddingSize { get; private set; }

        /// <summary>
        /// The CRC bytes.
        /// </summary>
        public byte[] CRC { get; private set; }

        #region Create Extended Header

        internal static ExtendedTagHeaderV3 Create(int paddingSize, bool crcDataPresent, byte[] crc)
        {
            var extendedHeader = new ExtendedTagHeaderV3
                                     {
                                         PaddingSize = paddingSize,
                                         CRCDataPresent = crcDataPresent,
                                         CRC = crc
                                     };

            return extendedHeader;
        }

        internal static ExtendedTagHeaderV3 Create(byte[] content)
        {
            var flags = new byte[2];
            var paddingBytes = new byte[4];

            Array.Copy(content, 0, flags, 0, 2);
            Array.Copy(content, 2, paddingBytes, 0, 4);

            var extendedHeader = new ExtendedTagHeaderV3();
            extendedHeader.CRCDataPresent = (flags[0] & 0x80) == 0x80;
            extendedHeader.PaddingSize = Utils.CalculateExtendedHeaderPaddingSize(paddingBytes);

            if (extendedHeader.CRCDataPresent)
            {
                var crcBytes = new byte[4];
                Array.Copy(content, 6, crcBytes, 0, 4);
                extendedHeader.CRC = crcBytes;
            }

            return extendedHeader;
        }

        #endregion
    }
}