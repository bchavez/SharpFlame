using Microsoft.VisualBasic;

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
            string PathFixed = Path.ToLower().Replace('\\', '/');
            int A = 0;

            Parts = PathFixed.Split('/');
            PartCount = Parts.GetUpperBound(0) + 1;
            FilePath = "";
            for ( A = 0; A <= PartCount - 2; A++ )
            {
                FilePath += Parts[A] + "/";
            }
            FileTitle = Parts[A];
            A = Strings.InStrRev(FileTitle, ".", -1, CompareMethod.Binary);
            if ( A > 0 )
            {
                FileExtension = Strings.Right(FileTitle, FileTitle.Length - A);
                FileTitleWithoutExtension = Strings.Left(FileTitle, A - 1);
            }
            else
            {
                FileExtension = "";
                FileTitleWithoutExtension = FileTitle;
            }
        }
    }
}