using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

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
    }
}
