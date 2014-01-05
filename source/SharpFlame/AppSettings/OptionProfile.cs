using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.FileIO.Ini;

namespace SharpFlame.AppSettings
{
    public class OptionProfile : Translator
    {
        public bool IsAnythingChanged
        {
            get
            {
                foreach ( OptionInterface item in _Options.Options )
                {
                    if ( get_Changes(item) != null )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private OptionGroup _Options;

        public OptionGroup Options
        {
            get { return _Options; }
        }

        private ChangeInterface[] _Changes;

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
            int index = optionItem.GroupLink.ArrayPosition;
            ChangeInterface change = _Changes[index];
            if ( change == null )
            {
                return optionItem.DefaultValueObject;
            }
            else
            {
                return change.ValueObject;
            }
        }

        public OptionProfile(OptionGroup options)
        {
            _Options = options;
            _Changes = new ChangeInterface[options.Options.Count];
        }

        public virtual OptionProfile GetCopy(OptionProfileCreator creator)
        {
            creator.Options = _Options;
            OptionProfile result = creator.Create();

            for ( int i = 0; i <= _Options.Options.Count - 1; i++ )
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
            clsResult returnResult = new clsResult("Writing options to INI");

            foreach ( OptionInterface item in _Options.Options )
            {
                if ( get_Changes(item) == null )
                {
                    continue;
                }
                object optionValue = get_Value(item);
                string valueText = null;
                if ( item is Option<KeyboardControl> )
                {
                    KeyboardControl control = (KeyboardControl)optionValue;
                    valueText = "";
                    for ( int i = 0; i <= control.Keys.GetUpperBound(0); i++ )
                    {
                        Keys key = Keys.A;
                        valueText += IOUtil.InvariantToString((Int32)key);
                        if ( i < control.Keys.GetUpperBound(0) )
                        {
                            valueText += ",";
                        }
                    }
                    if ( control.UnlessKeys.GetUpperBound(0) >= 0 )
                    {
                        valueText += "unless ";
                        for ( int i = 0; i <= control.UnlessKeys.GetUpperBound(0); i++ )
                        {
                            Keys key = Keys.A;
                            valueText += IOUtil.InvariantToString((Int32)key);
                            if ( i < control.UnlessKeys.GetUpperBound(0) )
                            {
                                valueText += ",";
                            }
                        }
                    }
                }
                else if ( item is Option<SimpleList<string>> )
                {
                    SimpleList<string> list = (SimpleList<string>)optionValue;
                    for ( int i = 0; i <= list.Count - 1; i++ )
                    {
                        file.AppendProperty(item.SaveKey, list[i]);
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
                    valueText = IOUtil.InvariantToString(Convert.ToBoolean(optionValue));
                }
                else if ( item is Option<byte> )
                {
                    valueText = IOUtil.InvariantToString(Convert.ToByte(optionValue));
                }
                else if ( item is Option<short> )
                {
                    valueText = IOUtil.InvariantToString((int)(short)optionValue);
                }
                else if ( item is Option<int> )
                {
                    valueText = IOUtil.InvariantToString(Convert.ToInt32(optionValue));
                }
                else if ( item is Option<UInt32> )
                {
                    valueText = IOUtil.InvariantToString(Convert.ToUInt32(optionValue));
                }
                else if ( item is Option<Single> )
                {
                    valueText = IOUtil.InvariantToString(Convert.ToSingle(Convert.ToSingle(optionValue)));
                }
                else if ( item is Option<double> )
                {
                    valueText = IOUtil.InvariantToString(Convert.ToDouble(optionValue));
                }
                else if ( item is Option<string> )
                {
                    valueText = Convert.ToString(optionValue);
                }
                else
                {
                    returnResult.ProblemAdd("Value for option " + Convert.ToString(ControlChars.Quote) + item.SaveKey +
                                            Convert.ToString(ControlChars.Quote) + " could not be written because it is of type " +
                                            optionValue.GetType().FullName);
                }
                if ( valueText != null )
                {
                    file.AppendProperty(item.SaveKey, valueText);
                }
            }

            return returnResult;
        }

        public override TranslatorResult Translate(Section.SectionProperty INIProperty)
        {
            foreach ( OptionInterface item in _Options.Options )
            {
                if ( item.SaveKey.ToLower() != INIProperty.Name )
                {
                    continue;
                }
                if ( item is Option<KeyboardControl> )
                {
                    int unlessIndex = Convert.ToInt32(INIProperty.Value.ToLower().IndexOf("unless"));
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

                    Keys[] keys = new Keys[keysText.GetUpperBound(0) + 1];

                    bool valid = true;
                    for ( int j = 0; j <= keysText.GetUpperBound(0); j++ )
                    {
                        int number = 0;
                        if ( IOUtil.InvariantParse(keysText[j], ref number) )
                        {
                            keys[j] = (Keys)number;
                        }
                        else
                        {
                            valid = false;
                        }
                    }
                    Keys[] unlessKeys = new Keys[unlessKeysText.GetUpperBound(0) + 1];
                    for ( int j = 0; j <= unlessKeysText.GetUpperBound(0); j++ )
                    {
                        int number = 0;
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
                    KeyboardControl control = new KeyboardControl(keys, unlessKeys);
                    if ( !item.IsValueValid(control) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<KeyboardControl>(control));
                    return TranslatorResult.Translated;
                }
                else if ( item is Option<SimpleList<string>> )
                {
                    SimpleList<string> list = default(SimpleList<string>);
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
                else if ( item is Option<FontFamily> )
                {
                    FontFamily fontFamily = default(FontFamily);
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
                else if ( item is Option<clsRGB_sng> )
                {
                    clsRGB_sng value = new clsRGB_sng(0.0F, 0.0F, 0.0F);
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
                else if ( item is Option<clsRGBA_sng> )
                {
                    clsRGBA_sng value = new clsRGBA_sng(0.0F, 0.0F, 0.0F, 0.0F);
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
                else if ( item is Option<bool> )
                {
                    bool value = default(bool);
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
                else if ( item is Option<byte> )
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
                else if ( item is Option<short> )
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
                else if ( item is Option<int> )
                {
                    int value = 0;
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
                else if ( item is Option<UInt32> )
                {
                    UInt32 value = 0;
                    if ( !IOUtil.InvariantParse(Convert.ToString(INIProperty.Value), value) )
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
                else if ( item is Option<Single> )
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
                else if ( item is Option<double> )
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
                else if ( item is Option<string> )
                {
                    string value = Convert.ToString(INIProperty.Value);
                    if ( !item.IsValueValid(value) )
                    {
                        return TranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new Change<string>(value));
                    return TranslatorResult.Translated;
                }
                else
                {
                    return TranslatorResult.ValueInvalid;
                }
            }

            return TranslatorResult.ValueInvalid;
        }
    }
}