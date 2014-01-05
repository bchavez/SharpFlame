using SharpFlame.Collections;

namespace SharpFlame.AppSettings
{
    public class OptionGroup
    {
        public OptionGroup()
        {
            Options = new ConnectedList<OptionInterface, OptionGroup>(this);
        }

        public ConnectedList<OptionInterface, OptionGroup> Options;
    }
}