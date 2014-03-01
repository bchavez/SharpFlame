using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Old.Graphics.OpenGL;

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
        event EventHandler Resize;
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
            Control = ((IGLSurfaceHandler)Handler).Control;
            Control.Initialized += OnInitialized;
            Control.Resize += OnResize;
            Control.ShuttingDown += OnShuttingDown;
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
            Control.MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            Control.SwapBuffers ();
        }

        public GLFont CreateGLFont(System.Drawing.Font baseFont)
        {
            return new GLFont(new System.Drawing.Font(baseFont.FontFamily, 24.0F, baseFont.Style, System.Drawing.GraphicsUnit.Pixel));
        }
    }
}