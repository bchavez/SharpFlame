namespace SharpFlame.Old.Mapping.Tiles
{
    public struct TileDirection
    {
        public byte X; //0-2, 1=middle
        public byte Y; //0-2, 1=middle

        public TileDirection(byte NewX, byte NewY)
        {
            X = NewX;
            Y = NewY;
        }

        public TileDirection GetRotated(TileOrientation Orientation)
        {
            var returnResult = new TileDirection();

            if ( Orientation.SwitchedAxes )
            {
                if ( Orientation.ResultXFlip )
                {
                    returnResult.X = (byte)(2 - Y);
                }
                else
                {
                    returnResult.X = Y;
                }
                if ( Orientation.ResultYFlip )
                {
                    returnResult.Y = (byte)(2 - X);
                }
                else
                {
                    returnResult.Y = X;
                }
            }
            else
            {
                if ( Orientation.ResultXFlip )
                {
                    returnResult.X = (byte)(2 - X);
                }
                else
                {
                    returnResult.X = X;
                }
                if ( Orientation.ResultYFlip )
                {
                    returnResult.Y = (byte)(2 - Y);
                }
                else
                {
                    returnResult.Y = Y;
                }
            }

            return returnResult;
        }

        public void FlipX()
        {
            X = (byte)(2 - X);
        }

        public void FlipY()
        {
            Y = (byte)(2 - Y);
        }

        public void RotateClockwise()
        {
            byte byteTemp = 0;

            byteTemp = X;
            X = (byte)(2 - Y);
            Y = byteTemp;
        }

        public void RotateAnticlockwise()
        {
            byte byteTemp = 0;

            byteTemp = X;
            X = Y;
            Y = (byte)(2 - byteTemp);
        }

        public void SwitchAxes()
        {
            byte byteTemp = 0;

            byteTemp = X;
            X = Y;
            Y = byteTemp;
        }
    }
}