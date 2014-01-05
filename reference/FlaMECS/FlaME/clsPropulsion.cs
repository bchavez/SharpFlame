namespace FlaME
{
    using System;
    using System.Runtime.InteropServices;

    public class clsPropulsion : clsComponent
    {
        public sBody[] Bodies;
        public int HitPoints;
        public modLists.ConnectedListLink<clsPropulsion, clsObjectData> ObjectDataLink;

        public clsPropulsion(int BodyCount)
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsPropulsion, clsObjectData>(this);
            this.Bodies = new sBody[0];
            base.ComponentType = clsComponent.enumComponentType.Propulsion;
            this.Bodies = new sBody[(BodyCount - 1) + 1];
            int num2 = BodyCount - 1;
            for (int i = 0; i <= num2; i++)
            {
                this.Bodies[i].LeftAttachment = new clsUnitType.clsAttachment();
                this.Bodies[i].RightAttachment = new clsUnitType.clsAttachment();
            }
        }

        [StructLayout(LayoutKind.Sequential)]
        public struct sBody
        {
            public clsUnitType.clsAttachment LeftAttachment;
            public clsUnitType.clsAttachment RightAttachment;
        }
    }
}

