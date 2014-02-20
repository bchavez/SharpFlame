namespace SharpFlame.Mapping.Tiles
{
    public struct TileOrientation
    {
        public bool ResultXFlip;
        public bool ResultYFlip;
        public bool SwitchedAxes;

        public TileOrientation(bool resultXFlip, bool resultZFlip, bool switchedAxes)
        {
            ResultXFlip = resultXFlip;
            ResultYFlip = resultZFlip;
            SwitchedAxes = switchedAxes;
        }

        public TileOrientation GetRotated(TileOrientation Orientation)
        {
            var ReturnResult = new TileOrientation();

            ReturnResult.SwitchedAxes = SwitchedAxes ^ Orientation.SwitchedAxes;

            if ( Orientation.SwitchedAxes )
            {
                if ( Orientation.ResultXFlip )
                {
                    ReturnResult.ResultXFlip = !ResultYFlip;
                }
                else
                {
                    ReturnResult.ResultXFlip = ResultYFlip;
                }
                if ( Orientation.ResultYFlip )
                {
                    ReturnResult.ResultYFlip = !ResultXFlip;
                }
                else
                {
                    ReturnResult.ResultYFlip = ResultXFlip;
                }
            }
            else
            {
                if ( Orientation.ResultXFlip )
                {
                    ReturnResult.ResultXFlip = !ResultXFlip;
                }
                else
                {
                    ReturnResult.ResultXFlip = ResultXFlip;
                }
                if ( Orientation.ResultYFlip )
                {
                    ReturnResult.ResultYFlip = !ResultYFlip;
                }
                else
                {
                    ReturnResult.ResultYFlip = ResultYFlip;
                }
            }

            return ReturnResult;
        }

        public void Reverse()
        {
            if ( SwitchedAxes )
            {
                if ( ResultXFlip ^ ResultYFlip )
                {
                    ResultXFlip = !ResultXFlip;
                    ResultYFlip = !ResultYFlip;
                }
            }
        }

        public void RotateClockwise()
        {
            SwitchedAxes = !SwitchedAxes;
            if ( ResultXFlip ^ ResultYFlip )
            {
                ResultYFlip = !ResultYFlip;
            }
            else
            {
                ResultXFlip = !ResultXFlip;
            }
        }

        public void RotateAntiClockwise()
        {
            SwitchedAxes = !SwitchedAxes;
            if ( ResultXFlip ^ ResultYFlip )
            {
                ResultXFlip = !ResultXFlip;
            }
            else
            {
                ResultYFlip = !ResultYFlip;
            }
        }
    }
}