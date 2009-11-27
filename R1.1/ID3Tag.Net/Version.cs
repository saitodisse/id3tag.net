using System.Reflection;

namespace Id3Tag
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
    	public static string ReadableVersion
    	{
    		get
    		{
    			var assembly = Assembly.GetExecutingAssembly();
    			var assemblyName = assembly.GetName();

    			return assemblyName.Version.ToString();
    		}
    	}
    }
}