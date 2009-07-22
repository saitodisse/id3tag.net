namespace ID3Tag.HighLevel
{
    public class TagContainerV3 : TagContainer
    {
        private readonly TagDescriptorV3 m_Descriptor;

        public TagContainerV3() : base(TagVersion.Id3V23)
        {
            m_Descriptor = new TagDescriptorV3();
        }

        public TagDescriptorV3 Tag
        {
            get { return m_Descriptor; }
        }
    }
}