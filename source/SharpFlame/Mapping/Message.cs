#region

using System;

#endregion

namespace SharpFlame.Mapping
{
    public class Message
    {
        private readonly DateTime _CreatedDate = DateTime.Now;
        public string Text;

        public DateTime CreatedDate
        {
            get { return _CreatedDate; }
        }
    }
}