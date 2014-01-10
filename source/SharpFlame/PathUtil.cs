using System.IO;

static internal class PathUtil
{
    public static string EndWithPathSeperator(string text)
    {
        return Path.GetFullPath(text + Path.DirectorySeparatorChar);
    }
}