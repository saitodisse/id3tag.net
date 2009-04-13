using ID3Tag.HighLevel.ID3Frame;

namespace ID3Tag.HighLevel
{
    public static class FrameUtils
    {
        public static UnknownFrame Convert(IFrame frame)
        {
            if (frame.Type != FrameType.Unknown)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var unknownFrame = frame as UnknownFrame;
            if (unknownFrame == null)
            {
                throw new ID3TagException("Could not cast To UnknownFrame!");
            }

            return unknownFrame;
        }

        public static TextFrame ConvertToText(IFrame frame)
        {
            if (frame.Type != FrameType.Text)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var textFrame = frame as TextFrame;
            if (textFrame == null)
            {
                throw new ID3TagException("Could not cast To TextFrame!");
            }

            return textFrame;
        }

        public static UserDefinedTextFrame ConvertToUserDefinedText(IFrame frame)
        {
            if (frame.Type != FrameType.UserDefinedText)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var textFrame = frame as UserDefinedTextFrame;
            if (textFrame == null)
            {
                throw new ID3TagException("Could not cast To UserDefinedTextFrame!");
            }

            return textFrame;
        }

        public static PrivateFrame ConvertToPrivateFrame(IFrame frame)
        {
            if (frame.Type != FrameType.Private)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var privateFrame = frame as PrivateFrame;
            if (privateFrame == null)
            {
                throw new ID3TagException("Could not cast To PrivateFrame!");
            }

            return privateFrame;
        }

        public static MusicCdIdentifierFrame ConvertToMusicCDIdentifierFrame(IFrame frame)
        {
            if (frame.Type != FrameType.MusicCDIdentifier)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var musicFrame = frame as MusicCdIdentifierFrame;
            if (musicFrame == null)
            {
                throw new ID3TagException("Could not cast To MusicCDIdentifierFrame!");
            }

            return musicFrame;
        }

        public static CommentFrame ConvertToCommentFrame(IFrame frame)
        {
            if (frame.Type != FrameType.Comment)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var commentFrame = frame as CommentFrame;
            if (commentFrame == null)
            {
                throw new ID3TagException("Could not cast To CommentFrame!");
            }

            return commentFrame;
        }

        public static UrlLinkFrame ConvertToURLLinkFrame(IFrame frame)
        {
            if (frame.Type != FrameType.URLLink)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var urlLinkFrame = frame as UrlLinkFrame;
            if (urlLinkFrame == null)
            {
                throw new ID3TagException("Could not cast To UrlLinkFrame!");
            }

            return urlLinkFrame;
        }

        public static UserDefinedURLLinkFrame ConvertToUserDefinedURLLinkFrame(IFrame frame)
        {
            if (frame.Type != FrameType.UserDefindedURLLink)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var urlLinkFrame = frame as UserDefinedURLLinkFrame;
            if (urlLinkFrame == null)
            {
                throw new ID3TagException("Could not cast To UserDefinedURLLinkFrame!");
            }

            return urlLinkFrame;
        }

        public static AudioEncryptionFrame ConvertToAudioEncryptionFrame(IFrame frame)
        {
            if (frame.Type != FrameType.AudoEncryption)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var audioEncryptionFrame = frame as AudioEncryptionFrame;
            if (audioEncryptionFrame == null)
            {
                throw new ID3TagException("Could not cast To AudioEncryptionFrame!");
            }

            return audioEncryptionFrame;
        }
    }
}