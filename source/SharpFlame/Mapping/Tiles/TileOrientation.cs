namespace SharpFlame.Mapping.Tiles
{
    public struct TileOrientation
    {
        public bool XFlip;
        public bool YFlip;
        public bool SwitchedAxes;

        public TileOrientation(bool resultXFlip, bool resultZFlip, bool switchedAxes)
        {
            XFlip = resultXFlip;
            YFlip = resultZFlip;
            SwitchedAxes = switchedAxes;
        }

        public TileOrientation GetRotated(TileOrientation orientation)
        {
            var ReturnResult = new TileOrientation();

            ReturnResult.SwitchedAxes = SwitchedAxes ^ orientation.SwitchedAxes;

            if ( orientation.SwitchedAxes )
            {
                if ( orientation.XFlip )
                {
                    ReturnResult.XFlip = !YFlip;
                }
                else
                {
                    ReturnResult.XFlip = YFlip;
                }
                if ( orientation.YFlip )
                {
                    ReturnResult.YFlip = !XFlip;
                }
                else
                {
                    ReturnResult.YFlip = XFlip;
                }
            }
            else
            {
                if ( orientation.XFlip )
                {
                    ReturnResult.XFlip = !XFlip;
                }
                else
                {
                    ReturnResult.XFlip = XFlip;
                }
                if ( orientation.YFlip )
                {
                    ReturnResult.YFlip = !YFlip;
                }
                else
                {
                    ReturnResult.YFlip = YFlip;
                }
            }

            return ReturnResult;
        }

        public void FlipX()
        {
            if ( SwitchedAxes )
            {
                YFlip = !YFlip;
            }
            else
            {
                XFlip = !XFlip;
            }
        }

        public void Reverse()
        {
            if ( SwitchedAxes )
            {
                if ( XFlip ^ YFlip )
                {
                    XFlip = !XFlip;
                    YFlip = !YFlip;
                }
            }
        }

        public void RotateClockwise()
        {
            SwitchedAxes = !SwitchedAxes;
            if ( XFlip ^ YFlip )
            {
                YFlip = !YFlip;
            }
            else
            {
                XFlip = !XFlip;
            }
        }

        public void RotateAntiClockwise()
        {
            SwitchedAxes = !SwitchedAxes;
            if ( XFlip ^ YFlip )
            {
                XFlip = !XFlip;
            }
            else
            {
                YFlip = !YFlip;
            }
        }

        public new string ToString()
        {
            return string.Format ("SwitchAxes={0}, ResultXFlip={1}, ResultYFlip={2}", 
                                  SwitchedAxes, XFlip, YFlip);
        }
    }
}