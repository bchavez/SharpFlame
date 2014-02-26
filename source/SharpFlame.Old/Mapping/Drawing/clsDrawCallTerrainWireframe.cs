#region

using OpenTK.Graphics.OpenGL;
using SharpFlame.Old.Mapping.Tools;

#endregion

namespace SharpFlame.Old.Mapping.Drawing
{
    public class clsDrawCallTerrainWireframe : clsAction
    {
        public override void ActionPerform()
        {
            GL.CallList(Map.Sectors[PosNum.X, PosNum.Y].GLList_Wireframe);
        }
    }
}