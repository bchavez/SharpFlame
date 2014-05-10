

using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class ResizeTab : Panel
	{
        private readonly Options uiOptions;

        public ResizeTab (Options argUiOptions)
		{
            uiOptions = argUiOptions;

            var mainLayout = new DynamicLayout ();

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

            SetupEventHandlers ();

			Content = mainLayout;
		}

        void SetupEventHandlers() {
            // Set Mousetool, when we are shown.
            Shown += delegate {
                uiOptions.MouseTool = MouseTool.Default;
            };
        }
	}
}

