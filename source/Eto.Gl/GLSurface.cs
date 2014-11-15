

using System;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Gl
{
    [Handler(typeof(GLSurface.IHandler))]
    public class GLSurface : Control
    {
        new IHandler Handler { get { return (IHandler)base.Handler; } }

        public new interface IHandler : Control.IHandler
        {
            Size GLSize { get; set; }
            bool IsInitialized { get; }

            void MakeCurrent();
            void SwapBuffers();
        }

        static readonly object callback = new Callback();

        protected override object GetCallback()
        {
            return callback;
        }

        public new interface ICallback : Control.ICallback
        {
            void OnInitialized(GLSurface w, EventArgs e);
            void OnClick(GLSurface widget, EventArgs e);
        }

        protected new class Callback : Control.Callback, ICallback
        {
            public void OnInitialized(GLSurface w, EventArgs e)
            {
                w.Platform.Invoke(() => w.OnInitialized(w, e));
            }

            public void OnClick(GLSurface widget, EventArgs e)
            {
                widget.Platform.Invoke( () => widget.OnClick(e) );
            }
        }

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
            this.Handler.MakeCurrent ();
        }

        public virtual void SwapBuffers() 
        {
            this.Handler.SwapBuffers ();
        }
    }
}