namespace ID3Tag.HighLevel
{
    public class TagContainerV4 : TagContainer
    {
        private readonly TagDescriptorV4 m_Descriptor;

        public TagContainerV4() : base(TagVersion.Id3V24)
        {
            m_Descriptor = new TagDescriptorV4();
        }

        public TagDescriptorV4 Tag
        {
            get { return m_Descriptor; }
        }
    }
}