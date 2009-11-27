using Id3Tag.HighLevel;
using Id3Tag.LowLevel;

namespace Id3Tag
{
    /// <summary>
    /// Factory for the ID3 tag controller.
    /// </summary>
    public static class Id3TagFactory
    {
        /// <summary>
        /// Creates a new I/O controller.
        /// </summary>
        /// <returns>a new I/O controller.</returns>
        public static IIOController CreateIOController()
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

        /// <summary>
        /// Creates a new ID3 Tag representation. 
        /// </summary>
        /// <param name="version">the ID3 Tag version.</param>
        /// <returns>the representation instance.</returns>
        public static TagContainer CreateId3Tag(TagVersion version)
        {
            TagContainer container;

            switch (version)
            {
                case TagVersion.Id3V23:
                    container = new TagContainerV3();
                    break;
                case TagVersion.Id3V24:
                    container = new TagContainerV4();
                    break;
                default:
                    throw new Id3TagException("Invalid tag version!");
            }

            return container;
        }

		/// <summary>
		/// Creates new instance of <see cref="Id3TagManager"/>.
		/// </summary>
		/// <returns>New instance of new instance of <see cref="Id3TagManager"/>.</returns>
		public static Id3TagManager CreateId3TagManager()
		{
			return new Id3TagManager();
		}
    }
}