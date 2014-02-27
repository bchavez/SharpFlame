#region

using System.IO;

#endregion

public static class PathUtil
{
    public static string EndWithPathSeperator(string text)
    {
        return Path.GetFullPath(text + Path.DirectorySeparatorChar);
    }
}