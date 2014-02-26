namespace SharpFlame.Old.Domain
{
    public enum DroidType
    {
        Weapon = 0,
        Sensor = 1,
        ECM = 2,
        Construct = 3,
        Person = 4,
        Cyborg = 5,
        Transporter = 6,
        Command = 7,
        Repair = 8,
        Default = 9,
        CyborgConstruct = 10,
        CyborgRepair = 11,
        CyborgSuper = 12
    }

    public enum StructureType
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
}