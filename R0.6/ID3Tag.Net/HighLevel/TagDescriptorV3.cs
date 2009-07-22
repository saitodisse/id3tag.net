using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents an TagDescriptor for ID3v2.3 spec.
    /// </summary>
    public class TagDescriptorV3 : TagDescriptor
    {
        /// <summary>
        /// Creates a new instance of a TagDescriptorV3
        /// </summary>
        public TagDescriptorV3()
            : base(3, 0)
        {
        }

        /// <summary>
        /// The padding size.
        /// </summary>
        public int PaddingSize { get; private set; }

        /// <summary>
        /// Sets the extended header values.
        /// </summary>
        /// <param name="paddingSize">the padding size.</param>
        /// <param name="crcDataPresent">the CRC data present flag</param>
        public void SetExtendedHeader(int paddingSize, bool crcDataPresent)
        {
            PaddingSize = paddingSize;
            CrcDataPresent = crcDataPresent;
        }

        /// <summary>
        /// Set the header flags.
        /// </summary>
        /// <param name="unsynchronisation">the unsynchronisation flag.</param>
        /// <param name="extendedHeader">the extended header flag.</param>
        /// <param name="experimentalIndicator">the experimental indicator flag.</param>
        public void SetHeaderFlags(bool unsynchronisation, bool extendedHeader, bool experimentalIndicator)
        {
            Unsynchronisation = unsynchronisation;
            ExtendedHeader = extendedHeader;
            ExperimentalIndicator = experimentalIndicator;
        }
    }
}
