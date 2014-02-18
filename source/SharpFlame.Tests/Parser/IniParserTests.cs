using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using FluentAssertions;
using NUnit.Framework;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Parsers;
using SharpFlame.Core.Parsers.Ini;
using Sprache;

namespace SharpFlame.Tests.Parser
{
    [TestFixture]
    public class IniParserTests
    {
        [Test]
        public void CanReadHealthPercent() {
            var data = @"100%";
            var tmpInt = IniReader.ReadHealthPercent (data);
            tmpInt.Should ().Be (100);
        }

        [Test]
        public void CanParseSettingsIni()
        {
            var path = Path.Combine ("Data", 
                       Path.Combine ("Inis", 
                       Path.Combine ("settings.ini")));

            Console.WriteLine( "Parsing: {0}", path );
            var iniFile = IniReader.ReadFile (path);
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
                        var color = RGBA.FromString (d.Data);
                        color.R.Should ().Be (1D);
                        color.G.Should ().Be (0.25D);
                        color.B.Should ().Be (0.25D);
                        color.A.Should ().Be (0.5D);
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
                    case "PickOrientation":
                        Convert.ToBoolean (d.Data).Should ().Be (true);
                        break;                    
                    }
                }
            }
        }

        [Test]
        public void CanParseDroidIni()
        {
            var path = Path.Combine ("Data", 
                       Path.Combine ("Inis", 
                       Path.Combine ("droid.ini")));

            Console.WriteLine ("Parsing: {0}", path);
            var iniFile = IniReader.ReadFile (path);
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
                    var tmpPosition = XYZInt.FromString (d.Data);
                    tmpPosition.X.Should ().Be (9792);
                    tmpPosition.Y.Should ().Be (26048);
                    tmpPosition.Z.Should ().Be (0);
                    break;
                case "rotation":
                    var tmpRotation = Rotation.FromString (d.Data);
                    tmpRotation.Direction.Should ().Be (0);
                    tmpRotation.Pitch.Should ().Be (0);
                    tmpRotation.Roll.Should ().Be (0);
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
            var path = Path.Combine ("Data", 
                          Path.Combine ("Inis", 
                          Path.Combine ("feature.ini")));

            Console.WriteLine ("Parsing: {0}", path);
            var iniFile = IniReader.ReadFile (path);

            iniFile [3].Name.Should ().Be ("feature_1493");
            foreach (var d in iniFile[3].Data) {
                switch (d.Name) {
                case "id":
                    int.Parse (d.Data).Should ().Be (1493);
                    break;
                case "position":
                    var tmpPosition = XYZInt.FromString (d.Data);
                    tmpPosition.X.Should ().Be (2496);
                    tmpPosition.Y.Should ().Be (26688);
                    tmpPosition.Z.Should ().Be (0);
                    break;
                    case "rotation":
                    var tmpRotation = Rotation.FromString (d.Data);
                    tmpRotation.Direction.Should ().Be (0);
                    tmpRotation.Pitch.Should ().Be (0);
                    tmpRotation.Roll.Should ().Be (0);
                    break;
                    case "name":
                    d.Data.Should ().Be ("AirTrafficControl");
                    break;
                default:
                    throw new Exception (string.Format ("Invalid ID \"{0}\" value \"{1}\"", d.Name, d.Data));
                }
            }
        }

        [Test]
        public void CanParseStructIni()
        {
            var path = Path.Combine ("Data", 
                                     Path.Combine ("Inis", 
                          Path.Combine ("struct.ini")));

            Console.WriteLine ("Parsing: {0}", path);
            var iniFile = IniReader.ReadFile (path);
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
                    var tmpPosition = XYZInt.FromString (d.Data);
                    tmpPosition.X.Should ().Be (6208);
                    tmpPosition.Y.Should ().Be (5440);
                    tmpPosition.Z.Should ().Be (0);
                    break;
                case "rotation":
                    var tmpRotation = Rotation.FromString (d.Data);
                    tmpRotation.Direction.Should ().Be (16384);
                    tmpRotation.Pitch.Should ().Be (0);
                    tmpRotation.Roll.Should ().Be (0);
                    break;
                case "modules":
                    int.Parse (d.Data).Should ().Be (0);
                    break;
                default:
                    throw new Exception (string.Format ("Invalid ID \"{0}\" value \"{1}\"", d.Name, d.Data));
                }
            }
        }
    }
}

