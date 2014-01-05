namespace FlaME
{
    using Matrix3D;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections;

    public abstract class clsUnitType
    {
        public bool IsUnknown;
        public enumType Type;
        public readonly modLists.ConnectedListLink<clsUnitType, frmMain> UnitType_frmMainSelectedLink;
        public readonly modLists.ConnectedListLink<clsUnitType, clsObjectData> UnitType_ObjectDataLink;

        protected clsUnitType()
        {
            this.UnitType_ObjectDataLink = new modLists.ConnectedListLink<clsUnitType, clsObjectData>(this);
            this.UnitType_frmMainSelectedLink = new modLists.ConnectedListLink<clsUnitType, frmMain>(this);
            this.IsUnknown = false;
        }

        public bool GetCode(ref string Result)
        {
            switch (this.Type)
            {
                case enumType.Feature:
                    Result = ((clsFeatureType) this).Code;
                    return true;

                case enumType.PlayerStructure:
                    Result = ((clsStructureType) this).Code;
                    return true;

                case enumType.PlayerDroid:
                {
                    clsDroidDesign design = (clsDroidDesign) this;
                    if (!design.IsTemplate)
                    {
                        Result = null;
                        return false;
                    }
                    Result = ((clsDroidTemplate) this).Code;
                    return true;
                }
            }
            Result = null;
            return false;
        }

        public string GetDisplayTextCode()
        {
            switch (this.Type)
            {
                case enumType.Feature:
                {
                    clsFeatureType type = (clsFeatureType) this;
                    return (type.Code + " (" + type.Name + ")");
                }
                case enumType.PlayerStructure:
                {
                    clsStructureType type2 = (clsStructureType) this;
                    return (type2.Code + " (" + type2.Name + ")");
                }
                case enumType.PlayerDroid:
                {
                    clsDroidDesign design = (clsDroidDesign) this;
                    if (!design.IsTemplate)
                    {
                        return ("<droid> (" + design.GenerateName() + ")");
                    }
                    clsDroidTemplate template = (clsDroidTemplate) this;
                    return (template.Code + " (" + template.Name + ")");
                }
            }
            return "";
        }

        public string GetDisplayTextName()
        {
            switch (this.Type)
            {
                case enumType.Feature:
                {
                    clsFeatureType type = (clsFeatureType) this;
                    return (type.Name + " (" + type.Code + ")");
                }
                case enumType.PlayerStructure:
                {
                    clsStructureType type2 = (clsStructureType) this;
                    return (type2.Name + " (" + type2.Code + ")");
                }
                case enumType.PlayerDroid:
                {
                    clsDroidDesign design = (clsDroidDesign) this;
                    if (!design.IsTemplate)
                    {
                        return (design.GenerateName() + " (<droid>)");
                    }
                    clsDroidTemplate template = (clsDroidTemplate) this;
                    return (template.Name + " (" + template.Code + ")");
                }
            }
            return "";
        }

        public virtual string GetName()
        {
            return "";
        }

        public void GLDraw(float RotationDegrees)
        {
            switch (modProgram.Draw_Lighting)
            {
                case modProgram.enumDrawLighting.Off:
                    GL.Color3((float) 1f, (float) 1f, (float) 1f);
                    break;

                case modProgram.enumDrawLighting.Half:
                    GL.Color3((float) 0.875f, (float) 0.875f, (float) 0.875f);
                    break;

                case modProgram.enumDrawLighting.Normal:
                    GL.Color3((float) 0.75f, (float) 0.75f, (float) 0.75f);
                    break;
            }
            GL.Rotate(RotationDegrees, 0f, 1f, 0f);
            this.TypeGLDraw();
        }

        protected virtual void TypeGLDraw()
        {
        }

        public modMath.sXY_int this[int Rotation]
        {
            get
            {
                modMath.sXY_int footprint;
                switch (this.Type)
                {
                    case enumType.Feature:
                        footprint = ((clsFeatureType) this).Footprint;
                        break;

                    case enumType.PlayerStructure:
                        footprint = ((clsStructureType) this).Footprint;
                        break;

                    default:
                        return new modMath.sXY_int(1, 1);
                }
                double num = ((((double) Rotation) / 90.0) + 0.5) % 2.0;
                if (num < 0.0)
                {
                    num += 2.0;
                }
                if (num >= 1.0)
                {
                    int x = footprint.X;
                    footprint.X = footprint.Y;
                    footprint.Y = x;
                }
                return footprint;
            }
        }

        public modMath.sXY_int GetFootprintOld
        {
            get
            {
                switch (this.Type)
                {
                    case enumType.Feature:
                        return ((clsFeatureType) this).Footprint;

                    case enumType.PlayerStructure:
                        return ((clsStructureType) this).Footprint;
                }
                return new modMath.sXY_int(1, 1);
            }
        }

        public modMath.sXY_int this[int Rotation]
        {
            get
            {
                if (modMain.frmMainInstance.cbxFootprintRotate.Checked)
                {
                    return this.get_GetFootprintNew(Rotation);
                }
                return this.GetFootprintOld;
            }
        }

        public class clsAttachment
        {
            public Matrix3DMath.Matrix3D AngleOffsetMatrix = new Matrix3DMath.Matrix3D();
            public modLists.SimpleClassList<clsUnitType.clsAttachment> Attachments = new modLists.SimpleClassList<clsUnitType.clsAttachment>();
            public modLists.SimpleClassList<clsModel> Models = new modLists.SimpleClassList<clsModel>();
            public modMath.sXYZ_sng Pos_Offset;

            public clsAttachment()
            {
                this.Models.AddNothingAction = modLists.SimpleClassList_AddNothingAction.DisallowIgnore;
                Matrix3DMath.MatrixSetToIdentity(this.AngleOffsetMatrix);
            }

            public clsUnitType.clsAttachment AddCopyOfAttachment(clsUnitType.clsAttachment AttachmentToCopy)
            {
                IEnumerator enumerator;
                clsUnitType.clsAttachment newItem = new clsUnitType.clsAttachment();
                this.Attachments.Add(newItem);
                Matrix3DMath.MatrixCopy(AttachmentToCopy.AngleOffsetMatrix, newItem.AngleOffsetMatrix);
                newItem.Models.AddSimpleList(AttachmentToCopy.Models);
                try
                {
                    enumerator = AttachmentToCopy.Attachments.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsUnitType.clsAttachment current = (clsUnitType.clsAttachment) enumerator.Current;
                        newItem.AddCopyOfAttachment(current);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                return newItem;
            }

            public clsUnitType.clsAttachment CopyAttachment(clsUnitType.clsAttachment Other)
            {
                clsUnitType.clsAttachment newItem = new clsUnitType.clsAttachment {
                    Pos_Offset = Other.Pos_Offset
                };
                this.Attachments.Add(newItem);
                Matrix3DMath.MatrixCopy(Other.AngleOffsetMatrix, newItem.AngleOffsetMatrix);
                newItem.Models.AddSimpleList(Other.Models);
                newItem.Attachments.AddSimpleList(Other.Attachments);
                return newItem;
            }

            public clsUnitType.clsAttachment CreateAttachment()
            {
                clsUnitType.clsAttachment newItem = new clsUnitType.clsAttachment();
                this.Attachments.Add(newItem);
                return newItem;
            }

            public void GLDraw()
            {
                IEnumerator enumerator;
                IEnumerator enumerator2;
                Matrix3DMath.Matrix3D matrixOut = new Matrix3DMath.Matrix3D();
                try
                {
                    enumerator = this.Models.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        ((clsModel) enumerator.Current).GLDraw();
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                try
                {
                    enumerator2 = this.Attachments.GetEnumerator();
                    while (enumerator2.MoveNext())
                    {
                        Angles.AngleRPY erpy;
                        clsUnitType.clsAttachment current = (clsUnitType.clsAttachment) enumerator2.Current;
                        GL.PushMatrix();
                        Matrix3DMath.MatrixInvert(current.AngleOffsetMatrix, matrixOut);
                        Matrix3DMath.MatrixToRPY(matrixOut, ref erpy);
                        GL.Translate(current.Pos_Offset.X, current.Pos_Offset.Y, -current.Pos_Offset.Z);
                        GL.Rotate((double) (erpy.Roll / 0.017453292519943295), (double) 0.0, (double) 0.0, (double) -1.0);
                        GL.Rotate((double) (erpy.Pitch / 0.017453292519943295), (double) 1.0, (double) 0.0, (double) 0.0);
                        GL.Rotate((double) (erpy.Yaw / 0.017453292519943295), (double) 0.0, (double) 1.0, (double) 0.0);
                        current.GLDraw();
                        GL.PopMatrix();
                    }
                }
                finally
                {
                    if (enumerator2 is IDisposable)
                    {
                        (enumerator2 as IDisposable).Dispose();
                    }
                }
            }
        }

        public enum enumType : byte
        {
            Feature = 1,
            PlayerDroid = 3,
            PlayerStructure = 2,
            Unspecified = 0
        }
    }
}

