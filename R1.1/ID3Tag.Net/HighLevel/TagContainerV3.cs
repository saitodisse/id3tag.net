using System.Diagnostics.CodeAnalysis;

namespace Id3Tag.HighLevel
{
	/// <summary>
	/// Contains ver 2.3 specifics for <see cref="TagContainer"/>
	/// </summary>
	[SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix", Justification = "This is not exactly a collection.")]
	public class TagContainerV3 : TagContainer
    {
        private readonly TagDescriptorV3 m_Descriptor;

		/// <summary>
		/// Initializes a new instance of the <see cref="TagContainerV3"/> class.
		/// </summary>
        public TagContainerV3() : base(TagVersion.Id3V23)
        {
            m_Descriptor = new TagDescriptorV3();
        }

		/// <summary>
		/// Gets the tag.
		/// </summary>
		/// <value>The tag.</value>
        public TagDescriptorV3 Tag
        {
            get { return m_Descriptor; }
        }
    }
}