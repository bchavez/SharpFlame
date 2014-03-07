namespace SharpFlame.Domain.ObjData
{
    public struct sBytes
    {
        public byte[] Bytes;
    }

    public class clsTexturePage
    {
        public string FileTitle;
        public int GLTexture_Num;
    }

    public class clsPIE
    {
        public string LCaseFileTitle;
        public clsModel Model;
        public string Path;
    }

    internal struct BodyProp
    {
        public string LeftPIE;
        public string RightPIE;
    }

    public struct sLines
    {
        public string[] Lines;

        public void RemoveComments()
        {
            var lineNum = 0;
            var lineCount = Lines.GetUpperBound( 0 ) + 1;
            var inCommentBlock = default( bool );
            var commentStart = 0;

            for( lineNum = 0; lineNum <= lineCount - 1; lineNum++ )
            {
                var charNum = 0;
                if( inCommentBlock )
                {
                    commentStart = 0;
                }
                do
                {
                    if( charNum >= Lines[lineNum].Length )
                    {
                        if( inCommentBlock )
                        {
                            Lines[lineNum] = Lines[lineNum].Substring( 0, commentStart );
                        }
                        break;
                    }
                    var commentLength = 0;
                    if( inCommentBlock )
                    {
                        if( Lines[lineNum][charNum] == '*' )
                        {
                            charNum++;
                            if( charNum >= Lines[lineNum].Length )
                            {
                            }
                            else if( Lines[lineNum][charNum] == '/' )
                            {
                                charNum++;
                                commentLength = charNum - commentStart;
                                inCommentBlock = false;
                                Lines[lineNum] = Lines[lineNum].Substring( commentStart, Lines[lineNum].Length - commentStart )
                                    .Substring( commentStart + commentLength, Lines[lineNum].Length - commentStart - commentLength );
                                charNum -= commentLength;
                            }
                        }
                        else
                        {
                            charNum++;
                        }
                    }
                    else if( Lines[lineNum][charNum] == '/' )
                    {
                        charNum++;
                        if( charNum >= Lines[lineNum].Length )
                        {
                        }
                        else if( Lines[lineNum][charNum] == '/' )
                        {
                            commentStart = charNum - 1;
                            charNum = Lines[lineNum].Length;
                            commentLength = charNum - commentStart;
                            Lines[lineNum] = Lines[lineNum].Substring( commentStart, Lines[lineNum].Length - commentStart )
                                .Substring( commentStart + commentLength, Lines[lineNum].Length - commentStart - commentLength );
                            charNum -= commentLength;
                            break;
                        }
                        else if( Lines[lineNum][charNum] == '*' )
                        {
                            commentStart = charNum - 1;
                            charNum++;
                            inCommentBlock = true;
                        }
                    }
                    else
                    {
                        charNum++;
                    }
                } while( true );
            }
        }
    }
}