using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ID3Tag.HighLevel;

namespace ID3Tag.LowLevel
{
    internal abstract class Validator
    {
        public string FailureDescription { get; set; }
        public abstract bool Validate(TagContainer container);
    }
}
