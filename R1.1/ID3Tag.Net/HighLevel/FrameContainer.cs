using System;
using System.Collections.Generic;
using System.Reflection;

namespace Id3Tag.HighLevel
{
    internal class FrameContainer : IFrameCreationService
    {
        private readonly Dictionary<string, Type> m_Frames;

        internal FrameContainer()
        {
            m_Frames = new Dictionary<string, Type>();
            ScanForFrames();
        }

        #region IFrameCreationService Members

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
            Assembly assembly = Assembly.GetAssembly(typeof (FrameContainer));

            Type[] types = assembly.GetTypes();
            foreach (Type type in types)
            {
                bool isFrame = IsFrame(type) && !type.IsAbstract;
                if (isFrame)
                {
                    //
                    //  Frame implementation found!
                    //
                    IFrame frame = CreateFrame(type);
                    string id = frame.Descriptor.Id;

                    if (!m_Frames.ContainsKey(id))
                    {
                        m_Frames.Add(frame.Descriptor.Id, type);
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
                Logger.LogError(ex);
                throw new Id3TagException("Cannot create frame : " + ex.Message, ex);
            }

            return frame;
        }

        private static bool IsFrame(Type type)
        {
            bool ok = false;
            Type[] interfaces = type.GetInterfaces();
            foreach (Type curInterface in interfaces)
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
            bool found = Search(id);
            if (!found)
            {
                var ex = new ArgumentException("Frame not found", "id");
                Logger.LogError(ex);

                throw ex;
            }

            IFrame frame = CreateFrame(m_Frames[id]);
            return frame;
        }
    }
}