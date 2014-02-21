using System;
using System.IO;
using SharpFlame.Collections;
using SharpFlame.Core;
using SharpFlame.Core.Collections;

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
            foreach ( var text in ResultData )
            {
                if ( text.GetLength(0) != FieldCount )
                {
                    return false;
                }
            }

            return true;
        }

        public bool CalcUniqueField()
        {
            if ( UniqueField >= 0 )
            {
                for ( var a = 0; a <= ResultData.Count - 1; a++ )
                {
                    var text = Convert.ToString(ResultData[a][UniqueField]);
                    for ( var b = a + 1; b <= ResultData.Count - 1; b++ )
                    {
                        if ( text == ResultData[b][UniqueField] )
                        {
                            return false;
                        }
                    }
                }
            }

            return true;
        }

        public Result LoadCommaFile(string Path)
        {
            var result = new Result(String.Format("Loading comma separated file \"{0}\"", SubDirectory));
            StreamReader reader;

            try
            {
                reader = new StreamReader(Path + SubDirectory, App.UTF8Encoding);
            }
            catch ( Exception ex )
            {
                result.ProblemAdd(ex.Message);
                return result;
            }

            while ( !reader.EndOfStream )
            {
                var line = reader.ReadLine();
                line = line.Trim();
                if ( line.Length > 0 )
                {
                    string[] lineFields = line.Split(',');
                    for ( var a = 0; a <= lineFields.GetUpperBound(0); a++ )
                    {
                        lineFields[a] = lineFields[a].Trim();
                    }
                    ResultData.Add(lineFields);
                }
            }

            reader.Close();

            return result;
        }

        public Result LoadNamesFile(string path)
        {
            var result = new Result(String.Format("Loading names file \"{0}\"", SubDirectory));
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
                var prevChar = currentChar;
                var prevCharExists = currentCharExists;
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
                            if ( prevCharExists )
                            {
                                line += prevChar.ToString();
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
                            if ( prevCharExists && prevChar == '/' )
                            {
                                inCommentBlock = true;
                                currentCharExists = false;
                                goto MonoContinueDo;
                            }
                            break;
                        case '/':
                            if ( prevCharExists )
                            {
                                if ( prevChar == '/' )
                                {
                                    inLineComment = true;
                                    currentCharExists = false;
                                    goto MonoContinueDo;
                                }
                                if ( prevChar == '*' )
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
                    if ( prevCharExists )
                    {
                        line += prevChar.ToString();
                    }
                    if ( line.Length > 0 )
                    {
                        var endCodeTab = line.IndexOf('\t');
                        var endCodeSpace = line.IndexOf(' ');
                        var endCode = endCodeTab;
                        if ( endCodeSpace >= 0 && (endCodeSpace < endCode | endCode < 0) )
                        {
                            endCode = endCodeSpace;
                        }
                        if ( endCode >= 0 )
                        {
                            var firstQuote = line.IndexOf('"', endCode + 1, line.Length - (endCode + 1));
                            if ( firstQuote >= 0 )
                            {
                                var secondQuote = line.IndexOf('"', firstQuote + 1, line.Length - (firstQuote + 1));
                                if ( secondQuote >= 0 )
                                {
                                    var value = new string[2];
                                    value[0] = line.Substring(0, endCode);
                                    value[1] = line.Substring(firstQuote + 1, secondQuote - (firstQuote + 1));
                                    ResultData.Add(value);
                                }
                            }
                        }
                        line = "";
                    }

                    break;
                }
                if ( prevCharExists )
                {
                    if ( !(inCommentBlock || inLineComment) )
                    {
                        line += prevChar.ToString();
                    }
                }
            } while ( true );

            reader.Close();

            return result;
        }
    }
}