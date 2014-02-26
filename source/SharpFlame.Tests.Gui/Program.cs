using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using System.Windows.Forms;
using OpenTK;

namespace SharpFlame.Tests.Gui
{
    static class Program
    {
        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault( false );

            //check
            try
            {
                //var x = OpenTK.Platform.Egl.
                Toolkit.Init();
            }
            catch
            {
                var test = 1 + 1;
            }


            Application.Run( new frmMain() );
        }
    }
}
