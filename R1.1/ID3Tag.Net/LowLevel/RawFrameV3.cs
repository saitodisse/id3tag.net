using System.Collections.Generic;

namespace Id3Tag.LowLevel
{
    /// <summary>
    /// Contains version 2.3 specifics of <see cref="RawFrame"/>
    /// </summary>
    public sealed class RawFrameV3 : RawFrame
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="RawFrameV3"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="options">The options.</param>
        /// <param name="payload">The payload.</param>
        public RawFrameV3(string id, byte[] options, IList<byte> payload)
            : base(id, payload)
        {
            DecodeFlags(options);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawFrameV3"/> class.
        /// </summary>
        /// <param name="id">The id.</param>
        /// <param name="options">The options.</param>
        /// <param name="payload">The payload.</param>
        public RawFrameV3(string id, FrameOptions options, IList<byte> payload)
            : base(id, payload)
        {
            Options = options;
        }

        internal override byte[] EncodeFlags()
        {
            var flagsByte = new byte[2];

            // Decode the flags
            if (Options.TagAlterPreservation)
            {
                flagsByte[0] |= 0x80;
            }

            if (Options.FileAlterPreservation)
            {
                flagsByte[0] |= 0x40;
            }

            if (Options.ReadOnly)
            {
                flagsByte[0] |= 0x20;
            }

            if (Options.Compression)
            {
                flagsByte[1] |= 0x80;
            }

            if (Options.Encryption)
            {
                flagsByte[1] |= 0x40;
            }

            if (Options.GroupingIdentify)
            {
                flagsByte[1] |= 0x20;
            }

            return flagsByte;
        }

        private void DecodeFlags(byte[] flags)
        {
            Options = new FrameOptions();
            Options.TagAlterPreservation = (flags[0] & 0x80) == 0x80;
            Options.FileAlterPreservation = (flags[0] & 0x40) == 0x40;
            Options.ReadOnly = (flags[0] & 0x20) == 0x20;
            Options.Compression = (flags[1] & 0x80) == 0x80;
            Options.Encryption = (flags[1] & 0x40) == 0x40;
            Options.GroupingIdentify = (flags[1] & 0x20) == 0x20;
        }
    }
}