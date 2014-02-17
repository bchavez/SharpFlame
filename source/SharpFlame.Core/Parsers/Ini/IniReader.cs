using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using Sprache;

namespace SharpFlame.Core.Parsers.Ini
{
    public class IniReader
    {
        private static string rootSectionName = "Global";
        public static string RootSectionName { 
            get { return rootSectionName; }
            set { rootSectionName = value; }      
        }

        public static List<Section> ReadString (string txt) {
            var result = new List<Section> ();

            var currentSection = new Section (RootSectionName);            

            foreach (var line in txt.Split(new[]{"\n"}, StringSplitOptions.RemoveEmptyEntries)
                     .Where(t => !string.IsNullOrWhiteSpace(t))
                     .Select(t => t.Trim())) {

                if (line.StartsWith("[") && line.EndsWith("]"))
                {
                    currentSection = new Section (line.Substring(1, line.LastIndexOf("]") - 1).Trim());
                    result.Add(currentSection);
                    continue;
                }

                var idx = line.IndexOf("=");
                if (idx == -1) {
                    currentSection.Data.Add (new Token { Name = line.Substring(0, idx).Trim(), Data = "" });
                } else {
                    currentSection.Data.Add (new Token { Name = line.Substring(0, idx).Trim(), Data = line.Substring(idx + 1).Trim()});
                }
            }

            if (currentSection.Name == RootSectionName) {
                result.Add(currentSection);
            }

            return result;

        }

        public static List<Section> ReadFile (string file) 
        {
            var txt = File.ReadAllText(file);
            return ReadString (txt);
        }

        // Parses: 1, 0.25, 0.25, 0.5
        public static readonly Parser<Double4> Double4 = 
            from p1 in Numerics.Double
            from i1 in Parse.String(", ")
            from p2 in Numerics.Double
            from i2 in Parse.String(", ")
            from p3 in Numerics.Double
            from i3 in Parse.String(", ")
            from p4 in Numerics.Double
            select new Double4 {
                P1 = p1,
                P2 = p2,
                P3 = p3,
                P4 = p4
            };       

        // Parses: 19136, 4288, 0
        public static readonly Parser<Int3> Int3 = 
            from p1 in Numerics.Int
            from i1 in Parse.String(", ")
            from p2 in Numerics.Int
            from i2 in Parse.String(", ")
            from p3 in Numerics.Int
            select new Int3 {
                I1 = p1,
                I2 = p2,
                I3 = p3
            };

        // Parses: %100
        public static readonly Parser<int> Health =
            from result in Numerics.Int
            from sign in Parse.Char ('%')
            select result;
    }
}