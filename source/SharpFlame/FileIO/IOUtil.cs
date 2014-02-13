using NLog;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using SharpFlame.Collections;
using SharpFlame.Maths;
using SharpFlame.Util;

namespace SharpFlame.FileIO
{
    public static class IOUtil
    {
        private static Logger logger = LogManager.GetCurrentClassLogger();

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

        public static bool InvariantParse(string Text, ref UInt32 Result)
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
                File.Write((uint)Text.Length);
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

        public static List<string> BytesToLinesRemoveComments(BinaryReader reader)
        {
            char currentChar = (char)0;
            bool currentCharExists = false;
            bool inLineComment = false;
            bool inCommentBlock = false;
            char prevChar = (char)0;
            bool prevCharExists = false;
            string Line = "";
            List<string> result = new List<string>();

            do
            {
                prevChar = currentChar;
                prevCharExists = currentCharExists;

                try
                {
                    currentChar = reader.ReadChar();
                    currentCharExists = true;
                }
                catch ( Exception )
                {
                    currentCharExists = false;
                }

                if ( currentCharExists )
                {
                    switch ( currentChar )
                    {
                        case '\r':
                        case '\n':
                        if (!inLineComment) {    
                            if ( prevCharExists )
                            {
                                Line += prevChar.ToString();
                            }

                            if ( Line.Length > 0 )
                            {   
                                result.Add(Line);
                                Line = "";
                            }
                        }

                        inLineComment = false;
                        currentCharExists = false;
                        continue;
                        case '*':
                        if ( prevCharExists && prevChar == '/' )
                        {
                            inCommentBlock = true;
                            currentCharExists = false;
                            continue;
                        }
                        break;
                        case '/':
                        if ( prevCharExists )
                        {
                            if ( prevChar == '/' )
                            {
                                inLineComment = true;
                                currentCharExists = false;
                                continue;
                            }
                            else if ( prevChar == '*' )
                            {
                                inCommentBlock = false;
                                currentCharExists = false;
                                continue;
                            }
                        }
                        break;
                    }
                }
                else
                {
                    inLineComment = false;
                    if ( prevCharExists )
                    {
                        Line += prevChar.ToString();
                    }
                    if ( Line.Length > 0 )
                    {
                        result.Add(Line);
                        Line = "";
                    }

                    break;
                }
                if ( prevCharExists )
                {
                    if ( !(inCommentBlock || inLineComment) )
                    {
                        Line += prevChar.ToString();
                    }
                }
            } while ( true );

            return result;  
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