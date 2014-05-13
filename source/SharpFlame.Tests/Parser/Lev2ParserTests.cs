using System;
using System.Diagnostics;
using System.IO;
using System.Linq;
using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Extensions;
using SharpFlame.Core.Parsers.Lev2;
using SharpFlame.Core.Parsers.Lev2;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class Lev2ParserTests
    {
        [Test]
        public void test()
        {
            var lineEnd = Parse.Return( "" ).End()
                .XOr( Parse.String( "\r" ).Text() )
                .Or( Parse.String( "\n" ).Text() )
                .Or( Parse.String( "\r\n" ) );

            var parser = Parse.String("//").Then(
                _ => Parse.AnyChar.Until(lineEnd)).Text();

            

            var result = parser.Parse("// foobar");
            
            result.Should().Be(" foobar");
        }

        [Test]
        public void can_parse_single_line_comment()
        {
            var data = @"// Made with SharpFlame 0.20 Windows";

            var result = Lev2Grammar.SingleLineComment.Parse(data);

            result.Should().Be(" Made with SharpFlame 0.20 Windows");
        }


        [Test]
        public void can_parse_multi_line_comment()
        {
            var data = @"/** .foo. **/";

            var result = Lev2Grammar.MultiLineComment.Parse( data );

            result.Should().Be( "* .foo. *" );
        }


        [Test]
        public void can_parse_campaign_directive()
        {
            var data = @"campaign	MULTI_CAM_1
";

            var result = Lev2Grammar.CampaingDirective.Parse(data);

            result.Should().Be("MULTI_CAM_1");
        }

        [Test]
        public void can_parse_data_driective()
        {
            var data = @"data		""wrf/basic.wrf""
";
            var result = Lev2Grammar.DataDirective.Parse(data);

            result.Should().Be("wrf/basic.wrf");
        }

        [Test]
        public void can_parse_campaign_section()
        {
            var data = @"campaign	MULTI_CAM_3
data		""wrf/vidmem3.wrf""
data		""wrf/basic.wrf""
data		""wrf/cam3.wrf""
data		""wrf/audio.wrf""
data		""wrf/piestats.wrf""
data		""wrf/stats.wrf""
data		""wrf/multires.wrf""
";

            var result = Lev2Grammar.Campaign.Parse(data);
            result.Data.Length.Should().Be(7);
            result.Data[3].Should().Be("wrf/audio.wrf");
            result.Name.Should().Be("MULTI_CAM_3");
        }

        [Test]
        public void can_parse_level_directive()
        {
            var data = @"level 		Sk-Rush-T3
";
            var result = Lev2Grammar.LevelDirective.Parse(data);
            result.Should().Be("Sk-Rush-T3");
        }

        [Test]
        public void can_parse_player_directive()
        {
            var data = @"players		4
";
            var result = Lev2Grammar.PlayersDirective.Parse(data);
            result.Should().Be(4);
        }

        [Test]
        public void can_parse_type_directive()
        {
            var data = @"type		18
";
            var result = Lev2Grammar.TypeDirective.Parse( data );
            result.Should().Be( 18 );
        }


        [Test]
        public void can_parse_dataset_directive()
        {
            var data = @"dataset		MULTI_T2_C1
";

            var result = Lev2Grammar.DatasetDirective.Parse(data);
            result.Should().Be("MULTI_T2_C1");
        }


        [Test]
        public void can_parse_game_directive()
        {
            var data = @"game		""multiplay/maps/4c-rush.gam""
";
            var result = Lev2Grammar.GameDirective.Parse( data );
            result.Should().Be( "multiplay/maps/4c-rush.gam" );
        }

        [Test]
        public void can_parse_level_section()
        {
            var data = @"level 		Sk-Rush2-T2
players		4
type		18
dataset		MULTI_T2_C1
game		""multiplay/maps/4c-rush2.gam""
";
            var result = Lev2Grammar.Level.Parse(data);

            result.Name.Should().Be("Sk-Rush2-T2");
            result.Players.Should().Be(4);
            result.Type.Should().Be(18);
            result.Dataset.Should().Be("MULTI_T2_C1");
            result.Game.Should().Be("multiplay/maps/4c-rush2.gam");
        }

        [Test]
        public void can_parse_level_with_mutiple_game_directives()
        {
            var data = @"level   Tinny-War-T3
players 2
type    19
dataset MULTI_T3_C1
game    ""multiplay/maps/2c-Tinny-War.gam""
data    ""wrf/multi/t3-skirmish2.wrf""
data    ""wrf/multi/fog1.wrf""
";
            var result = Lev2Grammar.Level.Parse( data );
            result.Name.Should().Be( "Tinny-War-T3" );
            result.Players.Should().Be( 2 );
            result.Type.Should().Be( 19 );
            result.Dataset.Should().Be( "MULTI_T3_C1" );
            result.Game.Should().Be( "multiplay/maps/2c-Tinny-War.gam" );
            result.Data[0].Should().Be( "wrf/multi/t3-skirmish2.wrf" );
            result.Data[1].Should().Be( "wrf/multi/fog1.wrf" );

        }

        [Test]
        public void can_parse_tinny_war_lev_file()
        {
            var data = @"// Made with SharpFlame 0.20 Windows
// Date: 2014/02/13 12:23:17
// Author: Unknown
// License: CC0

level   Tinny-War-T1
players 2
type    14
dataset MULTI_CAM_1
game    ""multiplay/maps/2c-Tinny-War.gam""
data    ""wrf/multi/skirmish2.wrf""
data    ""wrf/multi/fog1.wrf""

level   Tinny-War-T2
players 2
type    18
dataset MULTI_T2_C1
game    ""multiplay/maps/2c-Tinny-War.gam""
data    ""wrf/multi/t2-skirmish2.wrf""
data    ""wrf/multi/fog1.wrf""

level   Tinny-War-T3
players 2
type    19
dataset MULTI_T3_C1
game    ""multiplay/maps/2c-Tinny-War.gam""
data    ""wrf/multi/t3-skirmish2.wrf""
data    ""wrf/multi/fog1.wrf""
";
            var result = Lev2Grammar.Lev.Parse(data);

            result.Levels.Length.Should().Be(3);
            result.Levels[1].Players.Should().Be(2);
            result.Levels[0].Type.Should().Be( 14 );
            result.Levels[1].Type.Should().Be( 18 );
            result.Levels[2].Type.Should().Be( 19 );

            result.Levels[0].Dataset.Should().Be( "MULTI_CAM_1" );
            result.Levels[1].Dataset.Should().Be( "MULTI_T2_C1" );
            result.Levels[2].Dataset.Should().Be( "MULTI_T3_C1" );

            result.Levels[0].Name.Should().Be( "Tinny-War-T1" );
            result.Levels[1].Name.Should().Be( "Tinny-War-T2" );
            result.Levels[2].Name.Should().Be( "Tinny-War-T3" );

            result.Levels[1].Data.Count().Should().Be( 2 );
            result.Levels[1].Data[0].Should().Be( "wrf/multi/t2-skirmish2.wrf" );
        }

        [Test]
        public void can_parse_addon_lev()
        {
            var file = "Data"
                .CombinePathWith("Levels")
                .CombinePathWith("addon.lev");

            var txt = File.ReadAllText(file);

            var levfile = Lev2Grammar.Lev.Parse(txt);

            Console.WriteLine( "# of campaigns: {0}", levfile.Campaigns.Length );
            Console.WriteLine( "# of levels: {0}", levfile.Levels.Length );

            levfile.Campaigns.Length.Should().Be(9);
            levfile.Levels.Length.Should().Be(114);
        }

        [Test]
        [Explicit]
        public void benchmark()
        {
            var file = "Data"
             .CombinePathWith( "Levels" )
             .CombinePathWith( "addon.lev" );

            var txt = File.ReadAllText( file );
            Console.WriteLine( "Parsing: {0}", file );
            var s = new Stopwatch();

            s.Start();
            for( int i = 0; i < 1000; i++ )
            {
                Lev2Grammar.Lev.Parse( txt );
            }
            s.Stop();
            Console.WriteLine( "Total Time: " + s.Elapsed );
            //CPU: INTEL 3960X EE, 64GB RAM
            //Total Time: 00:00:13.5444390
        }
    }

}