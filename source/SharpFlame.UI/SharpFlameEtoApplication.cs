using System;
using Eto;
using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Test.UI
{
	/// <summary>
	/// Set this class as your startup object
	/// </summary>
	public class SharpFlameEtoApplication : Application
	{
	    public SharpFlameEtoApplication(Generator g) : base(g)
	    {
	    }

	    /// <summary>
		/// Handles when the application is initialized and running
		/// </summary>
		public override void OnInitialized(EventArgs e)
		{
			base.OnInitialized(e);

			MainForm = new ExampleForm();
			MainForm.Show();
		}
	}
}
