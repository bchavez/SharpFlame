using System;
using Eto.Forms;
using MonoMac.AppKit;
using MonoMac.CoreVideo;
using MonoMac.Foundation;
using MonoMac.OpenGL;

namespace Eto.Gl.Mac
{
    public class MacGLView2 : NSView
    {
        //required for sharing textures between contexts,
        //NSOpenGLView does not allow contexts to be created
        //by the programmer using a shared context.
        public static NSOpenGLContext GlobalSharedContext = null;

        public delegate void GLEventHandler(MacGLView2 sender, NSEvent args);

        public delegate void DrawRectHandler(System.Drawing.RectangleF dirtyRect);

        private OpenTK.Graphics.GraphicsContext openTK = null;
        private OpenTK.Platform.IWindowInfo windowInfo = null;

        private NSOpenGLContext openGLContext;
        private NSOpenGLPixelFormat pixelFormat;

        private CVDisplayLink displayLink;

        private NSObject notificationProxy;


        public MacGLView2() : base(NSObjectFlag.Empty)
        {
           
            // Look for changes in view size
            // Note, -reshape will not be called automatically on size changes because NSView does not export it to override 
            notificationProxy = NSNotificationCenter.DefaultCenter.AddObserver(NSView.GlobalFrameChangedNotification, HandleReshape);
        }

        public override void DrawRect(System.Drawing.RectangleF dirtyRect)
        {
            if (!this.IsInitialized)
                InitGL ();
            // Ignore if the display link is still running
            if( !displayLink.IsRunning )
                DrawView();
        }

        public override bool AcceptsFirstResponder()
        {
            // We want this view to be able to receive key events
            return true;
        }

        public override void LockFocus()
        {
            base.LockFocus();
            if( openGLContext.View != this )
                openGLContext.View = this;
        }

        public override void KeyDown(NSEvent theEvent)
        {
            GLKeyDown(this, theEvent);
        }

        public override void MouseDown(NSEvent theEvent)
        {
            GLMouseDown(this, theEvent);
        }

        // All Setup For OpenGL Goes Here
        public bool InitGL()
        {
            pixelFormat = NSOpenGLView.DefaultPixelFormat;

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

            //// Enables Smooth Shading  
            //GL.ShadeModel(ShadingModel.Smooth);
            //// Set background color to black     
            //GL.ClearColor(Color.Black);

            //// Setup Depth Testing

            //// Depth Buffer setup
            //GL.ClearDepth(1.0);
            //// Enables Depth testing
            //GL.Enable(EnableCap.DepthTest);
            //// The type of depth testing to do
            //GL.DepthFunc(DepthFunction.Lequal);

            //// Really Nice Perspective Calculations
            //GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);

            var ctxPtr = this.openGLContext.CGLContext.Handle;
            var ctxHandle = new OpenTK.ContextHandle(ctxPtr);

            //Initialize OpenTK structures using OSX GL core handles.
            this.windowInfo = OpenTK.Platform.Utilities.CreateMacOSCarbonWindowInfo(this.Handle, false, true);
            this.openTK = new OpenTK.Graphics.GraphicsContext(ctxHandle, windowInfo);


            // Create a display link capable of being used with all active displays
            displayLink = new CVDisplayLink();

            // Set the renderer output callback function
            displayLink.SetOutputCallback(MyDisplayLinkOutputCallback);

            // Set the display link for the current renderer
            CGLContext cglContext = openGLContext.CGLContext;
            CGLPixelFormat cglPixelFormat = PixelFormat.CGLPixelFormat;
            displayLink.SetCurrentDisplay(cglContext, cglPixelFormat);


            this.IsInitialized = true;

            this.Initialized(this, EventArgs.Empty);

            return true;
        }

        private void DrawView()
        {
            // This method will be called on both the main thread (through -drawRect:) and a secondary thread (through the display link rendering loop)
            // Also, when resizing the view, -reshape is called on the main thread, but we may be drawing on a secondary thread
            // Add a mutex around to avoid the threads accessing the context simultaneously 
            openGLContext.CGLContext.Lock();

            // Make sure we draw to the right context
            openGLContext.MakeCurrentContext();

            // Delegate to the scene object for rendering
            //controller.Scene.DrawGLScene();
            this.DrawNow(this, EventArgs.Empty);

            openGLContext.FlushBuffer();

            openGLContext.CGLContext.Unlock();
        }


        private void SetupDisplayLink()
        {


        }

        public CVReturn MyDisplayLinkOutputCallback(CVDisplayLink displayLink, ref CVTimeStamp inNow, ref CVTimeStamp inOutputTime, CVOptionFlags flagsIn,
            ref CVOptionFlags flagsOut)
        {
            CVReturn result = GetFrameForTime(inOutputTime);

            return result;
        }


        private CVReturn GetFrameForTime(CVTimeStamp outputTime)
        {
            // There is no autorelease pool when this method is called because it will be called from a background thread
            // It's important to create one or you will leak objects
            using( NSAutoreleasePool pool = new NSAutoreleasePool() )
            {

                // Update the animation
                BeginInvokeOnMainThread(DrawView);
            }

            return CVReturn.Success;

        }

        public NSOpenGLContext OpenGLContext
        {
            get { return openGLContext; }
        }

        public NSOpenGLPixelFormat PixelFormat
        {
            get { return pixelFormat; }
        }

        public void UpdateView()
        {
            if (!this.IsInitialized)
                return;
            // This method will be called on the main thread when resizing, but we may be drawing on a secondary thread through the display link
            // Add a mutex around to avoid the threads accessing the context simultaneously
            openGLContext.CGLContext.Lock();

            // Delegate to the scene object to update for a change in the view size
            //controller.Scene.ResizeGLScene(Bounds);
            this.Resize(this, EventArgs.Empty);

            openGLContext.Update();

            openGLContext.CGLContext.Unlock();
        }

        private void HandleReshape(NSNotification note)
        {
            UpdateView();
        }

        public void StartAnimation()
        {
            if( displayLink != null && !displayLink.IsRunning )
                displayLink.Start();
        }

        public void StopAnimation()
        {
            if( displayLink != null && displayLink.IsRunning )
                displayLink.Stop();
        }

        // Clean up the notifications
        public void DeAllocate()
        {
            displayLink.Stop();
            displayLink.SetOutputCallback(null);

            NSNotificationCenter.DefaultCenter.RemoveObserver(notificationProxy);
        }

        //[Export("toggleFullScreen:")]
        //public void toggleFullScreen(NSObject sender)
        //{
        //    controller.toggleFullScreen(sender);
        //}

        public Eto.Drawing.Size GLSize
        {
            get { return this.Bounds.Size.ToSize().ToEto(); }
            set { }
        }

        public bool IsInitialized { get; private set; }

        public void MakeCurrent()
        {
            this.openGLContext.MakeCurrentContext();
            this.openTK.MakeCurrent(this.windowInfo);
        }

        public void SwapBuffers()
        {
            this.openTK.SwapBuffers();
        }

        public event EventHandler Initialized = delegate { };
        public event EventHandler Resize = delegate { };
        public event EventHandler ShuttingDown = delegate { };
        public event EventHandler DrawNow = delegate { };

        public event GLEventHandler GLKeyDown = delegate { };

        public event EventHandler<KeyEventArgs> GlKeyUp = delegate { };

        public event GLEventHandler GLMouseDown = delegate { };

        public event GLEventHandler GLMouseUp = delegate { };
        public event GLEventHandler GLMouseDragged = delegate { };
        public event GLEventHandler GLMouseMoved = delegate { };
        public event GLEventHandler GLScrollWheel = delegate { };


    }
}