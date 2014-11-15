using System;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
    public class LabelsTab : Panel
    {
        private readonly Options uiOptions;

        public LabelsTab (Options argUiOptions)
        {
            uiOptions = argUiOptions;

            var mainLayout = new DynamicLayout ();

            var nLayout0 = new DynamicLayout ();
            nLayout0.AddRow(new Button { Text = "Create Area From Selection" }, 
                            new Label { Text = "Hold P and click to make positions.", VerticalAlign = VerticalAlign.Middle, Size = new Size (150, 28) });
            mainLayout.AddRow (null, nLayout0, null);

            var nLayout1 = new DynamicLayout ();
            nLayout1.AddRow (new Label { Text = "Positions:", VerticalAlign = VerticalAlign.Bottom },
                             new Label { Text = "Areas:", VerticalAlign = VerticalAlign.Bottom });
            nLayout1.AddRow (new ListBox { Size = new Size (170, 150) }, new ListBox { Size = new Size (170, 150) });

            var smGroupBox = new GroupBox { Text = "Selected Marker" };
            var nLayout2 = new DynamicLayout ();
            var nLayout3 = new DynamicLayout ();
            nLayout3.AddRow (new Label { Text = "Label:", VerticalAlign = VerticalAlign.Middle },
                             new TextBox());
            nLayout2.Add (nLayout3);

            var nLayout4 = new DynamicLayout ();
            nLayout4.AddRow(new Label { Text = "x:", VerticalAlign = VerticalAlign.Bottom },
                            new Label { Text = "y:", VerticalAlign = VerticalAlign.Bottom });
            nLayout4.AddRow(new TextBox {Size = new Size (75, 25)},
                            new TextBox {Size = new Size (75, 25)});

            nLayout4.AddRow(new Label { Text = "x2:", VerticalAlign = VerticalAlign.Bottom },
                            new Label { Text = "y2:", VerticalAlign = VerticalAlign.Bottom });
            nLayout4.AddRow(new TextBox {Size = new Size (75, 25)},
                            new TextBox {Size = new Size (75, 25)});

            nLayout2.Add (nLayout4);
            nLayout2.Add (new Button { Text = "Remove" });

            smGroupBox.Content = nLayout2;

            nLayout1.AddRow (smGroupBox);


            mainLayout.AddRow (null, nLayout1, null);

            mainLayout.Add (null);

            Content = mainLayout;
        }

        protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);            

            // Set Mousetool, when we are shown.
            Shown += delegate {
                uiOptions.MouseTool = MouseTool.Default;
            };
        }
    }
}

