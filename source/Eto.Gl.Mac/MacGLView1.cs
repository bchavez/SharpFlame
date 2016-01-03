using System;
using Eto.Drawing;
using Eto.Forms;
using Eto.Mac.Forms;
using MonoMac.AppKit;

namespace Eto.Gl.Mac
{
    /// <summary>
    /// An NSOpenGLView object maintains an NSOpenGLPixelFormat and NSOpenGLContext object into which OpenGL calls can be rendered. 
    /// The view provides methods for accessing and managing the NSOpenGLPixelFormat and NSOpenGLContext objects,
    /// as well as notifications of visible region changes. An NSOpenGLView object cannot have subviews.
    /// </summary>
    /// <remarks>
    /// -- Method Summery --
    /// * Initializing an NSOpenGLView
    ///     – initWithFrame:pixelFormat:
    /// * Managing the NSOpenGLPixelFormat
    ///     + defaultPixelFormat
    ///     – pixelFormat
    ///     – setPixelFormat:
    /// * Managing the NSOpenGLContext
    ///     – prepareOpenGL
    ///     – clearGLContext
    ///     – openGLContext
    ///     – setOpenGLContext:
    /// * Managing the Visible Region
    ///     – reshape
    ///     – update
    /// * Displaying
    ///     – isOpaque
    /// </remarks>
    public class MacGLView1 : NSOpenGLView, IMacControl
    {

        private static NSOpenGLContext globalContext;

        public delegate void GLEventHandler(MacGLView1 sender, NSEvent args);

        public delegate void DrawRectHandler(System.Drawing.RectangleF dirtyRect);

        private OpenTK.Graphics.GraphicsContext openTK = null;
        private OpenTK.Platform.IWindowInfo windowInfo = null;


        public MacGLView1(){
            var ctx = this.OpenGLContext;

            globalContext = ctx;
        }

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            this.DrawNow(this, EventArgs.Empty);

        }
	

        public override void AwakeFromNib()
        {

            var pf = DefaultPixelFormat;

            var context = new NSOpenGLContext (pf, globalContext);
            if (globalContext == null)
                globalContext = context;

            this.OpenGLContext = context;
        }

        /// <summary>
        /// Used by subclasses to initialize OpenGL state. 
        /// </summary>
        /// <remarks>
        /// This method is called only once after the OpenGL context 
        /// is made the current context. Subclasses that implement this method can use it to configure the Open GL
        /// state in preparation for drawing.
        /// </remarks>
        public override void PrepareOpenGL()
        {        
            base.PrepareOpenGL();

            //get access to low-level OSX GL core handle.
            var contextPtr = this.OpenGLContext.CGLContext.Handle;
            var contextHandle = new OpenTK.ContextHandle(contextPtr);

            //Initialize OpenTK structures using OSX GL core handles.
            this.windowInfo = OpenTK.Platform.Utilities.CreateMacOSCarbonWindowInfo(this.Handle, false, true);
            this.openTK = new OpenTK.Graphics.GraphicsContext(contextHandle, windowInfo);


            this.IsInitialized = true;
            this.Initialized(this, EventArgs.Empty);

        }

        /// <summary>
        /// Releases the NSOpenGLContext object associated with the view. 
        /// </summary>
        /// <remarks>
        /// If necessary, this method calls the clearDrawable method of the context object before releasing it.
        /// </remarks>
        public override void ClearGLContext()
        {
            base.ClearGLContext();
            this.ShuttingDown(this, EventArgs.Empty);
        }

        /// <summary>
        /// Called by Cocoa when the view's visible rectangle or bounds change.
        /// </summary>
        /// <remarks>
        /// Cocoa typically calls this method during scrolling and resize operations but may call it in other
        /// situations when the view's rectangles change. The default implementation does nothing. You can override
        /// this method if you need to adjust the viewport and display frustum.
        /// </remarks>
        public override void Reshape()
        {
            base.Reshape();
            this.Resize(this, EventArgs.Empty);
        }

        //TODO: Not sure if we need this.
        public override void ViewDidEndLiveResize()
        {
            base.ViewDidEndLiveResize();
            this.Resize(this, EventArgs.Empty);
        }


        public Size GLSize
        {
            get { return this.Bounds.Size.ToSize().ToEto(); }
            set
            {
                // warn: uncommenting this causes stack overflow, resize, redraw texture, resize, redraw texture, resize....
                // TODO: need to figure out how to deal with this.
                //  this.Bounds =
                //new RectangleF(this.Bounds.X, this.Bounds.Y, value.Width, value.Height);
            }
        }

        public bool IsInitialized { get; private set; }
        public bool HasFocus { get; private set; }

        public void MakeCurrent()
        {
            //OSX without OpenTK: this.OpenGLContext.MakeCurrentContext();
            this.OpenGLContext.MakeCurrentContext ();
            this.openTK.MakeCurrent(this.windowInfo);
        }

        public void SwapBuffers()
        {
            //OSX without OpenTK: this.OpenGLContext.FlushBuffer();
            this.openTK.SwapBuffers();
        }

        public void Focus()
        {
            throw new NotImplementedException();
        }

        public event EventHandler Initialized = delegate { };

        public event EventHandler Resize = delegate { };

        public event EventHandler ShuttingDown = delegate { };
        public event EventHandler DrawNow = delegate { };
        public event EventHandler<KeyEventArgs> GlKeyDown;
        public event EventHandler<KeyEventArgs> GlKeyUp;

        public event GLEventHandler GLKeyDown = delegate { };

        public override void KeyDown(NSEvent theEvent)
        {
            base.KeyDown(theEvent);
            GLKeyDown(this, theEvent);
        }

        public event GLEventHandler GLMouseDown = delegate { };

        public override void MouseDown(NSEvent theEvent)
        {
            base.MouseDown(theEvent);
            GLMouseDown(this, theEvent);
        }

        public event GLEventHandler GLMouseUp = delegate { };

        public override void MouseUp(NSEvent theEvent)
        {
            base.MouseUp(theEvent);
            GLMouseUp(this, theEvent);
        }

        public event GLEventHandler GLMouseDragged = delegate { };

        public override void MouseDragged(NSEvent theEvent)
        {
            base.MouseDragged(theEvent);
            GLMouseDragged(this, theEvent);
        }

        public event GLEventHandler GLMouseMoved = delegate { };

        public override void MouseMoved(NSEvent theEvent)
        {
            base.MouseMoved(theEvent);
            GLMouseMoved(this, theEvent);
        }

        public event GLEventHandler GLScrollWheel = delegate { };

        public override void ScrollWheel(NSEvent theEvent)
        {
            base.ScrollWheel(theEvent);
            GLScrollWheel(this, theEvent);
        }

        public WeakReference WeakHandler{ get; set; }
    }
}