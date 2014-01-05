namespace FlaME
{
    using System;
    using System.Runtime.InteropServices;

    public class clsDroidDesign : clsUnitType
    {
        public bool AlwaysDrawTextLabel;
        public clsUnitType.clsAttachment BaseAttachment = new clsUnitType.clsAttachment();
        public clsBody Body;
        public bool IsTemplate;
        public string Name = "";
        public clsPropulsion Propulsion;
        public clsTemplateDroidType TemplateDroidType;
        public clsTurret Turret1;
        public clsTurret Turret2;
        public clsTurret Turret3;
        public byte TurretCount;

        public clsDroidDesign()
        {
            base.Type = clsUnitType.enumType.PlayerDroid;
        }

        public void CopyDesign(clsDroidDesign DroidTypeToCopy)
        {
            this.TemplateDroidType = DroidTypeToCopy.TemplateDroidType;
            this.Body = DroidTypeToCopy.Body;
            this.Propulsion = DroidTypeToCopy.Propulsion;
            this.TurretCount = DroidTypeToCopy.TurretCount;
            this.Turret1 = DroidTypeToCopy.Turret1;
            this.Turret2 = DroidTypeToCopy.Turret2;
            this.Turret3 = DroidTypeToCopy.Turret3;
        }

        public string GenerateName()
        {
            string str2 = "";
            if (this.Propulsion != null)
            {
                if (str2.Length > 0)
                {
                    str2 = " " + str2;
                }
                str2 = this.Propulsion.Name + str2;
            }
            if (this.Body != null)
            {
                if (str2.Length > 0)
                {
                    str2 = " " + str2;
                }
                str2 = this.Body.Name + str2;
            }
            if ((this.TurretCount >= 3) && (this.Turret3 != null))
            {
                if (str2.Length > 0)
                {
                    str2 = " " + str2;
                }
                str2 = this.Turret3.Name + str2;
            }
            if ((this.TurretCount >= 2) && (this.Turret2 != null))
            {
                if (str2.Length > 0)
                {
                    str2 = " " + str2;
                }
                str2 = this.Turret2.Name + str2;
            }
            if ((this.TurretCount < 1) || (this.Turret1 == null))
            {
                return str2;
            }
            if (str2.Length > 0)
            {
                str2 = " " + str2;
            }
            return (this.Turret1.Name + str2);
        }

        public string GetBrainCode()
        {
            bool flag;
            if (this.TurretCount >= 1)
            {
                if (this.Turret1 == null)
                {
                    flag = true;
                }
                else if (this.Turret1.TurretType != clsTurret.enumTurretType.Brain)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                return "ZNULLBRAIN";
            }
            return this.Turret1.Code;
        }

        public string GetConstructCode()
        {
            bool flag;
            if (this.TurretCount >= 1)
            {
                if (this.Turret1 == null)
                {
                    flag = true;
                }
                else if (this.Turret1.TurretType != clsTurret.enumTurretType.Construct)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                return "ZNULLCONSTRUCT";
            }
            return this.Turret1.Code;
        }

        public modProgram.enumDroidType GetDroidType()
        {
            if (this.TemplateDroidType != modProgram.TemplateDroidType_Null)
            {
                if (this.TemplateDroidType == modProgram.TemplateDroidType_Person)
                {
                    return modProgram.enumDroidType.Person;
                }
                if (this.TemplateDroidType == modProgram.TemplateDroidType_Cyborg)
                {
                    return modProgram.enumDroidType.Cyborg;
                }
                if (this.TemplateDroidType == modProgram.TemplateDroidType_CyborgSuper)
                {
                    return modProgram.enumDroidType.Cyborg_Super;
                }
                if (this.TemplateDroidType == modProgram.TemplateDroidType_CyborgConstruct)
                {
                    return modProgram.enumDroidType.Cyborg_Construct;
                }
                if (this.TemplateDroidType == modProgram.TemplateDroidType_CyborgRepair)
                {
                    return modProgram.enumDroidType.Cyborg_Repair;
                }
                if (this.TemplateDroidType == modProgram.TemplateDroidType_Transporter)
                {
                    return modProgram.enumDroidType.Transporter;
                }
                if (this.Turret1 == null)
                {
                    return modProgram.enumDroidType.Default_;
                }
                if (this.Turret1.TurretType == clsTurret.enumTurretType.Brain)
                {
                    return modProgram.enumDroidType.Command;
                }
                if (this.Turret1.TurretType == clsTurret.enumTurretType.Sensor)
                {
                    return modProgram.enumDroidType.Sensor;
                }
                if (this.Turret1.TurretType == clsTurret.enumTurretType.ECM)
                {
                    return modProgram.enumDroidType.ECM;
                }
                if (this.Turret1.TurretType == clsTurret.enumTurretType.Construct)
                {
                    return modProgram.enumDroidType.Construct;
                }
                if (this.Turret1.TurretType == clsTurret.enumTurretType.Repair)
                {
                    return modProgram.enumDroidType.Repair;
                }
                if (this.Turret1.TurretType == clsTurret.enumTurretType.Weapon)
                {
                    return modProgram.enumDroidType.Weapon;
                }
            }
            return modProgram.enumDroidType.Default_;
        }

        public string GetECMCode()
        {
            bool flag;
            if (this.TurretCount >= 1)
            {
                if (this.Turret1 == null)
                {
                    flag = true;
                }
                else if (this.Turret1.TurretType != clsTurret.enumTurretType.ECM)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                return "ZNULLECM";
            }
            return this.Turret1.Code;
        }

        public int GetMaxHitPoints()
        {
            if (this.Body == null)
            {
                return 0;
            }
            int hitpoints = this.Body.Hitpoints;
            if (this.Propulsion == null)
            {
                return hitpoints;
            }
            hitpoints += (int) Math.Round((double) (((double) (this.Body.Hitpoints * this.Propulsion.HitPoints)) / 100.0));
            if (this.Turret1 == null)
            {
                return hitpoints;
            }
            hitpoints += this.Body.Hitpoints + this.Turret1.HitPoints;
            if ((this.TurretCount < 2) | (this.Turret2 == null))
            {
                return hitpoints;
            }
            if (this.Turret2.TurretType != clsTurret.enumTurretType.Weapon)
            {
                return hitpoints;
            }
            hitpoints += this.Body.Hitpoints + this.Turret2.HitPoints;
            if ((this.TurretCount < 3) | (this.Turret3 == null))
            {
                return hitpoints;
            }
            if (this.Turret3.TurretType != clsTurret.enumTurretType.Weapon)
            {
                return hitpoints;
            }
            return (hitpoints + (this.Body.Hitpoints + this.Turret3.HitPoints));
        }

        public override string GetName()
        {
            return this.Name;
        }

        public string GetRepairCode()
        {
            bool flag;
            if (this.TurretCount >= 1)
            {
                if (this.Turret1 == null)
                {
                    flag = true;
                }
                else if (this.Turret1.TurretType != clsTurret.enumTurretType.Repair)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                return "ZNULLREPAIR";
            }
            return this.Turret1.Code;
        }

        public string GetSensorCode()
        {
            bool flag;
            if (this.TurretCount >= 1)
            {
                if (this.Turret1 == null)
                {
                    flag = true;
                }
                else if (this.Turret1.TurretType != clsTurret.enumTurretType.Sensor)
                {
                    flag = true;
                }
                else
                {
                    flag = false;
                }
            }
            else
            {
                flag = true;
            }
            if (flag)
            {
                return "ZNULLSENSOR";
            }
            return this.Turret1.Code;
        }

        public bool LoadParts(sLoadPartsArgs Args)
        {
            this.Body = Args.Body;
            this.Propulsion = Args.Propulsion;
            bool flag2 = false;
            if ((Args.Construct != null) && (Args.Construct.Code != "ZNULLCONSTRUCT"))
            {
                if (this.Turret1 != null)
                {
                    flag2 = true;
                }
                this.TurretCount = 1;
                this.Turret1 = Args.Construct;
            }
            if ((Args.Repair != null) && (Args.Repair.Code != "ZNULLREPAIR"))
            {
                if (this.Turret1 != null)
                {
                    flag2 = true;
                }
                this.TurretCount = 1;
                this.Turret1 = Args.Repair;
            }
            if ((Args.Brain != null) && (Args.Brain.Code != "ZNULLBRAIN"))
            {
                if (this.Turret1 != null)
                {
                    flag2 = true;
                }
                this.TurretCount = 1;
                this.Turret1 = Args.Brain;
            }
            if (Args.Weapon1 != null)
            {
                bool flag3;
                if (this.Turret1 != null)
                {
                    if (this.Turret1.TurretType == clsTurret.enumTurretType.Brain)
                    {
                        flag3 = false;
                    }
                    else
                    {
                        flag3 = true;
                        flag2 = true;
                    }
                }
                else
                {
                    flag3 = true;
                }
                if (flag3)
                {
                    this.TurretCount = 1;
                    this.Turret1 = Args.Weapon1;
                    if (Args.Weapon2 != null)
                    {
                        this.Turret2 = Args.Weapon2;
                        this.TurretCount = (byte) (this.TurretCount + 1);
                        if (Args.Weapon3 != null)
                        {
                            this.Turret3 = Args.Weapon3;
                            this.TurretCount = (byte) (this.TurretCount + 1);
                        }
                    }
                }
            }
            if ((Args.Sensor != null) && (Args.Sensor.Location == clsSensor.enumLocation.Turret))
            {
                if (this.Turret1 != null)
                {
                    flag2 = true;
                }
                this.TurretCount = 1;
                this.Turret1 = Args.Sensor;
            }
            this.UpdateAttachments();
            return !flag2;
        }

        public bool SetDroidType(modProgram.enumDroidType DroidType)
        {
            switch (DroidType)
            {
                case modProgram.enumDroidType.Weapon:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                    break;

                case modProgram.enumDroidType.Sensor:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                    break;

                case modProgram.enumDroidType.ECM:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                    break;

                case modProgram.enumDroidType.Construct:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                    break;

                case modProgram.enumDroidType.Person:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Person;
                    break;

                case modProgram.enumDroidType.Cyborg:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Cyborg;
                    break;

                case modProgram.enumDroidType.Transporter:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Transporter;
                    break;

                case modProgram.enumDroidType.Command:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                    break;

                case modProgram.enumDroidType.Repair:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Droid;
                    break;

                case modProgram.enumDroidType.Default_:
                    this.TemplateDroidType = modProgram.TemplateDroidType_Null;
                    break;

                case modProgram.enumDroidType.Cyborg_Construct:
                    this.TemplateDroidType = modProgram.TemplateDroidType_CyborgConstruct;
                    break;

                case modProgram.enumDroidType.Cyborg_Repair:
                    this.TemplateDroidType = modProgram.TemplateDroidType_CyborgRepair;
                    break;

                case modProgram.enumDroidType.Cyborg_Super:
                    this.TemplateDroidType = modProgram.TemplateDroidType_CyborgSuper;
                    break;

                default:
                    this.TemplateDroidType = null;
                    return false;
            }
            return true;
        }

        protected override void TypeGLDraw()
        {
            if (this.BaseAttachment != null)
            {
                this.BaseAttachment.GLDraw();
            }
        }

        public void UpdateAttachments()
        {
            this.BaseAttachment = new clsUnitType.clsAttachment();
            if (this.Body == null)
            {
                this.AlwaysDrawTextLabel = true;
            }
            else
            {
                clsUnitType.clsAttachment attachment = this.BaseAttachment.AddCopyOfAttachment(this.Body.Attachment);
                this.AlwaysDrawTextLabel = attachment.Models.Count == 0;
                if ((this.Propulsion != null) && this.Body.ObjectDataLink.IsConnected)
                {
                    this.BaseAttachment.AddCopyOfAttachment(this.Propulsion.Bodies[this.Body.ObjectDataLink.ArrayPosition].LeftAttachment);
                    this.BaseAttachment.AddCopyOfAttachment(this.Propulsion.Bodies[this.Body.ObjectDataLink.ArrayPosition].RightAttachment);
                }
                if ((attachment.Models.Count != 0) && (attachment.Models[0].ConnectorCount > 0))
                {
                    modMath.sXYZ_sng _sng = this.Body.Attachment.Models[0].Connectors[0];
                    if ((this.TurretCount >= 1) && (this.Turret1 != null))
                    {
                        attachment.AddCopyOfAttachment(this.Turret1.Attachment).Pos_Offset = _sng;
                    }
                    if (this.Body.Attachment.Models[0].ConnectorCount > 1)
                    {
                        _sng = this.Body.Attachment.Models[0].Connectors[1];
                        if ((this.TurretCount >= 2) && (this.Turret2 != null))
                        {
                            attachment.AddCopyOfAttachment(this.Turret2.Attachment).Pos_Offset = _sng;
                        }
                    }
                }
            }
        }

        public class clsTemplateDroidType
        {
            public string Name;
            public int Num = -1;
            public string TemplateCode;

            public clsTemplateDroidType(string NewName, string NewTemplateCode)
            {
                this.Name = NewName;
                this.TemplateCode = NewTemplateCode;
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sLoadPartsArgs
        {
            public clsBody Body;
            public clsPropulsion Propulsion;
            public clsConstruct Construct;
            public clsSensor Sensor;
            public clsRepair Repair;
            public clsBrain Brain;
            public clsECM ECM;
            public clsWeapon Weapon1;
            public clsWeapon Weapon2;
            public clsWeapon Weapon3;
        }
    }
}

