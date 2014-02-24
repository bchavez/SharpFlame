using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;

namespace SharpFlame.Gui
{
	public class ResizeTab : Panel
	{
		public ResizeTab ()
		{
			var mainLayout = new DynamicLayout ();

			var nLayout1 = new DynamicLayout ();
			nLayout1.AddRow(new Label { Text = "Size X:" }, 
							new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Size Y:" }, 
							new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Offset X:" }, 
			new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			nLayout1.AddRow(new Label { Text = "Offset Y:" }, 
			new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = Constants.MapMaxSize, MinValue = 1 });

			mainLayout.AddRow (null, nLayout1, null);

			mainLayout.AddRow (null, new Button { Text = "Resize" }, null);
			mainLayout.AddRow (null, new Button { Text = "Resize To Selection" }, null);


			mainLayout.Add (null);

			Content = mainLayout;
		}
	}
}

