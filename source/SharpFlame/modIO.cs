using System;
using System.Globalization;
using System.IO;
using ICSharpCode.SharpZipLib.Zip;
using Microsoft.VisualBasic;
using SharpFlame.Collections;

namespace SharpFlame
{
    public sealed class modIO
    {
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
                Result.ProblemAdd("Zip entry " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote) + " failed: " +
                                  ex.Message);
                return null;
            }
        }

        public static string InvariantToString_bool(bool Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_byte(byte Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_short(short Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_int(int Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_uint(UInt32 Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_sng(float Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_dbl(double Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static bool InvariantParse_bool(string Text, ref bool Result)
        {
            return bool.TryParse(Text, out Result);
        }

        public static bool InvariantParse_byte(string Text, ref byte Result)
        {
            return byte.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_short(string Text, ref short Result)
        {
            return short.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_ushort(string Text, UInt16 Result)
        {
            return UInt16.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_int(string Text, ref int Result)
        {
            return int.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_uint(string Text, UInt32 Result)
        {
            return UInt32.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_sng(string Text, ref float Result)
        {
            return float.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_dbl(string Text, ref double Result)
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
                Result += Convert.ToString(Strings.Chr(File.ReadByte()));
            }
            return Result;
        }

        public static string ReadOldTextOfLength(BinaryReader File, int Length)
        {
            string Result = "";
            int A = 0;

            for ( A = 0; A <= Length - 1; A++ )
            {
                Result += Convert.ToString(Strings.Chr(File.ReadByte()));
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
                File.Write((byte)(Strings.Asc(Text[A])));
            }
        }

        public static void WriteTextOfLength(BinaryWriter File, int Length, string Text)
        {
            int A = 0;

            for ( A = 0; A <= Math.Min(Text.Length, Length) - 1; A++ )
            {
                File.Write((byte)(Strings.Asc(Text[A])));
            }
            for ( A = Text.Length; A <= Length - 1; A++ )
            {
                File.Write((byte)0);
            }
        }

        public static clsResult WriteMemoryToNewFile(MemoryStream Memory, string Path)
        {
            clsResult ReturnResult = new clsResult("Writing to " + Convert.ToString(ControlChars.Quote) + Path + Convert.ToString(ControlChars.Quote));

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
            clsResult ReturnResult = new clsResult("Writing to zip stream");

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

        public static modProgram.sResult TryOpenFileStream(string Path, ref FileStream Output)
        {
            modProgram.sResult ReturnResult = new modProgram.sResult();
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

        public static bool WZAngleFromINIText(string Text, ref modProgram.sWZAngle Result)
        {
            clsSplitCommaText VectorText = new clsSplitCommaText(Text);
            modProgram.sWZAngle WZAngle = new modProgram.sWZAngle();

            if ( VectorText.PartCount != 3 )
            {
                return false;
            }

            if ( !InvariantParse_ushort(VectorText.Parts[0], WZAngle.Direction) )
            {
                int ErrorValue = 0;
                if ( !InvariantParse_int(VectorText.Parts[0], ref ErrorValue) )
                {
                    return false;
                }
                int Remainder = 0;
                int Multiplier = Math.DivRem(ErrorValue, modProgram.INIRotationMax, out Remainder);
                try
                {
                    if ( Remainder < 0 )
                    {
                        WZAngle.Direction = (ushort)(Remainder + modProgram.INIRotationMax);
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
            if ( !InvariantParse_ushort(VectorText.Parts[1], WZAngle.Pitch) )
            {
                return false;
            }
            if ( !InvariantParse_ushort(VectorText.Parts[2], WZAngle.Roll) )
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
            if ( !InvariantParse_int(Text, ref Health) )
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

        public static bool WorldPosFromINIText(string Text, ref modProgram.clsWorldPos Result)
        {
            clsSplitCommaText VectorText = new clsSplitCommaText(Text);
            int A = 0;
            int B = 0;

            if ( VectorText.PartCount != 3 )
            {
                return false;
            }
            int[] Positions = new int[3];
            for ( A = 0; A <= 2; A++ )
            {
                if ( InvariantParse_int(VectorText.Parts[A], ref B) )
                {
                    Positions[A] = B;
                }
                else
                {
                    return false;
                }
            }
            Result = new modProgram.clsWorldPos(new modProgram.sWorldPos(new modMath.sXY_int(Positions[0], Positions[1]), Positions[2]));
            return true;
        }

        public static clsZipStreamEntry FindZipEntryFromPath(string Path, string ZipPathToFind)
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
                    clsZipStreamEntry Result = new clsZipStreamEntry();
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
                        case ControlChars.Cr:
                        case ControlChars.Lf:
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

    public class clsPositionFromText
    {
        public modMath.sXY_int Pos;

        public bool Translate(string Text)
        {
            int A = 0;
            clsSplitCommaText Positions = new clsSplitCommaText(Text);

            if ( Positions.PartCount < 2 )
            {
                return false;
            }
            if ( modIO.InvariantParse_int(Positions.Parts[0], ref A) )
            {
                Pos.X = A;
            }
            else
            {
                return false;
            }
            if ( modIO.InvariantParse_int(Positions.Parts[1], ref A) )
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

    public class clsZipStreamEntry
    {
        public ZipInputStream Stream;
        public ZipEntry Entry;
    }

    public class clsSplitCommaText
    {
        public string[] Parts;
        public int PartCount;

        public clsSplitCommaText(string Text)
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