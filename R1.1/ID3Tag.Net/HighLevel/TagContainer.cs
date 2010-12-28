using System;
using System.Collections;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Globalization;

namespace Id3Tag.HighLevel
{
    /// <summary>
    /// Represents a high level ID3 tag.
    /// </summary>
    [SuppressMessage("Microsoft.Naming", "CA1710:IdentifiersShouldHaveCorrectSuffix",
        Justification = "This is not exactly a collection.")]
    public abstract class TagContainer : ICollection<IFrame>
    {
        private readonly List<IFrame> m_Frames;

        /// <summary>
        /// Creates a new TagContainer.
        /// </summary>
        protected TagContainer(TagVersion version)
        {
            m_Frames = new List<IFrame>();
            TagVersion = version;
        }

        /// <summary>
        /// The ID3v2 version
        /// </summary>
        public TagVersion TagVersion { get; private set; }

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
                    throw new ArgumentOutOfRangeException("index", index, "Value exceeds the count");
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
        /// Search for a specific frame.
        /// </summary>
        /// <param name="id">the ID of the frame.</param>
        /// <returns>the frame instance if available otherwise null.</returns>
        public IFrame SearchFrame(string id)
        {
            foreach (IFrame frame in m_Frames)
            {
                if (id.Equals(frame.Descriptor.Id))
                {
                    return frame;
                }
            }

            return null;
        }

        // TODO FIX: ABSTRACTIONS SHOULD NOT DEPEND UPON DETAILS. DETAILS SHOULD DEPEND UPON ABSTRACTIONS.
        /// <summary>
        /// Gets the version 2.3 descriptor.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Must be a method because of the exception thrown.")]
        public TagDescriptorV3 GetId3V23Descriptor()
        {
            if (TagVersion == TagVersion.Id3V23)
            {
                var v3Container = this as TagContainerV3;
                if (v3Container != null)
                {
                    return v3Container.Tag;
                }
            }

            throw new Id3TagException("Invaild tag container!");
        }

        // TODO FIX: ABSTRACTIONS SHOULD NOT DEPEND UPON DETAILS. DETAILS SHOULD DEPEND UPON ABSTRACTIONS.
        /// <summary>
        /// Gets the version 2.4 descriptor.
        /// </summary>
        /// <returns></returns>
        [SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate",
            Justification = "Must be a method because of the exception thrown.")]
        public TagDescriptorV4 GetId3V24Descriptor()
        {
            if (TagVersion == TagVersion.Id3V24)
            {
                var v3Container = this as TagContainerV4;
                if (v3Container != null)
                {
                    return v3Container.Tag;
                }
            }

            throw new Id3TagException("Invaild tag container!");
        }

        /// <summary>
        /// Overwrites the ToString.
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return string.Format(CultureInfo.InvariantCulture, "TagContainer : Count = {0}", Count);
        }
    }
}