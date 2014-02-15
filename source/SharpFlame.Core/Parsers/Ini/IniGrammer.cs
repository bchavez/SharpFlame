using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace SharpFlame.Core.Parsers.Ini
{
    public class IniGrammar
    {
        private static string globalSectionName = "Global";
        public static string GlobalSectionName { 
            get { return globalSectionName; }
            set { globalSectionName = value; }      
        } 

        private static readonly Parser<string> newLine =
            Parse.String ("\r\n").XOr (Parse.Char ('\n').Once ().Text ()).Text();

        private static readonly Parser<string> recordTerminator =
            Parse.Return("").End().XOr(
                newLine.End()).Or(
                newLine);

        private static readonly Parser<char> quotedCellDelimiter = Parse.Char('"');

        private static readonly Parser<char> quotedCellContent =
            Parse.AnyChar.Except(quotedCellDelimiter);

        private static readonly Parser<string> quotedText =
            from open in quotedCellDelimiter
            from content in quotedCellContent.Many().Text()
            from end in quotedCellDelimiter
            select content;

        private static readonly Parser<char> content = 
            Parse.AnyChar.Except(Parse.Char('=')).Except(Parse.WhiteSpace).Except(recordTerminator);

        private static readonly Parser<string> Cell =
            quotedText.XOr(Parse.AnyChar.Except(recordTerminator).XMany().Text());

        internal static Parser<Token> Token =
            from name in content.Many().Text()
            from leading in Parse.Char(' ').Or(Parse.Char('\t')).Many()
            from equalSign in Parse.Char('=')
            from trailing in Parse.Char(' ').Or(Parse.Char('\t')).Many()
            from data in Cell
            from nl in recordTerminator.Once()
            select new Token {
                Name = name,
                Data = data
            };

        internal static readonly Parser<string> Section =
            from open in Parse.Char('[')
            from content in Parse.AnyChar.Except(Parse.Char(']')).Except(recordTerminator).AtLeastOnce().Text()
            from end in Parse.Char(']')
            select content;       

        public static readonly Parser<List<Section>> Ini = 
            from secs in (
                    from stripout in
                        (from spaces in Parse.Char (' ').Or (Parse.Char('\t')).Many ()
                         from nl in recordTerminator.AtLeastOnce ()
                         select nl).Many ()
                    
                    from sectionArray in 
                        (from section in Section.Optional ()
                         from tokens in Token.Many ()
                         select new Section {
                            Name = section.IsDefined ? section.ToString() : GlobalSectionName,
                            Data = tokens.ToList<Token>()
                        }).Many ()

                    select sectionArray
                ).Many ()
                from nl in recordTerminator.AtLeastOnce().End()

                select new List<Section> (secs.SelectMany (l => l).ToList<Section> ());

        public static readonly Parser<List<Token>> Tokens = 
            from tokens in Token.Many ()
            select tokens.ToList<Token>();

        // Parses: 1, 0.25, 0.25, 0.5
        public static readonly Parser<Double4> Double4 = 
            from p1 in Parse.Digit.Or(Parse.Char('.')).AtLeastOnce().Text()
            from i1 in Parse.String(", ")
            from p2 in Parse.Digit.Or(Parse.Char('.')).AtLeastOnce().Text()
            from i2 in Parse.String(", ")
            from p3 in Parse.Digit.Or(Parse.Char('.')).AtLeastOnce().Text()
            from i3 in Parse.String(", ")
            from p4 in Parse.Digit.Or(Parse.Char('.')).AtLeastOnce().Text()
            select new Double4 {
                P1 = double.Parse(p1),
                P2 = double.Parse(p2),
                P3 = double.Parse(p3),
                P4 = double.Parse(p4)
            };

        // Parses: 19136, 4288, 0
        public static readonly Parser<Int3> Int3 = 
            from p1 in Parse.Digit.AtLeastOnce().Text()
            from i1 in Parse.String(", ")
            from p2 in Parse.Digit.AtLeastOnce().Text()
            from i2 in Parse.String(", ")
            from p3 in Parse.Digit.AtLeastOnce().Text()
            select new Int3 {
                I1 = Int32.Parse(p1),
                I2 = Int32.Parse(p2),
                I3 = Int32.Parse(p3)
            };
    }
}

