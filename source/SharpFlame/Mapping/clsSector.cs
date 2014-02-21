#region

using OpenTK.Graphics.OpenGL;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.Core.Domain;
using SharpFlame.Mapping.Objects;

#endregion

namespace SharpFlame.Mapping
{
    public class clsSector
    {
        public int GLList_Textured;
        public int GLList_Wireframe;
        public XYInt Pos;
        public ConnectedList<clsUnitSectorConnection, clsSector> Units;

        public clsSector()
        {
            Units = new ConnectedList<clsUnitSectorConnection, clsSector>(this);
        }

        public clsSector(XYInt NewPos)
        {
            Units = new ConnectedList<clsUnitSectorConnection, clsSector>(this);


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