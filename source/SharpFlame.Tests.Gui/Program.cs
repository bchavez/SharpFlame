using System;
using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;
using Eto.Gl;
using Eto.Gl.Windows;
using OpenTK;
using Application = Eto.Forms.Application;

namespace SharpFlame.Tests.Gui
{
    internal static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        private static void Main()
        {
            //check
            try
            {
                Toolkit.Init();
            }
            catch
            {
                Debugger.Break();
            }
            var gen = Eto.Platform.Detect;

            //gen.Add<GLSurface.IHandler>(() => new MacGLSurfaceHandler());
            gen.Add<GLSurface.IHandler>(() => new WinGLSurfaceHandler());

            new Application().Run(new MainForm());
        }
    }

    public class MainForm : Form
    {
        public MainForm()
        {
            this.ClientSize = new Size(1024, 768);

            var leftlayout = new DynamicLayout();
            leftlayout.BackgroundColor = Color.FromArgb(0, 255, 0);

            var cmdImgButton = new Button()
                {
                    Text = "test",
                };

            leftlayout.Add(cmdImgButton);

            var left = new Panel()
                {
                    BackgroundColor = Color.FromArgb(255, 0, 0),
                    Content = leftlayout
                };
            

            var gl = new GLSurface();

            var splitter = new Splitter()
                {
                    Position = 392,
                    FixedPanel = SplitterFixedPanel.Panel1,
                    Panel1 = left,
                    Panel2 = gl,
                };

            this.Content = splitter;

        }
    }




}

