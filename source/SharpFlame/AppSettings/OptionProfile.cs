#region

using Newtonsoft.Json;

#endregion

namespace SharpFlame.AppSettings
{
    public class OptionProfile
    {
        private readonly ChangeInterface[] changes;

        [JsonIgnore]
        private readonly OptionGroup options;

        public OptionProfile(OptionGroup newOptions)
        {
            options = newOptions;
            changes = new ChangeInterface[newOptions.Options.Count];
        }

        [JsonIgnore]
        public bool IsAnythingChanged
        {
            get
            {
                foreach ( var item in options.Options )
                {
                    if ( GetChanges(item) != null )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        [JsonIgnore]
        public OptionGroup Options
        {
            get { return options; }
        }

        public ChangeInterface GetChanges(OptionInterface optionItem)
        {
            return changes[optionItem.GroupLink.ArrayPosition];
        }

        public void SetChanges(OptionInterface optionItem, ChangeInterface value)
        {
            changes[optionItem.GroupLink.ArrayPosition] = value;
        }

        public object GetValue(OptionInterface optionItem)
        {
            var index = optionItem.GroupLink.ArrayPosition;
            var change = changes[index];
            if ( change == null )
            {
                return optionItem.DefaultValueObject;
            }
            return change.ValueObject;
        }

        public virtual OptionProfile GetCopy(OptionProfileCreator creator)
        {
            creator.Options = options;
            var result = creator.Create ();

            for (var i = 0; i <= options.Options.Count - 1; i++) {
                if (changes [i] != null) {
                    result.changes [i] = changes [i].GetCopy ();
                }
            }

            return result;
        }
    }
}