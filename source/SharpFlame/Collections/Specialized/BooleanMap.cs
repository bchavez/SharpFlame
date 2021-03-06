

using System;
using SharpFlame.Core.Domain;


namespace SharpFlame.Collections.Specialized
{
    public class BooleanMap
    {
        public BooleanMapDataValue ValueData = new BooleanMapDataValue();

        public void Blank(int sizeX, int sizeY)
        {
            ValueData.Size.X = sizeX;
            ValueData.Size.Y = sizeY;
            ValueData.Value = new bool[sizeY, sizeX];
        }

        public void SizeCopy(BooleanMap source)
        {
            ValueData.Size.X = source.ValueData.Size.X;
            ValueData.Size.Y = source.ValueData.Size.Y;
            ValueData.Value = new bool[ValueData.Size.Y, ValueData.Size.X];
        }

        public void Copy(BooleanMap source)
        {
            SizeCopy(source);
            for (var y = 0; y <= source.ValueData.Size.Y - 1; y++)
            {
                for (var x = 0; x <= source.ValueData.Size.X - 1; x++)
                {
                    ValueData.Value[y, x] = source.ValueData.Value[y, x];
                }
            }
        }

        public void Convert_Heightmap(clsHeightmap source, long atOrAboveThisHeightEqualsTrue)
        {
            ValueData.Size.X = source.HeightData.SizeX;
            ValueData.Size.Y = source.HeightData.SizeY;
            ValueData.Value = new bool[ValueData.Size.Y, ValueData.Size.X];
            for (var y = 0; y <= source.HeightData.SizeY - 1; y++)
            {
                for (var x = 0; x <= source.HeightData.SizeX - 1; x++)
                {
                    ValueData.Value[y, x] = source.HeightData.Height[y, x] >= atOrAboveThisHeightEqualsTrue;
                }
            }
        }

        public void RemoveDiagonals()
        {
            var y = 0;
            var flag = default(bool);

            var rnd = new Random();

            while (y < ValueData.Size.Y - 1)
            {
                var x = 0;
                while (x < ValueData.Size.X - 1)
                {
                    flag = false;
                    if (ValueData.Value[y, x])
                    {
                        if (ValueData.Value[y, x + 1])
                        {
                            if (ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //i i i i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //i i i o
                                }
                            }
                            else if (!ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //i i o i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //i i o o
                                }
                            }
                        }
                        else if (!ValueData.Value[y, x + 1])
                        {
                            if (ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //i o i i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //i o i o
                                }
                            }
                            else if (!ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //i o o i
                                    if (rnd.Next() < 0.5F)
                                    {
                                        ValueData.Value[y, x] = false;
                                    }
                                    else
                                    {
                                        ValueData.Value[y + 1, x + 1] = false;
                                    }
                                    flag = true;
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //i o o o
                                }
                            }
                        }
                    }
                    else if (!ValueData.Value[y, x])
                    {
                        if (ValueData.Value[y, x + 1])
                        {
                            if (ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //o i i i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //o i i o
                                    if (rnd.Next() < 0.5F)
                                    {
                                        ValueData.Value[y, x + 1] = false;
                                    }
                                    else
                                    {
                                        ValueData.Value[y + 1, x] = false;
                                    }
                                    flag = true;
                                }
                            }
                            else if (!ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //o i o i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //o i o o
                                }
                            }
                        }
                        else if (!ValueData.Value[y, x + 1])
                        {
                            if (ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //o o i i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //o o i o
                                }
                            }
                            else if (!ValueData.Value[y + 1, x])
                            {
                                if (ValueData.Value[y + 1, x + 1])
                                {
                                    //o o o i
                                }
                                else if (!ValueData.Value[y + 1, x + 1])
                                {
                                    //o o o o
                                }
                            }
                        }
                    }
                    //when flag, go back one in each direction, incase a new diagonal was created
                    if (flag)
                    {
                        break;
                    }
                    x++;
                }
                if (flag)
                {
                    if (y > 0)
                    {
                        y--;
                    }
                }
                else
                {
                    y++;
                }
            }
        }

        public void ExpandOneTile(BooleanMap source)
        {
            SizeCopy(source);
            for (var y = 0; y <= ValueData.Size.Y - 1; y++)
            {
                for (var x = 0; x <= source.ValueData.Size.X - 1; x++)
                {
                    if (source.ValueData.Value[y, x])
                    {
                        ValueData.Value[y, x] = true;
                        if (y > 0)
                        {
                            if (x > 0)
                            {
                                ValueData.Value[y - 1, x - 1] = true;
                            }
                            ValueData.Value[y - 1, x] = true;
                            if (x < source.ValueData.Size.X - 1)
                            {
                                ValueData.Value[y - 1, x + 1] = true;
                            }
                        }
                        if (x > 0)
                        {
                            ValueData.Value[y, x - 1] = true;
                        }
                        ValueData.Value[y, x] = true;
                        if (x < ValueData.Size.X - 1)
                        {
                            ValueData.Value[y, x + 1] = true;
                        }
                        if (y < ValueData.Size.Y - 1)
                        {
                            if (x > 0)
                            {
                                ValueData.Value[y + 1, x - 1] = true;
                            }
                            ValueData.Value[y + 1, x] = true;
                            if (x < ValueData.Size.X - 1)
                            {
                                ValueData.Value[y + 1, x + 1] = true;
                            }
                        }
                    }
                }
            }
        }

        public void Remove(BooleanMap source, BooleanMap remove)
        {
            SizeCopy(source);
            for (var y = 0; y <= source.ValueData.Size.Y - 1; y++)
            {
                for (var x = 0; x <= source.ValueData.Size.X - 1; x++)
                {
                    if (remove.ValueData.Value[y, x])
                    {
                        ValueData.Value[y, x] = false;
                    }
                    else
                    {
                        ValueData.Value[y, x] = source.ValueData.Value[y, x];
                    }
                }
            }
        }

        public void Combine(BooleanMap source, BooleanMap insert)
        {
            SizeCopy(source);
            for (var y = 0; y <= source.ValueData.Size.Y - 1; y++)
            {
                for (var x = 0; x <= source.ValueData.Size.X - 1; x++)
                {
                    if (insert.ValueData.Value[y, x])
                    {
                        ValueData.Value[y, x] = true;
                    }
                    else
                    {
                        ValueData.Value[y, x] = source.ValueData.Value[y, x];
                    }
                }
            }
        }

        public void Within(BooleanMap interior, BooleanMap exterior)
        {
            SizeCopy(interior);
            for (var y = 0; y <= interior.ValueData.Size.Y - 1; y++)
            {
                for (var x = 0; x <= interior.ValueData.Size.X - 1; x++)
                {
                    if (interior.ValueData.Value[y, x])
                    {
                        var flag = false;
                        if (y > 0)
                        {
                            if (x > 0)
                            {
                                if (!exterior.ValueData.Value[y - 1, x - 1])
                                {
                                    flag = true;
                                }
                            }
                            if (!exterior.ValueData.Value[y - 1, x])
                            {
                                flag = true;
                            }
                            if (x < interior.ValueData.Size.X - 1)
                            {
                                if (!exterior.ValueData.Value[y - 1, x + 1])
                                {
                                    flag = true;
                                }
                            }
                        }
                        if (x > 0)
                        {
                            if (!exterior.ValueData.Value[y, x - 1])
                            {
                                flag = true;
                            }
                        }
                        if (!exterior.ValueData.Value[y, x])
                        {
                            flag = true;
                        }
                        if (x < interior.ValueData.Size.X - 1)
                        {
                            if (!exterior.ValueData.Value[y, x + 1])
                            {
                                flag = true;
                            }
                        }
                        if (y < interior.ValueData.Size.Y - 1)
                        {
                            if (x > 0)
                            {
                                if (!exterior.ValueData.Value[y + 1, x - 1])
                                {
                                    flag = true;
                                }
                            }
                            if (!exterior.ValueData.Value[y + 1, x])
                            {
                                flag = true;
                            }
                            if (x < interior.ValueData.Size.X - 1)
                            {
                                if (!exterior.ValueData.Value[y + 1, x + 1])
                                {
                                    flag = true;
                                }
                            }
                        }
                        ValueData.Value[y, x] = !flag;
                    }
                }
            }
        }
    }

    public class BooleanMapDataValue
    {
        public XYInt Size;
        public bool[,] Value;
    }
}