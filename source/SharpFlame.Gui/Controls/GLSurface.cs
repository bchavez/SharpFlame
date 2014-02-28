using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Controls
{
    public interface IGLSurfaceHandler : IControl
    {
        IGLSurfaceControl Control { get; }
    }

    public interface IGLSurfaceControl 
    {
        Size Size { get; set; }

        void MakeCurrent();
        void SwapBuffers();

        event EventHandler Initialized;
        event EventHandler Paint;
        event EventHandler ShuttingDown;
    }

    public class GLSurface : Control, IGLSurfaceControl
    {
        IGLSurfaceControl Control { get; set; }

        public new Size Size {
            get { return Control.Size; } 
            set { Control.Size = value; }
        }

        public GLSurface() : this(Generator.Current)
        {
        }
        public GLSurface(Generator generator) : this(generator, typeof(IGLSurfaceHandler), true)
        {
            Control = (IGLSurfaceControl)((IGLSurfaceHandler)this.Handler).Control;
            Control.Initialized += new System.EventHandler(OnInitialized);
            Control.Paint += new System.EventHandler(OnPaint);
            Control.ShuttingDown += new System.EventHandler(OnShuttingDown);
        }
        public GLSurface(Generator generator, Type type, bool initialize = true) : base(generator, type, initialize)
        {
        }
        public GLSurface(Generator generator, IControl handler, bool initialize = true) : base(generator, handler, initialize)
        {
        }

        public event EventHandler Initialized = delegate {};
        public virtual void OnInitialized(object obj, EventArgs e) 
        {
            Initialized (obj, e);
        }

        public event EventHandler Paint = delegate {};
        public virtual void OnPaint(object obj, EventArgs e) 
        {
            Paint (obj, e);
        }

        public event EventHandler ShuttingDown = delegate {};
        public virtual void OnShuttingDown(object obj, EventArgs e) 
        {
            ShuttingDown (obj, e);
        }

        public virtual void MakeCurrent() 
        {
            Control.MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            Control.SwapBuffers ();
        }

        public override void OnLoadComplete( EventArgs e )
        {
            base.OnLoadComplete( e );
        }
    }
}