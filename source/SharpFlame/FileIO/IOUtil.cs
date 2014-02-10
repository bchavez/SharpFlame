using NLog;
using System;
using System.Globalization;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using SharpFlame.Collections;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.FileIO
{
    public static class IOUtil
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

        public static ZipEntry ZipMakeEntry(ZipOutputStream ZipOutputStream, string Path, clsResult Result)
        {
            try
            {
                ZipEntry NewZipEntry = new ZipEntry(Path);
                NewZipEntry.DateTime = DateTime.Now;
                ZipOutputStream.PutNextEntry(NewZipEntry);
                return NewZipEntry;
            }
            catch ( Exception ex )
            {
                Result.ProblemAdd("Zip entry \"" + Path + "\" failed: " +
                                  ex.Message);
                return null;
            }
        }

        public static bool InvariantParse(string Text, ref bool Result)
        {
            return bool.TryParse(Text, out Result);
        }

        public static bool InvariantParse(string Text, ref byte Result)
        {
            return byte.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse(string Text, ref short Result)
        {
            return short.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse(string Text, UInt16 Result)
        {
            return UInt16.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse(string Text, ref int Result)
        {
            return int.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse(string Text, UInt32 Result)
        {
            return UInt32.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse(string Text, ref float Result)
        {
            return float.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse(string Text, ref double Result)
        {
            return double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static string ReadOldText(BinaryReader File)
        {
            string Result = "";
            int A = 0;
            int Length = (int)(File.ReadUInt32());

            for ( A = 0; A <= Length - 1; A++ )
            {
                Result += Convert.ToString((char)File.ReadByte());
            }
            return Result;
        }

        public static string ReadOldTextOfLength(BinaryReader File, int Length)
        {
            string Result = "";
            int A = 0;

            for ( A = 0; A <= Length - 1; A++ )
            {
                Result += Convert.ToString((char)File.ReadByte());
            }
            return Result;
        }

        public static void WriteText(BinaryWriter File, bool WriteLength, string Text)
        {
            if ( WriteLength )
            {
                File.Write(Convert.ToBoolean((uint)Text.Length));
            }
            int A = 0;
            for ( A = 0; A <= Text.Length - 1; A++ )
            {
                File.Write((byte)Text[A]);
            }
        }

        public static void WriteTextOfLength(BinaryWriter File, int Length, string Text)
        {
            int A = 0;

            for ( A = 0; A <= Math.Min(Text.Length, Length) - 1; A++ )
            {
                File.Write((byte)Text[A]);
            }
            for ( A = Text.Length; A <= Length - 1; A++ )
            {
                File.Write((byte)0);
            }
        }

        public static clsResult WriteMemoryToNewFile(MemoryStream Memory, string Path)
        {
            clsResult ReturnResult = new clsResult("Writing to \"{0}".Format2(Path), false);
            logger.Info ("Writing to \"{0}".Format2(Path));

            FileStream NewFile = default(FileStream);
            try
            {
                NewFile = new FileStream(Path, FileMode.CreateNew);
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }
            try
            {
                Memory.WriteTo(NewFile);
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            Memory.Close();
            NewFile.Close();

            return ReturnResult;
        }

        public static clsResult WriteMemoryToZipEntryAndFlush(MemoryStream Memory, ZipOutputStream Stream)
        {
            clsResult ReturnResult = new clsResult("Writing to zip stream", false);
            logger.Info ("Writing to zip stream");

            try
            {
                Memory.WriteTo(Stream);
                Memory.Flush();
                Stream.Flush();
                Stream.CloseEntry();
            }
            catch ( Exception ex )
            {
                ReturnResult.ProblemAdd(ex.Message);
                return ReturnResult;
            }

            return ReturnResult;
        }

        public static sResult TryOpenFileStream(string Path, ref FileStream Output)
        {
            sResult ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            try
            {
                Output = new FileStream(Path, FileMode.Open);
            }
            catch ( Exception ex )
            {
                Output = null;
                ReturnResult.Problem = ex.Message;
                return ReturnResult;
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }

        public static bool WZAngleFromINIText(string Text, ref sWZAngle Result)
        {
            SplitCommaText VectorText = new SplitCommaText(Text);
            sWZAngle WZAngle = new sWZAngle();

            if ( VectorText.PartCount != 3 )
            {
                return false;
            }

            if ( !InvariantParse(VectorText.Parts[0], WZAngle.Direction) )
            {
                int ErrorValue = 0;
                if ( !InvariantParse(VectorText.Parts[0], ref ErrorValue) )
                {
                    return false;
                }
                int Remainder = 0;
                int Multiplier = Math.DivRem(ErrorValue, App.INIRotationMax, out Remainder);
                try
                {
                    if ( Remainder < 0 )
                    {
                        WZAngle.Direction = (ushort)(Remainder + App.INIRotationMax);
                    }
                    else
                    {
                        WZAngle.Direction = (ushort)Remainder;
                    }
                }
                catch ( Exception )
                {
                    return false;
                }
                return true;
            }
            if ( !InvariantParse(VectorText.Parts[1], WZAngle.Pitch) )
            {
                return false;
            }
            if ( !InvariantParse(VectorText.Parts[2], WZAngle.Roll) )
            {
                return false;
            }
            Result = WZAngle;
            return true;
        }

        public static bool HealthFromINIText(string Text, ref int Result)
        {
            int A = 0;
            int Health = 0;

            A = Text.IndexOf('%');
            if ( A < 0 )
            {
                return false;
            }
            Text = Text.Replace("%", "");
            if ( !InvariantParse(Text, ref Health) )
            {
                return false;
            }
            if ( Health < 0 | Health > 100 )
            {
                return false;
            }
            Result = Health;
            return true;
        }

        public static bool WorldPosFromINIText(string Text, ref clsWorldPos Result)
        {
            SplitCommaText VectorText = new SplitCommaText(Text);
            int A = 0;
            int B = 0;

            if ( VectorText.PartCount != 3 )
            {
                return false;
            }
            int[] Positions = new int[3];
            for ( A = 0; A <= 2; A++ )
            {
                if ( InvariantParse(VectorText.Parts[A], ref B) )
                {
                    Positions[A] = B;
                }
                else
                {
                    return false;
                }
            }
            Result = new clsWorldPos(new sWorldPos(new sXY_int(Positions[0], Positions[1]), Positions[2]));
            return true;
        }

        public static ZipStreamEntry FindZipEntryFromPath(string Path, string ZipPathToFind)
        {
            ZipInputStream ZipStream = default(ZipInputStream);
            ZipEntry ZipEntry = default(ZipEntry);
            string FindPath = ZipPathToFind.ToLower().Replace('\\', '/');
            string ZipPath;

            ZipStream = new ZipInputStream(File.OpenRead(Path));
            do
            {
                try
                {
                    ZipEntry = ZipStream.GetNextEntry();
                }
                catch ( Exception )
                {
                    goto endOfDoLoop;
                }
                if ( ZipEntry == null )
                {
                    break;
                }

                ZipPath = ZipEntry.Name.ToLower().Replace('\\', '/');
                if ( ZipPath == FindPath )
                {
                    ZipStreamEntry Result = new ZipStreamEntry();
                    Result.Stream = ZipStream;
                    Result.Entry = ZipEntry;
                    return Result;
                }
            } while ( true );
            endOfDoLoop:
            ZipStream.Close();

            return null;
        }

        public static SimpleList<string> BytesToLinesRemoveComments(BinaryReader reader)
        {
            char CurrentChar = (char)0;
            bool CurrentCharExists = default(bool);
            bool InLineComment = default(bool);
            bool InCommentBlock = default(bool);
            char PrevChar = (char)0;
            bool PrevCharExists = default(bool);
            string Line = "";
            SimpleList<string> Result = new SimpleList<string>();

            do
            {
                MonoContinueDo:
                PrevChar = CurrentChar;
                PrevCharExists = CurrentCharExists;
                try
                {
                    CurrentChar = reader.ReadChar();
                    CurrentCharExists = true;
                }
                catch ( Exception )
                {
                    CurrentCharExists = false;
                }
                if ( CurrentCharExists )
                {
                    switch ( CurrentChar )
                    {
                        case '\r':
                        case '\n':
                            InLineComment = false;
                            if ( PrevCharExists )
                            {
                                Line += PrevChar.ToString();
                            }
                            CurrentCharExists = false;

                            if ( Line.Length > 0 )
                            {
                                Result.Add(Line);
                                Line = "";
                            }

                            goto MonoContinueDo;
                        case '*':
                            if ( PrevCharExists && PrevChar == '/' )
                            {
                                InCommentBlock = true;
                                CurrentCharExists = false;
                                goto MonoContinueDo;
                            }
                            break;
                        case '/':
                            if ( PrevCharExists )
                            {
                                if ( PrevChar == '/' )
                                {
                                    InLineComment = true;
                                    CurrentCharExists = false;
                                    goto MonoContinueDo;
                                }
                                else if ( PrevChar == '*' )
                                {
                                    InCommentBlock = false;
                                    CurrentCharExists = false;
                                    goto MonoContinueDo;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    InLineComment = false;
                    if ( PrevCharExists )
                    {
                        Line += PrevChar.ToString();
                    }
                    if ( Line.Length > 0 )
                    {
                        Result.Add(Line);
                        Line = "";
                    }

                    break;
                }
                if ( PrevCharExists )
                {
                    if ( !(InCommentBlock || InLineComment) )
                    {
                        Line += PrevChar.ToString();
                    }
                }
            } while ( true );

            return Result;
        }
    }

    public class PositionFromText
    {
        public sXY_int Pos;

        public bool Translate(string Text)
        {
            int A = 0;
            SplitCommaText Positions = new SplitCommaText(Text);

            if ( Positions.PartCount < 2 )
            {
                return false;
            }
            if ( IOUtil.InvariantParse(Positions.Parts[0], ref A) )
            {
                Pos.X = A;
            }
            else
            {
                return false;
            }
            if ( IOUtil.InvariantParse(Positions.Parts[1], ref A) )
            {
                Pos.Y = A;
            }
            else
            {
                return false;
            }
            return true;
        }
    }

    public class ZipStreamEntry
    {
        public ZipInputStream Stream;
        public ZipEntry Entry;
    }

    public class SplitCommaText
    {
        public string[] Parts;
        public int PartCount;

        public SplitCommaText(string Text)
        {
            int A = 0;

            Parts = Text.Split(',');
            PartCount = Parts.GetUpperBound(0) + 1;
            for ( A = 0; A <= PartCount - 1; A++ )
            {
                Parts[A] = Convert.ToString(Parts[A].Trim());
            }
        }
    }
}