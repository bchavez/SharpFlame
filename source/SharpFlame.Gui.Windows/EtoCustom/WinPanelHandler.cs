using Eto.WinForms.Forms.Controls;

namespace SharpFlame.Gui.Windows.EtoCustom
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
                //ETO 1.0
                //this.Widget.OnShown(e);

                this.Widget.Properties.TriggerEvent(Eto.Forms.Control.ShownEvent, this, e);
            }
        }
    }
    
}