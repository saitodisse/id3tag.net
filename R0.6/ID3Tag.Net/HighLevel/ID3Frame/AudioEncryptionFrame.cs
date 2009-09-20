using System;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// This frame indicates if the actual audio stream is encrypted, and by whom. Since standardisation 
    /// of such encrypion scheme is beyond this document, all "AENC" frames begin with a terminated string 
    /// with a URL containing an email address, or a link to a location where an email address can be found, 
    /// that belongs to the organisation responsible for this specific encrypted audio file. 
    /// Questions regarding the encrypted audio should be sent to the email address specified. 
    /// If a $00 is found directly after the 'Frame size' and the audiofile indeed is encrypted, 
    /// the whole file may be considered useless.
    /// After the 'Owner identifier', a pointer to an unencrypted part of the audio can be specified. 
    /// The 'Preview start' and 'Preview length' is described in frames. If no part is unencrypted, 
    /// these fields should be left zeroed. After the 'preview length' field follows optionally a 
    /// datablock required for decryption of the audio. There may be more than one "AENC" frames in a tag, 
    /// but only one with the same 'Owner identifier'. 
    /// </summary>
    public class AudioEncryptionFrame : Frame
    {
        /// <summary>
        /// Creates a new instance of AudioEncryptionFrame.
        /// </summary>
        public AudioEncryptionFrame()
            : this("Unknown", 0, 0, new byte[0])
        {
        }

        /// <summary>
        /// Creates a new instance of AudioEncryptionFrame.
        /// </summary>
        /// <param name="owner">the owner.</param>
        /// <param name="previewStart">the preview start.</param>
        /// <param name="previewLength">the preview length.</param>
        /// <param name="encryption">the encryption bytes</param>
        public AudioEncryptionFrame(string owner, ushort previewStart, ushort previewLength, byte[] encryption)
        {
            Descriptor.ID = "AENC";
            Owner = owner;
            PreviewStart = previewStart;
            PreviewLength = previewLength;
            Encryption = encryption;
        }

        /// <summary>
        /// The owner.
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The preview start value.
        /// </summary>
        public ushort PreviewStart { get; set; }

        /// <summary>
        /// The preview length value.
        /// </summary>
        public ushort PreviewLength { get; set; }

        /// <summary>
        /// The encryption data.
        /// </summary>
        public byte[] Encryption { get; set; }

        /// <summary>
        /// The frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.AudioEncryption; }
        }

        /*
         * <Header for 'Audio encryption', ID: "AENC"> 
            Owner identifier        <text string> $00
            Preview start           $xx xx
            Preview length          $xx xx
            Encryption info         <binary data>
         */

        /// <summary>
        /// Convert the values to a raw frame.
        /// </summary>
        /// <returns>the raw frame.</returns>
        public override RawFrame Convert(TagVersion version)
        {
            var flag = Descriptor.GetFlags();

			byte[] payload;
			using (var writer = new FrameDataWriter())
			{
				writer.WriteString(Owner, Encoding.GetEncoding(28591), true);
				writer.WriteUInt16(PreviewStart);
				writer.WriteUInt16(PreviewLength);
				writer.WriteBytes(Encryption);
				payload = writer.ToArray();
			}

            return RawFrame.CreateFrame("AENC", flag, payload, version);
        }

		/// <summary>
		/// Import the raw frame.
		/// </summary>
		/// <param name="rawFrame">the raw frame.</param>
		/// <param name="codePage">Default code page for Ansi encoding. Pass 0 to use default system encoding code page.</param>
        public override void Import(RawFrame rawFrame, int codePage)
        {
            ImportRawFrameHeader(rawFrame);

			using (var reader = new FrameDataReader(rawFrame.Payload))
			{
				Owner = reader.ReadVariableString(Encoding.GetEncoding(28591));
				PreviewStart = reader.ReadUInt16();
				PreviewLength = reader.ReadUInt16();
				Encryption = reader.ReadBytes();
			}
        }

        /// <summary>
        /// Overwrite ToString
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
        	return
        		String.Format(
        			"Audio Encryption : Owner = {0}, Preview Start = {1}, Preview Length = {2}, EncryptionInfo = {3}",
        			Owner,
        			PreviewStart,
        			PreviewLength,
        			Utils.BytesToString(Encryption));
        }
    }
}