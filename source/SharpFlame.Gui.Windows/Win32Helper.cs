using System;

namespace SharpFlame.Gui.Windows
{
    internal class Win32Helper
    {
        #region P/Invoke declarations

        #region Message

        struct MSG
        {
            public IntPtr HWnd;
            public uint Message;
            public IntPtr WParam;
            public IntPtr LParam;
            public uint Time;
            public POINT Point;
            //internal object RefObject;

            public override string ToString()
            {
                return String.Format( "msg=0x{0:x} ({1}) hwnd=0x{2:x} wparam=0x{3:x} lparam=0x{4:x} pt=0x{5:x}", (int)Message, Message.ToString(), HWnd.ToInt32(), WParam.ToInt32(), LParam.ToInt32(), Point );
            }
        }

        #endregion

        #region Point

        struct POINT
        {
            public int X;
            public int Y;

            public POINT( int x, int y )
            {
                this.X = x;
                this.Y = y;
            }

            public System.Drawing.Point ToPoint()
            {
                return new System.Drawing.Point( X, Y );
            }

            public override string ToString()
            {
                return "Point {" + X.ToString() + ", " + Y.ToString() + ")";
            }
        }

        #endregion

        #region PeekMessage

        [System.Security.SuppressUnmanagedCodeSecurity]
        [System.Runtime.InteropServices.DllImport( "User32.dll" )]
        static extern bool PeekMessage( ref MSG msg, IntPtr hWnd, int messageFilterMin, int messageFilterMax, int flags );

        #endregion

        #region

        #endregion

        #endregion

        #region Fields

        static MSG msg = new MSG();

        #endregion


        #region IGLControl Members

        public static bool IsIdle
        {
            get { return !PeekMessage( ref msg, IntPtr.Zero, 0, 0, 0 ); }
        }

        #endregion
    }
}