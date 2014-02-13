namespace SharpFlame
{
	partial class frmClose : System.Windows.Forms.Form
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
			this.btnSave = new System.Windows.Forms.Button();
			this.btnClose = new System.Windows.Forms.Button();
			this.btnCancel = new System.Windows.Forms.Button();
			this.btnQuickSave = new System.Windows.Forms.Button();
			this.SuspendLayout();
			//
			//Label1
			//
			this.Label1.AutoSize = true;
			this.Label1.Location = new System.Drawing.Point(40, 20);
			this.Label1.Name = "Label1";
			this.Label1.Size = new System.Drawing.Size(282, 20);
			this.Label1.TabIndex = 0;
			this.Label1.Text = "This map has changed. Save before closing it?";
			this.Label1.UseCompatibleTextRendering = true;
			//
			//btnSave
			//
			this.btnSave.DialogResult = System.Windows.Forms.DialogResult.OK;
			this.btnSave.Location = new System.Drawing.Point(12, 63);
			this.btnSave.Name = "btnSave";
			this.btnSave.Size = new System.Drawing.Size(96, 32);
			this.btnSave.TabIndex = 1;
			this.btnSave.Text = "Save As...";
			this.btnSave.UseCompatibleTextRendering = true;
			this.btnSave.UseVisualStyleBackColor = true;
			//
			//btnClose
			//
			this.btnClose.DialogResult = System.Windows.Forms.DialogResult.No;
			this.btnClose.Location = new System.Drawing.Point(216, 63);
			this.btnClose.Name = "btnClose";
			this.btnClose.Size = new System.Drawing.Size(96, 32);
			this.btnClose.TabIndex = 3;
			this.btnClose.Text = "Close";
			this.btnClose.UseCompatibleTextRendering = true;
			this.btnClose.UseVisualStyleBackColor = true;
			//
			//btnCancel
			//
			this.btnCancel.DialogResult = System.Windows.Forms.DialogResult.Cancel;
			this.btnCancel.Location = new System.Drawing.Point(318, 63);
			this.btnCancel.Name = "btnCancel";
			this.btnCancel.Size = new System.Drawing.Size(96, 32);
			this.btnCancel.TabIndex = 4;
			this.btnCancel.Text = "Cancel";
			this.btnCancel.UseCompatibleTextRendering = true;
			this.btnCancel.UseVisualStyleBackColor = true;
			//
			//btnQuickSave
			//
			this.btnQuickSave.DialogResult = System.Windows.Forms.DialogResult.Yes;
			this.btnQuickSave.Location = new System.Drawing.Point(114, 63);
			this.btnQuickSave.Name = "btnQuickSave";
			this.btnQuickSave.Size = new System.Drawing.Size(96, 32);
			this.btnQuickSave.TabIndex = 2;
			this.btnQuickSave.Text = "Quick Save";
			this.btnQuickSave.UseCompatibleTextRendering = true;
			this.btnQuickSave.UseVisualStyleBackColor = true;
			//
			//frmClose
			//
			this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.None;
			this.CancelButton = this.btnCancel;
			this.ClientSize = new System.Drawing.Size(425, 107);
			this.Controls.Add(this.btnQuickSave);
			this.Controls.Add(this.btnCancel);
			this.Controls.Add(this.btnClose);
			this.Controls.Add(this.btnSave);
			this.Controls.Add(this.Label1);
			this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
			this.MaximizeBox = false;
			this.MinimizeBox = false;
			this.Name = "frmClose";
			this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
			this.TopMost = true;
			this.ResumeLayout(false);
			this.PerformLayout();
			
		}
		internal System.Windows.Forms.Label Label1;
		internal System.Windows.Forms.Button btnSave;
		internal System.Windows.Forms.Button btnClose;
		internal System.Windows.Forms.Button btnCancel;
		internal System.Windows.Forms.Button btnQuickSave;
	}
	
}
