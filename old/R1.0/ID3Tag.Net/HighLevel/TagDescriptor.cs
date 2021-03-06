﻿using System;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents the tag header.
    /// </summary>
    public abstract class TagDescriptor
    {
        /// <summary>
        /// Creates a new instance of TagDescriptor.
        /// </summary>
        protected TagDescriptor(int major, int revision)
        {
            MajorVersion = major;
            Revision = revision;
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
        public bool Unsynchronisation { get; protected set; }

        /// <summary>
        /// The extended header flag.
        /// </summary>
        public bool ExtendedHeader { get; protected set; }

        /// <summary>
        /// The experimental indicator flag.
        /// </summary>
        public bool ExperimentalIndicator { get; protected set; }

        /// <summary>
        /// The CRC data present flag.
        /// </summary>
        public bool CrcDataPresent { get; protected set; }

        /// <summary>
        /// The CRC data.
        /// </summary>
        public byte[] Crc { get; protected set; }

        /// <summary>
        /// Sets the calculated CRC32 values.
        /// </summary>
        /// <param name="crc">the crc Values in bytes (MSB!)</param>
        public void SetCrc32(byte[] crc)
        {
            Crc = crc;
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