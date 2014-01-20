using System;
using Microsoft.VisualBasic;

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
            A = Strings.InStrRev( FileTitle, ".", -1, (CompareMethod)0 );
            if( A > 0 )
            {
                FileExtension = Strings.Right( FileTitle, FileTitle.Length - A );
                FileTitleWithoutExtension = Strings.Left( FileTitle, A - 1 );
            }
            else
            {
                FileExtension = "";
                FileTitleWithoutExtension = FileTitle;
            }
        }
    }
}