using System;
using System.Drawing;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.Foundation;
using MonoMac.OpenGL;
using NLog;
using Size = Eto.Drawing.Size;

namespace Eto.Gl.Mac
{
    public class MacGLView3 : NSScrollView
    {
        
        private bool suspendResize = true;
        private NSObject notificationProxy;
        private Logger logger = LogManager.GetCurrentClassLogger();

        public MacGLView3()
        {
            // Look for changes in view size
            // Note, -reshape will not be called automatically on size changes because NSView does not export it to override 
            notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver(NSView.GlobalFrameChangedNotification, HandleReshape);
        }

        public override bool NeedsDisplay
        {
            get { return false; }
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
            //    base.SetBoundsSize(newSize);
        }

        public override void SetFrameSize(SizeF newSize)
        {
            //if( suspendResize )
            //    base.SetFrameSize(newSize);
        }

        public bool IsInitialized { get; private set; }

        public void MakeCurrent()
        {
            this.openGLContext.MakeCurrentContext();
            this.openTK.MakeCurrent(this.windowInfo);
        }

        public void SwapBuffers()
        {
            //openglFLush
            this.openGLContext.FlushBuffer();
            //this.openTK.SwapBuffers();
        }

        public event EventHandler Initialized = delegate { };

        public event EventHandler Resize = delegate { };

        public event EventHandler ShuttingDown = delegate { };

        public event EventHandler DrawNow = delegate { };


        private NSOpenGLContext openGLContext;

        private NSOpenGLPixelFormat pixelFormat;

        private OpenTK.Graphics.GraphicsContext openTK = null;

        private OpenTK.Platform.IWindowInfo windowInfo = null;

        public static NSOpenGLContext GlobalSharedContext = null;

        private CVDisplayLink displayLink;

        public override void LockFocus()
        {
            if( this.openGLContext == null ) return;
            base.LockFocus();
            if( this.openGLContext.View != this )
            {
                this.openGLContext.View = this;
            }

            this.openGLContext.MakeCurrentContext();
        }

        //public override void SetBoundsOrigin(PointF newOrigin)

        //{

        //}

        public void InitGL()
        {
            this.Superview.AutoresizesSubviews = true;
			this.Superview.AutoresizingMask = NSViewResizingMask.NotSizable;
            this.Superview.NeedsDisplay = false;

            var attribs = new object[]
                {
                    NSOpenGLPixelFormatAttribute.Window,
                    NSOpenGLPixelFormatAttribute.NoRecovery,
                    NSOpenGLPixelFormatAttribute.DoubleBuffer,
                    NSOpenGLPixelFormatAttribute.ColorSize, 24,
                    NSOpenGLPixelFormatAttribute.AlphaSize, 8,
                    NSOpenGLPixelFormatAttribute.DepthSize, 24,
                    NSOpenGLPixelFormatAttribute.MinimumPolicy, 0
                };

            pixelFormat = new NSOpenGLPixelFormat(attribs);

            if( pixelFormat == null )
                Console.WriteLine("No OpenGL pixel format");

            // NSOpenGLView does not handle context sharing, so we draw to a custom NSView instead
            openGLContext = new NSOpenGLContext(pixelFormat, GlobalSharedContext);
            if( GlobalSharedContext == null )
                GlobalSharedContext = openGLContext;

            openGLContext.View = this;

            openGLContext.MakeCurrentContext();

            // Synchronize buffer swaps with vertical refresh rate
            openGLContext.SwapInterval = true;

            var ctxPtr = this.openGLContext.CGLContext.Handle;
            var ctxHandle = new OpenTK.ContextHandle(ctxPtr);

            //Initialize OpenTK structures using OSX GL core handles.
			this.windowInfo = OpenTK.Platform.Utilities.CreateMacOSCarbonWindowInfo(this.openGLContext.Handle, false, true);
            this.openTK = new OpenTK.Graphics.GraphicsContext(ctxHandle, windowInfo);

            GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

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