

using System;
using Eto.Drawing;
using Eto.Forms;
using OpenTK.Graphics;

namespace Eto.Gl
{
    [Handler(typeof(GLSurface.IHandler))]
    public class GLSurface : Control
    {
	    public GLSurface() :
			this(GraphicsMode.Default)
	    {
	    }

	    public GLSurface(GraphicsMode graphicsMode):
			this(graphicsMode, 3, 0, GraphicsContextFlags.Default)
	    {
		    
	    }

	    public GLSurface(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
	    {
		    this.Handler.CreateWithParams(mode, major, minor, flags);
		    this.Initialize();
	    }


	    private new IHandler Handler{get{return (IHandler)base.Handler;}}

        // interface to the platform implementations
        // ETO WIDGET -> Platform Control
		[AutoInitialize(false)]
        public new interface IHandler : Control.IHandler
		{
			void CreateWithParams(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags);

            Size GLSize { get; set; }
            bool IsInitialized { get; }

            void MakeCurrent();
            void SwapBuffers();
        }

        public new interface ICallback : Control.ICallback
        {
            void OnInitialized(GLSurface w, EventArgs e);
            void OnShuttingDown(GLSurface w, EventArgs e);
        }

        //PLATFORM CONTROL -> ETO WIDGET
        protected new class Callback : Control.Callback, ICallback
        {
            public void OnInitialized(GLSurface w, EventArgs e)
            {
                w.Platform.Invoke(() => w.OnInitialized(w, e));
            }

            public void OnShuttingDown(GLSurface w, EventArgs e)
            {
                w.Platform.Invoke(() => w.OnShuttingDown(w, e));
            }
        }

        //Gets an instance of an object used to perform callbacks to the widget from handler implementations
        protected override object GetCallback()
        {
            return callback;
        }

        static readonly object callback = new Callback();

        public virtual void OnClick(EventArgs e)
        {
            Click(this, e);
        }

        public event EventHandler Click;

        public Size GLSize {
            get { return this.Handler.GLSize; } 
            set { this.Handler.GLSize = value; }
        }

        public bool IsInitialized {
            get { return this.Handler.IsInitialized; }
        }

        public event EventHandler Initialized = delegate {};

        public virtual void OnInitialized(object obj, EventArgs e) 
        {
            Initialized (obj, e);
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
            this.Handler.MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            this.Handler.SwapBuffers ();
        }
    }
}