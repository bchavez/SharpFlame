namespace FlaME
{
    using Microsoft.VisualBasic;
    using System;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.InteropServices;

    public class clsHeightmap
    {
        public clsHeightData HeightData = new clsHeightData();
        public double HeightScale = 0.0001;

        public void Add(clsHeightmap Source, double Amount)
        {
            this.SizeCopy(Source);
            int num3 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (((Source.HeightData.Height[i, j] * Source.HeightScale) + Amount) / this.HeightScale));
                }
            }
        }

        public void Add2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            double num = SourceA.HeightScale / this.HeightScale;
            double num2 = SourceB.HeightScale / this.HeightScale;
            if (!SourceA.IsSizeSame(SourceB))
            {
                Debugger.Break();
            }
            this.SizeCopy(SourceA);
            int num5 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num5; i++)
            {
                int num6 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num6; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((SourceA.HeightData.Height[i, j] * num) + (SourceB.HeightData.Height[i, j] * num2)));
                }
            }
        }

        public void Blank(int SizeY, int SizeX)
        {
            this.HeightData.SizeX = SizeX;
            this.HeightData.SizeY = SizeY;
            this.HeightData.Height = new long[(SizeY - 1) + 1, (SizeX - 1) + 1];
        }

        public void Clamp(clsHeightmap Source, double HeightMin, double HeightMax)
        {
            this.SizeCopy(Source);
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    double num = Source.HeightData.Height[i, j] * Source.HeightScale;
                    if (num < HeightMin)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) (HeightMin / this.HeightScale));
                    }
                    else if (num > HeightMax)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) (HeightMax / this.HeightScale));
                    }
                    else
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) (num / this.HeightScale));
                    }
                }
            }
        }

        public void Copy(clsHeightmap Source)
        {
            this.HeightScale = Source.HeightScale;
            this.SizeCopy(Source);
            int num3 = Source.HeightData.SizeY - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.HeightData.SizeX - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.HeightData.Height[i, j] = Source.HeightData.Height[i, j];
                }
            }
        }

        public void Divide(clsHeightmap Source, double Denominator)
        {
            double num = Source.HeightScale / (Denominator * this.HeightScale);
            this.SizeCopy(Source);
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (Source.HeightData.Height[i, j] * num));
                }
            }
        }

        public void Divide2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            double num = SourceA.HeightScale / (SourceB.HeightScale * this.HeightScale);
            if (!SourceA.IsSizeSame(SourceB))
            {
                Debugger.Break();
            }
            this.SizeCopy(SourceA);
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((((double) SourceA.HeightData.Height[i, j]) / ((double) SourceB.HeightData.Height[i, j])) * num));
                }
            }
        }

        public void FadeMultiple(clsHeightmap hmSource, ref sHeightmaps AlterationMaps, ref sHeights AlterationHeights)
        {
            int upperBound = AlterationHeights.Heights.GetUpperBound(0);
            this.SizeCopy(hmSource);
            int num10 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num10; i++)
            {
                int num11 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num11; j++)
                {
                    float num5 = (float) (hmSource.HeightData.Height[i, j] * hmSource.HeightScale);
                    int num12 = upperBound;
                    int index = 0;
                    while (index <= num12)
                    {
                        if (num5 <= AlterationHeights.Heights[index])
                        {
                            break;
                        }
                        index++;
                    }
                    if (index == 0)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) ((AlterationMaps.Heightmaps[index].HeightData.Height[i, j] * AlterationMaps.Heightmaps[index].HeightScale) / this.HeightScale));
                    }
                    else if (index > upperBound)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) ((AlterationMaps.Heightmaps[upperBound].HeightData.Height[i, j] * AlterationMaps.Heightmaps[upperBound].HeightScale) / this.HeightScale));
                    }
                    else
                    {
                        int num2 = index - 1;
                        float num6 = AlterationHeights.Heights[num2];
                        float num7 = AlterationHeights.Heights[index];
                        float num4 = (num5 - num6) / (num7 - num6);
                        this.HeightData.Height[i, j] = (long) Math.Round((double) ((((AlterationMaps.Heightmaps[num2].HeightData.Height[i, j] * AlterationMaps.Heightmaps[num2].HeightScale) * (1f - num4)) + ((AlterationMaps.Heightmaps[index].HeightData.Height[i, j] * AlterationMaps.Heightmaps[index].HeightScale) * num4)) / this.HeightScale));
                    }
                }
            }
        }

        public void Generate(clsHeightmap Source, int Inflations, double NoiseFactor, double HeightMultiplier)
        {
            clsHeightmap heightmap = new clsHeightmap();
            if (Inflations >= 1)
            {
                heightmap.Inflate(Source, NoiseFactor, HeightMultiplier, 1);
                this.HeightData = heightmap.HeightData;
                heightmap.HeightData = new clsHeightData();
                int num2 = Inflations;
                for (int i = 2; i <= num2; i++)
                {
                    heightmap.Inflate(this, NoiseFactor, HeightMultiplier, i);
                    this.HeightData = heightmap.HeightData;
                    heightmap.HeightData = new clsHeightData();
                }
            }
            else if (Inflations == 0)
            {
                this.Copy(Source);
            }
        }

        public void GenerateNew(int SizeY, int SizeX, int Inflations, double NoiseFactor, double HeightMultiplier)
        {
            clsHeightmap heightmap = new clsHeightmap();
            this.Blank(SizeY, SizeX);
            this.Randomize(HeightMultiplier / this.HeightScale);
            heightmap.HeightScale = this.HeightScale;
            heightmap.Generate(this, Inflations, NoiseFactor, HeightMultiplier / this.HeightScale);
            this.HeightData = heightmap.HeightData;
        }

        public void GenerateNewOfSize(int Final_SizeY, int Final_SizeX, float Scale, double HeightMultiplier)
        {
            int num;
            clsHeightmap heightmap = new clsHeightmap();
            double num3 = Math.Log(2.0);
            if (Final_SizeX > Final_SizeY)
            {
                num = (int) Math.Round(Math.Ceiling((double) (Math.Log((double) (Final_SizeX - 1)) / num3)));
            }
            else
            {
                num = (int) Math.Round(Math.Ceiling((double) (Math.Log((double) (Final_SizeY - 1)) / num3)));
            }
            num = (int) Math.Round(Math.Ceiling((double) Scale));
            if (num < 0)
            {
                Debugger.Break();
            }
            double num4 = Math.Pow(2.0, (double) (Scale - num));
            int num2 = (int) Math.Round(Math.Pow(2.0, (double) num));
            int sizeX = ((int) Math.Round(Math.Ceiling((double) (((((double) Final_SizeX) / num4) - 1.0) / ((double) num2))))) + 1;
            int sizeY = ((int) Math.Round(Math.Ceiling((double) (((((double) Final_SizeY) / num4) - 1.0) / ((double) num2))))) + 1;
            this.GenerateNew(sizeY, sizeX, num, 1.0, HeightMultiplier);
            if (num > Scale)
            {
                heightmap.Stretch(this, (int) Math.Round((double) (this.HeightData.SizeX * num4)), (int) Math.Round((double) (this.HeightData.SizeY * num4)));
                this.HeightData = heightmap.HeightData;
                heightmap.HeightData = new clsHeightData();
            }
            if ((this.HeightData.SizeX != Final_SizeX) | (this.HeightData.SizeY != Final_SizeY))
            {
                heightmap.Resize(this, 0, 0, Final_SizeY, Final_SizeX);
                this.HeightData = heightmap.HeightData;
            }
        }

        public void Highest(clsHeightmap Source, double Value)
        {
            double num = Source.HeightScale / this.HeightScale;
            double a = Value / this.HeightScale;
            this.SizeCopy(Source);
            int num6 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num6; i++)
            {
                int num7 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num7; j++)
                {
                    double num3 = Source.HeightData.Height[i, j] * num;
                    if (num3 >= a)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(num3);
                    }
                    else
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(a);
                    }
                }
            }
        }

        public void Highest2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            double num3 = SourceA.HeightScale / this.HeightScale;
            double num4 = SourceB.HeightScale / this.HeightScale;
            if (!SourceA.IsSizeSame(SourceB))
            {
                Debugger.Break();
            }
            this.SizeCopy(SourceA);
            int num7 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num7; i++)
            {
                int num8 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num8; j++)
                {
                    double a = SourceA.HeightData.Height[i, j] * num3;
                    double num2 = SourceB.HeightData.Height[i, j] * num4;
                    if (a >= num2)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(a);
                    }
                    else
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(num2);
                    }
                }
            }
        }

        public void Inflate(clsHeightmap Source, double NoiseFactor, double HeightMultiplier, int VariationReduction)
        {
            if (!((Source.HeightData.SizeY == 0) | (Source.HeightData.SizeX == 0)))
            {
                double num3;
                long num4;
                int num7;
                int num8;
                this.Blank(((Source.HeightData.SizeY - 1) * 2) + 1, ((Source.HeightData.SizeX - 1) * 2) + 1);
                int num9 = Source.HeightData.SizeY - 1;
                for (num8 = 0; num8 <= num9; num8++)
                {
                    int num10 = Source.HeightData.SizeX - 1;
                    num7 = 0;
                    while (num7 <= num10)
                    {
                        this.HeightData.Height[((num8 + 1) * 2) - 2, ((num7 + 1) * 2) - 2] = Source.HeightData.Height[num8, num7];
                        num7++;
                    }
                }
                if (NoiseFactor == 0.0)
                {
                    num3 = 0.0;
                }
                else
                {
                    num3 = Math.Pow(2.0 / NoiseFactor, (double) (0 - VariationReduction));
                }
                double num2 = 1.4142135623730951;
                double num5 = (num2 * num3) * HeightMultiplier;
                long num6 = (long) Math.Round((double) (num5 / 2.0));
                int num11 = this.HeightData.SizeY - 2;
                for (num8 = 1; num8 <= num11; num8 += 2)
                {
                    int num12 = this.HeightData.SizeX - 2;
                    num7 = 1;
                    while (num7 <= num12)
                    {
                        num4 = (long) Math.Round((double) (((double) (((this.HeightData.Height[num8 - 1, num7 - 1] + this.HeightData.Height[num8 - 1, num7 + 1]) + this.HeightData.Height[num8 + 1, num7 - 1]) + this.HeightData.Height[num8 + 1, num7 + 1])) / 4.0));
                        this.HeightData.Height[num8, num7] = (num4 + ((long) Math.Round((double) (App.Random.Next() * num5)))) - num6;
                        num7 += 2;
                    }
                }
                num2 = 1.0;
                num5 = (num2 * num3) * HeightMultiplier;
                num6 = (long) Math.Round((double) (num5 / 2.0));
                int num13 = this.HeightData.SizeY - 2;
                for (num8 = 1; num8 <= num13; num8++)
                {
                    int num = num8 - (((int) Math.Round(Conversion.Int((double) (((double) num8) / 2.0)))) * 2);
                    int num14 = (this.HeightData.SizeX - 2) - num;
                    num7 = 1 + num;
                    while (num7 <= num14)
                    {
                        num4 = (long) Math.Round((double) (((double) (((this.HeightData.Height[num8 - 1, num7] + this.HeightData.Height[num8, num7 - 1]) + this.HeightData.Height[num8, num7 + 1]) + this.HeightData.Height[num8 + 1, num7])) / 4.0));
                        this.HeightData.Height[num8, num7] = (num4 + ((long) Math.Round((double) (App.Random.Next() * num5)))) - num6;
                        num7 += 2;
                    }
                }
                num8 = 0;
                int num15 = this.HeightData.SizeX - 2;
                for (num7 = 1; num7 <= num15; num7 += 2)
                {
                    num4 = (long) Math.Round((double) (((double) ((this.HeightData.Height[num8, num7 - 1] + this.HeightData.Height[num8, num7 + 1]) + this.HeightData.Height[num8 + 1, num7])) / 3.0));
                    this.HeightData.Height[num8, num7] = (num4 + ((long) Math.Round((double) (App.Random.Next() * num5)))) - num6;
                }
                num7 = 0;
                int num16 = this.HeightData.SizeY - 2;
                for (num8 = 1; num8 <= num16; num8 += 2)
                {
                    num4 = (long) Math.Round((double) (((double) ((this.HeightData.Height[num8 - 1, num7] + this.HeightData.Height[num8, num7 + 1]) + this.HeightData.Height[num8 + 1, num7])) / 3.0));
                    this.HeightData.Height[num8, num7] = (num4 + ((long) Math.Round((double) (App.Random.Next() * num5)))) - num6;
                }
                num7 = this.HeightData.SizeX - 1;
                int num17 = this.HeightData.SizeY - 2;
                for (num8 = 1; num8 <= num17; num8 += 2)
                {
                    num4 = (long) Math.Round((double) (((double) ((this.HeightData.Height[num8 - 1, num7] + this.HeightData.Height[num8, num7 - 1]) + this.HeightData.Height[num8 + 1, num7])) / 3.0));
                    this.HeightData.Height[num8, num7] = (num4 + ((long) Math.Round((double) (App.Random.Next() * num5)))) - num6;
                }
                num8 = this.HeightData.SizeY - 1;
                int num18 = this.HeightData.SizeX - 2;
                for (num7 = 1; num7 <= num18; num7 += 2)
                {
                    num4 = (long) Math.Round((double) (((double) ((this.HeightData.Height[num8 - 1, num7] + this.HeightData.Height[num8, num7 - 1]) + this.HeightData.Height[num8, num7 + 1])) / 3.0));
                    this.HeightData.Height[num8, num7] = (num4 + ((long) Math.Round((double) (App.Random.Next() * num5)))) - num6;
                }
            }
        }

        public void Insert(clsHeightmap Source, int Y1, int X1)
        {
            int num3 = Source.HeightData.SizeY - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = Source.HeightData.SizeX - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.HeightData.Height[Y1 + i, X1 + j] = Source.HeightData.Height[i, j];
                }
            }
        }

        public void Intervalise(clsHeightmap Source, double Interval)
        {
            double num = Source.HeightScale / Interval;
            double num2 = Interval / this.HeightScale;
            this.SizeCopy(Source);
            int num5 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num5; i++)
            {
                int num6 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num6; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (Conversion.Int((double) (Source.HeightData.Height[i, j] * num)) * num2));
                }
            }
        }

        public void Invert(clsHeightmap Source)
        {
            double num = -Source.HeightScale / this.HeightScale;
            this.SizeCopy(Source);
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (Source.HeightData.Height[i, j] * num));
                }
            }
        }

        public bool IsSizeSame(clsHeightmap Source)
        {
            return ((this.HeightData.SizeX == Source.HeightData.SizeX) & (this.HeightData.SizeY == Source.HeightData.SizeY));
        }

        public modProgram.sResult Load_Image(string Path)
        {
            modProgram.sResult result3;
            result3.Success = false;
            result3.Problem = "";
            Bitmap resultBitmap = null;
            modProgram.sResult result2 = modBitmap.LoadBitmap(Path, ref resultBitmap);
            if (!result2.Success)
            {
                result3.Problem = result2.Problem;
                return result3;
            }
            this.Blank(resultBitmap.Height, resultBitmap.Width);
            int num3 = resultBitmap.Height - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = resultBitmap.Width - 1;
                for (int j = 0; j <= num4; j++)
                {
                    Color pixel = resultBitmap.GetPixel(j, i);
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (((double) ((short) (((short) (pixel.R + pixel.G)) + pixel.B))) / (3.0 * this.HeightScale)));
                }
            }
            result3.Success = true;
            return result3;
        }

        public void Lowest(clsHeightmap Source, double Value)
        {
            double num = Source.HeightScale / this.HeightScale;
            double a = Value / this.HeightScale;
            this.SizeCopy(Source);
            int num6 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num6; i++)
            {
                int num7 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num7; j++)
                {
                    double num3 = Source.HeightData.Height[i, j] * num;
                    if (num3 <= a)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(num3);
                    }
                    else
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(a);
                    }
                }
            }
        }

        public void Lowest2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            double num3 = SourceA.HeightScale / this.HeightScale;
            double num4 = SourceB.HeightScale / this.HeightScale;
            if (!SourceA.IsSizeSame(SourceB))
            {
                Debugger.Break();
            }
            this.SizeCopy(SourceA);
            int num7 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num7; i++)
            {
                int num8 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num8; j++)
                {
                    double a = SourceA.HeightData.Height[i, j] * num3;
                    double num2 = SourceB.HeightData.Height[i, j] * num4;
                    if (a <= num2)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(a);
                    }
                    else
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round(num2);
                    }
                }
            }
        }

        public void MinMaxGet(ref sMinMax MinMax_Output)
        {
            long num;
            long num2;
            if (!((this.HeightData.SizeY == 0) | (this.HeightData.SizeX == 0)))
            {
                num2 = this.HeightData.Height[0, 0];
                num = this.HeightData.Height[0, 0];
                int num6 = this.HeightData.SizeY - 1;
                for (int i = 0; i <= num6; i++)
                {
                    int num7 = this.HeightData.SizeX - 1;
                    for (int j = 0; j <= num7; j++)
                    {
                        long num3 = this.HeightData.Height[i, j];
                        if (num3 < num2)
                        {
                            num2 = num3;
                        }
                        if (num3 > num)
                        {
                            num = num3;
                        }
                    }
                }
            }
            MinMax_Output.Min = num2;
            MinMax_Output.Max = num;
        }

        public void Multiply(clsHeightmap Source, double Multiplier)
        {
            double num = (Source.HeightScale * Multiplier) / this.HeightScale;
            this.SizeCopy(Source);
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (Source.HeightData.Height[i, j] * num));
                }
            }
        }

        public void Multiply2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            if (!SourceA.IsSizeSame(SourceB))
            {
                Debugger.Break();
            }
            this.SizeCopy(SourceA);
            int num3 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((((SourceA.HeightData.Height[i, j] * SourceA.HeightScale) * SourceB.HeightData.Height[i, j]) * SourceB.HeightScale) / this.HeightScale));
                }
            }
        }

        public void Randomize(double HeightMultiplier)
        {
            long num = (long) Math.Round((double) (HeightMultiplier / 2.0));
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((App.Random.Next() * HeightMultiplier) - num));
                }
            }
        }

        public void Rescale(clsHeightmap Source, double HeightMin, double HeightMax)
        {
            long num2;
            int num5;
            int num6;
            sMinMax max = new sMinMax();
            Source.MinMaxGet(ref max);
            this.SizeCopy(Source);
            long num = max.Max - max.Min;
            long num3 = 0L - max.Min;
            if (num > 0L)
            {
                double num4 = (HeightMax - HeightMin) / (num * this.HeightScale);
                num2 = (long) Math.Round((double) (HeightMin / this.HeightScale));
                int num7 = this.HeightData.SizeY - 1;
                for (num6 = 0; num6 <= num7; num6++)
                {
                    int num8 = this.HeightData.SizeX - 1;
                    num5 = 0;
                    while (num5 <= num8)
                    {
                        this.HeightData.Height[num6, num5] = num2 + ((long) Math.Round((double) ((num3 + Source.HeightData.Height[num6, num5]) * num4)));
                        num5++;
                    }
                }
            }
            else
            {
                num2 = (long) Math.Round((double) ((HeightMin + HeightMax) / 2.0));
                int num9 = this.HeightData.SizeY - 1;
                for (num6 = 0; num6 <= num9; num6++)
                {
                    int num10 = this.HeightData.SizeX - 1;
                    for (num5 = 0; num5 <= num10; num5++)
                    {
                        this.HeightData.Height[num6, num5] = num2;
                    }
                }
            }
        }

        public void Resize(clsHeightmap Source, int OffsetY, int OffsetX, int SizeY, int SizeX)
        {
            this.Blank(SizeY, SizeX);
            int num3 = Math.Max(0 - OffsetX, 0);
            int num4 = Math.Max(0 - OffsetY, 0);
            int num = Math.Min(Source.HeightData.SizeX - OffsetX, this.HeightData.SizeX) - 1;
            int num7 = Math.Min(Source.HeightData.SizeY - OffsetY, this.HeightData.SizeY) - 1;
            for (int i = num4; i <= num7; i++)
            {
                int num8 = num;
                for (int j = num3; j <= num8; j++)
                {
                    this.HeightData.Height[i, j] = Source.HeightData.Height[OffsetY + i, OffsetX + j];
                }
            }
        }

        public void ShiftToZero(clsHeightmap Source)
        {
            sMinMax max = new sMinMax();
            double num = Source.HeightScale / this.HeightScale;
            Source.MinMaxGet(ref max);
            this.SizeCopy(Source);
            long num2 = 0L - max.Min;
            int num5 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num5; i++)
            {
                int num6 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num6; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((num2 + Source.HeightData.Height[i, j]) * num));
                }
            }
        }

        public void SizeCopy(clsHeightmap Source)
        {
            this.HeightData.SizeX = Source.HeightData.SizeX;
            this.HeightData.SizeY = Source.HeightData.SizeY;
            this.HeightData.Height = new long[(this.HeightData.SizeY - 1) + 1, (this.HeightData.SizeX - 1) + 1];
        }

        public void Stretch(clsHeightmap hmSource, int SizeX, int SizeY)
        {
            float num14 = (float) (hmSource.HeightScale / this.HeightScale);
            int sizeX = hmSource.HeightData.SizeX;
            int sizeY = hmSource.HeightData.SizeY;
            this.Blank(SizeY, SizeX);
            float num = (float) (((double) SizeX) / ((double) sizeX));
            float num2 = (float) (((double) SizeY) / ((double) sizeY));
            int num17 = sizeY - 1;
            for (int i = 0; i <= num17; i++)
            {
                int num18 = sizeX - 1;
                for (int j = 0; j <= num18; j++)
                {
                    float number = j * num;
                    float num10 = i * num2;
                    float num7 = (j + 1) * num;
                    float num8 = (i + 1) * num2;
                    int num19 = (int) Math.Round((double) -Conversion.Int(-num8));
                    for (int k = (int) Math.Round((double) Conversion.Int(num10)); k <= num19; k++)
                    {
                        if (k >= SizeY)
                        {
                            break;
                        }
                        int num20 = (int) Math.Round((double) -Conversion.Int(-num7));
                        for (int m = (int) Math.Round((double) Conversion.Int(number)); m <= num20; m++)
                        {
                            if (m >= SizeX)
                            {
                                break;
                            }
                            if ((((num8 > k) & (num10 < (k + 1))) & (num7 > m)) & (number < (m + 1)))
                            {
                                float num15 = 1f;
                                float num16 = 1f;
                                if (num10 > k)
                                {
                                    num16 -= num10 - k;
                                }
                                if (number > m)
                                {
                                    num15 -= number - m;
                                }
                                if (num8 < (k + 1))
                                {
                                    num16 -= (k + 1) - num8;
                                }
                                if (num7 < (m + 1))
                                {
                                    num15 -= (m + 1) - num7;
                                }
                                float num13 = num15 * num16;
                                this.HeightData.Height[k, m] = (long) Math.Round((double) (this.HeightData.Height[k, m] + ((hmSource.HeightData.Height[i, j] * num13) * num14)));
                            }
                        }
                    }
                }
            }
        }

        public void Subtract(clsHeightmap Source, double Amount)
        {
            this.SizeCopy(Source);
            int num3 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num3; i++)
            {
                int num4 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num4; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) (((Source.HeightData.Height[i, j] * Source.HeightScale) - Amount) / this.HeightScale));
                }
            }
        }

        public void Subtract2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            double num = SourceA.HeightScale / this.HeightScale;
            double num2 = SourceB.HeightScale / this.HeightScale;
            if (!SourceA.IsSizeSame(SourceB))
            {
                Debugger.Break();
            }
            this.SizeCopy(SourceA);
            int num5 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num5; i++)
            {
                int num6 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num6; j++)
                {
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((SourceA.HeightData.Height[i, j] * num) - (SourceB.HeightData.Height[i, j] * num2)));
                }
            }
        }

        public void Swap3(clsHeightmap SourceA, clsHeightmap SourceB, clsHeightmap Swapper)
        {
            if (!(Swapper.IsSizeSame(SourceA) & Swapper.IsSizeSame(SourceB)))
            {
                Debugger.Break();
            }
            this.SizeCopy(Swapper);
            int num4 = this.HeightData.SizeY - 1;
            for (int i = 0; i <= num4; i++)
            {
                int num5 = this.HeightData.SizeX - 1;
                for (int j = 0; j <= num5; j++)
                {
                    double num = Swapper.HeightData.Height[i, j] * Swapper.HeightScale;
                    this.HeightData.Height[i, j] = (long) Math.Round((double) ((((SourceA.HeightData.Height[i, j] * SourceA.HeightScale) * (1.0 - num)) + ((SourceB.HeightData.Height[i, j] * num) * SourceB.HeightScale)) / this.HeightScale));
                }
            }
        }

        public void WaveHigh(clsHeightmap Source)
        {
            sMinMax max = new sMinMax();
            Source.MinMaxGet(ref max);
            long num2 = max.Max - max.Min;
            long min = max.Min;
            if (num2 != 0.0)
            {
                this.SizeCopy(Source);
                int num5 = this.HeightData.SizeY - 1;
                for (int i = 0; i <= num5; i++)
                {
                    int num6 = this.HeightData.SizeX - 1;
                    for (int j = 0; j <= num6; j++)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) ((((Math.Sin((((double) (Source.HeightData.Height[i, j] - min)) / ((double) num2)) * 1.5707963267948966) * num2) + min) * Source.HeightScale) / this.HeightScale));
                    }
                }
            }
        }

        public void WaveLow(clsHeightmap Source)
        {
            sMinMax max = new sMinMax();
            Source.MinMaxGet(ref max);
            long num2 = max.Max - max.Min;
            long min = max.Min;
            if (num2 != 0.0)
            {
                this.SizeCopy(Source);
                int num5 = this.HeightData.SizeY - 1;
                for (int i = 0; i <= num5; i++)
                {
                    int num6 = this.HeightData.SizeX - 1;
                    for (int j = 0; j <= num6; j++)
                    {
                        this.HeightData.Height[i, j] = (long) Math.Round((double) (((((1.0 - Math.Sin((1.0 - (((double) (Source.HeightData.Height[i, j] - min)) / ((double) num2))) * 1.5707963267948966)) * num2) + min) * Source.HeightScale) / this.HeightScale));
                    }
                }
            }
        }

        public class clsHeightData
        {
            public long[,] Height;
            public int SizeX;
            public int SizeY;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sHeights
        {
            public float[] Heights;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sMinMax
        {
            public long Min;
            public long Max;
        }
    }
}

