namespace SharpFlame.FileIO
{
    public struct ZipSplitPath
    {
        public string FileExtension;
        public string FilePath;
        public string FileTitle;
        public string FileTitleWithoutExtension;
        public int PartCount;
        public string[] Parts;

        public ZipSplitPath(string path)
        {
            int a;
            var PathilhFixed = path.ToLower().Replace('\\', '/');

            Parts = PathFixed.Split('/');
            PartCount = Parts.Length;

            FilePath = "";
            for ( a = 0; a <= PartCount - 2; a++ )
            {
                FilePath += Parts[a] + "/";
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