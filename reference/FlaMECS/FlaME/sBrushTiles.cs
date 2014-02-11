namespace FlaME
{
    using Microsoft.VisualBasic;
    using System;
    using System.Runtime.InteropServices;

    [StructLayout(LayoutKind.Sequential)]
    public struct sBrushTiles
    {
        public int[] XMin;
        public int[] XMax;
        public int YMin;
        public int YMax;
        public double ResultRadius;
        public void CreateCircle(double Radius, double TileSize, bool Alignment)
        {
            int num;
            int num2;
            double num3;
            double num4;
            double num6;
            int num7;
            int num8;
            double number = Radius / TileSize;
            if (Alignment)
            {
                number++;
                num8 = (int) Math.Round((number));
                this.YMin = 0 - num8;
                this.YMax = num8 - 1;
                num2 = this.YMax - this.YMin;
                this.XMin = new int[num2 + 1];
                this.XMax = new int[num2 + 1];
                num6 = number * number;
                int yMax = this.YMax;
                for (num8 = this.YMin; num8 <= yMax; num8++)
                {
                    num4 = num8 + 0.5;
                    num3 = Math.Sqrt(num6 - (num4 * num4)) + 0.5;
                    num = num8 - this.YMin;
                    num7 = (int) Math.Round((num3));
                    this.XMin[num] = 0 - num7;
                    this.XMax[num] = num7 - 1;
                }
            }
            else
            {
                number += 0.125;
                num8 = (int) Math.Round((number));
                this.YMin = 0 - num8;
                this.YMax = num8;
                num2 = this.YMax - this.YMin;
                this.XMin = new int[num2 + 1];
                this.XMax = new int[num2 + 1];
                num6 = number * number;
                int num10 = this.YMax;
                for (num8 = this.YMin; num8 <= num10; num8++)
                {
                    num4 = num8;
                    num3 = Math.Sqrt(num6 - (num4 * num4));
                    num = num8 - this.YMin;
                    num7 = (int) Math.Round((num3));
                    this.XMin[num] = 0 - num7;
                    this.XMax[num] = num7;
                }
            }
            this.ResultRadius = ((double) num2) / 2.0;
        }

        public void CreateSquare(double Radius, double TileSize, bool Alignment)
        {
            int num;
            double number = (Radius / TileSize) + 0.5;
            if (Alignment)
            {
                number += 0.5;
                num = (int) Math.Round((number));
                this.YMin = 0 - num;
                this.YMax = num - 1;
            }
            else
            {
                num = (int) Math.Round((number));
                this.YMin = 0 - num;
                this.YMax = num;
            }
            int num2 = this.YMax - this.YMin;
            this.XMin = new int[num2 + 1];
            this.XMax = new int[num2 + 1];
            int num5 = num2;
            for (int i = 0; i <= num5; i++)
            {
                this.XMin[i] = this.YMin;
                this.XMax[i] = this.YMax;
            }
            this.ResultRadius = ((double) num2) / 2.0;
        }
    }
}

