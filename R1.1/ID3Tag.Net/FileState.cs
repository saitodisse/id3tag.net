using System;
using System.Globalization;

namespace Id3Tag
{
    /// <summary>
    /// Represents the Id3Tag state of a file.
    /// </summary>
    public class FileState
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="FileState"/> class.
        /// </summary>
        /// <param name="id3V1TagFound">if set to <c>true</c>, then ID3 v1 tag is found.</param>
        /// <param name="id3V2TagFound">if set to <c>true</c>, then ID3 v2 tag is found.</param>
        internal FileState(bool id3V1TagFound, bool id3V2TagFound)
        {
            Id3V1TagFound = id3V1TagFound;
            Id3V2TagFound = id3V2TagFound;
        }

        /// <summary>
        /// True if a ID3V1 tag is found otherwise false.
        /// </summary>
        public bool Id3V1TagFound { get; internal set; }

        /// <summary>
        /// True if a ID3V2 tag is found otherwise false.
        /// </summary>
        public bool Id3V2TagFound { get; internal set; }

        /// <summary>
        /// Overwrites the ToString()
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return String.Format(CultureInfo.InvariantCulture, "FileState : V1 Tag Found = {0}, V2 Tag Found = {1}",
                                 Id3V1TagFound, Id3V2TagFound);
        }
    }
}