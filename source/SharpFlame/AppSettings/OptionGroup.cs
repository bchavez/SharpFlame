#region

using SharpFlame.Collections;

#endregion

namespace SharpFlame.AppSettings
{
    public class OptionGroup
    {
        public ConnectedList<OptionInterface, OptionGroup> Options;

        public OptionGroup()
        {
            Options = new ConnectedList<OptionInterface, OptionGroup>(this);
        }
    }
}