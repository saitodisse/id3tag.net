﻿using System;
using System.Globalization;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel
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
            Id = "????";
            TagAlterPreservation = false;
            FileAlterPreservation = false;
            ReadOnly = false;
            Compression = false;
            Encryption = false;
            GroupingIdentify = false;
            Unsynchronisation = false;
            DataLengthIndicator = false;
        }

        /// <summary>
        /// The frame ID.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// The Unsynchronisation flag ( ID3v2.4 only)
        /// </summary>
        public bool Unsynchronisation { get; set; }

        /// <summary>
        /// The DataLengthIndicator ( ID3v2.4 only)
        /// </summary>
        public bool DataLengthIndicator { get; set; }

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
        /// Gets the Flags
        /// </summary>
        /// <returns>The flags</returns>
        public FrameOptions Options
        {
            get
            {
                var flags = new FrameOptions
                                {
                                    Compression = Compression,
                                    DataLengthIndicator = DataLengthIndicator,
                                    Encryption = Encryption,
                                    FileAlterPreservation = FileAlterPreservation,
                                    GroupingIdentify = GroupingIdentify,
                                    ReadOnly = ReadOnly,
                                    TagAlterPreservation = TagAlterPreservation,
                                    Unsynchronisation = Unsynchronisation
                                };

                return flags;
            }
        }

        /// <summary>
        /// Overwrites ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "FrameDescriptor : ID = {0}", Id);
        }
    }
}