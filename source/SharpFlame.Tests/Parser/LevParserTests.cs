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
        public void can_parse_level()
        {
            var data = "level   test_flame-T1";
            var result = LevGrammar.Level.Parse(data);
            result.Should().Be("test_flame-T1");
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
    }

}