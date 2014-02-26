namespace SharpFlame.Old.AppSettings
{
    public class KeyboardProfileCreator : OptionProfileCreator
    {
        public override OptionProfile Create()
        {
            return new KeyboardProfile(Options);
        }
    }
}