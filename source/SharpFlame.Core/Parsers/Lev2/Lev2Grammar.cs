using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using Sprache;

namespace SharpFlame.Core.Parsers.Lev2
{
    public class Lev2Grammar
    {
        public static readonly Parser<string> QuotedText =
            Parse.CharExcept('"').AtLeastOnce().Text()
                .Contained(Parse.Char('"'), Parse.Char('"'));

        //campaign	MULTI_CAM_1
        public static readonly Parser<string> CampaingDirective =
            from directive in Parse.String("campaign").Token()
            from name in Parse.AnyChar.Until(Parse.WhiteSpace).Token().Text()
            select name;

        //data		"wrf/basic.wrf"
        public static readonly Parser<string> DataDirective =
            from directive in Parse.String("data").Token()
            from datapath in QuotedText.Token()
            select datapath;

        //campaign	MULTI_T3_C2
        //data		"wrf/vidmem2.wrf"
        //data		"wrf/basic.wrf"
        //data		"wrf/cam2.wrf"
        //data		"wrf/audio.wrf"
        //data		"wrf/piestats.wrf"
        //data		"wrf/stats.wrf"
        //data		"wrf/multires3.wrf"
        public static readonly Parser<Campaign> Campaign =
            from campaignName in CampaingDirective
            from dataArray in DataDirective.AtLeastOnce()
            select new Campaign
                {
                    Name = campaignName,
                    Data = dataArray.ToArray()
                };

        //level 		Sk-ThePit-T2
        public static readonly Parser<string> LevelDirective =
            from directive in Parse.String("level").Token()
            from name in Parse.AnyChar.Until(Parse.WhiteSpace).Token().Text()
            select name;

        //players		4
        public static readonly Parser<int> PlayersDirective =
            from directive in Parse.String("players").Token()
            from numberStr in Parse.Number
            select int.Parse(numberStr);
        
        //type		18
        public static readonly Parser<int> TypeDirective =
            from directive in Parse.String( "type" ).Token()
            from numberStr in Parse.Number
            select int.Parse( numberStr );

        //dataset		MULTI_T2_C1
        public static readonly Parser<string> DatasetDirective =
            from directive in Parse.String("dataset").Token()
            from name in Parse.AnyChar.Until(Parse.WhiteSpace).Token().Text()
            select name;

        //game		"multiplay/maps/4c-rush.gam"
        public static readonly Parser<string> GameDirective =
            from directive in Parse.String("game").Token()
            from gamepath in QuotedText.Token()
            select gamepath;

        //level 		Sk-Rush2-T2
        //players		4
        //type		18
        //dataset		MULTI_T2_C1
        //game		"multiplay/maps/4c-rush2.gam"
        public static readonly Parser<Level> Level =
            from level in LevelDirective
            from players in PlayersDirective
            from type in TypeDirective
            from dataset in DatasetDirective
            from game in GameDirective
            from data in DataDirective.AtLeastOnce().Optional()
            select new Level
                       {
                           Name = level,
                           Players = players,
                           Type = type,
                           Dataset = dataset,
                           Game = game,
                           Data = data.IsDefined ? data.Get().ToArray() : null
                       };

        //public static readonly Parser<Lev> Lev =
        //    from ignore in Parse.AnyChar.AtLeastOnce().Text().Token()
        //        .Where(x => Campaign.TryParse(x).WasSuccessful)
        //        .Select(y => Campaign.Parse(y)).Optional().AtLeastOnce()
        //    select new Lev
        //        {
        //            Campaigns = ignore.Where(x => x.IsDefined)
        //                .Select(x => x.Get()).ToArray()
        //        };

        //public static readonly Parser<Lev> Lev =
        //    from campaigns in Campaign.Optional().AtLeastOnce()
        //    from levels in Level.Optional().AtLeastOnce()
        //    select new Lev
        //        {
        //            Campaigns = campaigns.Where(c => c.IsDefined)
        //                .Select(o => o.Get()).ToArray()
        //        };

        public static readonly Parser<Lev> Lev =
            from loop in
                (
                    from ignore in Parse.AnyChar.Optional().Except(Campaign)
                    from ignore2 in Parse.AnyChar.Optional().Except(Level)
                    from campaigns in Campaign.Optional().AtLeastOnce()
                    from levels in Level.Optional().AtLeastOnce()
                    select new
                        {
                            Campaigns = campaigns.Where( c => c.IsDefined).Select(c => c.Get()),
                            Levels = levels.Where( l => l.IsDefined).Select(l => l.Get()),

                        }
                    ).AtLeastOnce()
            select new Lev
                {
                    Campaigns = loop
                        .SelectMany(i => i.Campaigns).ToArray(),
                    Levels = loop
                        .SelectMany(i => i.Levels).ToArray()
                };
    }

    public class Lev
    {
        public Campaign[] Campaigns { get; set; }
        public Level[] Levels { get; set; }
    }

    public class Campaign
    {
        public string Name { get; set; }
        public string[] Data { get; set; }
    }

    public class Level
    {
        public string Name { get; set; }
        public int Players { get; set; }
        public int Type { get; set; }
        public string Dataset { get; set; }
        public string Game { get; set; }
        public string[] Data { get; set; }
    }
}