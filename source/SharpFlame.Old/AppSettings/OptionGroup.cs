#region

using Newtonsoft.Json;
using SharpFlame.Old.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.Old.AppSettings
{
    public class OptionGroup
    {
        [JsonIgnore]
        public ConnectedList<OptionInterface, OptionGroup> Options;

        public OptionGroup()
        {
            Options = new ConnectedList<OptionInterface, OptionGroup>(this);
        }
    }
}