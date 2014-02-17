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

        internal static Parser<Token> Token =
            from first in Parse.LetterOrDigit.Once().Text()
            from name in Parse.CharExcept('=').Except(Parse.Char(' ')).Except(General.NewLine).Many().Text()
            from equalSign in Parse.String(" = ")
            from data in Parse.AnyChar.Except(General.NewLine).AtLeastOnce().Text()
            from nl in General.EndOfLineOrFile
            select new Token {
                Name = first + name,
                Data = data
            };

        internal static readonly Parser<string> Section =
            from co in Parse.AnyChar.Except(Parse.Char(']')).Except(General.EndOfLineOrFile).Many().Text()
            from end in Parse.Char(']')
            from nl in General.NewLine.Once()
            select co;

        public static readonly Parser<List<Section>> Ini = 
            from secs in (
                from spaces in Parse.Char (' ').Or (Parse.Char('\t')).Or(Parse.Char ('\r')).Or(Parse.Char('\n')).Many ()
                from section in Parse.Char('[').Then(s => Section).Optional()

                // from section in Section.Optional ()
                from tokens in Token.AtLeastOnce()
                select new Section {
                    Name = section.IsDefined ? section.Get() : RootSectionName,
                    Data = tokens.ToList<Token>()
                }
            ).Many ()
            from nl in General.EndOfLineOrFile

            select secs.ToList<Section>();

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
            from result in Numerics.Int
            from sign in Parse.Char ('%')
            select result;
    }
}

