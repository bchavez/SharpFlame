using Microsoft.VisualBasic;

namespace SharpFlame
{
    public class clsINIRead
    {
        public class clsSection
        {
            public string Name;
#if !Mono
            public struct sProperty
            {
#else
				public class sProperty
				{
#endif
                public string Name;
                public string Value;
#if !Mono
            }
#else
			}
#endif
            public modLists.SimpleList<sProperty> Properties = new modLists.SimpleList<sProperty>();

            public void CreateProperty(string Name, string Value)
            {
                sProperty NewProperty = new sProperty();

                NewProperty.Name = Name;
                NewProperty.Value = Value;
                Properties.Add(NewProperty);
            }

            public clsResult ReadFile(System.IO.StreamReader File)
            {
                clsResult ReturnResult = new clsResult("");

                int InvalidLineCount = 0;
                int CurrentEntryNum = -1;
                string LineText = null;
                int A = 0;

                do
                {
                    LineText = File.ReadLine();
                    if ( LineText == null )
                    {
                        break;
                    }
                    LineText = LineText.Trim();
                    A = LineText.IndexOf('#');
                    if ( A >= 0 )
                    {
                        LineText = Strings.Left(LineText, A).Trim();
                    }
                    if ( LineText.Length >= 1 )
                    {
                        A = LineText.IndexOf('=');
                        if ( A >= 0 )
                        {
                            CreateProperty(LineText.Substring(0, A).ToLower().Trim(), LineText.Substring(A + 1, LineText.Length - A - 1).Trim());
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                    else if ( LineText.Length > 0 )
                    {
                        InvalidLineCount++;
                    }
                } while ( true );

                Properties.RemoveBuffer();

                if ( InvalidLineCount > 0 )
                {
                    ReturnResult.WarningAdd("There were " + System.Convert.ToString(InvalidLineCount) + " invalid lines that were ignored.");
                }

                return ReturnResult;
            }

            public clsResult Translate(int SectionNum, clsINIRead.clsSectionTranslator Translator, sErrorCount ErrorCount)
            {
                clsResult ReturnResult = new clsResult("Section " + Name);

                int A = 0;
                enumTranslatorResult TranslatorResult = default(enumTranslatorResult);

                for ( A = 0; A <= Properties.Count - 1; A++ )
                {
                    TranslatorResult = Translator.Translate(SectionNum, Properties[A]);
                    switch ( TranslatorResult )
                    {
                        case enumTranslatorResult.NameUnknown:
                            if ( ErrorCount.NameErrorCount < 16 )
                            {
                                ReturnResult.WarningAdd("Property name " + System.Convert.ToString(ControlChars.Quote) + Properties[A].Name +
                                                        System.Convert.ToString(ControlChars.Quote) + " is unknown.");
                            }
                            ErrorCount.NameErrorCount++;
                            break;
                        case enumTranslatorResult.ValueInvalid:
                            if ( ErrorCount.ValueErrorCount < 16 )
                            {
                                ReturnResult.WarningAdd("Value " + System.Convert.ToString(ControlChars.Quote) + Properties[A].Value +
                                                        System.Convert.ToString(ControlChars.Quote) + " for property name " +
                                                        System.Convert.ToString(ControlChars.Quote) + Properties[A].Name +
                                                        System.Convert.ToString(ControlChars.Quote) + " is not valid.");
                            }
                            ErrorCount.ValueErrorCount++;
                            break;
                    }
                }

                return ReturnResult;
            }

            public clsResult Translate(clsINIRead.clsTranslator Translator)
            {
                clsResult ReturnResult = new clsResult("Section " + Name);

                int A = 0;
                enumTranslatorResult TranslatorResult = default(enumTranslatorResult);
                sErrorCount ErrorCount = new sErrorCount();

                ErrorCount.NameWarningCountMax = 16;
                ErrorCount.ValueWarningCountMax = 16;

                for ( A = 0; A <= Properties.Count - 1; A++ )
                {
                    TranslatorResult = Translator.Translate(Properties[A]);
                    switch ( TranslatorResult )
                    {
                        case enumTranslatorResult.NameUnknown:
                            if ( ErrorCount.NameErrorCount < 16 )
                            {
                                ReturnResult.WarningAdd("Property name " + System.Convert.ToString(ControlChars.Quote) + Properties[A].Name +
                                                        System.Convert.ToString(ControlChars.Quote) + " is unknown.");
                            }
                            ErrorCount.NameErrorCount++;
                            break;
                        case enumTranslatorResult.ValueInvalid:
                            if ( ErrorCount.ValueErrorCount < 16 )
                            {
                                ReturnResult.WarningAdd("Value " + System.Convert.ToString(ControlChars.Quote) + Properties[A].Value +
                                                        System.Convert.ToString(ControlChars.Quote) + " for property name " +
                                                        System.Convert.ToString(ControlChars.Quote) + Properties[A].Name +
                                                        System.Convert.ToString(ControlChars.Quote) + " is not valid.");
                            }
                            ErrorCount.ValueErrorCount++;
                            break;
                    }
                }

                if ( ErrorCount.NameErrorCount > ErrorCount.NameWarningCountMax )
                {
                    ReturnResult.WarningAdd("There were " + System.Convert.ToString(ErrorCount.NameErrorCount) + " unknown property names that were ignored.");
                }
                if ( ErrorCount.ValueErrorCount > ErrorCount.ValueWarningCountMax )
                {
                    ReturnResult.WarningAdd("There were " + System.Convert.ToString(ErrorCount.ValueErrorCount) + " invalid values that were ignored.");
                }

                return ReturnResult;
            }

            public string GetLastPropertyValue(string LCasePropertyName)
            {
                int A = 0;

                for ( A = Properties.Count - 1; A >= 0; A-- )
                {
                    if ( Properties[A].Name == LCasePropertyName )
                    {
                        return Properties[A].Value;
                    }
                }
                return null;
            }
        }

        public modLists.SimpleList<clsSection> Sections = new modLists.SimpleList<clsSection>();
        public clsSection RootSection;

        public void CreateSection(string Name)
        {
            clsSection newSection = new clsSection();
            newSection.Name = Name;

            Sections.Add(newSection);
        }

        public clsResult ReadFile(System.IO.StreamReader File)
        {
            clsResult ReturnResult = new clsResult("Reading INI");

            int InvalidLineCount = 0;
            int CurrentEntryNum = -1;
            string LineText = null;
            int A = 0;
            string SectionName = "";

            RootSection = new clsSection();

            do
            {
                LineText = File.ReadLine();
                if ( LineText == null )
                {
                    break;
                }
                LineText = LineText.Trim();
                A = LineText.IndexOf('#');
                if ( A >= 0 )
                {
                    LineText = Strings.Left(LineText, A).Trim();
                }
                if ( LineText.Length >= 2 )
                {
                    if ( LineText[0] == '[' )
                    {
                        if ( LineText[LineText.Length - 1] == ']' )
                        {
                            SectionName = LineText.Substring(1, LineText.Length - 2);
                            for ( A = 0; A <= Sections.Count - 1; A++ )
                            {
                                if ( Sections[A].Name == SectionName )
                                {
                                    break;
                                }
                            }
                            CurrentEntryNum = A;
                            if ( CurrentEntryNum == Sections.Count )
                            {
                                CreateSection(SectionName);
                            }
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                    else if ( CurrentEntryNum >= 0 )
                    {
                        A = LineText.IndexOf('=');
                        if ( A >= 0 )
                        {
                            Sections[CurrentEntryNum].CreateProperty(LineText.Substring(0, A).ToLower().Trim(),
                                LineText.Substring(A + 1, LineText.Length - A - 1).Trim());
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                    else
                    {
                        A = LineText.IndexOf('=');
                        if ( A >= 0 )
                        {
                            RootSection.CreateProperty(LineText.Substring(0, A).ToLower().Trim(), LineText.Substring(A + 1, LineText.Length - A - 1).Trim());
                        }
                        else
                        {
                            InvalidLineCount++;
                        }
                    }
                }
                else if ( LineText.Length > 0 )
                {
                    InvalidLineCount++;
                }
            } while ( true );

            Sections.RemoveBuffer();

            if ( InvalidLineCount > 0 )
            {
                ReturnResult.WarningAdd("There were " + System.Convert.ToString(InvalidLineCount) + " invalid lines that were ignored.");
            }

            return ReturnResult;
        }

        public enum enumTranslatorResult
        {
            NameUnknown,
            ValueInvalid,
            Translated
        }

        public struct sErrorCount
        {
            public int NameErrorCount;
            public int ValueErrorCount;
            public int NameWarningCountMax;
            public int ValueWarningCountMax;
        }

        public clsResult Translate(clsINIRead.clsSectionTranslator Translator)
        {
            clsResult ReturnResult = new clsResult("Translating INI");

            int A = 0;
            sErrorCount ErrorCount = new sErrorCount();

            ErrorCount.NameWarningCountMax = 16;
            ErrorCount.ValueWarningCountMax = 16;

            for ( A = 0; A <= Sections.Count - 1; A++ )
            {
                ReturnResult.Add(Sections[A].Translate(A, Translator, ErrorCount));
            }

            if ( ErrorCount.NameErrorCount > ErrorCount.NameWarningCountMax )
            {
                ReturnResult.WarningAdd("There were " + System.Convert.ToString(ErrorCount.NameErrorCount) + " unknown property names that were ignored.");
            }
            if ( ErrorCount.ValueErrorCount > ErrorCount.ValueWarningCountMax )
            {
                ReturnResult.WarningAdd("There were " + System.Convert.ToString(ErrorCount.ValueErrorCount) + " invalid values that were ignored.");
            }

            return ReturnResult;
        }

        public abstract class clsSectionTranslator
        {
            public abstract enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty);
        }

        public abstract class clsTranslator
        {
            public abstract enumTranslatorResult Translate(clsINIRead.clsSection.sProperty INIProperty);
        }
    }

    public class clsINIWrite
    {
        public System.IO.StreamWriter File;
        public char LineEndChar;
        public char EqualsChar = '=';

        public clsINIWrite()
        {
            LineEndChar = '\n';
        }

        public static clsINIWrite CreateFile(System.IO.Stream Output)
        {
            clsINIWrite NewINI = new clsINIWrite();

            NewINI.File = new System.IO.StreamWriter(Output, modProgram.UTF8Encoding);

            return NewINI;
        }

        public void SectionName_Append(string Name)
        {
            Name = Name.Replace(LineEndChar.ToString(), "");

            File.Write('[' + Name + System.Convert.ToString(']') + System.Convert.ToString(LineEndChar));
        }

        public void Property_Append(string Name, string Value)
        {
            Name = Name.Replace(LineEndChar.ToString(), "");
            Name = Name.Replace(EqualsChar.ToString(), "");
            Value = Value.Replace(LineEndChar.ToString(), "");

            File.Write(Name + " " + System.Convert.ToString(EqualsChar) + " " + Value + System.Convert.ToString(LineEndChar));
        }

        public void Gap_Append()
        {
            File.Write(LineEndChar);
        }
    }
}