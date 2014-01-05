namespace SharpFlame.AppSettings
{
    public class OptionProfileCreator
    {
        public OptionGroup Options;

        public OptionProfileCreator()
        {
        }

        public OptionProfileCreator(OptionGroup options)
        {
            Options = options;
        }

        public virtual OptionProfile Create()
        {
            return new OptionProfile(Options);
        }
    }
}