//
// The Open Toolkit Library License
//
// Copyright (c) 2006 - 2009 the Open Toolkit library, except where noted.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights to 
// use, copy, modify, merge, publish, distribute, sublicense, and/or sell copies of
// the Software, and to permit persons to whom the Software is furnished to do
// so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all
// copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND,
// EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES
// OF MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND
// NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT
// HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY,
// WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING
// FROM, OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR
// OTHER DEALINGS IN THE SOFTWARE.
//

using System;
using System.ComponentModel;
using System.Drawing;
using System.Windows.Forms;
using Eto.WinForms;
using OpenTK;
using OpenTK.Graphics;
using OpenTK.Platform;
using ef = Eto.Forms;

namespace Eto.Gl.Windows
{
    /// <summary>
    /// OpenGL-aware WinForms control.
    /// The WinForms designer will always call the default constructor.
    /// Inherit from this class and call one of its specialized constructors
    /// to enable antialiasing or custom <see cref="GraphicsMode"/>s.
    /// </summary>
    public partial class WinGLUserControl : UserControl
    {
        IGraphicsContext context;
        readonly GraphicsMode graphicsMode;
        readonly int major;
        readonly int minor;
        readonly GraphicsContextFlags flags;
        bool? initialVsyncValue;
        // Indicates that OnResize was called before OnHandleCreated.
        // To avoid issues with missing OpenGL contexts, we suppress
        // the premature Resize event and raise it as soon as the handle
        // is ready.
        bool resizeEventSuppressed;
        private IWindowInfo windowInfo;

        /// <summary>
        /// Constructs a new instance.
        /// </summary>
        public WinGLUserControl()
            : this(GraphicsMode.Default)
        { }

        /// <summary>
        /// Constructs a new instance with the specified GraphicsMode.
        /// </summary>
        /// <param name="mode">The OpenTK.Graphics.GraphicsMode of the control.</param>
        public WinGLUserControl(GraphicsMode mode)
            : this(mode, 3, 0, GraphicsContextFlags.Default)
        { }

        /// <summary>
        /// Constructs a new instance with the specified GraphicsMode.
        /// </summary>
        /// <param name="mode">The OpenTK.Graphics.GraphicsMode of the control.</param>
        /// <param name="major">The major version for the OpenGL GraphicsContext.</param>
        /// <param name="minor">The minor version for the OpenGL GraphicsContext.</param>
        /// <param name="flags">The GraphicsContextFlags for the OpenGL GraphicsContext.</param>
        public WinGLUserControl(GraphicsMode mode, int major, int minor, GraphicsContextFlags flags)
        {
            if( mode == null )
                throw new ArgumentNullException("mode");

            // SDL does not currently support embedding
            // on external windows. If Open.Toolkit is not yet
            // initialized, we'll try to request a native backend
            // that supports embedding.
            // Most people are using GLControl through the
            // WinForms designer in Visual Studio. This approach
            // works perfectly in that case.
            Toolkit.Init(new ToolkitOptions
            {
                Backend = PlatformBackend.PreferNative
            });

            SetStyle(ControlStyles.Opaque, true);
            SetStyle(ControlStyles.UserPaint, true);
            SetStyle(ControlStyles.AllPaintingInWmPaint, true);
            DoubleBuffered = false;

            this.graphicsMode = mode;
            this.major = major;
            this.minor = minor;
            this.flags = flags;

            InitializeComponent();

            this.Disposed += WinGLUserControl_Disposed;
            /*this.KeyDown += Control_KeyDown;
            this.KeyPress += Control_KeyPress;
            this.KeyUp += Control_KeyUp;*/
        }

        void WinGLUserControl_Disposed(object sender, EventArgs e)
        {
            this.ShuttingDown(sender, e);
        }

        private void EnsureValidHandle()
        {
            if( IsDisposed )
                throw new ObjectDisposedException(GetType().Name);

            if( !IsHandleCreated )
                CreateControl();

            if( windowInfo == null || context == null || context.IsDisposed )
                RecreateHandle();
        }


        /// <summary>
        /// Gets the <c>CreateParams</c> instance for this <c>GLControl</c>
        /// </summary>
        protected override CreateParams CreateParams
        {
            get
            {
                const int CS_VREDRAW = 0x1;
                const int CS_HREDRAW = 0x2;
                const int CS_OWNDC = 0x20;

                CreateParams cp = base.CreateParams;
                if( Configuration.RunningOnWindows )
                {
                    // Setup necessary class style for OpenGL on windows
                    cp.ClassStyle |= CS_VREDRAW | CS_HREDRAW | CS_OWNDC;
                }
                return cp;
            }
        }

        /// <summary>Raises the HandleCreated event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleCreated(EventArgs e)
        {
            if( context != null )
                context.Dispose();

            if( windowInfo != null )
                windowInfo.Dispose();

            this.windowInfo = Utilities.CreateWindowsWindowInfo(this.Handle);

            this.context = new GraphicsContext(this.graphicsMode, this.windowInfo, major, minor, flags);

            MakeCurrent();

            ( (IGraphicsContextInternal)Context ).LoadAll();

            if( !this.IsInitialized )
            {
                this.IsInitialized = true;
                this.Initialized(this, EventArgs.Empty);
            }

            // Deferred setting of vsync mode. See VSync property for more information.
            if( initialVsyncValue.HasValue )
            {
                Context.SwapInterval = initialVsyncValue.Value ? 1 : 0;
                initialVsyncValue = null;
            }

            base.OnHandleCreated(e);

            if( resizeEventSuppressed )
            {
                OnResize(EventArgs.Empty);
                resizeEventSuppressed = false;
            }
        }

        /// <summary>Raises the HandleDestroyed event.</summary>
        /// <param name="e">Not used.</param>
        protected override void OnHandleDestroyed(EventArgs e)
        {
            if( context != null )
            {
                context.Dispose();
                context = null;
            }

            if( windowInfo != null )
            {
                windowInfo.Dispose();
                windowInfo = null;
            }

            base.OnHandleDestroyed(e);
        }

        /// <summary>
        /// Raises the System.Windows.Forms.Control.Paint event.
        /// </summary>
        /// <param name="e">A System.Windows.Forms.PaintEventArgs that contains the event data.</param>
        protected override void OnPaint(PaintEventArgs e)
        {
            EnsureValidHandle();

            base.OnPaint(e);
        }

        /// <summary>
        /// Raises the Resize event.
        /// Note: this method may be called before the OpenGL context is ready.
        /// Check that IsHandleCreated is true before using any OpenGL methods.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnResize(EventArgs e)
        {
            // Do not raise OnResize event before the handle and context are created.
            if( !IsHandleCreated )
            {
                resizeEventSuppressed = true;
                return;
            }

            if( context != null )
            {
                EnsureValidHandle();
                context.Update(windowInfo);
            }

            base.OnResize(e);
        }

        /// <summary>
        /// Raises the ParentChanged event.
        /// </summary>
        /// <param name="e">A System.EventArgs that contains the event data.</param>
        protected override void OnParentChanged(EventArgs e)
        {
            if( context != null )
            {
                EnsureValidHandle();
                context.Update(windowInfo);
            }

            base.OnParentChanged(e);
        }


        /// <summary>
        /// Swaps the front and back buffers, presenting the rendered scene to the screen.
        /// </summary>
        public void SwapBuffers()
        {
            EnsureValidHandle();
            Context.SwapBuffers();
        }

        public event EventHandler Initialized = delegate { };
        public event EventHandler ShuttingDown = delegate { };

        public bool IsInitialized { get; private set; }

        /// <summary>
        /// Makes the underlying this GLControl current in the calling thread.
        /// All OpenGL commands issued are hereafter interpreted by this GLControl.
        /// </summary>
        public void MakeCurrent()
        {
            EnsureValidHandle();
            Context.MakeCurrent(windowInfo);
        }


        /// <summary>
        /// Gets a value indicating whether the current thread contains pending system messages.
        /// </summary>
        [Browsable(false)]
        public bool IsIdle
        {
            get
            {
                EnsureValidHandle();
                return Win32Helper.IsIdle;
            }
        }
        /// <summary>
        /// Gets an interface to the underlying GraphicsContext used by this GLControl.
        /// </summary>
        [Browsable(false)]
        public IGraphicsContext Context
        {
            get
            {
                EnsureValidHandle();
                return context;
            }
            private set { context = value; }
        }


        /// <summary>
        /// Gets the aspect ratio of this GLControl.
        /// </summary>
        [Description("The aspect ratio of the client area of this GLControl.")]
        public float AspectRatio
        {
            get
            {
                EnsureValidHandle();
                return ClientSize.Width / (float)ClientSize.Height;
            }
        }

        /// <summary>
        /// Gets or sets a value indicating whether vsync is active for this GLControl.
        /// </summary>
        [Description("Indicates whether GLControl updates are synced to the monitor's refresh rate.")]
        public bool VSync
        {
            get
            {
                if( !IsHandleCreated )
                    return false;

                EnsureValidHandle();
                return Context.VSync;
            }
            set
            {
                // The winforms designer sets this to false by default which forces control creation.
                // However, events are typically connected after the VSync = false assignment, which
                // can lead to "event xyz is not fired" issues.
                // Work around this issue by deferring VSync mode setting to the HandleCreated event.
                if( !IsHandleCreated )
                {
                    initialVsyncValue = value;
                    return;
                }

                EnsureValidHandle();
                Context.VSync = value;
            }
        }

        /// <summary>
        /// Gets the GraphicsMode of the GraphicsContext attached to this GLControl.
        /// </summary>
        /// <remarks>
        /// To change the GraphicsMode, you must destroy and recreate the GLControl.
        /// </remarks>
        public GraphicsMode GraphicsMode
        {
            get
            {
                EnsureValidHandle();
                return Context.GraphicsMode;
            }
        }


        /// <summary>
        /// Gets the <see cref="OpenTK.Platform.IWindowInfo"/> for this instance.
        /// </summary>
        public IWindowInfo WindowInfo
        {
            get { return windowInfo; }
        }

        public bool HasFocus
        {
            get { return Focused; }
        }

        public new void Focus()
        {
            if( IsHandleCreated )
            {
                base.Focus();
            }
        }

        /*public event EventHandler<ef.KeyEventArgs> GlKeyDown = delegate { };
        public event EventHandler<ef.KeyEventArgs> GlKeyUp = delegate { };
        */

        /*
    ef.Keys key;
    bool handled;
    char keyChar;
    bool charPressed;
    public ef.Keys? LastKeyDown { get; set; }
    */
        /*
        void Control_KeyDown(object sender, KeyEventArgs e)
        {
            charPressed = false;
            handled = true;
            key = e.KeyData.ToEto();

            if( key != ef.Keys.None && LastKeyDown != key )
            {
                var kpea = new ef.KeyEventArgs(key, ef.KeyEventType.KeyDown);
                GlKeyDown(this, kpea);

                handled = e.SuppressKeyPress = e.Handled = kpea.Handled;
            }
            else
                handled = false;

            if( !handled && charPressed )
            {
                // this is when something in the event causes messages to be processed for some reason (e.g. show dialog box)
                // we want the char event to come after the dialog is closed, and handled is set to true!
                var kpea = new ef.KeyEventArgs(key, ef.KeyEventType.KeyDown, keyChar);
                GlKeyDown(this, kpea);
                e.SuppressKeyPress = e.Handled = kpea.Handled;
            }

            LastKeyDown = null;
        }

        void Control_KeyPress(object sender, System.Windows.Forms.KeyPressEventArgs e)
        {
            charPressed = true;
            keyChar = e.KeyChar;
            if( !handled )
            {
                var kpea = new ef.KeyEventArgs(key, ef.KeyEventType.KeyDown, keyChar);
                GlKeyDown(this, kpea);
                e.Handled = kpea.Handled;
            }
            else
                e.Handled = true;
        }

        void Control_KeyUp(object sender, System.Windows.Forms.KeyEventArgs e)
        {
            key = e.KeyData.ToEto();

            var kpea = new ef.KeyEventArgs(key, ef.KeyEventType.KeyUp);
            GlKeyUp(this, kpea);
            e.Handled = kpea.Handled;
        }
        */
    }
}
