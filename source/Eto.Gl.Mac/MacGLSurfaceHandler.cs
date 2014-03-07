using Eto.Platform.Mac.Forms;
using MonoMac.AppKit;

namespace Eto.Gl.Mac
{
    public class MacGLSurfaceHandler : MacView<MacGLView, GLSurface>, IGLSurfaceHandler
    {
        public override MacGLView CreateControl()
        {
            return new MacGLView();
        }

        public override void AttachEvent(string id)
        {
            switch( id )
            {
                case "Control.MouseEnter":
                    break;

                case "Control.MouseLeave":

                    break;
                case "Control.MouseMove":
                    break;

                case "Control.SizeChanged":
                    break;

                case "Control.MouseDown":
                    this.Control.GLMouseDown += Control_GLMouseDown;
                    break;

                case "Control.MouseUp":
                    break;

                case "Control.MouseDoubleClick":
                    break;

                case "Control.MouseWheel":
                    break;

                case "Control.KeyDown":
                    break;

                case "Control.KeyUp":
                    break;

                case "Control.LostFocus":
                    break;

                case "Control.GotFocus":
                    break;

                case "Control.Shown":
                    break;

                default:
                    base.AttachEvent(id);
                    break;
            }

        }

        private void Control_GLMouseDown(MacGLView sender, NSEvent args)
        {
            var mouseEvent = Eto.Platform.Mac.Conversions.GetMouseEvent(sender, args, false);
            this.Widget.OnMouseDown(mouseEvent);
        }

        public override bool Enabled { get; set; }

        public override NSView ContainerControl
        {
            get { return this.Control; }
        }
    }
}