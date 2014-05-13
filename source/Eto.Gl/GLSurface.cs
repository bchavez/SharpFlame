

using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using Eto.Misc;

namespace Eto.Gl
{
    public interface IGLSurfacePlatformHandler : IControl
    {
        Size GLSize { get; set; }
        bool IsInitialized { get; }

        void MakeCurrent();
        void SwapBuffers();
    }

    public class GLSurface : Control
    {
        public IGLSurfacePlatformHandler PlatformHandler
        {
            get { return this.Handler as IGLSurfacePlatformHandler; }
        }

        public Size GLSize {
            get { return PlatformHandler.GLSize; } 
            set { PlatformHandler.GLSize = value; }
        }

        public bool IsInitialized {
            get { return PlatformHandler.IsInitialized; }
        }

        public GLSurface() : this(Generator.Current)
        {
        }
        public GLSurface(Generator generator) : this(generator, typeof(IGLSurfacePlatformHandler), true)
        {
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

        public event EventHandler Resize = delegate {};

        public virtual void OnResize(object obj, EventArgs e) 
        {
            Resize (obj, e);
        }

        public event EventHandler ShuttingDown = delegate {};

        public event EventHandler DrawNow = delegate { };

        private void OnDrawNow(object sender, EventArgs e)
        {
            this.DrawNow(sender, e);
        }

        public virtual void OnShuttingDown(object obj, EventArgs e) 
        {
            ShuttingDown (obj, e);
        }

        public virtual void MakeCurrent() 
        {
            PlatformHandler.MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            PlatformHandler.SwapBuffers ();
        }
    }
}