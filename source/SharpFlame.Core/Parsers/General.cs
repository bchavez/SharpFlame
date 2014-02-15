using System;
using System.Linq;
using Sprache;

namespace SharpFlame.Core.Parsers
{
    public class General
    {
        internal static readonly Parser<string> NewLine =
            Parse.String ("\r\n").XOr (Parse.Char ('\n').Once ().Text ()).Text();

        internal static readonly Parser<string> EndOfLineOrFile =
            Parse.Return("").End().XOr(
                NewLine.End()).Or(
                NewLine);

        internal static readonly Parser<char> quotedCellDelimiter = Parse.Char('"');

        internal static readonly Parser<char> quotedCellContent =
            Parse.AnyChar.Except(quotedCellDelimiter);

        internal static readonly Parser<string> QuotedText =
            from open in quotedCellDelimiter
            from content in quotedCellContent.Many().Text()
            from end in quotedCellDelimiter
            select content;

        internal static readonly Parser<string> Cell =
            QuotedText.XOr(Parse.AnyChar.Except(EndOfLineOrFile).XMany().Text());

        internal static readonly Parser<string> SingleLineComment =
            from ignore in Parse.String ("//")
                from comment in Parse.AnyChar.Except(General.EndOfLineOrFile).Many().Text()
                from nl in General.EndOfLineOrFile.Text()
                select comment;

        private static Parser<T> endOfComment<T> (Parser<T> following)
        {
            return from escape in Parse.Char ('*')
                from f in following
                    select f;
        }

        internal static readonly Parser<string> MultilineComment =
            (from open in Parse.String ("/*")
             from comment in Parse.AnyChar.Except (endOfComment (Parse.Char ('/'))).Many ().Text ()
             from close in Parse.String ("*/")
             select comment).Token ();


    }
}

