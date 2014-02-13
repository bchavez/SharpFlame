using System;
using System.IO;

namespace SharpFlame.FileIO
{
    public struct ZipSplitPath
    {
        public string[] Parts;
        public int PartCount;
        public string FilePath;
        public string FileTitle;
        public string FileTitleWithoutExtension;
        public string FileExtension;

        public ZipSplitPath(string path)
        {
            int a;
            string PathFixed = path.ToLower ().Replace ('\\', '/');

            Parts = PathFixed.Split('/');
            PartCount = Parts.Length;

            FilePath = "";
            for (a = 0; a <= PartCount - 2; a++ )
            {
                FilePath += Parts[a] + "/";
            }
            FileTitle = Parts[a];

            a = FileTitle.LastIndexOf ('.');

            if (a > 0) {
                FileTitleWithoutExtension = FileTitle.Substring (0, a);
                FileExtension = FileTitle.Substring (a + 1, FileTitle.Length - a - 1);
            } else {
                FileTitleWithoutExtension = FileTitle;
                FileExtension = "";
            }
        }
    }
}