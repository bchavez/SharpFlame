

using System.IO;


public static class PathUtil
{
    public static string EndWithPathSeperator(string text)
    {
        return Path.GetFullPath(text + Path.DirectorySeparatorChar);
    }
}