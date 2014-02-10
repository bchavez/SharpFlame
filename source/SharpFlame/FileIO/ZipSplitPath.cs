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

        public ZipSplitPath(string Path)
        {
            int A;
            string PathFixed = Path.ToLower ().Replace ('\\', '/');

            Parts = PathFixed.Split('/');
            PartCount = Parts.Length;

            FilePath = "";
            for (A = 0; A <= PartCount - 2; A++ )
            {
                FilePath += Parts[A] + "/";
            }
            FileTitle = Parts[A];

            A = FileTitle.LastIndexOf ('.');

            if (A > 0) {
                FileTitleWithoutExtension = FileTitle.Substring (0, A);
                FileExtension = FileTitle.Substring (A + 1, FileTitle.Length - A - 1);
            } else {
                FileTitleWithoutExtension = FileTitle;
                FileExtension = "";
            }
        }
    }
}