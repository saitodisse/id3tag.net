namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Encapsulates the ID3 Frames
    /// </summary>
    internal interface IFrameContainer
    {
        /// <summary>
        /// Determines whether the frame ID is available.
        /// </summary>
        /// <param name="id">the frame ID</param>
        /// <returns>true if found otherwise false</returns>
        bool Search(string id);
        /// <summary>
        /// Get the text frame instance
        /// </summary>
        /// <returns>the IFrame instance</returns>
        IFrame GetTextFrame();
        /// <summary>
        /// Get the URL link frame instance
        /// </summary>
        /// <returns>the IFrame instance</returns>
        IFrame GetUrlLinkFrame();
        /// <summary>
        /// Get the text frame instance
        /// </summary>
        /// <param name="id">the </param>
        /// <returns>the IFrame instance</returns>
        IFrame GetFrameInstance(string id);
    }
}