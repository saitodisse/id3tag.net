using System;
using System.Collections.Generic;
using System.Text;
using ID3Tag.LowLevel;

namespace ID3Tag.HighLevel.ID3Frame
{
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