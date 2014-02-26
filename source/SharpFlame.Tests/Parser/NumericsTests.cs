using System;
using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class NumericsTests
    {
        [Test]
        public void CanParseDouble()
        {
            var data = @"123.123";
            var myDouble = Numerics.Double.Parse (data);
            myDouble.Should ().Be (123.123D);
        }

        [Test]
        public void CanParseInt()
        {
            var data = @"123";
            var myInt = Numerics.Int.Parse(data);
            myInt.Should ().Be (123);
        }

        [Test]
        public void CanParseFloat()
        {
            var data = @"17.123";
            var myFloat = Numerics.Float.Parse (data);
            myFloat.Should ().Be (17.123F);
        }

        [Test]
        public void CanParseSignedFloat()
        {
            var data = @"-17.123";
            var myFloat = Numerics.Float.Parse (data);
            myFloat.Should ().Be (-17.123F);
        }     
    }
}

