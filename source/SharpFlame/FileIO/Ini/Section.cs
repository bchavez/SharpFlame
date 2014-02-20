#region

using System;
using System.IO;
using SharpFlame.Collections;

#endregion

namespace SharpFlame.FileIO.Ini
{
    public class Section
    {
        public string Name;

#if !Mono
        public struct SectionProperty
#else
        public class SectionProperty
#endif

        {
            public string Name;

            public string Value;
        }

        public SimpleList<SectionProperty> Properties = new SimpleList<SectionProperty>();

        public void CreateProperty(string Name, string Value)
        {
            var NewProperty = new SectionProperty();

            NewProperty.Name = Name;
            NewProperty.Value = Value;
            Properties.Add(NewProperty);
        }

        public clsResult ReadFile(StreamReader File)
        {
            var ReturnResult = new clsResult("", false);

            var InvalidLineCount = 0;
            string LineText = null;
            var A = 0;

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
                    LineText = LineText.Substring(0, A).Trim();
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
                ReturnResult.WarningAdd("There were " + Convert.ToString(InvalidLineCount) + " invalid lines that were ignored.");
            }

            return ReturnResult;
        }

        public clsResult Translate(int SectionNum, SectionTranslator Translator, ErrorCount ErrorCount)
        {
            var ReturnResult = new clsResult("Section " + Name);

            var A = 0;
            var TranslatorResult = default(TranslatorResult);

            for ( A = 0; A <= Properties.Count - 1; A++ )
            {
                TranslatorResult = Translator.Translate(SectionNum, Properties[A]);
                switch ( TranslatorResult )
                {
                    case TranslatorResult.NameUnknown:
                        if ( ErrorCount.NameErrorCount < 16 )
                        {
                            ReturnResult.WarningAdd("Property name \"{0}\" is unknown.".Format2(Properties[A].Name));
                        }
                        ErrorCount.NameErrorCount++;
                        break;
                    case TranslatorResult.ValueInvalid:
                        if ( ErrorCount.ValueErrorCount < 16 )
                        {
                            ReturnResult.WarningAdd("Value \"{0}\" for property name \"{1}\" is not valid."
                                .Format2(Properties[A].Value, Properties[A].Name));
                        }
                        ErrorCount.ValueErrorCount++;
                        break;
                }
            }

            return ReturnResult;
        }

        public clsResult Translate(Translator Translator)
        {
            var ReturnResult = new clsResult("Section " + Name);

            var A = 0;
            var TranslatorResult = default(TranslatorResult);
            var ErrorCount = new ErrorCount();

            ErrorCount.NameWarningCountMax = 16;
            ErrorCount.ValueWarningCountMax = 16;

            for ( A = 0; A <= Properties.Count - 1; A++ )
            {
                TranslatorResult = Translator.Translate(Properties[A]);
                switch ( TranslatorResult )
                {
                    case TranslatorResult.NameUnknown:
                        if ( ErrorCount.NameErrorCount < 16 )
                        {
                            ReturnResult.WarningAdd("Property name \"{0}\" is unknown.".Format2(Properties[A].Name));
                        }
                        ErrorCount.NameErrorCount++;
                        break;
                    case TranslatorResult.ValueInvalid:
                        if ( ErrorCount.ValueErrorCount < 16 )
                        {
                            ReturnResult.WarningAdd("Value \"{0}\" for property name \"{1}\" is not valid."
                                .Format2(Properties[A].Value, Properties[A].Name));
                        }
                        ErrorCount.ValueErrorCount++;
                        break;
                }
            }

            if ( ErrorCount.NameErrorCount > ErrorCount.NameWarningCountMax )
            {
                ReturnResult.WarningAdd("There were " + Convert.ToString(ErrorCount.NameErrorCount) + " unknown property names that were ignored.");
            }
            if ( ErrorCount.ValueErrorCount > ErrorCount.ValueWarningCountMax )
            {
                ReturnResult.WarningAdd("There were " + Convert.ToString(ErrorCount.ValueErrorCount) + " invalid values that were ignored.");
            }

            return ReturnResult;
        }

        public string GetLastPropertyValue(string LCasePropertyName)
        {
            var A = 0;

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
}