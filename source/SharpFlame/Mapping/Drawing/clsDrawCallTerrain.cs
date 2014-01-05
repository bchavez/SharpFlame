using OpenTK.Graphics.OpenGL;
using SharpFlame.Mapping.Tools;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawCallTerrain : clsAction
    {
        public override void ActionPerform()
        {
            GL.CallList(Map.Sectors[PosNum.X, PosNum.Y].GLList_Textured);
        }
    }
}