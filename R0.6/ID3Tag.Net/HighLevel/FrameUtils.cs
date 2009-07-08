using ID3Tag.HighLevel.ID3Frame;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Provides some static helper methods.
    /// </summary>
    public static class FrameUtils
    {
        /// <summary>
        /// Convert a abstract frame to an UnknownFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an TextFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an UserDefinedTextFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an PrivateFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an MusicCdIdentifierFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an CommentFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an UrlLinkFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an UserDefinedURLLinkFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to an AudioEncryptionFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrecte type.</returns>
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

        /// <summary>
        /// Convert a abstract frame to a PictureFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrete type.</returns>
        public static PictureFrame ConvertToPictureFrame(IFrame frame)
        {
            if (frame.Type != FrameType.Picture)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var pictureFrame = frame as PictureFrame;
            if (pictureFrame == null)
            {
                throw new ID3TagException("Could not cast To PictureFrame!");
            }

            return pictureFrame;
        }

        /// <summary>
        /// Convert a abstract frame to a PlayCounterFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrete type.</returns>
        public static PlayCounterFrame ConvertToPlayCounterFrame(IFrame frame)
        {
            if (frame.Type != FrameType.PlayCounter)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var playCounterFrame = frame as PlayCounterFrame;
            if (playCounterFrame == null)
            {
                throw new ID3TagException("Could not cast To PlayCounterFrame!");
            }

            return playCounterFrame;
        }

        /// <summary>
        /// Convert a abstract frame to a PopularimeterFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrete type.</returns>
        public static PopularimeterFrame ConvertToPopularimeterFrame(IFrame frame)
        {
            if (frame.Type != FrameType.Popularimeter)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var popm = frame as PopularimeterFrame;
            if (popm == null)
            {
                throw new ID3TagException("Could not cast To PopularimeterFrame!");
            }

            return popm;
        }

        /// <summary>
        /// Convert a abstract frame to a UniqueFileIdentifierFrame.
        /// </summary>
        /// <param name="frame">the abstract frame.</param>
        /// <returns>the concrete type.</returns>
        public static UniqueFileIdentifierFrame ConvertToUniqueIdentifierFrame(IFrame frame)
        {
            if (frame.Type != FrameType.UniqueFileIdentifier)
            {
                throw new ID3TagException("Frame Type does not mathch");
            }

            var ufid = frame as UniqueFileIdentifierFrame;
            if (ufid == null)
            {
                throw new ID3TagException("Could not cast To UniqueFileIdentifierFrame!");
            }

            return ufid;
        }
    }
}