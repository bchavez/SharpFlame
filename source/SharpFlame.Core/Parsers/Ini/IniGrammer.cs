using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using SharpFlame.Core.Parsers;
using Sprache;

namespace SharpFlame.Core.Parsers.Ini
{
    public class IniGrammar
    {
        private static string rootSectionName = "Global";
        public static string RootSectionName { 
            get { return rootSectionName; }
            set { rootSectionName = value; }      
        }       

        private static readonly Parser<char> content = 
            Parse.AnyChar.Except(Parse.Char('=')).Except(Parse.WhiteSpace).Except(General.EndOfLineOrFile);

        internal static Parser<Token> Token =
            from name in content.Many().Text()
            from leading in Parse.Char(' ').Or(Parse.Char('\t')).Many()
            from equalSign in Parse.Char('=')
            from trailing in Parse.Char(' ').Or(Parse.Char('\t')).Many()
            from data in General.Cell
            from nl in General.EndOfLineOrFile.Once()
            select new Token {
                Name = name,
                Data = data
            };

        internal static readonly Parser<string> Section =
            from open in Parse.Char('[')
            from content in Parse.AnyChar.Except(Parse.Char(']')).Except(General.EndOfLineOrFile).AtLeastOnce().Text()
            from end in Parse.Char(']')
            from nl in General.EndOfLineOrFile.Once()
            select content;       

        public static readonly Parser<List<Section>> Ini = 
            from secs in (
                    from stripout in
                        (from spaces in Parse.Char (' ').Or (Parse.Char('\t')).Many ()
                         from nl in General.EndOfLineOrFile.AtLeastOnce ()
                         select nl).Many ()
                    
                    from sectionArray in 
                        (from section in Section.Optional ()
                         from tokens in Token.Many ()
                         select new Section {
                            Name = section.IsDefined ? section.Get() : RootSectionName,
                            Data = tokens.ToList<Token>()
                        }).Many ()

                    select sectionArray
                ).Many ()
                from nl in General.EndOfLineOrFile.AtLeastOnce().End()

                select new List<Section> (secs.SelectMany (l => l).ToList<Section> ());

        public static readonly Parser<List<Token>> Tokens = 
            from tokens in Token.Many ()
            select tokens.ToList<Token>();

        // Parses: 1, 0.25, 0.25, 0.5
        public static readonly Parser<Double4> Double4 = 
            from p1 in Numerics.Double
            from i1 in Parse.String(", ")
            from p2 in Numerics.Double
            from i2 in Parse.String(", ")
            from p3 in Numerics.Double
            from i3 in Parse.String(", ")
            from p4 in Numerics.Double
            select new Double4 {
                P1 = p1,
                P2 = p2,
                P3 = p3,
                P4 = p4
            };       

        // Parses: 19136, 4288, 0
        public static readonly Parser<Int3> Int3 = 
            from p1 in Numerics.Int
            from i1 in Parse.String(", ")
            from p2 in Numerics.Int
            from i2 in Parse.String(", ")
            from p3 in Numerics.Int
            select new Int3 {
                I1 = p1,
                I2 = p2,
                I3 = p3
            };

        // Parses: %100
        public static readonly Parser<int> Health =
            from sign in Parse.Char ('%')
                from result in Numerics.Int
            select result;
    }
}

