using System;
using Matrix3D;
using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Maths;

namespace SharpFlame.Domain
{
    public abstract class UnitTypeBase
    {
        public readonly ConnectedListLink<UnitTypeBase, clsObjectData> UnitType_ObjectDataLink;

        public readonly ConnectedListLink<UnitTypeBase, frmMain> UnitType_frmMainSelectedLink;

        public bool IsUnknown = false;

        public UnitType Type;

        public UnitTypeBase()
        {
            UnitType_frmMainSelectedLink = new ConnectedListLink<UnitTypeBase, frmMain>(this);
            UnitType_ObjectDataLink = new ConnectedListLink<UnitTypeBase, clsObjectData>(this);
        }

        public void GLDraw(float RotationDegrees)
        {
            switch ( App.Draw_Lighting )
            {
                case App.enumDrawLighting.Off:
                    GL.Color3(1.0F, 1.0F, 1.0F);
                    break;
                case App.enumDrawLighting.Half:
                    GL.Color3(0.875F, 0.875F, 0.875F);
                    break;
                case App.enumDrawLighting.Normal:
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

        public sXY_int GetFootprintOld
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
                        sXY_int XY_int = new sXY_int(1, 1);
                        return XY_int;
                }
            }
        }

        public sXY_int get_GetFootprintNew(int Rotation)
        {
            //get initial footprint
            sXY_int Result = new sXY_int();
            switch ( Type )
            {
                case UnitType.Feature:
                    Result = ((FeatureTypeBase)this).Footprint;
                    break;
                case UnitType.PlayerStructure:
                    Result = ((StructureTypeBase)this).Footprint;
                    break;
                default:
                    //return droid footprint
                    Result = new sXY_int(1, 1);
                    return Result;
            }
            //switch footprint axes if not a droid
            double Remainder = Convert.ToDouble((Rotation / 90.0D + 0.5D) % 2.0D);
            if ( Remainder < 0.0D )
            {
                Remainder += 2.0D;
            }
            if ( Remainder >= 1.0D )
            {
                int X = Result.X;
                Result.X = Result.Y;
                Result.Y = X;
            }
            return Result;
        }

        public sXY_int get_GetFootprintSelected(int Rotation)
        {
            if ( Program.frmMainInstance.cbxFootprintRotate.Checked )
            {
                return get_GetFootprintNew(Rotation);
            }
            else
            {
                return GetFootprintOld;
            }
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
                    DroidDesign Droid = (DroidDesign)this;
                    if ( Droid.IsTemplate )
                    {
                        Result = ((DroidTemplate)this).Code;
                        return true;
                    }
                    else
                    {
                        Result = null;
                        return false;
                    }
                    break;
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
                    FeatureTypeBase featureTypeBase = (FeatureTypeBase)this;
                    return featureTypeBase.Code + " (" + featureTypeBase.Name + ")";
                case UnitType.PlayerStructure:
                    StructureTypeBase structureTypeBase = (StructureTypeBase)this;
                    return structureTypeBase.Code + " (" + structureTypeBase.Name + ")";
                case UnitType.PlayerDroid:
                    DroidDesign DroidType = (DroidDesign)this;
                    if ( DroidType.IsTemplate )
                    {
                        DroidTemplate Template = (DroidTemplate)this;
                        return Template.Code + " (" + Template.Name + ")";
                    }
                    else
                    {
                        return "<droid> (" + DroidType.GenerateName() + ")";
                    }
                    break;
                default:
                    return "";
            }
        }

        public string GetDisplayTextName()
        {
            switch ( Type )
            {
                case UnitType.Feature:
                    FeatureTypeBase featureTypeBase = (FeatureTypeBase)this;
                    return featureTypeBase.Name + " (" + featureTypeBase.Code + ")";
                case UnitType.PlayerStructure:
                    StructureTypeBase structureTypeBase = (StructureTypeBase)this;
                    return structureTypeBase.Name + " (" + structureTypeBase.Code + ")";
                case UnitType.PlayerDroid:
                    DroidDesign DroidType = (DroidDesign)this;
                    if ( DroidType.IsTemplate )
                    {
                        DroidTemplate Template = (DroidTemplate)this;
                        return Template.Name + " (" + Template.Code + ")";
                    }
                    else
                    {
                        return DroidType.GenerateName() + " (<droid>)";
                    }
                    break;
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
        public Position.XYZ_dbl Pos_Offset;
        public Matrix3DMath.Matrix3D AngleOffsetMatrix = new Matrix3DMath.Matrix3D();
        public SimpleClassList<clsModel> Models = new SimpleClassList<clsModel>();
        public SimpleClassList<clsAttachment> Attachments = new SimpleClassList<clsAttachment>();

        public clsAttachment()
        {
            Models.AddNullItemBehavior = AddNullItemBehavior.DisallowIgnore;
            Matrix3DMath.MatrixSetToIdentity(AngleOffsetMatrix);
        }

        public void GLDraw()
        {
            Angles.AngleRPY AngleRPY = default(Angles.AngleRPY);
            Matrix3DMath.Matrix3D matrixA = new Matrix3DMath.Matrix3D();
            clsAttachment Attachment = default(clsAttachment);
            clsModel Model = default(clsModel);

            foreach ( clsModel tempLoopVar_Model in Models )
            {
                Model = tempLoopVar_Model;
                Model.GLDraw();
            }

            foreach ( clsAttachment tempLoopVar_Attachment in Attachments )
            {
                Attachment = tempLoopVar_Attachment;
                GL.PushMatrix();
                Matrix3DMath.MatrixInvert(Attachment.AngleOffsetMatrix, matrixA);
                Matrix3DMath.MatrixToRPY(matrixA, ref AngleRPY);
                GL.Translate(Attachment.Pos_Offset.X, Attachment.Pos_Offset.Y, Convert.ToDouble(- Attachment.Pos_Offset.Z));
                GL.Rotate((float)(AngleRPY.Roll / MathUtil.RadOf1Deg), 0.0F, 0.0F, -1.0F);
                GL.Rotate((float)(AngleRPY.Pitch / MathUtil.RadOf1Deg), 1.0F, 0.0F, 0.0F);
                GL.Rotate((float)(AngleRPY.Yaw / MathUtil.RadOf1Deg), 0.0F, 1.0F, 0.0F);
                Attachment.GLDraw();
                GL.PopMatrix();
            }
        }

        public clsAttachment CreateAttachment()
        {
            clsAttachment Result = new clsAttachment();

            Attachments.Add(Result);
            return Result;
        }

        public clsAttachment CopyAttachment(clsAttachment Other)
        {
            clsAttachment Result = new clsAttachment();

            Result.Pos_Offset = Other.Pos_Offset;
            Attachments.Add(Result);
            Matrix3DMath.MatrixCopy(Other.AngleOffsetMatrix, Result.AngleOffsetMatrix);
            Result.Models.AddSimpleList(Other.Models);
            Result.Attachments.AddSimpleList(Other.Attachments);

            return Result;
        }

        public clsAttachment AddCopyOfAttachment(clsAttachment AttachmentToCopy)
        {
            clsAttachment ResultAttachment = new clsAttachment();
            clsAttachment Attachment = default(clsAttachment);

            Attachments.Add(ResultAttachment);
            Matrix3DMath.MatrixCopy(AttachmentToCopy.AngleOffsetMatrix, ResultAttachment.AngleOffsetMatrix);
            ResultAttachment.Models.AddSimpleList(AttachmentToCopy.Models);
            foreach ( clsAttachment tempLoopVar_Attachment in AttachmentToCopy.Attachments )
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
        public ConnectedListLink<FeatureTypeBase, clsObjectData> FeatureType_ObjectDataLink;

        public string Code = "";
        public string Name = "Unknown";
        public sXY_int Footprint;

        public enum enumFeatureType
        {
            Unknown,
            OilResource
        }

        public enumFeatureType FeatureType = enumFeatureType.Unknown;

        public clsAttachment BaseAttachment;

        public FeatureTypeBase()
        {
            FeatureType_ObjectDataLink = new ConnectedListLink<FeatureTypeBase, clsObjectData>(this);


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

    public class StructureTypeBase : UnitTypeBase
    {
        public ConnectedListLink<StructureTypeBase, clsObjectData> StructureType_ObjectDataLink;

        public string Code = "";
        public string Name = "Unknown";
        public sXY_int Footprint;

        public enum enumStructureType
        {
            Unknown,
            Demolish,
            Wall,
            CornerWall,
            Factory,
            CyborgFactory,
            VTOLFactory,
            Command,
            HQ,
            Defense,
            PowerGenerator,
            PowerModule,
            Research,
            ResearchModule,
            FactoryModule,
            DOOR,
            RepairFacility,
            SatUplink,
            RearmPad,
            MissileSilo,
            ResourceExtractor
        }

        public enumStructureType StructureType = enumStructureType.Unknown;

        public ConnectedListLink<StructureTypeBase, clsWallType> WallLink;

        public clsAttachment BaseAttachment = new clsAttachment();
        public clsModel StructureBasePlate;

        public StructureTypeBase()
        {
            StructureType_ObjectDataLink = new ConnectedListLink<StructureTypeBase, clsObjectData>(this);
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
            return StructureType == enumStructureType.FactoryModule
                   | StructureType == enumStructureType.PowerModule
                   | StructureType == enumStructureType.ResearchModule;
        }

        public override string GetName()
        {
            return Name;
        }
    }

    public class DroidDesign : UnitTypeBase
    {
        public bool IsTemplate;

        public string Name = "";

        public class clsTemplateDroidType
        {
            public int Num = -1;

            public string Name;

            public string TemplateCode;

            public clsTemplateDroidType(string NewName, string NewTemplateCode)
            {
                Name = NewName;
                TemplateCode = NewTemplateCode;
            }
        }

        public clsTemplateDroidType TemplateDroidType;

        public Body Body;
        public Propulsion Propulsion;
        public byte TurretCount;
        public Turret Turret1;
        public Turret Turret2;
        public Turret Turret3;

        public clsAttachment BaseAttachment = new clsAttachment();

        public bool AlwaysDrawTextLabel;

        public DroidDesign()
        {
            Type = UnitType.PlayerDroid;
        }

        public void CopyDesign(DroidDesign DroidTypeToCopy)
        {
            TemplateDroidType = DroidTypeToCopy.TemplateDroidType;
            Body = DroidTypeToCopy.Body;
            Propulsion = DroidTypeToCopy.Propulsion;
            TurretCount = DroidTypeToCopy.TurretCount;
            Turret1 = DroidTypeToCopy.Turret1;
            Turret2 = DroidTypeToCopy.Turret2;
            Turret3 = DroidTypeToCopy.Turret3;
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

            clsAttachment NewBody = BaseAttachment.AddCopyOfAttachment(Body.Attachment);

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

            Position.XYZ_dbl TurretConnector = new Position.XYZ_dbl();

            TurretConnector = Body.Attachment.Models[0].Connectors[0];

            if ( TurretCount >= 1 )
            {
                if ( Turret1 != null )
                {
                    clsAttachment NewTurret = NewBody.AddCopyOfAttachment(Turret1.Attachment);
                    NewTurret.Pos_Offset = TurretConnector;
                }
            }

            if ( Body.Attachment.Models[0].ConnectorCount <= 1 )
            {
                return;
            }

            TurretConnector = Body.Attachment.Models[0].Connectors[1];

            if ( TurretCount >= 2 )
            {
                if ( Turret2 != null )
                {
                    clsAttachment NewTurret = NewBody.AddCopyOfAttachment(Turret2.Attachment);
                    NewTurret.Pos_Offset = TurretConnector;
                }
            }
        }

        public int GetMaxHitPoints()
        {
            int Result = 0;

            //this is inaccurate

            if ( Body == null )
            {
                return 0;
            }
            Result = Body.Hitpoints;
            if ( Propulsion == null )
            {
                return Result;
            }
            Result += (int)(Body.Hitpoints * Propulsion.HitPoints / 100.0D);
            if ( Turret1 == null )
            {
                return Result;
            }
            Result += Body.Hitpoints + Turret1.HitPoints;
            if ( TurretCount < 2 || Turret2 == null )
            {
                return Result;
            }
            if ( Turret2.TurretType != Turret.enumTurretType.Weapon )
            {
                return Result;
            }
            Result += Body.Hitpoints + Turret2.HitPoints;
            if ( TurretCount < 3 || Turret3 == null )
            {
                return Result;
            }
            if ( Turret3.TurretType != Turret.enumTurretType.Weapon )
            {
                return Result;
            }
            Result += Body.Hitpoints + Turret3.HitPoints;
            return Result;
        }

        public struct sLoadPartsArgs
        {
            public Body Body;
            public Propulsion Propulsion;
            public Construct Construct;
            public Sensor Sensor;
            public Repair Repair;
            public Brain Brain;
            public Ecm ECM;
            public Weapon Weapon1;
            public Weapon Weapon2;
            public Weapon Weapon3;
        }

        public bool LoadParts(sLoadPartsArgs Args)
        {
            bool TurretConflict = default(bool);

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
                    TurretCount = (byte)1;
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
                    TurretCount = (byte)1;
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
                    TurretCount = (byte)1;
                    Turret1 = Args.Brain;
                }
            }
            if ( Args.Weapon1 != null )
            {
                bool UseWeapon = default(bool);
                if ( Turret1 != null )
                {
                    if ( Turret1.TurretType == Turret.enumTurretType.Brain )
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
                    TurretCount = (byte)1;
                    Turret1 = Args.Weapon1;
                    if ( Args.Weapon2 != null )
                    {
                        Turret2 = Args.Weapon2;
                        TurretCount += (byte)1;
                        if ( Args.Weapon3 != null )
                        {
                            Turret3 = Args.Weapon3;
                            TurretCount += (byte)1;
                        }
                    }
                }
            }
            if ( Args.Sensor != null )
            {
                if ( Args.Sensor.Location == Sensor.enumLocation.Turret )
                {
                    if ( Turret1 != null )
                    {
                        TurretConflict = true;
                    }
                    TurretCount = (byte)1;
                    Turret1 = Args.Sensor;
                }
            }
            UpdateAttachments();

            return !TurretConflict; //return if all is ok
        }

        public string GenerateName()
        {
            string Result = "";

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

        public App.enumDroidType GetDroidType()
        {
            App.enumDroidType Result = default(App.enumDroidType);

            if ( TemplateDroidType == App.TemplateDroidType_Null )
            {
                Result = App.enumDroidType.Default_;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_Person )
            {
                Result = App.enumDroidType.Person;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_Cyborg )
            {
                Result = App.enumDroidType.Cyborg;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_CyborgSuper )
            {
                Result = App.enumDroidType.Cyborg_Super;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_CyborgConstruct )
            {
                Result = App.enumDroidType.Cyborg_Construct;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_CyborgRepair )
            {
                Result = App.enumDroidType.Cyborg_Repair;
            }
            else if ( TemplateDroidType == App.TemplateDroidType_Transporter )
            {
                Result = App.enumDroidType.Transporter;
            }
            else if ( Turret1 == null )
            {
                Result = App.enumDroidType.Default_;
            }
            else if ( Turret1.TurretType == Turret.enumTurretType.Brain )
            {
                Result = App.enumDroidType.Command;
            }
            else if ( Turret1.TurretType == Turret.enumTurretType.Sensor )
            {
                Result = App.enumDroidType.Sensor;
            }
            else if ( Turret1.TurretType == Turret.enumTurretType.ECM )
            {
                Result = App.enumDroidType.ECM;
            }
            else if ( Turret1.TurretType == Turret.enumTurretType.Construct )
            {
                Result = App.enumDroidType.Construct;
            }
            else if ( Turret1.TurretType == Turret.enumTurretType.Repair )
            {
                Result = App.enumDroidType.Repair;
            }
            else if ( Turret1.TurretType == Turret.enumTurretType.Weapon )
            {
                Result = App.enumDroidType.Weapon;
            }
            else
            {
                Result = App.enumDroidType.Default_;
            }
            return Result;
        }

        public bool SetDroidType(App.enumDroidType DroidType)
        {
            switch ( DroidType )
            {
                case App.enumDroidType.Weapon:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case App.enumDroidType.Sensor:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case App.enumDroidType.ECM:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case App.enumDroidType.Construct:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case App.enumDroidType.Person:
                    TemplateDroidType = App.TemplateDroidType_Person;
                    break;
                case App.enumDroidType.Cyborg:
                    TemplateDroidType = App.TemplateDroidType_Cyborg;
                    break;
                case App.enumDroidType.Transporter:
                    TemplateDroidType = App.TemplateDroidType_Transporter;
                    break;
                case App.enumDroidType.Command:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case App.enumDroidType.Repair:
                    TemplateDroidType = App.TemplateDroidType_Droid;
                    break;
                case App.enumDroidType.Default_:
                    TemplateDroidType = App.TemplateDroidType_Null;
                    break;
                case App.enumDroidType.Cyborg_Construct:
                    TemplateDroidType = App.TemplateDroidType_CyborgConstruct;
                    break;
                case App.enumDroidType.Cyborg_Repair:
                    TemplateDroidType = App.TemplateDroidType_CyborgRepair;
                    break;
                case App.enumDroidType.Cyborg_Super:
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
            bool NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != Turret.enumTurretType.Construct )
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
            else
            {
                return Turret1.Code;
            }
        }

        public string GetRepairCode()
        {
            bool NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != Turret.enumTurretType.Repair )
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
            else
            {
                return Turret1.Code;
            }
        }

        public string GetSensorCode()
        {
            bool NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != Turret.enumTurretType.Sensor )
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
            else
            {
                return Turret1.Code;
            }
        }

        public string GetBrainCode()
        {
            bool NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != Turret.enumTurretType.Brain )
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
            else
            {
                return Turret1.Code;
            }
        }

        public string GetECMCode()
        {
            bool NotThis = default(bool);

            if ( TurretCount >= 1 )
            {
                if ( Turret1 == null )
                {
                    NotThis = true;
                }
                else if ( Turret1.TurretType != Turret.enumTurretType.ECM )
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
            else
            {
                return Turret1.Code;
            }
        }

        public override string GetName()
        {
            return Name;
        }
    }

    public class DroidTemplate : DroidDesign
    {
        public ConnectedListLink<DroidTemplate, clsObjectData> DroidTemplate_ObjectDataLink;

        public string Code = "";

        public DroidTemplate()
        {
            DroidTemplate_ObjectDataLink = new ConnectedListLink<DroidTemplate, clsObjectData>(this);


            IsTemplate = true;
            Name = "Unknown";
        }
    }

    public class clsWallType
    {
        public ConnectedListLink<clsWallType, clsObjectData> WallType_ObjectDataLink;

        public string Code = "";
        public string Name = "Unknown";

        public int[] TileWalls_Segment = new int[] {0, 0, 0, 0, 0, 3, 3, 2, 0, 3, 3, 2, 0, 2, 2, 1};
        private const int d0 = 0;
        private const int d1 = 90;
        private const int d2 = 180;
        private const int d3 = 270;
        public int[] TileWalls_Direction = new int[] {d0, d0, d2, d0, d3, d0, d3, d0, d1, d1, d2, d2, d3, d1, d3, d0};

        public ConnectedList<StructureTypeBase, clsWallType> Segments;

        public clsWallType()
        {
            WallType_ObjectDataLink = new ConnectedListLink<clsWallType, clsObjectData>(this);
            Segments = new ConnectedList<StructureTypeBase, clsWallType>(this);


            Segments.MaintainOrder = true;
        }
    }
}