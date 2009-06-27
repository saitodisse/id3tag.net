using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    internal class Id3V2Validator : Validator
    {
        public override bool Validate(TagContainer container)
        {
            if (container.Tag.MajorVersion == 3 && container.Tag.Revision == 0)
            {
                var valid = true;

                foreach (var frame in container)
                {
                    /*
                     *  T___
                     *  TXXX
                     *  WXXX
                     *  APIC
                     * 
                     *  fehlt noch:
                     *  IPLS
                     *  USLT
                     *  SYLT
                     *  COMM
                     *  GEOB
                     *  USER
                     *  OWNE
                     *  COMR
                     *  
                     */

                    switch (frame.Type)
                    {
                        case FrameType.Text:
                            valid = ValidateTextFrame(frame);
                            break;
                        case FrameType.UserDefinedText:
                            valid = ValidateUserDefinedTextFrame(frame);
                            break;
                        case FrameType.UserDefindedURLLink:
                            valid = ValidateUserDefinedLink(frame);
                            break;
                        case FrameType.Picture:
                            valid = ValidatePictureFrame(frame);
                            break;
                    }

                    if (!valid)
                    {
                        //
                        //  Abort. Invalid frame found!
                        //
                        FailureDescription = String.Format("Frame {0} is not valid! (must be 3.0)", frame.Descriptor.ID);
                        break;
                    }
                }

                return valid;
            }
            else
            {
                FailureDescription = "The ID3 header version does not match!";
                return false;
            }
        }

        private static bool ValidatePictureFrame(IFrame frame)
        {
            var pictureFrame = FrameUtils.ConvertToPictureFrame(frame);

            var ok = ValidateTextEncoding(pictureFrame.TextEncoding);
            return ok;
        }

        private static bool ValidateUserDefinedLink(IFrame frame)
        {
            var urlLinkFrame = FrameUtils.ConvertToUserDefinedURLLinkFrame(frame);

            var ok = ValidateTextEncoding(urlLinkFrame.TextEncoding);
            return ok;
        }

        private static bool ValidateUserDefinedTextFrame(IFrame frame)
        {
            var textFrame = FrameUtils.ConvertToUserDefinedText(frame);

            var ok = ValidateTextEncoding(textFrame.TextEncoding);
            return ok;
        }

        private static bool ValidateTextFrame(IFrame frame)
        {
            var textFrame = FrameUtils.ConvertToText(frame);

            var ok = ValidateTextEncoding(textFrame.TextEncoding);
            return ok;
        }

        private static bool ValidateTextEncoding(TextEncodingType type)
        {
            bool result;
            if (type == TextEncodingType.ISO_8859_1 || type == TextEncodingType.UTF16)
            {
                result = true;
            }
            else
            {
                // UTF_BE and UTF8 is not supported in ID3v2.3!
                result = false;
            }

            return result;
        }
    }
}
