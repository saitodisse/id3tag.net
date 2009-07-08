using System.Reflection;

namespace ID3Tag
{
    /// <summary>
    /// Returns the current version of this library.
    /// </summary>
    public static class Version
    {
        /// <summary>
        /// Gets the current assemly version.
        /// </summary>
        /// <returns>a readable string.</returns>
        public static string GetReadableVersion()
        {
            var assembly = Assembly.GetExecutingAssembly();
            var assemblyName = assembly.GetName();

            return assemblyName.Version.ToString();
        }
    }
}