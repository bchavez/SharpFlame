namespace SharpFlame.Old.AppSettings
{
    public class KeyboardProfile : OptionProfile
    {
        public KeyboardProfile(OptionGroup options) : base(options)
        {
        }

        public bool Active(Option<KeyboardControl> control)
        {
            return ((KeyboardControl)(GetValue(control))).Active;
        }
    }
}