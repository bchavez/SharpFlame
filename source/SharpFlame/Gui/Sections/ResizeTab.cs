using System;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class ResizeTab : TabPage
	{
        [Inject]
        public Options UiOptions { get; set; }

        public ResizeTab ()
        {
            XomlReader.Load(this);

            /*var mainLayout = new DynamicLayout ();

			var nLayout1 = new DynamicLayout ();
			nLayout1.AddRow(new Label { Text = "Size X:" }, 
							new NumericUpDown { Size = new Size(-1, -1), Value = 1, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Size Y:" }, 
							new NumericUpDown { Size = new Size(-1, -1), Value = 1, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Offset X:" }, 
			                new NumericUpDown { Size = new Size(-1, -1), Value = 1, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Offset Y:" }, 
			                new NumericUpDown { Size = new Size(-1, -1), Value = 1, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			mainLayout.AddRow (null, nLayout1, null);

			mainLayout.AddRow (null, new Button { Text = "Resize" }, null);
			mainLayout.AddRow (null, new Button { Text = "Resize To Selection" }, null);


			mainLayout.Add (null);

			Content = mainLayout;*/
		}

	    protected void cmdResize_Click(object sender, EventArgs e)
	    {
	        
	    }

	    protected void cmdResizeSelection_Click(object sender, EventArgs e)
	    {
	        
	    }

	    protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);

            // Set Mousetool, when we are shown.
            Shown += delegate {
                UiOptions.MouseTool = MouseTool.Default;
            };
        }
	}
}

