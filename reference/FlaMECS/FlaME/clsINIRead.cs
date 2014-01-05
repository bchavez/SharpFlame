namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.IO;
    using System.Runtime.InteropServices;

    public class clsINIRead
    {
        public clsSection RootSection;
        public modLists.SimpleList<clsSection> Sections = new modLists.SimpleList<clsSection>();

        public void CreateSection(string Name)
        {
            clsSection newItem = new clsSection {
                Name = Name
            };
            this.Sections.Add(newItem);
        }

        public clsResult ReadFile(StreamReader File)
        {
            clsResult result2 = new clsResult("Reading INI");
            int num3 = 0;
            int num2 = -1;
            string str = null;
            this.RootSection = new clsSection();
            while (true)
            {
                str = File.ReadLine();
                if (str == null)
                {
                    this.Sections.RemoveBuffer();
                    if (num3 > 0)
                    {
                        result2.WarningAdd("There were " + Conversions.ToString(num3) + " invalid lines that were ignored.");
                    }
                    return result2;
                }
                str = str.Trim();
                int index = str.IndexOf('#');
                if (index >= 0)
                {
                    str = Strings.Left(str, index).Trim();
                }
                if (str.Length < 2)
                {
                    if (str.Length > 0)
                    {
                        num3++;
                    }
                }
                else if (str[0] != '[')
                {
                    if (num2 >= 0)
                    {
                        index = str.IndexOf('=');
                        if (index >= 0)
                        {
                            this.Sections[num2].CreateProperty(str.Substring(0, index).ToLower().Trim(), str.Substring(index + 1, (str.Length - index) - 1).Trim());
                        }
                        else
                        {
                            num3++;
                        }
                    }
                    else
                    {
                        index = str.IndexOf('=');
                        if (index >= 0)
                        {
                            this.RootSection.CreateProperty(str.Substring(0, index).ToLower().Trim(), str.Substring(index + 1, (str.Length - index) - 1).Trim());
                        }
                        else
                        {
                            num3++;
                        }
                    }
                }
                else if (str[str.Length - 1] != ']')
                {
                    num3++;
                }
                else
                {
                    string name = str.Substring(1, str.Length - 2);
                    int num4 = this.Sections.Count - 1;
                    index = 0;
                    while (index <= num4)
                    {
                        if (this.Sections[index].Name == name)
                        {
                            break;
                        }
                        index++;
                    }
                    num2 = index;
                    if (num2 == this.Sections.Count)
                    {
                        this.CreateSection(name);
                    }
                }
            }
        }

        public clsResult Translate(clsSectionTranslator Translator)
        {
            sErrorCount count;
            clsResult result = new clsResult("Translating INI");
            count.NameWarningCountMax = 0x10;
            count.ValueWarningCountMax = 0x10;
            int num2 = this.Sections.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                result.Add(this.Sections[i].Translate(i, Translator, ref count));
            }
            if (count.NameErrorCount > count.NameWarningCountMax)
            {
                result.WarningAdd("There were " + Conversions.ToString(count.NameErrorCount) + " unknown property names that were ignored.");
            }
            if (count.ValueErrorCount > count.ValueWarningCountMax)
            {
                result.WarningAdd("There were " + Conversions.ToString(count.ValueErrorCount) + " invalid values that were ignored.");
            }
            return result;
        }

        public class clsSection
        {
            public string Name;
            public modLists.SimpleList<sProperty> Properties = new modLists.SimpleList<sProperty>();

            public void CreateProperty(string Name, string Value)
            {
                sProperty newItem = new sProperty {
                    Name = Name,
                    Value = Value
                };
                this.Properties.Add(newItem);
            }

            public string GetLastPropertyValue(string LCasePropertyName)
            {
                for (int i = this.Properties.Count - 1; i >= 0; i += -1)
                {
                    sProperty property = this.Properties[i];
                    if (property.Name == LCasePropertyName)
                    {
                        sProperty property2 = this.Properties[i];
                        return property2.Value;
                    }
                }
                return null;
            }

            public clsResult ReadFile(StreamReader File)
            {
                clsResult result2 = new clsResult("");
                int num3 = 0;
                string str = null;
            Label_0012:
                str = File.ReadLine();
                if (str != null)
                {
                    str = str.Trim();
                    int index = str.IndexOf('#');
                    if (index >= 0)
                    {
                        str = Strings.Left(str, index).Trim();
                    }
                    if (str.Length >= 1)
                    {
                        index = str.IndexOf('=');
                        if (index >= 0)
                        {
                            this.CreateProperty(str.Substring(0, index).ToLower().Trim(), str.Substring(index + 1, (str.Length - index) - 1).Trim());
                        }
                        else
                        {
                            num3++;
                        }
                    }
                    else if (str.Length > 0)
                    {
                        num3++;
                    }
                    goto Label_0012;
                }
                this.Properties.RemoveBuffer();
                if (num3 > 0)
                {
                    result2.WarningAdd("There were " + Conversions.ToString(num3) + " invalid lines that were ignored.");
                }
                return result2;
            }

            public clsResult Translate(clsINIRead.clsTranslator Translator)
            {
                clsINIRead.sErrorCount count;
                clsResult result = new clsResult("Section " + this.Name);
                count.NameWarningCountMax = 0x10;
                count.ValueWarningCountMax = 0x10;
                int num2 = this.Properties.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    sProperty property;
                    switch (Translator.Translate(this.Properties[i]))
                    {
                        case clsINIRead.enumTranslatorResult.NameUnknown:
                            if (count.NameErrorCount < 0x10)
                            {
                                property = this.Properties[i];
                                result.WarningAdd("Property name \"" + property.Name + "\" is unknown.");
                            }
                            count.NameErrorCount++;
                            break;

                        case clsINIRead.enumTranslatorResult.ValueInvalid:
                            if (count.ValueErrorCount < 0x10)
                            {
                                string[] strArray = new string[5];
                                strArray[0] = "Value \"";
                                sProperty property2 = this.Properties[i];
                                strArray[1] = property2.Value;
                                strArray[2] = "\" for property name \"";
                                property = this.Properties[i];
                                strArray[3] = property.Name;
                                strArray[4] = "\" is not valid.";
                                result.WarningAdd(string.Concat(strArray));
                            }
                            count.ValueErrorCount++;
                            break;
                    }
                }
                if (count.NameErrorCount > count.NameWarningCountMax)
                {
                    result.WarningAdd("There were " + Conversions.ToString(count.NameErrorCount) + " unknown property names that were ignored.");
                }
                if (count.ValueErrorCount > count.ValueWarningCountMax)
                {
                    result.WarningAdd("There were " + Conversions.ToString(count.ValueErrorCount) + " invalid values that were ignored.");
                }
                return result;
            }

            public clsResult Translate(int SectionNum, clsINIRead.clsSectionTranslator Translator, ref clsINIRead.sErrorCount ErrorCount)
            {
                clsResult result = new clsResult("Section " + this.Name);
                int num2 = this.Properties.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    sProperty property;
                    switch (Translator.Translate(SectionNum, this.Properties[i]))
                    {
                        case clsINIRead.enumTranslatorResult.NameUnknown:
                            if (ErrorCount.NameErrorCount < 0x10)
                            {
                                property = this.Properties[i];
                                result.WarningAdd("Property name \"" + property.Name + "\" is unknown.");
                            }
                            ErrorCount.NameErrorCount++;
                            break;

                        case clsINIRead.enumTranslatorResult.ValueInvalid:
                            if (ErrorCount.ValueErrorCount < 0x10)
                            {
                                string[] strArray = new string[5];
                                strArray[0] = "Value \"";
                                sProperty property2 = this.Properties[i];
                                strArray[1] = property2.Value;
                                strArray[2] = "\" for property name \"";
                                property = this.Properties[i];
                                strArray[3] = property.Name;
                                strArray[4] = "\" is not valid.";
                                result.WarningAdd(string.Concat(strArray));
                            }
                            ErrorCount.ValueErrorCount++;
                            break;
                    }
                }
                return result;
            }

            [StructLayout(LayoutKind.Sequential)]
            public struct sProperty
            {
                public string Name;
                public string Value;
            }
        }

        public abstract class clsSectionTranslator
        {
            protected clsSectionTranslator()
            {
            }

            public abstract clsINIRead.enumTranslatorResult Translate(int INISectionNum, clsINIRead.clsSection.sProperty INIProperty);
        }

        public abstract class clsTranslator
        {
            protected clsTranslator()
            {
            }

            public abstract clsINIRead.enumTranslatorResult Translate(clsINIRead.clsSection.sProperty INIProperty);
        }

        public enum enumTranslatorResult : byte
        {
            NameUnknown = 0,
            Translated = 2,
            ValueInvalid = 1
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sErrorCount
        {
            public int NameErrorCount;
            public int ValueErrorCount;
            public int NameWarningCountMax;
            public int ValueWarningCountMax;
        }
    }
}

