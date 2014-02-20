#region

using SharpFlame.Collections;

#endregion

namespace SharpFlame.Mapping.Objects
{
    public class clsUnitSectorConnection
    {
        protected Link<clsSector> _SectorLink;
        protected Link<clsUnit> _UnitLink;

        public clsUnitSectorConnection()
        {
            _UnitLink = new Link<clsUnit>(this);
            _SectorLink = new Link<clsSector>(this);
        }

        public virtual clsUnit Unit
        {
            get { return _UnitLink.Source; }
        }

        public virtual clsSector Sector
        {
            get { return _SectorLink.Source; }
        }

        public static clsUnitSectorConnection Create(clsUnit Unit, clsSector Sector)
        {
            if ( Unit == null )
            {
                return null;
            }
            if ( Unit.Sectors == null )
            {
                return null;
            }
            if ( Sector == null )
            {
                return null;
            }
            if ( Sector.Units == null )
            {
                return null;
            }

            var Result = new clsUnitSectorConnection();
            Result._UnitLink.Connect(Unit.Sectors);
            Result._SectorLink.Connect(Sector.Units);
            return Result;
        }

        public void Deallocate()
        {
            _UnitLink.Deallocate();
            _SectorLink.Deallocate();
        }

        protected class Link<SourceType> : ConnectedListLink<clsUnitSectorConnection, SourceType> where SourceType : class
        {
            public Link(clsUnitSectorConnection Owner) : base(Owner)
            {
            }

            public override void AfterRemove()
            {              
                base.AfterRemove();

                if (Item != null) {
                    Item.Deallocate ();
                }
            }
        }
    }
}