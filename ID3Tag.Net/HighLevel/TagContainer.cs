using System.Collections;
using System.Collections.Generic;

namespace ID3Tag.HighLevel
{
    public class TagContainer : ICollection<IFrame>
    {
        private readonly List<IFrame> m_Frames;

        public TagContainer()
        {
            m_Frames = new List<IFrame>();
            Tag = new TagDescriptor();
        }

        public TagDescriptor Tag { get; private set; }

        public IFrame this[int index]
        {
            get
            {
                if (index > m_Frames.Count)
                {
                    throw new ID3TagException("Index is not available");
                }

                return m_Frames[index];
            }
        }

        #region ICollection<IFrame> Members

        public void Add(IFrame item)
        {
            m_Frames.Add(item);
        }

        public void Clear()
        {
            m_Frames.Clear();
        }

        public bool Contains(IFrame item)
        {
            return m_Frames.Contains(item);
        }

        public void CopyTo(IFrame[] array, int arrayIndex)
        {
            m_Frames.CopyTo(array, arrayIndex);
        }

        public int Count
        {
            get { return m_Frames.Count; }
        }

        public bool IsReadOnly
        {
            get { return false; }
        }

        public bool Remove(IFrame item)
        {
            return m_Frames.Remove(item);
        }

        public IEnumerator<IFrame> GetEnumerator()
        {
            return m_Frames.GetEnumerator();
        }

        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Frames.GetEnumerator();
        }

        #endregion

        public override string ToString()
        {
            return string.Format("TagContainer : Count = {0}", Count);
        }
    }
}