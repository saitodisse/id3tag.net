﻿using ID3Tag.HighLevel.ID3Frame;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel
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
                    throw new ID3TagException("This major revision is not supported!");
            }

            //
            //  Import the frames
            //
            IFrameCreationService creationService = new FrameContainer();
            foreach (var rawFrame in info.Frames)
            {
                //
                //  Analyse the frame ID
                //
                var frame = AnalyseFrameId(rawFrame, creationService);
                if (frame != null)
                {
                    frame.Import(rawFrame, codePage);
                    container.Add(frame);
                }
                else
                {
                    throw new ID3TagException("Frame analysing failed!");
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
                    throw new ID3TagException("Unknown version!");
            }

            foreach (var frame in container)
            {
                var rawFrame = frame.Convert(container.TagVersion);
                tagInfo.Frames.Add(rawFrame);
            }

            return tagInfo;
        }

        #endregion

        #region Private Helper

        private static IFrame AnalyseFrameId(RawFrame rawFrame, IFrameCreationService frameService)
        {
            var id = rawFrame.ID;
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
            var descriptor = container.Tag;

            descriptor.SetHeaderFlags(info.UnsynchronisationFlag, info.ExtendedHeaderAvailable, info.Experimental,
                                      info.FooterFlag);
            if (info.ExtendedHeaderAvailable)
            {
                var extendedHeader = info.ExtendedHeader.ConvertToV24();
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
            var descriptor = container.Tag;

            descriptor.SetHeaderFlags(info.UnsynchronisationFlag, info.ExtendedHeaderAvailable, info.Experimental);

            if (info.ExtendedHeaderAvailable)
            {
                var extendedHeader = info.ExtendedHeader.ConvertToV23();
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
            var descriptor = container.GetId3V24Descriptor();

            tagInfo.Experimental = descriptor.ExperimentalIndicator;
            tagInfo.ExtendedHeaderAvailable = descriptor.ExtendedHeader;
            tagInfo.UnsynchronisationFlag = descriptor.Unsynchronisation;
            tagInfo.FooterFlag = descriptor.Footer;

            if (descriptor.ExtendedHeader)
            {
                tagInfo.ExtendedHeader = ExtendedTagHeaderV4.Create(descriptor.UpdateTag, descriptor.CrcDataPresent,
                                                                    descriptor.RestrictionPresent,
                                                                    descriptor.Restriction, descriptor.Crc);
            }
        }

        private static void EncodeV3(Id3TagInfo tagInfo, TagContainer container)
        {
            var descriptor = container.GetId3V23Descriptor();

            tagInfo.Experimental = descriptor.ExperimentalIndicator;
            tagInfo.ExtendedHeaderAvailable = descriptor.ExtendedHeader;
            tagInfo.UnsynchronisationFlag = descriptor.Unsynchronisation;

            if (descriptor.ExtendedHeader)
            {
                tagInfo.ExtendedHeader = ExtendedTagHeaderV3.Create(descriptor.PaddingSize, descriptor.CrcDataPresent,
                                                                    descriptor.Crc);
            }
        }

        #endregion
    }
}