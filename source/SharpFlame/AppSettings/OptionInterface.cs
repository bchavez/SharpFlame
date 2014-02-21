#region

using SharpFlame.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.AppSettings
{
    public abstract class OptionInterface
    {
        public abstract ConnectedListLink<OptionInterface, OptionGroup> GroupLink { get; }

        public abstract string SaveKey { get; }
        public abstract object DefaultValueObject { get; }
        public abstract bool IsValueValid(object value);
    }
}