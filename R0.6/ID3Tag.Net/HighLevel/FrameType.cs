namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Defines the ID3 Frame types.
    /// </summary>
    public enum FrameType
    {
        /// <summary>
        /// Unknown frame.
        /// </summary>
        Unknown,
        /// <summary>
        /// Text frame.
        /// </summary>
        Text,
        /// <summary>
        /// Userdefined frame.
        /// </summary>
        UserDefinedText,
        /// <summary>
        /// Private frame.
        /// </summary>
        Private,
        /// <summary>
        /// Music CD Identifier frame.
        /// </summary>
        MusicCDIdentifier,
        /// <summary>
        /// Comment frame.
        /// </summary>
        Comment,
        /// <summary>
        /// URL link frame.
        /// </summary>
        URLLink,
        /// <summary>
        /// Userdefinded URL link frame.
        /// </summary>
        UserDefindedURLLink,
        /// <summary>
        /// AudioEncryption Frame.
        /// </summary>
        AudioEncryption,
        /// <summary>
        /// Picture Frame
        /// </summary>
        Picture,
        /// <summary>
        /// Popularimeter Frame
        /// </summary>        
        Popularimeter,
        /// <summary>
        /// Play Counter Frame
        /// </summary>
        PlayCounter,
        /// <summary>
        /// Unique File Identifier
        /// </summary>
        UniqueFileIdentifier,
        /// <summary>
        /// Involved People list
        /// </summary>
        InvolvedPeopleList
    }
}