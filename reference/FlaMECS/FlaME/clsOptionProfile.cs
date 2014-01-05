namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    public class clsOptionProfile : clsINIRead.clsTranslator
    {
        private clsChangeInterface[] _Changes;
        private clsOptionGroup _Options;

        public clsOptionProfile(clsOptionGroup options)
        {
            this._Options = options;
            this._Changes = new clsChangeInterface[(options.Options.Count - 1) + 1];
        }

        public virtual clsOptionProfile GetCopy(clsOptionProfileCreator creator)
        {
            creator.Options = this._Options;
            clsOptionProfile profile2 = creator.Create();
            int num2 = this._Options.Options.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                if (this._Changes[i] != null)
                {
                    profile2._Changes[i] = this._Changes[i].GetCopy();
                }
            }
            return profile2;
        }

        public clsResult INIWrite(clsINIWrite file)
        {
            IEnumerator enumerator;
            clsResult result2 = new clsResult("Writing options to INI");
            try
            {
                enumerator = this._Options.Options.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsOptionInterface current = (clsOptionInterface) enumerator.Current;
                    if (this.get_Changes(current) != null)
                    {
                        object objectValue = RuntimeHelpers.GetObjectValue(this.get_Value(current));
                        string iNIOutput = null;
                        if (current is clsOption<clsKeyboardControl>)
                        {
                            clsKeyboardControl control = (clsKeyboardControl) objectValue;
                            iNIOutput = "";
                            int upperBound = control.Keys.GetUpperBound(0);
                            for (int i = 0; i <= upperBound; i++)
                            {
                                Keys keys = control.Keys[i];
                                iNIOutput = iNIOutput + modIO.InvariantToString_int((int) keys);
                                if (i < control.Keys.GetUpperBound(0))
                                {
                                    iNIOutput = iNIOutput + ",";
                                }
                            }
                            if (control.UnlessKeys.GetUpperBound(0) >= 0)
                            {
                                iNIOutput = iNIOutput + "unless ";
                                int num5 = control.UnlessKeys.GetUpperBound(0);
                                for (int j = 0; j <= num5; j++)
                                {
                                    Keys keys2 = control.UnlessKeys[j];
                                    iNIOutput = iNIOutput + modIO.InvariantToString_int((int) keys2);
                                    if (j < control.UnlessKeys.GetUpperBound(0))
                                    {
                                        iNIOutput = iNIOutput + ",";
                                    }
                                }
                            }
                        }
                        else if (current is clsOption<modLists.SimpleList<string>>)
                        {
                            modLists.SimpleList<string> list = (modLists.SimpleList<string>) objectValue;
                            int num6 = list.Count - 1;
                            for (int k = 0; k <= num6; k++)
                            {
                                file.Property_Append(current.SaveKey, list[k]);
                            }
                        }
                        else if (current is clsOption<clsRGB_sng>)
                        {
                            iNIOutput = ((clsRGB_sng) objectValue).GetINIOutput();
                        }
                        else if (current is clsOption<clsRGBA_sng>)
                        {
                            iNIOutput = ((clsRGBA_sng) objectValue).GetINIOutput();
                        }
                        else if (current is clsOption<FontFamily>)
                        {
                            iNIOutput = ((FontFamily) objectValue).Name;
                        }
                        else if (current is clsOption<bool>)
                        {
                            iNIOutput = modIO.InvariantToString_bool(Conversions.ToBoolean(objectValue));
                        }
                        else if (current is clsOption<byte>)
                        {
                            iNIOutput = modIO.InvariantToString_byte(Conversions.ToByte(objectValue));
                        }
                        else if (current is clsOption<short>)
                        {
                            iNIOutput = modIO.InvariantToString_int(Conversions.ToShort(objectValue));
                        }
                        else if (current is clsOption<int>)
                        {
                            iNIOutput = modIO.InvariantToString_int(Conversions.ToInteger(objectValue));
                        }
                        else if (current is clsOption<uint>)
                        {
                            iNIOutput = modIO.InvariantToString_uint(Conversions.ToUInteger(objectValue));
                        }
                        else if (current is clsOption<float>)
                        {
                            iNIOutput = modIO.InvariantToString_sng(Conversions.ToSingle(objectValue));
                        }
                        else if (current is clsOption<double>)
                        {
                            iNIOutput = modIO.InvariantToString_dbl(Conversions.ToDouble(objectValue));
                        }
                        else if (current is clsOption<string>)
                        {
                            iNIOutput = Conversions.ToString(objectValue);
                        }
                        else
                        {
                            result2.ProblemAdd("Value for option \"" + current.SaveKey + "\" could not be written because it is of type " + objectValue.GetType().FullName);
                        }
                        if (iNIOutput != null)
                        {
                            file.Property_Append(current.SaveKey, iNIOutput);
                        }
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return result2;
        }

        public override clsINIRead.enumTranslatorResult Translate(clsINIRead.clsSection.sProperty INIProperty)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this._Options.Options.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsOptionInterface current = (clsOptionInterface) enumerator.Current;
                    if (current.SaveKey.ToLower() == INIProperty.Name)
                    {
                        if (current is clsOption<clsKeyboardControl>)
                        {
                            string[] strArray;
                            string[] strArray2;
                            int index = INIProperty.Value.ToLower().IndexOf("unless");
                            if (index < 0)
                            {
                                strArray = INIProperty.Value.Split(new char[] { ',' });
                                strArray2 = new string[0];
                            }
                            else
                            {
                                strArray = INIProperty.Value.Substring(0, index - 1).Split(new char[] { ',' });
                                strArray2 = INIProperty.Value.Substring(index + 6, INIProperty.Value.Length - (index + 6)).Split(new char[] { ',' });
                            }
                            Keys[] keys = new Keys[strArray.GetUpperBound(0) + 1];
                            bool flag = true;
                            int upperBound = strArray.GetUpperBound(0);
                            for (int i = 0; i <= upperBound; i++)
                            {
                                int num3;
                                if (modIO.InvariantParse_int(strArray[i], ref num3))
                                {
                                    keys[i] = (Keys) num3;
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            Keys[] unlessKeys = new Keys[strArray2.GetUpperBound(0) + 1];
                            int num13 = strArray2.GetUpperBound(0);
                            for (int j = 0; j <= num13; j++)
                            {
                                int num5;
                                if (modIO.InvariantParse_int(strArray2[j], ref num5))
                                {
                                    unlessKeys[j] = (Keys) num5;
                                }
                                else
                                {
                                    flag = false;
                                }
                            }
                            if (!flag)
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            clsKeyboardControl control = new clsKeyboardControl(keys, unlessKeys);
                            if (!current.IsValueValid(control))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<clsKeyboardControl>(control));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<modLists.SimpleList<string>>)
                        {
                            modLists.SimpleList<string> valueObject;
                            if (this.get_Changes(current) == null)
                            {
                                valueObject = new modLists.SimpleList<string>();
                                this.set_Changes(current, new clsChange<modLists.SimpleList<string>>(valueObject));
                            }
                            else
                            {
                                valueObject = (modLists.SimpleList<string>) this.get_Changes(current).ValueObject;
                            }
                            valueObject.Add(INIProperty.Value);
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<FontFamily>)
                        {
                            FontFamily family;
                            try
                            {
                                family = new FontFamily(INIProperty.Value);
                            }
                            catch (Exception exception1)
                            {
                                ProjectData.SetProjectError(exception1);
                                clsINIRead.enumTranslatorResult valueInvalid = clsINIRead.enumTranslatorResult.ValueInvalid;
                                ProjectData.ClearProjectError();
                                return valueInvalid;
                            }
                            if (!current.IsValueValid(family))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<FontFamily>(family));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<clsRGB_sng>)
                        {
                            clsRGB_sng _sng = new clsRGB_sng(0f, 0f, 0f);
                            if (!_sng.ReadINIText(new clsSplitCommaText(INIProperty.Value)))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(_sng))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<clsRGB_sng>(_sng));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<clsRGBA_sng>)
                        {
                            clsRGBA_sng _sng2 = new clsRGBA_sng(0f, 0f, 0f, 0f);
                            if (!_sng2.ReadINIText(new clsSplitCommaText(INIProperty.Value)))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(_sng2))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<clsRGBA_sng>(_sng2));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<bool>)
                        {
                            bool flag2;
                            if (!modIO.InvariantParse_bool(INIProperty.Value, ref flag2))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(flag2))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<bool>(flag2));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<byte>)
                        {
                            byte num6;
                            if (!modIO.InvariantParse_byte(INIProperty.Value, ref num6))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(num6))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<byte>(num6));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<short>)
                        {
                            short num7;
                            if (!modIO.InvariantParse_short(INIProperty.Value, ref num7))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(num7))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<short>(num7));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<int>)
                        {
                            int num8;
                            if (!modIO.InvariantParse_int(INIProperty.Value, ref num8))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(num8))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<int>(num8));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<uint>)
                        {
                            uint num9;
                            if (!modIO.InvariantParse_uint(INIProperty.Value, ref num9))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(num9))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<uint>(num9));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<float>)
                        {
                            float num10;
                            if (!modIO.InvariantParse_sng(INIProperty.Value, ref num10))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(num10))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<float>(num10));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<double>)
                        {
                            double num11;
                            if (!modIO.InvariantParse_dbl(INIProperty.Value, ref num11))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            if (!current.IsValueValid(num11))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<double>(num11));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        if (current is clsOption<string>)
                        {
                            string str = INIProperty.Value;
                            if (!current.IsValueValid(str))
                            {
                                return clsINIRead.enumTranslatorResult.ValueInvalid;
                            }
                            this.set_Changes(current, new clsChange<string>(str));
                            return clsINIRead.enumTranslatorResult.Translated;
                        }
                        return clsINIRead.enumTranslatorResult.ValueInvalid;
                    }
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            return clsINIRead.enumTranslatorResult.ValueInvalid;
        }

        public clsChangeInterface this[clsOptionInterface optionItem]
        {
            get
            {
                return this._Changes[optionItem.GroupLink.ArrayPosition];
            }
            set
            {
                this._Changes[optionItem.GroupLink.ArrayPosition] = value;
            }
        }

        public bool IsAnythingChanged
        {
            get
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this._Options.Options.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsOptionInterface current = (clsOptionInterface) enumerator.Current;
                        if (this.get_Changes(current) != null)
                        {
                            return true;
                        }
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return false;
            }
        }

        public clsOptionGroup Options
        {
            get
            {
                return this._Options;
            }
        }

        public object this[clsOptionInterface optionItem]
        {
            get
            {
                int arrayPosition = optionItem.GroupLink.ArrayPosition;
                clsChangeInterface interface2 = this._Changes[arrayPosition];
                if (interface2 == null)
                {
                    return optionItem.DefaultValueObject;
                }
                return interface2.ValueObject;
            }
        }

        public class clsChange<ValueType> : clsOptionProfile.clsChangeInterface
        {
            public ValueType Value;

            public clsChange(ValueType value)
            {
                this.Value = value;
            }

            public override clsOptionProfile.clsChangeInterface GetCopy()
            {
                return new clsOptionProfile.clsChange<ValueType>(this.Value);
            }

            public override object ValueObject
            {
                get
                {
                    return this.Value;
                }
            }
        }

        public abstract class clsChangeInterface
        {
            protected clsChangeInterface()
            {
            }

            public abstract clsOptionProfile.clsChangeInterface GetCopy();

            public abstract object ValueObject { get; }
        }
    }
}

