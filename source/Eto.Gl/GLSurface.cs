#region License
/*
 The MIT License (MIT)

 Copyright (c) 2013-2014 The SharpFlame Authors.

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 */
#endregion

using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace Eto.Gl
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
    }
}