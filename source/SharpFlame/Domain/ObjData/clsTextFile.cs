using System;
using System.IO;
using SharpFlame.Collections;

namespace SharpFlame.Domain.ObjData
{
    public class clsTextFile
    {
        public int FieldCount = 0;

        public SimpleList<string[]> ResultData = new SimpleList<string[]>();
        public string SubDirectory;
        public int UniqueField = 0;

        public bool CalcIsFieldCountValid()
        {
            string[] Text = null;
            foreach ( var tempLoopVar_Text in ResultData )
            {
                Text = tempLoopVar_Text;
                if ( Text.GetLength(0) != FieldCount )
                {
                    return false;
                }
            }

            return true;
        }

        public bool CalcUniqueField()
        {
            var A = 0;
            var B = 0;
            string Text;

            if ( UniqueField >= 0 )
            {
                for ( A = 0; A <= ResultData.Count - 1; A++ )
                {
                    Text = Convert.ToString(ResultData[A][UniqueField]);
                    for ( B = A + 1; B <= ResultData.Count - 1; B++ )
                    {
                        if ( Text == ResultData[B][UniqueField] )
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public clsResult LoadCommaFile(string Path)
        {
            var Result = new clsResult(String.Format("Loading comma separated file \"{0}\"", SubDirectory));
            var Reader = default(StreamReader);

            try
            {
                Reader = new StreamReader(Path + SubDirectory, App.UTF8Encoding);
            }
            catch ( Exception ex )
            {
                Result.ProblemAdd(ex.Message);
                return Result;
            }

            var Line = "";
            string[] LineFields = null;
            var A = 0;

            while ( !Reader.EndOfStream )
            {
                Line = Reader.ReadLine();
                Line = Line.Trim();
                if ( Line.Length > 0 )
                {
                    LineFields = Line.Split(',');
                    for ( A = 0; A <= LineFields.GetUpperBound(0); A++ )
                    {
                        LineFields[A] = LineFields[A].Trim();
                    }
                    ResultData.Add(LineFields);
                }
            }

            Reader.Close();

            return Result;
        }

        public clsResult LoadNamesFile(string path)
        {
            var result = new clsResult(String.Format("Loading names file \"{0}\"", SubDirectory));
            FileStream file;
            BinaryReader reader;

            try
            {
                file = new FileStream(path + SubDirectory, FileMode.Open);
            }
            catch ( Exception ex )
            {
                result.ProblemAdd(ex.Message);
                return result;
            }

            try
            {
                reader = new BinaryReader(file, App.UTF8Encoding);
            }
            catch ( Exception ex )
            {
                file.Close();
                result.ProblemAdd(ex.Message);
                return result;
            }

            var currentChar = (char)0;
            var inLineComment = false;
            var inCommentBlock = false;
            var line = "";
            var currentCharExists = false;

            do
            {
                MonoContinueDo:
                var PrevChar = currentChar;
                var PrevCharExists = currentCharExists;
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
                            inLineComment = false;
                            if ( PrevCharExists )
                            {
                                line += PrevChar.ToString();
                            }
                            currentCharExists = false;

                            if ( line.Length > 0 )
                            {
                                var EndCodeTab = line.IndexOf('\t');
                                var EndCodeSpace = line.IndexOf(' ');
                                var EndCode = EndCodeTab;
                                if ( EndCodeSpace >= 0 && (EndCodeSpace < EndCode | EndCode < 0) )
                                {
                                    EndCode = EndCodeSpace;
                                }
                                if ( EndCode >= 0 )
                                {
                                    var FirstQuote = line.IndexOf('"', EndCode + 1, line.Length - (EndCode + 1));
                                    if ( FirstQuote >= 0 )
                                    {
                                        var SecondQuote = line.IndexOf('"', FirstQuote + 1, line.Length - (FirstQuote + 1));
                                        if ( SecondQuote >= 0 )
                                        {
                                            var Value = new string[2];
                                            Value[0] = line.Substring(0, EndCode);
                                            Value[1] = line.Substring(FirstQuote + 1, SecondQuote - (FirstQuote + 1));
                                            ResultData.Add(Value);
                                        }
                                    }
                                }
                                line = "";
                            }

                            goto MonoContinueDo;
                        case '*':
                            if ( PrevCharExists && PrevChar == '/' )
                            {
                                inCommentBlock = true;
                                currentCharExists = false;
                                goto MonoContinueDo;
                            }
                            break;
                        case '/':
                            if ( PrevCharExists )
                            {
                                if ( PrevChar == '/' )
                                {
                                    inLineComment = true;
                                    currentCharExists = false;
                                    goto MonoContinueDo;
                                }
                                if ( PrevChar == '*' )
                                {
                                    inCommentBlock = false;
                                    currentCharExists = false;
                                    goto MonoContinueDo;
                                }
                            }
                            break;
                    }
                }
                else
                {
                    if ( PrevCharExists )
                    {
                        line += PrevChar.ToString();
                    }
                    if ( line.Length > 0 )
                    {
                        var EndCodeTab = line.IndexOf('\t');
                        var EndCodeSpace = line.IndexOf(' ');
                        var EndCode = EndCodeTab;
                        if ( EndCodeSpace >= 0 && (EndCodeSpace < EndCode | EndCode < 0) )
                        {
                            EndCode = EndCodeSpace;
                        }
                        if ( EndCode >= 0 )
                        {
                            var FirstQuote = line.IndexOf('"', EndCode + 1, line.Length - (EndCode + 1));
                            if ( FirstQuote >= 0 )
                            {
                                var SecondQuote = line.IndexOf('"', FirstQuote + 1, line.Length - (FirstQuote + 1));
                                if ( SecondQuote >= 0 )
                                {
                                    var Value = new string[2];
                                    Value[0] = line.Substring(0, EndCode);
                                    Value[1] = line.Substring(FirstQuote + 1, SecondQuote - (FirstQuote + 1));
                                    ResultData.Add(Value);
                                }
                            }
                        }
                        line = "";
                    }

                    break;
                }
                if ( PrevCharExists )
                {
                    if ( !(inCommentBlock || inLineComment) )
                    {
                        line += PrevChar.ToString();
                    }
                }
            } while ( true );

            reader.Close();

            return result;
        }
    }
}