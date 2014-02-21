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
            var levelCount = 0;
            var newQuadCount = 0;
            var newTriCount = 0;
            var textureName = "";
            sPIELevel[] levels = null;
            var levelNum = 0;
            var D = 0;
            var pieVersion = 0;

            levels = new sPIELevel[0];
            levelNum = -1;
            do
            {
                var strTemp = file.ReadLine();
                if ( strTemp == null )
                {
                    goto FileFinished;
                }
                Reeval:
                if ( strTemp.Substring(0, 3) == "PIE" )
                {
                    pieVersion = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 4), strTemp.Length - 4));
                    if ( pieVersion != 2 & pieVersion != 3 )
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
                    textureName = strTemp.Substring(strTemp.Length - (strTemp.Length - 10), strTemp.Length - 10);
                    a = textureName.LastIndexOf(" ");
                    if ( a > 0 )
                    {
                        a = textureName.LastIndexOf(" ", a - 1) + 1;
                    }
                    else
                    {
                        returnResult.ProblemAdd("Bad texture name.");
                        return returnResult;
                    }

                    if ( a > 0 )
                    {
                        textureName = textureName.Substring(0, a - 1);
                    }
                    else
                    {
                        returnResult.ProblemAdd("Bad texture name.");
                        return returnResult;
                    }
                }
                else if ( strTemp.Substring(0, 6) == "LEVELS" )
                {
                    levelCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 7), strTemp.Length - 7));
                    levels = new sPIELevel[levelCount];
                }
                else if ( strTemp.Substring(0, 6) == "LEVEL " )
                {
                    levelNum = (int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 6), strTemp.Length - 6))) - 1;
                    if ( levelNum >= levelCount )
                    {
                        returnResult.ProblemAdd("Level number >= number of levels.");
                        return returnResult;
                    }
                }
                else
                {
                    var b = 0;
                    string[] splitText = null;
                    var c = 0;
                    var gotText = default(bool);
                    string strTemp2;
                    if ( strTemp.Substring(0, 6) == "POINTS" )
                    {
                        levels[levelNum].PointCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 7), strTemp.Length - 7));
                        levels[levelNum].Point = new XYZDouble[levels[levelNum].PointCount];
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
                                c = 0;
                                splitText[0] = "";
                                gotText = false;
                                for ( b = 0; b <= strTemp.Length - 1; b++ )
                                {
                                    if ( strTemp[b] != ' ' && strTemp[b] != '\t' )
                                    {
                                        gotText = true;
                                        splitText[c] += strTemp[b].ToString();
                                    }
                                    else
                                    {
                                        if ( gotText )
                                        {
                                            c++;
                                            if ( c == 3 )
                                            {
                                                break;
                                            }
                                            splitText[c] = "";
                                            gotText = false;
                                        }
                                    }
                                }

                                try
                                {
                                    levels[levelNum].Point[a].X = float.Parse(splitText[0]);
                                    levels[levelNum].Point[a].Y = float.Parse(splitText[1]);
                                    levels[levelNum].Point[a].Z = float.Parse(splitText[2]);
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
                        levels[levelNum].PolygonCount = int.Parse(strTemp.Substring(strTemp.Length - (strTemp.Length - 9), strTemp.Length - 9));
                        levels[levelNum].Polygon = new sPIELevel.sPolygon[levels[levelNum].PolygonCount];
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
                                c = 0;
                                splitText = new string[c + 1];
                                splitText[c] = "";
                                for ( b = 0; b <= strTemp.Length - 1; b++ )
                                {
                                    if ( strTemp[b] == ' ' || strTemp[b] == '\t' )
                                    {
                                        if ( splitText[c].Length > 0 )
                                        {
                                            c++;
                                            Array.Resize(ref splitText, c + 1);
                                            splitText[c] = "";
                                        }
                                    }
                                    else
                                    {
                                        splitText[c] += strTemp[b].ToString();
                                    }
                                }
                                if ( splitText[c].Length == 0 )
                                {
                                    Array.Resize(ref splitText, c);
                                }
                                else
                                {
                                    c++;
                                }

                                if ( pieVersion == 3 )
                                {
                                    //200, pointcount, points, texcoords
                                    if ( c < 2 )
                                    {
                                        returnResult.ProblemAdd("Too few fields for polygon " + Convert.ToString(a));
                                        return returnResult;
                                    }
                                    var count = 0;
                                    try
                                    {
                                        count = int.Parse(splitText[1]);
                                    }
                                    catch ( Exception ex )
                                    {
                                        returnResult.ProblemAdd("Bad polygon point count: " + ex.Message);
                                        return returnResult;
                                    }
                                    levels[levelNum].Polygon[a].PointCount = count;
                                    levels[levelNum].Polygon[a].PointNum = new int[count];
                                    levels[levelNum].Polygon[a].TexCoord = new XYDouble[count];
                                    if ( count == 3 )
                                    {
                                        newTriCount++;
                                    }
                                    else if ( count == 4 )
                                    {
                                        newQuadCount++;
                                    }
                                    if ( splitText.GetUpperBound(0) + 1 == 0 )
                                    {
                                        goto Reeval;
                                    }
                                    if ( splitText.GetUpperBound(0) + 1 != (2 + count * 3) )
                                    {
                                        returnResult.ProblemAdd("Wrong number of fields (" + Convert.ToString(splitText.GetUpperBound(0) + 1) + ") for polygon " +
                                                                Convert.ToString(a));
                                        return returnResult;
                                    }
                                    for ( b = 0; b <= count - 1; b++ )
                                    {
                                        try
                                        {
                                            levels[levelNum].Polygon[a].PointNum[b] = int.Parse(splitText[2 + b]);
                                        }
                                        catch ( Exception ex )
                                        {
                                            returnResult.ProblemAdd("Bad polygon point: " + ex.Message);
                                            return returnResult;
                                        }

                                        try
                                        {
                                            levels[levelNum].Polygon[a].TexCoord[b].X = float.Parse(splitText[2 + count + 2 * b]);
                                        }
                                        catch ( Exception ex )
                                        {
                                            returnResult.ProblemAdd("Bad polygon x tex coord: " + ex.Message);
                                            return returnResult;
                                        }
                                        try
                                        {
                                            levels[levelNum].Polygon[a].TexCoord[b].Y = float.Parse(splitText[2 + count + 2 * b + 1]);
                                        }
                                        catch ( Exception ex )
                                        {
                                            returnResult.ProblemAdd("Bad polygon y tex coord: " + ex.Message);
                                            return returnResult;
                                        }
                                    }
                                    a++;
                                }
                                else if ( pieVersion == 2 )
                                {
                                    D = 0;
                                    do
                                    {
                                        //flag, numpoints, points[], x4 ignore if animated, texcoord[]xy
                                        levels[levelNum].Polygon[a].PointCount = int.Parse(splitText[D + 1]);
                                        levels[levelNum].Polygon[a].PointNum = new int[levels[levelNum].Polygon[a].PointCount];
                                        levels[levelNum].Polygon[a].TexCoord = new XYDouble[levels[levelNum].Polygon[a].PointCount];
                                        if ( levels[levelNum].Polygon[a].PointCount == 3 )
                                        {
                                            newTriCount++;
                                        }
                                        else if ( levels[levelNum].Polygon[a].PointCount == 4 )
                                        {
                                            newQuadCount++;
                                        }
                                        for ( b = 0; b <= levels[levelNum].Polygon[a].PointCount - 1; b++ )
                                        {
                                            levels[levelNum].Polygon[a].PointNum[b] = int.Parse(splitText[D + 2 + b]);
                                        }
                                        c = D + 2 + levels[levelNum].Polygon[a].PointCount;
                                        if ( splitText[D] == "4200" || splitText[D] == "4000" || splitText[D] == "6a00" || splitText[D] == "4a00" || splitText[D] == "6200" ||
                                             splitText[D] == "14200" || splitText[D] == "14a00" || splitText[D] == "16a00" )
                                        {
                                            c += 4;
                                        }
                                        for ( b = 0; b <= levels[levelNum].Polygon[a].PointCount - 1; b++ )
                                        {
                                            levels[levelNum].Polygon[a].TexCoord[b].X = float.Parse(splitText[c]);
                                            levels[levelNum].Polygon[a].TexCoord[b].Y = float.Parse(splitText[c + 1]);
                                            c += 2;
                                        }
                                        D = c;
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
                                c = 0;
                                splitText[0] = "";
                                gotText = false;
                                for ( b = 0; b <= strTemp.Length - 1; b++ )
                                {
                                    if ( strTemp[b] != ' ' && strTemp[b] != '\t' )
                                    {
                                        gotText = true;
                                        splitText[c] += strTemp[b].ToString();
                                    }
                                    else
                                    {
                                        if ( gotText )
                                        {
                                            c++;
                                            if ( c == 3 )
                                            {
                                                break;
                                            }
                                            splitText[c] = "";
                                            gotText = false;
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

            GLTextureNum = owner.Get_TexturePage_GLTexture(textureName.Substring(0, textureName.Length - 4));
            if ( GLTextureNum == 0 )
            {
                returnResult.WarningAdd("Texture \"{0}\" was not loaded".Format2(textureName));
            }

            TriangleCount = newTriCount;
            QuadCount = newQuadCount;
            Triangles = new sTriangle[TriangleCount];
            Quads = new sQuad[QuadCount];
            newTriCount = 0;
            newQuadCount = 0;
            for ( levelNum = 0; levelNum <= levelCount - 1; levelNum++ )
            {
                for ( a = 0; a <= levels[levelNum].PolygonCount - 1; a++ )
                {
                    if ( levels[levelNum].Polygon[a].PointCount == 3 )
                    {
                        Triangles[newTriCount].PosA = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[0]];
                        Triangles[newTriCount].PosB = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[1]];
                        Triangles[newTriCount].PosC = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[2]];
                        if ( pieVersion == 2 )
                        {
                            Triangles[newTriCount].TexCoordA.X = (float)(levels[levelNum].Polygon[a].TexCoord[0].X / 255.0D);
                            Triangles[newTriCount].TexCoordA.Y = (float)(levels[levelNum].Polygon[a].TexCoord[0].Y / 255.0D);
                            Triangles[newTriCount].TexCoordB.X = (float)(levels[levelNum].Polygon[a].TexCoord[1].X / 255.0D);
                            Triangles[newTriCount].TexCoordB.Y = (float)(levels[levelNum].Polygon[a].TexCoord[1].Y / 255.0D);
                            Triangles[newTriCount].TexCoordC.X = (float)(levels[levelNum].Polygon[a].TexCoord[2].X / 255.0D);
                            Triangles[newTriCount].TexCoordC.Y = (float)(levels[levelNum].Polygon[a].TexCoord[2].Y / 255.0D);
                        }
                        else if ( pieVersion == 3 )
                        {
                            Triangles[newTriCount].TexCoordA = levels[levelNum].Polygon[a].TexCoord[0];
                            Triangles[newTriCount].TexCoordB = levels[levelNum].Polygon[a].TexCoord[1];
                            Triangles[newTriCount].TexCoordC = levels[levelNum].Polygon[a].TexCoord[2];
                        }
                        newTriCount++;
                    }
                    else if ( levels[levelNum].Polygon[a].PointCount == 4 )
                    {
                        Quads[newQuadCount].PosA = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[0]];
                        Quads[newQuadCount].PosB = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[1]];
                        Quads[newQuadCount].PosC = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[2]];
                        Quads[newQuadCount].PosD = levels[levelNum].Point[levels[levelNum].Polygon[a].PointNum[3]];
                        if ( pieVersion == 2 )
                        {
                            Quads[newQuadCount].TexCoordA.X = (float)(levels[levelNum].Polygon[a].TexCoord[0].X / 255.0D);
                            Quads[newQuadCount].TexCoordA.Y = (float)(levels[levelNum].Polygon[a].TexCoord[0].Y / 255.0D);
                            Quads[newQuadCount].TexCoordB.X = (float)(levels[levelNum].Polygon[a].TexCoord[1].X / 255.0D);
                            Quads[newQuadCount].TexCoordB.Y = (float)(levels[levelNum].Polygon[a].TexCoord[1].Y / 255.0D);
                            Quads[newQuadCount].TexCoordC.X = (float)(levels[levelNum].Polygon[a].TexCoord[2].X / 255.0D);
                            Quads[newQuadCount].TexCoordC.Y = (float)(levels[levelNum].Polygon[a].TexCoord[2].Y / 255.0D);
                            Quads[newQuadCount].TexCoordD.X = (float)(levels[levelNum].Polygon[a].TexCoord[3].X / 255.0D);
                            Quads[newQuadCount].TexCoordD.Y = (float)(levels[levelNum].Polygon[a].TexCoord[3].Y / 255.0D);
                        }
                        else if ( pieVersion == 3 )
                        {
                            Quads[newQuadCount].TexCoordA = levels[levelNum].Polygon[a].TexCoord[0];
                            Quads[newQuadCount].TexCoordB = levels[levelNum].Polygon[a].TexCoord[1];
                            Quads[newQuadCount].TexCoordC = levels[levelNum].Polygon[a].TexCoord[2];
                            Quads[newQuadCount].TexCoordD = levels[levelNum].Polygon[a].TexCoord[3];
                        }
                        newQuadCount++;
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