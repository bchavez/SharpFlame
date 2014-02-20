#region

using System;
using System.Drawing;
using System.IO;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Bitmaps;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.FileIO;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Domain
{
    public class clsObjectData
    {
        public ConnectedList<Body, clsObjectData> Bodies;
        public ConnectedList<Brain, clsObjectData> Brains;
        public ConnectedList<Construct, clsObjectData> Constructors;
        public ConnectedList<DroidTemplate, clsObjectData> DroidTemplates;
        public ConnectedList<Ecm, clsObjectData> ECMs;
        public ConnectedList<FeatureTypeBase, clsObjectData> FeatureTypes;
        public ConnectedList<Propulsion, clsObjectData> Propulsions;
        public ConnectedList<Repair, clsObjectData> Repairs;
        public ConnectedList<Sensor, clsObjectData> Sensors;
        public ConnectedList<StructureTypeBase, clsObjectData> StructureTypes;

        public SimpleList<clsTexturePage> TexturePages = new SimpleList<clsTexturePage>();
        public ConnectedList<Turret, clsObjectData> Turrets;
        public ConnectedList<UnitTypeBase, clsObjectData> UnitTypes;
        public ConnectedList<clsWallType, clsObjectData> WallTypes;
        public ConnectedList<Weapon, clsObjectData> Weapons;

        public clsObjectData()
        {
            UnitTypes = new ConnectedList<UnitTypeBase, clsObjectData>(this);
            FeatureTypes = new ConnectedList<FeatureTypeBase, clsObjectData>(this);
            StructureTypes = new ConnectedList<StructureTypeBase, clsObjectData>(this);
            DroidTemplates = new ConnectedList<DroidTemplate, clsObjectData>(this);
            WallTypes = new ConnectedList<clsWallType, clsObjectData>(this);
            Bodies = new ConnectedList<Body, clsObjectData>(this);
            Propulsions = new ConnectedList<Propulsion, clsObjectData>(this);
            Turrets = new ConnectedList<Turret, clsObjectData>(this);
            Weapons = new ConnectedList<Weapon, clsObjectData>(this);
            Sensors = new ConnectedList<Sensor, clsObjectData>(this);
            Repairs = new ConnectedList<Repair, clsObjectData>(this);
            Constructors = new ConnectedList<Construct, clsObjectData>(this);
            Brains = new ConnectedList<Brain, clsObjectData>(this);
            ECMs = new ConnectedList<Ecm, clsObjectData>(this);
        }

        public clsResult LoadDirectory(string Path)
        {
            var ReturnResult = new clsResult(string.Format("Loading object data from \"{0}\"", Path));

            Path = PathUtil.EndWithPathSeperator(Path);

            var SubDirNames = "";
            var SubDirStructures = "";
            var SubDirBrain = "";
            var SubDirBody = "";
            var SubDirPropulsion = "";
            var SubDirBodyPropulsion = "";
            var SubDirConstruction = "";
            var SubDirSensor = "";
            var SubDirRepair = "";
            var SubDirTemplates = "";
            var SubDirWeapons = "";
            var SubDirECM = "";
            var SubDirFeatures = "";
            var SubDirTexpages = "";
            var SubDirAssignWeapons = "";
            var SubDirStructureWeapons = "";
            var SubDirPIEs = "";

            SubDirNames = "messages" + Convert.ToString(App.PlatformPathSeparator) + "strings" +
                          Convert.ToString(App.PlatformPathSeparator) + "names.txt";
            SubDirStructures = "stats" + Convert.ToString(App.PlatformPathSeparator) + "structures.txt";
            SubDirBrain = "stats" + Convert.ToString(App.PlatformPathSeparator) + "brain.txt";
            SubDirBody = "stats" + Convert.ToString(App.PlatformPathSeparator) + "body.txt";
            SubDirPropulsion = "stats" + Convert.ToString(App.PlatformPathSeparator) + "propulsion.txt";
            SubDirBodyPropulsion = "stats" + Convert.ToString(App.PlatformPathSeparator) + "bodypropulsionimd.txt";
            SubDirConstruction = "stats" + Convert.ToString(App.PlatformPathSeparator) + "construction.txt";
            SubDirSensor = "stats" + Convert.ToString(App.PlatformPathSeparator) + "sensor.txt";
            SubDirRepair = "stats" + Convert.ToString(App.PlatformPathSeparator) + "repair.txt";
            SubDirTemplates = "stats" + Convert.ToString(App.PlatformPathSeparator) + "templates.txt";
            SubDirWeapons = "stats" + Convert.ToString(App.PlatformPathSeparator) + "weapons.txt";
            SubDirECM = "stats" + Convert.ToString(App.PlatformPathSeparator) + "ecm.txt";
            SubDirFeatures = "stats" + Convert.ToString(App.PlatformPathSeparator) + "features.txt";
            SubDirPIEs = "pies" + Convert.ToString(App.PlatformPathSeparator);
            //SubDirStructurePIE = "structs" & ospathseperator
            //SubDirBodiesPIE = "components" & ospathseperator & "bodies" & ospathseperator
            //SubDirPropPIE = "components" & ospathseperator & "prop" & ospathseperator
            //SubDirWeaponsPIE = "components" & ospathseperator & "weapons" & ospathseperator
            SubDirTexpages = "texpages" + Convert.ToString(App.PlatformPathSeparator);
            SubDirAssignWeapons = "stats" + Convert.ToString(App.PlatformPathSeparator) + "assignweapons.txt";
            //SubDirFeaturePIE = "features" & ospathseperator
            SubDirStructureWeapons = "stats" + Convert.ToString(App.PlatformPathSeparator) + "structureweapons.txt";

            var CommaFiles = new SimpleList<clsTextFile>();

            var DataNames = new clsTextFile();
            DataNames.SubDirectory = SubDirNames;
            DataNames.UniqueField = 0;

            ReturnResult.Add(DataNames.LoadNamesFile(Path));
            if ( !DataNames.CalcUniqueField() )
            {
                ReturnResult.ProblemAdd("There are two entries for the same code in " + SubDirNames + ".");
            }

            var DataStructures = new clsTextFile();
            DataStructures.SubDirectory = SubDirStructures;
            DataStructures.FieldCount = 25;
            CommaFiles.Add(DataStructures);

            var DataBrain = new clsTextFile();
            DataBrain.SubDirectory = SubDirBrain;
            DataBrain.FieldCount = 9;
            CommaFiles.Add(DataBrain);

            var DataBody = new clsTextFile();
            DataBody.SubDirectory = SubDirBody;
            DataBody.FieldCount = 25;
            CommaFiles.Add(DataBody);

            var DataPropulsion = new clsTextFile();
            DataPropulsion.SubDirectory = SubDirPropulsion;
            DataPropulsion.FieldCount = 12;
            CommaFiles.Add(DataPropulsion);

            var DataBodyPropulsion = new clsTextFile();
            DataBodyPropulsion.SubDirectory = SubDirBodyPropulsion;
            DataBodyPropulsion.FieldCount = 5;
            DataBodyPropulsion.UniqueField = -1; //no unique requirement
            CommaFiles.Add(DataBodyPropulsion);

            var DataConstruction = new clsTextFile();
            DataConstruction.SubDirectory = SubDirConstruction;
            DataConstruction.FieldCount = 12;
            CommaFiles.Add(DataConstruction);

            var DataSensor = new clsTextFile();
            DataSensor.SubDirectory = SubDirSensor;
            DataSensor.FieldCount = 16;
            CommaFiles.Add(DataSensor);

            var DataRepair = new clsTextFile();
            DataRepair.SubDirectory = SubDirRepair;
            DataRepair.FieldCount = 14;
            CommaFiles.Add(DataRepair);

            var DataTemplates = new clsTextFile();
            DataTemplates.SubDirectory = SubDirTemplates;
            DataTemplates.FieldCount = 12;
            CommaFiles.Add(DataTemplates);

            var DataECM = new clsTextFile();
            DataECM.SubDirectory = SubDirECM;
            DataECM.FieldCount = 14;
            CommaFiles.Add(DataECM);

            var DataFeatures = new clsTextFile();
            DataFeatures.SubDirectory = SubDirFeatures;
            DataFeatures.FieldCount = 11;
            CommaFiles.Add(DataFeatures);

            var DataAssignWeapons = new clsTextFile();
            DataAssignWeapons.SubDirectory = SubDirAssignWeapons;
            DataAssignWeapons.FieldCount = 5;
            CommaFiles.Add(DataAssignWeapons);

            var DataWeapons = new clsTextFile();
            DataWeapons.SubDirectory = SubDirWeapons;
            DataWeapons.FieldCount = 53;
            CommaFiles.Add(DataWeapons);

            var DataStructureWeapons = new clsTextFile();
            DataStructureWeapons.SubDirectory = SubDirStructureWeapons;
            DataStructureWeapons.FieldCount = 6;
            CommaFiles.Add(DataStructureWeapons);

            var TextFile = default(clsTextFile);

            foreach ( var tempLoopVar_TextFile in CommaFiles )
            {
                TextFile = tempLoopVar_TextFile;
                var Result = TextFile.LoadCommaFile(Path);
                ReturnResult.Add(Result);
                if ( !Result.HasProblems )
                {
                    if ( TextFile.CalcIsFieldCountValid() )
                    {
                        if ( !TextFile.CalcUniqueField() )
                        {
                            ReturnResult.ProblemAdd("An entry in field " + Convert.ToString(TextFile.UniqueField) + " was not unique for file " +
                                                    TextFile.SubDirectory + ".");
                        }
                    }
                    else
                    {
                        ReturnResult.ProblemAdd("There were entries with the wrong number of fields for file " + TextFile.SubDirectory + ".");
                    }
                }
            }

            if ( ReturnResult.HasProblems )
            {
                return ReturnResult;
            }

            //load texpages

            string[] TexFiles = null;

            try
            {
                TexFiles = Directory.GetFiles(Path + SubDirTexpages);
            }
            catch ( Exception )
            {
                ReturnResult.WarningAdd("Unable to access texture pages.");
                TexFiles = new string[0];
            }

            var Text = "";
            Bitmap Bitmap = null;
            var InstrPos2 = 0;
            var BitmapResult = new sResult();

            foreach ( var tempLoopVar_Text in TexFiles )
            {
                Text = tempLoopVar_Text;
                if ( Text.Substring(Text.Length - 4, 4).ToLower() == ".png" )
                {
                    var Result = new clsResult(string.Format("Loading texture page \"{0}\"", Text));
                    if ( File.Exists(Text) )
                    {
                        BitmapResult = BitmapUtil.LoadBitmap(Text, ref Bitmap);
                        var NewPage = new clsTexturePage();
                        if ( BitmapResult.Success )
                        {
                            Result.Take(BitmapUtil.BitmapIsGlCompatible(Bitmap));
                            NewPage.GLTexture_Num = BitmapUtil.CreateGLTexture (Bitmap, 0);
                        }
                        else
                        {
                            Result.WarningAdd(BitmapResult.Problem);
                        }
                        InstrPos2 = Text.LastIndexOf(System.IO.Path.DirectorySeparatorChar);
                        NewPage.FileTitle = Text.Substring(InstrPos2 + 1, Text.Length - 5 - InstrPos2);

                        TexturePages.Add(NewPage);
                    }
                    else
                    {
                        Result.WarningAdd("Texture page missing (" + Text + ").");
                    }
                    ReturnResult.Add(Result);
                }
            }

            //load PIEs

            string[] PIE_Files = null;
            var PIE_List = new SimpleList<clsPIE>();
            var NewPIE = default(clsPIE);

            try
            {
                PIE_Files = Directory.GetFiles(Path + SubDirPIEs);
            }
            catch ( Exception )
            {
                ReturnResult.WarningAdd("Unable to access PIE files.");
                PIE_Files = new string[0];
            }

            var SplitPath = new sSplitPath();

            foreach ( var tempLoopVar_Text in PIE_Files )
            {
                Text = tempLoopVar_Text;
                SplitPath = new sSplitPath(Text);
                if ( SplitPath.FileExtension.ToLower() == "pie" )
                {
                    NewPIE = new clsPIE();
                    NewPIE.Path = Text;
                    NewPIE.LCaseFileTitle = SplitPath.FileTitle.ToLower();
                    PIE_List.Add(NewPIE);
                }
            }

            //interpret stats

            var Attachment = default(clsAttachment);
            var BaseAttachment = default(clsAttachment);
            var Connector = new XYZDouble();
            var structureTypeBase = default(StructureTypeBase);
            var featureTypeBase = default(FeatureTypeBase);
            var Template = default(DroidTemplate);
            var Body = default(Body);
            var Propulsion = default(Propulsion);
            var Construct = default(Construct);
            var Weapon = default(Weapon);
            var Repair = default(Repair);
            var Sensor = default(Sensor);
            var Brain = default(Brain);
            var ECM = default(Ecm);
            string[] Fields = null;

            //interpret body

            foreach ( var tempLoopVar_Fields in DataBody.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Body = new Body();
                Body.ObjectDataLink.Connect(Bodies);
                Body.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Body, ReturnResult);
                IOUtil.InvariantParse(Fields[6], ref Body.Hitpoints);
                Body.Designable = Fields[24] != "0";
                Body.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[7].ToLower(), ReturnResult));
            }

            //interpret propulsion

            foreach ( var tempLoopVar_Fields in DataPropulsion.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Propulsion = new Propulsion(Bodies.Count);
                Propulsion.ObjectDataLink.Connect(Propulsions);
                Propulsion.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Propulsion, ReturnResult);
                IOUtil.InvariantParse(Fields[7], ref Propulsion.HitPoints);
                //.Propulsions(Propulsion_Num).PIE = LCase(DataPropulsion.Entries(Propulsion_Num).FieldValues(8))
                Propulsion.Designable = Fields[11] != "0";
            }

            //interpret body-propulsions

            var BodyPropulsionPIEs = new BodyProp[Bodies.Count, Propulsions.Count];
            for ( var A = 0; A <= Bodies.Count - 1; A++ )
            {
                for ( var B = 0; B <= Propulsions.Count - 1; B++ )
                {
                    BodyPropulsionPIEs[A, B] = new BodyProp();
                    BodyPropulsionPIEs[A, B].LeftPIE = "0";
                    BodyPropulsionPIEs[A, B].RightPIE = "0";
                }
            }

            foreach ( var tempLoopVar_Fields in DataBodyPropulsion.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Body = FindBodyCode(Fields[0]);
                Propulsion = FindPropulsionCode(Fields[1]);
                if ( Body != null && Propulsion != null )
                {
                    if ( Fields[2] != "0" )
                    {
                        BodyPropulsionPIEs[Body.ObjectDataLink.ArrayPosition, Propulsion.ObjectDataLink.ArrayPosition].LeftPIE = Fields[2].ToLower();
                    }
                    if ( Fields[3] != "0" )
                    {
                        BodyPropulsionPIEs[Body.ObjectDataLink.ArrayPosition, Propulsion.ObjectDataLink.ArrayPosition].RightPIE = Fields[3].ToLower();
                    }
                }
            }

            //set propulsion-body PIEs

            for ( var A = 0; A <= Propulsions.Count - 1; A++ )
            {
                Propulsion = Propulsions[A];
                for ( var B = 0; B <= Bodies.Count - 1; B++ )
                {
                    Body = Bodies[B];
                    Propulsion.Bodies[B].LeftAttachment = new clsAttachment();
                    Propulsion.Bodies[B].LeftAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs[B, A].LeftPIE, ReturnResult));
                    Propulsion.Bodies[B].RightAttachment = new clsAttachment();
                    Propulsion.Bodies[B].RightAttachment.Models.Add(GetModelForPIE(PIE_List, BodyPropulsionPIEs[B, A].RightPIE, ReturnResult));
                }
            }

            //interpret construction

            foreach ( var tempLoopVar_Fields in DataConstruction.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Construct = new Construct();
                Construct.ObjectDataLink.Connect(Constructors);
                Construct.TurretObjectDataLink.Connect(Turrets);
                Construct.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Construct, ReturnResult);
                Construct.Designable = Fields[11] != "0";
                Construct.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[8].ToLower(), ReturnResult));
            }

            //interpret weapons

            foreach ( var tempLoopVar_Fields in DataWeapons.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Weapon = new Weapon();
                Weapon.ObjectDataLink.Connect(Weapons);
                Weapon.TurretObjectDataLink.Connect(Turrets);
                Weapon.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Weapon, ReturnResult);
                IOUtil.InvariantParse(Fields[7], ref Weapon.HitPoints);
                Weapon.Designable = Fields[51] != "0";
                Weapon.Attachment.Models.Add(GetModelForPIE(PIE_List, Convert.ToString(Fields[8].ToLower()), ReturnResult));
                Weapon.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[9].ToLower(), ReturnResult));
            }

            //interpret sensor

            foreach ( var tempLoopVar_Fields in DataSensor.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Sensor = new Sensor();
                Sensor.ObjectDataLink.Connect(Sensors);
                Sensor.TurretObjectDataLink.Connect(Turrets);
                Sensor.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Sensor, ReturnResult);
                IOUtil.InvariantParse(Fields[7], ref Sensor.HitPoints);
                Sensor.Designable = Fields[15] != "0";
                switch ( Fields[11].ToLower() )
                {
                    case "turret":
                        Sensor.Location = Sensor.enumLocation.Turret;
                        break;
                    case "default":
                        Sensor.Location = Sensor.enumLocation.Invisible;
                        break;
                    default:
                        Sensor.Location = Sensor.enumLocation.Invisible;
                        break;
                }
                Sensor.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[8].ToLower(), ReturnResult));
                Sensor.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[9].ToLower(), ReturnResult));
            }

            //interpret repair

            foreach ( var tempLoopVar_Fields in DataRepair.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Repair = new Repair();
                Repair.ObjectDataLink.Connect(Repairs);
                Repair.TurretObjectDataLink.Connect(Turrets);
                Repair.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Repair, ReturnResult);
                Repair.Designable = Fields[13] != "0";
                Repair.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[9].ToLower(), ReturnResult));
                Repair.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[10].ToLower(), ReturnResult));
            }

            //interpret brain

            foreach ( var tempLoopVar_Fields in DataBrain.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Brain = new Brain();
                Brain.ObjectDataLink.Connect(Brains);
                Brain.TurretObjectDataLink.Connect(Turrets);
                Brain.Code = Fields[0];
                SetComponentName(DataNames.ResultData, Brain, ReturnResult);
                Brain.Designable = true;
                Weapon = FindWeaponCode(Fields[7]);
                if ( Weapon != null )
                {
                    Brain.Weapon = Weapon;
                    Brain.Attachment = Weapon.Attachment;
                }
            }

            //interpret ecm

            foreach ( var tempLoopVar_Fields in DataECM.ResultData )
            {
                Fields = tempLoopVar_Fields;
                ECM = new Ecm();
                ECM.ObjectDataLink.Connect(ECMs);
                ECM.TurretObjectDataLink.Connect(Turrets);
                ECM.Code = Fields[0];
                SetComponentName(DataNames.ResultData, ECM, ReturnResult);
                IOUtil.InvariantParse(Fields[7], ref ECM.HitPoints);
                ECM.Designable = false;
                ECM.Attachment.Models.Add(GetModelForPIE(PIE_List, Fields[8].ToLower(), ReturnResult));
            }

            //interpret feature

            foreach ( var tempLoopVar_Fields in DataFeatures.ResultData )
            {
                Fields = tempLoopVar_Fields;
                featureTypeBase = new FeatureTypeBase();
                featureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                featureTypeBase.FeatureType_ObjectDataLink.Connect(FeatureTypes);
                featureTypeBase.Code = Fields[0];
                if ( Fields[7] == "OIL RESOURCE" ) //type
                {
                    featureTypeBase.FeatureType = FeatureTypeBase.enumFeatureType.OilResource;
                }
                SetFeatureName(DataNames.ResultData, featureTypeBase, ReturnResult);
                if ( !IOUtil.InvariantParse(Fields[1], ref featureTypeBase.Footprint.X) )
                {
                    ReturnResult.WarningAdd("Feature footprint-x was not an integer for " + featureTypeBase.Code + ".");
                }
                if ( !IOUtil.InvariantParse(Fields[2], ref featureTypeBase.Footprint.Y) )
                {
                    ReturnResult.WarningAdd("Feature footprint-y was not an integer for " + featureTypeBase.Code + ".");
                }
                featureTypeBase.BaseAttachment = new clsAttachment();
                BaseAttachment = featureTypeBase.BaseAttachment;
                Text = Fields[6].ToLower();
                Attachment = BaseAttachment.CreateAttachment();
                Attachment.Models.Add(GetModelForPIE(PIE_List, Text, ReturnResult));
            }

            //interpret structure

            foreach ( var tempLoopVar_Fields in DataStructures.ResultData )
            {
                Fields = tempLoopVar_Fields;
                var StructureCode = Fields[0];
                var StructureTypeText = Fields[1];
                var StructurePIEs = Fields[21].ToLower().Split('@');
                var StructureFootprint = new XYInt();
                var StructureBasePIE = Fields[22].ToLower();
                if ( !IOUtil.InvariantParse(Fields[5], ref StructureFootprint.X) )
                {
                    ReturnResult.WarningAdd("Structure footprint-x was not an integer for " + StructureCode + ".");
                }
                if ( !IOUtil.InvariantParse(Fields[6], ref StructureFootprint.Y) )
                {
                    ReturnResult.WarningAdd("Structure footprint-y was not an integer for " + StructureCode + ".");
                }
                if ( StructureTypeText != "WALL" || StructurePIEs.GetLength(0) != 4 )
                {
                    //this is NOT a generic wall
                    structureTypeBase = new StructureTypeBase();
                    structureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                    structureTypeBase.StructureType_ObjectDataLink.Connect(StructureTypes);
                    structureTypeBase.Code = StructureCode;
                    SetStructureName(DataNames.ResultData, structureTypeBase, ReturnResult);
                    structureTypeBase.Footprint = StructureFootprint;
                    switch ( StructureTypeText )
                    {
                        case "DEMOLISH":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Demolish;
                            break;
                        case "WALL":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Wall;
                            break;
                        case "CORNER WALL":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.CornerWall;
                            break;
                        case "FACTORY":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Factory;
                            break;
                        case "CYBORG FACTORY":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.CyborgFactory;
                            break;
                        case "VTOL FACTORY":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.VTOLFactory;
                            break;
                        case "COMMAND":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Command;
                            break;
                        case "HQ":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.HQ;
                            break;
                        case "DEFENSE":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Defense;
                            break;
                        case "POWER GENERATOR":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.PowerGenerator;
                            break;
                        case "POWER MODULE":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.PowerModule;
                            break;
                        case "RESEARCH":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Research;
                            break;
                        case "RESEARCH MODULE":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.ResearchModule;
                            break;
                        case "FACTORY MODULE":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.FactoryModule;
                            break;
                        case "DOOR":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.DOOR;
                            break;
                        case "REPAIR FACILITY":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.RepairFacility;
                            break;
                        case "SAT UPLINK":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.DOOR;
                            break;
                        case "REARM PAD":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.RearmPad;
                            break;
                        case "MISSILE SILO":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.MissileSilo;
                            break;
                        case "RESOURCE EXTRACTOR":
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.ResourceExtractor;
                            break;
                        default:
                            structureTypeBase.StructureType = StructureTypeBase.enumStructureType.Unknown;
                            break;
                    }

                    BaseAttachment = structureTypeBase.BaseAttachment;
                    if ( StructurePIEs.GetLength(0) > 0 )
                    {
                        BaseAttachment.Models.Add(GetModelForPIE(PIE_List, StructurePIEs[0], ReturnResult));
                    }
                    structureTypeBase.StructureBasePlate = GetModelForPIE(PIE_List, StructureBasePIE, ReturnResult);
                    if ( BaseAttachment.Models.Count == 1 )
                    {
                        if ( BaseAttachment.Models[0].ConnectorCount >= 1 )
                        {
                            Connector = BaseAttachment.Models[0].Connectors[0];
                            var StructureWeapons = default(SimpleList<string[]>);
                            StructureWeapons = GetRowsWithValue(DataStructureWeapons.ResultData, structureTypeBase.Code);
                            if ( StructureWeapons.Count > 0 )
                            {
                                Weapon = FindWeaponCode(Convert.ToString(StructureWeapons[0][1]));
                            }
                            else
                            {
                                Weapon = null;
                            }
                            ECM = FindECMCode(Fields[18]);
                            Sensor = FindSensorCode(Fields[19]);
                            if ( Weapon != null )
                            {
                                if ( Weapon.Code != "ZNULLWEAPON" )
                                {
                                    Attachment = BaseAttachment.CopyAttachment(Weapon.Attachment);
                                    Attachment.Pos_Offset = Connector;
                                }
                            }
                            if ( ECM != null )
                            {
                                if ( ECM.Code != "ZNULLECM" )
                                {
                                    Attachment = BaseAttachment.CopyAttachment(ECM.Attachment);
                                    Attachment.Pos_Offset = Connector;
                                }
                            }
                            if ( Sensor != null )
                            {
                                if ( Sensor.Code != "ZNULLSENSOR" )
                                {
                                    Attachment = BaseAttachment.CopyAttachment(Sensor.Attachment);
                                    Attachment.Pos_Offset = Connector;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //this is a generic wall
                    var NewWall = new clsWallType();
                    NewWall.WallType_ObjectDataLink.Connect(WallTypes);
                    NewWall.Code = StructureCode;
                    SetWallName(DataNames.ResultData, NewWall, ReturnResult);
                    var WallBasePlate = GetModelForPIE(PIE_List, StructureBasePIE, ReturnResult);

                    var WallNum = 0;
                    var wallStructureTypeBase = default(StructureTypeBase);
                    for ( WallNum = 0; WallNum <= 3; WallNum++ )
                    {
                        wallStructureTypeBase = new StructureTypeBase();
                        wallStructureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                        wallStructureTypeBase.StructureType_ObjectDataLink.Connect(StructureTypes);
                        wallStructureTypeBase.WallLink.Connect(NewWall.Segments);
                        wallStructureTypeBase.Code = StructureCode;
                        Text = NewWall.Name;
                        switch ( WallNum )
                        {
                            case 0:
                                Text += " - ";
                                break;
                            case 1:
                                Text += " + ";
                                break;
                            case 2:
                                Text += " T ";
                                break;
                            case 3:
                                Text += " L ";
                                break;
                        }
                        wallStructureTypeBase.Name = Text;
                        wallStructureTypeBase.Footprint = StructureFootprint;
                        wallStructureTypeBase.StructureType = StructureTypeBase.enumStructureType.Wall;

                        BaseAttachment = wallStructureTypeBase.BaseAttachment;

                        Text = StructurePIEs[WallNum];
                        BaseAttachment.Models.Add(GetModelForPIE(PIE_List, Text, ReturnResult));
                        wallStructureTypeBase.StructureBasePlate = WallBasePlate;
                    }
                }
            }

            //interpret templates

            var TurretConflictCount = 0;
            foreach ( var tempLoopVar_Fields in DataTemplates.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Template = new DroidTemplate();
                Template.UnitType_ObjectDataLink.Connect(UnitTypes);
                Template.DroidTemplate_ObjectDataLink.Connect(DroidTemplates);
                Template.Code = Fields[0];
                SetTemplateName(DataNames.ResultData, Template, ReturnResult);
                switch ( Fields[9] ) //type
                {
                    case "ZNULLDROID":
                        Template.TemplateDroidType = App.TemplateDroidType_Null;
                        break;
                    case "DROID":
                        Template.TemplateDroidType = App.TemplateDroidType_Droid;
                        break;
                    case "CYBORG":
                        Template.TemplateDroidType = App.TemplateDroidType_Cyborg;
                        break;
                    case "CYBORG_CONSTRUCT":
                        Template.TemplateDroidType = App.TemplateDroidType_CyborgConstruct;
                        break;
                    case "CYBORG_REPAIR":
                        Template.TemplateDroidType = App.TemplateDroidType_CyborgRepair;
                        break;
                    case "CYBORG_SUPER":
                        Template.TemplateDroidType = App.TemplateDroidType_CyborgSuper;
                        break;
                    case "TRANSPORTER":
                        Template.TemplateDroidType = App.TemplateDroidType_Transporter;
                        break;
                    case "PERSON":
                        Template.TemplateDroidType = App.TemplateDroidType_Person;
                        break;
                    default:
                        Template.TemplateDroidType = null;
                        ReturnResult.WarningAdd("Template " + Template.GetDisplayTextCode() + " had an unrecognised type.");
                        break;
                }
                var LoadPartsArgs = new DroidDesign.sLoadPartsArgs();
                LoadPartsArgs.Body = FindBodyCode(Fields[2]);
                LoadPartsArgs.Brain = FindBrainCode(Fields[3]);
                LoadPartsArgs.Construct = FindConstructorCode(Fields[4]);
                LoadPartsArgs.ECM = FindECMCode(Fields[5]);
                LoadPartsArgs.Propulsion = FindPropulsionCode(Fields[7]);
                LoadPartsArgs.Repair = FindRepairCode(Fields[8]);
                LoadPartsArgs.Sensor = FindSensorCode(Fields[10]);
                var TemplateWeapons = GetRowsWithValue(DataAssignWeapons.ResultData, Template.Code);
                if ( TemplateWeapons.Count > 0 )
                {
                    Text = Convert.ToString(TemplateWeapons[0][1]);
                    if ( Text != "NULL" )
                    {
                        LoadPartsArgs.Weapon1 = FindWeaponCode(Text);
                    }
                    Text = Convert.ToString(TemplateWeapons[0][2]);
                    if ( Text != "NULL" )
                    {
                        LoadPartsArgs.Weapon2 = FindWeaponCode(Text);
                    }
                    Text = Convert.ToString(TemplateWeapons[0][3]);
                    if ( Text != "NULL" )
                    {
                        LoadPartsArgs.Weapon3 = FindWeaponCode(Text);
                    }
                }
                if ( !Template.LoadParts(LoadPartsArgs) )
                {
                    if ( TurretConflictCount < 16 )
                    {
                        ReturnResult.WarningAdd("Template " + Template.GetDisplayTextCode() + " had multiple conflicting turrets.");
                    }
                    TurretConflictCount++;
                }
            }
            if ( TurretConflictCount > 0 )
            {
                ReturnResult.WarningAdd(TurretConflictCount + " templates had multiple conflicting turrets.");
            }

            return ReturnResult;
        }

        public SimpleList<string[]> GetRowsWithValue(SimpleList<string[]> TextLines, string Value)
        {
            var Result = new SimpleList<string[]>();

            string[] Line = null;
            foreach ( var tempLoopVar_Line in TextLines )
            {
                Line = tempLoopVar_Line;
                if ( Line[0] == Value )
                {
                    Result.Add(Line);
                }
            }

            return Result;
        }

        public clsModel GetModelForPIE(SimpleList<clsPIE> PIE_List, string PIE_LCaseFileTitle, clsResult ResultOutput)
        {
            if ( PIE_LCaseFileTitle == "0" )
            {
                return null;
            }

            var A = 0;
            var PIEFile = default(StreamReader);
            var PIE = default(clsPIE);

            var Result = new clsResult("Loading PIE file " + PIE_LCaseFileTitle);

            for ( A = 0; A <= PIE_List.Count - 1; A++ )
            {
                PIE = PIE_List[A];
                if ( PIE.LCaseFileTitle == PIE_LCaseFileTitle )
                {
                    if ( PIE.Model == null )
                    {
                        PIE.Model = new clsModel();
                        try
                        {
                            PIEFile = new StreamReader(PIE.Path);
                            try
                            {
                                Result.Take(PIE.Model.ReadPIE(PIEFile, this));
                            }
                            catch ( Exception ex )
                            {
                                PIEFile.Close();
                                Result.WarningAdd(ex.Message);
                                ResultOutput.Add(Result);
                                return PIE.Model;
                            }
                        }
                        catch ( Exception ex )
                        {
                            Result.WarningAdd(ex.Message);
                        }
                    }
                    ResultOutput.Add(Result);
                    return PIE.Model;
                }
            }

            if ( !Result.HasWarnings )
            {
                Result.WarningAdd("file is missing");
            }
            ResultOutput.Add(Result);

            return null;
        }

        public void SetComponentName(SimpleList<string[]> Names, ComponentBase componentBase, clsResult Result)
        {
            var ValueSearchResults = default(SimpleList<string[]>);

            ValueSearchResults = GetRowsWithValue(Names, componentBase.Code);
            if ( ValueSearchResults.Count == 0 )
            {
                Result.WarningAdd("No name for component " + componentBase.Code + ".");
            }
            else
            {
                componentBase.Name = Convert.ToString(ValueSearchResults[0][1]);
            }
        }

        public void SetFeatureName(SimpleList<string[]> Names, FeatureTypeBase featureTypeBase, clsResult Result)
        {
            var ValueSearchResults = default(SimpleList<string[]>);

            ValueSearchResults = GetRowsWithValue(Names, featureTypeBase.Code);
            if ( ValueSearchResults.Count == 0 )
            {
                Result.WarningAdd("No name for feature type " + featureTypeBase.Code + ".");
            }
            else
            {
                featureTypeBase.Name = Convert.ToString(ValueSearchResults[0][1]);
            }
        }

        public void SetStructureName(SimpleList<string[]> Names, StructureTypeBase structureTypeBase, clsResult Result)
        {
            var ValueSearchResults = default(SimpleList<string[]>);

            ValueSearchResults = GetRowsWithValue(Names, structureTypeBase.Code);
            if ( ValueSearchResults.Count == 0 )
            {
                Result.WarningAdd("No name for structure type " + structureTypeBase.Code + ".");
            }
            else
            {
                structureTypeBase.Name = Convert.ToString(ValueSearchResults[0][1]);
            }
        }

        public void SetTemplateName(SimpleList<string[]> Names, DroidTemplate Template, clsResult Result)
        {
            var ValueSearchResults = default(SimpleList<string[]>);

            ValueSearchResults = GetRowsWithValue(Names, Template.Code);
            if ( ValueSearchResults.Count == 0 )
            {
                Result.WarningAdd("No name for droid template " + Template.Code + ".");
            }
            else
            {
                Template.Name = Convert.ToString(ValueSearchResults[0][1]);
            }
        }

        public void SetWallName(SimpleList<string[]> Names, clsWallType WallType, clsResult Result)
        {
            var ValueSearchResults = default(SimpleList<string[]>);

            ValueSearchResults = GetRowsWithValue(Names, WallType.Code);
            if ( ValueSearchResults.Count == 0 )
            {
                Result.WarningAdd("No name for structure type " + WallType.Code + ".");
            }
            else
            {
                WallType.Name = Convert.ToString(ValueSearchResults[0][1]);
            }
        }

        public Body FindBodyCode(string Code)
        {
            var Component = default(Body);

            foreach ( var tempLoopVar_Component in Bodies )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Propulsion FindPropulsionCode(string Code)
        {
            var Component = default(Propulsion);

            foreach ( var tempLoopVar_Component in Propulsions )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Construct FindConstructorCode(string Code)
        {
            var Component = default(Construct);

            foreach ( var tempLoopVar_Component in Constructors )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Sensor FindSensorCode(string Code)
        {
            var Component = default(Sensor);

            foreach ( var tempLoopVar_Component in Sensors )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Repair FindRepairCode(string Code)
        {
            var Component = default(Repair);

            foreach ( var tempLoopVar_Component in Repairs )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Ecm FindECMCode(string Code)
        {
            var Component = default(Ecm);

            foreach ( var tempLoopVar_Component in ECMs )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Brain FindBrainCode(string Code)
        {
            var Component = default(Brain);

            foreach ( var tempLoopVar_Component in Brains )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public Weapon FindWeaponCode(string Code)
        {
            var Component = default(Weapon);

            foreach ( var tempLoopVar_Component in Weapons )
            {
                Component = tempLoopVar_Component;
                if ( Component.Code == Code )
                {
                    return Component;
                }
            }

            return null;
        }

        public int Get_TexturePage_GLTexture(string FileTitle)
        {
            var LCaseTitle = FileTitle.ToLower();
            var TexPage = default(clsTexturePage);

            foreach ( var tempLoopVar_TexPage in TexturePages )
            {
                TexPage = tempLoopVar_TexPage;
                if ( TexPage.FileTitle.ToLower() == LCaseTitle )
                {
                    return TexPage.GLTexture_Num;
                }
            }
            return 0;
        }

        public Weapon FindOrCreateWeapon(string Code)
        {
            var Result = default(Weapon);

            Result = FindWeaponCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Weapon();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Construct FindOrCreateConstruct(string Code)
        {
            var Result = default(Construct);

            Result = FindConstructorCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Construct();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Repair FindOrCreateRepair(string Code)
        {
            var Result = default(Repair);

            Result = FindRepairCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Repair();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Sensor FindOrCreateSensor(string Code)
        {
            var Result = default(Sensor);

            Result = FindSensorCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Sensor();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Brain FindOrCreateBrain(string Code)
        {
            var Result = default(Brain);

            Result = FindBrainCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Brain();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Ecm FindOrCreateECM(string Code)
        {
            var Result = default(Ecm);

            Result = FindECMCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Ecm();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Turret FindOrCreateTurret(enumTurretType TurretType, string TurretCode)
        {
            switch ( TurretType )
            {
                case enumTurretType.Weapon:
                    return FindOrCreateWeapon(TurretCode);
                case enumTurretType.Construct:
                    return FindOrCreateConstruct(TurretCode);
                case enumTurretType.Repair:
                    return FindOrCreateRepair(TurretCode);
                case enumTurretType.Sensor:
                    return FindOrCreateSensor(TurretCode);
                case enumTurretType.Brain:
                    return FindOrCreateBrain(TurretCode);
                case enumTurretType.ECM:
                    return FindOrCreateECM(TurretCode);
                default:
                    return null;
            }
        }

        public Body FindOrCreateBody(string Code)
        {
            var Result = default(Body);

            Result = FindBodyCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Body();
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public Propulsion FindOrCreatePropulsion(string Code)
        {
            var Result = default(Propulsion);

            Result = FindPropulsionCode(Code);
            if ( Result != null )
            {
                return Result;
            }
            Result = new Propulsion(Bodies.Count);
            Result.IsUnknown = true;
            Result.Code = Code;
            return Result;
        }

        public UnitTypeBase FindOrCreateUnitType(string Code, UnitType Type, int WallType)
        {
            switch ( Type )
            {
                case UnitType.Feature:
                    var featureTypeBase = default(FeatureTypeBase);
                    foreach ( var tempLoopVar_FeatureType in FeatureTypes )
                    {
                        featureTypeBase = tempLoopVar_FeatureType;
                        if ( featureTypeBase.Code == Code )
                        {
                            return featureTypeBase;
                        }
                    }
                    featureTypeBase = new FeatureTypeBase();
                    featureTypeBase.IsUnknown = true;
                    featureTypeBase.Code = Code;
                    featureTypeBase.Footprint.X = 1;
                    featureTypeBase.Footprint.Y = 1;
                    return featureTypeBase;
                case UnitType.PlayerStructure:
                    var structureTypeBase = default(StructureTypeBase);
                    foreach ( var tempLoopVar_StructureType in StructureTypes )
                    {
                        structureTypeBase = tempLoopVar_StructureType;
                        if ( structureTypeBase.Code == Code )
                        {
                            if ( WallType < 0 )
                            {
                                return structureTypeBase;
                            }
                            if ( structureTypeBase.WallLink.IsConnected )
                            {
                                if ( structureTypeBase.WallLink.ArrayPosition == WallType )
                                {
                                    return structureTypeBase;
                                }
                            }
                        }
                    }
                    structureTypeBase = new StructureTypeBase();
                    structureTypeBase.IsUnknown = true;
                    structureTypeBase.Code = Code;
                    structureTypeBase.Footprint.X = 1;
                    structureTypeBase.Footprint.Y = 1;
                    return structureTypeBase;

                case UnitType.PlayerDroid:
                    var DroidType = default(DroidTemplate);
                    foreach ( var tempLoopVar_DroidType in DroidTemplates )
                    {
                        DroidType = tempLoopVar_DroidType;
                        if ( DroidType.IsTemplate )
                        {
                            if ( DroidType.Code == Code )
                            {
                                return DroidType;
                            }
                        }
                    }
                    DroidType = new DroidTemplate();
                    DroidType.IsUnknown = true;
                    DroidType.Code = Code;
                    return DroidType;
                default:
                    return null;
            }
        }

        public StructureTypeBase FindFirstStructureType(StructureTypeBase.enumStructureType Type)
        {
            var structureTypeBase = default(StructureTypeBase);

            foreach ( var tempLoopVar_StructureType in StructureTypes )
            {
                structureTypeBase = tempLoopVar_StructureType;
                if ( structureTypeBase.StructureType == Type )
                {
                    return structureTypeBase;
                }
            }

            return null;
        }

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

            public SimpleList<string[]> ResultData = new SimpleList<string[]>();
            public string SubDirectory;
            public int UniqueField = 0;

            public bool CalcIsFieldCountValid()
            {
                string[] Text = null;
                foreach ( var tempLoopVar_Text in ResultData )
                {
                    Text = tempLoopVar_Text;
                    if ( Text.GetLength(0) != FieldCount )
                    {
                        return false;
                    }
                }

                return true;
            }

            public bool CalcUniqueField()
            {
                var A = 0;
                var B = 0;
                string Text;

                if ( UniqueField >= 0 )
                {
                    for ( A = 0; A <= ResultData.Count - 1; A++ )
                    {
                        Text = Convert.ToString(ResultData[A][UniqueField]);
                        for ( B = A + 1; B <= ResultData.Count - 1; B++ )
                        {
                            if ( Text == ResultData[B][UniqueField] )
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
                var Result = new clsResult(string.Format("Loading comma separated file \"{0}\"", SubDirectory));
                var Reader = default(StreamReader);

                try
                {
                    Reader = new StreamReader(Path + SubDirectory, App.UTF8Encoding);
                }
                catch ( Exception ex )
                {
                    Result.ProblemAdd(ex.Message);
                    return Result;
                }

                var Line = "";
                string[] LineFields = null;
                var A = 0;

                while ( !Reader.EndOfStream )
                {
                    Line = Reader.ReadLine();
                    Line = Line.Trim();
                    if ( Line.Length > 0 )
                    {
                        LineFields = Line.Split(',');
                        for ( A = 0; A <= LineFields.GetUpperBound(0); A++ )
                        {
                            LineFields[A] = LineFields[A].Trim();
                        }
                        ResultData.Add(LineFields);
                    }
                }

                Reader.Close();

                return Result;
            }

            public clsResult LoadNamesFile(string Path)
            {
                var Result = new clsResult(string.Format("Loading names file \"{0}\"", SubDirectory));
                var File = default(FileStream);
                var Reader = default(BinaryReader);

                try
                {
                    File = new FileStream(Path + SubDirectory, FileMode.Open);
                }
                catch ( Exception ex )
                {
                    Result.ProblemAdd(ex.Message);
                    return Result;
                }

                try
                {
                    Reader = new BinaryReader(File, App.UTF8Encoding);
                }
                catch ( Exception ex )
                {
                    File.Close();
                    Result.ProblemAdd(ex.Message);
                    return Result;
                }

                var CurrentChar = (char)0;
                var InLineComment = default(bool);
                var InCommentBlock = default(bool);
                var PrevChar = (char)0;
                var Line = "";
                var PrevCharExists = default(bool);
                var CurrentCharExists = false;

                do
                {
                    MonoContinueDo:
                    PrevChar = CurrentChar;
                    PrevCharExists = CurrentCharExists;
                    try
                    {
                        CurrentChar = Reader.ReadChar();
                        CurrentCharExists = true;
                    }
                    catch ( Exception )
                    {
                        CurrentCharExists = false;
                    }
                    if ( CurrentCharExists )
                    {
                        switch ( CurrentChar )
                        {
                            case '\r':
                            case '\n':
                                InLineComment = false;
                                if ( PrevCharExists )
                                {
                                    Line += PrevChar.ToString();
                                }
                                CurrentCharExists = false;

                                if ( Line.Length > 0 )
                                {
                                    var EndCodeTab = Line.IndexOf('\t');
                                    var EndCodeSpace = Line.IndexOf(' ');
                                    var EndCode = EndCodeTab;
                                    if ( EndCodeSpace >= 0 && (EndCodeSpace < EndCode | EndCode < 0) )
                                    {
                                        EndCode = EndCodeSpace;
                                    }
                                    if ( EndCode >= 0 )
                                    {
                                        var FirstQuote = Line.IndexOf('"', EndCode + 1, Line.Length - (EndCode + 1));
                                        if ( FirstQuote >= 0 )
                                        {
                                            var SecondQuote = Line.IndexOf('"', FirstQuote + 1, Line.Length - (FirstQuote + 1));
                                            if ( SecondQuote >= 0 )
                                            {
                                                var Value = new string[2];
                                                Value[0] = Line.Substring(0, EndCode);
                                                Value[1] = Line.Substring(FirstQuote + 1, SecondQuote - (FirstQuote + 1));
                                                ResultData.Add(Value);
                                            }
                                        }
                                    }
                                    Line = "";
                                }

                                goto MonoContinueDo;
                            case '*':
                                if ( PrevCharExists && PrevChar == '/' )
                                {
                                    InCommentBlock = true;
                                    CurrentCharExists = false;
                                    goto MonoContinueDo;
                                }
                                break;
                            case '/':
                                if ( PrevCharExists )
                                {
                                    if ( PrevChar == '/' )
                                    {
                                        InLineComment = true;
                                        CurrentCharExists = false;
                                        goto MonoContinueDo;
                                    }
                                    if ( PrevChar == '*' )
                                    {
                                        InCommentBlock = false;
                                        CurrentCharExists = false;
                                        goto MonoContinueDo;
                                    }
                                }
                                break;
                        }
                    }
                    else
                    {
                        if ( PrevCharExists )
                        {
                            Line += PrevChar.ToString();
                        }
                        if ( Line.Length > 0 )
                        {
                            var EndCodeTab = Line.IndexOf('\t');
                            var EndCodeSpace = Line.IndexOf(' ');
                            var EndCode = EndCodeTab;
                            if ( EndCodeSpace >= 0 && (EndCodeSpace < EndCode | EndCode < 0) )
                            {
                                EndCode = EndCodeSpace;
                            }
                            if ( EndCode >= 0 )
                            {
                                var FirstQuote = Line.IndexOf('"', EndCode + 1, Line.Length - (EndCode + 1));
                                if ( FirstQuote >= 0 )
                                {
                                    var SecondQuote = Line.IndexOf('"', FirstQuote + 1, Line.Length - (FirstQuote + 1));
                                    if ( SecondQuote >= 0 )
                                    {
                                        var Value = new string[2];
                                        Value[0] = Line.Substring(0, EndCode);
                                        Value[1] = Line.Substring(FirstQuote + 1, SecondQuote - (FirstQuote + 1));
                                        ResultData.Add(Value);
                                    }
                                }
                            }
                            Line = "";
                        }

                        break;
                    }
                    if ( PrevCharExists )
                    {
                        if ( !(InCommentBlock || InLineComment) )
                        {
                            Line += PrevChar.ToString();
                        }
                    }
                } while ( true );

                Reader.Close();

                return Result;
            }
        }

        public class clsTexturePage
        {
            public string FileTitle;
            public int GLTexture_Num;
        }

        public struct sBytes
        {
            public byte[] Bytes;
        }

        public struct sLines
        {
            public string[] Lines;

            public void RemoveComments()
            {
                var LineNum = 0;
                var LineCount = Lines.GetUpperBound(0) + 1;
                var InCommentBlock = default(bool);
                var CommentStart = 0;
                var CharNum = 0;
                var CommentLength = 0;

                for ( LineNum = 0; LineNum <= LineCount - 1; LineNum++ )
                {
                    CharNum = 0;
                    if ( InCommentBlock )
                    {
                        CommentStart = 0;
                    }
                    do
                    {
                        if ( CharNum >= Lines[LineNum].Length )
                        {
                            if ( InCommentBlock )
                            {
                                Lines[LineNum] = Lines[LineNum].Substring(0, CommentStart);
                            }
                            break;
                        }
                        if ( InCommentBlock )
                        {
                            if ( Lines[LineNum][CharNum] == '*' )
                            {
                                CharNum++;
                                if ( CharNum >= Lines[LineNum].Length )
                                {
                                }
                                else if ( Lines[LineNum][CharNum] == '/' )
                                {
                                    CharNum++;
                                    CommentLength = CharNum - CommentStart;
                                    InCommentBlock = false;
                                    Lines[LineNum] = Lines[LineNum].Substring(CommentStart, Lines[LineNum].Length - CommentStart)
                                        .Substring(CommentStart + CommentLength, Lines[LineNum].Length - CommentStart - CommentLength);
                                    CharNum -= CommentLength;
                                }
                            }
                            else
                            {
                                CharNum++;
                            }
                        }
                        else if ( Lines[LineNum][CharNum] == '/' )
                        {
                            CharNum++;
                            if ( CharNum >= Lines[LineNum].Length )
                            {
                            }
                            else if ( Lines[LineNum][CharNum] == '/' )
                            {
                                CommentStart = CharNum - 1;
                                CharNum = Lines[LineNum].Length;
                                CommentLength = CharNum - CommentStart;
                                Lines[LineNum] = Lines[LineNum].Substring(CommentStart, Lines[LineNum].Length - CommentStart)
                                    .Substring(CommentStart + CommentLength, Lines[LineNum].Length - CommentStart - CommentLength);
                                CharNum -= CommentLength;
                                break;
                            }
                            else if ( Lines[LineNum][CharNum] == '*' )
                            {
                                CommentStart = CharNum - 1;
                                CharNum++;
                                InCommentBlock = true;
                            }
                        }
                        else
                        {
                            CharNum++;
                        }
                    } while ( true );
                }
            }
        }
    }
}