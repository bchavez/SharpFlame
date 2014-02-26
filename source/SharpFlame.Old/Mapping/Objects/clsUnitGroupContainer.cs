#region

using System;

#endregion

namespace SharpFlame.Old.Mapping.Objects
{
    public class clsUnitGroupContainer
    {
        public delegate void ChangedEventHandler();

        private ChangedEventHandler ChangedEvent;
        private clsUnitGroup _Item;

        public clsUnitGroup Item
        {
            get { return _Item; }
            set
            {
                if ( value == _Item )
                {
                    return;
                }
                _Item = value;
                if ( ChangedEvent != null )
                    ChangedEvent();
            }
        }

        public event ChangedEventHandler Changed
        {
            add { ChangedEvent = (ChangedEventHandler)Delegate.Combine(ChangedEvent, value); }
            remove { ChangedEvent = (ChangedEventHandler)Delegate.Remove(ChangedEvent, value); }
        }
    }
}