

using System;
using System.Diagnostics;
using System.Drawing;
using SharpFlame.Bitmaps;
using SharpFlame.Core;
using SharpFlame.Core.Extensions;
using SharpFlame.Maths;
using SharpFlame.Util;



namespace SharpFlame
{
    public class clsHeightmap
    {
        public clsHeightData HeightData = new clsHeightData();
        public double HeightScale = 0.0001D;

        public void Blank(int SizeY, int SizeX)
        {
            HeightData.SizeX = SizeX;
            HeightData.SizeY = SizeY;
            HeightData.Height = new long[SizeY, SizeX];
        }

        public void Randomize(double HeightMultiplier)
        {
            var X = 0;
            var Y = 0;
            long HeightMultiplierHalved = 0;

            HeightMultiplierHalved = Convert.ToInt64(HeightMultiplier / 2.0D);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt64(App.Random.Next() * HeightMultiplier - HeightMultiplierHalved);
                }
            }
        }

        public void GenerateNew(int SizeY, int SizeX, int Inflations, double NoiseFactor, double HeightMultiplier)
        {
            var Temp = new clsHeightmap();

            Blank(SizeY, SizeX);
            Randomize(HeightMultiplier / HeightScale);
            Temp.HeightScale = HeightScale;
            Temp.Generate(this, Inflations, NoiseFactor, HeightMultiplier / HeightScale);
            HeightData = Temp.HeightData; //steal the temporary heightmap's data
        }

        public void Generate(clsHeightmap Source, int Inflations, double NoiseFactor, double HeightMultiplier)
        {
            var Temp = new clsHeightmap();
            var A = 0;

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
            }
        }

        public void Inflate(clsHeightmap Source, double NoiseFactor, double HeightMultiplier, int VariationReduction)
        {
            var A = 0;
            var Y = 0;
            var X = 0;

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
            Dist = MathUtil.RootTwo;
            Variation = Dist * LayerFactor * HeightMultiplier;
            VariationHalved = Convert.ToInt64(Variation / 2.0D);
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y += 2 )
            {
                for ( X = 1; X <= HeightData.SizeX - 2; X += 2 )
                {
                    Mean = Convert.ToInt64((HeightData.Height[Y - 1, X - 1] + HeightData.Height[Y - 1, X + 1] + HeightData.Height[Y + 1, X - 1] + HeightData.Height[Y + 1, X + 1]) / 4.0d);
                    HeightData.Height[Y, X] = Mean + Convert.ToInt64(App.Random.Next() * Variation) - VariationHalved;
                }
            }

            //side points
            Dist = 1.0D;
            Variation = Dist * LayerFactor * HeightMultiplier;
            VariationHalved = Convert.ToInt64(Variation / 2.0D);
            //inner side points
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y++ )
            {
                A = Y - (Y / 2.0D).Floor().ToInt() * 2;
                for ( X = 1 + A; X <= HeightData.SizeX - 2 - A; X += 2 )
                {
                    Mean = Convert.ToInt64((HeightData.Height[Y - 1, X] + HeightData.Height[Y, X - 1] + HeightData.Height[Y, X + 1] + HeightData.Height[Y + 1, X]) / 4.0d);
                    HeightData.Height[Y, X] = Mean + Convert.ToInt64(App.Random.Next() * Variation) - VariationHalved;
                }
            }
            //top side points
            Y = 0;
            for ( X = 1; X <= HeightData.SizeX - 2; X += 2 )
            {
                Mean = Convert.ToInt64((HeightData.Height[Y, X - 1] + HeightData.Height[Y, X + 1] + HeightData.Height[Y + 1, X]) / 3.0d);
                HeightData.Height[Y, X] = Mean + Convert.ToInt64(App.Random.Next() * Variation) - VariationHalved;
            }
            //left side points
            X = 0;
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y += 2 )
            {
                Mean = Convert.ToInt64((HeightData.Height[Y - 1, X] + HeightData.Height[Y, X + 1] + HeightData.Height[Y + 1, X]) / 3.0d);
                HeightData.Height[Y, X] = Mean + Convert.ToInt64(App.Random.Next() * Variation) - VariationHalved;
            }
            //right side points
            X = HeightData.SizeX - 1;
            for ( Y = 1; Y <= HeightData.SizeY - 2; Y += 2 )
            {
                Mean = Convert.ToInt64((HeightData.Height[Y - 1, X] + HeightData.Height[Y, X - 1] + HeightData.Height[Y + 1, X]) / 3.0d);
                HeightData.Height[Y, X] = Mean + Convert.ToInt64(App.Random.Next() * Variation) - VariationHalved;
            }
            //bottom side points
            Y = HeightData.SizeY - 1;
            for ( X = 1; X <= HeightData.SizeX - 2; X += 2 )
            {
                Mean = Convert.ToInt64((HeightData.Height[Y - 1, X] + HeightData.Height[Y, X - 1] + HeightData.Height[Y, X + 1]) / 3.0d);
                HeightData.Height[Y, X] = Mean + Convert.ToInt64(App.Random.Next() * Variation) - VariationHalved;
            }
        }

        public void MinMaxGet(ref sMinMax MinMax_Output)
        {
            long HeightMin = 0;
            long HeightMax = 0;
            long lngTemp = 0;
            var Y = 0;
            var X = 0;

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
            var Y = 0;
            var X = 0;

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
            var Y = 0;
            var X = 0;

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
            var Y = 0;
            var X = 0;
            var dblTemp = Source.HeightScale * Multiplier / HeightScale;

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
            var Y = 0;
            var X = 0;
            var dblTemp = SourceA.HeightScale / (SourceB.HeightScale * HeightScale);

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
            var Y = 0;
            var X = 0;
            var dblTemp = Source.HeightScale / (Denominator * HeightScale);

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
            var Y = 0;
            var X = 0;
            var dblTemp = Source.HeightScale / Interval;
            var dblTemp2 = Interval / HeightScale;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y, X] = Convert.ToInt32(Convert.ToDouble((Source.HeightData.Height[Y, X] * dblTemp)) * dblTemp2);
                }
            }
        }

        public void Add2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            var Y = 0;
            var X = 0;
            var dblTempA = SourceA.HeightScale / HeightScale;
            var dblTempB = SourceB.HeightScale / HeightScale;

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
            var Y = 0;
            var X = 0;

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
            var Y = 0;
            var X = 0;
            var dblTempA = SourceA.HeightScale / HeightScale;
            var dblTempB = SourceB.HeightScale / HeightScale;

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
            var Y = 0;
            var X = 0;

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
            var Y = 0;
            var X = 0;
            double dblTempA = 0;
            double dblTempB = 0;
            var dblTempC = SourceA.HeightScale / HeightScale;
            var dblTempD = SourceB.HeightScale / HeightScale;

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
                        HeightData.Height[Y, X] = dblTempA.ToLong();
                    }
                    else
                    {
                        HeightData.Height[Y, X] = dblTempB.ToLong();
                    }
                }
            }
        }

        public void Highest(clsHeightmap Source, double Value)
        {
            var Y = 0;
            var X = 0;
            var dblTemp = Source.HeightScale / HeightScale;
            var dblTemp2 = Value / HeightScale;
            double dblTemp3 = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTemp3 = Convert.ToDouble(Source.HeightData.Height[Y, X] * dblTemp);
                    if ( dblTemp3 >= dblTemp2 )
                    {
                        HeightData.Height[Y, X] = dblTemp3.ToLong();
                    }
                    else
                    {
                        HeightData.Height[Y, X] = dblTemp2.ToLong();
                    }
                }
            }
        }

        public void Lowest2(clsHeightmap SourceA, clsHeightmap SourceB)
        {
            var Y = 0;
            var X = 0;
            double dblTempA = 0;
            double dblTempB = 0;
            var dblTempC = SourceA.HeightScale / HeightScale;
            var dblTempD = SourceB.HeightScale / HeightScale;

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
                        HeightData.Height[Y, X] = dblTempA.ToLong();
                    }
                    else
                    {
                        HeightData.Height[Y, X] = dblTempB.ToLong();
                    }
                }
            }
        }

        public void Lowest(clsHeightmap Source, double Value)
        {
            var Y = 0;
            var X = 0;
            var dblTemp = Source.HeightScale / HeightScale;
            var dblTemp2 = Value / HeightScale;
            double dblTemp3 = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTemp3 = Convert.ToDouble(Source.HeightData.Height[Y, X] * dblTemp);
                    if ( dblTemp3 <= dblTemp2 )
                    {
                        HeightData.Height[Y, X] = dblTemp3.ToLong();
                    }
                    else
                    {
                        HeightData.Height[Y, X] = dblTemp2.ToLong();
                    }
                }
            }
        }

        public void Swap3(clsHeightmap SourceA, clsHeightmap SourceB, clsHeightmap Swapper)
        {
            var Y = 0;
            var X = 0;
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
            var Y = 0;
            var X = 0;
            double dblTemp = 0;

            SizeCopy(Source);
            for ( Y = 0; Y <= HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= HeightData.SizeX - 1; X++ )
                {
                    dblTemp = Convert.ToDouble(Source.HeightData.Height[Y, X] * Source.HeightScale);
                    if ( dblTemp < HeightMin )
                    {
                        HeightData.Height[Y, X] = (HeightMin / HeightScale).ToLong();
                    }
                    else if ( dblTemp > HeightMax )
                    {
                        HeightData.Height[Y, X] = (HeightMax / HeightScale).ToLong();
                    }
                    else
                    {
                        HeightData.Height[Y, X] = (dblTemp / HeightScale).ToLong();
                    }
                }
            }
        }

        public void Invert(clsHeightmap Source)
        {
            var Y = 0;
            var X = 0;
            var dblTemp = - Source.HeightScale / HeightScale;

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
            var Y = 0;
            var X = 0;
            long HeightRange = 0;
            long HeightMin = 0;
            var MinMax = new sMinMax();

            Source.MinMaxGet(ref MinMax);
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
                    var val =
                        (((1.0d - Math.Sin((1.0d - (Source.HeightData.Height[Y, X] - HeightMin) / (double)HeightRange) * MathUtil.RadOf90Deg)) * HeightRange + HeightMin) *
                         Source.HeightScale / HeightScale).ToLong();

                    HeightData.Height[Y, X] = val;
                }
            }
        }

        public void WaveHigh(clsHeightmap Source)
        {
            var Y = 0;
            var X = 0;
            long HeightRange = 0;
            long HeightMin = 0;
            var MinMax = new sMinMax();

            Source.MinMaxGet(ref MinMax);
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
                    var val =
                        ((Math.Sin((Source.HeightData.Height[Y, X] - HeightMin) / (double)HeightRange * MathUtil.RadOf90Deg) * HeightRange + HeightMin) * Source.HeightScale / HeightScale)
                            .ToLong();

                    HeightData.Height[Y, X] = val;
                }
            }
        }

        public void Rescale(clsHeightmap Source, double HeightMin, double HeightMax)
        {
            var Y = 0;
            var X = 0;
            var MinMax = new sMinMax();

            Source.MinMaxGet(ref MinMax);

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
                lngTemp = (HeightMin / HeightScale).ToLong();
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
                lngTemp = ((HeightMin + HeightMax) / 2.0D).ToLong();
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
            var Y = 0;
            var X = 0;
            var MinMax = new sMinMax();
            var dblTemp = Source.HeightScale / HeightScale;

            Source.MinMaxGet(ref MinMax);

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
            var StartY = 0;
            var StartX = 0;
            var EndY = 0;
            var EndX = 0;
            var Y = 0;
            var X = 0;

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
            var Y = 0;
            var X = 0;

            for ( Y = 0; Y <= Source.HeightData.SizeY - 1; Y++ )
            {
                for ( X = 0; X <= Source.HeightData.SizeX - 1; X++ )
                {
                    HeightData.Height[Y1 + Y, X1 + X] = Source.HeightData.Height[Y, X];
                }
            }
        }

        public SimpleResult Load_Image(string Path)
        {
            var ReturnResult = new SimpleResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            Bitmap HeightmapBitmap = null;
            var Result = new SimpleResult();

            Result = BitmapUtil.LoadBitmap(Path, ref HeightmapBitmap);
            if ( !Result.Success )
            {
                ReturnResult.Problem = Result.Problem;
                return ReturnResult;
            }

            Blank(HeightmapBitmap.Height, HeightmapBitmap.Width);
            var X = 0;
            var Y = 0;
            for ( Y = 0; Y <= HeightmapBitmap.Height - 1; Y++ )
            {
                for ( X = 0; X <= HeightmapBitmap.Width - 1; X++ )
                {
                    var with_1 = HeightmapBitmap.GetPixel(X, Y);
                    HeightData.Height[Y, X] = Convert.ToInt32(((with_1.R) + with_1.G + with_1.B) / (3.0D * HeightScale));
                }
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public void GenerateNewOfSize(int Final_SizeY, int Final_SizeX, float Scale, double HeightMultiplier)
        {
            var Inflations = 0;
            var SizeY = 0;
            var SizeX = 0;
            double Log2 = 0;
            var intTemp = 0;
            var hmTemp = new clsHeightmap();
            double Ratio = 0;

            Log2 = Math.Log(2.0D);
            if ( Final_SizeX > Final_SizeY )
            {
                Inflations = Math.Ceiling(Math.Log(Final_SizeX - 1) / Log2).ToInt();
            }
            else
            {
                Inflations = Math.Ceiling(Math.Log(Final_SizeY - 1) / Log2).ToInt();
            }
            Inflations = Math.Ceiling(Scale).ToInt();
            if ( Inflations < 0 )
            {
                Debugger.Break();
            }
            Ratio = Math.Pow(2.0D, (Scale - Inflations));
            intTemp = Math.Pow(2.0D, Inflations).ToInt();
            SizeX = Math.Ceiling((Final_SizeX / Ratio - 1) / intTemp).ToInt() + 1;
            SizeY = Math.Ceiling((Final_SizeY / Ratio - 1) / intTemp).ToInt() + 1;

            GenerateNew(SizeY, SizeX, Inflations, 1.0D, HeightMultiplier);
            if ( Inflations > Scale )
            {
                hmTemp.Stretch(this, (HeightData.SizeX * Ratio).ToInt(), (HeightData.SizeY * Ratio).ToInt());
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
            var OldSizeX = 0;
            var OldSizeY = 0;
            float New_Per_OldX = 0;
            float New_Per_OldY = 0;
            float OldPixStartX = 0;
            float OldPixStartY = 0;
            float OldPixEndX = 0;
            float OldPixEndY = 0;
            float Ratio = 0;
            var NewPixelX = 0;
            var NewPixelY = 0;
            var OldPixelX = 0;
            var OldPixelY = 0;
            float XTemp = 0;
            float YTemp = 0;
            var Temp = (float)(hmSource.HeightScale / HeightScale);

            OldSizeX = hmSource.HeightData.SizeX;
            OldSizeY = hmSource.HeightData.SizeY;

            Blank(SizeY, SizeX);
            //new ratios convert original image positions into new image positions
            New_Per_OldX = (float)((double)SizeX / OldSizeX);
            New_Per_OldY = (float)((double)SizeY / OldSizeY);
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
                    for ( NewPixelY = Math.Floor(OldPixStartY).ToInt(); NewPixelY <= -Math.Floor(-OldPixEndY).ToInt(); NewPixelY++ )
                    {
                        if ( NewPixelY >= SizeY )
                        {
                            break;
                        }
                        for ( NewPixelX = Math.Floor(OldPixStartX).ToInt(); NewPixelX <= -Math.Floor(-OldPixEndX).ToInt(); NewPixelX++ )
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

        public void FadeMultiple(clsHeightmap hmSource, ref sHeightmaps AlterationMaps, ref sHeights AlterationHeights)
        {
            var Level = 0;
            var Y = 0;
            var X = 0;
            float srcHeight = 0;
            float Ratio = 0;
            var AlterationHeight_Ubound = AlterationHeights.Heights.GetUpperBound(0);
            var intTemp = 0;
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
                        Ratio = (srcHeight - TempA) / (TempB - TempA);
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

        public class clsHeightData
        {
            public long[,] Height;
            public int SizeX;
            public int SizeY;
        }

        public struct sHeights
        {
            public float[] Heights;
        }

        public struct sMinMax
        {
            public long Max;
            public long Min;
        }
    }

    public struct sHeightmaps
    {
        public clsHeightmap[] Heightmaps;
    }
}