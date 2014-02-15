using System;
using System.IO;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Parsers.Lev;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class LevParseTests
    {
        [Test]
        public void CanParseIdentifier()
        {
            var data = "level   test_flame-T1";
            var result = LevGrammar.Identifier.Parse(data);
            result.Name.Should ().Be ("level");
            result.Data.Should().Be("test_flame-T1");
        }

        public void CanParseQuottedIdentifier()
        {
            var data = "data    \"wrf/multi/skirmish2.wrf\"";
            var result = LevGrammar.Identifier.Parse(data);
            result.Name.Should ().Be ("data");
            result.Data.Should().Be("wrf/multi/skirmish2.wrf");
        }

        [Test]
        public void can_parse_player()
        {
            var data = "players 2";
            var result = LevGrammar.Players.Parse(data);
            result.Should().Be(2);
        }

        [Test]
        public void can_parse_type()
        {
            var data = "type 14";
            var result = LevGrammar.Type.Parse (data);
            result.Should ().Be (14);
        }

        [Test]
        public void can_parse_dataset() 
        {
            var data = "dataset MULTI_CAM_1";
            var result = LevGrammar.Dataset.Parse (data);
            result.Should ().Be ("MULTI_CAM_1");
        }

        [Test]
        public void can_parse_game()
        {
            var data = "game    \"multiplay/maps/2c-Tinny-War.gam\"";
            var result = LevGrammar.Game.Parse (data);
            result.Should ().Be ("multiplay/maps/2c-Tinny-War.gam");
        }

        [Test]
        public void can_parse_data()
        {
            var data = "data    \"wrf/multi/skirmish2.wrf\"";
            var result = LevGrammar.Data.Parse (data);
            result.Should ().Be ("wrf/multi/skirmish2.wrf");
        }

        [Test]
        public void can_parse_comment()
        {
            var data = "/*** Test123\n ***/";
            var result = LevGrammar.Comment.Parse (data);
            result.Should ().Be ("** Test123\n **");
        }

        [Test]
        public void can_parse_campaign_name()
        {
            var data = "campaign	MULTI_CAM_1";

            var result = LevGrammar.CampaignDirective.Parse( data );

            result.Should().Be( "MULTI_CAM_1" );
        }
        [Test]
        public void CanParseCampaign()
        {
            var data = @"campaign	MULTI_CAM_1
data		""wrf/vidmem.wrf""
data		""wrf/basic.wrf""
data		""wrf/cam1.wrf""
data		""wrf/audio.wrf""
data		""wrf/piestats.wrf""
data		""wrf/stats.wrf""
data		""wrf/multires.wrf""";

            var result = LevGrammar.Campaign.Parse (data);
            result.Data.Count.Should ().Be (7);
            result.Data [4].Should ().Be ("wrf/piestats.wrf");
        }

        [Test]
        public void can_parse_lev_section()
        {
            var data = @"level   Tinny-War-T3
players 2
type    19
dataset MULTI_T3_C1
game    ""multiplay/maps/2c-Tinny-War.gam""
data    ""wrf/multi/t3-skirmish2.wrf""
data    ""wrf/multi/fog1.wrf""
";

            var result = LevGrammar.LevelSection.Parse(data);
            result.Name.Should().Be("Tinny-War-T3");
            result.Players.Should().Be(2);
        }

        [Test]
        public void CanParseAddonLev()
        {
            var file = Path.Combine ("Data", 
                      Path.Combine ("Levels", 
                      Path.Combine ("addon.lev")));

            var txt = File.ReadAllText( file );
            Console.WriteLine( "Parsing: {0}", file );
            var tmp = LevGrammar.Lev.Parse (txt);
            Console.WriteLine (tmp.Name);
        }
    }

}