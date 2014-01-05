namespace FlaME
{
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections;
    using System.Drawing;
    using System.IO;
    using System.Runtime.InteropServices;

    public class clsObjectData
    {
        public modLists.ConnectedList<clsBody, clsObjectData> Bodies;
        public modLists.ConnectedList<clsBrain, clsObjectData> Brains;
        public modLists.ConnectedList<clsConstruct, clsObjectData> Constructors;
        public modLists.ConnectedList<clsDroidTemplate, clsObjectData> DroidTemplates;
        public modLists.ConnectedList<clsECM, clsObjectData> ECMs;
        public modLists.ConnectedList<clsFeatureType, clsObjectData> FeatureTypes;
        public modLists.ConnectedList<clsPropulsion, clsObjectData> Propulsions;
        public modLists.ConnectedList<clsRepair, clsObjectData> Repairs;
        public modLists.ConnectedList<clsSensor, clsObjectData> Sensors;
        public modLists.ConnectedList<clsStructureType, clsObjectData> StructureTypes;
        public modLists.SimpleList<clsTexturePage> TexturePages;
        public modLists.ConnectedList<clsTurret, clsObjectData> Turrets;
        public modLists.ConnectedList<clsUnitType, clsObjectData> UnitTypes;
        public modLists.ConnectedList<clsWallType, clsObjectData> WallTypes;
        public modLists.ConnectedList<clsWeapon, clsObjectData> Weapons;

        public clsObjectData()
        {
            this.UnitTypes = new modLists.ConnectedList<clsUnitType, clsObjectData>(this);
            this.FeatureTypes = new modLists.ConnectedList<clsFeatureType, clsObjectData>(this);
            this.StructureTypes = new modLists.ConnectedList<clsStructureType, clsObjectData>(this);
            this.DroidTemplates = new modLists.ConnectedList<clsDroidTemplate, clsObjectData>(this);
            this.WallTypes = new modLists.ConnectedList<clsWallType, clsObjectData>(this);
            this.Bodies = new modLists.ConnectedList<clsBody, clsObjectData>(this);
            this.Propulsions = new modLists.ConnectedList<clsPropulsion, clsObjectData>(this);
            this.Turrets = new modLists.ConnectedList<clsTurret, clsObjectData>(this);
            this.Weapons = new modLists.ConnectedList<clsWeapon, clsObjectData>(this);
            this.Sensors = new modLists.ConnectedList<clsSensor, clsObjectData>(this);
            this.Repairs = new modLists.ConnectedList<clsRepair, clsObjectData>(this);
            this.Constructors = new modLists.ConnectedList<clsConstruct, clsObjectData>(this);
            this.Brains = new modLists.ConnectedList<clsBrain, clsObjectData>(this);
            this.ECMs = new modLists.ConnectedList<clsECM, clsObjectData>(this);
            this.TexturePages = new modLists.SimpleList<clsTexturePage>();
        }

        public clsBody FindBodyCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Bodies.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsBody current = (clsBody) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsBrain FindBrainCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Brains.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsBrain current = (clsBrain) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsConstruct FindConstructorCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Constructors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsConstruct current = (clsConstruct) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsECM FindECMCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.ECMs.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsECM current = (clsECM) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsStructureType FindFirstStructureType(clsStructureType.enumStructureType Type)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.StructureTypes.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsStructureType current = (clsStructureType) enumerator.Current;
                    if (current.StructureType == Type)
                    {
                        return current;
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
            return null;
        }

        public clsBody FindOrCreateBody(string Code)
        {
            clsBody body2 = this.FindBodyCode(Code);
            if (body2 == null)
            {
                body2 = new clsBody {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return body2;
        }

        public clsBrain FindOrCreateBrain(string Code)
        {
            clsBrain brain2 = this.FindBrainCode(Code);
            if (brain2 == null)
            {
                brain2 = new clsBrain {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return brain2;
        }

        public clsConstruct FindOrCreateConstruct(string Code)
        {
            clsConstruct construct2 = this.FindConstructorCode(Code);
            if (construct2 == null)
            {
                construct2 = new clsConstruct {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return construct2;
        }

        public clsECM FindOrCreateECM(string Code)
        {
            clsECM secm2 = this.FindECMCode(Code);
            if (secm2 == null)
            {
                secm2 = new clsECM {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return secm2;
        }

        public clsPropulsion FindOrCreatePropulsion(string Code)
        {
            clsPropulsion propulsion2 = this.FindPropulsionCode(Code);
            if (propulsion2 == null)
            {
                propulsion2 = new clsPropulsion(this.Bodies.Count) {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return propulsion2;
        }

        public clsRepair FindOrCreateRepair(string Code)
        {
            clsRepair repair2 = this.FindRepairCode(Code);
            if (repair2 == null)
            {
                repair2 = new clsRepair {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return repair2;
        }

        public clsSensor FindOrCreateSensor(string Code)
        {
            clsSensor sensor2 = this.FindSensorCode(Code);
            if (sensor2 == null)
            {
                sensor2 = new clsSensor {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return sensor2;
        }

        public clsTurret FindOrCreateTurret(clsTurret.enumTurretType TurretType, string TurretCode)
        {
            switch (TurretType)
            {
                case clsTurret.enumTurretType.Weapon:
                    return this.FindOrCreateWeapon(TurretCode);

                case clsTurret.enumTurretType.Construct:
                    return this.FindOrCreateConstruct(TurretCode);

                case clsTurret.enumTurretType.Repair:
                    return this.FindOrCreateRepair(TurretCode);

                case clsTurret.enumTurretType.Sensor:
                    return this.FindOrCreateSensor(TurretCode);

                case clsTurret.enumTurretType.Brain:
                    return this.FindOrCreateBrain(TurretCode);

                case clsTurret.enumTurretType.ECM:
                    return this.FindOrCreateECM(TurretCode);
            }
            return null;
        }

        public clsUnitType FindOrCreateUnitType(string Code, clsUnitType.enumType Type, int WallType)
        {
            switch (Type)
            {
                case clsUnitType.enumType.Feature:
                    clsFeatureType current;
                    IEnumerator enumerator;
                    try
                    {
                        enumerator = this.FeatureTypes.GetEnumerator();
                        while (enumerator.MoveNext())
                        {
                            current = (clsFeatureType) enumerator.Current;
                            if (current.Code == Code)
                            {
                                return current;
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
                    current = new clsFeatureType {
                        IsUnknown = true,
                        Code = Code
                    };
                    current.Footprint.X = 1;
                    current.Footprint.Y = 1;
                    return current;

                case clsUnitType.enumType.PlayerStructure:
                    clsStructureType type3;
                    IEnumerator enumerator2;
                    try
                    {
                        enumerator2 = this.StructureTypes.GetEnumerator();
                        while (enumerator2.MoveNext())
                        {
                            type3 = (clsStructureType) enumerator2.Current;
                            if (type3.Code == Code)
                            {
                                if (WallType < 0)
                                {
                                    return type3;
                                }
                                if (type3.WallLink.IsConnected && (type3.WallLink.ArrayPosition == WallType))
                                {
                                    return type3;
                                }
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator2 is IDisposable)
                        {
                            (enumerator2 as IDisposable).Dispose();
                        }
                    }
                    type3 = new clsStructureType {
                        IsUnknown = true,
                        Code = Code
                    };
                    type3.Footprint.X = 1;
                    type3.Footprint.Y = 1;
                    return type3;

                case clsUnitType.enumType.PlayerDroid:
                    IEnumerator enumerator3;
                    try
                    {
                        enumerator3 = this.DroidTemplates.GetEnumerator();
                        while (enumerator3.MoveNext())
                        {
                            clsDroidTemplate template = (clsDroidTemplate) enumerator3.Current;
                            if (template.IsTemplate && (template.Code == Code))
                            {
                                return template;
                            }
                        }
                    }
                    finally
                    {
                        if (enumerator3 is IDisposable)
                        {
                            (enumerator3 as IDisposable).Dispose();
                        }
                    }
                    return new clsDroidTemplate { IsUnknown = true, Code = Code };
            }
            return null;
        }

        public clsWeapon FindOrCreateWeapon(string Code)
        {
            clsWeapon weapon2 = this.FindWeaponCode(Code);
            if (weapon2 == null)
            {
                weapon2 = new clsWeapon {
                    IsUnknown = true,
                    Code = Code
                };
            }
            return weapon2;
        }

        public clsPropulsion FindPropulsionCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Propulsions.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsPropulsion current = (clsPropulsion) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsRepair FindRepairCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Repairs.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsRepair current = (clsRepair) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsSensor FindSensorCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Sensors.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsSensor current = (clsSensor) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public clsWeapon FindWeaponCode(string Code)
        {
            IEnumerator enumerator;
            try
            {
                enumerator = this.Weapons.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsWeapon current = (clsWeapon) enumerator.Current;
                    if (current.Code == Code)
                    {
                        return current;
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
            return null;
        }

        public int Get_TexturePage_GLTexture(string FileTitle)
        {
            IEnumerator enumerator;
            string str = FileTitle.ToLower();
            try
            {
                enumerator = this.TexturePages.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsTexturePage current = (clsTexturePage) enumerator.Current;
                    if (current.FileTitle.ToLower() == str)
                    {
                        return current.GLTexture_Num;
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
            return 0;
        }

        public clsModel GetModelForPIE(modLists.SimpleList<clsPIE> PIE_List, string PIE_LCaseFileTitle, clsResult ResultOutput)
        {
            if (PIE_LCaseFileTitle != "0")
            {
                clsResult resultToAdd = new clsResult("Loading PIE file " + PIE_LCaseFileTitle);
                int num2 = PIE_List.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    clsPIE spie = PIE_List[i];
                    if (spie.LCaseFileTitle == PIE_LCaseFileTitle)
                    {
                        if (spie.Model == null)
                        {
                            spie.Model = new clsModel();
                            try
                            {
                                StreamReader file = new StreamReader(spie.Path);
                                try
                                {
                                    resultToAdd.Take(spie.Model.ReadPIE(file, this));
                                }
                                catch (Exception exception1)
                                {
                                    ProjectData.SetProjectError(exception1);
                                    Exception exception = exception1;
                                    file.Close();
                                    resultToAdd.WarningAdd(exception.Message);
                                    ResultOutput.Add(resultToAdd);
                                    clsModel model = spie.Model;
                                    ProjectData.ClearProjectError();
                                    return model;
                                }
                            }
                            catch (Exception exception3)
                            {
                                ProjectData.SetProjectError(exception3);
                                Exception exception2 = exception3;
                                resultToAdd.WarningAdd(exception2.Message);
                                ProjectData.ClearProjectError();
                            }
                        }
                        ResultOutput.Add(resultToAdd);
                        return spie.Model;
                    }
                }
                if (!resultToAdd.HasWarnings)
                {
                    resultToAdd.WarningAdd("file is missing");
                }
                ResultOutput.Add(resultToAdd);
            }
            return null;
        }

        public modLists.SimpleList<string[]> GetRowsWithValue(modLists.SimpleList<string[]> TextLines, string Value)
        {
            IEnumerator enumerator;
            modLists.SimpleList<string[]> list2 = new modLists.SimpleList<string[]>();
            try
            {
                enumerator = TextLines.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    string[] current = (string[]) enumerator.Current;
                    if (current[0] == Value)
                    {
                        list2.Add(current);
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
            return list2;
        }

        public clsResult LoadDirectory(string Path)
        {
            IEnumerator enumerator;
            clsResult result3 = new clsResult("Loading object data from \"" + Path + "\"");
            Path = modProgram.EndWithPathSeperator(Path);
            string str10 = "messages" + Conversions.ToString(modProgram.PlatformPathSeparator) + "strings" + Conversions.ToString(modProgram.PlatformPathSeparator) + "names.txt";
            string str17 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "structures.txt";
            string str5 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "brain.txt";
            string str3 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "body.txt";
            string str13 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "propulsion.txt";
            string str4 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "bodypropulsionimd.txt";
            string str6 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "construction.txt";
            string str15 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "sensor.txt";
            string str14 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "repair.txt";
            string str19 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "templates.txt";
            string str21 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "weapons.txt";
            string str7 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "ecm.txt";
            string str9 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "features.txt";
            string str11 = "pies" + Conversions.ToString(modProgram.PlatformPathSeparator);
            string str16 = str11;
            string str2 = str11;
            string str12 = str11;
            string str22 = str11;
            string str20 = "texpages" + Conversions.ToString(modProgram.PlatformPathSeparator);
            string str = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "assignweapons.txt";
            string str8 = str11;
            string str18 = "stats" + Conversions.ToString(modProgram.PlatformPathSeparator) + "structureweapons.txt";
            modLists.SimpleList<clsTextFile> list = new modLists.SimpleList<clsTextFile>();
            clsTextFile file8 = new clsTextFile {
                SubDirectory = str10,
                UniqueField = 0
            };
            result3.Add(file8.LoadNamesFile(Path));
            if (!file8.CalcUniqueField())
            {
                result3.ProblemAdd("There are two entries for the same code in " + str10 + ".");
            }
            clsTextFile newItem = new clsTextFile {
                SubDirectory = str17,
                FieldCount = 0x19
            };
            list.Add(newItem);
            clsTextFile file4 = new clsTextFile {
                SubDirectory = str5,
                FieldCount = 9
            };
            list.Add(file4);
            clsTextFile file2 = new clsTextFile {
                SubDirectory = str3,
                FieldCount = 0x19
            };
            list.Add(file2);
            clsTextFile file9 = new clsTextFile {
                SubDirectory = str13,
                FieldCount = 12
            };
            list.Add(file9);
            clsTextFile file3 = new clsTextFile {
                SubDirectory = str4,
                FieldCount = 5,
                UniqueField = -1
            };
            list.Add(file3);
            clsTextFile file5 = new clsTextFile {
                SubDirectory = str6,
                FieldCount = 12
            };
            list.Add(file5);
            clsTextFile file11 = new clsTextFile {
                SubDirectory = str15,
                FieldCount = 0x10
            };
            list.Add(file11);
            clsTextFile file10 = new clsTextFile {
                SubDirectory = str14,
                FieldCount = 14
            };
            list.Add(file10);
            clsTextFile file14 = new clsTextFile {
                SubDirectory = str19,
                FieldCount = 12
            };
            list.Add(file14);
            clsTextFile file6 = new clsTextFile {
                SubDirectory = str7,
                FieldCount = 14
            };
            list.Add(file6);
            clsTextFile file7 = new clsTextFile {
                SubDirectory = str9,
                FieldCount = 11
            };
            list.Add(file7);
            clsTextFile file = new clsTextFile {
                SubDirectory = str,
                FieldCount = 5
            };
            list.Add(file);
            clsTextFile file15 = new clsTextFile {
                SubDirectory = str21,
                FieldCount = 0x35
            };
            list.Add(file15);
            clsTextFile file13 = new clsTextFile {
                SubDirectory = str18,
                FieldCount = 6
            };
            list.Add(file13);
            try
            {
                enumerator = list.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsTextFile current = (clsTextFile) enumerator.Current;
                    clsResult resultToAdd = current.LoadCommaFile(Path);
                    result3.Add(resultToAdd);
                    if (!resultToAdd.HasProblems)
                    {
                        if (current.CalcIsFieldCountValid())
                        {
                            if (!current.CalcUniqueField())
                            {
                                result3.ProblemAdd("An entry in field " + Conversions.ToString(current.UniqueField) + " was not unique for file " + current.SubDirectory + ".");
                            }
                        }
                        else
                        {
                            result3.ProblemAdd("There were entries with the wrong number of fields for file " + current.SubDirectory + ".");
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
            if (!result3.HasProblems)
            {
                clsUnitType.clsAttachment baseAttachment;
                clsBody body;
                clsECM secm;
                string[] strArray;
                string[] strArray2;
                clsPropulsion propulsion;
                clsSensor sensor;
                string[] files;
                clsWeapon weapon;
                IEnumerator enumerator2;
                IEnumerator enumerator3;
                IEnumerator enumerator4;
                IEnumerator enumerator5;
                IEnumerator enumerator6;
                IEnumerator enumerator7;
                IEnumerator enumerator8;
                IEnumerator enumerator9;
                IEnumerator enumerator10;
                IEnumerator enumerator11;
                IEnumerator enumerator12;
                IEnumerator enumerator13;
                try
                {
                    files = Directory.GetFiles(Path + str20);
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    result3.WarningAdd("Unable to access texture pages.");
                    files = new string[0];
                    ProjectData.ClearProjectError();
                }
                System.Drawing.Bitmap resultBitmap = null;
                foreach (string str23 in files)
                {
                    if (Strings.Right(str23, 4).ToLower() == ".png")
                    {
                        clsResult result5 = new clsResult("Loading texture page \"" + str23 + "\"");
                        if (File.Exists(str23))
                        {
                            modProgram.sResult result = modBitmap.LoadBitmap(str23, ref resultBitmap);
                            clsTexturePage page = new clsTexturePage();
                            if (result.Success)
                            {
                                modBitmap.sBitmapGLTexture texture;
                                result5.Take(modBitmap.BitmapIsGLCompatible(resultBitmap));
                                texture.MagFilter = TextureMagFilter.Nearest;
                                texture.MinFilter = TextureMinFilter.Nearest;
                                texture.TextureNum = 0;
                                texture.MipMapLevel = 0;
                                texture.Texture = resultBitmap;
                                texture.Perform();
                                page.GLTexture_Num = texture.TextureNum;
                            }
                            else
                            {
                                result5.WarningAdd(result.Problem);
                            }
                            int num = Strings.InStrRev(str23, Conversions.ToString(modProgram.PlatformPathSeparator), -1, CompareMethod.Binary);
                            page.FileTitle = Strings.Mid(str23, num + 1, (str23.Length - 4) - num);
                            this.TexturePages.Add(page);
                        }
                        else
                        {
                            result5.WarningAdd("Texture page missing (" + str23 + ").");
                        }
                        result3.Add(result5);
                    }
                }
                modLists.SimpleList<clsPIE> list2 = new modLists.SimpleList<clsPIE>();
                try
                {
                    strArray2 = Directory.GetFiles(Path + str11);
                }
                catch (Exception exception3)
                {
                    ProjectData.SetProjectError(exception3);
                    Exception exception2 = exception3;
                    result3.WarningAdd("Unable to access PIE files.");
                    strArray2 = new string[0];
                    ProjectData.ClearProjectError();
                }
                foreach (string str23 in strArray2)
                {
                    modProgram.sSplitPath path = new modProgram.sSplitPath(str23);
                    if (path.FileExtension.ToLower() == "pie")
                    {
                        clsPIE spie = new clsPIE {
                            Path = str23,
                            LCaseFileTitle = path.FileTitle.ToLower()
                        };
                        list2.Add(spie);
                    }
                }
                try
                {
                    enumerator2 = file2.ResultData.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        strArray = (string[]) enumerator2.Current;
                        body = new clsBody();
                        body.ObjectDataLink.Connect(this.Bodies);
                        body.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, body, result3);
                        modIO.InvariantParse_int(strArray[6], ref body.Hitpoints);
                        body.Designable = strArray[0x18] != "0";
                        body.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[7].ToLower(), result3));
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator3 = file9.ResultData.GetEnumerator();
                    while (enumerator3.MoveNext())
                    {
                        strArray = (string[]) enumerator3.Current;
                        propulsion = new clsPropulsion(this.Bodies.Count);
                        propulsion.ObjectDataLink.Connect(this.Propulsions);
                        propulsion.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, propulsion, result3);
                        modIO.InvariantParse_int(strArray[7], ref propulsion.HitPoints);
                        propulsion.Designable = strArray[11] != "0";
                    }
                }
                finally
                {
                    if (enumerator3 is IDisposable)
                    {
                        (enumerator3 as IDisposable).Dispose();
                    }
                }
                BodyProp[,] propArray = new BodyProp[(this.Bodies.Count - 1) + 1, (this.Propulsions.Count - 1) + 1];
                int num10 = this.Bodies.Count - 1;
                for (int i = 0; i <= num10; i++)
                {
                    int num11 = this.Propulsions.Count - 1;
                    for (int k = 0; k <= num11; k++)
                    {
                        propArray[i, k] = new BodyProp();
                        propArray[i, k].LeftPIE = "0";
                        propArray[i, k].RightPIE = "0";
                    }
                }
                try
                {
                    enumerator4 = file3.ResultData.GetEnumerator();
                    while (enumerator4.MoveNext())
                    {
                        strArray = (string[]) enumerator4.Current;
                        body = this.FindBodyCode(strArray[0]);
                        propulsion = this.FindPropulsionCode(strArray[1]);
                        if ((body != null) & (propulsion != null))
                        {
                            if (strArray[2] != "0")
                            {
                                propArray[body.ObjectDataLink.ArrayPosition, propulsion.ObjectDataLink.ArrayPosition].LeftPIE = strArray[2].ToLower();
                            }
                            if (strArray[3] != "0")
                            {
                                propArray[body.ObjectDataLink.ArrayPosition, propulsion.ObjectDataLink.ArrayPosition].RightPIE = strArray[3].ToLower();
                            }
                        }
                    }
                }
                finally
                {
                    if (enumerator4 is IDisposable)
                    {
                        (enumerator4 as IDisposable).Dispose();
                    }
                }
                int num12 = this.Propulsions.Count - 1;
                for (int j = 0; j <= num12; j++)
                {
                    propulsion = this.Propulsions[j];
                    int num13 = this.Bodies.Count - 1;
                    for (int m = 0; m <= num13; m++)
                    {
                        body = this.Bodies[m];
                        propulsion.Bodies[m].LeftAttachment = new clsUnitType.clsAttachment();
                        propulsion.Bodies[m].LeftAttachment.Models.Add(this.GetModelForPIE(list2, propArray[m, j].LeftPIE, result3));
                        propulsion.Bodies[m].RightAttachment = new clsUnitType.clsAttachment();
                        propulsion.Bodies[m].RightAttachment.Models.Add(this.GetModelForPIE(list2, propArray[m, j].RightPIE, result3));
                    }
                }
                try
                {
                    enumerator5 = file5.ResultData.GetEnumerator();
                    while (enumerator5.MoveNext())
                    {
                        strArray = (string[]) enumerator5.Current;
                        clsConstruct component = new clsConstruct();
                        component.ObjectDataLink.Connect(this.Constructors);
                        component.TurretObjectDataLink.Connect(this.Turrets);
                        component.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, component, result3);
                        component.Designable = strArray[11] != "0";
                        component.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[8].ToLower(), result3));
                    }
                }
                finally
                {
                    if (enumerator5 is IDisposable)
                    {
                        (enumerator5 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator6 = file15.ResultData.GetEnumerator();
                    while (enumerator6.MoveNext())
                    {
                        strArray = (string[]) enumerator6.Current;
                        weapon = new clsWeapon();
                        weapon.ObjectDataLink.Connect(this.Weapons);
                        weapon.TurretObjectDataLink.Connect(this.Turrets);
                        weapon.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, weapon, result3);
                        modIO.InvariantParse_int(strArray[7], ref weapon.HitPoints);
                        weapon.Designable = strArray[0x33] != "0";
                        weapon.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[8].ToLower(), result3));
                        weapon.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[9].ToLower(), result3));
                    }
                }
                finally
                {
                    if (enumerator6 is IDisposable)
                    {
                        (enumerator6 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator7 = file11.ResultData.GetEnumerator();
                    while (enumerator7.MoveNext())
                    {
                        strArray = (string[]) enumerator7.Current;
                        sensor = new clsSensor();
                        sensor.ObjectDataLink.Connect(this.Sensors);
                        sensor.TurretObjectDataLink.Connect(this.Turrets);
                        sensor.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, sensor, result3);
                        modIO.InvariantParse_int(strArray[7], ref sensor.HitPoints);
                        sensor.Designable = strArray[15] != "0";
                        string str27 = strArray[11].ToLower();
                        if (str27 == "turret")
                        {
                            sensor.Location = clsSensor.enumLocation.Turret;
                        }
                        else if (str27 == "default")
                        {
                            sensor.Location = clsSensor.enumLocation.Invisible;
                        }
                        else
                        {
                            sensor.Location = clsSensor.enumLocation.Invisible;
                        }
                        sensor.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[8].ToLower(), result3));
                        sensor.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[9].ToLower(), result3));
                    }
                }
                finally
                {
                    if (enumerator7 is IDisposable)
                    {
                        (enumerator7 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator8 = file10.ResultData.GetEnumerator();
                    while (enumerator8.MoveNext())
                    {
                        strArray = (string[]) enumerator8.Current;
                        clsRepair repair = new clsRepair();
                        repair.ObjectDataLink.Connect(this.Repairs);
                        repair.TurretObjectDataLink.Connect(this.Turrets);
                        repair.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, repair, result3);
                        repair.Designable = strArray[13] != "0";
                        repair.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[9].ToLower(), result3));
                        repair.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[10].ToLower(), result3));
                    }
                }
                finally
                {
                    if (enumerator8 is IDisposable)
                    {
                        (enumerator8 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator9 = file4.ResultData.GetEnumerator();
                    while (enumerator9.MoveNext())
                    {
                        strArray = (string[]) enumerator9.Current;
                        clsBrain brain = new clsBrain();
                        brain.ObjectDataLink.Connect(this.Brains);
                        brain.TurretObjectDataLink.Connect(this.Turrets);
                        brain.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, brain, result3);
                        brain.Designable = true;
                        weapon = this.FindWeaponCode(strArray[7]);
                        if (weapon != null)
                        {
                            brain.Weapon = weapon;
                            brain.Attachment = weapon.Attachment;
                        }
                    }
                }
                finally
                {
                    if (enumerator9 is IDisposable)
                    {
                        (enumerator9 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator10 = file6.ResultData.GetEnumerator();
                    while (enumerator10.MoveNext())
                    {
                        strArray = (string[]) enumerator10.Current;
                        secm = new clsECM();
                        secm.ObjectDataLink.Connect(this.ECMs);
                        secm.TurretObjectDataLink.Connect(this.Turrets);
                        secm.Code = strArray[0];
                        this.SetComponentName(file8.ResultData, secm, result3);
                        modIO.InvariantParse_int(strArray[7], ref secm.HitPoints);
                        secm.Designable = false;
                        secm.Attachment.Models.Add(this.GetModelForPIE(list2, strArray[8].ToLower(), result3));
                    }
                }
                finally
                {
                    if (enumerator10 is IDisposable)
                    {
                        (enumerator10 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator11 = file7.ResultData.GetEnumerator();
                    while (enumerator11.MoveNext())
                    {
                        strArray = (string[]) enumerator11.Current;
                        clsFeatureType featureType = new clsFeatureType();
                        featureType.UnitType_ObjectDataLink.Connect(this.UnitTypes);
                        featureType.FeatureType_ObjectDataLink.Connect(this.FeatureTypes);
                        featureType.Code = strArray[0];
                        if (strArray[7] == "OIL RESOURCE")
                        {
                            featureType.FeatureType = clsFeatureType.enumFeatureType.OilResource;
                        }
                        this.SetFeatureName(file8.ResultData, featureType, result3);
                        if (!modIO.InvariantParse_int(strArray[1], ref featureType.Footprint.X))
                        {
                            result3.WarningAdd("Feature footprint-x was not an integer for " + featureType.Code + ".");
                        }
                        if (!modIO.InvariantParse_int(strArray[2], ref featureType.Footprint.Y))
                        {
                            result3.WarningAdd("Feature footprint-y was not an integer for " + featureType.Code + ".");
                        }
                        featureType.BaseAttachment = new clsUnitType.clsAttachment();
                        baseAttachment = featureType.BaseAttachment;
                        name = strArray[6].ToLower();
                        baseAttachment.CreateAttachment().Models.Add(this.GetModelForPIE(list2, name, result3));
                    }
                }
                finally
                {
                    if (enumerator11 is IDisposable)
                    {
                        (enumerator11 as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator12 = newItem.ResultData.GetEnumerator();
                    while (enumerator12.MoveNext())
                    {
                        modMath.sXY_int _int;
                        strArray = (string[]) enumerator12.Current;
                        string str25 = strArray[0];
                        string str26 = strArray[1];
                        string[] strArray4 = strArray[0x15].ToLower().Split(new char[] { '@' });
                        string str24 = strArray[0x16].ToLower();
                        if (!modIO.InvariantParse_int(strArray[5], ref _int.X))
                        {
                            result3.WarningAdd("Structure footprint-x was not an integer for " + str25 + ".");
                        }
                        if (!modIO.InvariantParse_int(strArray[6], ref _int.Y))
                        {
                            result3.WarningAdd("Structure footprint-y was not an integer for " + str25 + ".");
                        }
                        if ((str26 != "WALL") | (strArray4.GetLength(0) != 4))
                        {
                            clsStructureType structureType = new clsStructureType();
                            structureType.UnitType_ObjectDataLink.Connect(this.UnitTypes);
                            structureType.StructureType_ObjectDataLink.Connect(this.StructureTypes);
                            structureType.Code = str25;
                            this.SetStructureName(file8.ResultData, structureType, result3);
                            structureType.Footprint = _int;
                            string str28 = str26;
                            if (str28 == "DEMOLISH")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Demolish;
                            }
                            else if (str28 == "WALL")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Wall;
                            }
                            else if (str28 == "CORNER WALL")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.CornerWall;
                            }
                            else if (str28 == "FACTORY")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Factory;
                            }
                            else if (str28 == "CYBORG FACTORY")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.CyborgFactory;
                            }
                            else if (str28 == "VTOL FACTORY")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.VTOLFactory;
                            }
                            else if (str28 == "COMMAND")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Command;
                            }
                            else if (str28 == "HQ")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.HQ;
                            }
                            else if (str28 == "DEFENSE")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Defense;
                            }
                            else if (str28 == "POWER GENERATOR")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.PowerGenerator;
                            }
                            else if (str28 == "POWER MODULE")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.PowerModule;
                            }
                            else if (str28 == "RESEARCH")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Research;
                            }
                            else if (str28 == "RESEARCH MODULE")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.ResearchModule;
                            }
                            else if (str28 == "FACTORY MODULE")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.FactoryModule;
                            }
                            else if (str28 == "DOOR")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.DOOR;
                            }
                            else if (str28 == "REPAIR FACILITY")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.RepairFacility;
                            }
                            else if (str28 == "SAT UPLINK")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.DOOR;
                            }
                            else if (str28 == "REARM PAD")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.RearmPad;
                            }
                            else if (str28 == "MISSILE SILO")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.MissileSilo;
                            }
                            else if (str28 == "RESOURCE EXTRACTOR")
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.ResourceExtractor;
                            }
                            else
                            {
                                structureType.StructureType = clsStructureType.enumStructureType.Unknown;
                            }
                            baseAttachment = structureType.BaseAttachment;
                            if (strArray4.GetLength(0) > 0)
                            {
                                baseAttachment.Models.Add(this.GetModelForPIE(list2, strArray4[0], result3));
                            }
                            structureType.StructureBasePlate = this.GetModelForPIE(list2, str24, result3);
                            if ((baseAttachment.Models.Count == 1) && (baseAttachment.Models[0].ConnectorCount >= 1))
                            {
                                modMath.sXYZ_sng _sng = baseAttachment.Models[0].Connectors[0];
                                modLists.SimpleList<string[]> rowsWithValue = this.GetRowsWithValue(file13.ResultData, structureType.Code);
                                if (rowsWithValue.Count > 0)
                                {
                                    weapon = this.FindWeaponCode(rowsWithValue[0][1]);
                                }
                                else
                                {
                                    weapon = null;
                                }
                                secm = this.FindECMCode(strArray[0x12]);
                                sensor = this.FindSensorCode(strArray[0x13]);
                                if ((weapon != null) && (weapon.Code != "ZNULLWEAPON"))
                                {
                                    baseAttachment.CopyAttachment(weapon.Attachment).Pos_Offset = _sng;
                                }
                                if ((secm != null) && (secm.Code != "ZNULLECM"))
                                {
                                    baseAttachment.CopyAttachment(secm.Attachment).Pos_Offset = _sng;
                                }
                                if ((sensor != null) && (sensor.Code != "ZNULLSENSOR"))
                                {
                                    baseAttachment.CopyAttachment(sensor.Attachment).Pos_Offset = _sng;
                                }
                            }
                            continue;
                        }
                        clsWallType wallType = new clsWallType();
                        wallType.WallType_ObjectDataLink.Connect(this.WallTypes);
                        wallType.Code = str25;
                        this.SetWallName(file8.ResultData, wallType, result3);
                        clsModel model = this.GetModelForPIE(list2, str24, result3);
                        int index = 0;
                        do
                        {
                            clsStructureType type4 = new clsStructureType();
                            type4.UnitType_ObjectDataLink.Connect(this.UnitTypes);
                            type4.StructureType_ObjectDataLink.Connect(this.StructureTypes);
                            type4.WallLink.Connect(wallType.Segments);
                            type4.Code = str25;
                            name = wallType.Name;
                            switch (index)
                            {
                                case 0:
                                    name = name + " - ";
                                    break;

                                case 1:
                                    name = name + " + ";
                                    break;

                                case 2:
                                    name = name + " T ";
                                    break;

                                case 3:
                                    name = name + " L ";
                                    break;
                            }
                            type4.Name = name;
                            type4.Footprint = _int;
                            type4.StructureType = clsStructureType.enumStructureType.Wall;
                            baseAttachment = type4.BaseAttachment;
                            name = strArray4[index];
                            baseAttachment.Models.Add(this.GetModelForPIE(list2, name, result3));
                            type4.StructureBasePlate = model;
                            index++;
                        }
                        while (index <= 3);
                    }
                }
                finally
                {
                    if (enumerator12 is IDisposable)
                    {
                        (enumerator12 as IDisposable).Dispose();
                    }
                }
                int num2 = 0;
                try
                {
                    enumerator13 = file14.ResultData.GetEnumerator();
                    while (enumerator13.MoveNext())
                    {
                        strArray = (string[]) enumerator13.Current;
                        clsDroidTemplate template = new clsDroidTemplate();
                        template.UnitType_ObjectDataLink.Connect(this.UnitTypes);
                        template.DroidTemplate_ObjectDataLink.Connect(this.DroidTemplates);
                        template.Code = strArray[0];
                        this.SetTemplateName(file8.ResultData, template, result3);
                        string str29 = strArray[9];
                        if (str29 == "ZNULLDROID")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_Null;
                        }
                        else if (str29 == "DROID")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                        }
                        else if (str29 == "CYBORG")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_Cyborg;
                        }
                        else if (str29 == "CYBORG_CONSTRUCT")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_CyborgConstruct;
                        }
                        else if (str29 == "CYBORG_REPAIR")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_CyborgRepair;
                        }
                        else if (str29 == "CYBORG_SUPER")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_CyborgSuper;
                        }
                        else if (str29 == "TRANSPORTER")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_Transporter;
                        }
                        else if (str29 == "PERSON")
                        {
                            template.TemplateDroidType = modProgram.TemplateDroidType_Person;
                        }
                        else
                        {
                            template.TemplateDroidType = null;
                            result3.WarningAdd("Template " + template.GetDisplayTextCode() + " had an unrecognised type.");
                        }
                        clsDroidDesign.sLoadPartsArgs args = new clsDroidDesign.sLoadPartsArgs {
                            Body = this.FindBodyCode(strArray[2]),
                            Brain = this.FindBrainCode(strArray[3]),
                            Construct = this.FindConstructorCode(strArray[4]),
                            ECM = this.FindECMCode(strArray[5]),
                            Propulsion = this.FindPropulsionCode(strArray[7]),
                            Repair = this.FindRepairCode(strArray[8]),
                            Sensor = this.FindSensorCode(strArray[10])
                        };
                        modLists.SimpleList<string[]> list4 = this.GetRowsWithValue(file.ResultData, template.Code);
                        if (list4.Count > 0)
                        {
                            name = list4[0][1];
                            if (name != "NULL")
                            {
                                args.Weapon1 = this.FindWeaponCode(name);
                            }
                            name = list4[0][2];
                            if (name != "NULL")
                            {
                                args.Weapon2 = this.FindWeaponCode(name);
                            }
                            name = list4[0][3];
                            if (name != "NULL")
                            {
                                args.Weapon3 = this.FindWeaponCode(name);
                            }
                        }
                        if (!template.LoadParts(args))
                        {
                            if (num2 < 0x10)
                            {
                                result3.WarningAdd("Template " + template.GetDisplayTextCode() + " had multiple conflicting turrets.");
                            }
                            num2++;
                        }
                    }
                }
                finally
                {
                    if (enumerator13 is IDisposable)
                    {
                        (enumerator13 as IDisposable).Dispose();
                    }
                }
                if (num2 > 0)
                {
                    result3.WarningAdd(Conversions.ToString(num2) + " templates had multiple conflicting turrets.");
                }
            }
            return result3;
        }

        public void SetComponentName(modLists.SimpleList<string[]> Names, clsComponent Component, clsResult Result)
        {
            modLists.SimpleList<string[]> rowsWithValue = this.GetRowsWithValue(Names, Component.Code);
            if (rowsWithValue.Count == 0)
            {
                Result.WarningAdd("No name for component " + Component.Code + ".");
            }
            else
            {
                Component.Name = rowsWithValue[0][1];
            }
        }

        public void SetFeatureName(modLists.SimpleList<string[]> Names, clsFeatureType FeatureType, clsResult Result)
        {
            modLists.SimpleList<string[]> rowsWithValue = this.GetRowsWithValue(Names, FeatureType.Code);
            if (rowsWithValue.Count == 0)
            {
                Result.WarningAdd("No name for feature type " + FeatureType.Code + ".");
            }
            else
            {
                FeatureType.Name = rowsWithValue[0][1];
            }
        }

        public void SetStructureName(modLists.SimpleList<string[]> Names, clsStructureType StructureType, clsResult Result)
        {
            modLists.SimpleList<string[]> rowsWithValue = this.GetRowsWithValue(Names, StructureType.Code);
            if (rowsWithValue.Count == 0)
            {
                Result.WarningAdd("No name for structure type " + StructureType.Code + ".");
            }
            else
            {
                StructureType.Name = rowsWithValue[0][1];
            }
        }

        public void SetTemplateName(modLists.SimpleList<string[]> Names, clsDroidTemplate Template, clsResult Result)
        {
            modLists.SimpleList<string[]> rowsWithValue = this.GetRowsWithValue(Names, Template.Code);
            if (rowsWithValue.Count == 0)
            {
                Result.WarningAdd("No name for droid template " + Template.Code + ".");
            }
            else
            {
                Template.Name = rowsWithValue[0][1];
            }
        }

        public void SetWallName(modLists.SimpleList<string[]> Names, clsWallType WallType, clsResult Result)
        {
            modLists.SimpleList<string[]> rowsWithValue = this.GetRowsWithValue(Names, WallType.Code);
            if (rowsWithValue.Count == 0)
            {
                Result.WarningAdd("No name for structure type " + WallType.Code + ".");
            }
            else
            {
                WallType.Name = rowsWithValue[0][1];
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct BodyProp
        {
            public string LeftPIE;
            public string RightPIE;
        }

        public class clsPIE
        {
            public string LCaseFileTitle;
            public clsModel Model;
            public string Path;
        }

        public class clsTextFile
        {
            public int FieldCount = 0;
            public modLists.SimpleList<string[]> ResultData = new modLists.SimpleList<string[]>();
            public string SubDirectory;
            public int UniqueField = 0;

            public bool CalcIsFieldCountValid()
            {
                IEnumerator enumerator;
                try
                {
                    enumerator = this.ResultData.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        string[] current = (string[]) enumerator.Current;
                        if (current.GetLength(0) != this.FieldCount)
                        {
                            return false;
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
                return true;
            }

            public bool CalcUniqueField()
            {
                if (this.UniqueField >= 0)
                {
                    int num3 = this.ResultData.Count - 1;
                    for (int i = 0; i <= num3; i++)
                    {
                        string str = this.ResultData[i][this.UniqueField];
                        int num4 = this.ResultData.Count - 1;
                        for (int j = i + 1; j <= num4; j++)
                        {
                            if (str == this.ResultData[j][this.UniqueField])
                            {
                                return false;
                            }
                        }
                    }
                }
                return true;
            }

            public clsResult LoadCommaFile(string Path)
            {
                string str;
                StreamReader reader;
                clsResult result2 = new clsResult("Loading comma separated file \"" + this.SubDirectory + "\"");
                try
                {
                    reader = new StreamReader(Path + this.SubDirectory, modProgram.UTF8Encoding);
                    goto Label_00B2;
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    result2.ProblemAdd(exception.Message);
                    clsResult result = result2;
                    ProjectData.ClearProjectError();
                    return result;
                }
            Label_0056:
                str = reader.ReadLine().Trim();
                if (str.Length > 0)
                {
                    string[] newItem = str.Split(new char[] { ',' });
                    int upperBound = newItem.GetUpperBound(0);
                    for (int i = 0; i <= upperBound; i++)
                    {
                        newItem[i] = newItem[i].Trim();
                    }
                    this.ResultData.Add(newItem);
                }
            Label_00B2:
                if (!reader.EndOfStream)
                {
                    goto Label_0056;
                }
                reader.Close();
                return result2;
            }

            public clsResult LoadNamesFile(string Path)
            {
                char ch;
                FileStream stream;
                bool flag2;
                bool flag3;
                clsResult result;
                char ch2;
                BinaryReader reader;
                clsResult result2 = new clsResult("Loading names file \"" + this.SubDirectory + "\"");
                try
                {
                    stream = new FileStream(Path + this.SubDirectory, FileMode.Open);
                }
                catch (Exception exception1)
                {
                    ProjectData.SetProjectError(exception1);
                    Exception exception = exception1;
                    result2.ProblemAdd(exception.Message);
                    result = result2;
                    ProjectData.ClearProjectError();
                    return result;
                }
                try
                {
                    reader = new BinaryReader(stream, modProgram.UTF8Encoding);
                }
                catch (Exception exception4)
                {
                    ProjectData.SetProjectError(exception4);
                    Exception exception2 = exception4;
                    stream.Close();
                    result2.ProblemAdd(exception2.Message);
                    result = result2;
                    ProjectData.ClearProjectError();
                    return result;
                }
                string str = "";
                bool flag = false;
            Label_0097:
                ch2 = ch;
                bool flag4 = flag;
                try
                {
                    ch = reader.ReadChar();
                    flag = true;
                }
                catch (Exception exception5)
                {
                    ProjectData.SetProjectError(exception5);
                    Exception exception3 = exception5;
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
                                str = str + Conversions.ToString(ch2);
                            }
                            flag = false;
                            if (str.Length > 0)
                            {
                                int index = str.IndexOf('\t');
                                int num2 = str.IndexOf(' ');
                                int length = index;
                                if ((num2 >= 0) & ((num2 < length) | (length < 0)))
                                {
                                    length = num2;
                                }
                                if (length >= 0)
                                {
                                    int num4 = str.IndexOf('"', length + 1, str.Length - (length + 1));
                                    if (num4 >= 0)
                                    {
                                        int num5 = str.IndexOf('"', num4 + 1, str.Length - (num4 + 1));
                                        if (num5 >= 0)
                                        {
                                            string[] newItem = new string[] { str.Substring(0, length), str.Substring(num4 + 1, num5 - (num4 + 1)) };
                                            this.ResultData.Add(newItem);
                                        }
                                    }
                                }
                                str = "";
                            }
                            goto Label_0097;
                    }
                    if (ch3 == '*')
                    {
                        if (!(flag4 & (ch2 == '/')))
                        {
                            goto Label_02DF;
                        }
                        flag2 = true;
                        flag = false;
                    }
                    else
                    {
                        if ((ch3 != '/') || !flag4)
                        {
                            goto Label_02DF;
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
                                goto Label_02DF;
                            }
                            flag2 = false;
                            flag = false;
                        }
                    }
                    goto Label_0097;
                }
                if (flag4)
                {
                    str = str + Conversions.ToString(ch2);
                }
                if (str.Length > 0)
                {
                    int num8 = str.IndexOf('\t');
                    int num7 = str.IndexOf(' ');
                    int num6 = num8;
                    if ((num7 >= 0) & ((num7 < num6) | (num6 < 0)))
                    {
                        num6 = num7;
                    }
                    if (num6 >= 0)
                    {
                        int num9 = str.IndexOf('"', num6 + 1, str.Length - (num6 + 1));
                        if (num9 >= 0)
                        {
                            int num10 = str.IndexOf('"', num9 + 1, str.Length - (num9 + 1));
                            if (num10 >= 0)
                            {
                                string[] strArray2 = new string[] { str.Substring(0, num6), str.Substring(num9 + 1, num10 - (num9 + 1)) };
                                this.ResultData.Add(strArray2);
                            }
                        }
                    }
                    str = "";
                }
                reader.Close();
                return result2;
            Label_02DF:
                if (flag4 && !(flag2 | flag3))
                {
                    str = str + Conversions.ToString(ch2);
                }
                goto Label_0097;
            }
        }

        public class clsTexturePage
        {
            public string FileTitle;
            public int GLTexture_Num;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sBytes
        {
            public byte[] Bytes;
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sLines
        {
            public string[] Lines;
            public void RemoveComments()
            {
                int num4 = this.Lines.GetUpperBound(0) + 1;
                int num6 = num4 - 1;
                for (int i = 0; i <= num6; i++)
                {
                    int num3;
                    bool flag;
                    int num = 0;
                    if (flag)
                    {
                        num3 = 0;
                    }
                Label_0025:
                    if (num >= this.Lines[i].Length)
                    {
                        if (flag)
                        {
                            this.Lines[i] = Strings.Left(this.Lines[i], num3);
                        }
                    }
                    else
                    {
                        int num2;
                        if (flag)
                        {
                            if (this.Lines[i][num] == '*')
                            {
                                num++;
                                if ((num < this.Lines[i].Length) && (this.Lines[i][num] == '/'))
                                {
                                    num++;
                                    num2 = num - num3;
                                    flag = false;
                                    this.Lines[i] = Strings.Left(this.Lines[i], num3) + Strings.Right(this.Lines[i], this.Lines[i].Length - (num3 + num2));
                                    num -= num2;
                                }
                            }
                            else
                            {
                                num++;
                            }
                        }
                        else if (this.Lines[i][num] == '/')
                        {
                            num++;
                            if (num < this.Lines[i].Length)
                            {
                                if (this.Lines[i][num] == '/')
                                {
                                    num3 = num - 1;
                                    num2 = this.Lines[i].Length - num3;
                                    this.Lines[i] = Strings.Left(this.Lines[i], num3) + Strings.Right(this.Lines[i], this.Lines[i].Length - (num3 + num2));
                                    continue;
                                }
                                if (this.Lines[i][num] == '*')
                                {
                                    num3 = num - 1;
                                    num++;
                                    flag = true;
                                }
                            }
                        }
                        else
                        {
                            num++;
                        }
                        goto Label_0025;
                    }
                }
            }
        }
    }
}

