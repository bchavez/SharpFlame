namespace FlaME
{
    using System;

    public abstract class clsComponent
    {
        public string Code;
        public enumComponentType ComponentType = enumComponentType.Unspecified;
        public bool Designable;
        public bool IsUnknown = false;
        public string Name = "Unknown";

        protected clsComponent()
        {
        }

        public enum enumComponentType : byte
        {
            Body = 1,
            Propulsion = 2,
            Turret = 3,
            Unspecified = 0
        }
    }
}

