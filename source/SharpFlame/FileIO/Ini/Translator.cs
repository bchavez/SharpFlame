namespace SharpFlame.FileIO.Ini
{
    public abstract class Translator
    {
        public abstract TranslatorResult Translate(Section.SectionProperty INIProperty);
    }
}