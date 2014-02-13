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
    }

}