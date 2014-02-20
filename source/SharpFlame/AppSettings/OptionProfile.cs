#region

using System;
using System.Drawing;
using System.Windows.Forms;
using NLog;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Core.Parsers.Ini;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;
using Section = SharpFlame.FileIO.Ini.Section;

#endregion

namespace SharpFlame.AppSettings
{
    public class OptionProfile : Translator
    {
        private static readonly Logger logger = LogManager.GetCurrentClassLogger();
        private readonly ChangeInterface[] _Changes;

        private readonly OptionGroup _Options;

        public OptionProfile(OptionGroup options)
        {
            _Options = options;
            _Changes = new ChangeInterface[options.Options.Count];
        }

        public bool IsAnythingChanged
        {
            get
            {
                foreach ( var item in _Options.Options )
                {
                    if ( get_Changes(item) != null )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        public OptionGroup Options
        {
            get { return _Options; }
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
            creator.Options = _Options;
            var result = creator.Create();

            for ( var i = 0; i <= _Options.Options.Count - 1; i++ )
            {
                if ( _Changes[i] != null )
                {
                    result._Changes[i] = _Changes[i].GetCopy();
                }
            }

            return result;
        }

        public clsResult INIWrite(IniWriter file)
        {
            var returnResult = new clsResult("Writing options to INI", false);
            logger.Info("Writing options to INI");

            foreach ( var item in _Options.Options )
            {
                if ( get_Changes(item) == null )
                {
                    continue;
                }
                var optionValue = get_Value(item);
                string valueText = null;
                if ( item is Option<KeyboardControl> )
                {
                    var control = (KeyboardControl)optionValue;
                    valueText = "";
                    for ( var i = 0; i <= control.Keys.GetUpperBound(0); i++ )
                    {
                        var key = Keys.A;
                        valueText += ((Int32)key).ToStringInvariant();
                        if ( i < control.Keys.GetUpperBound(0) )
                        {
                            valueText += ",";
                        }
                    }
                    if ( control.UnlessKeys.GetUpperBound(0) >= 0 )
                    {
                        valueText += "unless ";
                        for ( var i = 0; i <= control.UnlessKeys.GetUpperBound(0); i++ )
                        {
                            var key = Keys.A;
                            valueText += ((Int32)key).ToStringInvariant();
                            if ( i < control.UnlessKeys.GetUpperBound(0) )
                            {
                                valueText += ",";
                            }
                        }
                    }
                }
                else if ( item is Option<SimpleList<string>> )
                {
                    var list = (SimpleList<string>)optionValue;
                    for ( var i = 0; i <= list.Count - 1; i++ )
                    {
                        file.AddProperty(item.SaveKey, list[i]);
                    }
                }
                else if ( item is Option<clsRGB_sng> )
                {
                    valueText = ((clsRGB_sng)optionValue).GetINIOutput();
                }
                else if ( item is Option<clsRGBA_sng> )
                {
                    valueText = ((clsRGBA_sng)optionValue).GetINIOutput();
                }
                else if ( item is Option<FontFamily> )
                {
                    valueText = ((FontFamily)optionValue).Name;
                }
                else if ( item is Option<bool> )
                {
                    valueText = Convert.ToBoolean(optionValue).ToStringInvariant();
                }
                else if ( item is Option<byte> )
                {
                    valueText = Convert.ToByte(optionValue).ToStringInvariant();
                }
                else if ( item is Option<short> )
                {
                    valueText = ((int)(short)optionValue).ToStringInvariant();
                }
                else if ( item is Option<int> )
                {
                    valueText = Convert.ToInt32(optionValue).ToStringInvariant();
                }
                else if ( item is Option<UInt32> )
                {
                    valueText = Convert.ToUInt32(optionValue).ToStringInvariant();
                }
                else if ( item is Option<Single> )
                {
                    valueText = Convert.ToSingle(Convert.ToSingle(optionValue)).ToStringInvariant();
                }
                else if ( item is Option<double> )
                {
                    valueText = Convert.ToDouble(optionValue).ToStringInvariant();
                }
                else if ( item is Option<string> )
                {
                    valueText = Convert.ToString(optionValue);
                }
                else
                {
                    returnResult.ProblemAdd(
                        "Value for option \"{0\" could not be written because it is of type \"{1}\""
                            .Format2(item.SaveKey, optionValue.GetType().FullName)
                        );
                }
                if ( valueText != null )
                {
                    file.AddProperty(item.SaveKey, valueText);
                }
            }

            return returnResult;
        }

        public override TranslatorResult Translate(Section.SectionProperty INIProperty)
        {
            foreach ( var item in _Options.Options )
            {
                if ( item.SaveKey.ToLower() != INIProperty.Name )
                {
                    continue;
                }
                if ( item is Option<KeyboardControl> )
                {
                    var unlessIndex = Convert.ToInt32(INIProperty.Value.ToLower().IndexOf("unless"));
                    string[] keysText = null;
                    string[] unlessKeysText = null;
                    if ( unlessIndex < 0 )
                    {
                        keysText = INIProperty.Value.Split(',');
                        unlessKeysText = new string[0];
                    }
                    else
                    {
                        keysText = INIProperty.Value.Substring(0, unlessIndex - 1).Split(',');
                        unlessKeysText = INIProperty.Value.Substring(unlessIndex + 6, INIProperty.Value.Length - (unlessIndex + 6)).Split(',');
                    }

                    var keys = new Keys[keysText.GetUpperBound(0) + 1];

                    var valid = true;
                    for ( var j = 0; j <= keysText.GetUpperBound(0); j++ )
                    {
                        var number = 0;
                        if ( IOUtil.InvariantParse(keysText[j], ref number) )
                        {
                            keys[j] = (Keys)number;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    var unlessKeys = new Keys[unlessKeysText.GetUpperBound(0) + 1];
                    for ( var j = 0; j <= unlessKeysText.GetUpperBound(0); j++ )
                    {
                        var number = 0;
                        if ( IOUtil.InvariantParse(unlessKeysText[j], ref number) )
                        {
                            unlessKeys[j] = (Keys)number;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    if ( !valid )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    var control = new KeyboardControl(keys, unlessKeys);
                    if ( !item.IsValueValid(control) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<KeyboardControl>(control));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<SimpleList<string>> )
                {
                    var list = default(SimpleList<string>);
                    if ( get_Changes(item) == null )
                    {
                        list = new SimpleList<string>();
                        set_Changes(item, new Change<SimpleList<string>>(list));
                    }
                    else
                    {
                        list = (SimpleList<string>)(get_Changes(item).ValueObject);
                    }
                    list.Add(INIProperty.Value);
                    return TranslatorResult.Translated;
                }
                if ( item is Option<FontFamily> )
                {
                    var fontFamily = default(FontFamily);
                    try
                    {
                        fontFamily = new FontFamily(Convert.ToString(INIProperty.Value));
                    }
                    catch
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(fontFamily) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<FontFamily>(fontFamily));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<clsRGB_sng> )
                {
                    var value = new clsRGB_sng(0.0F, 0.0F, 0.0F);
                    if ( !value.ReadINIText(new SplitCommaText(Convert.ToString(INIProperty.Value))) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<clsRGB_sng>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<clsRGBA_sng> )
                {
                    var value = new clsRGBA_sng(0.0F, 0.0F, 0.0F, 0.0F);
                    if ( !value.ReadINIText(new SplitCommaText(Convert.ToString(INIProperty.Value))) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<clsRGBA_sng>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<bool> )
                {
                    var value = default(bool);
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<bool>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<byte> )
                {
                    byte value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<byte>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<short> )
                {
                    short value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<short>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<int> )
                {
                    var value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<int>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<UInt32> )
                {
                    UInt32 value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<UInt32>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<Single> )
                {
                    float value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<Single>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<double> )
                {
                    double value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<double>(value));
                    return TranslatorResult.Translated;
                }
                if ( item is Option<string> )
                {
                    var value = Convert.ToString(INIProperty.Value);
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<string>(value));
                    return TranslatorResult.Translated;
                }
                return TranslatorResult.ValueInvalid;
            }

            return TranslatorResult.ValueInvalid;
        }
    }
}