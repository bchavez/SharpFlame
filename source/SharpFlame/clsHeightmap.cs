using System;
using System.Diagnostics;
using System.Drawing;
using Microsoft.VisualBasic;

namespace SharpFlame
{
    public class clsHeightmap
    {
        public double HeightScale = 0.0001D;

        public struct sMinMax
        {
            public long Min;
            public long Max;
        }

        public class clsHeightData
        {
            public int SizeX;
            public int SizeY;
            public long[,] Height;
        }

        public clsHeightData HeightData = new clsHeightData();

        public void Blank(int SizeY, int SizeX)
        {
            HeightData.SizeX = SizeX;
            HeightData.SizeY = SizeY;
            HeightData.Height = new long[SizeY, SizeX];
        }

        public void Randomize(double HeightMultiplier)
        {
            int X = 0;
            int Y = 0;
            long HeightMultiplierHalved = 0;

            HeightMultiplierHalved = (int)(HeightMultiplier / 2.0D);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = (int)(VBMath.Rnd() * HeightMultiplier - HeightMultiplierHalved);
                }
            }
        }

        public void GenerateNew(int SizeY, int SizeX, int Inflations, double NoiseFactor, double HeightMultiplier)
        {
            clsHeightmap Temp = new clsHeightmap();

            Blank(SizeY, SizeX);
            Randomize(HeightMultiplier / HeightScale);
            Temp.HeightScale = HeightScale;
            Temp.Generate(this, Inflations, NoiseFactor, HeightMultiplier / HeightScale);
            HeightData = Temp.HeightData; //steal the temporary heightmap's data
        }

        public void Generate(clsHeightmap Source, int Inflations, double NoiseFactor, double HeightMultiplier)
        {
            clsHeightmap Temp = new clsHeightmap();
            int A = 0;

            if ( Inflations >= 1 )
            {
                Temp.Inflate(Source, NoiseFactor, HeightMultiplier, 1);
                HeightData = Temp.HeightData;
                Temp.HeightData = new clsHeightData();
                for ( A = 2; A <= Inflations; A++ )
                {
                    Temp.Inflate(this, NoiseFactor, HeightMultiplier, A);
                    HeightData = Temp.HeightData;
                    Temp.HeightData = new clsHeightData();
                }
            }
            else if ( Inflations == 0 )
            {
                Copy(Source);
            }
            else
            {
                return;
            }
        }

        public void Inflate(clsHeightmap Source, double NoiseFactor, double HeightMultiplier, int VariationReduction)
        {
            int A = 0;
            int Y = 0;
            int X = 0;

            double Variation = 0;
            long VariationHalved = 0;
            long Mean = 0;
            double Dist = 0;
            double LayerFactor = 0;

            //make a larger copy of heightmap
            if ( Source.HeightData.SizeY == 0 | Source.HeightData.SizeX == 0 )
            {
                return;
            }
            Blank((Source.HeightData.SizeY - 1) * 2 + 1, Convert.ToInt32((Source.HeightData.SizeX - 1) * 2 + 1));
            for ( Y = 0; Y <= Source.HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= Source.HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[(Y + 1) * 2 - 2, (X + 1) * 2 - 2] = Source.HeightData.Height[Y, X];
                }
            }

            if ( NoiseFactor == 0.0D )
            {
                LayerFactor = 0.0D;
            }
            else
            {
                LayerFactor = Math.Pow((2.0D / NoiseFactor), (- VariationReduction));
            }

            //centre points
            Dist = modMath.RootTwo;
            Variation = Dist * LayerFactor * HeightMultiplier;
            VariationHalved = (int)(Variation / 2.0D);
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y += 2 )
            {
                for ( X = 1; X <= HeightData.SizeX - 2; X += 2 )
                {
                    Mean =
                        Convert.ToInt32(
                            Convert.ToDouble(Convert.ToInt32(HeightData.Height[Y - 1, X - 1] + HeightData.Height[Y - 1, X + 1]) +
                                                    HeightData.Height[Y + 1, X - 1] + HeightData.Height[Y + 1, X + 1]) / 4.0D);
                    HeightData.Height[Y, X] = Convert.ToInt64(Convert.ToInt32(Mean + ((int)(VBMath.Rnd() * Variation))) - VariationHalved);
                }
            }

            //side points
            Dist = 1.0D;
            Variation = Dist * LayerFactor * HeightMultiplier;
            VariationHalved = (int)(Variation / 2.0D);
            //inner side points
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y++ )
            {
                A = Y - ((int)(Conversion.Int(Y / 2.0D))) * 2;
                for ( X = 1 + A; X <= HeightData.SizeX - 2 - A; X += 2 )
                {
                    Mean =
                        Convert.ToInt32(
                            Convert.ToDouble(Convert.ToInt32(HeightData.Height[Y - 1, X] + HeightData.Height[Y, X - 1]) + HeightData.Height[Y, X + 1] +
                                                    HeightData.Height[Y + 1, X]) / 4.0D);
                    HeightData.Height[Y, X] = Convert.ToInt64(Convert.ToInt32(Mean + ((int)(VBMath.Rnd() * Variation))) - VariationHalved);
                }
            }
            //top side points
            Y = 0;
            for ( X = 1; X <= HeightData.SizeX - 2; X += 2 )
            {
                Mean =
                    Convert.ToInt32(
                        Convert.ToDouble(Convert.ToInt32(HeightData.Height[Y, X - 1] + HeightData.Height[Y, X + 1]) + HeightData.Height[Y + 1, X]) /
                        3.0D);
                HeightData.Height[Y, X] = Convert.ToInt64(Convert.ToInt32(Mean + ((int)(VBMath.Rnd() * Variation))) - VariationHalved);
            }
            //left side points
            X = 0;
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y += 2 )
            {
                Mean =
                    Convert.ToInt32(
                        Convert.ToDouble(Convert.ToInt32(HeightData.Height[Y - 1, X] + HeightData.Height[Y, X + 1]) + HeightData.Height[Y + 1, X]) /
                        3.0D);
                HeightData.Height[Y, X] = Convert.ToInt64(Convert.ToInt32(Mean + ((int)(VBMath.Rnd() * Variation))) - VariationHalved);
            }
            //right side points
            X = HeightData.SizeX - 1;
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y += 2 )
            {
                Mean =
                    Convert.ToInt32(
                        Convert.ToDouble(Convert.ToInt32(HeightData.Height[Y - 1, X] + HeightData.Height[Y, X - 1]) + HeightData.Height[Y + 1, X]) /
                        3.0D);
                HeightData.Height[Y, X] = Convert.ToInt64(Convert.ToInt32(Mean + ((int)(VBMath.Rnd() * Variation))) - VariationHalved);
            }
            //bottom side points
            Y = HeightData.SizeY - 1;
            for ( X = 1; X <= HeightData.SizeX - 2; X += 2 )
            {
                Mean =
                    Convert.ToInt32(
                        Convert.ToDouble(Convert.ToInt32(HeightData.Height[Y - 1, X] + HeightData.Height[Y, X - 1]) + HeightData.Height[Y, X + 1]) /
                        3.0D);
                HeightData.Height[Y, X] = Convert.ToInt64(Convert.ToInt32(Mean + ((int)(VBMath.Rnd() * Variation))) - VariationHalved);
            }
        }

        public void MinMaxGet(sMinMax MinMax_Output)
        {
            long HeightMin = 0;
            long HeightMax = 0;
            long lngTemp = 0;
            int Y = 0;
            int X = 0;

            if ( !(HeightData.SizeY == 0 | HeightData.SizeX == 0) )
            {
                HeightMin = HeightData.Height[0, 0];
                HeightMax = HeightData.Height[0, 0];
                for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
                {
                    for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                    {
                        lngTemp = HeightData.Height[Y, X];
                        if ( lngTemp < HeightMin )
                        {
                            HeightMin = lngTemp;
                        }
                        if ( lngTemp > HeightMax )
                        {
                            HeightMax = lngTemp;
                        }
                    }
                }
            }
            MinMax_Output.Min = HeightMin;
            MinMax_Output.Max = HeightMax;
        }

        public void Copy(clsHeightmap Source)
        {
            int Y = 0;
            int X = 0;

            HeightScale = Source.HeightScale;
            SizeCopy(Source);
            for ( Y = 0; Y <= Source.HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= Source.HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Source.HeightData.Height[Y, X];
                }
            }
        }

        public bool IsSizeSame(clsHeightmap Source)
        {
            return (HeightData.SizeX == Source.HeightData.SizeX) && (HeightData.SizeY == Source.HeightData.SizeY);
        }

        public void Multiply2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            int Y = 0;
            int X = 0;

            if ( !SourceA.IsSizeSame(SourceB) )
            {
                Debugger.Break();
            }
            SizeCopy(SourceA);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(
                            Convert.ToInt32(
                                Convert.ToDouble(Convert.ToDouble(SourceA.HeightData.Height[Y, X] * SourceA.HeightScale) *
                                                        SourceB.HeightData.Height[Y, X]) * SourceB.HeightScale) / HeightScale);
                }
            }
        }

        public void Multiply(clsHeightmap Source, double Multiplier)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = Source.HeightScale * Multiplier / HeightScale;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt32(Source.HeightData.Height[Y, X] * dblTemp);
                }
            }
        }

        public void Divide2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = SourceA.HeightScale / (SourceB.HeightScale * HeightScale);

            if ( !SourceA.IsSizeSame(SourceB) )
            {
                Debugger.Break();
            }
            SizeCopy(SourceA);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(Convert.ToDouble(SourceA.HeightData.Height[Y, X] / SourceB.HeightData.Height[Y, X]) * dblTemp);
                }
            }
        }

        public void Divide(clsHeightmap Source, double Denominator)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = Source.HeightScale / (Denominator * HeightScale);

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt32(Source.HeightData.Height[Y, X] * dblTemp);
                }
            }
        }

        public void Intervalise(clsHeightmap Source, double Interval)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = Source.HeightScale / Interval;
            double dblTemp2 = Interval / HeightScale;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt32(Convert.ToDouble(Conversion.Int(Source.HeightData.Height[Y, X] * dblTemp)) * dblTemp2);
                }
            }
        }

        public void Add2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            int Y = 0;
            int X = 0;
            double dblTempA = SourceA.HeightScale / HeightScale;
            double dblTempB = SourceB.HeightScale / HeightScale;

            if ( !SourceA.IsSizeSame(SourceB) )
            {
                Debugger.Break();
            }
            SizeCopy(SourceA);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(Convert.ToDouble(SourceA.HeightData.Height[Y, X] * dblTempA) +
                                               Convert.ToDouble(SourceB.HeightData.Height[Y, X] * dblTempB));
                }
            }
        }

        public void Add(clsHeightmap Source, double Amount)
        {
            int Y = 0;
            int X = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Source.HeightData.Height[Y, X] * Source.HeightScale) + Amount) /
                                               HeightScale);
                }
            }
        }

        public void Subtract2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            int Y = 0;
            int X = 0;
            double dblTempA = SourceA.HeightScale / HeightScale;
            double dblTempB = SourceB.HeightScale / HeightScale;

            if ( !SourceA.IsSizeSame(SourceB) )
            {
                Debugger.Break();
            }
            SizeCopy(SourceA);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(Convert.ToDouble(SourceA.HeightData.Height[Y, X] * dblTempA) -
                                               Convert.ToDouble(SourceB.HeightData.Height[Y, X] * dblTempB));
                }
            }
        }

        public void Subtract(clsHeightmap Source, double Amount)
        {
            int Y = 0;
            int X = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(Convert.ToDouble(Convert.ToDouble(Source.HeightData.Height[Y, X] * Source.HeightScale) - Amount) /
                                               HeightScale);
                }
            }
        }

        public void Highest2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            int Y = 0;
            int X = 0;
            double dblTempA = 0;
            double dblTempB = 0;
            double dblTempC = SourceA.HeightScale / HeightScale;
            double dblTempD = SourceB.HeightScale / HeightScale;

            if ( !SourceA.IsSizeSame(SourceB) )
            {
                Debugger.Break();
            }
            SizeCopy(SourceA);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTempA = Convert.ToDouble(SourceA.HeightData.Height[Y, X] * dblTempC);
                    dblTempB = Convert.ToDouble(SourceB.HeightData.Height[Y, X] * dblTempD);
                    if ( dblTempA >= dblTempB )
                    {
                        HeightData.Height[Y, X] = (int)dblTempA;
                    }
                    else
                    {
                        HeightData.Height[Y, X] = (int)dblTempB;
                    }
                }
            }
        }

        public void Highest(clsHeightmap Source, double Value)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = Source.HeightScale / HeightScale;
            double dblTemp2 = Value / HeightScale;
            double dblTemp3 = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTemp3 = Convert.ToDouble(Source.HeightData.Height[Y, X] * dblTemp);
                    if ( dblTemp3 >= dblTemp2 )
                    {
                        HeightData.Height[Y, X] = (int)dblTemp3;
                    }
                    else
                    {
                        HeightData.Height[Y, X] = (int)dblTemp2;
                    }
                }
            }
        }

        public void Lowest2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            int Y = 0;
            int X = 0;
            double dblTempA = 0;
            double dblTempB = 0;
            double dblTempC = SourceA.HeightScale / HeightScale;
            double dblTempD = SourceB.HeightScale / HeightScale;

            if ( !SourceA.IsSizeSame(SourceB) )
            {
                Debugger.Break();
            }
            SizeCopy(SourceA);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTempA = Convert.ToDouble(SourceA.HeightData.Height[Y, X] * dblTempC);
                    dblTempB = Convert.ToDouble(SourceB.HeightData.Height[Y, X] * dblTempD);
                    if ( dblTempA <= dblTempB )
                    {
                        HeightData.Height[Y, X] = (int)dblTempA;
                    }
                    else
                    {
                        HeightData.Height[Y, X] = (int)dblTempB;
                    }
                }
            }
        }

        public void Lowest(clsHeightmap Source, double Value)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = Source.HeightScale / HeightScale;
            double dblTemp2 = Value / HeightScale;
            double dblTemp3 = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTemp3 = Convert.ToDouble(Source.HeightData.Height[Y, X] * dblTemp);
                    if ( dblTemp3 <= dblTemp2 )
                    {
                        HeightData.Height[Y, X] = (int)dblTemp3;
                    }
                    else
                    {
                        HeightData.Height[Y, X] = (int)dblTemp2;
                    }
                }
            }
        }

        public void Swap3(clsHeightmap SourceA, clsHeightmap SourceB, clsHeightmap Swapper)
        {
            int Y = 0;
            int X = 0;
            double Ratio = 0;

            if ( !(Swapper.IsSizeSame(SourceA) && Swapper.IsSizeSame(SourceB)) )
            {
                Debugger.Break();
            }
            SizeCopy(Swapper);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    Ratio = Convert.ToDouble(Swapper.HeightData.Height[Y, X] * Swapper.HeightScale);
                    HeightData.Height[Y, X] =
                        Convert.ToInt32(
                            Convert.ToDouble(Convert.ToDouble(SourceA.HeightData.Height[Y, X] * SourceA.HeightScale) * (1.0D - Ratio) +
                                                    Convert.ToDouble(SourceB.HeightData.Height[Y, X] * Ratio * SourceB.HeightScale)) / HeightScale);
                }
            }
        }

        public void Clamp(clsHeightmap Source, double HeightMin, double HeightMax)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTemp = Convert.ToDouble(Source.HeightData.Height[Y, X] * Source.HeightScale);
                    if ( dblTemp < HeightMin )
                    {
                        HeightData.Height[Y, X] = (int)(HeightMin / HeightScale);
                    }
                    else if ( dblTemp > HeightMax )
                    {
                        HeightData.Height[Y, X] = (int)(HeightMax / HeightScale);
                    }
                    else
                    {
                        HeightData.Height[Y, X] = (int)(dblTemp / HeightScale);
                    }
                }
            }
        }

        public void Invert(clsHeightmap Source)
        {
            int Y = 0;
            int X = 0;
            double dblTemp = - Source.HeightScale / HeightScale;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt32(Source.HeightData.Height[Y, X] * dblTemp);
                }
            }
        }

        public void WaveLow(clsHeightmap Source)
        {
            int Y = 0;
            int X = 0;
            long HeightRange = 0;
            long HeightMin = 0;
            sMinMax MinMax = new sMinMax();

            Source.MinMaxGet(MinMax);
            HeightRange = Convert.ToInt64(MinMax.Max - MinMax.Min);
            HeightMin = MinMax.Min;

            if ( HeightRange == 0.0D )
            {
                return;
            }

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        (int)
                            (((1.0D - Math.Sin((1.0D - Convert.ToInt32(Source.HeightData.Height[Y, X] - HeightMin) / HeightRange) * modMath.RadOf90Deg)) *
                              HeightRange + HeightMin) * Source.HeightScale / HeightScale);
                }
            }
        }

        public void WaveHigh(clsHeightmap Source)
        {
            int Y = 0;
            int X = 0;
            long HeightRange = 0;
            long HeightMin = 0;
            sMinMax MinMax = new sMinMax();

            Source.MinMaxGet(MinMax);
            HeightRange = Convert.ToInt64(MinMax.Max - MinMax.Min);
            HeightMin = MinMax.Min;

            if ( HeightRange == 0.0D )
            {
                return;
            }

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] =
                        (int)
                            ((Math.Sin(
                                Convert.ToDouble(Convert.ToInt32(Convert.ToDouble(Source.HeightData.Height[Y, X] - HeightMin) / HeightRange) *
                                                        modMath.RadOf90Deg)) * HeightRange + HeightMin) * Source.HeightScale / HeightScale);
                }
            }
        }

        public void Rescale(clsHeightmap Source, double HeightMin, double HeightMax)
        {
            int Y = 0;
            int X = 0;
            sMinMax MinMax = new sMinMax();

            Source.MinMaxGet(MinMax);

            long HeightRange = 0;
            long Offset = 0;
            double Ratio = 0;
            long lngTemp = 0;

            SizeCopy(Source);
            HeightRange = Convert.ToInt64(MinMax.Max - MinMax.Min);
            Offset = Convert.ToInt64(0 - MinMax.Min);
            if ( HeightRange > 0 )
            {
                Ratio = (HeightMax - HeightMin) / (HeightRange * HeightScale);
                lngTemp = (int)(HeightMin / HeightScale);
                for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
                {
                    for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                    {
                        HeightData.Height[Y, X] =
                            Convert.ToInt64(lngTemp + Convert.ToInt32(Convert.ToDouble(Offset + Source.HeightData.Height[Y, X]) * Ratio));
                    }
                }
            }
            else
            {
                lngTemp = (int)((HeightMin + HeightMax) / 2.0D);
                for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
                {
                    for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                    {
                        HeightData.Height[Y, X] = lngTemp;
                    }
                }
            }
        }

        public void ShiftToZero(clsHeightmap Source)
        {
            int Y = 0;
            int X = 0;
            sMinMax MinMax = new sMinMax();
            double dblTemp = Source.HeightScale / HeightScale;

            Source.MinMaxGet(MinMax);

            long Offset = 0;
            SizeCopy(Source);
            Offset = Convert.ToInt64(0 - MinMax.Min);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt32(Convert.ToDouble(Offset + Source.HeightData.Height[Y, X]) * dblTemp);
                }
            }
        }

        public void Resize(clsHeightmap Source, int OffsetY, int OffsetX, int SizeY, int SizeX)
        {
            int StartY = 0;
            int StartX = 0;
            int EndY = 0;
            int EndX = 0;
            int Y = 0;
            int X = 0;

            Blank(SizeY, SizeX);
            StartX = Math.Max(0 - OffsetX, 0);
            StartY = Math.Max(0 - OffsetY, 0);
            EndX = Math.Min(Source.HeightData.SizeX - OffsetX, HeightData.SizeX) - 1;
            EndY = Math.Min(Source.HeightData.SizeY - OffsetY, HeightData.SizeY) - 1;
            for ( Y = StartY; Y <= EndY; Y++ )
            {
                for ( X = StartX; X <= EndX; X++ )
                {
                    HeightData.Height[Y, X] = Source.HeightData.Height[OffsetY + Y, OffsetX + X];
                }
            }
        }

        public void SizeCopy(clsHeightmap Source)
        {
            HeightData.SizeX = Source.HeightData.SizeX;
            HeightData.SizeY = Source.HeightData.SizeY;
            HeightData.Height = new long[HeightData.SizeY, HeightData.SizeX];
        }

        public void Insert(clsHeightmap Source, int Y1, int X1)
        {
            int Y = 0;
            int X = 0;

            for ( Y = 0; Y <= Source.HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= Source.HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y1 + Y, X1 + X] = Source.HeightData.Height[Y, X];
                }
            }
        }

        public modProgram.sResult Load_Image(string Path)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            Bitmap HeightmapBitmap = null;
            modProgram.sResult Result = new modProgram.sResult();

            Result = modBitmap.LoadBitmap(Path, ref HeightmapBitmap);
            if ( !Result.Success )
            {
                ReturnResult.Problem = Result.Problem;
                return ReturnResult;
            }

            Blank(HeightmapBitmap.Height, HeightmapBitmap.Width);
            int X = 0;
            int Y = 0;
            for ( Y = 0; Y <= HeightmapBitmap.Height - 1; Y++ )
            {
                for ( X = 0; X <= HeightmapBitmap.Width - 1; X++ )
                {
                    Color with_1 = HeightmapBitmap.GetPixel(X, Y);
                    HeightData.Height[Y, X] = Convert.ToInt32(((with_1.R) + with_1.G + with_1.B) / (3.0D * HeightScale));
                }
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public void GenerateNewOfSize(int Final_SizeY, int Final_SizeX, float Scale, double HeightMultiplier)
        {
            int Inflations = 0;
            int SizeY = 0;
            int SizeX = 0;
            double Log2 = 0;
            int intTemp = 0;
            clsHeightmap hmTemp = new clsHeightmap();
            double Ratio = 0;

            Log2 = Math.Log(2.0D);
            if ( Final_SizeX > Final_SizeY )
            {
                Inflations = (int)(Math.Ceiling(Math.Log(Final_SizeX - 1) / Log2));
            }
            else
            {
                Inflations = (int)(Math.Ceiling(Math.Log(Final_SizeY - 1) / Log2));
            }
            Inflations = (int)(Math.Ceiling(Scale));
            if ( Inflations < 0 )
            {
                Debugger.Break();
            }
            Ratio = Math.Pow(2.0D, (Scale - Inflations));
            intTemp = (int)(Math.Pow(2.0D, Inflations));
            SizeX = ((int)(Math.Ceiling((Final_SizeX / Ratio - 1) / intTemp))) + 1;
            SizeY = ((int)(Math.Ceiling((Final_SizeY / Ratio - 1) / intTemp))) + 1;

            GenerateNew(SizeY, SizeX, Inflations, 1.0D, HeightMultiplier);
            if ( Inflations > Scale )
            {
                hmTemp.Stretch(this, (int)(HeightData.SizeX * Ratio), (int)(HeightData.SizeY * Ratio));
                HeightData = hmTemp.HeightData;
                hmTemp.HeightData = new clsHeightData();
            }
            if ( HeightData.SizeX != Final_SizeX | HeightData.SizeY != Final_SizeY )
            {
                //If HeightData.SizeX / Final_SizeX > HeightData.SizeY / Final_SizeY Then
                //    hmTemp.Resize(Me, 0, 0, HeightData.SizeY, Final_SizeX * HeightData.SizeY / Final_SizeY)
                //Else
                //    hmTemp.Resize(Me, 0, 0, Final_SizeY * HeightData.SizeX / Final_SizeX, HeightData.SizeX)
                //End If
                //StretchPixelated(hmTemp, Final_SizeX, Final_SizeY)
                hmTemp.Resize(this, 0, 0, Final_SizeY, Final_SizeX);
                HeightData = hmTemp.HeightData;
            }
        }

        public void Stretch(clsHeightmap hmSource, int SizeX, int SizeY)
        {
            int OldSizeX = 0;
            int OldSizeY = 0;
            float New_Per_OldX = 0;
            float New_Per_OldY = 0;
            float OldPixStartX = 0;
            float OldPixStartY = 0;
            float OldPixEndX = 0;
            float OldPixEndY = 0;
            float Ratio = 0;
            int NewPixelX = 0;
            int NewPixelY = 0;
            int OldPixelX = 0;
            int OldPixelY = 0;
            float XTemp = 0;
            float YTemp = 0;
            float Temp = (float)(hmSource.HeightScale / HeightScale);

            OldSizeX = hmSource.HeightData.SizeX;
            OldSizeY = hmSource.HeightData.SizeY;

            Blank(SizeY, SizeX);
            //new ratios convert original image positions into new image positions
            New_Per_OldX = (float)(SizeX / OldSizeX);
            New_Per_OldY = (float)(SizeY / OldSizeY);
            //cycles through each pixel in the new image
            for ( OldPixelY = 0; OldPixelY <= OldSizeY - 1; OldPixelY++ )
            {
                for ( OldPixelX = 0; OldPixelX <= OldSizeX - 1; OldPixelX++ )
                {
                    //find where the old pixel goes on the new image
                    OldPixStartX = OldPixelX * New_Per_OldX;
                    OldPixStartY = OldPixelY * New_Per_OldY;
                    OldPixEndX = (OldPixelX + 1) * New_Per_OldX;
                    OldPixEndY = (OldPixelY + 1) * New_Per_OldY;
                    //cycles through each new image pixel that is to be influenced
                    for ( NewPixelY = (int)OldPixStartY; NewPixelY <= (int)(-(-OldPixEndY)); NewPixelY++ )
                    {
                        if ( NewPixelY >= SizeY )
                        {
                            break;
                        }
                        for ( NewPixelX = (int)OldPixStartX; NewPixelX <= (int)(-(-OldPixEndX)); NewPixelX++ )
                        {
                            if ( NewPixelX >= SizeX )
                            {
                                break;
                            }
                            //ensure that the original pixel imposes on the new pixel
                            if ( !(OldPixEndY > NewPixelY & OldPixStartY < NewPixelY + 1 & OldPixEndX > NewPixelX & OldPixStartX < NewPixelX + 1) )
                            {
                                //Stop
                            }
                            else
                            {
                                //measure the amount of original pixel in the new pixel
                                XTemp = 1.0F;
                                YTemp = 1.0F;
                                if ( OldPixStartY > NewPixelY )
                                {
                                    YTemp -= OldPixStartY - NewPixelY;
                                }
                                if ( OldPixStartX > NewPixelX )
                                {
                                    XTemp -= OldPixStartX - NewPixelX;
                                }
                                if ( OldPixEndY < NewPixelY + 1 )
                                {
                                    YTemp -= (NewPixelY + 1) - OldPixEndY;
                                }
                                if ( OldPixEndX < NewPixelX + 1 )
                                {
                                    XTemp -= (NewPixelX + 1) - OldPixEndX;
                                }
                                Ratio = XTemp * YTemp;
                                //add the neccessary fraction of the original pixel's color into the new pixel
                                HeightData.Height[NewPixelY, NewPixelX] =
                                    Convert.ToInt32(HeightData.Height[NewPixelY, NewPixelX] +
                                                           Convert.ToInt32(hmSource.HeightData.Height[OldPixelY, OldPixelX] * Ratio * Temp));
                            }
                        }
                    }
                }
            }
        }

        public struct sHeights
        {
            public float[] Heights;
        }

        public void FadeMultiple(clsHeightmap hmSource, sHeightmaps AlterationMaps, sHeights AlterationHeights)
        {
            int Level = 0;
            int Y = 0;
            int X = 0;
            float srcHeight = 0;
            float Ratio = 0;
            int AlterationHeight_Ubound = AlterationHeights.Heights.GetUpperBound(0);
            int intTemp = 0;
            float TempA = 0;
            float TempB = 0;

            SizeCopy(hmSource);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    srcHeight = Convert.ToSingle(hmSource.HeightData.Height[Y, X] * hmSource.HeightScale);
                    for ( Level = 0; Level <= AlterationHeight_Ubound; Level++ )
                    {
                        if ( srcHeight <= AlterationHeights.Heights[Level] )
                        {
                            break;
                        }
                    }
                    if ( Level == 0 )
                    {
                        HeightData.Height[Y, X] =
                            Convert.ToInt32(
                                Convert.ToDouble(AlterationMaps.Heightmaps[Level].HeightData.Height[Y, X] * AlterationMaps.Heightmaps[Level].HeightScale) /
                                HeightScale);
                    }
                    else if ( Level > AlterationHeight_Ubound )
                    {
                        HeightData.Height[Y, X] =
                            Convert.ToInt32(
                                Convert.ToDouble(AlterationMaps.Heightmaps[AlterationHeight_Ubound].HeightData.Height[Y, X] *
                                                        AlterationMaps.Heightmaps[AlterationHeight_Ubound].HeightScale) / HeightScale);
                    }
                    else
                    {
                        intTemp = Level - 1;
                        TempA = AlterationHeights.Heights[intTemp];
                        TempB = AlterationHeights.Heights[Level];
                        Ratio = (float)((srcHeight - TempA) / (TempB - TempA));
                        HeightData.Height[Y, X] =
                            Convert.ToInt32(
                                Convert.ToDouble(
                                    Convert.ToDouble(AlterationMaps.Heightmaps[intTemp].HeightData.Height[Y, X] * AlterationMaps.Heightmaps[intTemp].HeightScale *
                                                            (1.0F - Ratio)) +
                                    Convert.ToDouble(AlterationMaps.Heightmaps[Level].HeightData.Height[Y, X] * AlterationMaps.Heightmaps[Level].HeightScale *
                                                            Ratio)) / HeightScale);
                    }
                }
            }
        }
    }

    public struct sHeightmaps
    {
        public clsHeightmap[] Heightmaps;
    }
}