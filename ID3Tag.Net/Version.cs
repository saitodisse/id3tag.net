using System.Reflection;

namespace ID3Tag
{
    public static class Version
    {
        public static string GetReadableVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();

            return assemblyName.Version.ToString();
        }
    }
}