namespace SharpFlame.AppSettings
{
    public class KeyboardProfile : OptionProfile
    {
        public bool Active(Option<KeyboardControl> control)
        {
            return ((KeyboardControl)(get_Value(control))).Active;
        }

        public KeyboardProfile(OptionGroup options) : base(options)
        {
        }
    }
}