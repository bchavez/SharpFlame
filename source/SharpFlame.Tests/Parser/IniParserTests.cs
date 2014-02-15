using System;
using System.IO;
using System.Collections.Generic;
using FluentAssertions;
using NUnit.Framework;
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
            d4.P1.Equals (1D);
            d4.P2.Equals (0.25D);
            d4.P3.Equals (0.25D);
            d4.P4.Equals (0.5D);
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
            foreach (var sec in iniFile) {
                sec.Name.Should ().Be ("Global");
                sec.Data.Count.Should ().Be (25);
                Console.WriteLine ("Section {0} Token count is: {1}", sec.Name, sec.Data.Count);
                foreach (var d in sec.Data) {
                    switch (d.Name) {
                    case "AutoSave":
                        Convert.ToBoolean (d.Data).Equals (true);
                        break;
                    case "AutoSaveCompress":
                        Convert.ToBoolean (d.Data).Equals (false);
                        break;
                    case "AutoSaveMinInterval":
                        Convert.ToInt32 (d.Data).Equals (180);
                        break;
                    case "AutoSaveMinChanges":
                        Convert.ToInt32 (d.Data).Equals (20);
                        break;
                    case "MinimapCliffColour":
                        var d4 = IniGrammar.Double4.Parse (d.Data);
                        d4.P1.Equals (1D);
                        d4.P2.Equals (0.25D);
                        d4.P3.Equals (0.25D);
                        d4.P4.Equals (0.5D);
                        break;
                    case "FOVDefault":
                        Convert.ToDecimal (d.Data).Equals (0.000666666666666667D);
                        break;
                    case "FontFamily":
                        d.Data.Equals ("DejaVu Serif");
                        break;
                    case "TilesetsPath":
                        d.Data.Equals ("/home/pcdummy/Projekte/wzlobby/SharpFlame/source/Data/tilesets");
                        break;
                    }
                }
            }

        }
    }
}

