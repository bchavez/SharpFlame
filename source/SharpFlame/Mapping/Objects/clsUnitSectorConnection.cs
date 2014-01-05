using SharpFlame.Collections;

namespace SharpFlame.Mapping.Objects
{
    public class clsUnitSectorConnection
    {
        public clsUnitSectorConnection()
        {
            _UnitLink = new Link<clsUnit>(this);
            _SectorLink = new Link<clsMap.clsSector>(this);
        }

        protected class Link<SourceType> : ConnectedListLink<clsUnitSectorConnection, SourceType> where SourceType : class
        {
            public Link(clsUnitSectorConnection Owner) : base(Owner)
            {
            }

            public override void AfterRemove()
            {
                base.AfterRemove();

                Item.Deallocate();
            }
        }

        protected Link<clsUnit> _UnitLink;
        protected Link<clsMap.clsSector> _SectorLink;

        public virtual clsUnit Unit
        {
            get { return _UnitLink.Source; }
        }

        public virtual clsMap.clsSector Sector
        {
            get { return _SectorLink.Source; }
        }

        public static clsUnitSectorConnection Create(clsUnit Unit, clsMap.clsSector Sector)
        {
            if ( Unit == null )
            {
                return null;
            }
            if ( Unit.Sectors == null )
            {
                return null;
            }
            if ( Unit.Sectors.IsBusy )
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
            if ( Sector.Units.IsBusy )
            {
                return null;
            }

            clsUnitSectorConnection Result = new clsUnitSectorConnection();
            Result._UnitLink.Connect(Unit.Sectors);
            Result._SectorLink.Connect(Sector.Units);
            return Result;
        }

        public void Deallocate()
        {
            _UnitLink.Deallocate();
            _SectorLink.Deallocate();
        }
    }
}