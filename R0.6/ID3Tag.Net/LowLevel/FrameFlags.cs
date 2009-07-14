using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace ID3Tag.LowLevel
{
    public class FrameFlags
    {
        /// <summary>
        /// The TagAlterPreservation flag.
        /// </summary>
        public bool TagAlterPreservation { get; internal set; }

        /// <summary>
        /// The FileAlterPreservation flag.
        /// </summary>
        public bool FileAlterPreservation { get; internal set; }

        /// <summary>
        /// The ReadOnly flag.
        /// </summary>
        public bool ReadOnly { get; internal set; }

        /// <summary>
        /// The Compression flag.
        /// </summary>
        public bool Compression { get; internal set; }

        /// <summary>
        /// The Encryption flag.
        /// </summary>
        public bool Encryption { get; internal set; }

        /// <summary>
        /// The GroupingIdentify flag.
        /// </summary>
        public bool GroupingIdentify { get; internal set; }

        /// <summary>
        /// The Unsynchronisation flag ( ID3V.2.4 only! )
        /// </summary>
        public bool Unsynchronisation { get; internal set; }

        /// <summary>
        /// The DataLengthIndicator ( ID3v2.4. only! )
        /// </summary>
        public bool DataLengthIndicator { get; internal set; }
    }
}
