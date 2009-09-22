using System;
using System.Collections.Generic;
using System.Reflection;

namespace ID3Tag.HighLevel
{
    internal class FrameContainer : IFrameContainer
    {
        private readonly Dictionary<string, Type> m_Frames;

        internal FrameContainer()
        {
            m_Frames = new Dictionary<string, Type>();
            ScanForFrames();
        }

        #region IFrameContainer Members

        public bool Search(string id)
        {
            return m_Frames.ContainsKey(id);
        }

        public IFrame GetFrameInstance(string id)
        {
            return GetInstance(id);
        }

        public IFrame GetTextFrame()
        {
            return GetInstance("T???");
        }

        public IFrame GetUrlLinkFrame()
        {
            return GetInstance("W???");
        }

        #endregion

        private void ScanForFrames()
        {
            var assembly = Assembly.GetAssembly(typeof (FrameContainer));

            var types = assembly.GetTypes();
            foreach (var type in types)
            {
                var isFrame = IsFrame(type) && !type.IsAbstract;
                if (isFrame)
                {
                    //
                    //  Frame implementation found!
                    //
                    var frame = CreateFrame(type);
                    var id = frame.Descriptor.ID;

                    if (!m_Frames.ContainsKey(id))
                    {
                        m_Frames.Add(frame.Descriptor.ID, type);
                    }
                }
            }
        }

        private static IFrame CreateFrame(Type type)
        {
            object instance = null;
            IFrame frame = null;
            try
            {
                instance = Activator.CreateInstance(type);
                frame = instance as IFrame;
            }
            catch (Exception ex)
            {
                throw new ID3TagException("Cannot create frame : " + ex.Message, ex);
            }

            return frame;
        }

        private static bool IsFrame(Type type)
        {
            var ok = false;
            var interfaces = type.GetInterfaces();
            foreach (var curInterface in interfaces)
            {
                if (curInterface == typeof (IFrame))
                {
                    ok = true;
                    break;
                }
            }

            return ok;
        }

        private IFrame GetInstance(string id)
        {
            var found = Search(id);
            if (!found)
            {
                throw new ArgumentException("Frame not found", "id");
            }

            var frame = CreateFrame(m_Frames[id]);
            return frame;
        }
    }
}