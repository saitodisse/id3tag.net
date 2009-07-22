using System;

namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Represents the extended tag header of the ID3V2.4 tag.
    /// </summary>
    public class ExtendedTagHeaderV4 : ExtendedHeader
    {
        private ExtendedTagHeaderV4()
        {
        }

        /// <summary>
        /// True if the Update Tag flag is set otherwise false.
        /// </summary>
        public bool UpdateTag { get; internal set; }

        /// <summary>
        /// True if the Restriction flag is set otherwise false.
        /// </summary>
        public bool RestrictionPresent { get; internal set; }

        /// <summary>
        /// The Restriction byte
        /// </summary>
        public byte Restriction { get; internal set; }

        internal static ExtendedTagHeaderV4 Create(bool updateFlag, bool crcPresent, bool restrictionPresent, byte restriction, byte[] crc32)
        {
            var extendedHeader = new ExtendedTagHeaderV4();

            extendedHeader.UpdateTag = updateFlag;
            extendedHeader.CrcDataPresent = crcPresent;
            extendedHeader.RestrictionPresent = restrictionPresent;
            extendedHeader.Restriction = restriction;
            extendedHeader.Crc32 = crc32;

            return extendedHeader;
        }

        internal static ExtendedTagHeaderV4 Create(byte[] content)
        {
            var extendedHeader = new ExtendedTagHeaderV4();
            // ignore content[0] ( always 01 )
            var flagByte = content[1];
            var updateFlag = (flagByte & 0x40) == 0x40;
            var crc32Flag = (flagByte & 0x20) == 0x020;
            var restrictionFlag = (flagByte & 0x10) == 0x10;

            var startIndex = 2;
            if (updateFlag)
            {
                // Read the update content

                //TODO: The length is always 0. hm... better implementation here?
                var length = content[startIndex];
                startIndex++;

                extendedHeader.UpdateTag = true;
            }

            if (crc32Flag)
            {
                // Read the crc32

                var crcBytes = new byte[5];
                Array.Copy(content, startIndex + 1, crcBytes, 0, 5);

                startIndex += 6;

                extendedHeader.CrcDataPresent = true;
                extendedHeader.Crc32 = crcBytes;
            }

            if (restrictionFlag)
            {
                // Read the restriction byte

                var restrictionByte = content[startIndex + 1];

                extendedHeader.RestrictionPresent = true;
                extendedHeader.Restriction = restrictionByte;
            }

            return extendedHeader;
        }

        /// <summary>
        /// Defines the extend header type.
        /// </summary>
        public override ExtendedHeaderType HeaderType
        {
            get { return ExtendedHeaderType.V24; }
        }
    }
}