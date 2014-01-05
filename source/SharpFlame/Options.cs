using System;
using System.Drawing;
using System.Windows.Forms;
using Microsoft.VisualBasic;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;

namespace SharpFlame
{
    public class clsOptionGroup
    {
        public clsOptionGroup()
        {
            Options = new ConnectedList<clsOptionInterface, clsOptionGroup>(this);
        }

        public ConnectedList<clsOptionInterface, clsOptionGroup> Options;
    }

    public abstract class clsOptionInterface
    {
        public abstract ConnectedListLink<clsOptionInterface, clsOptionGroup> GroupLink { get; }

        public abstract string SaveKey { get; }
        public abstract object DefaultValueObject { get; }
        public abstract bool IsValueValid(object value);
    }

    public class clsOption<ValueType> : clsOptionInterface
    {
        private ConnectedListLink<clsOptionInterface, clsOptionGroup> _GroupLink;

        public override ConnectedListLink<clsOptionInterface, clsOptionGroup> GroupLink
        {
            get { return _GroupLink; }
        }

        private string _SaveKey;
        private ValueType _DefaultValue;

        public ValueType DefaultValue
        {
            get { return _DefaultValue; }
        }

        public clsOption(string saveKey, ValueType defaultValue)
        {
            _GroupLink = new ConnectedListLink<clsOptionInterface, clsOptionGroup>(this);


            _SaveKey = saveKey;
            _DefaultValue = defaultValue;
        }

        public override object DefaultValueObject
        {
            get { return _DefaultValue; }
        }

        public override string SaveKey
        {
            get { return _SaveKey; }
        }

        public override bool IsValueValid(object value)
        {
            return true;
        }
    }

    public class clsOptionCreator<ValueType>
    {
        public string SaveKey;
        public ValueType DefaultValue;

        public virtual clsOption<ValueType> Create()
        {
            return new clsOption<ValueType>(SaveKey, DefaultValue);
        }
    }

    public class clsOptionProfile : clsINIRead.clsTranslator
    {
        public abstract class clsChangeInterface
        {
            public abstract object ValueObject { get; }
            public abstract clsChangeInterface GetCopy();
        }

        public class clsChange<ValueType> : clsChangeInterface
        {
            public ValueType Value;

            public override object ValueObject
            {
                get { return Value; }
            }

            public clsChange(ValueType value)
            {
                Value = value;
            }

            public override clsChangeInterface GetCopy()
            {
                return new clsChange<ValueType>(Value);
            }
        }

        public bool IsAnythingChanged
        {
            get
            {
                foreach ( clsOptionInterface item in _Options.Options )
                {
                    if ( get_Changes(item) != null )
                    {
                        return true;
                    }
                }
                return false;
            }
        }

        private clsOptionGroup _Options;

        public clsOptionGroup Options
        {
            get { return _Options; }
        }

        private clsChangeInterface[] _Changes;

        public clsChangeInterface get_Changes(clsOptionInterface optionItem)
        {
            return _Changes[optionItem.GroupLink.ArrayPosition];
        }

        public void set_Changes(clsOptionInterface optionItem, clsChangeInterface value)
        {
            _Changes[optionItem.GroupLink.ArrayPosition] = value;
        }

        public object get_Value(clsOptionInterface optionItem)
        {
            int index = optionItem.GroupLink.ArrayPosition;
            clsChangeInterface change = _Changes[index];
            if ( change == null )
            {
                return optionItem.DefaultValueObject;
            }
            else
            {
                return change.ValueObject;
            }
        }

        public clsOptionProfile(clsOptionGroup options)
        {
            _Options = options;
            _Changes = new clsChangeInterface[options.Options.Count];
        }

        public virtual clsOptionProfile GetCopy(clsOptionProfileCreator creator)
        {
            creator.Options = _Options;
            clsOptionProfile result = creator.Create();

            for ( int i = 0; i <= _Options.Options.Count - 1; i++ )
            {
                if ( _Changes[i] != null )
                {
                    result._Changes[i] = _Changes[i].GetCopy();
                }
            }

            return result;
        }

        public clsResult INIWrite(clsINIWrite file)
        {
            clsResult returnResult = new clsResult("Writing options to INI");

            foreach ( clsOptionInterface item in _Options.Options )
            {
                if ( get_Changes(item) == null )
                {
                    continue;
                }
                object optionValue = get_Value(item);
                string valueText = null;
                if ( item is clsOption<clsKeyboardControl> )
                {
                    clsKeyboardControl control = (clsKeyboardControl)optionValue;
                    valueText = "";
                    for ( int i = 0; i <= control.Keys.GetUpperBound(0); i++ )
                    {
                        Keys key = Keys.A;
                        valueText += IOUtil.InvariantToString_int((Int32)key);
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
                            valueText += IOUtil.InvariantToString_int((Int32)key);
                            if ( i < control.UnlessKeys.GetUpperBound(0) )
                            {
                                valueText += ",";
                            }
                        }
                    }
                }
                else if ( item is clsOption<SimpleList<string>> )
                {
                    SimpleList<string> list = (SimpleList<string>)optionValue;
                    for ( int i = 0; i <= list.Count - 1; i++ )
                    {
                        file.Property_Append(item.SaveKey, list[i]);
                    }
                }
                else if ( item is clsOption<clsRGB_sng> )
                {
                    valueText = ((clsRGB_sng)optionValue).GetINIOutput();
                }
                else if ( item is clsOption<clsRGBA_sng> )
                {
                    valueText = ((clsRGBA_sng)optionValue).GetINIOutput();
                }
                else if ( item is clsOption<FontFamily> )
                {
                    valueText = ((FontFamily)optionValue).Name;
                }
                else if ( item is clsOption<bool> )
                {
                    valueText = IOUtil.InvariantToString_bool(Convert.ToBoolean(optionValue));
                }
                else if ( item is clsOption<byte> )
                {
                    valueText = IOUtil.InvariantToString_byte(Convert.ToByte(optionValue));
                }
                else if ( item is clsOption<short> )
                {
                    valueText = IOUtil.InvariantToString_int((short)optionValue);
                }
                else if ( item is clsOption<int> )
                {
                    valueText = IOUtil.InvariantToString_int(Convert.ToInt32(optionValue));
                }
                else if ( item is clsOption<UInt32> )
                {
                    valueText = IOUtil.InvariantToString_uint(Convert.ToUInt32(optionValue));
                }
                else if ( item is clsOption<Single> )
                {
                    valueText = IOUtil.InvariantToString_sng(Convert.ToSingle(Convert.ToSingle(optionValue)));
                }
                else if ( item is clsOption<double> )
                {
                    valueText = IOUtil.InvariantToString_dbl(Convert.ToDouble(optionValue));
                }
                else if ( item is clsOption<string> )
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
                    file.Property_Append(item.SaveKey, valueText);
                }
            }

            return returnResult;
        }

        public override clsINIRead.enumTranslatorResult Translate(clsINIRead.clsSection.sProperty INIProperty)
        {
            foreach ( clsOptionInterface item in _Options.Options )
            {
                if ( item.SaveKey.ToLower() != INIProperty.Name )
                {
                    continue;
                }
                if ( item is clsOption<clsKeyboardControl> )
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
                        if ( IOUtil.InvariantParse_int(keysText[j], ref number) )
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
                        if ( IOUtil.InvariantParse_int(unlessKeysText[j], ref number) )
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
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    clsKeyboardControl control = new clsKeyboardControl(keys, unlessKeys);
                    if ( !item.IsValueValid(control) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<clsKeyboardControl>(control));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<SimpleList<string>> )
                {
                    SimpleList<string> list = default(SimpleList<string>);
                    if ( get_Changes(item) == null )
                    {
                        list = new SimpleList<string>();
                        set_Changes(item, new clsChange<SimpleList<string>>(list));
                    }
                    else
                    {
                        list = (SimpleList<string>)(get_Changes(item).ValueObject);
                    }
                    list.Add(INIProperty.Value);
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<FontFamily> )
                {
                    FontFamily fontFamily = default(FontFamily);
                    try
                    {
                        fontFamily = new FontFamily(Convert.ToString(INIProperty.Value));
                    }
                    catch
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(fontFamily) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<FontFamily>(fontFamily));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<clsRGB_sng> )
                {
                    clsRGB_sng value = new clsRGB_sng(0.0F, 0.0F, 0.0F);
                    if ( !value.ReadINIText(new SplitCommaText(Convert.ToString(INIProperty.Value))) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<clsRGB_sng>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<clsRGBA_sng> )
                {
                    clsRGBA_sng value = new clsRGBA_sng(0.0F, 0.0F, 0.0F, 0.0F);
                    if ( !value.ReadINIText(new SplitCommaText(Convert.ToString(INIProperty.Value))) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<clsRGBA_sng>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<bool> )
                {
                    bool value = default(bool);
                    if ( !IOUtil.InvariantParse_bool(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<bool>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<byte> )
                {
                    byte value = 0;
                    if ( !IOUtil.InvariantParse_byte(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<byte>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<short> )
                {
                    short value = 0;
                    if ( !IOUtil.InvariantParse_short(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<short>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<int> )
                {
                    int value = 0;
                    if ( !IOUtil.InvariantParse_int(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<int>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<UInt32> )
                {
                    UInt32 value = 0;
                    if ( !IOUtil.InvariantParse_uint(Convert.ToString(INIProperty.Value), value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<UInt32>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<Single> )
                {
                    float value = 0;
                    if ( !IOUtil.InvariantParse_sng(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<Single>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<double> )
                {
                    double value = 0;
                    if ( !IOUtil.InvariantParse_dbl(Convert.ToString(INIProperty.Value), ref value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<double>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else if ( item is clsOption<string> )
                {
                    string value = Convert.ToString(INIProperty.Value);
                    if ( !item.IsValueValid(value) )
                    {
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                    set_Changes(item, new clsChange<string>(value));
                    return clsINIRead.enumTranslatorResult.Translated;
                }
                else
                {
                    return clsINIRead.enumTranslatorResult.ValueInvalid;
                }
            }

            return clsINIRead.enumTranslatorResult.ValueInvalid;
        }
    }

    public class clsOptionProfileCreator
    {
        public clsOptionGroup Options;

        public clsOptionProfileCreator()
        {
        }

        public clsOptionProfileCreator(clsOptionGroup options)
        {
            Options = options;
        }

        public virtual clsOptionProfile Create()
        {
            return new clsOptionProfile(Options);
        }
    }
}