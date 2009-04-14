using System;
using System.Collections.Generic;
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
        public string Owner { get; set; }
        public ushort PreviewStart { get; set; }
        public ushort PreviewLength { get; set; }
        public byte[] Encryption { get; set; }

        public AudioEncryptionFrame()
            : this("Unknown",0,0,new byte[0])
        {
        }

        public AudioEncryptionFrame(string owner, ushort previewStart, ushort previewLength, byte[] encryption)
        {
            Descriptor.ID = "AENC";
            Owner = owner;
            PreviewStart = previewStart;
            PreviewLength = previewLength;
            Encryption = encryption;
        }

        /*
         * <Header for 'Audio encryption', ID: "AENC"> 
            Owner identifier        <text string> $00
            Preview start           $xx xx
            Preview length          $xx xx
            Encryption info         <binary data>
         */

        public override RawFrame Convert()
        {
            var flagBytes = Descriptor.GetFlagBytes();
            var ownerBytes = Converter.GetContentBytes(TextEncodingType.ISO_8859_1, Owner);
            var startBytes = BitConverter.GetBytes(PreviewStart);
            var lengthBytes = BitConverter.GetBytes(PreviewLength);
            
            Array.Reverse(startBytes);
            Array.Reverse(lengthBytes);

            var data = new List<byte>();
            data.AddRange(ownerBytes);
            data.Add(0);
            data.AddRange(startBytes);
            data.AddRange(lengthBytes);
            data.AddRange(Encryption);

            var dataBytes = data.ToArray();
            var payload = new byte[dataBytes.Length];
            Array.Copy(dataBytes,0,payload,0,dataBytes.Length);

            var rawFrame = RawFrame.CreateFrame("AENC", flagBytes, payload);
            return rawFrame;
        }

        public override void Import(RawFrame rawFrame)
        {
            ImportRawFrameHeader(rawFrame);
            var payloadBytes = rawFrame.Payload;

            // Read the text bytes.
            var textBytes = new List<byte>();
            var curPos = 0;
            for (curPos = 0; curPos < payloadBytes.Length; curPos++ )
            {
                var curByte = payloadBytes[curPos];
                if (curByte == 0)
                {
                    // Termination found. Abort.
                    break;
                }

                textBytes.Add(payloadBytes[curPos]);
            }

            curPos++;

            var chars = Converter.Extract(TextEncodingType.ISO_8859_1, textBytes.ToArray());
            Owner = new string(chars);

            var startBytes = new byte[2];
            var lengthBytes = new byte[2];

            Array.Copy(payloadBytes,curPos,startBytes,0,2);
            Array.Copy(payloadBytes,curPos+2,lengthBytes,0,2);
            Array.Reverse(startBytes);
            Array.Reverse(lengthBytes);

            PreviewStart = BitConverter.ToUInt16(startBytes, 0);
            PreviewLength = BitConverter.ToUInt16(lengthBytes, 0);

            var encryptionPos = curPos + 4;
            var encryptionSize = payloadBytes.Length - encryptionPos;
            Encryption = new byte[encryptionSize];
            Array.Copy(payloadBytes,encryptionPos,Encryption,0,Encryption.Length);
        }

        public override FrameType Type
        {
            get { return FrameType.AudoEncryption; }
        }

        public override string ToString()
        {
            var stringBuilder = new StringBuilder();

            var infoString = Utils.BytesToString(Encryption);

            stringBuilder.Append("AudioEncryptionFrame : ");
            stringBuilder.AppendFormat("Owner = {0} Preview Start = {1} Preview Length = {2} EncryptionInfo = {3}", Owner, PreviewStart, PreviewLength, infoString);

            return stringBuilder.ToString();
        }
    }
}