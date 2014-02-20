namespace SharpFlame.AppSettings
{
    public class KeyboardProfile : OptionProfile
    {
        public KeyboardProfile(OptionGroup options) : base(options)
        {
        }

        public bool Active(Option<KeyboardControl> control)
        {
            return ((KeyboardControl)(get_Value(control))).Active;
        }
    }
}