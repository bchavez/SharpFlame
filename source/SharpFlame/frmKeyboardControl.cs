using System;
using System.Windows.Forms;

namespace SharpFlame
{
	public partial class frmKeyboardControl
	{
		
		public modLists.SimpleList<clsContainer<Keys>> Results = new modLists.SimpleList<clsContainer<Keys>>();
		
		public frmKeyboardControl()
		{
			InitializeComponent();
			
			Icon = modProgram.ProgramIcon;
			
			UpdateLabel();
		}
		
		private void UpdateLabel()
		{
			
			lblKeys.Text = "";
			for (int i = 0; i <= Results.Count - 1; i++)
			{
				Keys key = System.Windows.Forms.Keys.A;
				string text = Enum.GetName(typeof(Keys), key);
				if (text == null)
				{
					lblKeys.Text += modIO.InvariantToString_int(System.Convert.ToInt32(Results[i].Item));
				}
				else
				{
					lblKeys.Text += text;
				}
				lblKeys.Text += " ";
			}
		}
		
		public void btnSave_Click(System.Object sender, System.EventArgs e)
		{
			
			DialogResult = System.Windows.Forms.DialogResult.OK;
		}
		
		public void btnCancel_Click(System.Object sender, System.EventArgs e)
		{
			
			DialogResult = System.Windows.Forms.DialogResult.Cancel;
		}
		
		public void frmKeyboardControl_KeyDown(object sender, System.Windows.Forms.KeyEventArgs e)
		{
			
			if (Results.Count > 8)
			{
				return;
			}
			foreach (clsContainer<Keys> key in Results)
			{
				if (key.Item == e.KeyCode)
				{
					return;
				}
			}
			Results.Add(new clsContainer<Keys>(e.KeyCode));
			UpdateLabel();
		}
	}
}
