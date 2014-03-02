using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Old.Graphics.OpenGL;

namespace SharpFlame.Gui.Controls
{
    public interface IGLSurfaceHandler : IControl
    {

    }

    public interface IGLSurface 
    {
        Size GLSize { get; set; }
        bool IsInitialized { get; }

        void MakeCurrent();
        void SwapBuffers();

        event EventHandler Initialized;
        event EventHandler Resize;
        event EventHandler ShuttingDown;
    }

    public class GLSurface : Control, IGLSurface
    {
        public IGLSurface PlatformControl
        {
            get { return this.ControlObject as IGLSurface; }
        }

        public Size GLSize {
            get { return PlatformControl.GLSize; } 
            set { PlatformControl.GLSize = value; }
        }

        public bool IsInitialized {
            get { return PlatformControl.IsInitialized; }
        }

        public GLSurface() : this(Generator.Current)
        {
        }
        public GLSurface(Generator generator) : this(generator, typeof(IGLSurfaceHandler), true)
        {
            PlatformControl.Initialized += OnInitialized;
            PlatformControl.Resize += OnResize;
            PlatformControl.ShuttingDown += OnShuttingDown;
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
        public virtual void OnShuttingDown(object obj, EventArgs e) 
        {
            ShuttingDown (obj, e);
        }

        public virtual void MakeCurrent() 
        {
            PlatformControl.MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            PlatformControl.SwapBuffers ();
        }

        public GLFont CreateGLFont(System.Drawing.Font baseFont)
        {
            return new GLFont(new System.Drawing.Font(baseFont.FontFamily, 24.0F, baseFont.Style, System.Drawing.GraphicsUnit.Pixel));
        }
    }
}