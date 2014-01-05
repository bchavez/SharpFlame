using Microsoft.VisualBasic;
using SharpFlame.Maths;

namespace SharpFlame
{
    public class clsBooleanMap
    {
        public class clsValueData
        {
            public sXY_int Size;
            public bool[,] Value;
        }

        public clsValueData ValueData = new clsValueData();

        public void Blank(int SizeX, int SizeY)
        {
            ValueData.Size.X = SizeX;
            ValueData.Size.Y = SizeY;
            ValueData.Value = new bool[SizeY, SizeX];
        }

        public void SizeCopy(clsBooleanMap Source)
        {
            ValueData.Size.X = Source.ValueData.Size.X;
            ValueData.Size.Y = Source.ValueData.Size.Y;
            ValueData.Value = new bool[ValueData.Size.Y, ValueData.Size.X];
        }

        public void Copy(clsBooleanMap Source)
        {
            int X = 0;
            int Y = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= Source.ValueData.Size.Y - 1; Y++ )
            {
                for ( X = 0; X <= Source.ValueData.Size.X - 1; X++ )
                {
                    ValueData.Value[Y, X] = Source.ValueData.Value[Y, X];
                }
            }
        }

        public void Convert_Heightmap(clsHeightmap Source, long AtOrAboveThisHeightEqualsTrue)
        {
            int X = 0;
            int Y = 0;

            ValueData.Size.X = Source.HeightData.SizeX;
            ValueData.Size.Y = Source.HeightData.SizeY;
            ValueData.Value = new bool[ValueData.Size.Y, ValueData.Size.X];
            for ( Y = 0; Y <= Source.HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= Source.HeightData.SizeX - 1; X++ )
                {
                    ValueData.Value[Y, X] = Source.HeightData.Height[Y, X] >= AtOrAboveThisHeightEqualsTrue;
                }
            }
        }

        public void Remove_Diagonals()
        {
            int X = 0;
            int Y = 0;
            bool Flag = default(bool);

            X = 0;
            Y = 0;
            while ( Y < ValueData.Size.Y - 1 )
            {
                X = 0;
                while ( X < ValueData.Size.X - 1 )
                {
                    Flag = false;
                    if ( ValueData.Value[Y, X] )
                    {
                        if ( ValueData.Value[Y, X + 1] )
                        {
                            if ( ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i i i i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i i i o
                                }
                            }
                            else if ( !ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i i o i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i i o o
                                }
                            }
                        }
                        else if ( !ValueData.Value[Y, X + 1] )
                        {
                            if ( ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i o i i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i o i o
                                }
                            }
                            else if ( !ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i o o i
                                    if ( VBMath.Rnd() < 0.5F )
                                    {
                                        ValueData.Value[Y, X] = false;
                                    }
                                    else
                                    {
                                        ValueData.Value[Y + 1, X + 1] = false;
                                    }
                                    Flag = true;
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //i o o o
                                }
                            }
                        }
                    }
                    else if ( !ValueData.Value[Y, X] )
                    {
                        if ( ValueData.Value[Y, X + 1] )
                        {
                            if ( ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o i i i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o i i o
                                    if ( VBMath.Rnd() < 0.5F )
                                    {
                                        ValueData.Value[Y, X + 1] = false;
                                    }
                                    else
                                    {
                                        ValueData.Value[Y + 1, X] = false;
                                    }
                                    Flag = true;
                                }
                            }
                            else if ( !ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o i o i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o i o o
                                }
                            }
                        }
                        else if ( !ValueData.Value[Y, X + 1] )
                        {
                            if ( ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o o i i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o o i o
                                }
                            }
                            else if ( !ValueData.Value[Y + 1, X] )
                            {
                                if ( ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o o o i
                                }
                                else if ( !ValueData.Value[Y + 1, X + 1] )
                                {
                                    //o o o o
                                }
                            }
                        }
                    }
                    //when flag, go back one in each direction, incase a new diagonal was created
                    if ( Flag )
                    {
                        if ( X > 0 )
                        {
                            X--;
                        }
                        break;
                    }
                    else
                    {
                        X++;
                    }
                }
                if ( Flag )
                {
                    if ( Y > 0 )
                    {
                        Y--;
                    }
                }
                else
                {
                    Y++;
                }
            }
        }

        public void Expand_One_Tile(clsBooleanMap Source)
        {
            int X = 0;
            int Y = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= ValueData.Size.Y - 1; Y++ )
            {
                for ( X = 0; X <= Source.ValueData.Size.X - 1; X++ )
                {
                    if ( Source.ValueData.Value[Y, X] )
                    {
                        ValueData.Value[Y, X] = true;
                        if ( Y > 0 )
                        {
                            if ( X > 0 )
                            {
                                ValueData.Value[Y - 1, X - 1] = true;
                            }
                            ValueData.Value[Y - 1, X] = true;
                            if ( X < Source.ValueData.Size.X - 1 )
                            {
                                ValueData.Value[Y - 1, X + 1] = true;
                            }
                        }
                        if ( X > 0 )
                        {
                            ValueData.Value[Y, X - 1] = true;
                        }
                        ValueData.Value[Y, X] = true;
                        if ( X < ValueData.Size.X - 1 )
                        {
                            ValueData.Value[Y, X + 1] = true;
                        }
                        if ( Y < ValueData.Size.Y - 1 )
                        {
                            if ( X > 0 )
                            {
                                ValueData.Value[Y + 1, X - 1] = true;
                            }
                            ValueData.Value[Y + 1, X] = true;
                            if ( X < ValueData.Size.X - 1 )
                            {
                                ValueData.Value[Y + 1, X + 1] = true;
                            }
                        }
                    }
                }
            }
        }

        public void Remove(clsBooleanMap Source, clsBooleanMap Remove)
        {
            int X = 0;
            int Y = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= Source.ValueData.Size.Y - 1; Y++ )
            {
                for ( X = 0; X <= Source.ValueData.Size.X - 1; X++ )
                {
                    if ( Remove.ValueData.Value[Y, X] )
                    {
                        ValueData.Value[Y, X] = false;
                    }
                    else
                    {
                        ValueData.Value[Y, X] = Source.ValueData.Value[Y, X];
                    }
                }
            }
        }

        public void Combine(clsBooleanMap Source, clsBooleanMap Insert)
        {
            int X = 0;
            int Y = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= Source.ValueData.Size.Y - 1; Y++ )
            {
                for ( X = 0; X <= Source.ValueData.Size.X - 1; X++ )
                {
                    if ( Insert.ValueData.Value[Y, X] )
                    {
                        ValueData.Value[Y, X] = true;
                    }
                    else
                    {
                        ValueData.Value[Y, X] = Source.ValueData.Value[Y, X];
                    }
                }
            }
        }

        public void Within(clsBooleanMap Interior, clsBooleanMap Exterior)
        {
            int Y = 0;
            int X = 0;
            bool Flag = default(bool);

            SizeCopy(Interior);
            for ( Y = 0; Y <= Interior.ValueData.Size.Y - 1; Y++ )
            {
                for ( X = 0; X <= Interior.ValueData.Size.X - 1; X++ )
                {
                    if ( Interior.ValueData.Value[Y, X] )
                    {
                        Flag = false;
                        if ( Y > 0 )
                        {
                            if ( X > 0 )
                            {
                                if ( !Exterior.ValueData.Value[Y - 1, X - 1] )
                                {
                                    Flag = true;
                                }
                            }
                            if ( !Exterior.ValueData.Value[Y - 1, X] )
                            {
                                Flag = true;
                            }
                            if ( X < Interior.ValueData.Size.X - 1 )
                            {
                                if ( !Exterior.ValueData.Value[Y - 1, X + 1] )
                                {
                                    Flag = true;
                                }
                            }
                        }
                        if ( X > 0 )
                        {
                            if ( !Exterior.ValueData.Value[Y, X - 1] )
                            {
                                Flag = true;
                            }
                        }
                        if ( !Exterior.ValueData.Value[Y, X] )
                        {
                            Flag = true;
                        }
                        if ( X < Interior.ValueData.Size.X - 1 )
                        {
                            if ( !Exterior.ValueData.Value[Y, X + 1] )
                            {
                                Flag = true;
                            }
                        }
                        if ( Y < Interior.ValueData.Size.Y - 1 )
                        {
                            if ( X > 0 )
                            {
                                if ( !Exterior.ValueData.Value[Y + 1, X - 1] )
                                {
                                    Flag = true;
                                }
                            }
                            if ( !Exterior.ValueData.Value[Y + 1, X] )
                            {
                                Flag = true;
                            }
                            if ( X < Interior.ValueData.Size.X - 1 )
                            {
                                if ( !Exterior.ValueData.Value[Y + 1, X + 1] )
                                {
                                    Flag = true;
                                }
                            }
                        }
                        ValueData.Value[Y, X] = !Flag;
                    }
                }
            }
        }
    }
}