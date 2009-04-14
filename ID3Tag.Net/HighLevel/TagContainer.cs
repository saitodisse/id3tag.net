using System.Collections;
using System.Collections.Generic;

namespace ID3Tag.HighLevel
{
    /// <summary>
    /// Represents a high level ID3 tag.
    /// </summary>
    public class TagContainer : ICollection<IFrame>
    {
        private readonly List<IFrame> m_Frames;

        /// <summary>
        /// Creates a new TagContainer.
        /// </summary>
        public TagContainer()
        {
            m_Frames = new List<IFrame>();
            Tag = new TagDescriptor();
        }

        /// <summary>
        /// The tag descriptor.
        /// </summary>
        public TagDescriptor Tag { get; private set; }

        /// <summary>
        /// Get a specific IFrame from the container.
        /// </summary>
        /// <param name="index">the index.</param>
        /// <returns>the IFrame instance.</returns>
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

        /// <summary>
        /// Add a new frame.
        /// </summary>
        /// <param name="item"></param>
        public void Add(IFrame item)
        {
            m_Frames.Add(item);
        }

        /// <summary>
        /// Clear the collection.
        /// </summary>
        public void Clear()
        {
            m_Frames.Clear();
        }

        /// <summary>
        /// Search for a specific frame.
        /// </summary>
        /// <param name="item">the frame.</param>
        /// <returns>True if the items exists otherwise false.</returns>
        public bool Contains(IFrame item)
        {
            return m_Frames.Contains(item);
        }

        /// <summary>
        /// Add a number of frames to the list.
        /// </summary>
        /// <param name="array">the frames.</param>
        /// <param name="arrayIndex">the array index.</param>
        public void CopyTo(IFrame[] array, int arrayIndex)
        {
            m_Frames.CopyTo(array, arrayIndex);
        }

        /// <summary>
        /// The number of frames.
        /// </summary>
        public int Count
        {
            get { return m_Frames.Count; }
        }

        /// <summary>
        /// Determine whether this collection is read only.
        /// </summary>
        public bool IsReadOnly
        {
            get { return false; }
        }

        /// <summary>
        /// Remove an item.
        /// </summary>
        /// <param name="item">the item.</param>
        /// <returns>true if the item can be removed otherwise false.</returns>
        public bool Remove(IFrame item)
        {
            return m_Frames.Remove(item);
        }

        /// <summary>
        /// Gets the enumerator of the frames.
        /// </summary>
        /// <returns>the enumerator.</returns>
        public IEnumerator<IFrame> GetEnumerator()
        {
            return m_Frames.GetEnumerator();
        }

        /// <summary>
        /// Gets the enumerator of the frames.
        /// </summary>
        /// <returns>the enumerator.</returns>
        IEnumerator IEnumerable.GetEnumerator()
        {
            return m_Frames.GetEnumerator();
        }

        #endregion

        /// <summary>
        /// Overwrites the ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format("TagContainer : Count = {0}", Count);
        }
    }
}