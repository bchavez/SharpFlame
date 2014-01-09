namespace SharpFlame.Mapping
{
    public class clsPathInfo
    {
        private string _Path;
        private bool _IsFMap;

        public string Path
        {
            get { return _Path; }
        }

        public bool IsFMap
        {
            get { return _IsFMap; }
        }

        public clsPathInfo(string Path, bool IsFMap)
        {
            _Path = Path;
            _IsFMap = IsFMap;
        }
    }
}