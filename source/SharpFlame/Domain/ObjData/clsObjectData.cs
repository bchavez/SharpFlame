#region

using System;
using System.Drawing;
using System.IO;
using System.Linq;
using SharpFlame.Bitmaps;
using SharpFlame.Collections;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Core.Extensions;
using SharpFlame.FileIO;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Domain.ObjData
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

        public Result LoadDirectory(string path)
        {
            var returnResult = new Result(string.Format("Loading object data from \"{0}\"", path));

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
            subDirTexpages = "texpages".CombinePathWith("", endWithPathSeparator: true);
            subDirAssignWeapons = "stats".CombinePathWith("assignweapons.txt");
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

            var dataStructures = new clsTextFile{SubDirectory = subDirStructures, FieldCount = 25};
            commaFiles.Add(dataStructures);

            var dataBrain = new clsTextFile{SubDirectory = subDirBrain, FieldCount = 9};
            commaFiles.Add(dataBrain);

            var dataBody = new clsTextFile{SubDirectory = subDirBody, FieldCount = 25};
            commaFiles.Add(dataBody);

            var dataPropulsion = new clsTextFile{SubDirectory = subDirPropulsion, FieldCount = 12};
            commaFiles.Add(dataPropulsion);

            var dataBodyPropulsion = new clsTextFile{SubDirectory = subDirBodyPropulsion, FieldCount = 5, UniqueField = -1};
            commaFiles.Add(dataBodyPropulsion);

            var dataConstruction = new clsTextFile{SubDirectory = subDirConstruction, FieldCount = 12};
            commaFiles.Add(dataConstruction);

            var dataSensor = new clsTextFile{SubDirectory = subDirSensor, FieldCount = 16};
            commaFiles.Add(dataSensor);

            var dataRepair = new clsTextFile {SubDirectory = subDirRepair, FieldCount = 14};
            commaFiles.Add(dataRepair);

            var dataTemplates = new clsTextFile {SubDirectory = subDirTemplates, FieldCount = 12};
            commaFiles.Add(dataTemplates);

            var dataEcm = new clsTextFile {SubDirectory = subDirEcm, FieldCount = 14};
            commaFiles.Add(dataEcm);

            var dataFeatures = new clsTextFile {SubDirectory = subDirFeatures, FieldCount = 11};
            commaFiles.Add(dataFeatures);

            var dataAssignWeapons = new clsTextFile {SubDirectory = subDirAssignWeapons, FieldCount = 5};
            commaFiles.Add(dataAssignWeapons);

            var dataWeapons = new clsTextFile {SubDirectory = subDirWeapons, FieldCount = 53};
            commaFiles.Add(dataWeapons);

            var dataStructureWeapons = new clsTextFile {SubDirectory = subDirStructureWeapons, FieldCount = 6};
            commaFiles.Add(dataStructureWeapons);

            foreach ( var textFile in commaFiles )
            {
                var result = textFile.LoadCommaFile(path);
                returnResult.Add(result);
                if ( !result.HasProblems )
                {
                    if ( textFile.CalcIsFieldCountValid() )
                    {
                        if ( !textFile.CalcUniqueField() )
                        {
                            returnResult.ProblemAdd("An entry in field " + Convert.ToString(textFile.UniqueField) + " was not unique for file " +
                                                    textFile.SubDirectory + ".");
                        }
                    }
                    else
                    {
                        returnResult.ProblemAdd("There were entries with the wrong number of fields for file " + textFile.SubDirectory + ".");
                    }
                }
            }

            if ( returnResult.HasProblems )
            {
                return returnResult;
            }

            //load texpages

            string[] texFiles = null;

            try
            {
                texFiles = Directory.GetFiles(path + subDirTexpages);
            }
            catch ( Exception )
            {
                returnResult.WarningAdd("Unable to access texture pages.");
                texFiles = new string[0];
            }

            var text = "";
            Bitmap bitmap = null;

            foreach ( var texFile in texFiles )
            {
                text = texFile;
                if ( text.Substring(text.Length - 4, 4).ToLower() == ".png" )
                {
                    var result = new Result(string.Format("Loading texture page \"{0}\"", text));
                    if ( File.Exists(text) )
                    {
                        sResult bitmapResult = BitmapUtil.LoadBitmap(text, ref bitmap);
                        var newPage = new clsTexturePage();
                        if ( bitmapResult.Success )
                        {
                            result.Take(BitmapUtil.BitmapIsGlCompatible(bitmap));
                            newPage.GLTexture_Num = BitmapUtil.CreateGLTexture (bitmap, 0);
                        }
                        else
                        {
                            result.WarningAdd(bitmapResult.Problem);
                        }
                        var instrPos2 = text.LastIndexOf(Path.DirectorySeparatorChar);
                        newPage.FileTitle = text.Substring(instrPos2 + 1, text.Length - 5 - instrPos2);

                        TexturePages.Add(newPage);
                    }
                    else
                    {
                        result.WarningAdd("Texture page missing (" + text + ").");
                    }
                    returnResult.Add(result);
                }
            }

            //load PIEs

            string[] pieFiles = null;
            var pieList = new SimpleList<clsPIE>();

            try
            {
                pieFiles = Directory.GetFiles(path + subDirPiEs);
            }
            catch ( Exception )
            {
                returnResult.WarningAdd("Unable to access PIE files.");
                pieFiles = new string[0];
            }

            foreach ( var tempLoopVar_Text in pieFiles )
            {
                text = tempLoopVar_Text;
                var splitPath = new sSplitPath(text);
                if ( splitPath.FileExtension.ToLower() == "pie" )
                {
                    var newPie = new clsPIE {Path = text, LCaseFileTitle = splitPath.FileTitle.ToLower()};
                    pieList.Add(newPie);
                }
            }

            //interpret stats

            clsAttachment attachment;
            clsAttachment baseAttachment;
            Body body;
            Propulsion propulsion;
            Weapon weapon;
            Sensor sensor;
            Ecm ecm;
            string[] fields = null;

            //interpret body

            foreach ( var tempLoopVar_Fields in dataBody.ResultData )
            {
                fields = tempLoopVar_Fields;
                body = new Body();
                body.ObjectDataLink.Connect(Bodies);
                body.Code = fields[0];
                SetComponentName(dataNames.ResultData, body, returnResult);
                IOUtil.InvariantParse(fields[6], ref body.Hitpoints);
                body.Designable = fields[24] != "0";
                body.Attachment.Models.Add(GetModelForPIE(pieList, fields[7].ToLower(), returnResult));
            }

            //interpret propulsion

            foreach ( var tempLoopVar_Fields in dataPropulsion.ResultData )
            {
                fields = tempLoopVar_Fields;
                propulsion = new Propulsion(Bodies.Count);
                propulsion.ObjectDataLink.Connect(Propulsions);
                propulsion.Code = fields[0];
                SetComponentName(dataNames.ResultData, propulsion, returnResult);
                IOUtil.InvariantParse(fields[7], ref propulsion.HitPoints);
                //.Propulsions(Propulsion_Num).PIE = LCase(DataPropulsion.Entries(Propulsion_Num).FieldValues(8))
                propulsion.Designable = fields[11] != "0";
            }

            //interpret body-propulsions

            var bodyPropulsionPIEs = new BodyProp[Bodies.Count, Propulsions.Count];
            for ( var A = 0; A <= Bodies.Count - 1; A++ )
            {
                for ( var B = 0; B <= Propulsions.Count - 1; B++ )
                {
                    bodyPropulsionPIEs[A, B] = new BodyProp();
                    bodyPropulsionPIEs[A, B].LeftPIE = "0";
                    bodyPropulsionPIEs[A, B].RightPIE = "0";
                }
            }

            foreach ( var tempLoopVar_Fields in dataBodyPropulsion.ResultData )
            {
                fields = tempLoopVar_Fields;
                body = FindBodyCode(fields[0]);
                propulsion = FindPropulsionCode(fields[1]);
                if ( body != null && propulsion != null )
                {
                    if ( fields[2] != "0" )
                    {
                        bodyPropulsionPIEs[body.ObjectDataLink.ArrayPosition, propulsion.ObjectDataLink.ArrayPosition].LeftPIE = fields[2].ToLower();
                    }
                    if ( fields[3] != "0" )
                    {
                        bodyPropulsionPIEs[body.ObjectDataLink.ArrayPosition, propulsion.ObjectDataLink.ArrayPosition].RightPIE = fields[3].ToLower();
                    }
                }
            }

            //set propulsion-body PIEs

            for ( var a = 0; a <= Propulsions.Count - 1; a++ )
            {
                propulsion = Propulsions[a];
                for ( var B = 0; B <= Bodies.Count - 1; B++ )
                {
                    body = Bodies[B];
                    propulsion.Bodies[B].LeftAttachment = new clsAttachment();
                    propulsion.Bodies[B].LeftAttachment.Models.Add(GetModelForPIE(pieList, bodyPropulsionPIEs[B, a].LeftPIE, returnResult));
                    propulsion.Bodies[B].RightAttachment = new clsAttachment();
                    propulsion.Bodies[B].RightAttachment.Models.Add(GetModelForPIE(pieList, bodyPropulsionPIEs[B, a].RightPIE, returnResult));
                }
            }

            //interpret construction

            foreach ( var tempLoopVar_Fields in dataConstruction.ResultData )
            {
                fields = tempLoopVar_Fields;
                Construct Construct = new Construct();
                Construct.ObjectDataLink.Connect(Constructors);
                Construct.TurretObjectDataLink.Connect(Turrets);
                Construct.Code = fields[0];
                SetComponentName(dataNames.ResultData, Construct, returnResult);
                Construct.Designable = fields[11] != "0";
                Construct.Attachment.Models.Add(GetModelForPIE(pieList, fields[8].ToLower(), returnResult));
            }

            //interpret weapons

            foreach ( var tempLoopVar_Fields in dataWeapons.ResultData )
            {
                fields = tempLoopVar_Fields;
                weapon = new Weapon();
                weapon.ObjectDataLink.Connect(Weapons);
                weapon.TurretObjectDataLink.Connect(Turrets);
                weapon.Code = fields[0];
                SetComponentName(dataNames.ResultData, weapon, returnResult);
                IOUtil.InvariantParse(fields[7], ref weapon.HitPoints);
                weapon.Designable = fields[51] != "0";
                weapon.Attachment.Models.Add(GetModelForPIE(pieList, Convert.ToString(fields[8].ToLower()), returnResult));
                weapon.Attachment.Models.Add(GetModelForPIE(pieList, fields[9].ToLower(), returnResult));
            }

            //interpret sensor

            foreach ( var tempLoopVar_Fields in dataSensor.ResultData )
            {
                fields = tempLoopVar_Fields;
                sensor = new Sensor();
                sensor.ObjectDataLink.Connect(Sensors);
                sensor.TurretObjectDataLink.Connect(Turrets);
                sensor.Code = fields[0];
                SetComponentName(dataNames.ResultData, sensor, returnResult);
                IOUtil.InvariantParse(fields[7], ref sensor.HitPoints);
                sensor.Designable = fields[15] != "0";
                switch ( fields[11].ToLower() )
                {
                    case "turret":
                        sensor.Location = SensorLocationType.Turret;
                        break;
                    case "default":
                        sensor.Location = SensorLocationType.Invisible;
                        break;
                    default:
                        sensor.Location = SensorLocationType.Invisible;
                        break;
                }
                sensor.Attachment.Models.Add(GetModelForPIE(pieList, fields[8].ToLower(), returnResult));
                sensor.Attachment.Models.Add(GetModelForPIE(pieList, fields[9].ToLower(), returnResult));
            }

            //interpret repair

            foreach ( var tempLoopVar_Fields in dataRepair.ResultData )
            {
                fields = tempLoopVar_Fields;
                Repair Repair = new Repair();
                Repair.ObjectDataLink.Connect(Repairs);
                Repair.TurretObjectDataLink.Connect(Turrets);
                Repair.Code = fields[0];
                SetComponentName(dataNames.ResultData, Repair, returnResult);
                Repair.Designable = fields[13] != "0";
                Repair.Attachment.Models.Add(GetModelForPIE(pieList, fields[9].ToLower(), returnResult));
                Repair.Attachment.Models.Add(GetModelForPIE(pieList, fields[10].ToLower(), returnResult));
            }

            //interpret brain

            foreach ( var tempLoopVar_Fields in dataBrain.ResultData )
            {
                fields = tempLoopVar_Fields;
                Brain Brain = new Brain();
                Brain.ObjectDataLink.Connect(Brains);
                Brain.TurretObjectDataLink.Connect(Turrets);
                Brain.Code = fields[0];
                SetComponentName(dataNames.ResultData, Brain, returnResult);
                Brain.Designable = true;
                weapon = FindWeaponCode(fields[7]);
                if ( weapon != null )
                {
                    Brain.Weapon = weapon;
                    Brain.Attachment = weapon.Attachment;
                }
            }

            //interpret ecm

            foreach ( var tempLoopVar_Fields in dataEcm.ResultData )
            {
                fields = tempLoopVar_Fields;
                ecm = new Ecm();
                ecm.ObjectDataLink.Connect(ECMs);
                ecm.TurretObjectDataLink.Connect(Turrets);
                ecm.Code = fields[0];
                SetComponentName(dataNames.ResultData, ecm, returnResult);
                IOUtil.InvariantParse(fields[7], ref ecm.HitPoints);
                ecm.Designable = false;
                ecm.Attachment.Models.Add(GetModelForPIE(pieList, fields[8].ToLower(), returnResult));
            }

            //interpret feature

            foreach ( var tempLoopVar_Fields in dataFeatures.ResultData )
            {
                fields = tempLoopVar_Fields;
                FeatureTypeBase featureTypeBase = new FeatureTypeBase();
                featureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                featureTypeBase.FeatureType_ObjectDataLink.Connect(FeatureTypes);
                featureTypeBase.Code = fields[0];
                if ( fields[7] == "OIL RESOURCE" ) //type
                {
                    featureTypeBase.FeatureType = FeatureType.OilResource;
                }
                SetFeatureName(dataNames.ResultData, featureTypeBase, returnResult);
                if ( !IOUtil.InvariantParse(fields[1], ref featureTypeBase.Footprint.X) )
                {
                    returnResult.WarningAdd("Feature footprint-x was not an integer for " + featureTypeBase.Code + ".");
                }
                if ( !IOUtil.InvariantParse(fields[2], ref featureTypeBase.Footprint.Y) )
                {
                    returnResult.WarningAdd("Feature footprint-y was not an integer for " + featureTypeBase.Code + ".");
                }
                featureTypeBase.BaseAttachment = new clsAttachment();
                baseAttachment = featureTypeBase.BaseAttachment;
                text = fields[6].ToLower();
                attachment = baseAttachment.CreateAttachment();
                attachment.Models.Add(GetModelForPIE(pieList, text, returnResult));
            }

            //interpret structure

            foreach ( var tempLoopVar_Fields in dataStructures.ResultData )
            {
                fields = tempLoopVar_Fields;
                var structureCode = fields[0];
                var structureTypeText = fields[1];
                var structurePiEs = fields[21].ToLower().Split('@');
                var structureFootprint = new XYInt();
                var structureBasePie = fields[22].ToLower();
                if ( !IOUtil.InvariantParse(fields[5], ref structureFootprint.X) )
                {
                    returnResult.WarningAdd("Structure footprint-x was not an integer for " + structureCode + ".");
                }
                if ( !IOUtil.InvariantParse(fields[6], ref structureFootprint.Y) )
                {
                    returnResult.WarningAdd("Structure footprint-y was not an integer for " + structureCode + ".");
                }
                if ( structureTypeText != "WALL" || structurePiEs.GetLength(0) != 4 )
                {
                    //this is NOT a generic wall
                    StructureTypeBase structureTypeBase = new StructureTypeBase();
                    structureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                    structureTypeBase.StructureType_ObjectDataLink.Connect(StructureTypes);
                    structureTypeBase.Code = structureCode;
                    SetStructureName(dataNames.ResultData, structureTypeBase, returnResult);
                    structureTypeBase.Footprint = structureFootprint;
                    switch ( structureTypeText )
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

                    baseAttachment = structureTypeBase.BaseAttachment;
                    if ( structurePiEs.GetLength(0) > 0 )
                    {
                        baseAttachment.Models.Add(GetModelForPIE(pieList, structurePiEs[0], returnResult));
                    }
                    structureTypeBase.StructureBasePlate = GetModelForPIE(pieList, structureBasePie, returnResult);
                    if ( baseAttachment.Models.Count == 1 )
                    {
                        if ( baseAttachment.Models[0].ConnectorCount >= 1 )
                        {
                            XYZDouble connector = baseAttachment.Models[0].Connectors[0];
                            var StructureWeapons = default(SimpleList<string[]>);
                            StructureWeapons = GetRowsWithValue(dataStructureWeapons.ResultData, structureTypeBase.Code);
                            if ( StructureWeapons.Count > 0 )
                            {
                                weapon = FindWeaponCode(Convert.ToString(StructureWeapons[0][1]));
                            }
                            else
                            {
                                weapon = null;
                            }
                            ecm = FindECMCode(fields[18]);
                            sensor = FindSensorCode(fields[19]);
                            if ( weapon != null )
                            {
                                if ( weapon.Code != "ZNULLWEAPON" )
                                {
                                    attachment = baseAttachment.CopyAttachment(weapon.Attachment);
                                    attachment.PosOffset = connector;
                                }
                            }
                            if ( ecm != null )
                            {
                                if ( ecm.Code != "ZNULLECM" )
                                {
                                    attachment = baseAttachment.CopyAttachment(ecm.Attachment);
                                    attachment.PosOffset = connector;
                                }
                            }
                            if ( sensor != null )
                            {
                                if ( sensor.Code != "ZNULLSENSOR" )
                                {
                                    attachment = baseAttachment.CopyAttachment(sensor.Attachment);
                                    attachment.PosOffset = connector;
                                }
                            }
                        }
                    }
                }
                else
                {
                    //this is a generic wall
                    var newWall = new clsWallType();
                    newWall.WallType_ObjectDataLink.Connect(WallTypes);
                    newWall.Code = structureCode;
                    SetWallName(dataNames.ResultData, newWall, returnResult);
                    var wallBasePlate = GetModelForPIE(pieList, structureBasePie, returnResult);

                    var wallNum = 0;
                    for ( wallNum = 0; wallNum <= 3; wallNum++ )
                    {
                        var wallStructureTypeBase = new StructureTypeBase();
                        wallStructureTypeBase.UnitType_ObjectDataLink.Connect(UnitTypes);
                        wallStructureTypeBase.StructureType_ObjectDataLink.Connect(StructureTypes);
                        wallStructureTypeBase.WallLink.Connect(newWall.Segments);
                        wallStructureTypeBase.Code = structureCode;
                        text = newWall.Name;
                        switch ( wallNum )
                        {
                            case 0:
                                text += " - ";
                                break;
                            case 1:
                                text += " + ";
                                break;
                            case 2:
                                text += " T ";
                                break;
                            case 3:
                                text += " L ";
                                break;
                        }
                        wallStructureTypeBase.Name = text;
                        wallStructureTypeBase.Footprint = structureFootprint;
                        wallStructureTypeBase.StructureType = StructureType.Wall;

                        baseAttachment = wallStructureTypeBase.BaseAttachment;

                        text = structurePiEs[wallNum];
                        baseAttachment.Models.Add(GetModelForPIE(pieList, text, returnResult));
                        wallStructureTypeBase.StructureBasePlate = wallBasePlate;
                    }
                }
            }

            //interpret templates

            var turretConflictCount = 0;
            foreach ( var tempLoopVar_Fields in dataTemplates.ResultData )
            {
                fields = tempLoopVar_Fields;
                var template = new DroidTemplate();
                template.UnitType_ObjectDataLink.Connect(UnitTypes);
                template.DroidTemplate_ObjectDataLink.Connect(DroidTemplates);
                template.Code = fields[0];
                SetTemplateName(dataNames.ResultData, template, returnResult);
                switch ( fields[9] ) //type
                {
                    case "ZNULLDROID":
                        template.TemplateDroidType = App.TemplateDroidType_Null;
                        break;
                    case "DROID":
                        template.TemplateDroidType = App.TemplateDroidType_Droid;
                        break;
                    case "CYBORG":
                        template.TemplateDroidType = App.TemplateDroidType_Cyborg;
                        break;
                    case "CYBORG_CONSTRUCT":
                        template.TemplateDroidType = App.TemplateDroidType_CyborgConstruct;
                        break;
                    case "CYBORG_REPAIR":
                        template.TemplateDroidType = App.TemplateDroidType_CyborgRepair;
                        break;
                    case "CYBORG_SUPER":
                        template.TemplateDroidType = App.TemplateDroidType_CyborgSuper;
                        break;
                    case "TRANSPORTER":
                        template.TemplateDroidType = App.TemplateDroidType_Transporter;
                        break;
                    case "PERSON":
                        template.TemplateDroidType = App.TemplateDroidType_Person;
                        break;
                    default:
                        template.TemplateDroidType = null;
                        returnResult.WarningAdd("Template " + template.GetDisplayTextCode() + " had an unrecognised type.");
                        break;
                }
                var loadPartsArgs = new DroidDesign.sLoadPartsArgs();
                loadPartsArgs.Body = FindBodyCode(fields[2]);
                loadPartsArgs.Brain = FindBrainCode(fields[3]);
                loadPartsArgs.Construct = FindConstructorCode(fields[4]);
                loadPartsArgs.ECM = FindECMCode(fields[5]);
                loadPartsArgs.Propulsion = FindPropulsionCode(fields[7]);
                loadPartsArgs.Repair = FindRepairCode(fields[8]);
                loadPartsArgs.Sensor = FindSensorCode(fields[10]);
                var templateWeapons = GetRowsWithValue(dataAssignWeapons.ResultData, template.Code);
                if ( templateWeapons.Count > 0 )
                {
                    text = Convert.ToString(templateWeapons[0][1]);
                    if ( text != "NULL" )
                    {
                        loadPartsArgs.Weapon1 = FindWeaponCode(text);
                    }
                    text = Convert.ToString(templateWeapons[0][2]);
                    if ( text != "NULL" )
                    {
                        loadPartsArgs.Weapon2 = FindWeaponCode(text);
                    }
                    text = Convert.ToString(templateWeapons[0][3]);
                    if ( text != "NULL" )
                    {
                        loadPartsArgs.Weapon3 = FindWeaponCode(text);
                    }
                }
                if ( !template.LoadParts(loadPartsArgs) )
                {
                    if ( turretConflictCount < 16 )
                    {
                        returnResult.WarningAdd("Template " + template.GetDisplayTextCode() + " had multiple conflicting turrets.");
                    }
                    turretConflictCount++;
                }
            }
            if ( turretConflictCount > 0 )
            {
                returnResult.WarningAdd(turretConflictCount + " templates had multiple conflicting turrets.");
            }

            return returnResult;
        }

        public SimpleList<string[]> GetRowsWithValue(SimpleList<string[]> textLines, string value)
        {
            var result = new SimpleList<string[]>();
            result.AddRange(textLines.Where(line => line[0] == value));
            return result;
        }

        public clsModel GetModelForPIE(SimpleList<clsPIE> pieList, string pieLCaseFileTitle, Result resultOutput)
        {
            if ( pieLCaseFileTitle == "0" )
            {
                return null;
            }

            var a = 0;

            var result = new Result("Loading PIE file " + pieLCaseFileTitle);

            for ( a = 0; a <= pieList.Count - 1; a++ )
            {
                var pie = pieList[a];
                if ( pie.LCaseFileTitle == pieLCaseFileTitle )
                {
                    if ( pie.Model == null )
                    {
                        pie.Model = new clsModel();
                        try
                        {
                            var pieFile = new StreamReader(pie.Path);
                            try
                            {
                                result.Take(pie.Model.ReadPIE(pieFile, this));
                            }
                            catch ( Exception ex )
                            {
                                pieFile.Close();
                                result.WarningAdd(ex.Message);
                                resultOutput.Add(result);
                                return pie.Model;
                            }
                        }
                        catch ( Exception ex )
                        {
                            result.WarningAdd(ex.Message);
                        }
                    }
                    resultOutput.Add(result);
                    return pie.Model;
                }
            }

            if ( !result.HasWarnings )
            {
                result.WarningAdd("file is missing");
            }
            resultOutput.Add(result);

            return null;
        }

        public void SetComponentName(SimpleList<string[]> names, ComponentBase componentBase, Result result)
        {
            var valueSearchResults = GetRowsWithValue(names, componentBase.Code);
            if ( valueSearchResults.Count == 0 )
            {
                result.WarningAdd("No name for component " + componentBase.Code + ".");
            }
            else
            {
                componentBase.Name = Convert.ToString(valueSearchResults[0][1]);
            }
        }

        public void SetFeatureName(SimpleList<string[]> names, FeatureTypeBase featureTypeBase, Result result)
        {
            var valueSearchResults = GetRowsWithValue(names, featureTypeBase.Code);
            if ( valueSearchResults.Count == 0 )
            {
                result.WarningAdd("No name for feature type " + featureTypeBase.Code + ".");
            }
            else
            {
                featureTypeBase.Name = Convert.ToString(valueSearchResults[0][1]);
            }
        }

        public void SetStructureName(SimpleList<string[]> names, StructureTypeBase structureTypeBase, Result result)
        {
            var valueSearchResults = GetRowsWithValue(names, structureTypeBase.Code);
            if ( valueSearchResults.Count == 0 )
            {
                result.WarningAdd("No name for structure type " + structureTypeBase.Code + ".");
            }
            else
            {
                structureTypeBase.Name = Convert.ToString(valueSearchResults[0][1]);
            }
        }

        public void SetTemplateName(SimpleList<string[]> names, DroidTemplate template, Result result)
        {
            var valueSearchResults = GetRowsWithValue(names, template.Code);
            if ( valueSearchResults.Count == 0 )
            {
                result.WarningAdd("No name for droid template " + template.Code + ".");
            }
            else
            {
                template.Name = Convert.ToString(valueSearchResults[0][1]);
            }
        }

        public void SetWallName(SimpleList<string[]> names, clsWallType wallType, Result result)
        {
            var valueSearchResults = GetRowsWithValue(names, wallType.Code);
            if ( valueSearchResults.Count == 0 )
            {
                result.WarningAdd("No name for structure type " + wallType.Code + ".");
            }
            else
            {
                wallType.Name = Convert.ToString(valueSearchResults[0][1]);
            }
        }

        public Body FindBodyCode(string code)
        {
            return Bodies.FirstOrDefault(b => b.Code == code);
        }

        public Propulsion FindPropulsionCode(string code)
        {
            return Propulsions.FirstOrDefault(p => p.Code == code);
        }

        public Construct FindConstructorCode(string code)
        {
            return Constructors.FirstOrDefault(c => c.Code == code);
        }

        public Sensor FindSensorCode(string code)
        {
            return Sensors.FirstOrDefault(s => s.Code == code);
        }

        public Repair FindRepairCode(string code)
        {
            return Repairs.FirstOrDefault(r => r.Code == code);
        }

        public Ecm FindECMCode(string code)
        {
            return ECMs.FirstOrDefault(e => e.Code == code);
        }

        public Brain FindBrainCode(string code)
        {
            return Brains.FirstOrDefault(b => b.Code == code);
        }

        public Weapon FindWeaponCode(string code)
        {
            return Weapons.FirstOrDefault(w => w.Code == code);
        }

        public int Get_TexturePage_GLTexture(string fileTitle)
        {
            var lCaseTitle = fileTitle.ToLower();
            var texPage = TexturePages.FirstOrDefault(t => t.FileTitle.ToLower() == lCaseTitle);
            return texPage != null ? texPage.GLTexture_Num : 0;
        }

        public Weapon FindOrCreateWeapon(string code)
        {
            return FindWeaponCode(code) ?? new Weapon {IsUnknown = true, Code = code};
        }

        public Construct FindOrCreateConstruct(string code)
        {
            return  FindConstructorCode(code) ?? new Construct {IsUnknown = true, Code = code};
        }

        public Repair FindOrCreateRepair(string code)
        {
            return FindRepairCode(code) ?? new Repair {IsUnknown = true, Code = code};
        }

        public Sensor FindOrCreateSensor(string code)
        {
            return FindSensorCode(code) ?? new Sensor {IsUnknown = true, Code = code};
        }

        public Brain FindOrCreateBrain(string code)
        {
            return FindBrainCode(code) ?? new Brain {IsUnknown = true, Code = code};
        }

        public Ecm FindOrCreateECM(string code)
        {
            return FindECMCode(code) ?? new Ecm {IsUnknown = true, Code = code};
        }

        public Turret FindOrCreateTurret(TurretType turretType, string turretCode)
        {
            switch ( turretType )
            {
                case TurretType.Weapon:
                    return FindOrCreateWeapon(turretCode);
                case TurretType.Construct:
                    return FindOrCreateConstruct(turretCode);
                case TurretType.Repair:
                    return FindOrCreateRepair(turretCode);
                case TurretType.Sensor:
                    return FindOrCreateSensor(turretCode);
                case TurretType.Brain:
                    return FindOrCreateBrain(turretCode);
                case TurretType.ECM:
                    return FindOrCreateECM(turretCode);
                default:
                    return null;
            }
        }

        public Body FindOrCreateBody(string code)
        {
            return FindBodyCode(code) ?? new Body {IsUnknown = true, Code = code};
        }

        public Propulsion FindOrCreatePropulsion(string code)
        {
            return FindPropulsionCode(code) ?? new Propulsion(Bodies.Count) {IsUnknown = true, Code = code};
        }

        public UnitTypeBase FindOrCreateUnitType(string code, UnitType type, int wallType)
        {
            switch ( type )
            {
                case UnitType.Feature:
                    return FeatureTypes.FirstOrDefault(ft => ft.Code == code)
                           ?? new FeatureTypeBase
                               {
                                   IsUnknown = true,
                                   Code = code,
                                   Footprint = {X = 1, Y = 1}
                               };
                case UnitType.PlayerStructure:
                    foreach ( var structure in StructureTypes.Where(s => s.Code == code) )
                    {
                        if ( wallType < 0 )
                        {
                            return structure;
                        }
                        if ( structure.WallLink.IsConnected )
                        {
                            if ( structure.WallLink.ArrayPosition == wallType )
                            {
                                return structure;
                            }
                        }
                    }
                    return new StructureTypeBase {IsUnknown = true, Code = code, Footprint = {X = 1, Y = 1}};

                case UnitType.PlayerDroid:
                    return DroidTemplates
                        .Where(dt => dt.IsTemplate)
                        .FirstOrDefault(dt => dt.Code == code)
                           ?? new DroidTemplate
                               {
                                   IsUnknown = true,
                                   Code = code
                               };
                default:
                    return null;
            }
        }

        public StructureTypeBase FindFirstStructureType(StructureType type)
        {
            return StructureTypes.FirstOrDefault(s => s.StructureType == type);
        }
    }
}