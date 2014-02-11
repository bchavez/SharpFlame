using System;
using System.IO;
using System.Linq;
using FluentAssertions;
using FluentValidation.Attributes;
using NUnit.Framework;
using SharpFlame.Core.Parsers.Pie;
using SharpFlame.Core.Parsers.Validators;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class PieParseTests
    {
        [Test]
        public void can_parse_version_number()
        {
            var data = @"PIE 2";

            var result = PieGrammar.Version.Parse(data);

            result.Should().Be(2);
        }

        [Test]
        public void can_parse_pie_header()
        {
            var data = @"TYPE 10200";

            var result = PieGrammar.Type.Parse(data);
            result.Should().Be(10200);
        }

        [Test]
        public void can_parse_texture_directive()
        {
            var data = @"TEXTURE 0 page-17-droid-weapons.png 256 256";

            var result = PieGrammar.Texture.Parse(data);
            result.ShouldBeEquivalentTo(new Texture
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

            var result = PieGrammar.Levels.Parse(data);

            result.Should().Be(3);
        }


        [Test]
        public void can_parse_xyz_coord()
        {
            var data = @"	-52 7 22";

            var result = PieGrammar.PointLine.Parse(data);

            result.ShouldBeEquivalentTo(new Point 
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

            var result = PieGrammar.Points.Parse(data);

            result.Should().Be(56);
        }

        [Test]
        public void can_parse_level_directive()
        {
            var data = @"LEVEL 1";

            var result = PieGrammar.Level.Parse( data );

            result.Should().Be( 1 );
        }


        [Test]
        public void can_parse_polygon_line()
        {
            var data = @"	200 3 3 2 1 237 220 239 220 239 222";
 
            //          200                 3     3    2    1    237  220    239  220    239  222
            //[PolygonFlags] [Number of Points] [P1] [P2] [P3]  [VU] [VV]   [VU] [VV]   [VU] [VV]


            var result = PieGrammar.PolygonLine.Parse(data);

            var truth = new Polygon
                {
                    Flags = PolygonFlags.Texture,
                    PointCount = 3,
                    P1 = 3,
                    P2 = 2,
                    P3 = 1,
                    Frames = 1,
                    PlaybackRate = 1,
                    TexCoords = new[]
                        {
                            new TexCoord {U = 237f, V = 220f},
                            new TexCoord {U = 239f, V = 220f},
                            new TexCoord {U = 239f, V = 222f},
                        }
                };

            result.ShouldBeEquivalentTo(truth);
        }

        [Test]
        public void can_parse_texcoords()
        {
            var data = @" 237 220 239 220 239 222";

            var result = PieGrammar.TexCoord.Repeat(3)
                .Parse(data).ToArray();

            var truth = new[]
                {
                    new TexCoord {U = 237f, V = 220f},
                    new TexCoord {U = 239f, V = 220f},
                    new TexCoord {U = 239f, V = 222f},
                };

            result.ShouldAllBeEquivalentTo(truth);
        }

        [Test]
        public void can_parse_polygon_line_texture_animation()
        {

            var data = @"	4200 3 7 6 5 2 1 11 14 45 128 54 128 54 140";
            //	        4200                  3    7    6    5        2               1       11        14      45  128     54  128     54  140
            //[PolygonFlags] [Number of Points] [P1] [P2] [P3] [frames]  [playbackRate]  [width]  [height]    [VU] [VV]   [VU] [VV]   [VU] [VV]

            var result = PieGrammar.PolygonLine.Parse( data );

            var truth = new Polygon
                {
                    Flags = PolygonFlags.Animation | PolygonFlags.Texture,
                    PointCount = 3,
                    P1 = 7,
                    P2 = 6,
                    P3 = 5,
                    Frames = 2,
                    PlaybackRate = 1,
                    Width = 11,
                    Height = 14,
                    TexCoords = new[]
                        {
                            new TexCoord {U = 45, V = 128},
                            new TexCoord {U = 54, V = 128},
                            new TexCoord {U = 54, V = 140},
                        }
                };

            result.ShouldBeEquivalentTo(truth);
        }

        [Test]
        public void can_read_simple_piev2()
        {
            var data = @"PIE 2
TYPE 10200
TEXTURE 0 page-17-droid-weapons.png 256 255
LEVELS 1
LEVEL 1
POINTS 13 
	14 3 0 
	7 3 12 
	7 37 12 
	14 37 0 
	-6 3 12 
	-6 37 12 
	-13 3 0 
	-13 37 0 
	-6 3 -12 
	-6 37 -12 
	7 3 -12 
	7 37 -12 
	0 37 0
POLYGONS 36
	200 3 0 1 2 10 192 12 192 12 177
	200 3 0 2 3 10 192 12 177 10 177
	200 3 3 2 1 10 177 12 177 12 192
	200 3 3 1 0 10 177 12 192 10 192
	200 3 1 4 5 0 192 1 192 1 177
	200 3 1 5 2 0 192 1 177 0 177
	200 3 2 5 4 0 177 1 177 1 192
	200 3 2 4 1 0 177 1 192 0 192
	200 3 4 6 7 1 192 3 192 3 177
	200 3 4 7 5 1 192 3 177 1 177
	200 3 5 7 6 1 177 3 177 3 192
	200 3 5 6 4 1 177 3 192 1 192
	200 3 6 8 9 3 192 5 192 5 177
	200 3 6 9 7 3 192 5 177 3 177
	200 3 7 9 8 3 177 5 177 5 192
	200 3 7 8 6 3 177 5 192 3 192
	200 3 8 10 11 5 192 8 192 8 177
	200 3 8 11 9 5 192 8 177 5 177
	200 3 9 11 10 5 177 8 177 8 192
	200 3 9 10 8 5 177 8 192 5 192
	200 3 10 0 3 8 192 10 192 10 177
	200 3 10 3 11 8 192 10 177 8 177
	200 3 11 3 0 8 177 10 177 10 192
	200 3 11 0 10 8 177 10 192 8 192
	200 3 12 11 3 23 185 26 192 29 185
	200 3 12 3 2 23 185 29 185 26 178
	200 3 2 3 11 26 178 29 185 26 192
	200 3 2 11 12 26 178 26 192 23 185
	200 3 12 2 5 23 185 26 178 19 178
	200 3 12 5 7 23 185 19 178 16 185
	200 3 7 5 2 16 185 19 178 26 178
	200 3 7 2 12 16 185 26 178 23 185
	200 3 12 7 9 23 185 16 185 19 192
	200 3 12 9 11 23 185 19 192 26 192
	200 3 11 9 7 26 192 19 192 16 185
	200 3 11 7 12 26 192 16 185 23 185";

            var pie = PieGrammar.Pie.Parse(data);

            var validator = new PieValidator();
            validator.Validate(pie).IsValid.Should().BeTrue();

            pie.Version.Should().Be( 2 );
            pie.Type.Should().Be(10200);
            pie.Texture.Path.Should().Be("page-17-droid-weapons.png");
            pie.Texture.Height.Should().Be( 255 );
            pie.Texture.Width.Should().Be( 256 );
            pie.Levels.Length.Should().Be(1);
            pie.Levels[0].Points.Length.Should().Be( 13 );
            pie.Levels[0].Polygons.Length.Should().Be( 36 );

            pie.Levels[0].Points[12].X.Should().Be( 0 );
            pie.Levels[0].Points[12].Y.Should().Be( 37 );
            pie.Levels[0].Points[12].Z.Should().Be( 0 );

            pie.Levels[0].Polygons[35].Flags.Should().Be( PolygonFlags.Texture );
            pie.Levels[0].Polygons[35].PointCount.Should().Be(3 );
            pie.Levels[0].Polygons[35].P1.Should().Be( 11 );
            pie.Levels[0].Polygons[35].P2.Should().Be( 7 );
            pie.Levels[0].Polygons[35].P3.Should().Be( 12 );
            
            //TODO more thorough testing here
            pie.Levels[0].Polygons[35].TexCoords[2].U.Should().Be( 23 );
            pie.Levels[0].Polygons[35].TexCoords[2].V.Should().Be( 185 );
        }

        [Test]
        public void can_read_piev2_with_multi_level()
        {
            var data = @"PIE 2
TYPE 10200
TEXTURE 0 page-11-player-buildings.png 254 256
LEVELS 3
LEVEL 1
POINTS 56
	-52 7 22
	51 7 22
	51 7 -21
	-52 7 -21
	51 0 -21
	-52 0 -21
	51 0 22
	-52 0 22
	-15 30 7
	43 30 7
	43 30 -6
	-15 30 -6
	48 7 -8
	-20 7 -8
	48 7 9
	-20 7 9
	-57 0 -39
	57 0 -39
	57 26 -39
	-57 26 -39
	57 0 39
	57 26 39
	-57 0 39
	-57 26 39
	17 77 12
	17 82 12
	-45 8 20
	-42 7 20
	-45 8 18
	17 82 7
	17 77 7
	-42 7 18
	-24 5 -1
	-27 5 -1
	-27 71 -1
	-24 71 -1
	-24 5 1
	-24 71 1
	-27 5 1
	-27 71 1
	17 7 -20
	17 80 -8
	22 80 -8
	22 8 -20
	17 80 9
	22 80 9
	17 8 21
	22 8 21
	17 82 -7
	17 82 -12
	-45 8 -20
	-45 8 -18
	-42 5 -18
	17 77 -7
	17 77 -12
	-42 7 -20
POLYGONS 66
	200 3 0 1 2 253 64 253 120 218 120
	200 3 0 2 3 253 64 218 120 218 64
	200 3 3 2 4 223 64 223 120 218 120
	200 3 3 4 5 223 64 218 120 218 64
	200 3 2 1 6 218 116 253 116 253 120
	200 3 2 6 4 218 116 253 120 218 120
	200 3 1 0 7 253 64 253 120 249 120
	200 3 1 7 6 253 64 249 120 249 64
	200 3 0 3 5 218 64 253 64 253 69
	200 3 0 5 7 218 64 253 69 218 69
	200 3 8 9 10 252 35 252 1 241 1
	200 3 8 10 11 252 35 241 1 241 35
	200 3 11 10 12 247 60 221 60 221 50
	200 3 11 12 13 247 60 221 50 247 50
	200 3 10 9 14 255 49 247 49 247 62
	200 3 10 14 12 255 49 247 62 255 62
	200 3 9 8 15 247 60 221 60 221 50
	200 3 9 15 14 247 60 221 50 247 50
	200 3 8 11 13 252 35 241 35 241 49
	200 3 8 13 15 252 35 241 49 252 49
	200 3 16 17 18 64 47 116 47 116 36
	200 3 16 18 19 64 47 116 36 64 36
	200 3 19 18 17 64 36 116 36 116 47
	200 3 19 17 16 64 36 116 47 64 47
	200 3 17 20 21 89 47 116 47 116 36
	200 3 17 21 18 89 47 116 36 89 36
	200 3 18 21 20 89 36 116 36 116 47
	200 3 18 20 17 89 36 116 47 89 47
	200 3 20 22 23 64 47 116 47 116 36
	200 3 20 23 21 64 47 116 36 64 36
	200 3 21 23 22 64 36 116 36 116 47
	200 3 21 22 20 64 36 116 47 64 47
	200 3 22 16 19 89 47 116 47 116 36
	200 3 22 19 23 89 47 116 36 89 36
	200 3 23 19 16 89 36 116 36 116 47
	200 3 23 16 22 89 36 116 47 89 47
	200 3 24 25 26 255 49 253 49 253 0
	200 3 24 26 27 255 49 253 0 255 0
	200 3 28 29 30 253 49 253 0 255 0
	200 3 28 30 31 253 49 255 0 255 49
	200 3 29 28 26 253 0 253 49 255 49
	200 3 29 26 25 253 0 255 49 255 0
	200 3 32 33 34 96 55 98 55 99 121
	200 3 32 34 35 96 55 99 121 97 121
	200 3 36 32 35 94 54 98 54 97 121
	200 3 36 35 37 94 54 97 121 93 121
	200 3 38 36 37 92 55 96 55 96 118
	200 3 38 37 39 92 55 96 118 92 118
	200 3 33 38 39 94 55 98 55 98 121
	200 3 33 39 34 94 55 98 121 94 121
	200 3 40 41 42 240 1 240 48 235 48
	200 3 40 42 43 240 1 235 48 235 1
	200 3 41 44 45 226 2 226 4 217 4
	200 3 41 45 42 226 2 217 4 217 2
	200 3 44 46 47 240 1 240 48 235 48
	200 3 44 47 45 240 1 235 48 235 1
	200 3 43 42 45 210 48 217 1 227 1
	200 3 43 45 47 210 48 227 1 234 49
	200 3 44 41 40 216 1 227 1 234 48
	200 3 44 40 46 216 1 234 48 210 48
	200 3 48 49 50 253 0 255 0 255 49
	200 3 48 50 51 253 0 255 49 253 49
	200 3 51 52 53 255 49 253 49 253 0
	200 3 51 53 48 255 49 253 0 255 0
	200 3 54 55 50 255 49 255 0 253 0
	200 3 54 50 49 255 49 253 0 253 49
LEVEL 2
POINTS 8
	-32 73 0
	-26 73 5
	-20 73 0
	-26 73 -5
	-20 22 0
	-26 22 -5
	-26 22 5
	-32 22 0
POLYGONS 10
	200 3 0 1 2 46 200 34 211 22 200
	200 3 0 2 3 46 200 22 200 34 189
	200 3 3 2 4 240 49 234 49 234 1
	200 3 3 4 5 240 49 234 1 240 1
	200 3 2 1 6 240 49 234 49 234 1
	200 3 2 6 4 240 49 234 1 240 1
	200 3 1 0 7 240 48 234 48 234 1
	200 3 1 7 6 240 48 234 1 240 1
	200 3 0 3 5 240 49 234 49 234 1
	200 3 0 5 7 240 49 234 1 240 1
LEVEL 3
POINTS 26
	31 85 7
	26 103 7
	8 98 7
	12 80 7
	8 98 -6
	26 103 -6
	31 85 -6
	12 80 -6
	19 88 2
	17 96 2
	-61 75 2
	-59 66 2
	-61 75 -1
	17 96 -1
	19 88 -1
	-59 66 -1
	-60 95 -2
	-67 90 -2
	-67 90 3
	-60 95 3
	-68 64 -2
	-68 64 3
	-50 35 -2
	-50 35 3
	-44 39 -2
	-44 39 3
POLYGONS 30
	200 3 0 1 2 28 244 27 252 2 251
	200 3 0 2 3 28 244 2 251 3 243
	200 3 4 5 6 27 251 3 251 3 243
	200 3 4 6 7 27 251 3 243 27 243
	200 3 6 5 1 6 251 6 245 24 245
	200 3 1 0 6 24 245 24 251 6 251
	200 3 5 4 2 28 252 1 252 1 243
	200 3 5 2 1 28 252 1 243 28 243
	200 3 4 7 3 25 244 25 250 7 250
	200 3 4 3 2 25 244 7 250 7 244
	200 3 8 9 10 240 48 235 48 235 1
	200 3 8 10 11 240 48 235 1 240 1
	200 3 12 13 14 240 1 240 48 235 48
	200 3 12 14 15 240 1 235 48 235 1
	200 3 13 12 10 239 2 239 48 236 48
	200 3 13 10 9 239 2 236 48 236 2
	200 3 16 17 18 31 252 30 247 31 247
	200 3 16 18 19 31 252 31 247 31 252
	200 3 17 20 21 31 244 17 244 17 242
	200 3 17 21 18 31 244 17 242 31 242
	200 3 20 22 23 17 244 0 244 0 242
	200 3 20 23 21 17 244 0 242 17 242
	200 3 24 16 19 0 252 31 252 31 250
	200 3 24 19 25 0 252 31 250 0 250
	200 3 23 25 19 0 244 0 252 31 252
	200 3 23 19 21 0 244 31 252 17 242
	200 3 21 19 18 17 242 31 252 31 244
	200 3 16 24 22 31 252 0 252 0 244
	200 3 16 22 20 31 252 0 244 17 242
	200 3 16 20 17 31 252 17 242 31 244
";
            var pie = PieGrammar.Pie.Parse( data );

            var validator = new PieValidator();
            validator.Validate( pie ).IsValid.Should().BeTrue();

            pie.Version.Should().Be( 2 );
            pie.Type.Should().Be( 10200 );
            pie.Texture.Path.Should().Be( "page-11-player-buildings.png" );
            pie.Texture.Width.Should().Be( 254 );
            pie.Texture.Height.Should().Be( 256 );
            pie.Levels.Length.Should().Be( 3 );
            pie.Levels[0].Points.Length.Should().Be( 56 );
            pie.Levels[1].Points.Length.Should().Be( 8 );
            pie.Levels[2].Points.Length.Should().Be( 26 );

            pie.Levels[0].Points[0].X.Should().Be( -52 );
            pie.Levels[1].Points[1].Y.Should().Be( 73 );
            pie.Levels[2].Points[2].Z.Should().Be( 7 );

            pie.Levels[2].Polygons[1].Flags.Should().Be( PolygonFlags.Texture );
            pie.Levels[2].Polygons[1].PointCount.Should().Be( 3 );
            pie.Levels[2].Polygons[1].P1.Should().Be( 0 );
            pie.Levels[2].Polygons[1].P2.Should().Be( 2 );
            pie.Levels[2].Polygons[1].P3.Should().Be( 3);

            //TODO more thorough testing here
            pie.Levels[2].Polygons[1].TexCoords[2].U.Should().Be( 3 );
            pie.Levels[2].Polygons[1].TexCoords[2].V.Should().Be( 243 );
        }

        [Test]
        public void can_parse_all_pie_fiels_without_error()
        {

            var files = Directory.GetFiles( @"..\..\..\Data\3.1_b4-objects\pies", "*.pie" );
            var f = new AttributedValidatorFactory();

            foreach( var file in files )
            {
                var txt = File.ReadAllText( file );
                Console.WriteLine("Parsing: {0}", file);
                var pie = PieGrammar.Pie.Parse( txt );


                var v = f.GetValidator<Pie>();
                v.Validate( pie ).IsValid.Should().BeTrue();
            }

        }

        [Test]
        [Explicit]
        public void test()
        {

        }

        [Test]
        [Explicit]
        public void test2()
        {

        }

        [Test]
        [Explicit]
        public void test3()
        {
            var file = @"..\..\..\Data\3.1_b4-objects\pies\radarsensor.pie";
            var f = new AttributedValidatorFactory();
            var txt = File.ReadAllText( file );
            Console.WriteLine( "Parsing: {0}", file );
            var pie = PieGrammar.Pie.Parse( txt );
            //            PieGrammar.Pie.parse

            var v = f.GetValidator<Pie>();
            v.Validate( pie ).IsValid.Should().BeTrue();
        }
    }
}