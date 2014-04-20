

using System;
using System.IO;
using System.Threading;
using System.Windows.Forms;
using OpenTK;
using OpenTK.Graphics;
using SharpFlame.Core;
using Timer = System.Windows.Forms.Timer;


namespace SharpFlame
{
    public sealed class Program
    {
        public static Timer InitializeDelay;
        public static Result InitializeResult = new Result("Startup result", false);

        public static frmMain frmMainInstance;
        public static frmGenerator frmGeneratorInstance;

        public static GLControl OpenGL1;
        public static GLControl OpenGL2;       
    }
}