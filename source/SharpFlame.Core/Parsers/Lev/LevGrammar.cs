using System;
using System.Collections.Generic;
using System.Linq;
using Sprache;

namespace SharpFlame.Core.Parsers.Lev
{
    public class LevGrammar
    {
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

        //level   Tinny-War-T3
        //players 2
        //type    19
        //dataset MULTI_T3_C1
        //game    ""multiplay/maps/2c-Tinny-War.gam""
        //data    ""wrf/multi/t3-skirmish2.wrf""
        //data    ""wrf/multi/fog1.wrf""

        public static readonly Parser<Level> LevelSection =
            from level in LevGrammar.Level
            from players in Players
            from type in Type
            from dataset in Dataset
            from game in Game
            from dataArray in Data.AtLeastOnce()
            select new Level
                {
                    Name = level,
                    Players = players,
                    Type = type,
                    Game = game,
                    Data = dataArray.ToArray()
                };


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
            from directive in Parse.String( "campaign" )
            from ignore in Parse.WhiteSpace.Many()
            from name in Parse.AnyChar.Until(Parse.WhiteSpace).Text().Token()
            from data in Data.AtLeastOnce()
            select new Campaign
                       {
                           Name = name,
                           Data = data.ToList()
                       };

        internal static Parser<Identifier> CommentOrIdentifier = 
            from c in Comment.Many ()
            from i in Identifier
                select i;

        internal static Parser<Identifier> Lev = 
            from ident in CommentOrIdentifier
                select ident;

        public static Parser<Lev2> Lev2 =
            from campaingArray in
                (from ignore in Parse.AnyChar.Until(Campaign)
                    from campaign in Campaign
                    select campaign).Optional().AtLeastOnce()
            from levelArray in
                (from ignore in Parse.AnyChar.Until(LevelSection)
                    from level in LevelSection
                    select level).AtLeastOnce()
            select new Lev2
                {
                    Campaigns = campaingArray.Where(option => option.IsDefined).Select(option => option.Get()).ToArray(),
                    Levels = levelArray.ToArray()
                };

        public static Parser<string> CampaignDirective =
            from directive in Parse.String( "campaign" )
            from name in Parse.AnyChar.Many().Except( Parse.WhiteSpace ).Token().Text()
            select name;
    }

    public class Lev2
    {
        public Campaign[] Campaigns { get; set; }
        public Level[] Levels { get; set; }
    }
}