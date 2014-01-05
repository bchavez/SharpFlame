namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class clsModel
    {
        public int ConnectorCount;
        public modMath.sXYZ_sng[] Connectors = new modMath.sXYZ_sng[0];
        public int GLTextureNum;
        public int QuadCount;
        public sQuad[] Quads;
        public int TriangleCount;
        public sTriangle[] Triangles;

        public void GLDraw()
        {
            int num;
            GL.BindTexture(TextureTarget.Texture2D, this.GLTextureNum);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, 0x2100);
            GL.Begin(BeginMode.Triangles);
            int num2 = this.TriangleCount - 1;
            for (num = 0; num <= num2; num++)
            {
                sTriangle[] triangles = this.Triangles;
                int index = num;
                GL.TexCoord2(triangles[index].TexCoordA.X, triangles[index].TexCoordA.Y);
                GL.Vertex3(triangles[index].PosA.X, triangles[index].PosA.Y, -triangles[index].PosA.Z);
                GL.TexCoord2(triangles[index].TexCoordB.X, triangles[index].TexCoordB.Y);
                GL.Vertex3(triangles[index].PosB.X, triangles[index].PosB.Y, -triangles[index].PosB.Z);
                GL.TexCoord2(triangles[index].TexCoordC.X, triangles[index].TexCoordC.Y);
                GL.Vertex3(triangles[index].PosC.X, triangles[index].PosC.Y, -triangles[index].PosC.Z);
            }
            GL.End();
            GL.Begin(BeginMode.Quads);
            int num4 = this.QuadCount - 1;
            for (num = 0; num <= num4; num++)
            {
                sQuad[] quads = this.Quads;
                int num5 = num;
                GL.TexCoord2(quads[num5].TexCoordA.X, quads[num5].TexCoordA.Y);
                GL.Vertex3(quads[num5].PosA.X, quads[num5].PosA.Y, -quads[num5].PosA.Z);
                GL.TexCoord2(quads[num5].TexCoordB.X, quads[num5].TexCoordB.Y);
                GL.Vertex3(quads[num5].PosB.X, quads[num5].PosB.Y, -quads[num5].PosB.Z);
                GL.TexCoord2(quads[num5].TexCoordC.X, quads[num5].TexCoordC.Y);
                GL.Vertex3(quads[num5].PosC.X, quads[num5].PosC.Y, -quads[num5].PosC.Z);
                GL.TexCoord2(quads[num5].TexCoordD.X, quads[num5].TexCoordD.Y);
                GL.Vertex3(quads[num5].PosD.X, quads[num5].PosD.Y, -quads[num5].PosD.Z);
            }
            GL.End();
        }

        public int GLList_Create()
        {
            int list = GL.GenLists(1);
            if (list != 0)
            {
                GL.NewList(list, ListMode.Compile);
                this.GLDraw();
                GL.EndList();
            }
            return list;
        }

        public clsResult ReadPIE(StreamReader File, clsObjectData Owner)
        {
            int num;
            int num2;
            int num3;
            bool flag;
            int num6;
            int num8;
            int num9;
            int num10;
            clsResult result;
            string[] strArray;
            string str;
            string str2;
            string[] strArray2;
            int num12;
            clsResult result2 = new clsResult("Reading PIE");
            string stringCheck = "";
            sPIELevel[] levelArray = new sPIELevel[0];
            int index = -1;
        Label_001E:
            str = File.ReadLine();
            if (str == null)
            {
                goto Label_0BD9;
            }
        Label_002F:
            if (Strings.Left(str, 3) == "PIE")
            {
                num10 = Conversions.ToInteger(Strings.Right(str, str.Length - 4));
                if ((num10 != 2) & (num10 != 3))
                {
                    result2.ProblemAdd("Version is unknown.");
                    return result2;
                }
                goto Label_001E;
            }
            if (Strings.Left(str, 4) == "TYPE")
            {
                goto Label_001E;
            }
            if (Strings.Left(str, 7) == "TEXTURE")
            {
                stringCheck = Strings.Right(str, str.Length - 10);
                num = Strings.InStrRev(stringCheck, " ", -1, CompareMethod.Binary);
                if (num > 0)
                {
                    num = Strings.InStrRev(stringCheck, " ", num - 1, CompareMethod.Binary);
                }
                else
                {
                    result2.ProblemAdd("Bad texture name.");
                    return result2;
                }
                if (num <= 0)
                {
                    result2.ProblemAdd("Bad texture name.");
                    return result2;
                }
                stringCheck = Strings.Left(stringCheck, num - 1);
                goto Label_001E;
            }
            if (Strings.Left(str, 6) == "LEVELS")
            {
                num6 = Conversions.ToInteger(Strings.Right(str, str.Length - 7));
                levelArray = new sPIELevel[(num6 - 1) + 1];
                goto Label_001E;
            }
            if (Strings.Left(str, 6) == "LEVEL ")
            {
                index = Conversions.ToInteger(Strings.Right(str, str.Length - 6)) - 1;
                if (index >= num6)
                {
                    result2.ProblemAdd("Level number >= number of levels.");
                    return result2;
                }
                goto Label_001E;
            }
            if (Strings.Left(str, 6) != "POINTS")
            {
                if (Strings.Left(str, 8) != "POLYGONS")
                {
                    if (Strings.Left(str, 10) != "CONNECTORS")
                    {
                        goto Label_001E;
                    }
                    this.ConnectorCount = Conversions.ToInteger(Strings.Right(str, str.Length - 11));
                    this.Connectors = new modMath.sXYZ_sng[(this.ConnectorCount - 1) + 1];
                    num = 0;
                    goto Label_0A69;
                }
                levelArray[index].PolygonCount = Conversions.ToInteger(Strings.Right(str, str.Length - 9));
                levelArray[index].Polygon = new sPIELevel.sPolygon[(levelArray[index].PolygonCount - 1) + 1];
                num = 0;
            }
            else
            {
                levelArray[index].PointCount = Conversions.ToInteger(Strings.Right(str, str.Length - 7));
                levelArray[index].Point = new modMath.sXYZ_sng[(levelArray[index].PointCount - 1) + 1];
                num = 0;
                while (true)
                {
                    str = File.ReadLine();
                    if (str == null)
                    {
                        goto Label_0BD9;
                    }
                    str2 = Strings.Left(str, 1);
                    if (!((str2 == "\t") | (str2 == " ")))
                    {
                        if (str2 != "")
                        {
                            goto Label_002F;
                        }
                    }
                    else
                    {
                        strArray = new string[3];
                        num3 = 0;
                        strArray[0] = "";
                        flag = false;
                        int num11 = str.Length - 1;
                        for (num2 = 0; num2 <= num11; num2++)
                        {
                            if ((str[num2] != ' ') & (str[num2] != '\t'))
                            {
                                flag = true;
                                strArray2 = strArray;
                                num12 = num3;
                                strArray2[num12] = strArray2[num12] + Conversions.ToString(str[num2]);
                            }
                            else if (flag)
                            {
                                num3++;
                                if (num3 == 3)
                                {
                                    break;
                                }
                                strArray[num3] = "";
                                flag = false;
                            }
                        }
                        try
                        {
                            levelArray[index].Point[num].X = Conversions.ToSingle(strArray[0]);
                            levelArray[index].Point[num].Y = Conversions.ToSingle(strArray[1]);
                            levelArray[index].Point[num].Z = Conversions.ToSingle(strArray[2]);
                        }
                        catch (Exception exception1)
                        {
                            ProjectData.SetProjectError(exception1);
                            Exception exception = exception1;
                            result2.ProblemAdd("Bad point " + Conversions.ToString(num));
                            result = result2;
                            ProjectData.ClearProjectError();
                            return result;
                        }
                        num++;
                    }
                }
            }
            while (true)
            {
                str = File.ReadLine();
                if (str == null)
                {
                    goto Label_0BD9;
                }
                str2 = Strings.Left(str, 1);
                if (!((str2 == "\t") | (str2 == " ")))
                {
                    if (str2 != "")
                    {
                        goto Label_002F;
                    }
                }
                else
                {
                    num3 = 0;
                    strArray = new string[num3 + 1];
                    strArray[num3] = "";
                    int num13 = str.Length - 1;
                    for (num2 = 0; num2 <= num13; num2++)
                    {
                        if ((str[num2] == ' ') | (str[num2] == '\t'))
                        {
                            if (strArray[num3].Length > 0)
                            {
                                num3++;
                                strArray = (string[]) Utils.CopyArray((Array) strArray, new string[num3 + 1]);
                                strArray[num3] = "";
                            }
                        }
                        else
                        {
                            strArray2 = strArray;
                            num12 = num3;
                            strArray2[num12] = strArray2[num12] + Conversions.ToString(str[num2]);
                        }
                    }
                    if (strArray[num3].Length == 0)
                    {
                        strArray = (string[]) Utils.CopyArray((Array) strArray, new string[(num3 - 1) + 1]);
                    }
                    else
                    {
                        num3++;
                    }
                    if (num10 == 3)
                    {
                        int num4;
                        if (num3 < 2)
                        {
                            result2.ProblemAdd("Too few fields for polygon " + Conversions.ToString(num));
                            return result2;
                        }
                        try
                        {
                            num4 = Conversions.ToInteger(strArray[1]);
                        }
                        catch (Exception exception7)
                        {
                            ProjectData.SetProjectError(exception7);
                            Exception exception2 = exception7;
                            result2.ProblemAdd("Bad polygon point count: " + exception2.Message);
                            result = result2;
                            ProjectData.ClearProjectError();
                            return result;
                        }
                        levelArray[index].Polygon[num].PointCount = num4;
                        levelArray[index].Polygon[num].PointNum = new int[(num4 - 1) + 1];
                        levelArray[index].Polygon[num].TexCoord = new modMath.sXY_sng[(num4 - 1) + 1];
                        switch (num4)
                        {
                            case 3:
                                num9++;
                                break;

                            case 4:
                                num8++;
                                break;
                        }
                        int num14 = strArray.GetUpperBound(0) + 1;
                        if (num14 == 0)
                        {
                            goto Label_002F;
                        }
                        if (num14 != (2 + (num4 * 3)))
                        {
                            result2.ProblemAdd("Wrong number of fields (" + Conversions.ToString((int) (strArray.GetUpperBound(0) + 1)) + ") for polygon " + Conversions.ToString(num));
                            return result2;
                        }
                        int num15 = num4 - 1;
                        for (num2 = 0; num2 <= num15; num2++)
                        {
                            try
                            {
                                levelArray[index].Polygon[num].PointNum[num2] = Conversions.ToInteger(strArray[2 + num2]);
                            }
                            catch (Exception exception8)
                            {
                                ProjectData.SetProjectError(exception8);
                                Exception exception3 = exception8;
                                result2.ProblemAdd("Bad polygon point: " + exception3.Message);
                                result = result2;
                                ProjectData.ClearProjectError();
                                return result;
                            }
                            try
                            {
                                levelArray[index].Polygon[num].TexCoord[num2].X = Conversions.ToSingle(strArray[(2 + num4) + (2 * num2)]);
                            }
                            catch (Exception exception9)
                            {
                                ProjectData.SetProjectError(exception9);
                                Exception exception4 = exception9;
                                result2.ProblemAdd("Bad polygon x tex coord: " + exception4.Message);
                                result = result2;
                                ProjectData.ClearProjectError();
                                return result;
                            }
                            try
                            {
                                levelArray[index].Polygon[num].TexCoord[num2].Y = Conversions.ToSingle(strArray[((2 + num4) + (2 * num2)) + 1]);
                            }
                            catch (Exception exception10)
                            {
                                ProjectData.SetProjectError(exception10);
                                Exception exception5 = exception10;
                                result2.ProblemAdd("Bad polygon y tex coord: " + exception5.Message);
                                result = result2;
                                ProjectData.ClearProjectError();
                                return result;
                            }
                        }
                        num++;
                    }
                    else if (num10 == 2)
                    {
                        int num5 = 0;
                        do
                        {
                            levelArray[index].Polygon[num].PointCount = Conversions.ToInteger(strArray[num5 + 1]);
                            levelArray[index].Polygon[num].PointNum = new int[(levelArray[index].Polygon[num].PointCount - 1) + 1];
                            levelArray[index].Polygon[num].TexCoord = new modMath.sXY_sng[(levelArray[index].Polygon[num].PointCount - 1) + 1];
                            if (levelArray[index].Polygon[num].PointCount == 3)
                            {
                                num9++;
                            }
                            else if (levelArray[index].Polygon[num].PointCount == 4)
                            {
                                num8++;
                            }
                            int num16 = levelArray[index].Polygon[num].PointCount - 1;
                            for (num2 = 0; num2 <= num16; num2++)
                            {
                                levelArray[index].Polygon[num].PointNum[num2] = Conversions.ToInteger(strArray[(num5 + 2) + num2]);
                            }
                            num3 = (num5 + 2) + levelArray[index].Polygon[num].PointCount;
                            if ((((((((strArray[num5] == "4200") | (strArray[num5] == "4000")) | (strArray[num5] == "6a00")) | (strArray[num5] == "4a00")) | (strArray[num5] == "6200")) | (strArray[num5] == "14200")) | (strArray[num5] == "14a00")) | (strArray[num5] == "16a00"))
                            {
                                num3 += 4;
                            }
                            int num17 = levelArray[index].Polygon[num].PointCount - 1;
                            for (num2 = 0; num2 <= num17; num2++)
                            {
                                levelArray[index].Polygon[num].TexCoord[num2].X = Conversions.ToSingle(strArray[num3]);
                                levelArray[index].Polygon[num].TexCoord[num2].Y = Conversions.ToSingle(strArray[num3 + 1]);
                                num3 += 2;
                            }
                            num5 = num3;
                            num++;
                        }
                        while (num5 < strArray.GetUpperBound(0));
                    }
                }
            }
        Label_0A69:
            str = File.ReadLine();
            if (str != null)
            {
                str2 = Strings.Left(str, 1);
                if (!((str2 == "\t") | (str2 == " ")))
                {
                    if (str2 != "")
                    {
                        goto Label_002F;
                    }
                }
                else
                {
                    strArray = new string[3];
                    num3 = 0;
                    strArray[0] = "";
                    flag = false;
                    int num18 = str.Length - 1;
                    for (num2 = 0; num2 <= num18; num2++)
                    {
                        if ((str[num2] != ' ') & (str[num2] != '\t'))
                        {
                            flag = true;
                            strArray2 = strArray;
                            num12 = num3;
                            strArray2[num12] = strArray2[num12] + Conversions.ToString(str[num2]);
                        }
                        else if (flag)
                        {
                            num3++;
                            if (num3 == 3)
                            {
                                break;
                            }
                            strArray[num3] = "";
                            flag = false;
                        }
                    }
                    try
                    {
                        this.Connectors[num].X = Conversions.ToSingle(strArray[0]);
                        this.Connectors[num].Y = Conversions.ToSingle(strArray[2]);
                        this.Connectors[num].Z = Conversions.ToSingle(strArray[1]);
                    }
                    catch (Exception exception11)
                    {
                        ProjectData.SetProjectError(exception11);
                        Exception exception6 = exception11;
                        result2.ProblemAdd("Bad connector " + Conversions.ToString(num));
                        result = result2;
                        ProjectData.ClearProjectError();
                        return result;
                    }
                    num++;
                }
                goto Label_0A69;
            }
        Label_0BD9:
            this.GLTextureNum = Owner.Get_TexturePage_GLTexture(Strings.Left(stringCheck, stringCheck.Length - 4));
            if (this.GLTextureNum == 0)
            {
                result2.WarningAdd("Texture \"" + stringCheck + "\" was not loaded");
            }
            this.TriangleCount = num9;
            this.QuadCount = num8;
            this.Triangles = new sTriangle[(this.TriangleCount - 1) + 1];
            this.Quads = new sQuad[(this.QuadCount - 1) + 1];
            num9 = 0;
            num8 = 0;
            int num19 = num6 - 1;
            for (index = 0; index <= num19; index++)
            {
                int num20 = levelArray[index].PolygonCount - 1;
                for (num = 0; num <= num20; num++)
                {
                    if (levelArray[index].Polygon[num].PointCount == 3)
                    {
                        this.Triangles[num9].PosA = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[0]];
                        this.Triangles[num9].PosB = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[1]];
                        this.Triangles[num9].PosC = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[2]];
                        switch (num10)
                        {
                            case 2:
                                this.Triangles[num9].TexCoordA.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[0].X) / 255.0);
                                this.Triangles[num9].TexCoordA.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[0].Y) / 255.0);
                                this.Triangles[num9].TexCoordB.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[1].X) / 255.0);
                                this.Triangles[num9].TexCoordB.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[1].Y) / 255.0);
                                this.Triangles[num9].TexCoordC.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[2].X) / 255.0);
                                this.Triangles[num9].TexCoordC.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[2].Y) / 255.0);
                                break;

                            case 3:
                                this.Triangles[num9].TexCoordA = levelArray[index].Polygon[num].TexCoord[0];
                                this.Triangles[num9].TexCoordB = levelArray[index].Polygon[num].TexCoord[1];
                                this.Triangles[num9].TexCoordC = levelArray[index].Polygon[num].TexCoord[2];
                                break;
                        }
                        num9++;
                        continue;
                    }
                    if (levelArray[index].Polygon[num].PointCount == 4)
                    {
                        this.Quads[num8].PosA = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[0]];
                        this.Quads[num8].PosB = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[1]];
                        this.Quads[num8].PosC = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[2]];
                        this.Quads[num8].PosD = levelArray[index].Point[levelArray[index].Polygon[num].PointNum[3]];
                        switch (num10)
                        {
                            case 2:
                                this.Quads[num8].TexCoordA.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[0].X) / 255.0);
                                this.Quads[num8].TexCoordA.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[0].Y) / 255.0);
                                this.Quads[num8].TexCoordB.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[1].X) / 255.0);
                                this.Quads[num8].TexCoordB.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[1].Y) / 255.0);
                                this.Quads[num8].TexCoordC.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[2].X) / 255.0);
                                this.Quads[num8].TexCoordC.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[2].Y) / 255.0);
                                this.Quads[num8].TexCoordD.X = (float) (((double) levelArray[index].Polygon[num].TexCoord[3].X) / 255.0);
                                this.Quads[num8].TexCoordD.Y = (float) (((double) levelArray[index].Polygon[num].TexCoord[3].Y) / 255.0);
                                break;

                            case 3:
                                this.Quads[num8].TexCoordA = levelArray[index].Polygon[num].TexCoord[0];
                                this.Quads[num8].TexCoordB = levelArray[index].Polygon[num].TexCoord[1];
                                this.Quads[num8].TexCoordC = levelArray[index].Polygon[num].TexCoord[2];
                                this.Quads[num8].TexCoordD = levelArray[index].Polygon[num].TexCoord[3];
                                break;
                        }
                        num8++;
                    }
                }
            }
            return result2;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sPIELevel
        {
            public sPolygon[] Polygon;
            public int PolygonCount;
            public modMath.sXYZ_sng[] Point;
            public int PointCount;
            [StructLayout(LayoutKind.Sequential)]
            public struct sPolygon
            {
                public int[] PointNum;
                public modMath.sXY_sng[] TexCoord;
                public int PointCount;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sQuad
        {
            public modMath.sXYZ_sng PosA;
            public modMath.sXYZ_sng PosB;
            public modMath.sXYZ_sng PosC;
            public modMath.sXYZ_sng PosD;
            public modMath.sXY_sng TexCoordA;
            public modMath.sXY_sng TexCoordB;
            public modMath.sXY_sng TexCoordC;
            public modMath.sXY_sng TexCoordD;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sTriangle
        {
            public modMath.sXYZ_sng PosA;
            public modMath.sXYZ_sng PosB;
            public modMath.sXYZ_sng PosC;
            public modMath.sXY_sng TexCoordA;
            public modMath.sXY_sng TexCoordB;
            public modMath.sXY_sng TexCoordC;
        }
    }
}

