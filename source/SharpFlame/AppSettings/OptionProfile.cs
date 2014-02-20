#region

using System;
using System.Drawing;
using System.Windows.Forms;
using Newtonsoft.Json;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.FileIO;

#endregion

namespace SharpFlame.AppSettings
{
    public class OptionProfile
    {
        private readonly ChangeInterface[] _Changes;

        [JsonIgnore]
        private readonly OptionGroup options;

        public OptionProfile(OptionGroup newOptions)
        {
            options = newOptions;
            _Changes = new ChangeInterface[newOptions.Options.Count];
        }

        [JsonIgnore]
        public bool IsAnythingChanged
        {
            get
            {
                foreach ( var item in options.Options )
                {
                    if ( get_Changes(item) != null )
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

        public ChangeInterface get_Changes(OptionInterface optionItem)
        {
            return _Changes[optionItem.GroupLink.ArrayPosition];
        }

        public void set_Changes(OptionInterface optionItem, ChangeInterface value)
        {
            _Changes[optionItem.GroupLink.ArrayPosition] = value;
        }

        public object get_Value(OptionInterface optionItem)
        {
            var index = optionItem.GroupLink.ArrayPosition;
            var change = _Changes[index];
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
                if (_Changes [i] != null) {
                    result._Changes [i] = _Changes [i].GetCopy ();
                }
            }

            return result;
        }
    }
}