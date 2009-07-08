using System;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents the tag header.
    /// </summary>
    public class TagDescriptor
    {
        /// <summary>
        /// Creates a new instance of TagDescriptor.
        /// </summary>
        public TagDescriptor()
        {
            MajorVersion = 3;
            Revision = 0;
            Crc = new byte[0];
        }

        /// <summary>
        /// The major version.
        /// </summary>
        public int MajorVersion { get; private set; }

        /// <summary>
        /// The  revision.
        /// </summary>
        public int Revision { get; private set; }

        /// <summary>
        /// The unsynchronisation flag.
        /// </summary>
        public bool Unsynchronisation { get; private set; }

        /// <summary>
        /// The extended header flag.
        /// </summary>
        public bool ExtendedHeader { get; private set; }

        /// <summary>
        /// The experimental indicator flag.
        /// </summary>
        public bool ExperimentalIndicator { get; private set; }

        /// <summary>
        /// The padding size.
        /// </summary>
        public int PaddingSize { get; private set; }

        /// <summary>
        /// The CRC data present flag.
        /// </summary>
        public bool CrcDataPresent { get; private set; }

        /// <summary>
        /// The CRC data.
        /// </summary>
        public byte[] Crc { get; private set; }

        /// <summary>
        /// Sets the version of the ID3 tag.
        /// </summary>
        /// <param name="major">the major version.</param>
        /// <param name="revision">the revision.</param>
        public void SetVersion(int major, int revision)
        {
            MajorVersion = major;
            Revision = revision;
        }

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
        /// Sets the calculated CRC32 values.
        /// </summary>
        /// <param name="crc">the crc Values in bytes (MSB!)</param>
        public void SetCrc32(byte[] crc)
        {
            Crc = crc;
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

        /// <summary>
        /// Overwrites ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("TagDescriptor : Major = {0}, Revision = {1}", MajorVersion, Revision);
        }
    }
}