namespace SharpFlame.Mapping
{
    public class clsPathInfo
    {
        private readonly bool _IsFMap;
        private readonly string _Path;

        public clsPathInfo(string Path, bool IsFMap)
        {
            _Path = Path;
            _IsFMap = IsFMap;
        }

        public string Path
        {
            get { return _Path; }
        }

        public bool IsFMap
        {
            get { return _IsFMap; }
        }
    }
}