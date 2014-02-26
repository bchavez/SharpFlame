#region

using System;

#endregion

namespace SharpFlame.Old.Mapping
{
    public class AutoSave
    {
        public int ChangeCount;
        public DateTime SavedDate;

        public AutoSave()
        {
            SavedDate = DateTime.Now;
        }
    }
}