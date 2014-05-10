
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using NLog;
using SharpFlame.Core.Extensions;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Util;


namespace SharpFlame.FileIO
{
    public static class IOUtil
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();

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
            var Result = "";
            var A = 0;
            var Length = (int)(File.ReadUInt32());

            for ( A = 0; A <= Length - 1; A++ )
            {
                Result += Convert.ToString((char)File.ReadByte());
            }
            return Result;
        }

        public static string ReadOldTextOfLength(BinaryReader File, int Length)
        {
            var Result = "";
            var A = 0;

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
            var A = 0;
            for ( A = 0; A <= Text.Length - 1; A++ )
            {
                File.Write((byte)Text[A]);
            }
        }

        public static void WriteTextOfLength(BinaryWriter File, int Length, string Text)
        {
            var A = 0;

            for ( A = 0; A <= Math.Min(Text.Length, Length) - 1; A++ )
            {
                File.Write((byte)Text[A]);
            }
            for ( A = Text.Length; A <= Length - 1; A++ )
            {
                File.Write((byte)0);
            }
        }

        public static Result WriteMemoryToNewFile(MemoryStream Memory, string Path)
        {
            var ReturnResult = new Result("Writing to \"{0}".Format2(Path), false);
            logger.Info("Writing to \"{0}".Format2(Path));

            var NewFile = default(FileStream);
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

        public static SimpleResult TryOpenFileStream(string Path, ref FileStream Output)
        {
            var ReturnResult = new SimpleResult();
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

        public static List<string> BytesToLinesRemoveComments(BinaryReader reader)
        {
            var currentChar = (char)0;
            var currentCharExists = false;
            var inLineComment = false;
            var inCommentBlock = false;
            var prevChar = (char)0;
            var prevCharExists = false;
            var Line = "";
            var result = new List<string>();

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
                            if ( !inLineComment )
                            {
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
                                if ( prevChar == '*' )
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
        public XYInt Pos;

        public bool Translate(string Text)
        {
            var A = 0;
            var Positions = new SplitCommaText(Text);

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
        public int PartCount;
        public string[] Parts;

        public SplitCommaText(string Text)
        {
            var A = 0;

            Parts = Text.Split(',');
            PartCount = Parts.GetUpperBound(0) + 1;
            for ( A = 0; A <= PartCount - 1; A++ )
            {
                Parts[A] = Convert.ToString(Parts[A].Trim());
            }
        }
    }
}