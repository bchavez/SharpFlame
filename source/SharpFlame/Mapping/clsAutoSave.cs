#region

using System;

#endregion

namespace SharpFlame.Mapping
{
    public class clsAutoSave
    {
        public int ChangeCount;
        public DateTime SavedDate;

        public clsAutoSave()
        {
            SavedDate = DateTime.Now;
        }
    }
}