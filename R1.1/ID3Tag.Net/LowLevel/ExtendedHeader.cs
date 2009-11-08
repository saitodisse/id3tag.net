namespace ID3Tag.LowLevel
{
    /// <summary>
    /// Defines the basic subset of an Extended Header.
    /// </summary>
    public abstract class ExtendedHeader
    {
        /// <summary>
        /// Gets the header version
        /// </summary>
        public abstract ExtendedHeaderType HeaderType { get; }

        /// <summary>
        /// True if the Crc32 flag is set otherwise false.
        /// </summary>
        public bool CrcDataPresent { get; internal set; }

        /// <summary>
        /// The CRC32 bytes
        /// </summary>
        public byte[] Crc32 { get; internal set; }

        /// <summary>
        /// Convert the header to ID3V2.3
        /// </summary>
        /// <returns>an ID3v2.3 header</returns>
        public ExtendedTagHeaderV3 ConvertToV23()
        {
            if (HeaderType == ExtendedHeaderType.V23)
            {
                var extendedHeader = this as ExtendedTagHeaderV3;

                if (extendedHeader == null)
                {
                    throw new ID3TagException("Cannot convert header to ID3V2.3");
                }

                return extendedHeader;
            }

            throw new ID3TagException("Cannot convert header to ID3V2.3");
        }

        /// <summary>
        /// Convert the extended header to ID3v2.4
        /// </summary>
        /// <returns>an ID3V2.4 extended header</returns>
        public ExtendedTagHeaderV4 ConvertToV24()
        {
            if (HeaderType == ExtendedHeaderType.V24)
            {
                var extendedHeader = this as ExtendedTagHeaderV4;

                if (extendedHeader == null)
                {
                    throw new ID3TagException("Cannot convert header to ID3V2.4");
                }

                return extendedHeader;
            }

            throw new ID3TagException("Cannot convert header to ID3V2.4");
        }
    }
}