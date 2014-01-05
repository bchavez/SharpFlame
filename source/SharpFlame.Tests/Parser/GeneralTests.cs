﻿using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class Pie2Tests
    {
        [TestFixtureSetUp]
        public void BeforeRunningTestSession()
        {

        }

        [TestFixtureTearDown]
        public void AfterRunningTestSession()
        {

        }


        [SetUp]
        public void BeforeEachTest()
        {

        }

        [TearDown]
        public void AfterEachTest()
        {

        }


        [Test]
        public void can_parse_version_number()
        {
            var data = @"PIE 2";

            var result = Pie.Version.Parse(data);

            result.Should().Be(2);
        }

        [Test]
        public void can_parse_pie_header()
        {
            var data = @"TYPE 10200";

            var result = Pie.Type.Parse(data);
            result.Should().Be(10200);
        }

        [Test]
        public void can_parse_texture_directive()
        {
            var data = @"TEXTURE 0 page-17-droid-weapons.png 256 256";

            var result = Pie.Texture.Parse(data);
            result.ShouldBeEquivalentTo(new TextureDirective
                {
                    Height = 256,
                    Width = 256,
                    Path = "page-17-droid-weapons.png"
                });
        }

        [Test]
        public void can_parse_levels_directive()
        {
            var data = @"LEVELS 3";

            var result = Pie.Levels.Parse(data);

            result.Should().Be(3);
        }


        [Test]
        public void can_parse_xyz_coord()
        {
            var data = @"	-52 7 22";

            var result = Pie.XyzParser.Parse(data);

            result.ShouldBeEquivalentTo(new Xyz 
                {
                    X = -52,
                    Y = 7,
                    Z = 22
                });
        }


        [Test]
        public void can_parse_points_directive()
        {
            var data = @"POINTS 56";

            var result = Pie.Points.Parse(data);

            result.Should().Be(56);
        }

        [Test]
        public void can_parse_level_directive()
        {
            var data = @"LEVEL 1";

            var result = Pie.Level.Parse( data );

            result.Should().Be( 1 );
        }

        [Test]
        public void can_read_points_data()
        {

            var data = @"	-45 8 -18
	-42 5 -18
	17 77 -7";

            var result = Pie.PointsData.Parse(data);
            
            result.Length.Should().Be(3);
            var truth = new[]
                {
                    new Xyz {X = -45, Y = 8, Z = -18},
                    new Xyz {X = -45, Y = 5, Z = -18},
                    new Xyz {X = 17, Y = 77, Z = -7},
                };
            truth.ShouldAllBeEquivalentTo(result);
        }
    }
}