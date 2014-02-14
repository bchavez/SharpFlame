using System;
using System.Collections.Generic;
using Sprache;

namespace SharpFlame.Core.Parsers.Lev
{
    public class LevGrammar
    {
        private static readonly Parser<string> newLine =
            Parse.String(Environment.NewLine).Text();

        private static Parser<T> endOfComment<T> (Parser<T> following)
        {
            return from escape in Parse.Char ('*')
                   from f in following
                    select f;
        }

        internal static readonly Parser<string> Comment =
            (from open in Parse.String ("/*")
             from comment in Parse.AnyChar.Except (endOfComment (Parse.Char ('/'))).Many ().Text ()
             from close in Parse.String ("*/")
             select comment).Token ();

        internal static readonly Parser<string> QuotedText =
            (from open in Parse.Char ('"')
             from content in Parse.CharExcept ('"').Many ().Text ()
             from close in Parse.Char ('"')
             select content).Token();

        /**
         * Parses:
         * text     "Tmp"
         * text     Tmp
         */
        internal static Parser<Identifier> Identifier =
            from name in Parse.Letter.AtLeastOnce ().Text ().Token ()
            from data in QuotedText.Or(Parse.AnyChar.Many().Text().Token())
                select new Identifier {
                        Name = name,
                        Data = data
                };

        //level   test_flame-T1
        public static readonly Parser<string> Level =
            from level in Parse.String ("level")
                from name in Parse.AnyChar.AtLeastOnce ().Token ().Text ()
                select name;

        //players 2
        public static readonly Parser<int> Players =
            from players in Parse.String ("players")
                from number in Parse.Number.Token ()
                let n = int.Parse (number)
                select n;

        //type 14
        public static readonly Parser<int> Type = 
            from type in Parse.String ("type")
                from number in Parse.Number.Token ()
                let n = int.Parse (number)
                select n;

        //dataset MULTI_CAM_1
        public static readonly Parser<string> Dataset = 
            from dataset in Parse.String ("dataset")
                from name in Parse.AnyChar.AtLeastOnce ().Token ().Text ()
                select name;

        //game    "multiplay/maps/2c-Tinny-War.gam"
        public static readonly Parser<string> Game = 
            from game in Parse.String ("game")
                from name in QuotedText
                select name;

        //data    "wrf/multi/skirmish2.wrf"
        //data    "wrf/multi/fog1.wrf"
        public static readonly Parser<string> Data = 
            from game in Parse.String ("data")
                from name in QuotedText
                select name;

        internal static Parser<Campaign> Campaign = 
            from data in Data.Many ()
                select new Campaign 
                    {   
                        Data = (List<string>)data
                    };

        internal static Parser<Identifier> CommentOrIdentifier = 
            from c in Comment.Many ()
            from i in Identifier
                select i;

        internal static Parser<Identifier> Lev = 
            from ident in CommentOrIdentifier
                select ident;

    }
}