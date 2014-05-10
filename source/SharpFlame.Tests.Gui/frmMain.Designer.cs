namespace SharpFlame.Tests.Gui
{
    partial class frmMain
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose( bool disposing )
        {
            if( disposing && ( components != null ) )
            {
                components.Dispose();
            }
            base.Dispose( disposing );
        }

        
        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.glc = new OpenTK.GLControl();
            this.SuspendLayout();
            // 
            // glc
            // 
            this.glc.BackColor = System.Drawing.Color.Black;
            this.glc.Location = new System.Drawing.Point(102, 83);
            this.glc.Name = "glc";
            this.glc.Size = new System.Drawing.Size(395, 193);
            this.glc.TabIndex = 0;
            this.glc.VSync = false;
            this.glc.Load += new System.EventHandler(this.glc_Load);
            this.glc.Paint += new System.Windows.Forms.PaintEventHandler(this.glc_Paint);
            this.glc.Resize += new System.EventHandler(this.glc_Resize);
            // 
            // frmMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(550, 369);
            this.Controls.Add(this.glc);
            this.Name = "frmMain";
            this.Text = "Test Harness";
            this.ResumeLayout(false);

        }

        

        private OpenTK.GLControl glc;

    }
}

