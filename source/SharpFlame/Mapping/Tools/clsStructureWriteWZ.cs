using System;
using System.Diagnostics;
using System.IO;
using SharpFlame.Collections;
using SharpFlame.Domain;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;

namespace SharpFlame.Mapping.Tools
{
    public class clsStructureWriteWZ : SimpleListTool<clsUnit>
    {
        public BinaryWriter File;
        public clsMap.sWrite_WZ_Args.enumCompileType CompileType;
        public int PlayerCount;

        private clsUnit Unit;

        private StructureTypeBase structureTypeBase;
        private byte[] StruZeroBytesA = new byte[12];
        private byte[] StruZeroBytesB = new byte[40];

        public void ActionPerform()
        {
            if ( CompileType == clsMap.sWrite_WZ_Args.enumCompileType.Unspecified )
            {
                Debugger.Break();
                return;
            }

            structureTypeBase = (StructureTypeBase)Unit.TypeBase;
            IOUtil.WriteTextOfLength(File, 40, structureTypeBase.Code);
            File.Write(Unit.ID);
            File.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.X));
            File.Write(Convert.ToBoolean((uint)Unit.Pos.Horizontal.Y));
            File.Write(Convert.ToBoolean((uint)Unit.Pos.Altitude));
            File.Write(Convert.ToBoolean((uint)Unit.Rotation));
            switch ( CompileType )
            {
                case clsMap.sWrite_WZ_Args.enumCompileType.Multiplayer:
                    File.Write(Unit.GetBJOMultiplayerPlayerNum(PlayerCount));
                    break;
                case clsMap.sWrite_WZ_Args.enumCompileType.Campaign:
                    File.Write(Unit.GetBJOCampaignPlayerNum());
                    break;
                default:
                    Debugger.Break();
                    break;
            }
            File.Write(StruZeroBytesA);
            File.Write((byte)1);
            File.Write((byte)26);
            File.Write((byte)127);
            File.Write((byte)0);
            File.Write(StruZeroBytesB);
        }

        public void SetItem(clsUnit Item)
        {
            Unit = Item;
        }
    }
}