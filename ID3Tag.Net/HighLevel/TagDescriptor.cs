using System;

namespace ID3Tag.HighLevel
{
    public class TagDescriptor
    {
        public TagDescriptor()
        {
            MajorVersion = 3;
            Revision = 0;
            Crc = new byte[0];
        }

        public int MajorVersion { get; private set; }
        public int Revision { get; private set; }
        public bool Unsynchronisation { get; private set; }
        public bool ExtendedHeader { get; private set; }
        public bool ExperimentalIndicator { get; private set; }

        public int PaddingSize { get; private set; }
        public bool CrcDataPresent { get; private set; }
        public byte[] Crc { get; private set; }

        public void SetVersion(int major, int revision)
        {
            MajorVersion = major;
            Revision = revision;
        }

        public void SetExtendedHeader(int paddingSize, bool crcDataPresent, byte[] crcData)
        {
            PaddingSize = paddingSize;
            CrcDataPresent = crcDataPresent;
            Crc = crcData;
        }

        public void SetHeaderFlags(bool unsynchronisation, bool extendedHeader, bool experimentalIndicator)
        {
            Unsynchronisation = unsynchronisation;
            ExtendedHeader = extendedHeader;
            ExperimentalIndicator = experimentalIndicator;
        }

        public override string ToString()
        {
            return String.Format("TagDescriptor : Major = {0}, Revision = {1}", MajorVersion, Revision);
        }
    }
}