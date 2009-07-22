using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents an ID3v2.4 tag 
    /// </summary>
    public class TagDescriptorV4 : TagDescriptor
    {
        /// <summary>
        /// Creates a new TagDescriptorV4 instance
        /// </summary>
        public TagDescriptorV4()
            : base(4, 0)
        {
        }

        /// <summary>
        /// Write a ID3v2.4 footer
        /// </summary>
        public bool Footer { get; set; }

        /// <summary>
        /// True if the Update Tag flag is set otherwise false.
        /// </summary>
        public bool UpdateTag { get; set; }

        /// <summary>
        /// True if the Restriction flag is set otherwise false.
        /// </summary>
        public bool RestrictionPresent { get; set; }

        /// <summary>
        /// The Restriction byte
        /// </summary>
        public byte Restriction { get; set; }

        /// <summary>
        /// Set the header flags.
        /// </summary>
        /// <param name="unsynchronisation">the unsynchronisation flag.</param>
        /// <param name="extendedHeader">the extended header flag.</param>
        /// <param name="experimentalIndicator">the experimental indicator flag.</param>
        /// <param name="footer">the footer flag</param>
        public void SetHeaderFlags(bool unsynchronisation, bool extendedHeader, bool experimentalIndicator,bool footer)
        {
            Unsynchronisation = unsynchronisation;
            ExtendedHeader = extendedHeader;
            ExperimentalIndicator = experimentalIndicator;
            Footer = footer;
        }

        /// <summary>
        /// Sets the extended header values.
        /// </summary>
        /// <param name="crcDataPresent">the CRC data present flag</param>
        /// <param name="updateTag">the update tag</param>
        /// <param name="restriction">the restriction byte</param>
        /// <param name="restrictionPresent">enable the restriction</param>
        public void SetExtendedHeader(bool crcDataPresent,bool updateTag, bool restrictionPresent, byte restriction)
        {
            CrcDataPresent = crcDataPresent;
            UpdateTag = updateTag;
            RestrictionPresent = restrictionPresent;
            Restriction = restriction;
        }
    }
}
