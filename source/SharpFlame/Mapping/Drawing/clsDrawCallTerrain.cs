using OpenTK.Graphics.OpenGL;

namespace SharpFlame.Mapping.Drawing
{
    public class clsDrawCallTerrain : clsMap.clsAction
    {
        public override void ActionPerform()
        {
            GL.CallList(Map.Sectors[PosNum.X, PosNum.Y].GLList_Textured);
        }
    }
}