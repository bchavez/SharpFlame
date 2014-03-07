#region

using System;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Domain.ObjData;
using SharpFlame.Maths;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Domain
{
    public abstract class UnitTypeBase
    {
        public readonly ConnectedListLink<UnitTypeBase, ObjectData> UnitType_ObjectDataLink;

        public readonly ConnectedListLink<UnitTypeBase, frmMain> UnitType_frmMainSelectedLink;

        public bool IsUnknown = false;

        public UnitType Type;

        protected UnitTypeBase()
        {
            UnitType_frmMainSelectedLink = new ConnectedListLink<UnitTypeBase, frmMain>(this);
            UnitType_ObjectDataLink = new ConnectedListLink<UnitTypeBase, ObjectData>(this);
        }

        public XYInt GetFootprintOld
        {
            get
            {
                switch ( Type )
                {
                    case UnitType.Feature:
                        return ((FeatureTypeBase)this).Footprint;
                    case UnitType.PlayerStructure:
                        return ((StructureTypeBase)this).Footprint;
                    default:
                        var XY_int = new XYInt(1, 1);
                        return XY_int;
                }
            }
        }

        public void GLDraw(float RotationDegrees)
        {
            switch ( App.Draw_Lighting )
            {
                case DrawLighting.Off:
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    break;
                case DrawLighting.Half:
                    GL.Color3(0.875F, 0.875F, 0.875F);
                    break;
                case DrawLighting.Normal:
                    GL.Color3(0.75F, 0.75F, 0.75F);
                    break;
            }
            //GL.Rotate(x, 1.0F, 0.0F, 0.0F)
            GL.Rotate(RotationDegrees, 0.0F, 1.0F, 0.0F);
            //GL.Rotate(z, 0.0F, 0.0F, -1.0F)

            TypeGLDraw();
        }

        protected virtual void TypeGLDraw()
        {
        }

        public XYInt GetGetFootprintNew(int Rotation)
        {
            //get initial footprint
            XYInt result;
            switch ( Type )
            {
                case UnitType.Feature:
                    result = ((FeatureTypeBase)this).Footprint;
                    break;
                case UnitType.PlayerStructure:
                    result = ((StructureTypeBase)this).Footprint;
                    break;
                default:
                    //return droid footprint
                    result = new XYInt(1, 1);
                    return result;
            }
            //switch footprint axes if not a droid
            var Remainder = Convert.ToDouble((Rotation / 90.0D + 0.5D) % 2.0D);
            if ( Remainder < 0.0D )
            {
                Remainder += 2.0D;
            }
            if ( Remainder >= 1.0D )
            {
                var X = result.X;
                result.X = result.Y;
                result.Y = X;
            }
            return result;
        }

        public XYInt GetGetFootprintSelected(int rotation)
        {
//            if ( Program.frmMainInstance.cbxFootprintRotate.Checked )
//            {
//                return GetGetFootprintNew(rotation);
//            }
            return GetFootprintOld;
        }

        public bool GetCode(ref string Result)
        {
            switch ( Type )
            {
                case UnitType.Feature:
                    Result = ((FeatureTypeBase)this).Code;
                    return true;
                case UnitType.PlayerStructure:
                    Result = ((StructureTypeBase)this).Code;
                    return true;
                case UnitType.PlayerDroid:
                    var Droid = (DroidDesign)this;
                    if ( Droid.IsTemplate )
                    {
                        Result = ((DroidTemplate)this).Code;
                        return true;
                    }
                    Result = null;
                    return false;
                default:
                    Result = null;
                    return false;
            }
        }

        public string GetDisplayTextCode()
        {
            switch ( Type )
            {
                case UnitType.Feature:
                    var featureTypeBase = (FeatureTypeBase)this;
                    return featureTypeBase.Code + " (" + featureTypeBase.Name + ")";
                case UnitType.PlayerStructure:
                    var structureTypeBase = (StructureTypeBase)this;
                    return structureTypeBase.Code + " (" + structureTypeBase.Name + ")";
                case UnitType.PlayerDroid:
                    var DroidType = (DroidDesign)this;
                    if ( DroidType.IsTemplate )
                    {
                        var Template = (DroidTemplate)this;
                        return Template.Code + " (" + Template.Name + ")";
                    }
                    return "<droid> (" + DroidType.GenerateName() + ")";
                default:
                    return "";
            }
        }

        public string GetDisplayTextName()
        {
            switch ( Type )
            {
                case UnitType.Feature:
                    var featureTypeBase = (FeatureTypeBase)this;
                    return featureTypeBase.Name + " (" + featureTypeBase.Code + ")";
                case UnitType.PlayerStructure:
                    var structureTypeBase = (StructureTypeBase)this;
                    return structureTypeBase.Name + " (" + structureTypeBase.Code + ")";
                case UnitType.PlayerDroid:
                    var DroidType = (DroidDesign)this;
                    if ( DroidType.IsTemplate )
                    {
                        var Template = (DroidTemplate)this;
                        return Template.Name + " (" + Template.Code + ")";
                    }
                    return DroidType.GenerateName() + " (<droid>)";
                default:
                    return "";
            }
        }

        public virtual string GetName()
        {
            return "";
        }
    }

    public class clsAttachment
    {
        public Matrix3DMath.Matrix3D AngleOffsetMatrix = new Matrix3DMath.Matrix3D();
        public SimpleClassList<clsAttachment> Attachments = new SimpleClassList<clsAttachment>();
        public SimpleClassList<clsModel> Models = new SimpleClassList<clsModel>();
        public XYZDouble PosOffset;

        public clsAttachment()
        {
            Models.AddNullItemBehavior = AddNullItemBehavior.DisallowIgnore;
            Matrix3DMath.MatrixSetToIdentity(AngleOffsetMatrix);
        }

        public void GLDraw()
        {
            var angleRpy = default(Angles.AngleRPY);
            var matrixA = new Matrix3DMath.Matrix3D();

            foreach ( var model in Models )
            {
                model.GLDraw();
            }

            foreach ( var attachment in Attachments )
            {
                GL.PushMatrix();
                Matrix3DMath.MatrixInvert(attachment.AngleOffsetMatrix, matrixA);
                Matrix3DMath.MatrixToRPY(matrixA, ref angleRpy);
                GL.Translate(attachment.PosOffset.X, attachment.PosOffset.Y, Convert.ToDouble(- attachment.PosOffset.Z));
                GL.Rotate((float)(angleRpy.Roll / MathUtil.RadOf1Deg), 0.0F, 0.0F, -1.0F);
                GL.Rotate((float)(angleRpy.Pitch / MathUtil.RadOf1Deg), 1.0F, 0.0F, 0.0F);
                GL.Rotate((float)(angleRpy.Yaw / MathUtil.RadOf1Deg), 0.0F, 1.0F, 0.0F);
                attachment.GLDraw();
                GL.PopMatrix();
            }
        }

        public clsAttachment CreateAttachment()
        {
            var result = new clsAttachment();

            Attachments.Add(result);
            return result;
        }

        public clsAttachment CopyAttachment(clsAttachment Other)
        {
            var result = new clsAttachment
                {
                    PosOffset = Other.PosOffset
                };

            Attachments.Add(result);
            Matrix3DMath.MatrixCopy(Other.AngleOffsetMatrix, result.AngleOffsetMatrix);
            result.Models.AddRange(Other.Models);
            result.Attachments.AddRange(Other.Attachments);

            return result;
        }

        public clsAttachment AddCopyOfAttachment(clsAttachment AttachmentToCopy)
        {
            var ResultAttachment = new clsAttachment();
            var Attachment = default(clsAttachment);

            Attachments.Add(ResultAttachment);
            Matrix3DMath.MatrixCopy(AttachmentToCopy.AngleOffsetMatrix, ResultAttachment.AngleOffsetMatrix);
            ResultAttachment.Models.AddRange(AttachmentToCopy.Models);
            foreach ( var tempLoopVar_Attachment in AttachmentToCopy.Attachments )
            {
                Attachment = tempLoopVar_Attachment;
                ResultAttachment.AddCopyOfAttachment(Attachment);
            }

            return ResultAttachment;
        }
    }

    public enum UnitType
    {
        Unspecified,
        Feature,
        PlayerStructure,
        PlayerDroid
    }

    public class FeatureTypeBase : UnitTypeBase
    {
        public clsAttachment BaseAttachment;
        public string Code = "";
        public FeatureType FeatureType = FeatureType.Unknown;
        public ConnectedListLink<FeatureTypeBase, ObjectData> FeatureType_ObjectDataLink;
        public XYInt Footprint;
        public string Name = "Unknown";

        public FeatureTypeBase()
        {
            Footprint = new XYInt(0, 0);
            FeatureType_ObjectDataLink = new ConnectedListLink<FeatureTypeBase, ObjectData>(this);


            Type = UnitType.Feature;
        }

        protected override void TypeGLDraw()
        {
            if ( BaseAttachment != null )
            {
                BaseAttachment.GLDraw();
            }
        }

        public override string GetName()
        {
            return Name;
        }
    }

    public enum FeatureType
    {
        Unknown,
        OilResource
    }

    public class StructureTypeBase : UnitTypeBase
    {
        public clsAttachment BaseAttachment = new clsAttachment();

        public string Code = "";
        public XYInt Footprint;
        public string Name = "Unknown";
        public clsModel StructureBasePlate;

        public StructureType StructureType = StructureType.Unknown;
        public ConnectedListLink<StructureTypeBase, ObjectData> StructureType_ObjectDataLink;

        public ConnectedListLink<StructureTypeBase, clsWallType> WallLink;

        public StructureTypeBase()
        {
            StructureType_ObjectDataLink = new ConnectedListLink<StructureTypeBase, ObjectData>(this);
            WallLink = new ConnectedListLink<StructureTypeBase, clsWallType>(this);


            Type = UnitType.PlayerStructure;
        }

        protected override void TypeGLDraw()
        {
            if ( BaseAttachment != null )
            {
                BaseAttachment.GLDraw();
            }
            if ( StructureBasePlate != null )
            {
                StructureBasePlate.GLDraw();
            }
        }

        public bool IsModule()
        {
            return StructureType == StructureType.FactoryModule
                   | StructureType == StructureType.PowerModule
                   | StructureType == StructureType.ResearchModule;
        }

        public override string GetName()
        {
            return Name;
        }
    }

    public class DroidDesign : UnitTypeBase
    {
        public bool AlwaysDrawTextLabel;
        public clsAttachment BaseAttachment = new clsAttachment();
        public Body Body;
        public bool IsTemplate;

        public string Name = "";

        public Propulsion Propulsion;
        public clsTemplateDroidType TemplateDroidType;
        public Turret Turret1;
        public Turret Turret2;
        public Turret Turret3;
        public byte TurretCount;

        public DroidDesign()
        {
            Type = UnitType.PlayerDroid;
        }

        public void CopyDesign(DroidDesign droidTypeToCopy)
        {
            TemplateDroidType = droidTypeToCopy.TemplateDroidType;
            Body = droidTypeToCopy.Body;
            Propulsion = droidTypeToCopy.Propulsion;
            TurretCount = droidTypeToCopy.TurretCount;
            Turret1 = droidTypeToCopy.Turret1;
            Turret2 = droidTypeToCopy.Turret2;
            Turret3 = droidTypeToCopy.Turret3;
        }

        protected override void TypeGLDraw()
        {
            if ( BaseAttachment != null )
            {
                BaseAttachment.GLDraw();
            }
        }

        public void UpdateAttachments()
        {
            BaseAttachment = new clsAttachment();

            if ( Body == null )
            {
                AlwaysDrawTextLabel = true;
                return;
            }

            var NewBody = BaseAttachment.AddCopyOfAttachment(Body.Attachment);

            AlwaysDrawTextLabel = NewBody.Models.Count == 0;

            if ( Propulsion != null )
            {
                if ( Body.ObjectDataLink.IsConnected )
                {
                    BaseAttachment.AddCopyOfAttachment(Propulsion.Bodies[Body.ObjectDataLink.ArrayPosition].LeftAttachment);
                    BaseAttachment.AddCopyOfAttachment(Propulsion.Bodies[Body.ObjectDataLink.ArrayPosition].RightAttachment);
                }
            }

            if ( NewBody.Models.Count == 0 )
            {
                return;
            }

            if ( NewBody.Models[0].ConnectorCount <= 0 )
            {
                return;
            }

            var turretConnector = Body.Attachment.Models[0].Connectors[0];

            if ( TurretCount >= 1 )
            {
                if ( Turret1 != null )
                {
                    var newTurret = NewBody.AddCopyOfAttachment(Turret1.Attachment);
                    newTurret.PosOffset = turretConnector;
                }
            }

            if ( Body.Attachment.Models[0].ConnectorCount <= 1 )
            {
                return;
            }

            turretConnector = Body.Attachment.Models[0].Connectors[1];

            if ( TurretCount >= 2 )
            {
                if ( Turret2 != null )
                {
                    var newTurret = NewBody.AddCopyOfAttachment(Turret2.Attachment);
                    newTurret.PosOffset = turretConnector;
                }
            }
        }

        public int GetMaxHitPoints()
        {
            var result = 0;

            //this is inaccurate

            if ( Body == null )
            {
                return 0;
            }
            result = Body.Hitpoints;
            if ( Propulsion == null )
            {
                return result;
            }
            result += (int)(Body.Hitpoints * Propulsion.HitPoints / 100.0D);
            if ( Turret1 == null )
            {
                return result;
            }
            result += Body.Hitpoints + Turret1.HitPoints;
            if ( TurretCount < 2 || Turret2 == null )
            {
                return result;
            }
            if ( Turret2.TurretType != TurretType.Weapon )
            {
                return result;
            }
            result += Body.Hitpoints + Turret2.HitPoints;
            if ( TurretCount < 3 || Turret3 == null )
            {
                return result;
            }
            if ( Turret3.TurretType != TurretType.Weapon )
            {
                return result;
            }
            result += Body.Hitpoints + Turret3.HitPoints;
            return result;
        }

        public bool LoadParts(sLoadPartsArgs Args)
        {
            var TurretConflict = default(bool);

            Body = Args.Body;
            Propulsion = Args.Propulsion;

            TurretConflict = false;
            if ( Args.Construct != null )
            {
                if ( Args.Construct.Code != "ZNULLCONSTRUCT" )
                {
                    if ( Turret1 != null )
                    {
                        TurretConflict = true;
                    }
                    TurretCount = 1;
                    Turret1 = Args.Construct;
                }
            }
            if ( Args.Repair != null )
            {
                if ( Args.Repair.Code != "ZNULLREPAIR" )
                {
                    if ( Turret1 != null )
                    {
                        TurretConflict = true;
                    }
                    TurretCount = 1;
                    Turret1 = Args.Repair;
                }
            }
            if ( Args.Brain != null )
            {
                if ( Args.Brain.Code != "ZNULLBRAIN" )
                {
                    if ( Turret1 != null )
                    {
                        TurretConflict = true;
                    }
                    TurretCount = 1;
                    Turret1 = Args.Brain;
                }
            }
            if ( Args.Weapon1 != null )
            {
                var UseWeapon = default(bool);
                if ( Turret1 != null )
                {
                    if ( Turret1.TurretType == TurretType.Brain )
                    {
                        UseWeapon = false;
                    }
                    else
                    {
                        UseWeapon = true;
                        TurretConflict = true;
                    }
                }
                else
                {
                    UseWeapon = true;
                }
                if ( UseWeapon )
                {
                    TurretCount = 1;
                    Turret1 = Args.Weapon1;
                    if ( Args.Weapon2 != null )
                    {
                        Turret2 = Args.Weapon2;
                        TurretCount += 1;
                        if ( Args.Weapon3 != null )
                        {
                            Turret3 = Args.Weapon3;
                            TurretCount += 1;
                        }
                    }
                }
            }
            if ( Args.Sensor != null )
            {
                if ( Args.Sensor.Location == SensorLocationType.Turret )
                {
                    if ( Turret1 != null )
                    {
                        TurretConflict = true;
                    }
                    TurretCount = 1;
                    Turret1 = Args.Sensor;
                }
            }
            UpdateAttachments();

            return !TurretConflict; //return if all is ok
        }

        public string GenerateName()
        {
            var Result = "";

            if ( Propulsion != null )
            {
                if ( Result.Length > 0 )
                {
                    Result = ' ' + Result;
                }
                Result = Propulsion.Name + Result;
            }

            if ( Body != null )
            {
                if ( Result.Length > 0 )
                {
                    Result = ' ' + Result;
                }
                Result = Body.Name + Result;
            }

            if ( TurretCount >= 3 )
            {
                if ( Turret3 != null )
                {
                    if ( Result.Length > 0 )
                    {
                        Result = ' ' + Result;
                    }
                    Result = Turret3.Name + Result;
                }
            }

            if ( TurretCount >= 2 )
            {
                if ( Turret2 != null )
                {
                    if ( Result.Length > 0 )
                    {
                        Result = ' ' + Result;
                    }
                    Result = Turret2.Name + Result;
                }
            }

            if ( TurretCount >= 1 )
            {
                if ( Turret1 != null )
                {
                    if ( Result.Length > 0 )
                    {
                        Result = ' ' + Result;
                    }
                    Result = Turret1.Name + Result;
                }
            }

            return Result;
        }

        public DroidType GetDroidType()
        {
            var Result = default(DroidType);

            if ( TemplateDroidType == App.TemplateDroidType_Null )
            {
                Result = DroidType.Default;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_Person )
            {
                Result = DroidType.Person;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_Cyborg )
            {
                Result = DroidType.Cyborg;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_CyborgSuper )
            {
                Result = DroidType.CyborgSuper;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_CyborgConstruct )
            {
                Result = DroidType.CyborgConstruct;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_CyborgRepair )
            {
                Result = DroidType.CyborgRepair;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_Transporter )
            {
                Result = DroidType.Transporter;
            }
            else if ( Turret1 == null )
            {
                Result = DroidType.Default;
            }
            else if ( Turret1.TurretType == TurretType.Brain )
            {
                Result = DroidType.Command;
            }
            else if ( Turret1.TurretType == TurretType.Sensor )
            {
                Result = DroidType.Sensor;
            }
            else if ( Turret1.TurretType == TurretType.ECM )
            {
                Result = DroidType.ECM;
            }
            else if ( Turret1.TurretType == TurretType.Construct )
            {
                Result = DroidType.Construct;
            }
            else if ( Turret1.TurretType == TurretType.Repair )
            {
                Result = DroidType.Repair;
            }
            else if ( Turret1.TurretType == TurretType.Weapon )
            {
                Result = DroidType.Weapon;
            }
            else
            {
                Result = DroidType.Default;
            }
            return Result;
        }

        public bool SetDroidType(DroidType DroidType)
        {
            switch ( DroidType )
            {
                case DroidType.Weapon:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case DroidType.Sensor:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case DroidType.ECM:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case DroidType.Construct:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case DroidType.Person:
                    TemplateDroidType = App.TemplateDroidType_Person;
                    break;
                case DroidType.Cyborg:
                    TemplateDroidType = App.TemplateDroidType_Cyborg;
                    break;
                case DroidType.Transporter:
                    TemplateDroidType = App.TemplateDroidType_Transporter;
                    break;
                case DroidType.Command:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case DroidType.Repair:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case DroidType.Default:
                    TemplateDroidType = App.TemplateDroidType_Null;
                    break;
                case DroidType.CyborgConstruct:
                    TemplateDroidType = App.TemplateDroidType_CyborgConstruct;
                    break;
                case DroidType.CyborgRepair:
                    TemplateDroidType = App.TemplateDroidType_CyborgRepair;
                    break;
                case DroidType.CyborgSuper:
                    TemplateDroidType = App.TemplateDroidType_CyborgSuper;
                    break;
                default:
                    TemplateDroidType = null;
                    return false;
            }
            return true;
        }

        public string GetConstructCode()
        {
            var NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != TurretType.Construct )
                {
                    NotThis = true;
                }
                else
                {
                    NotThis = false;
                }
            }
            else
            {
                NotThis = true;
            }

            if ( NotThis )
            {
                return "ZNULLCONSTRUCT";
            }
            return Turret1.Code;
        }

        public string GetRepairCode()
        {
            var NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != TurretType.Repair )
                {
                    NotThis = true;
                }
                else
                {
                    NotThis = false;
                }
            }
            else
            {
                NotThis = true;
            }

            if ( NotThis )
            {
                return "ZNULLREPAIR";
            }
            return Turret1.Code;
        }

        public string GetSensorCode()
        {
            var NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != TurretType.Sensor )
                {
                    NotThis = true;
                }
                else
                {
                    NotThis = false;
                }
            }
            else
            {
                NotThis = true;
            }

            if ( NotThis )
            {
                return "ZNULLSENSOR";
            }
            return Turret1.Code;
        }

        public string GetBrainCode()
        {
            var NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != TurretType.Brain )
                {
                    NotThis = true;
                }
                else
                {
                    NotThis = false;
                }
            }
            else
            {
                NotThis = true;
            }

            if ( NotThis )
            {
                return "ZNULLBRAIN";
            }
            return Turret1.Code;
        }

        public string GetECMCode()
        {
            var NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != TurretType.ECM )
                {
                    NotThis = true;
                }
                else
                {
                    NotThis = false;
                }
            }
            else
            {
                NotThis = true;
            }

            if ( NotThis )
            {
                return "ZNULLECM";
            }
            return Turret1.Code;
        }

        public override string GetName()
        {
            return Name;
        }

        public class clsTemplateDroidType
        {
            public string Name;
            public int Num = -1;

            public string TemplateCode;

            public clsTemplateDroidType(string NewName, string NewTemplateCode)
            {
                Name = NewName;
                TemplateCode = NewTemplateCode;
            }
        }

        public struct sLoadPartsArgs
        {
            public Body Body;
            public Brain Brain;
            public Construct Construct;
            public Ecm ECM;
            public Propulsion Propulsion;
            public Repair Repair;
            public Sensor Sensor;
            public Weapon Weapon1;
            public Weapon Weapon2;
            public Weapon Weapon3;
        }
    }

    public class DroidTemplate : DroidDesign
    {
        public string Code = "";
        public ConnectedListLink<DroidTemplate, ObjectData> DroidTemplate_ObjectDataLink;

        public DroidTemplate()
        {
            DroidTemplate_ObjectDataLink = new ConnectedListLink<DroidTemplate, ObjectData>(this);


            IsTemplate = true;
            Name = "Unknown";
        }
    }

    public class clsWallType
    {
        private const int d0 = 0;
        private const int d1 = 90;
        private const int d2 = 180;
        private const int d3 = 270;
        public string Code = "";
        public string Name = "Unknown";

        public ConnectedList<StructureTypeBase, clsWallType> Segments;
        public int[] TileWalls_Direction = {d0, d0, d2, d0, d3, d0, d3, d0, d1, d1, d2, d2, d3, d1, d3, d0};
        public int[] TileWalls_Segment = {0, 0, 0, 0, 0, 3, 3, 2, 0, 3, 3, 2, 0, 2, 2, 1};
        public ConnectedListLink<clsWallType, ObjectData> WallType_ObjectDataLink;

        public clsWallType()
        {
            WallType_ObjectDataLink = new ConnectedListLink<clsWallType, ObjectData>(this);
            Segments = new ConnectedList<StructureTypeBase, clsWallType>(this);


            Segments.MaintainOrder = true;
        }
    }
}