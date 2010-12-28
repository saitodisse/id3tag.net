using System;
using System.Collections.ObjectModel;

namespace Id3Tag.LowLevel
{
    /// <summary>
    /// Represents the extended tag header of the ID3V2.3 tag.
    /// </summary>
    public class ExtendedTagHeaderV3 : ExtendedHeader
    {
        private ExtendedTagHeaderV3()
        {
        }


        /// <summary>
        /// The padding size.
        /// </summary>
        public int PaddingSize { get; private set; }

        /// <summary>
        /// Defines the extended header type
        /// </summary>
        public override ExtendedHeaderType HeaderType
        {
            get { return ExtendedHeaderType.V23; }
        }

        #region Create Extended Header

        internal static ExtendedTagHeaderV3 Create(int paddingSize, bool crcDataPresent, ReadOnlyCollection<byte> crc)
        {
            var extendedHeader = new ExtendedTagHeaderV3
                                     {
                                         PaddingSize = paddingSize,
                                         CrcDataPresent = crcDataPresent,
                                         Crc32 = crc
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
            extendedHeader.CrcDataPresent = (flags[0] & 0x80) == 0x80;
            extendedHeader.PaddingSize = Utils.CalculateSize(paddingBytes);

            if (extendedHeader.CrcDataPresent)
            {
                var crcBytes = new byte[4];
                Array.Copy(content, 6, crcBytes, 0, 4);
                extendedHeader.Crc32 = new ReadOnlyCollection<byte>(crcBytes);
            }

            return extendedHeader;
        }

        #endregion
    }
}