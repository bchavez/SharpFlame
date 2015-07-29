

using System.Diagnostics;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;


namespace SharpFlame.Mapping.Objects
{
    public class UnitSectorConnection
    {
        protected Link<Sector> sectorLink;
        protected Link<Unit> unitLink;

        public UnitSectorConnection()
        {
            unitLink = new Link<Unit>(this);
            sectorLink = new Link<Sector>(this);
        }

        public virtual Unit Unit => unitLink.Owner;

        public virtual Sector Sector => sectorLink.Owner;

        public static UnitSectorConnection Create(Unit unit, Sector sector)
        {
            if ( unit?.Sectors?.IsBusy == null )
            {
                return null;
            }
            if ( sector?.Units?.IsBusy == null )
            {
                return null;
            }

            var result = new UnitSectorConnection();
            result.unitLink.Connect(unit.Sectors);
            result.sectorLink.Connect(sector.Units);
            return result;
        }

        public void Deallocate()
        {
            unitLink.Deallocate();
            sectorLink.Deallocate();
        }

        protected class Link<SourceType> : ConnectedListItem<UnitSectorConnection, SourceType> where SourceType : class
        {
            public Link(UnitSectorConnection item) : base(item)
            {
            }

            public override void OnRemoved()
            {
                base.OnRemoved();
                Item.Deallocate();
            }
        }
    }
}