using System.Diagnostics.CodeAnalysis;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Contains ver 2.4 specifics of <see cref="TagContainer"/>
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "This is not exactly a collection.")]
    public class TagContainerV4 : TagContainer
    {
        private readonly TagDescriptorV4 m_Descriptor;

        /// <summary>
        /// Initializes a new instance of the <see cref="TagContainerV4"/> class.
        /// </summary>
        public TagContainerV4() : base(TagVersion.Id3V24)
        {
            m_Descriptor = new TagDescriptorV4();
        }

        /// <summary>
        /// Gets the tag.
        /// </summary>
        /// <value>The tag.</value>
        public TagDescriptorV4 Tag
        {
            get { return m_Descriptor; }
        }
    }
}