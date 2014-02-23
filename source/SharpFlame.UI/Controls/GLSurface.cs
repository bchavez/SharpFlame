using System;
using Eto;
using Eto.Forms;

namespace SharpFlame.UI.Controls
{
    public interface IGLSurface : IControl
    {
    }

    public class GLSurface : Control
    {
        public GLSurface() : this(Generator.Current)
        {
        }
        public GLSurface(Generator generator) : this(generator, typeof(IGLSurface), true)
        {
        }
        public GLSurface(Generator generator, Type type, bool initialize = true) : base(generator, type, initialize)
        {
        }
        public GLSurface(Generator generator, IControl handler, bool initialize = true) : base(generator, handler, initialize)
        {
        }

        private IGLSurface Handler
        {
            get { return (IGLSurface)base.Handler; }
        }

        public override void OnLoadComplete( EventArgs e )
        {
            var ctrl = this.Handler;
            var ctrl2 = this.ControlObject;

            base.OnLoadComplete( e );
        }
    }
}