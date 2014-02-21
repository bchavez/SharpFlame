#region

using System;

#endregion

namespace SharpFlame.Util
{
    public struct sSplitPath
    {
        public string FileExtension;
        public string FilePath;
        public string FileTitle;
        public string FileTitleWithoutExtension;
        public int PartCount;
        public string[] Parts;

        public sSplitPath(string path)
        {
            var a = 0;

            Parts = path.Split(App.PlatformPathSeparator);
            PartCount = Parts.GetUpperBound(0) + 1;
            FilePath = "";
            for ( a = 0; a <= PartCount - 2; a++ )
            {
                FilePath += Parts[a] + Convert.ToString(App.PlatformPathSeparator);
            }
            FileTitle = Parts[a];
            a = FileTitle.LastIndexOf('.');
            if ( a > 0 )
            {
                FileTitleWithoutExtension = FileTitle.Substring(0, a);
                FileExtension = FileTitle.Substring(a + 1, FileTitle.Length - a - 1);
            }
            else
            {
                FileTitleWithoutExtension = FileTitle;
                FileExtension = "";
            }
        }
    }
}