using System;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents the ID3 Frame header.
    /// </summary>
    public class FrameDescriptor
    {
        /// <summary>
        /// Creates a new instance of FrameDescriptor
        /// </summary>
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

        /// <summary>
        /// The frame ID.
        /// </summary>
        public string ID { get; set; }
        /// <summary>
        /// The TagAlterPreservation flag.
        /// </summary>
        public bool TagAlterPreservation { get; set; }
        /// <summary>
        /// The FileAlterPreservation flag.
        /// </summary>
        public bool FileAlterPreservation { get; set; }
        /// <summary>
        /// The ReadOnly flag.
        /// </summary>
        public bool ReadOnly { get; set; }
        /// <summary>
        /// The Compression flag.
        /// </summary>
        public bool Compression { get; set; }
        /// <summary>
        /// The Encryption flag.
        /// </summary>
        public bool Encryption { get; set; }
        /// <summary>
        /// The GroupingIdentify flag.
        /// </summary>
        public bool GroupingIdentify { get; set; }

        /// <summary>
        /// Gets the flag coding of the frame.
        /// </summary>
        /// <returns>the byte array with the coding.</returns>
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

        /// <summary>
        /// Overwrites ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format("FrameDescriptor : ID = {0}", ID);
        }
    }
}