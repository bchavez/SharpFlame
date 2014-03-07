using Eto.Platform.Windows;

namespace Eto.Gl.Windows
{
    public class WinPanelHandler : PanelHandler
    {
        public override void AttachEvent( string id )
        {
            if ( id == "Control.Shown" )
            {
                this.Control.VisibleChanged += Panel_VisibleChanged;
                return;
            }

            base.AttachEvent( id );
        }

        void Panel_VisibleChanged( object sender, System.EventArgs e )
        {
            if ( this.Control.Visible )
            {
                this.Widget.OnShown(e);
            }
        }
    }
    
}