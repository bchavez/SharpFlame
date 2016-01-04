using System;
using System.Drawing;
using Eto.Mac.Forms;
using MonoMac.AppKit;
using MonoMac.Foundation;
using NLog;
using OpenTK.Graphics;
using OpenTK.Graphics.OpenGL;
using Size = Eto.Drawing.Size;

namespace Eto.Gl.Mac
{
    public class MacGLView8 : NSView, IMacControl
    {
        
        private bool suspendResize = true;
        private NSObject notificationProxy;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public MacGLView8()
        {
            // Look for changes in view size
            // Note, -reshape will not be called automatically on size changes because NSView does not export it to override 
            notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver(NSView.GlobalFrameChangedNotification, HandleReshape);
        }

        public override bool NeedsDisplay
        {
            get { return true; }
            set
            {
            }
        }

        public override bool MouseDownCanMoveWindow
        {
            get { return false; }
        }
        public override bool IsOpaque
        {
            get { return true; }
        }

        private void HandleReshape(NSNotification obj)
        {
            if( !suspendResize )
            {
                this.Resize(this, EventArgs.Empty);
            }
        }


        public override void DrawRect(RectangleF dirtyRect)
        {
            if( !IsInitialized )
                InitGL();

            this.DrawNow( this, EventArgs.Empty);
        }

        public Size GLSize
        {
            get
            {
                //return this.Bounds.Size.ToSize().ToEto();
                return this.Bounds.Size.ToSize().ToEto();
            }
            set
            {
                //this.suspendResize = true;
                //this.SetBoundsSize(value.ToSD());
                //this.suspendResize = false;
            }
        }

        public override void SetBoundsSize(SizeF newSize)
        {
			//if( suspendResize )
                base.SetBoundsSize(newSize);
        }

        public override void SetFrameSize(SizeF newSize)
        {
			//if( suspendResize )
                base.SetFrameSize(newSize);
        }

        public bool IsInitialized { get; private set; }

        public void MakeCurrent()
        {
            this.openTK?.MakeCurrent(this.windowInfo);
        }

        public void SwapBuffers()
        {
            this.openTK.SwapBuffers();
        }

        public event EventHandler Initialized = delegate { };

        public event EventHandler Resize = delegate { };

        public event EventHandler ShuttingDown = delegate { };

        public event EventHandler DrawNow = delegate { };


        private OpenTK.Graphics.GraphicsContext openTK = null;

        private OpenTK.Platform.IWindowInfo windowInfo = null;

        public void InitGL()
        {
	        OpenTK.Graphics.GraphicsContext.ShareContexts = true;

			var gpxMode = GraphicsMode.Default;

			this.windowInfo = OpenTK.Platform.Utilities.CreateMacOSWindowInfo(this.Window.Handle, this.Handle);

	        this.openTK = new OpenTK.Graphics.GraphicsContext(gpxMode, this.windowInfo, 1, 0, GraphicsContextFlags.ForwardCompatible);

	        this.openTK.MakeCurrent(this.windowInfo);

	        GL.ClearColor(OpenTK.Graphics.Color4.CornflowerBlue);

            this.IsInitialized = true;

            this.Initialized(this, EventArgs.Empty);

            if( suspendResize )
            {
                this.Resize(this, EventArgs.Empty);
                suspendResize = false;
            }
        }

        public WeakReference WeakHandler { get; set; }

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown (theEvent);
        }
        public override void MouseEntered(NSEvent theEvent)
        {
            base.MouseEntered(theEvent);
        }
        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
        }
        
        
    }
}