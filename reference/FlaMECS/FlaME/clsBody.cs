namespace FlaME
{
    using System;

    public class clsBody : clsComponent
    {
        public clsUnitType.clsAttachment Attachment;
        public int Hitpoints;
        public modLists.ConnectedListLink<clsBody, clsObjectData> ObjectDataLink;

        public clsBody()
        {
            this.ObjectDataLink = new modLists.ConnectedListLink<clsBody, clsObjectData>(this);
            this.Attachment = new clsUnitType.clsAttachment();
            base.ComponentType = clsComponent.enumComponentType.Body;
        }
    }
}

