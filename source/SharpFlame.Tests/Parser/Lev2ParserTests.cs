using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Parsers.Lev2;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class TestFixture
    {
        [Test]
        public void can_parse_campaign_directive()
        {
            var data = "campaign	MULTI_CAM_1";

            var result = Lev2Grammar.CampaingDirective.Parse(data);

            result.Should().Be("MULTI_CAM_1");
        }
    }

}