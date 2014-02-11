namespace SharpFlame
{
	[global::Microsoft.VisualBasic.CompilerServices.DesignerGenerated()]public 
	partial class frmKeyboardControl : System.Windows.Forms.Form
	{
		
		//Form overrides dispose to clean up the component list.
		[System.Diagnostics.DebuggerNonUserCode()]protected override void Dispose(bool disposing)
		{
			try
			{
				if (disposing && components != null)
				{
					components.Dispose();
				}
			}
			finally
			{
				base.Dispose(disposing);
			}
		}
		
		//Required by the Windows Form Designer
		private System.ComponentModel.Container components = null;
		
		//NOTE: The following procedure is required by the Windows Form Designer
		//It can be modified using the Windows Form Designer.
		//Do not modify it using the code editor.
		[System.Diagnostics.DebuggerStepThrough()]private void InitializeComponent()
		{
			this.Label1 = new System.Windows.Forms.Label();
			this.KeyDown += frmKeyboardControl_KeyDown;
			this.lblKeys = new System.Windows.Forms.Label();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnCancel.Click += this.btnCancel_Click;
			this.btnSave = new System.Windows.Forms.Button();
			this.btnSave.Click += this.btnSave_Click;
			this.SuspendLayout();
			//
			//Label1
			//
			this.Label1.Location = new System.Drawing.Point(12, 9);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(225, 21);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "Press keys...";
			this.Label1.UseCompatibleTextRendering = true;
			//
			//lblKeys
			//
			this.lblKeys.Location = new System.Drawing.Point(35, 30);
			this.lblKeys.Name = "lblKeys";
			this.lblKeys.Size = new System.Drawing.Size(320, 44);
			this.lblKeys.TabIndex = 1;
			this.lblKeys.Text = "keys...";
			this.lblKeys.UseCompatibleTextRendering = true;
			//
			//btnCancel
			//
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.No;
			this.btnCancel.Location = new System.Drawing.Point(251, 69);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 32);
			this.btnCancel.TabIndex = 5;
			this.btnCancel.TabStop = false;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseCompatibleTextRendering = true;
			this.btnCancel.UseVisualStyleBackColor = true;
			//
			//btnSave
			//
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Location = new System.Drawing.Point(149, 69);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(96, 32);
			this.btnSave.TabIndex = 4;
			this.btnSave.TabStop = false;
			this.btnSave.Text = "Accept";
			this.btnSave.UseCompatibleTextRendering = true;
			this.btnSave.UseVisualStyleBackColor = true;
			//
			//frmKeyboardControl
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.ClientSize = new System.Drawing.Size(359, 113);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.lblKeys);
			this.Controls.Add(this.Label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedSingle;
			this.MaximizeBox = false;
			this.Name = "frmKeyboardControl";
			this.Text = "Keyboard Control";
			this.ResumeLayout(false);
			
		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Label lblKeys;
		internal System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.Button btnSave;
	}
	
}
