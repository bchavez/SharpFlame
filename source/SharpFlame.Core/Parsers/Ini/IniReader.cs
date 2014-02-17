using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

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
    }
}