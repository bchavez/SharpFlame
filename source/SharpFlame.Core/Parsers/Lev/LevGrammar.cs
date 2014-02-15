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

        private static readonly Parser<string> newLine =
            Parse.String(Environment.NewLine).Text();

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
            Parse.AnyChar.Except(Parse.WhiteSpace).Except(recordTerminator);

        private static readonly Parser<string> Cell =
            quotedText.XOr(Parse.AnyChar.Except(Parse.WhiteSpace).Except(recordTerminator).XMany().Text());
        
        internal static readonly Parser<string> SingleLineComment =
            from ignore in Parse.String ("//")
                from comment in Parse.AnyChar.Except(Parse.String(Environment.NewLine)).Many().Text()
                from nl in recordTerminator.Text()
                select comment;

        internal static readonly Parser<string> MultilineComment =
            (from open in Parse.String ("/*")
             from comment in Parse.AnyChar.Except (endOfComment (Parse.Char ('/'))).Many ().Text ()
             from close in Parse.String ("*/")
             select comment).Token ();

        /**
         * Parses:
         * text     "Tmp"
         * text     Tmp
         */
        internal static Parser<Token> Token =
            from leading in Parse.Char(' ').Or(Parse.Char('\t')).Many()
            from name in content.Many().Text()
            from trailing in Parse.Char(' ').Or(Parse.Char('\t')).AtLeastOnce()
            from data in Cell
            from nl in recordTerminator.Once()
            select new Token {
                    Name = name,
                    Data = data
            };

        internal static Parser<Campaign> Campaign =
            from directive in Parse.String("campaign")
            from spaces in Parse.WhiteSpace.Many()
            from name in Cell
            from nl in recordTerminator
            from tokens in Token.AtLeastOnce()       
            select new Campaign {
                Name = name,
                Data = tokens.Where(d => d.Name == "data").Select(d => d.Data).ToList<string>()
            };

        internal static Parser<Level> Level = 
            from directive in Parse.String ("level")
            from name in Parse.CharExcept ('\r').Except (Parse.Char ('\n')).AtLeastOnce ().Text ().Token ()
            from tokens in Token.Many ()
            let players = tokens.Where (p => p.Name == "players").FirstOrDefault()
            let type = tokens.Where (p => p.Name == "type").FirstOrDefault()
            let dataset = tokens.Where (p => p.Name == "dataset").FirstOrDefault()
            let game = tokens.Where (p => p.Name == "game").FirstOrDefault()
            let data = tokens.Where (p => p.Name == "data")

            select new Level
                {
                    Name = name,
                    Players = players != null ? int.Parse(players.Data) : 0,
                    Type = type != null ? int.Parse(type.Data) : 0,
                    Dataset = dataset != null ? dataset.Data : "",
                    Game = game != null ? game.Data : "",
                    Data = data.Select(p => p.Data).ToList<string>()
                };

        public static Parser<LevelsFile> Lev =
            from lf in (
                from stripout in
                (from nl in recordTerminator.Many()
                from c1 in MultilineComment.Optional ().Many()
                from c2 in SingleLineComment.Optional ().Many()
                select c1).Many()

                from campaingArray in
                    (from camp in Campaign
                     select camp).Many()

                from levelArray in
                    (from level in Level
                     select level).Many()

                select new LevelsFile
                {
                    Campaigns = campaingArray.Where(option => option != null).ToList<Campaign>(),
                    Levels = levelArray.Where(option => option != null).ToList<Level>(),
                }).Many ()
            from nl in recordTerminator.AtLeastOnce().End()

            select new LevelsFile {
                Campaigns = lf.SelectMany(l => l.Campaigns).ToList<Campaign>(),
                Levels = lf.SelectMany(l => l.Levels).ToList<Level>()
            };      
    }
}