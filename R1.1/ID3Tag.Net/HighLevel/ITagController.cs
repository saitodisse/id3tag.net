﻿using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Represents a highlevel tag controller.
    /// </summary>
    public interface ITagController
    {
        /// <summary>
        /// Decodes a low level tag into a high level.
        /// </summary>
        /// <param name="info">the low level tag.</param>
        /// <returns>the high level tag representation.</returns>
        TagContainer Decode(Id3TagInfo info);

        /// <summary>
        /// Decodes a low level tag into a high level.
        /// </summary>
        /// <param name="info">the low level tag.</param>
        /// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding.</param>
        /// <returns>the high level tag representation.</returns>
        TagContainer Decode(Id3TagInfo info, int codePage);

        /// <summary>
        /// Encodes a high level tag to a low level.
        /// </summary>
        /// <param name="container">the high level tag.</param>
        /// <returns>a low level tag.</returns>
        Id3TagInfo Encode(TagContainer container);
    }
}