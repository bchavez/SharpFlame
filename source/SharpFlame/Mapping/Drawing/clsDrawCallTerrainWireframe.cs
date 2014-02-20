#region

using OpenTK.Graphics.OpenGL;
using SharpFlame.Mapping.Tools;

#endregion

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawCallTerrainWireframe : clsAction
    {
        public override void ActionPerform()
        {
            GL.CallList(Map.Sectors[PosNum.X, PosNum.Y].GLList_Wireframe);
        }
    }
}