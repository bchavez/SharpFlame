using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame.Gui.Forms;


namespace SharpFlame.Gui
{
	public class SharpFlameApplication : Application
	{
		public SharpFlameApplication(Generator generator)
			: base(generator)
		{
			// Allows manual Button size on GTK2.
			Button.DefaultSize = new Size (1, 1);

			this.Name = string.Format ("{0} {1}", Constants.ProgramName, Constants.ProgramVersionNumber);
			this.Style = "application";
		}

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm();

			base.OnInitialized(e);

			// show the main form
			MainForm.Show();
		}

		public override void OnTerminating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating(e);

			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
	}
}

