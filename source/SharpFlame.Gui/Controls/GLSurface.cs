using System;
using Eto;
using Eto.Forms;

namespace SharpFlame.Gui.Controls
{
    public interface IGLSurface : IControl
    {
        void MakeCurrent();
        void SwapBuffers();

        event EventHandler Initialized;
        event EventHandler Paint;
        event EventHandler ShuttingDown;
    }

    public interface IGLSurfaceControl 
    {
        void MakeCurrent();
        void SwapBuffers();

        event EventHandler Initialized;
        event EventHandler Paint;
        event EventHandler ShuttingDown;
    }

    public class GLSurface : Control, IGLSurfaceControl
    {
        public GLSurface() : this(Generator.Current)
        {
        }
        public GLSurface(Generator generator) : this(generator, typeof(IGLSurface), true)
        {
            var handler = (IGLSurface)this.Handler;
            handler.Initialized += new System.EventHandler(OnInitialized);
            handler.Paint += new System.EventHandler(OnPaint);
            handler.ShuttingDown += new System.EventHandler(OnShuttingDown);
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
            ((IGLSurface)Handler).MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            ((IGLSurface)Handler).SwapBuffers ();
        }

        public override void OnLoadComplete( EventArgs e )
        {
            base.OnLoadComplete( e );
        }
    }
}