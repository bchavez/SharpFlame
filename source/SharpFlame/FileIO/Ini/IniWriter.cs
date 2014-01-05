using System;
using System.IO;

namespace SharpFlame.FileIO.Ini
{
    public class IniWriter
    {
        public StreamWriter File;
        public char LineEndChar;
        public char EqualsChar = '=';

        public IniWriter()
        {
            LineEndChar = '\n';
        }

        public static IniWriter CreateFile(Stream Output)
        {
            IniWriter NewINI = new IniWriter();

            NewINI.File = new StreamWriter(Output, App.UTF8Encoding);

            return NewINI;
        }

        public void AppendSectionName(string Name)
        {
            Name = Name.Replace(LineEndChar.ToString(), "");

            File.Write('[' + Name + Convert.ToString(']') + Convert.ToString(LineEndChar));
        }

        public void AppendProperty(string Name, string Value)
        {
            Name = Name.Replace(LineEndChar.ToString(), "");
            Name = Name.Replace(EqualsChar.ToString(), "");
            Value = Value.Replace(LineEndChar.ToString(), "");

            File.Write(Name + " " + Convert.ToString(EqualsChar) + " " + Value + Convert.ToString(LineEndChar));
        }

        public void Gap_Append()
        {
            File.Write(LineEndChar);
        }
    }
}