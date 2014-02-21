#region

using SharpFlame.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.Mapping.Objects
{
    public class clsUnitSectorConnection
    {
        protected Link<Sector> _SectorLink;
        protected Link<Unit> _UnitLink;

        public clsUnitSectorConnection()
        {
            _UnitLink = new Link<Unit>(this);
            _SectorLink = new Link<Sector>(this);
        }

        public virtual Unit Unit
        {
            get { return _UnitLink.Source; }
        }

        public virtual Sector Sector
        {
            get { return _SectorLink.Source; }
        }

        public static clsUnitSectorConnection Create(Unit Unit, Sector Sector)
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