using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    internal class Id3V24Validator : Validator
    {
        public override bool Validate(TagContainer container)
        {
            if (container.TagVersion != TagVersion.Id3V24)
            {
                FailureDescription = "No ID3v2.4 format.";

                return false;
            }

            return true;
        }


    }
}
