#region

using System;
using System.IO;
using NLog;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;

#endregion

namespace SharpFlame.Domain
{
    public class clsModel
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        public int ConnectorCount;
        public XYZDouble[] Connectors = new XYZDouble[0];

        public int GLTextureNum;

        public int QuadCount;
        public sQuad[] Quads;
        public int TriangleCount;
        public sTriangle[] Triangles;

        public int GLList_Create()
        {
            var result = 0;

            result = GL.GenLists(1);
            if ( result == 0 )
            {
                return result;
            }

            GL.NewList(result, ListMode.Compile);
            GLDraw();
            GL.EndList();

            return result;
        }

        public void GLDraw()
        {
            var a = 0;

            GL.BindTexture(TextureTarget.Texture2D, GLTextureNum);
            GL.TexEnv(TextureEnvTarget.TextureEnv, TextureEnvParameter.TextureEnvMode, (int)TextureEnvMode.Modulate);
            GL.AlphaFunc(AlphaFunction.Greater, 0.5f);
            GL.Enable(EnableCap.AlphaTest);

            GL.Begin(BeginMode.Triangles);
            for ( a = 0; a <= TriangleCount - 1; a++ )
            {
                var with_1 = Triangles[a];
                GL.TexCoord2(with_1.TexCoordA.X, with_1.TexCoordA.Y);
                GL.Vertex3(with_1.PosA.X, with_1.PosA.Y, Convert.ToDouble(- with_1.PosA.Z));
                GL.TexCoord2(with_1.TexCoordB.X, with_1.TexCoordB.Y);
                GL.Vertex3(with_1.PosB.X, with_1.PosB.Y, Convert.ToDouble(- with_1.PosB.Z));
                GL.TexCoord2(with_1.TexCoordC.X, with_1.TexCoordC.Y);
                GL.Vertex3(with_1.PosC.X, with_1.PosC.Y, Convert.ToDouble(- with_1.PosC.Z));
            }
            GL.End();
            GL.Begin(BeginMode.Quads);
            for ( a = 0; a <= QuadCount - 1; a++ )
            {
                var with_2 = Quads[a];
                GL.TexCoord2(with_2.TexCoordA.X, with_2.TexCoordA.Y);
                GL.Vertex3(with_2.PosA.X, with_2.PosA.Y, Convert.ToDouble(- with_2.PosA.Z));
                GL.TexCoord2(with_2.TexCoordB.X, with_2.TexCoordB.Y);
                GL.Vertex3(with_2.PosB.X, with_2.PosB.Y, Convert.ToDouble(- with_2.PosB.Z));
                GL.TexCoord2(with_2.TexCoordC.X, with_2.TexCoordC.Y);
                GL.Vertex3(with_2.PosC.X, with_2.PosC.Y, Convert.ToDouble(- with_2.PosC.Z));
                GL.TexCoord2(with_2.TexCoordD.X, with_2.TexCoordD.Y);
                GL.Vertex3(with_2.PosD.X, with_2.PosD.Y, Convert.ToDouble(- with_2.PosD.Z));
            }
            GL.End();
        }

        public clsResult ReadPIE(StreamReader file, clsObjectData owner)
        {
            var returnResult = new clsResult("Reading PIE", false);
            logger.Debug("Reading PIE");

            var a = 0;
            var strTemp = "";
            var LevelCount = 0;
            var NewQuadCount = 0;
            var NewTriCount = 0;
            var C = 0;
            var TextureName = "";
            sPIELevel[] Levels = null;
            var LevelNum = 0;
            var GotText = default(bool);
            string strTemp2;
            var D = 0;
            var PIEVersion = 0;
            var Count = 0;

            Levels = new sPIELevel[0];
            LevelNum = -1;
            do
            {
                strTemp = file.ReadLine();
                if ( strTemp == null )
                {
                    goto FileFinished;
                }
                Reeval:
                if ( strTemp.Substring(0, 3) == "PIE" )
                {
                    PIEVersion = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 4), strTemp.Length - 4));
                    if ( PIEVersion != 2 & PIEVersion != 3 )
                    {
                        returnResult.ProblemAdd("Version is unknown.");
                        return returnResult;
                    }
                }
                else if ( strTemp.Substring(0, 4) == "TYPE" )
                {
                }
                else if ( strTemp.Substring(0, 7) == "TEXTURE" )
                {
                    TextureName = strTemp.Substring(strTemp.Length - (strTemp.Length - 10), strTemp.Length - 10);
                    a = TextureName.LastIndexOf(" ");
                    if ( a > 0 )
                    {
                        a = TextureName.LastIndexOf(" ", a - 1) + 1;
                    }
                    else
                    {
                        returnResult.ProblemAdd("Bad texture name.");
                        return returnResult;
                    }

                    if ( a > 0 )
                    {
                        TextureName = TextureName.Substring(0, a - 1);
                    }
                    else
                    {
                        returnResult.ProblemAdd("Bad texture name.");
                        return returnResult;
                    }
                }
                else if ( strTemp.Substring(0, 6) == "LEVELS" )
                {
                    LevelCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 7), strTemp.Length - 7));
                    Levels = new sPIELevel[LevelCount];
                }
                else if ( strTemp.Substring(0, 6) == "LEVEL " )
                {
                    LevelNum = (int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 6), strTemp.Length - 6))) - 1;
                    if ( LevelNum >= LevelCount )
                    {
                        returnResult.ProblemAdd("Level number >= number of levels.");
                        return returnResult;
                    }
                }
                else
                {
                    var b = 0;
                    string[] splitText = null;
                    if ( strTemp.Substring(0, 6) == "POINTS" )
                    {
                        Levels[LevelNum].PointCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 7), strTemp.Length - 7));
                        Levels[LevelNum].Point = new XYZDouble[Levels[LevelNum].PointCount];
                        a = 0;
                        do
                        {
                            strTemp = file.ReadLine();
                            if ( strTemp == null )
                            {
                                goto FileFinished;
                            }

                            strTemp2 = strTemp.Left(1);
                            if ( char.Parse(strTemp2) == '\t' || strTemp2 == " " )
                            {
                                splitText = new string[3];
                                C = 0;
                                splitText[0] = "";
                                GotText = false;
                                for ( b = 0; b <= strTemp.Length - 1; b++ )
                                {
                                    if ( strTemp[b] != ' ' && strTemp[b] != '\t' )
                                    {
                                        GotText = true;
                                        splitText[C] += strTemp[b].ToString();
                                    }
                                    else
                                    {
                                        if ( GotText )
                                        {
                                            C++;
                                            if ( C == 3 )
                                            {
                                                break;
                                            }
                                            splitText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                try
                                {
                                    Levels[LevelNum].Point[a].X = float.Parse(splitText[0]);
                                    Levels[LevelNum].Point[a].Y = float.Parse(splitText[1]);
                                    Levels[LevelNum].Point[a].Z = float.Parse(splitText[2]);
                                }
                                catch ( Exception )
                                {
                                    returnResult.ProblemAdd("Bad point " + Convert.ToString(a));
                                    return returnResult;
                                }
                                a++;
                            }
                            else if ( string.IsNullOrEmpty(strTemp2) )
                            {
                            }
                            else
                            {
                                goto Reeval;
                            }
                        } while ( true );
                    }
                    else if ( strTemp.Substring(0, 8) == "POLYGONS" )
                    {
                        Levels[LevelNum].PolygonCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 9), strTemp.Length - 9));
                        Levels[LevelNum].Polygon = new sPIELevel.sPolygon[Levels[LevelNum].PolygonCount];
                        a = 0;
                        do
                        {
                            strTemp = file.ReadLine();
                            if ( strTemp == null )
                            {
                                goto FileFinished;
                            }

                            strTemp2 = strTemp.Left(1);
                            if ( char.Parse(strTemp2) == '\t' || strTemp2 == " " )
                            {
                                C = 0;
                                splitText = new string[C + 1];
                                splitText[C] = "";
                                for ( b = 0; b <= strTemp.Length - 1; b++ )
                                {
                                    if ( strTemp[b] == ' ' || strTemp[b] == '\t' )
                                    {
                                        if ( splitText[C].Length > 0 )
                                        {
                                            C++;
                                            Array.Resize(ref splitText, C + 1);
                                            splitText[C] = "";
                                        }
                                    }
                                    else
                                    {
                                        splitText[C] += strTemp[b].ToString();
                                    }
                                }
                                if ( splitText[C].Length == 0 )
                                {
                                    Array.Resize(ref splitText, C);
                                }
                                else
                                {
                                    C++;
                                }

                                if ( PIEVersion == 3 )
                                {
                                    //200, pointcount, points, texcoords
                                    if ( C < 2 )
                                    {
                                        returnResult.ProblemAdd("Too few fields for polygon " + Convert.ToString(a));
                                        return returnResult;
                                    }
                                    try
                                    {
                                        Count = int.Parse(splitText[1]);
                                    }
                                    catch ( Exception ex )
                                    {
                                        returnResult.ProblemAdd("Bad polygon point count: " + ex.Message);
                                        return returnResult;
                                    }
                                    Levels[LevelNum].Polygon[a].PointCount = Count;
                                    Levels[LevelNum].Polygon[a].PointNum = new int[Count];
                                    Levels[LevelNum].Polygon[a].TexCoord = new XYDouble[Count];
                                    if ( Count == 3 )
                                    {
                                        NewTriCount++;
                                    }
                                    else if ( Count == 4 )
                                    {
                                        NewQuadCount++;
                                    }
                                    if ( splitText.GetUpperBound(0) + 1 == 0 )
                                    {
                                        goto Reeval;
                                    }
                                    if ( splitText.GetUpperBound(0) + 1 != (2 + Count * 3) )
                                    {
                                        returnResult.ProblemAdd("Wrong number of fields (" + Convert.ToString(splitText.GetUpperBound(0) + 1) + ") for polygon " +
                                                                Convert.ToString(a));
                                        return returnResult;
                                    }
                                    for ( b = 0; b <= Count - 1; b++ )
                                    {
                                        try
                                        {
                                            Levels[LevelNum].Polygon[a].PointNum[b] = int.Parse(splitText[2 + b]);
                                        }
                                        catch ( Exception ex )
                                        {
                                            returnResult.ProblemAdd("Bad polygon point: " + ex.Message);
                                            return returnResult;
                                        }

                                        try
                                        {
                                            Levels[LevelNum].Polygon[a].TexCoord[b].X = float.Parse(splitText[2 + Count + 2 * b]);
                                        }
                                        catch ( Exception ex )
                                        {
                                            returnResult.ProblemAdd("Bad polygon x tex coord: " + ex.Message);
                                            return returnResult;
                                        }
                                        try
                                        {
                                            Levels[LevelNum].Polygon[a].TexCoord[b].Y = float.Parse(splitText[2 + Count + 2 * b + 1]);
                                        }
                                        catch ( Exception ex )
                                        {
                                            returnResult.ProblemAdd("Bad polygon y tex coord: " + ex.Message);
                                            return returnResult;
                                        }
                                    }
                                    a++;
                                }
                                else if ( PIEVersion == 2 )
                                {
                                    D = 0;
                                    do
                                    {
                                        //flag, numpoints, points[], x4 ignore if animated, texcoord[]xy
                                        Levels[LevelNum].Polygon[a].PointCount = int.Parse(splitText[D + 1]);
                                        Levels[LevelNum].Polygon[a].PointNum = new int[Levels[LevelNum].Polygon[a].PointCount];
                                        Levels[LevelNum].Polygon[a].TexCoord = new XYDouble[Levels[LevelNum].Polygon[a].PointCount];
                                        if ( Levels[LevelNum].Polygon[a].PointCount == 3 )
                                        {
                                            NewTriCount++;
                                        }
                                        else if ( Levels[LevelNum].Polygon[a].PointCount == 4 )
                                        {
                                            NewQuadCount++;
                                        }
                                        for ( b = 0; b <= Levels[LevelNum].Polygon[a].PointCount - 1; b++ )
                                        {
                                            Levels[LevelNum].Polygon[a].PointNum[b] = int.Parse(splitText[D + 2 + b]);
                                        }
                                        C = D + 2 + Levels[LevelNum].Polygon[a].PointCount;
                                        if ( splitText[D] == "4200" || splitText[D] == "4000" || splitText[D] == "6a00" || splitText[D] == "4a00" || splitText[D] == "6200" ||
                                             splitText[D] == "14200" || splitText[D] == "14a00" || splitText[D] == "16a00" )
                                        {
                                            C += 4;
                                        }
                                        for ( b = 0; b <= Levels[LevelNum].Polygon[a].PointCount - 1; b++ )
                                        {
                                            Levels[LevelNum].Polygon[a].TexCoord[b].X = float.Parse(splitText[C]);
                                            Levels[LevelNum].Polygon[a].TexCoord[b].Y = float.Parse(splitText[C + 1]);
                                            C += 2;
                                        }
                                        D = C;
                                        a++;
                                    } while ( D < splitText.GetUpperBound(0) );
                                }
                            }
                            else if ( string.IsNullOrEmpty(strTemp2) )
                            {
                            }
                            else
                            {
                                goto Reeval;
                            }
                        } while ( true );
                    }
                    else if ( strTemp.Substring(0, 10) == "CONNECTORS" )
                    {
                        ConnectorCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 11), strTemp.Length - 11));
                        Connectors = new XYZDouble[ConnectorCount];
                        a = 0;
                        do
                        {
                            strTemp = file.ReadLine();
                            if ( strTemp == null )
                            {
                                goto FileFinished;
                            }

                            strTemp2 = strTemp.Left(1);
                            if ( char.Parse(strTemp2) == '\t' || strTemp2 == " " )
                            {
                                splitText = new string[3];
                                C = 0;
                                splitText[0] = "";
                                GotText = false;
                                for ( b = 0; b <= strTemp.Length - 1; b++ )
                                {
                                    if ( strTemp[b] != ' ' && strTemp[b] != '\t' )
                                    {
                                        GotText = true;
                                        splitText[C] += strTemp[b].ToString();
                                    }
                                    else
                                    {
                                        if ( GotText )
                                        {
                                            C++;
                                            if ( C == 3 )
                                            {
                                                break;
                                            }
                                            splitText[C] = "";
                                            GotText = false;
                                        }
                                    }
                                }

                                try
                                {
                                    Connectors[a].X = float.Parse(splitText[0]);
                                    Connectors[a].Y = float.Parse(splitText[2]);
                                    Connectors[a].Z = float.Parse(splitText[1]);
                                }
                                catch ( Exception )
                                {
                                    returnResult.ProblemAdd("Bad connector " + Convert.ToString(a));
                                    return returnResult;
                                }
                                a++;
                            }
                            else if ( string.IsNullOrEmpty(strTemp2) )
                            {
                            }
                            else
                            {
                                goto Reeval;
                            }
                        } while ( true );
                    }
                }
            } while ( true );
            FileFinished:

            GLTextureNum = owner.Get_TexturePage_GLTexture(TextureName.Substring(0, TextureName.Length - 4));
            if ( GLTextureNum == 0 )
            {
                returnResult.WarningAdd("Texture \"{0}\" was not loaded".Format2(TextureName));
            }

            TriangleCount = NewTriCount;
            QuadCount = NewQuadCount;
            Triangles = new sTriangle[TriangleCount];
            Quads = new sQuad[QuadCount];
            NewTriCount = 0;
            NewQuadCount = 0;
            for ( LevelNum = 0; LevelNum <= LevelCount - 1; LevelNum++ )
            {
                for ( a = 0; a <= Levels[LevelNum].PolygonCount - 1; a++ )
                {
                    if ( Levels[LevelNum].Polygon[a].PointCount == 3 )
                    {
                        Triangles[NewTriCount].PosA = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[0]];
                        Triangles[NewTriCount].PosB = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[1]];
                        Triangles[NewTriCount].PosC = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[2]];
                        if ( PIEVersion == 2 )
                        {
                            Triangles[NewTriCount].TexCoordA.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[0].X / 255.0D);
                            Triangles[NewTriCount].TexCoordA.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[0].Y / 255.0D);
                            Triangles[NewTriCount].TexCoordB.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[1].X / 255.0D);
                            Triangles[NewTriCount].TexCoordB.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[1].Y / 255.0D);
                            Triangles[NewTriCount].TexCoordC.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[2].X / 255.0D);
                            Triangles[NewTriCount].TexCoordC.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[2].Y / 255.0D);
                        }
                        else if ( PIEVersion == 3 )
                        {
                            Triangles[NewTriCount].TexCoordA = Levels[LevelNum].Polygon[a].TexCoord[0];
                            Triangles[NewTriCount].TexCoordB = Levels[LevelNum].Polygon[a].TexCoord[1];
                            Triangles[NewTriCount].TexCoordC = Levels[LevelNum].Polygon[a].TexCoord[2];
                        }
                        NewTriCount++;
                    }
                    else if ( Levels[LevelNum].Polygon[a].PointCount == 4 )
                    {
                        Quads[NewQuadCount].PosA = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[0]];
                        Quads[NewQuadCount].PosB = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[1]];
                        Quads[NewQuadCount].PosC = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[2]];
                        Quads[NewQuadCount].PosD = Levels[LevelNum].Point[Levels[LevelNum].Polygon[a].PointNum[3]];
                        if ( PIEVersion == 2 )
                        {
                            Quads[NewQuadCount].TexCoordA.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[0].X / 255.0D);
                            Quads[NewQuadCount].TexCoordA.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[0].Y / 255.0D);
                            Quads[NewQuadCount].TexCoordB.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[1].X / 255.0D);
                            Quads[NewQuadCount].TexCoordB.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[1].Y / 255.0D);
                            Quads[NewQuadCount].TexCoordC.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[2].X / 255.0D);
                            Quads[NewQuadCount].TexCoordC.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[2].Y / 255.0D);
                            Quads[NewQuadCount].TexCoordD.X = (float)(Levels[LevelNum].Polygon[a].TexCoord[3].X / 255.0D);
                            Quads[NewQuadCount].TexCoordD.Y = (float)(Levels[LevelNum].Polygon[a].TexCoord[3].Y / 255.0D);
                        }
                        else if ( PIEVersion == 3 )
                        {
                            Quads[NewQuadCount].TexCoordA = Levels[LevelNum].Polygon[a].TexCoord[0];
                            Quads[NewQuadCount].TexCoordB = Levels[LevelNum].Polygon[a].TexCoord[1];
                            Quads[NewQuadCount].TexCoordC = Levels[LevelNum].Polygon[a].TexCoord[2];
                            Quads[NewQuadCount].TexCoordD = Levels[LevelNum].Polygon[a].TexCoord[3];
                        }
                        NewQuadCount++;
                    }
                }
            }

            return returnResult;
        }

        public struct sPIELevel
        {
            public XYZDouble[] Point;
            public int PointCount;
            public sPolygon[] Polygon;
            public int PolygonCount;

            public struct sPolygon
            {
                public int PointCount;
                public int[] PointNum;
                public XYDouble[] TexCoord;
            }
        }

        public struct sQuad
        {
            public XYZDouble PosA;
            public XYZDouble PosB;
            public XYZDouble PosC;
            public XYZDouble PosD;
            public XYDouble TexCoordA;
            public XYDouble TexCoordB;
            public XYDouble TexCoordC;
            public XYDouble TexCoordD;
        }

        public struct sTriangle
        {
            public XYZDouble PosA;
            public XYZDouble PosB;
            public XYZDouble PosC;
            public XYDouble TexCoordA;
            public XYDouble TexCoordB;
            public XYDouble TexCoordC;
        }
    }
}