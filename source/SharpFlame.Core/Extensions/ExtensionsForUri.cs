
using System;

namespace SharpFlame.Core.Extensions
{
    public static class ExtensionsForUri
    {
        public static Uri MakeRelativeUriToBinPath(this Uri fullpath)
        {
            var relRoot = new Uri(AppDomain.CurrentDomain.BaseDirectory, UriKind.Absolute);
            return relRoot.MakeRelativeUri(fullpath);
        }
    }
}

