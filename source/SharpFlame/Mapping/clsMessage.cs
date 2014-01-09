using System;

namespace SharpFlame.Mapping
{
    public class clsMessage
    {
        public string Text;
        private DateTime _CreatedDate = DateTime.Now;

        public DateTime CreatedDate
        {
            get { return _CreatedDate; }
        }
    }
}