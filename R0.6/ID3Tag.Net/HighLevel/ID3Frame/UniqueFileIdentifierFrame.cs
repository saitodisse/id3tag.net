using System;
using System.Collections.Generic;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
    /// <summary>
    /// This frame's purpose is to be able to identify the audio file in a database that may contain more information relevant to the content. 
    /// Since standardisation of such a database is beyond this document, all frames begin with a null-terminated string with a 
    /// URL containing an email address, or a link to a location where an email address can be found, 
    /// that belongs to the organisation responsible for this specific database implementation. 
    /// Questions regarding the database should be sent to the indicated email address. The URL should not be used for the actual database queries.
    ///  The string "http://www.id3.org/dummy/ufid.html" should be used for tests. 
    /// Software that isn't told otherwise may safely remove such frames. The 'Owner identifier' must be non-empty (more than just a termination). 
    /// The 'Owner identifier' is then followed by the actual identifier, which may be up to 64 bytes. 
    /// There may be more than one "UFID" frame in a tag, but only one with the same 'Owner identifier'.
    /// </summary>
    public class UniqueFileIdentifierFrame : Frame
    {
        /// <summary>
        /// Creates a new instance of UniqueFileIdentifierFrame
        /// </summary>
        public UniqueFileIdentifierFrame()
            : this(String.Empty, new byte[0])
        {
        }

        /// <summary>
        /// Creates a new instance of UniqueFileIdentifierFrame
        /// </summary>
        /// <param name="owner">the owner</param>
        /// <param name="identifier">the identifier</param>
        public UniqueFileIdentifierFrame(string owner, byte[] identifier)
        {
            Descriptor.ID = "UFID";
            Owner = owner;
            Identifier = identifier;
        }

        /// <summary>
        /// The Owner Identifier
        /// </summary>
        public string Owner { get; set; }

        /// <summary>
        /// The Identifier
        /// </summary>
        public byte[] Identifier { get; set; }
        /// <summary>
        /// Defines the frame type.
        /// </summary>
        public override FrameType Type
        {
            get { return FrameType.UniqueFileIdentifier; }
        }

        /// <summary>
        /// Convert the frame to raw frame.
        /// </summary>
        /// <returns>the raw frame</returns>
        public override RawFrame Convert()
        {
            /*
             *  <Header for 'Unique file identifier', ID: "UFID"> 
                Owner identifier    <text string> $00
                Identifier    <up to 64 bytes binary data>

             */

            var flag = Descriptor.GetFlags();
            var owner = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, Owner);

            var payload = new List<byte>();
            payload.AddRange(owner);
            payload.Add(0x00);
            payload.AddRange(Identifier);

            var rawFrame = RawFrame.CreateV3Frame(Descriptor.ID, flag, payload.ToArray());
            return rawFrame;
        }

        /// <summary>
        /// Import a raw frame.
        /// </summary>
        /// <param name="rawFrame">the raw frame</param>
        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);
            var payload = rawFrame.Payload;

            var ownerBytes = new List<byte>();
            var identifierBytes = new List<byte>();

            var zeroByteFound = false;
            foreach (var curByte in payload)
            {
                if (curByte == 0x00)
                {
                    zeroByteFound = true;
                }
                else
                {
                    if (zeroByteFound)
                    {
                        identifierBytes.Add(curByte);
                    }
                    else
                    {
                        ownerBytes.Add(curByte);
                    }
                }
            }

            var ownerChars = Converter.Extract(TextEncodingType.ISO_8859_1, ownerBytes.ToArray());
            Owner = new string(ownerChars);
            Identifier = identifierBytes.ToArray();
        }

        /// <summary>
        /// Returns the representative ToString
        /// </summary>
        /// <returns>the string</returns>
        public override string ToString()
        {
            var bytes = Utils.BytesToString(Identifier);
            return String.Format("UniqueFileIdentifierFrame : Owner = {0}, Identifier = {1}", bytes);
        }
    }
}