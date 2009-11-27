using Id3Tag.HighLevel;

namespace Id3Tag.LowLevel
{
    internal abstract class Validator
    {
        public string FailureDescription { get; set; }
        public abstract bool Validate(TagContainer container);
    }
}