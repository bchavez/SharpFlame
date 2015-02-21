

using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame.Core.Extensions;
using System;
using SharpFlame.Util;

namespace SharpFlame.Gui.Dialogs
{
	public class About : Dialog
	{
	    protected Label lblVersion;
	    protected Label lblName;
	    protected ImageView imgLogo;
        protected Label lblCopy0;
        protected Label lblCopy1;
	    protected Button cmdClose;

	    public About()
	    {
	        XomlReader.Load(this);

	        BindSetup();
	    }

	    private void BindSetup()
	    {
            this.lblName.Text = Constants.ProgramName;
	        this.lblName.Font = new Font(SystemFont.Bold, 20);

            this.lblVersion.Text = "Version {0}".Format2(Constants.ProgramVersion());
	        this.lblVersion.Font = new Font(SystemFont.Default, 10);

            this.imgLogo.Image = Resources.ProgramIcon;

            this.lblCopy0.Font = new Font(SystemFont.Default, 10);
            this.lblCopy1.Font = new Font(SystemFont.Default, 10);
            
	        this.DefaultButton = cmdClose;
	        this.AbortButton = cmdClose;
	    }       

	    protected void cmdClose_Click(object sender, EventArgs e)
	    {
	        this.Close();
	    }
	}
}

