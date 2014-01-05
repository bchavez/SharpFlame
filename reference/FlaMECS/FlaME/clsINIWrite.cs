namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.IO;

    public class clsINIWrite
    {
        public char EqualsChar = '=';
        public StreamWriter File;
        public char LineEndChar = '\n';

        public static clsINIWrite CreateFile(Stream Output)
        {
            return new clsINIWrite { File = new StreamWriter(Output, modProgram.UTF8Encoding) };
        }

        public void Gap_Append()
        {
            this.File.Write(this.LineEndChar);
        }

        public void Property_Append(string Name, string Value)
        {
            Name = Name.Replace(Conversions.ToString(this.LineEndChar), "");
            Name = Name.Replace(Conversions.ToString(this.EqualsChar), "");
            Value = Value.Replace(Conversions.ToString(this.LineEndChar), "");
            this.File.Write(Name + " " + Conversions.ToString(this.EqualsChar) + " " + Value + Conversions.ToString(this.LineEndChar));
        }

        public void SectionName_Append(string Name)
        {
            Name = Name.Replace(Conversions.ToString(this.LineEndChar), "");
            this.File.Write("[" + Name + "]" + Conversions.ToString(this.LineEndChar));
        }
    }
}

