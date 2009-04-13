using System;

namespace ID3Tag.HighLevel
{
    public class FrameDescriptor
    {
        public FrameDescriptor()
        {
            ID = "????";
            TagAlterPreservation = false;
            FileAlterPreservation = false;
            ReadOnly = false;
            Compression = false;
            Encryption = false;
            GroupingIdentify = false;
        }

        public string ID { get; set; }
        public bool TagAlterPreservation { get; set; }
        public bool FileAlterPreservation { get; set; }
        public bool ReadOnly { get; set; }
        public bool Compression { get; set; }
        public bool Encryption { get; set; }
        public bool GroupingIdentify { get; set; }

        public byte[] GetFlagBytes()
        {
            /*
             * %abc00000 %ijk00000 
             * 
             * 
             * a = Tag Alter Preservation
             * b = File Alter Preservation
             * c = Read Only
             * 
             * i = Compression
             * j = Encryption
             * k = GroupingIdentify
             */

            ushort flagValue = 0;

            if (TagAlterPreservation)
            {
                flagValue |= 0x0080;
            }

            if (FileAlterPreservation)
            {
                flagValue |= 0x0040;
            }

            if (ReadOnly)
            {
                flagValue |= 0x0020;
            }

            if (Compression)
            {
                flagValue |= 0x8000;
            }

            if (Encryption)
            {
                flagValue |= 0x4000;
            }

            if (GroupingIdentify)
            {
                flagValue |= 0x2000;
            }

            var flagBytes = BitConverter.GetBytes(flagValue);
            return flagBytes;
        }

        public override string ToString()
        {
            return String.Format("FrameDescriptor : ID = {0}", ID);
        }
    }
}