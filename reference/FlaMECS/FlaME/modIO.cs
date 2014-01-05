namespace FlaME
{
    using ICSharpCode.SharpZipLib.Zip;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.Globalization;
    using System.IO;

    [StandardModule]
    public sealed class modIO
    {
        public static modLists.SimpleList<string> BytesToLinesRemoveComments(BinaryReader reader)
        {
            char ch;
            bool flag;
            bool flag2;
            bool flag3;
            char ch2;
            string newItem = "";
            modLists.SimpleList<string> list2 = new modLists.SimpleList<string>();
        Label_000E:
            ch2 = ch;
            bool flag4 = flag;
            try
            {
                ch = reader.ReadChar();
                flag = true;
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                flag = false;
                ProjectData.ClearProjectError();
            }
            if (flag)
            {
                char ch3 = ch;
                switch (ch3)
                {
                    case '\r':
                    case '\n':
                        flag3 = false;
                        if (flag4)
                        {
                            newItem = newItem + Conversions.ToString(ch2);
                        }
                        flag = false;
                        if (newItem.Length > 0)
                        {
                            list2.Add(newItem);
                            newItem = "";
                        }
                        goto Label_000E;
                }
                if (ch3 == '*')
                {
                    if (!(flag4 & (ch2 == '/')))
                    {
                        goto Label_00F2;
                    }
                    flag2 = true;
                    flag = false;
                }
                else
                {
                    if ((ch3 != '/') || !flag4)
                    {
                        goto Label_00F2;
                    }
                    if (ch2 == '/')
                    {
                        flag3 = true;
                        flag = false;
                    }
                    else
                    {
                        if (ch2 != '*')
                        {
                            goto Label_00F2;
                        }
                        flag2 = false;
                        flag = false;
                    }
                }
                goto Label_000E;
            }
            flag3 = false;
            if (flag4)
            {
                newItem = newItem + Conversions.ToString(ch2);
            }
            if (newItem.Length > 0)
            {
                list2.Add(newItem);
                newItem = "";
            }
            return list2;
        Label_00F2:
            if (flag4 && !(flag2 | flag3))
            {
                newItem = newItem + Conversions.ToString(ch2);
            }
            goto Label_000E;
        }

        public static clsZipStreamEntry FindZipEntryFromPath(string Path, string ZipPathToFind)
        {
            string str = ZipPathToFind.ToLower().Replace('\\', '/');
            ZipInputStream stream = new ZipInputStream(File.OpenRead(Path));
            while (true)
            {
                ZipEntry nextEntry;
                try
                {
                    nextEntry = stream.GetNextEntry();
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    ProjectData.ClearProjectError();
                    break;
                }
                if (nextEntry == null)
                {
                    break;
                }
                if (nextEntry.Name.ToLower().Replace('\\', '/') == str)
                {
                    return new clsZipStreamEntry { Stream = stream, Entry = nextEntry };
                }
            }
            stream.Close();
            return null;
        }

        public static bool HealthFromINIText(string Text, ref int Result)
        {
            int num2;
            if (Text.IndexOf('%') < 0)
            {
                return false;
            }
            Text = Text.Replace("%", "");
            if (!InvariantParse_int(Text, ref num2))
            {
                return false;
            }
            if ((num2 < 0) | (num2 > 100))
            {
                return false;
            }
            Result = num2;
            return true;
        }

        public static bool InvariantParse_bool(string Text, ref bool Result)
        {
            return bool.TryParse(Text, out Result);
        }

        public static bool InvariantParse_byte(string Text, ref byte Result)
        {
            return byte.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_dbl(string Text, ref double Result)
        {
            return double.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_int(string Text, ref int Result)
        {
            return int.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_short(string Text, ref short Result)
        {
            return short.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_sng(string Text, ref float Result)
        {
            return float.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_uint(string Text, ref uint Result)
        {
            return uint.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static bool InvariantParse_ushort(string Text, ref ushort Result)
        {
            return ushort.TryParse(Text, NumberStyles.Any, CultureInfo.InvariantCulture, out Result);
        }

        public static string InvariantToString_bool(bool Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_byte(byte Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_dbl(double Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_int(int Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_short(short Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_sng(float Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string InvariantToString_uint(uint Value)
        {
            return Value.ToString(CultureInfo.InvariantCulture);
        }

        public static string ReadOldText(BinaryReader File)
        {
            string str2 = "";
            int num2 = (int) File.ReadUInt32();
            int num3 = num2 - 1;
            for (int i = 0; i <= num3; i++)
            {
                str2 = str2 + Conversions.ToString(Strings.Chr(File.ReadByte()));
            }
            return str2;
        }

        public static string ReadOldTextOfLength(BinaryReader File, int Length)
        {
            string str2 = "";
            int num2 = Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                str2 = str2 + Conversions.ToString(Strings.Chr(File.ReadByte()));
            }
            return str2;
        }

        public static modProgram.sResult TryOpenFileStream(string Path, ref FileStream Output)
        {
            modProgram.sResult result;
            result.Success = false;
            result.Problem = "";
            try
            {
                Output = new FileStream(Path, FileMode.Open);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                Output = null;
                result.Problem = exception.Message;
                modProgram.sResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            result.Success = true;
            return result;
        }

        public static bool WorldPosFromINIText(string Text, ref modProgram.clsWorldPos Result)
        {
            clsSplitCommaText text = new clsSplitCommaText(Text);
            if (text.PartCount != 3)
            {
                return false;
            }
            int[] numArray = new int[3];
            int index = 0;
            do
            {
                int num2;
                if (InvariantParse_int(text.Parts[index], ref num2))
                {
                    numArray[index] = num2;
                }
                else
                {
                    return false;
                }
                index++;
            }
            while (index <= 2);
            modMath.sXY_int newHorizontal = new modMath.sXY_int(numArray[0], numArray[1]);
            modProgram.sWorldPos newWorldPos = new modProgram.sWorldPos(newHorizontal, numArray[2]);
            Result = new modProgram.clsWorldPos(newWorldPos);
            return true;
        }

        public static clsResult WriteMemoryToNewFile(MemoryStream Memory, string Path)
        {
            FileStream stream;
            clsResult result2;
            clsResult result = new clsResult("Writing to \"" + Path + "\"");
            try
            {
                stream = new FileStream(Path, FileMode.CreateNew);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            try
            {
                Memory.WriteTo(stream);
            }
            catch (Exception exception3)
            {
                ProjectData.SetProjectError(exception3);
                Exception exception2 = exception3;
                result.ProblemAdd(exception2.Message);
                result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            Memory.Close();
            stream.Close();
            return result;
        }

        public static clsResult WriteMemoryToZipEntryAndFlush(MemoryStream Memory, ZipOutputStream Stream)
        {
            clsResult result = new clsResult("Writing to zip stream");
            try
            {
                Memory.WriteTo(Stream);
                Memory.Flush();
                Stream.Flush();
                Stream.CloseEntry();
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                result.ProblemAdd(exception.Message);
                clsResult result2 = result;
                ProjectData.ClearProjectError();
                return result2;
            }
            return result;
        }

        public static void WriteText(BinaryWriter File, bool WriteLength, string Text)
        {
            if (WriteLength)
            {
                File.Write((uint) Text.Length);
            }
            int num2 = Text.Length - 1;
            for (int i = 0; i <= num2; i++)
            {
                File.Write((byte) Strings.Asc(Text[i]));
            }
        }

        public static void WriteTextOfLength(BinaryWriter File, int Length, string Text)
        {
            int num;
            int num2 = Math.Min(Text.Length, Length) - 1;
            for (num = 0; num <= num2; num++)
            {
                File.Write((byte) Strings.Asc(Text[num]));
            }
            int num3 = Length - 1;
            for (num = Text.Length; num <= num3; num++)
            {
                File.Write((byte) 0);
            }
        }

        public static bool WZAngleFromINIText(string Text, ref modProgram.sWZAngle Result)
        {
            modProgram.sWZAngle angle;
            clsSplitCommaText text = new clsSplitCommaText(Text);
            if (text.PartCount != 3)
            {
                return false;
            }
            if (!InvariantParse_ushort(text.Parts[0], ref angle.Direction))
            {
                int num;
                int remaidner;
                if (!InvariantParse_int(text.Parts[0], ref num))
                {
                    return false;
                }
                int mulitplaier = Math.DivRem(num, 0x10000, out remaidner);
                try
                {
                    if (remaidner < 0)
                    {
                        angle.Direction = (ushort) (remaidner + 0x10000);
                    }
                    else
                    {
                        angle.Direction = (ushort) remaidner;
                    }
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    ProjectData.ClearProjectError();
                    return false;
                }
                return true;
            }
            if (!InvariantParse_ushort(text.Parts[1], ref angle.Pitch))
            {
                return false;
            }
            if (!InvariantParse_ushort(text.Parts[2], ref angle.Roll))
            {
                return false;
            }
            Result = angle;
            return true;
        }

        public static ZipEntry ZipMakeEntry(ZipOutputStream ZipOutputStream, string Path, clsResult Result)
        {
            ZipEntry entry;
            try
            {
                ZipEntry entry2 = new ZipEntry(Path) {
                    DateTime = DateAndTime.Now
                };
                ZipOutputStream.PutNextEntry(entry2);
                entry = entry2;
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                Exception exception = exception1;
                Result.ProblemAdd("Zip entry \"" + Path + "\" failed: " + exception.Message);
                entry = null;
                ProjectData.ClearProjectError();
            }
            return entry;
        }
    }
}

