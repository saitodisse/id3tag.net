using Id3Tag.HighLevel.Id3Frame;
using Id3Tag.LowLevel;

namespace Id3Tag.HighLevel
{
    internal class TagController : ITagController
    {
        #region ITagController Members

        /// <summary>
        /// Decodes a low level tag into a high level.
        /// </summary>
        /// <param name="info">the low level tag.</param>
        /// <returns>the high level tag representation.</returns>
        public TagContainer Decode(Id3TagInfo info)
        {
            return Decode(info, 0);
        }

        /// <summary>
        /// Decodes a low level tag into a high level.
        /// </summary>
        /// <param name="info">the low level tag.</param>
        /// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding.</param>
        /// <returns>the high level tag representation.</returns>
        public TagContainer Decode(Id3TagInfo info, int codePage)
        {
            TagContainer container;
            switch (info.MajorVersion)
            {
                case 3:
                    container = DecodeV3Tag(info);
                    break;
                case 4:
                    container = DecodeV4Tag(info);
                    break;
                default:
                    var ex = new Id3TagException("This major revision is not supported!");
                    Logger.LogError(ex);
                    throw ex;
            }

            //
            //  Import the frames
            //
            IFrameCreationService creationService = new FrameContainer();
            foreach (RawFrame rawFrame in info.Frames)
            {
                //
                //  Analyse the frame ID
                //
                IFrame frame = AnalyseFrameId(rawFrame, creationService);
                if (frame != null)
                {
                    frame.Import(rawFrame, codePage);
                    container.Add(frame);
                }
                else
                {
                    var ex = new Id3TagException("Frame analysing failed!");
                    Logger.LogError(ex);

                    throw ex;
                }
            }

            return container;
        }

        public Id3TagInfo Encode(TagContainer container)
        {
            var tagInfo = new Id3TagInfo();

            switch (container.TagVersion)
            {
                case TagVersion.Id3V23:
                    tagInfo.MajorVersion = 3;
                    tagInfo.Revision = 0;
                    EncodeV3(tagInfo, container);
                    break;
                case TagVersion.Id3V24:
                    tagInfo.MajorVersion = 4;
                    tagInfo.Revision = 0;
                    EncodeV4(tagInfo, container);
                    break;
                default:
                    var ex = new Id3TagException("Unknown version!");
                    Logger.LogError(ex);

                    throw ex;
            }

            foreach (IFrame frame in container)
            {
                RawFrame rawFrame = frame.Convert(container.TagVersion);
                tagInfo.Frames.Add(rawFrame);
            }

            return tagInfo;
        }

        #endregion

        #region Private Helper

        private static IFrame AnalyseFrameId(RawFrame rawFrame, IFrameCreationService frameService)
        {
            string id = rawFrame.Id;
            IFrame frame = null;

            if (frameService.Search(id))
            {
                //
                //  Get the specific frame instance
                //
                frame = frameService.GetFrameInstance(id);
            }
            else
            {
                if (id[0] == 'T' && id[1] != 'X')
                {
                    //
                    // Handle Textfames
                    //
                    frame = frameService.GetTextFrame();
                }

                if (id[0] == 'W' && id[1] != 'X')
                {
                    //
                    // Handle Web Frames
                    //
                    frame = frameService.GetUrlLinkFrame();
                }
            }

            if (frame == null)
            {
                //
                //  If all is failed then create an Unknown frame instance
                //
                frame = new UnknownFrame();
            }

            return frame;
        }

        private static TagContainer DecodeV4Tag(Id3TagInfo info)
        {
            var container = new TagContainerV4();
            TagDescriptorV4 descriptor = container.Tag;

            descriptor.SetHeaderOptions(info.Unsynchronised, info.ExtendedHeaderAvailable, info.Experimental,
                                        info.HasFooter);
            if (info.ExtendedHeaderAvailable)
            {
                ExtendedTagHeaderV4 extendedHeader = info.ExtendedHeader.ConvertToV24();
                descriptor.SetExtendedHeader(extendedHeader.CrcDataPresent, extendedHeader.UpdateTag,
                                             extendedHeader.RestrictionPresent, extendedHeader.Restriction);

                if (extendedHeader.CrcDataPresent)
                {
                    descriptor.SetCrc32(extendedHeader.Crc32);
                }
            }

            return container;
        }

        private static TagContainer DecodeV3Tag(Id3TagInfo info)
        {
            var container = new TagContainerV3();
            TagDescriptorV3 descriptor = container.Tag;

            descriptor.SetHeaderOptions(info.Unsynchronised, info.ExtendedHeaderAvailable, info.Experimental);

            if (info.ExtendedHeaderAvailable)
            {
                ExtendedTagHeaderV3 extendedHeader = info.ExtendedHeader.ConvertToV23();
                descriptor.SetExtendedHeader(extendedHeader.PaddingSize, extendedHeader.CrcDataPresent);
                if (extendedHeader.CrcDataPresent)
                {
                    descriptor.SetCrc32(extendedHeader.Crc32);
                }
            }

            return container;
        }

        private static void EncodeV4(Id3TagInfo tagInfo, TagContainer container)
        {
            TagDescriptorV4 descriptor = container.GetId3V24Descriptor();

            tagInfo.Experimental = descriptor.ExperimentalIndicator;
            tagInfo.ExtendedHeaderAvailable = descriptor.ExtendedHeader;
            tagInfo.Unsynchronised = descriptor.Unsynchronisation;
            tagInfo.HasFooter = descriptor.Footer;

            if (descriptor.ExtendedHeader)
            {
                tagInfo.ExtendedHeader = ExtendedTagHeaderV4.Create(descriptor.UpdateTag, descriptor.CrcDataPresent,
                                                                    descriptor.RestrictionPresent,
                                                                    descriptor.Restriction, descriptor.Crc);
            }
        }

        private static void EncodeV3(Id3TagInfo tagInfo, TagContainer container)
        {
            TagDescriptorV3 descriptor = container.GetId3V23Descriptor();

            tagInfo.Experimental = descriptor.ExperimentalIndicator;
            tagInfo.ExtendedHeaderAvailable = descriptor.ExtendedHeader;
            tagInfo.Unsynchronised = descriptor.Unsynchronisation;

            if (descriptor.ExtendedHeader)
            {
                tagInfo.ExtendedHeader = ExtendedTagHeaderV3.Create(descriptor.PaddingSize, descriptor.CrcDataPresent,
                                                                    descriptor.Crc);
            }
        }

        #endregion
    }
}