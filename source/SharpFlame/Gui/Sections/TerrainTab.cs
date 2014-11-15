using System;
using System.Diagnostics;
using Eto.Drawing;
using Eto.Forms;
using Ninject.Extensions.Logging;
using SharpFlame;
using SharpFlame.Core;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
    public class TerrainTab : Panel
	{
        private readonly ILogger logger;
        private readonly Options uiOptions;

        private readonly RadioButton rbGroundPlace;
        private readonly RadioButton rbGroundFill;
        private readonly RadioButton rbRoadSides;
        private readonly RadioButton rbRoadLines;
        private readonly RadioButton rbRoadRemove;
        private readonly RadioButton rbCliffTriangle;
        private readonly RadioButton rbCliffBrush;
        private readonly RadioButton rbCliffRemove;

        private NumericUpDown nudTerrainBrushRadius;
        private NumericUpDown nudCliffBrushRadius;

        public TerrainTab (ILoggerFactory logFactory, Options argUiOptions)
		{
            logger = logFactory.GetCurrentClassLogger();
            uiOptions = argUiOptions;

            rbGroundPlace = new RadioButton { Text = "Place", Checked = true };
            rbGroundFill = new RadioButton (rbGroundPlace) { Text = "Fill" };
            rbRoadSides = new RadioButton (rbGroundFill) { Text = "Sides" };
            rbRoadLines = new RadioButton (rbRoadSides) { Text = "Lines" };
            rbRoadRemove = new RadioButton (rbRoadLines) { Text = "Remove" };
            rbCliffTriangle = new RadioButton (rbRoadRemove) { Text = "Triangle" };
            rbCliffBrush = new RadioButton (rbCliffTriangle) { Text = "Brush" };
            rbCliffRemove = new RadioButton (rbCliffBrush) { Text = "Remove" };

			var layout = new DynamicLayout();
            layout.Add (GroundTypeSection ());
			layout.Add (RoadTypeSection ());
			layout.Add (CliffSection ());
            layout.Add (null);

			Content = layout;
		}

        protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            base.OnLoadComplete(lcEventArgs);

            rbGroundPlace.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbGroundFill.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbRoadSides.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbRoadLines.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbRoadRemove.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbCliffTriangle.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbCliffBrush.CheckedChanged += delegate
            {
                setMouseMode ();
            };
            rbCliffRemove.CheckedChanged += delegate
            {
                setMouseMode ();
            };

            // Set Mousetool, when we are shown.
            Shown += delegate {
                setMouseMode();
            };

            var terrainOptions = uiOptions.Terrain;
            nudTerrainBrushRadius.ValueChanged += delegate
            {
                terrainOptions.Brush.Radius = nudTerrainBrushRadius.Value;
            };

            nudCliffBrushRadius.ValueChanged += delegate
            {
                terrainOptions.CliffBrush.Radius = nudCliffBrushRadius.Value;
            };
        }

        private void setMouseMode() {
            if (rbGroundPlace.Checked)
            {
                uiOptions.MouseTool = MouseTool.TerrainBrush;
            } else if (rbGroundFill.Checked)
            {
                uiOptions.MouseTool = MouseTool.TerrainFill;
            } else if (rbRoadSides.Checked)
            {
                uiOptions.MouseTool = MouseTool.RoadPlace;
            } else if (rbRoadLines.Checked)
            {
                uiOptions.MouseTool = MouseTool.RoadLines;
            } else if (rbRoadRemove.Checked)
            {
                uiOptions.MouseTool = MouseTool.RoadRemove;
            } else if (rbCliffTriangle.Checked)
            {
                uiOptions.MouseTool = MouseTool.CliffTriangle;
            } else if (rbCliffBrush.Checked)
            {
                uiOptions.MouseTool = MouseTool.CliffBrush;
            } else if (rbCliffRemove.Checked)
            {
                uiOptions.MouseTool = MouseTool.CliffRemove;
            } else
            {
                Debugger.Break ();
                logger.Error ("No Radiobutton in the TerrainTab selected, this should never happen!");
                uiOptions.MouseTool = MouseTool.Default;
            }
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
            var cliffRadios = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            cliffRadios.Add (rbCliffTriangle);
            cliffRadios.Add (rbCliffBrush);
            cliffRadios.Add (rbCliffRemove);

			nLayout4.AddRow (cliffRadios, null, nLayout3, null);

			mainLayout.AddRow (nLayout4);

			var circularButton = new Button { Text = "Circular", Enabled = false };
			var squareButton = new Button { Text = "Square" };
			circularButton.Click += (sender, e) => { 
				circularButton.Enabled = false;
				squareButton.Enabled = true;
                uiOptions.Terrain.CliffBrush.Shape = ShapeType.Circle;
			};
			squareButton.Click += (sender, e) => { 
				squareButton.Enabled = false;
				circularButton.Enabled = true;
                uiOptions.Terrain.CliffBrush.Shape = ShapeType.Square;
			};

			var nLayout5 = new DynamicLayout ();
			nLayout5.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle },
                nudCliffBrushRadius = new NumericUpDown { 
                    Size = new Size(-1, -1), 
                    Value = uiOptions.Terrain.CliffBrush.Radius, 
                    MaxValue = Constants.MapMaxSize, 
                    MinValue = 1 
                }, 
			    circularButton, 
				squareButton,
				null
            );

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

            var roadRadios = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            roadRadios.Add (rbRoadSides);
            roadRadios.Add (rbRoadLines);
            roadRadios.Add (rbRoadRemove);

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
                uiOptions.Terrain.Brush.Shape = ShapeType.Circle;
            };
            squareButton.Click += (sender, e) => { 
                squareButton.Enabled = false;
                circularButton.Enabled = true;
                uiOptions.Terrain.Brush.Shape = ShapeType.Square;
            };

            var mainLayout = new DynamicLayout ();
            mainLayout.BeginVertical();
            mainLayout.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle }, 
                nudTerrainBrushRadius = new NumericUpDown { 
                    Size = new Size(-1, -1), 
                    Value = uiOptions.Terrain.Brush.Radius,
                    MaxValue = Constants.MapMaxSize, 
                    MinValue = 1 
                }, 
				circularButton, 
				squareButton,
				null
            );
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

            var placeFillRadios = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            placeFillRadios.Add (rbGroundPlace);
            placeFillRadios.Add (rbGroundFill);

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