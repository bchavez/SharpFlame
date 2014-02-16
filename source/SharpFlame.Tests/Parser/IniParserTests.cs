using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers.Ini;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class IniParserTests
    {
        [Test]
        public void CanParseToken()
        {
            var data = @"id = 3496
startpos = 8
template = ConstructionDroid
position = 19136, 4288, 0
rotation = 32768, 0, 0
";
            var result = IniGrammar.Token.Parse(data);
            result.Name.Should ().Be ("id");
            result.Data.Should().Be ("3496");
        }

        [Test]
        public void CanParseSection()
        {
            var data = @"[droid_3695]
id = 3695
startpos = 1
template = ConstructionDroid
position = 19264, 26048, 0
rotation = 32768, 0, 0";

            var result = IniGrammar.Section.Parse(data);
            result.Should ().Be ("droid_3695");
        }

        [Test]
        public void CanParseDouble4()
        {
            var data = @"1, 0.25, 0.25, 0.5";
            var d4 = IniGrammar.Double4.Parse (data);
            d4.P1.Should ().Be (1D);
            d4.P2.Should ().Be (0.25D);
            d4.P3.Should ().Be (0.25D);
            d4.P4.Should ().Be (0.5D);
        }       

        [Test]
        public void CanParseInt3()
        {
            var data = @"19136, 4288, 0";
            var int3 = IniGrammar.Int3.Parse (data);
            int3.I1.Should ().Be (19136);
            int3.I2.Should ().Be (4288);
            int3.I3.Should ().Be (0);
        }

        [Test]
        public void CanParseSettingsIni()
        {
            var file = Path.Combine ("Data", 
                       Path.Combine ("Inis", 
                       Path.Combine ("settings.ini")));

            var txt = File.ReadAllText( file );
            Console.WriteLine( "Parsing: {0}", file );
            var iniFile = IniGrammar.Ini.Parse (txt);
            iniFile.Count.Should ().Be (1);
            foreach (var sec in iniFile) {
                sec.Name.Should ().Be ("Global");
                sec.Data.Count.Should ().Be (25);
                Console.WriteLine ("Section {0} Token count is: {1}", sec.Name, sec.Data.Count);
                foreach (var d in sec.Data) {
                    switch (d.Name) {
                    case "AutoSave":
                        Convert.ToBoolean (d.Data).Should ().Be (true);
                        break;
                    case "AutoSaveCompress":
                        Convert.ToBoolean (d.Data).Should ().Be (false);
                        break;
                    case "AutoSaveMinInterval":
                        Convert.ToInt32 (d.Data).Should ().Be (180);
                        break;
                    case "AutoSaveMinChanges":
                        Convert.ToInt32 (d.Data).Should ().Be (20);
                        break;
                    case "MinimapCliffColour":
                        var d4 = IniGrammar.Double4.Parse (d.Data);
                        d4.P1.Should ().Be (1D);
                        d4.P2.Should ().Be (0.25D);
                        d4.P3.Should ().Be (0.25D);
                        d4.P4.Should ().Be (0.5D);
                        break;
                    case "FOVDefault":
                        Convert.ToDouble (d.Data, CultureInfo.InvariantCulture).Should ().Be (0.000666666666666667D);
                        break;
                    case "FontFamily":
                        d.Data.Should ().Be ("DejaVu Serif");
                        break;
                    case "TilesetsPath":
                        d.Data.Should ().Be ("/home/pcdummy/Projekte/wzlobby/SharpFlame/source/Data/tilesets");
                        break;
                    }
                }
            }
        }

        [Test]
        public void CanParseDroidIni()
        {
            var file = Path.Combine ("Data", 
                       Path.Combine ("Inis", 
                       Path.Combine ("droid.ini")));

            var txt = File.ReadAllText (file);
            Console.WriteLine ("Parsing: {0}", file);
            var iniFile = IniGrammar.Ini.Parse (txt);
            iniFile.Count.Should().Be (40);
 
            // Parse:
            // id = 3693
            // startpos = 3
            // template = ConstructionDroid
            // position = 9792, 26048, 0
            // rotation = 0, 0, 0
            foreach (var d in iniFile[4].Data) {
                switch (d.Name) {
                case "id":
                    int.Parse (d.Data).Should ().Be (3693);
                    break;
                case "startpos":
                    int.Parse (d.Data).Should ().Be (3);
                    break;
                case "template":
                    d.Data.Should ().Be ("ConstructionDroid");
                    break;
                case "position":
                    var tmpPosition = IniGrammar.Int3.Parse (d.Data);
                    tmpPosition.I1.Should ().Be (9792);
                    tmpPosition.I2.Should ().Be (26048);
                    tmpPosition.I3.Should ().Be (0);
                    break;
                case "rotation":
                    var tmpRotation = IniGrammar.Int3.Parse (d.Data);
                    tmpRotation.I1.Should ().Be (0);
                    tmpRotation.I2.Should ().Be (0);
                    tmpRotation.I3.Should ().Be (0);
                    break;
                case "player":
                    break;
                case "name":
                    break;
                case "health":
                    break;
                case "droidtype":
                    break;
                case "weapons":
                    break;
                case "parts\\body":
                    break;
                case "parts\\propulsion":
                    break;
                case "parts\\brain":
                    break;
                case "parts\\repair":
                    break;
                case "parts\\ecm":
                    break;
                case "parts\\sensor":
                    break;
                case "parts\\construct":
                    break;
                case "parts\\weapon\\1":
                    break;
                case "parts\\weapon\\2":
                    break;
                case "parts\\weapon\\3":
                    break;
                }
            }
        }

        [Test]
        public void CanParseFeatureIni()
        {
            var file = Path.Combine ("Data", 
                                     Path.Combine ("Inis", 
                                     Path.Combine ("feature.ini")));

            var txt = File.ReadAllText (file);
            Console.WriteLine ("Parsing: {0}", file);
            var iniFile = IniGrammar.Ini.Parse (txt);
            iniFile.Count.Should().Be (730);

            iniFile [3].Name.Should ().Be ("feature_1493");
            foreach (var d in iniFile[3].Data) {
                switch (d.Name) {
                case "id":
                    int.Parse (d.Data).Should ().Be (1493);
                    break;
                case "position":
                    var tmpPosition = IniGrammar.Int3.Parse (d.Data);
                    tmpPosition.I1.Should ().Be (2496);
                    tmpPosition.I2.Should ().Be (26688);
                    tmpPosition.I3.Should ().Be (0);
                    break;
                case "rotation":
                    var tmpRotation = IniGrammar.Int3.Parse (d.Data);
                    tmpRotation.I1.Should ().Be (0);
                    tmpRotation.I2.Should ().Be (0);
                    tmpRotation.I3.Should ().Be (0);
                    break;
                case "name":
                    d.Data.Should ().Be ("AirTrafficControl");
                    break;
                }
            }
        }

        [Test]
        public void CanParseStructIni()
        {
            var file = Path.Combine ("Data", 
                                     Path.Combine ("Inis", 
                                     Path.Combine ("struct.ini")));

            var txt = File.ReadAllText (file);
            Console.WriteLine ("Parsing: {0}", file);
            var iniFile = IniGrammar.Ini.Parse (txt);
            iniFile.Count.Should ().Be (2006);          

            iniFile [2].Name.Should ().Be ("structure_1248");
            foreach (var d in iniFile[2].Data) {
                switch (d.Name) {
                case "id":
                    int.Parse (d.Data).Should ().Be (1248);
                    break;
                case "startpos":
                    int.Parse (d.Data).Should ().Be (5);
                    break;
                case "name":
                    break;
                case "wall/type":
                    int.Parse (d.Data).Should ().Be (0);
                    break;
                case "position":
                    var tmpPosition = IniGrammar.Int3.Parse (d.Data);
                    tmpPosition.I1.Should ().Be (6208);
                    tmpPosition.I2.Should ().Be (5440);
                    tmpPosition.I3.Should ().Be (0);
                    break;
                case "rotation":
                    var tmpRotation = IniGrammar.Int3.Parse (d.Data);
                    tmpRotation.I1.Should ().Be (16384);
                    tmpRotation.I2.Should ().Be (0);
                    tmpRotation.I3.Should ().Be (0);
                    break;
                case "modules":
                    int.Parse (d.Data).Should ().Be (0);
                    break;
                }
            }
        }
    }
}

