namespace FlaME
{
    using System;

    public class clsDroidTemplate : clsDroidDesign
    {
        public string Code;
        public modLists.ConnectedListLink<clsDroidTemplate, clsObjectData> DroidTemplate_ObjectDataLink;

        public clsDroidTemplate()
        {
            this.DroidTemplate_ObjectDataLink = new modLists.ConnectedListLink<clsDroidTemplate, clsObjectData>(this);
            this.Code = "";
            base.IsTemplate = true;
            base.Name = "Unknown";
        }
    }
}

