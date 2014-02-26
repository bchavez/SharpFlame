using System;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class GeneralTests
    {
        [Test]
        public void CanParseMultilineComment()
        {
            var data = @"/*** Test123
Test123
***/

level    Sk-Rush-T1
";
            var result = General.MultilineComment.Parse (data);
            result.Should ().Be ("** Test123\r\nTest123\r\n**");
        }

        [Test]
        public void CanParseSingleLineComment()
        {
            var data = @"// Test123

                data        ""wrf/vidmem.wrf""";
            var result = General.SingleLineComment.Parse (data);
            result.Should ().Be (" Test123");
        }
    }
}

