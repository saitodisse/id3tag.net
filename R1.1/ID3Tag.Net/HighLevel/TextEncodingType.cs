namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Defines the Text Encoding.
    /// </summary>
    public enum TextEncodingType
    {
        /// <summary>
        /// ISO-8859-1 encoding.
        /// </summary>
        Ansi = 0,
        /// <summary>
        /// Unicode UTF-16 encoding (little endian is default).
        /// </summary>
        Unicode = 1,
        /// <summary>
        /// Unicode UTF-16 big endian encoding.
        /// </summary>
        BigEndianUnicode = 2,
        /// <summary>
        /// Unicode UTF-8 encoding.
        /// </summary>
        Utf8 = 3
    }
}