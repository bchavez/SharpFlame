using System;

namespace SharpFlame.Util
{
    public struct sSplitPath
    {
        public string[] Parts;
        public int PartCount;
        public string FilePath;
        public string FileTitle;
        public string FileTitleWithoutExtension;
        public string FileExtension;

        public sSplitPath( string Path )
        {
            int A = 0;

            Parts = Path.Split( App.PlatformPathSeparator );
            PartCount = Parts.GetUpperBound( 0 ) + 1;
            FilePath = "";
            for( A = 0; A <= PartCount - 2; A++ )
            {
                FilePath += Parts[A] + Convert.ToString( App.PlatformPathSeparator );
            }
            FileTitle = Parts[A];
            A = FileTitle.IndexOf ('.');

            FileTitleWithoutExtension = "";
            FileExtension = "";
            if (A > 0) {
                FileTitleWithoutExtension = FileTitle.Substring (0, A);
                FileExtension = FileTitle.Substring (A + 1, FileTitle.Length - A - 1);
            }
        }
    }
}