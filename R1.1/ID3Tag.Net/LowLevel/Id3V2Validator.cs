using System;
using System.Globalization;
using System.Text;
using Id3Tag.HighLevel;

namespace Id3Tag.LowLevel
{
    internal class Id3V2Validator : Validator
    {
        public override bool Validate(TagContainer container)
        {
            if (container.TagVersion == TagVersion.Id3V23)
            {
                bool valid = true;

                foreach (IFrame frame in container)
                {
                    //
                    //  If the frame is a type of EncodedTextFrame then validate the coding
                    //
                    var textFrame = frame as EncodedTextFrame;
                    if (textFrame != null)
                    {
                        valid = ValidateTextEncoding(textFrame.TextEncoding);
                    }

                    if (!valid)
                    {
                        //
                        //  Abort. Invalid frame found!
                        //
                        FailureDescription = String.Format(
                            CultureInfo.InvariantCulture, "Frame {0} uses an invalid text coding.", frame.Descriptor.Id);
                        break;
                    }
                }

                return valid;
            }

            FailureDescription = "The ID3 header version does not match!";
            return false;
        }

        private static bool ValidateTextEncoding(Encoding encoding)
        {
            // ID3 v2.3 only supports single-byte ANSI encoding or unicode
            // UTF16 BE (12001) and UTF8 (65001) are not supported
            return !(encoding.CodePage == 65001 || encoding.CodePage == 1201);
        }
    }
}