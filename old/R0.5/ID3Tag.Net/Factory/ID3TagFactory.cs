﻿using ID3Tag.HighLevel;
using ID3Tag.LowLevel;

namespace ID3Tag.Factory
{
    //TODO: Move this factory to a upper namespace ( ID3Tag only )

    /// <summary>
    /// Factory for the ID3 tag controller.
    /// </summary>
    public static class Id3TagFactory
    {
        /// <summary>
        /// Creates a new I/O controller.
        /// </summary>
        /// <returns>a new I/O controller.</returns>
        public static IIoController CreateIoController()
        {
            return new IoController();
        }

        /// <summary>
        /// Creates a new tag controller.
        /// </summary>
        /// <returns>a new ITagController.</returns>
        public static ITagController CreateTagController()
        {
            return new TagController();
        }

        /// <summary>
        /// Creates a new ID3v1 converter.
        /// </summary>
        /// <returns>a new converter.</returns>
        public static IId3V1Controller CreateId3V1Controller()
        {
            return new Id3V1Controller();
        }
    }
}