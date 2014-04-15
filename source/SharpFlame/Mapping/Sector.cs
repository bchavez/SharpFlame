#region

using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping
{
    public class Sector
    {
        public int GLList_Textured;
        public int GLList_Wireframe;
        public XYInt Pos;
        public ConnectedList<clsUnitSectorConnection, Sector> Units;

        public Sector()
        {
            Pos = new XYInt (0, 0);
            Units = new ConnectedList<clsUnitSectorConnection, Sector>(this);
        }

        public Sector(XYInt NewPos)
        {
            Units = new ConnectedList<clsUnitSectorConnection, Sector>(this);


            Pos = NewPos;
        }

        public void DeleteLists()
        {
            if ( GLList_Textured != 0 )
            {
                GL.DeleteLists(GLList_Textured, 1);
                GLList_Textured = 0;
            }
            if ( GLList_Wireframe != 0 )
            {
                GL.DeleteLists(GLList_Wireframe, 1);
                GLList_Wireframe = 0;
            }
        }

        public void Deallocate()
        {
            Units.Deallocate();
        }
    }
}