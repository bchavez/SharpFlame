#region License
// /*
// The MIT License (MIT)
//
// Copyrigth (c) 2013 Flail13
// Copyright (c) 2014 Brian Chavez and René Jochum
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// */
#endregion

using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Gui.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class TerrainTab : Panel
	{
		RadioButtonList placeFillRadios;
		RadioButtonList roadRadios;
		RadioButtonList cliffRadios;

		public TerrainTab ()
		{
			placeFillRadios = new RadioButtonList { Spacing = Size.Empty, Orientation = RadioButtonListOrientation.Vertical };
			placeFillRadios.Items.Add(new ListItem { Text = "Place" });
			placeFillRadios.Items.Add(new ListItem { Text = "Fill" });

            roadRadios = new RadioButtonList { Spacing = Size.Empty, Orientation = RadioButtonListOrientation.Vertical };
			roadRadios.Items.Add(new ListItem { Text = "Sides" });
			roadRadios.Items.Add(new ListItem { Text = "Lines" });
			roadRadios.Items.Add(new ListItem { Text = "Remove" });

            cliffRadios = new RadioButtonList { Spacing = Size.Empty, Orientation = RadioButtonListOrientation.Vertical };
			cliffRadios.Items.Add(new ListItem { Text = "Cliff Triangle" });
			cliffRadios.Items.Add(new ListItem { Text = "Cliff Brush" });
			cliffRadios.Items.Add(new ListItem { Text = "Cliff Remove" });

			placeFillRadios.SelectedIndexChanged += (sender, e) => {
				var s = (RadioButtonList)sender;
				if (s.SelectedIndex != 0) 
				{
					roadRadios.SelectedIndex = 0;
					cliffRadios.SelectedIndex = 0;
				}
			};

			roadRadios.SelectedIndexChanged += (sender, e) => {
				var s = (RadioButtonList)sender;
				if (s.SelectedIndex != 0)
				{
					placeFillRadios.SelectedIndex = 0;
					cliffRadios.SelectedIndex = 0;
				}
			};

			cliffRadios.SelectedIndexChanged += (sender, e) => {
				var s = (RadioButtonList)sender;
				if (s.SelectedIndex != 0)
				{
					placeFillRadios.SelectedIndex = 0;
					roadRadios.SelectedIndex = 0;
				}
			};


			var layout = new DynamicLayout();
            layout.Add (GroundTypeSection ());
			layout.Add (RoadTypeSection ());
			layout.Add (CliffSection ());
            layout.Add (null);

            setBindings ();

			Content = layout;
		}

        void setBindings() {
            // Set Mousetool, when we are shown.
            Shown += delegate {
                App.UiOptions.MouseTool = MouseTool.Default;
            };
        }

		Control CliffSection () {
			var control = new GroupBox { Text = "Cliff:" };

			var mainLayout = new DynamicLayout ();

			var nLayout1 = new DynamicLayout { Padding = Padding.Empty };
			nLayout1.AddRow(new Label { Text = "Cliff Angle", VerticalAlign = VerticalAlign.Middle },
							new NumericUpDown { Size = new Size(-1, -1), Value = 35, MaxValue = 360, MinValue = 1 },
							null);

            var nLayout2 = new DynamicLayout { Padding = Padding.Empty };
            nLayout2.AddRow(new CheckBox { Text = "Set Tris" }, null);

            var nLayout3 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            nLayout3.Add(nLayout1);
			nLayout3.Add (nLayout2);
			nLayout3.Add (null);

            var nLayout4 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
			nLayout4.AddRow (cliffRadios, null, nLayout3, null);

			mainLayout.AddRow (nLayout4);

			var circularButton = new Button { Text = "Circular", Enabled = false };
			var squareButton = new Button { Text = "Square" };
			circularButton.Click += (sender, e) => { 
				circularButton.Enabled = false;
				squareButton.Enabled = true;
			};
			squareButton.Click += (sender, e) => { 
				squareButton.Enabled = false;
				circularButton.Enabled = true;
			};

			var nLayout5 = new DynamicLayout ();
			nLayout5.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle },
							new NumericUpDown { Size = new Size(-1, -1), Value = 2, MaxValue = 512, MinValue = 1 }, 
						    circularButton, 
							squareButton,
							null);

			mainLayout.AddRow (nLayout5, null);

			control.Content = mainLayout;
			return control;
		}

		Control RoadTypeSection () {
			var control = new GroupBox { Text = "Road Type:" };

			var mainLayout = new DynamicLayout ();
			
			var roadTypeListBox = RoadTypeListBox ();
			var eraseButton = new Button { Text = "Erase", Size = new Size(80, 27) };
			eraseButton.Click += delegate {
				roadTypeListBox.SelectedIndex = -1;
			};

			mainLayout.AddRow (roadTypeListBox,
			                   TableLayout.AutoSized (eraseButton),
			                   null);

			mainLayout.AddRow (roadRadios, null);

			control.Content = mainLayout;
			return control;
		}

        Control GroundTypeSection () {
            var control = new GroupBox { Text = "Ground Type:" };

            var circularButton = new Button { Text = "Circular" };
            circularButton.Enabled = false;
            var squareButton = new Button { Text = "Square" };
            circularButton.Click += (sender, e) => { 
                circularButton.Enabled = false;
                squareButton.Enabled = true;
            };
            squareButton.Click += (sender, e) => { 
                squareButton.Enabled = false;
                circularButton.Enabled = true;
            };

            var mainLayout = new DynamicLayout ();
            mainLayout.BeginVertical();
            mainLayout.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle }, 
            				new NumericUpDown { Size = new Size(-1, -1), Value = 2, MaxValue = 512, MinValue = 1 }, 
            				circularButton, 
            				squareButton,
            				null);
            mainLayout.EndVertical ();

            var gdLayout2 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            var gdLayout3 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            var gdLayout4 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };

			var groundTypeListBox = GroundTypeListBox ();
			var eraseButton = new Button { Text = "Erase", Size = new Size(80, 26) };
			eraseButton.Click += delegate {
				groundTypeListBox.SelectedIndex = -1;
			};

			gdLayout3.AddRow (new CheckBox { Checked = true, Text = "Make Invalid Tiles" },
							  null);
			gdLayout4.AddRow (eraseButton, null);

			gdLayout2.AddRow (gdLayout3);
            gdLayout2.Add (null);
            gdLayout2.AddRow (gdLayout4);

            var groundTypeLayout = new DynamicLayout ();
            groundTypeLayout.BeginHorizontal ();
			groundTypeLayout.Add (groundTypeListBox);
            groundTypeLayout.Add (gdLayout2);
			groundTypeLayout.Add (null);
			groundTypeLayout.EndHorizontal ();
            mainLayout.AddRow (groundTypeLayout);

			var gdCliffRadios = new RadioButtonList { Spacing = Size.Empty, Orientation = RadioButtonListOrientation.Vertical};
			gdCliffRadios.Items.Add(new ListItem { Text = "Ignore Cliff" });
			gdCliffRadios.Items.Add(new ListItem { Text = "Stop Before Cliff" });
			gdCliffRadios.Items.Add(new ListItem { Text = "Stop After Cliff" });
			gdCliffRadios.SelectedIndex = 0;

			var gdLayout6 = new DynamicLayout ();
			gdLayout6.AddRow(new CheckBox { Text = "Stop Before Edge" }, null);
			gdLayout6.AddRow (null);

			var gdLayout5 = new DynamicLayout();
			gdLayout5.BeginHorizontal ();
			gdLayout5.Add (placeFillRadios);
			gdLayout5.Add (gdCliffRadios);
			gdLayout5.Add (gdLayout6);
			gdLayout5.Add (null);
			gdLayout5.EndHorizontal ();

			mainLayout.Add (gdLayout5);

            control.Content = mainLayout;
            return control;
        }

		ListBox GroundTypeListBox() {
            var control = new ListBox { Size = new Size(200, 150)};
			control.Items.Add (new ListItem { Text = "Grass" });
            control.Items.Add (new ListItem { Text = "Gravel" });
            control.Items.Add (new ListItem { Text = "Dirt" });
            control.Items.Add (new ListItem { Text = "Grass Snow" });
            control.Items.Add (new ListItem { Text = "Gravel Snow" });
            control.Items.Add (new ListItem { Text = "Snow" });
            control.Items.Add (new ListItem { Text = "Concrete" });
            control.Items.Add (new ListItem { Text = "Water" });

            return control;
        }

		ListBox RoadTypeListBox()
		{
		    var control = new ListBox { Size = new Size(200, 48) };
			control.Items.Add (new ListItem { Text = "Road" }); 
			control.Items.Add (new ListItem { Text = "Track" });

			return control;
		}
	}
}