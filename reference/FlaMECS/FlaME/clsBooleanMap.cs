namespace FlaME
{
    using Microsoft.VisualBasic;
    using System;

    public class clsBooleanMap
    {
        public clsValueData ValueData = new clsValueData();

        public void Blank(int SizeX, int SizeY)
        {
            this.ValueData.Size.X = SizeX;
            this.ValueData.Size.Y = SizeY;
            this.ValueData.Value = new bool[(SizeY - 1) + 1, (SizeX - 1) + 1];
        }

        public void Combine(clsBooleanMap Source, clsBooleanMap Insert)
        {
            this.SizeCopy(Source);
            int num3 = Source.ValueData.Size.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.ValueData.Size.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    if (Insert.ValueData.Value[i, j])
                    {
                        this.ValueData.Value[i, j] = true;
                    }
                    else
                    {
                        this.ValueData.Value[i, j] = Source.ValueData.Value[i, j];
                    }
                }
            }
        }

        public void Convert_Heightmap(clsHeightmap Source, long AtOrAboveThisHeightEqualsTrue)
        {
            this.ValueData.Size.X = Source.HeightData.SizeX;
            this.ValueData.Size.Y = Source.HeightData.SizeY;
            this.ValueData.Value = new bool[(this.ValueData.Size.Y - 1) + 1, (this.ValueData.Size.X - 1) + 1];
            int num3 = Source.HeightData.SizeY - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.HeightData.SizeX - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.ValueData.Value[i, j] = Source.HeightData.Height[i, j] >= AtOrAboveThisHeightEqualsTrue;
                }
            }
        }

        public void Copy(clsBooleanMap Source)
        {
            this.SizeCopy(Source);
            int num3 = Source.ValueData.Size.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.ValueData.Size.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.ValueData.Value[i, j] = Source.ValueData.Value[i, j];
                }
            }
        }

        public void Expand_One_Tile(clsBooleanMap Source)
        {
            this.SizeCopy(Source);
            int num3 = this.ValueData.Size.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.ValueData.Size.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    if (Source.ValueData.Value[i, j])
                    {
                        this.ValueData.Value[i, j] = true;
                        if (i > 0)
                        {
                            if (j > 0)
                            {
                                this.ValueData.Value[i - 1, j - 1] = true;
                            }
                            this.ValueData.Value[i - 1, j] = true;
                            if (j < (Source.ValueData.Size.X - 1))
                            {
                                this.ValueData.Value[i - 1, j + 1] = true;
                            }
                        }
                        if (j > 0)
                        {
                            this.ValueData.Value[i, j - 1] = true;
                        }
                        this.ValueData.Value[i, j] = true;
                        if (j < (this.ValueData.Size.X - 1))
                        {
                            this.ValueData.Value[i, j + 1] = true;
                        }
                        if (i < (this.ValueData.Size.Y - 1))
                        {
                            if (j > 0)
                            {
                                this.ValueData.Value[i + 1, j - 1] = true;
                            }
                            this.ValueData.Value[i + 1, j] = true;
                            if (j < (this.ValueData.Size.X - 1))
                            {
                                this.ValueData.Value[i + 1, j + 1] = true;
                            }
                        }
                    }
                }
            }
        }

        public void Remove(clsBooleanMap Source, clsBooleanMap Remove)
        {
            this.SizeCopy(Source);
            int num3 = Source.ValueData.Size.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.ValueData.Size.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    if (Remove.ValueData.Value[i, j])
                    {
                        this.ValueData.Value[i, j] = false;
                    }
                    else
                    {
                        this.ValueData.Value[i, j] = Source.ValueData.Value[i, j];
                    }
                }
            }
        }

        public void Remove_Diagonals()
        {
            int num = 0;
            int num2 = 0;
            while (num2 < (this.ValueData.Size.Y - 1))
            {
                bool flag;
                for (num = 0; num < (this.ValueData.Size.X - 1); num++)
                {
                    flag = false;
                    if (this.ValueData.Value[num2, num])
                    {
                        if (this.ValueData.Value[num2, num + 1])
                        {
                            if (this.ValueData.Value[num2 + 1, num])
                            {
                                if (!this.ValueData.Value[num2 + 1, num + 1] && !this.ValueData.Value[num2 + 1, num + 1])
                                {
                                }
                            }
                            else if (!this.ValueData.Value[num2 + 1, num] && !this.ValueData.Value[num2 + 1, num + 1])
                            {
                                bool flag1 = this.ValueData.Value[num2 + 1, num + 1];
                            }
                        }
                        else if (!this.ValueData.Value[num2, num + 1])
                        {
                            if (this.ValueData.Value[num2 + 1, num])
                            {
                                if (this.ValueData.Value[num2 + 1, num + 1] || this.ValueData.Value[num2 + 1, num + 1])
                                {
                                }
                            }
                            else if (!this.ValueData.Value[num2 + 1, num])
                            {
                                if (this.ValueData.Value[num2 + 1, num + 1])
                                {
                                    if (App.Random.Next() < 0.5f)
                                    {
                                        this.ValueData.Value[num2, num] = false;
                                    }
                                    else
                                    {
                                        this.ValueData.Value[num2 + 1, num + 1] = false;
                                    }
                                    flag = true;
                                }
                                else
                                {
                                    bool flag2 = this.ValueData.Value[num2 + 1, num + 1];
                                }
                            }
                        }
                    }
                    else if (!this.ValueData.Value[num2, num])
                    {
                        if (this.ValueData.Value[num2, num + 1])
                        {
                            if (this.ValueData.Value[num2 + 1, num])
                            {
                                if (!this.ValueData.Value[num2 + 1, num + 1] && !this.ValueData.Value[num2 + 1, num + 1])
                                {
                                    if (App.Random.Next() < 0.5f)
                                    {
                                        this.ValueData.Value[num2, num + 1] = false;
                                    }
                                    else
                                    {
                                        this.ValueData.Value[num2 + 1, num] = false;
                                    }
                                    flag = true;
                                }
                            }
                            else if (!this.ValueData.Value[num2 + 1, num] && !this.ValueData.Value[num2 + 1, num + 1])
                            {
                                bool flag3 = this.ValueData.Value[num2 + 1, num + 1];
                            }
                        }
                        else if (!this.ValueData.Value[num2, num + 1])
                        {
                            if (this.ValueData.Value[num2 + 1, num])
                            {
                                if (this.ValueData.Value[num2 + 1, num + 1] || this.ValueData.Value[num2 + 1, num + 1])
                                {
                                }
                            }
                            else if (!this.ValueData.Value[num2 + 1, num] && !this.ValueData.Value[num2 + 1, num + 1])
                            {
                                bool flag4 = this.ValueData.Value[num2 + 1, num + 1];
                            }
                        }
                    }
                    if (flag)
                    {
                        if (num > 0)
                        {
                            num--;
                        }
                        break;
                    }
                }
                if (flag)
                {
                    if (num2 > 0)
                    {
                        num2--;
                    }
                }
                else
                {
                    num2++;
                }
            }
        }

        public void SizeCopy(clsBooleanMap Source)
        {
            this.ValueData.Size.X = Source.ValueData.Size.X;
            this.ValueData.Size.Y = Source.ValueData.Size.Y;
            this.ValueData.Value = new bool[(this.ValueData.Size.Y - 1) + 1, (this.ValueData.Size.X - 1) + 1];
        }

        public void Within(clsBooleanMap Interior, clsBooleanMap Exterior)
        {
            this.SizeCopy(Interior);
            int num3 = Interior.ValueData.Size.Y - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Interior.ValueData.Size.X - 1;
                for (int j = 0; j <= num4; j++)
                {
                    if (Interior.ValueData.Value[i, j])
                    {
                        bool flag = false;
                        if (i > 0)
                        {
                            if ((j > 0) && !Exterior.ValueData.Value[i - 1, j - 1])
                            {
                                flag = true;
                            }
                            if (!Exterior.ValueData.Value[i - 1, j])
                            {
                                flag = true;
                            }
                            if ((j < (Interior.ValueData.Size.X - 1)) && !Exterior.ValueData.Value[i - 1, j + 1])
                            {
                                flag = true;
                            }
                        }
                        if ((j > 0) && !Exterior.ValueData.Value[i, j - 1])
                        {
                            flag = true;
                        }
                        if (!Exterior.ValueData.Value[i, j])
                        {
                            flag = true;
                        }
                        if ((j < (Interior.ValueData.Size.X - 1)) && !Exterior.ValueData.Value[i, j + 1])
                        {
                            flag = true;
                        }
                        if (i < (Interior.ValueData.Size.Y - 1))
                        {
                            if ((j > 0) && !Exterior.ValueData.Value[i + 1, j - 1])
                            {
                                flag = true;
                            }
                            if (!Exterior.ValueData.Value[i + 1, j])
                            {
                                flag = true;
                            }
                            if ((j < (Interior.ValueData.Size.X - 1)) && !Exterior.ValueData.Value[i + 1, j + 1])
                            {
                                flag = true;
                            }
                        }
                        this.ValueData.Value[i, j] = !flag;
                    }
                }
            }
        }

        public class clsValueData
        {
            public modMath.sXY_int Size;
            public bool[,] Value;
        }
    }
}

