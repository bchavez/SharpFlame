namespace FlaME
{
    using System;

    public class clsResultItemTileGoto : clsResultItemGotoInterface
    {
        public modMath.sXY_int TileNum;
        public clsViewInfo View;

        public override void Perform()
        {
            this.View.LookAtTile(this.TileNum);
        }
    }
}

