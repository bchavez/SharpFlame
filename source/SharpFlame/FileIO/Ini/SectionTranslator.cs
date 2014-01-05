namespace SharpFlame.FileIO.Ini
{
    public abstract class SectionTranslator
    {
        public abstract TranslatorResult Translate(int INISectionNum, Section.SectionProperty INIProperty);
    }
}