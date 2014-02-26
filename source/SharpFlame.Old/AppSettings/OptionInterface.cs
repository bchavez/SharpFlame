#region

using SharpFlame.Old.Collections;
using SharpFlame.Core.Collections;

#endregion

namespace SharpFlame.Old.AppSettings
{
    public abstract class OptionInterface
    {
        public abstract ConnectedListLink<OptionInterface, OptionGroup> GroupLink { get; }

        public abstract string SaveKey { get; }
        public abstract object DefaultValueObject { get; }
        public abstract bool IsValueValid(object value);
    }
}