#region

using System;
using System.Drawing;
using System.IO;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Bitmaps;
using SharpFlame.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
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

        public clsResult LoadDirectory(string path)
        {
            var returnResult = new clsResult(string.Format("Loading object data from \"{0}\"", path));

            path = PathUtil.EndWithPathSeperator(path);

            var subDirNames = "";
            var subDirStructures = "";
            var subDirBrain = "";
            var subDirBody = "";
            var subDirPropulsion = "";
            var subDirBodyPropulsion = "";
            var subDirConstruction = "";
            var subDirSensor = "";
            var subDirRepair = "";
            var subDirTemplates = "";
            var subDirWeapons = "";
            var subDirEcm = "";
            var subDirFeatures = "";
            var subDirTexpages = "";
            var subDirAssignWeapons = "";
            var subDirStructureWeapons = "";
            var subDirPiEs = "";

            subDirNames = "messages".CombinePathWith("strings").CombinePathWith("names.txt");
            subDirStructures = "stats".CombinePathWith("structures.txt");
            subDirBrain = "stats".CombinePathWith("brain.txt");
            subDirBody = "stats".CombinePathWith("body.txt");
            subDirPropulsion = "stats".CombinePathWith("propulsion.txt");
            subDirBodyPropulsion = "stats".CombinePathWith("bodypropulsionimd.txt");
            subDirConstruction = "stats".CombinePathWith("construction.txt");
            subDirSensor = "stats".CombinePathWith("sensor.txt");
            subDirRepair = "stats".CombinePathWith("repair.txt");
            subDirTemplates = "stats".CombinePathWith("templates.txt");
            subDirWeapons = "stats".CombinePathWith("weapons.txt");
            subDirEcm = "stats".CombinePathWith("ecm.txt");
            subDirFeatures = "stats".CombinePathWith("features.txt");
            subDirPiEs = "pies".CombinePathWith("", endWithPathSeparator: true);
            //SubDirStructurePIE = "structs" & ospathseperator
            //SubDirBodiesPIE = "components" & ospathseperator & "bodies" & ospathseperator
            //SubDirPropPIE = "components" & ospathseperator & "prop" & ospathseperator
            //SubDirWeaponsPIE = "components" & ospathseperator & "weapons" & ospathseperator
            subDirTexpages = "texpages".CombinePathWith("", endWithPathSeparator: true);
            subDirAssignWeapons = "stats".CombinePathWith("assignweapons.txt");
            //SubDirFeaturePIE = "features" & ospathseperator
            subDirStructureWeapons = "stats".CombinePathWith("structureweapons.txt");

            var commaFiles = new SimpleList<clsTextFile>();

            var dataNames = new clsTextFile
                {
                    SubDirectory = subDirNames,
                    UniqueField = 0
                };

            returnResult.Add(dataNames.LoadNamesFile(path));
            if ( !dataNames.CalcUniqueField() )
            {
                returnResult.ProblemAdd("There are two entries for the same code in " + subDirNames + ".");
            }

            var dataStructures = new clsTextFile
                {
                    SubDirectory = subDirStructures, FieldCount = 25
                };
            commaFiles.Add(dataStructures);

            var DataBrain = new clsTextFile();
            DataBrain.SubDirectory = subDirBrain;
            DataBrain.FieldCount = 9;
            commaFiles.Add(DataBrain);

            var DataBody = new clsTextFile();
            DataBody.SubDirectory = subDirBody;
            DataBody.FieldCount = 25;
            commaFiles.Add(DataBody);

            var DataPropulsion = new clsTextFile();
            DataPropulsion.SubDirectory = subDirPropulsion;
            DataPropulsion.FieldCount = 12;
            commaFiles.Add(DataPropulsion);

            var DataBodyPropulsion = new clsTextFile();
            DataBodyPropulsion.SubDirectory = subDirBodyPropulsion;
            DataBodyPropulsion.FieldCount = 5;
            DataBodyPropulsion.UniqueField = -1; //no unique requirement
            commaFiles.Add(DataBodyPropulsion);

            var DataConstruction = new clsTextFile();
            DataConstruction.SubDirectory = subDirConstruction;
            DataConstruction.FieldCount = 12;
            commaFiles.Add(DataConstruction);

            var DataSensor = new clsTextFile();
            DataSensor.SubDirectory = subDirSensor;
            DataSensor.FieldCount = 16;
            commaFiles.Add(DataSensor);

            var DataRepair = new clsTextFile();
            DataRepair.SubDirectory = subDirRepair;
            DataRepair.FieldCount = 14;
            commaFiles.Add(DataRepair);

            var DataTemplates = new clsTextFile();
            DataTemplates.SubDirectory = subDirTemplates;
            DataTemplates.FieldCount = 12;
            commaFiles.Add(DataTemplates);

            var DataECM = new clsTextFile();
            DataECM.SubDirectory = subDirEcm;
            DataECM.FieldCount = 14;
            commaFiles.Add(DataECM);

            var DataFeatures = new clsTextFile();
            DataFeatures.SubDirectory = subDirFeatures;
            DataFeatures.FieldCount = 11;
            commaFiles.Add(DataFeatures);

            var DataAssignWeapons = new clsTextFile();
            DataAssignWeapons.SubDirectory = subDirAssignWeapons;
            DataAssignWeapons.FieldCount = 5;
            commaFiles.Add(DataAssignWeapons);

            var DataWeapons = new clsTextFile();
            DataWeapons.SubDirectory = subDirWeapons;
            DataWeapons.FieldCount = 53;
            commaFiles.Add(DataWeapons);

            var DataStructureWeapons = new clsTextFile();
            DataStructureWeapons.SubDirectory = subDirStructureWeapons;
            DataStructureWeapons.FieldCount = 6;
            commaFiles.Add(DataStructureWeapons);

            var TextFile = default(clsTextFile);

            foreach ( var tempLoopVar_TextFile in commaFiles )
            {
                TextFile = tempLoopVar_TextFile;
                var Result = TextFile.LoadCommaFile(path);
                returnResult.Add(Result);
                if ( !Result.HasProblems )
                {
                    if ( TextFile.CalcIsFieldCountValid() )
                    {
                        if ( !TextFile.CalcUniqueField() )
                        {
                            returnResult.ProblemAdd("An entry in field " + Convert.ToString(TextFile.UniqueField) + " was not unique for file " +
                                                    TextFile.SubDirectory + ".");
                        }
                    }
                    else
                    {
                        returnResult.ProblemAdd("There were entries with the wrong number of fields for file " + TextFile.SubDirectory + ".");
                    }
                }
            }

            if ( returnResult.HasProblems )
            {
                return returnResult;
            }

            //load texpages

            string[] TexFiles = null;

            try
            {
                TexFiles = Directory.GetFiles(path + subDirTexpages);
            }
            catch ( Exception )
            {
                returnResult.WarningAdd("Unable to access texture pages.");
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
                    returnResult.Add(Result);
                }
            }

            //load PIEs

            string[] pieFiles = null;
            var pieList = new SimpleList<clsPIE>();
            var NewPIE = default(clsPIE);

            try
            {
                pieFiles = Directory.GetFiles(path + subDirPiEs);
            }
            catch ( Exception )
            {
                returnResult.WarningAdd("Unable to access PIE files.");
                pieFiles = new string[0];
            }

            var SplitPath = new sSplitPath();

            foreach ( var tempLoopVar_Text in pieFiles )
            {
                Text = tempLoopVar_Text;
                SplitPath = new sSplitPath(Text);
                if ( SplitPath.FileExtension.ToLower() == "pie" )
                {
                    NewPIE = new clsPIE();
                    NewPIE.Path = Text;
                    NewPIE.LCaseFileTitle = SplitPath.FileTitle.ToLower();
                    pieList.Add(NewPIE);
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
                SetComponentName(dataNames.ResultData, Body, returnResult);
                IOUtil.InvariantParse(Fields[6], ref Body.Hitpoints);
                Body.Designable = Fields[24] != "0";
                Body.Attachment.Models.Add(GetModelForPIE(pieList, Fields[7].ToLower(), returnResult));
            }

            //interpret propulsion

            foreach ( var tempLoopVar_Fields in DataPropulsion.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Propulsion = new Propulsion(Bodies.Count);
                Propulsion.ObjectDataLink.Connect(Propulsions);
                Propulsion.Code = Fields[0];
                SetComponentName(dataNames.ResultData, Propulsion, returnResult);
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
                    Propulsion.Bodies[B].LeftAttachment.Models.Add(GetModelForPIE(pieList, BodyPropulsionPIEs[B, A].LeftPIE, returnResult));
                    Propulsion.Bodies[B].RightAttachment = new clsAttachment();
                    Propulsion.Bodies[B].RightAttachment.Models.Add(GetModelForPIE(pieList, BodyPropulsionPIEs[B, A].RightPIE, returnResult));
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
                SetComponentName(dataNames.ResultData, Construct, returnResult);
                Construct.Designable = Fields[11] != "0";
                Construct.Attachment.Models.Add(GetModelForPIE(pieList, Fields[8].ToLower(), returnResult));
            }

            //interpret weapons

            foreach ( var tempLoopVar_Fields in DataWeapons.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Weapon = new Weapon();
                Weapon.ObjectDataLink.Connect(Weapons);
                Weapon.TurretObjectDataLink.Connect(Turrets);
                Weapon.Code = Fields[0];
                SetComponentName(dataNames.ResultData, Weapon, returnResult);
                IOUtil.InvariantParse(Fields[7], ref Weapon.HitPoints);
                Weapon.Designable = Fields[51] != "0";
                Weapon.Attachment.Models.Add(GetModelForPIE(pieList, Convert.ToString(Fields[8].ToLower()), returnResult));
                Weapon.Attachment.Models.Add(GetModelForPIE(pieList, Fields[9].ToLower(), returnResult));
            }

            //interpret sensor

            foreach ( var tempLoopVar_Fields in DataSensor.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Sensor = new Sensor();
                Sensor.ObjectDataLink.Connect(Sensors);
                Sensor.TurretObjectDataLink.Connect(Turrets);
                Sensor.Code = Fields[0];
                SetComponentName(dataNames.ResultData, Sensor, returnResult);
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
                Sensor.Attachment.Models.Add(GetModelForPIE(pieList, Fields[8].ToLower(), returnResult));
                Sensor.Attachment.Models.Add(GetModelForPIE(pieList, Fields[9].ToLower(), returnResult));
            }

            //interpret repair

            foreach ( var tempLoopVar_Fields in DataRepair.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Repair = new Repair();
                Repair.ObjectDataLink.Connect(Repairs);
                Repair.TurretObjectDataLink.Connect(Turrets);
                Repair.Code = Fields[0];
                SetComponentName(dataNames.ResultData, Repair, returnResult);
                Repair.Designable = Fields[13] != "0";
                Repair.Attachment.Models.Add(GetModelForPIE(pieList, Fields[9].ToLower(), returnResult));
                Repair.Attachment.Models.Add(GetModelForPIE(pieList, Fields[10].ToLower(), returnResult));
            }

            //interpret brain

            foreach ( var tempLoopVar_Fields in DataBrain.ResultData )
            {
                Fields = tempLoopVar_Fields;
                Brain = new Brain();
                Brain.ObjectDataLink.Connect(Brains);
                Brain.TurretObjectDataLink.Connect(Turrets);
                Brain.Code = Fields[0];
                SetComponentName(dataNames.ResultData, Brain, returnResult);
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
                SetComponentName(dataNames.ResultData, ECM, returnResult);
                IOUtil.InvariantParse(Fields[7], ref ECM.HitPoints);
                ECM.Designable = false;
                ECM.Attachment.Models.Add(GetModelForPIE(pieList, Fields[8].ToLower(), returnResult));
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
                SetFeatureName(dataNames.ResultData, featureTypeBase, returnResult);
                if ( !IOUtil.InvariantParse(Fields[1], ref featureTypeBase.Footprint.X) )
                {
                    returnResult.WarningAdd("Feature footprint-x was not an integer for " + featureTypeBase.Code + ".");
                }
                if ( !IOUtil.InvariantParse(Fields[2], ref featureTypeBase.Footprint.Y) )
                {
                    returnResult.WarningAdd("Feature footprint-y was not an integer for " + featureTypeBase.Code + ".");
                }
                featureTypeBase.BaseAttachment = new clsAttachment();
                BaseAttachment = featureTypeBase.BaseAttachment;
                Text = Fields[6].ToLower();
                Attachment = BaseAttachment.CreateAttachment();
                Attachment.Models.Add(GetModelForPIE(pieList, Text, returnResult));
            }

            //interpret structure

            foreach ( var tempLoopVar_Fields in dataStructures.ResultData )
            {
                Fields = tempLoopVar_Fields;
                var StructureCode = Fields[0];
                var StructureTypeText = Fields[1];
                var StructurePIEs = Fields[21].ToLower().Split('@');
                var StructureFootprint = new XYInt();
                var StructureBasePIE = Fields[22].ToLower();
                if ( !IOUtil.InvariantParse(Fields[5], ref StructureFootprint.X) )
                {
                    returnResult.WarningAdd("Structure footprint-x was not an integer for " + StructureCode + ".");
                }
                if ( !IOUtil.InvariantParse(Fields[6], ref StructureFootprint.Y) )
                {
                    returnResult.WarningAdd("Structure footprint-y was not an integer for " + StructureCode + ".");
                }
                if ( StructureTypeText != "WALL" || StructurePIEs.GetLength(0) != 4 )
                {
                    //this is NOT a generic wall
                    structureTypeBase = new StructureTypeBase();
                    structureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                    structureTypeBase.StructureType_ObjectDataLink.Connect(StructureTypes);
                    structureTypeBase.Code = StructureCode;
                    SetStructureName(dataNames.ResultData, structureTypeBase, returnResult);
                    structureTypeBase.Footprint = StructureFootprint;
                    switch ( StructureTypeText )
                    {
                        case "DEMOLISH":
                            structureTypeBase.StructureType = StructureType.Demolish;
                            break;
                        case "WALL":
                            structureTypeBase.StructureType = StructureType.Wall;
                            break;
                        case "CORNER WALL":
                            structureTypeBase.StructureType = StructureType.CornerWall;
                            break;
                        case "FACTORY":
                            structureTypeBase.StructureType = StructureType.Factory;
                            break;
                        case "CYBORG FACTORY":
                            structureTypeBase.StructureType = StructureType.CyborgFactory;
                            break;
                        case "VTOL FACTORY":
                            structureTypeBase.StructureType = StructureType.VTOLFactory;
                            break;
                        case "COMMAND":
                            structureTypeBase.StructureType = StructureType.Command;
                            break;
                        case "HQ":
                            structureTypeBase.StructureType = StructureType.HQ;
                            break;
                        case "DEFENSE":
                            structureTypeBase.StructureType = StructureType.Defense;
                            break;
                        case "POWER GENERATOR":
                            structureTypeBase.StructureType = StructureType.PowerGenerator;
                            break;
                        case "POWER MODULE":
                            structureTypeBase.StructureType = StructureType.PowerModule;
                            break;
                        case "RESEARCH":
                            structureTypeBase.StructureType = StructureType.Research;
                            break;
                        case "RESEARCH MODULE":
                            structureTypeBase.StructureType = StructureType.ResearchModule;
                            break;
                        case "FACTORY MODULE":
                            structureTypeBase.StructureType = StructureType.FactoryModule;
                            break;
                        case "DOOR":
                            structureTypeBase.StructureType = StructureType.DOOR;
                            break;
                        case "REPAIR FACILITY":
                            structureTypeBase.StructureType = StructureType.RepairFacility;
                            break;
                        case "SAT UPLINK":
                            structureTypeBase.StructureType = StructureType.DOOR;
                            break;
                        case "REARM PAD":
                            structureTypeBase.StructureType = StructureType.RearmPad;
                            break;
                        case "MISSILE SILO":
                            structureTypeBase.StructureType = StructureType.MissileSilo;
                            break;
                        case "RESOURCE EXTRACTOR":
                            structureTypeBase.StructureType = StructureType.ResourceExtractor;
                            break;
                        default:
                            structureTypeBase.StructureType = StructureType.Unknown;
                            break;
                    }

                    BaseAttachment = structureTypeBase.BaseAttachment;
                    if ( StructurePIEs.GetLength(0) > 0 )
                    {
                        BaseAttachment.Models.Add(GetModelForPIE(pieList, StructurePIEs[0], returnResult));
                    }
                    structureTypeBase.StructureBasePlate = GetModelForPIE(pieList, StructureBasePIE, returnResult);
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
                                    Attachment.PosOffset = Connector;
                                }
                            }
                            if ( ECM != null )
                            {
                                if ( ECM.Code != "ZNULLECM" )
                                {
                                    Attachment = BaseAttachment.CopyAttachment(ECM.Attachment);
                                    Attachment.PosOffset = Connector;
                                }
                            }
                            if ( Sensor != null )
                            {
                                if ( Sensor.Code != "ZNULLSENSOR" )
                                {
                                    Attachment = BaseAttachment.CopyAttachment(Sensor.Attachment);
                                    Attachment.PosOffset = Connector;
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
                    SetWallName(dataNames.ResultData, NewWall, returnResult);
                    var WallBasePlate = GetModelForPIE(pieList, StructureBasePIE, returnResult);

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
                        wallStructureTypeBase.StructureType = StructureType.Wall;

                        BaseAttachment = wallStructureTypeBase.BaseAttachment;

                        Text = StructurePIEs[WallNum];
                        BaseAttachment.Models.Add(GetModelForPIE(pieList, Text, returnResult));
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
                SetTemplateName(dataNames.ResultData, Template, returnResult);
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
                        returnResult.WarningAdd("Template " + Template.GetDisplayTextCode() + " had an unrecognised type.");
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
                        returnResult.WarningAdd("Template " + Template.GetDisplayTextCode() + " had multiple conflicting turrets.");
                    }
                    TurretConflictCount++;
                }
            }
            if ( TurretConflictCount > 0 )
            {
                returnResult.WarningAdd(TurretConflictCount + " templates had multiple conflicting turrets.");
            }

            return returnResult;
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

        public StructureTypeBase FindFirstStructureType(StructureType Type)
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