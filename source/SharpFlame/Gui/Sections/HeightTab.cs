using System;
using Eto.Drawing;
using Eto.Forms;
using Ninject;
using SharpFlame.Core;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class HeightTab : TabPage
	{
        [Inject]
        internal Options UiOptions { get; set; }

        protected NumericUpDown numRadius;

        protected RadioButton rbSet;
        protected RadioButton rbChange;
        protected RadioButton rbSmooth;
		
        protected NumericUpDown nudLmbHeight;
        protected NumericUpDown nudRmbHeight;

        protected NumericUpDown nudChangeRate;
        protected CheckBox cbChangeFade;

		protected Button cmdCircularTool;
		protected Button cmdSquareTool;

        private readonly NumericUpDown nudSmoothRate;

		protected GroupBox grpHeight;
		protected GroupBox grpAmount;
		protected GroupBox grpSmooth;
		protected DropDown ddlMode;
		protected ImageView imgLeftClick;
		protected ImageView imgRightClick;
		protected Button lset1;

		public void ToolSelection(object sender, EventArgs e)
		{
			
		}

        public HeightTab ()
        {
	        XomlReader.Load(this);

	        this.cmdSquareTool.Image = Resources.Square;
	        this.cmdCircularTool.Image = Resources.Circle;
	        this.imgLeftClick.Image = Resources.MouseLeft;
	        this.imgRightClick.Image = Resources.MouseRight;

			this.grpHeight.Bind(x => x.Visible, this.ddlMode.SelectedIndexBinding.Convert(i => i == 0));
			//this.grpAmount.Bind(x => x.Visible, this.ddlMode.SelectedIndexBinding.Convert(i => i == 1));
			//this.grpSmooth.Bind(x => x.Visible, this.ddlMode.SelectedIndexBinding.Convert(i => i == 2));

	        /*rbSet = new RadioButton { Text = "Set", Checked = true };
            rbChange = new RadioButton (rbSet){ Text = "Change" };
            rbSmooth = new RadioButton (rbChange) { Text = "Smooth" };

			var mainLayout = new DynamicLayout ();

			var circularButton = new Button { Text = "Circular", Enabled = false };
			var squareButton = new Button { Text = "Square" };
			circularButton.Click += (sender, e) => { 
				circularButton.Enabled = false;
				squareButton.Enabled = true;
                UiOptions.Height.Brush.Shape = ShapeType.Circle;
			};
			squareButton.Click += (sender, e) => { 
				squareButton.Enabled = false;
				circularButton.Enabled = true;
                UiOptions.Height.Brush.Shape = ShapeType.Square;
			};

			var nLayout1 = new DynamicLayout { Padding = Padding.Empty };
			nLayout1.AddRow (
                new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle },
                nudBrushRadius = new NumericUpDown { 
                    Size = new Size(-1, -1), 
                    Value = UiOptions.Height.Brush.Radius, 
                    MaxValue = Constants.MapMaxSize, 
                    MinValue = 1
                }, 
                circularButton, 
                squareButton
            );
			mainLayout.AddRow (nLayout1);

			var nLayout2 = new DynamicLayout { Padding = Padding.Empty };
            nLayout2.AddRow (rbSet, new Label { Text = "(0 - 255)", VerticalAlign = VerticalAlign.Middle });
			mainLayout.AddRow (nLayout2);

            nudLmbHeight = new NumericUpDown { Size = new Size(-1, -1), Value = 85, MaxValue = 255, MinValue = 0 };
			var lmb0 = new Button { Text = "0", Size = new Size(35, 26) };
			var lmb64 = new Button { Text = "64", Size = new Size(35, 26) };
			var lmb85 = new Button { Text = "85", Size = new Size(35, 26), Enabled = false};
			var lmb128 = new Button { Text = "128", Size = new Size(35, 26) };
			var lmb170 = new Button { Text = "170", Size = new Size(35, 26) };
			var lmb192 = new Button { Text = "192", Size = new Size(35, 26) };
			var lmb255 = new Button { Text = "255", Size = new Size(35, 26) };

			lmb0.Click += delegate {
				nudLmbHeight.Value = 0D;	 
			};

			lmb64.Click += delegate {
				nudLmbHeight.Value = 64D;	 
			};

			lmb85.Click += delegate {
				nudLmbHeight.Value = 85D;	 
			};

			lmb128.Click += delegate {
				nudLmbHeight.Value = 128D;	 
			};

			lmb170.Click += delegate {
				nudLmbHeight.Value = 170D;	 
			};

			lmb192.Click += delegate {
				nudLmbHeight.Value = 192D;	 
			};

			lmb255.Click += delegate {
				nudLmbHeight.Value = 255D;	 
			};



			nudLmbHeight.ValueChanged += delegate {
				lmb0.Enabled = true;
				lmb64.Enabled = true;
				lmb85.Enabled = true;
				lmb128.Enabled = true;
				lmb170.Enabled = true;
				lmb192.Enabled = true;
				lmb255.Enabled = true;

				if (nudLmbHeight.Value == 0) {
					lmb0.Enabled = false;
				} else if (nudLmbHeight.Value == 64) {
					lmb64.Enabled = false;
				} else if (nudLmbHeight.Value == 85) {
					lmb85.Enabled = false;
				} else if (nudLmbHeight.Value == 128) {
					lmb128.Enabled = false;
				} else if (nudLmbHeight.Value == 170) {
					lmb170.Enabled = false;
				} else if (nudLmbHeight.Value == 192) {
					lmb192.Enabled = false;
				} else if (nudLmbHeight.Value == 255) {
					lmb255.Enabled = false;
				}
			};

			var nLayout3 = new DynamicLayout ();
			nLayout3.AddRow (
                null, 
                new Label { Text = "Left mouse button Height", VerticalAlign = VerticalAlign.Middle }, 
                nudLmbHeight, 
                null);
			mainLayout.AddRow (nLayout3);

            var nLayout4 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
			nLayout4.BeginHorizontal ();
			nLayout4.AddAutoSized (lmb0);
			nLayout4.AddAutoSized (lmb64);
			nLayout4.AddAutoSized (lmb85);
			nLayout4.AddAutoSized (lmb128);
			nLayout4.AddAutoSized (lmb170);
			nLayout4.AddAutoSized (lmb192);
			nLayout4.AddAutoSized (lmb255);
			nLayout4.EndHorizontal ();

			mainLayout.AddRow (nLayout4);

            nudRmbHeight = new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = 255, MinValue = 0 };
			var rmb0 = new Button { Text = "0", Size = new Size(35, 26), Enabled = false};
			var rmb64 = new Button { Text = "64", Size = new Size(35, 26) };
			var rmb85 = new Button { Text = "85", Size = new Size(35, 26) };
			var rmb128 = new Button { Text = "128", Size = new Size(35, 26) };
			var rmb170 = new Button { Text = "170", Size = new Size(35, 26) };
			var rmb192 = new Button { Text = "192", Size = new Size(35, 26) };
			var rmb255 = new Button { Text = "255", Size = new Size(35, 26) };

			rmb0.Click += delegate {
				nudRmbHeight.Value = 0D;	 
			};

			rmb64.Click += delegate {
				nudRmbHeight.Value = 64D;	 
			};

			rmb85.Click += delegate {
				nudRmbHeight.Value = 85D;	 
			};

			rmb128.Click += delegate {
				nudRmbHeight.Value = 128D;	 
			};

			rmb170.Click += delegate {
				nudRmbHeight.Value = 170D;	 
			};

			rmb192.Click += delegate {
				nudRmbHeight.Value = 192D;	 
			};

			rmb255.Click += delegate {
				nudRmbHeight.Value = 255D;	 
			};



			nudRmbHeight.ValueChanged += delegate {
				rmb0.Enabled = true;
				rmb64.Enabled = true;
				rmb85.Enabled = true;
				rmb128.Enabled = true;
				rmb170.Enabled = true;
				rmb192.Enabled = true;
				rmb255.Enabled = true;

				if (nudRmbHeight.Value == 0) {
					rmb0.Enabled = false;
				} else if (nudRmbHeight.Value == 64) {
					rmb64.Enabled = false;
				} else if (nudRmbHeight.Value == 85) {
					rmb85.Enabled = false;
				} else if (nudRmbHeight.Value == 128) {
					rmb128.Enabled = false;
				} else if (nudRmbHeight.Value == 170) {
					rmb170.Enabled = false;
				} else if (nudRmbHeight.Value == 192) {
					rmb192.Enabled = false;
				} else if (nudRmbHeight.Value == 255) {
					rmb255.Enabled = false;
				}
			};

			var nLayout5 = new DynamicLayout ();
            nLayout5.AddRow (null, new Label { Text = "Right mouse button Height", VerticalAlign = VerticalAlign.Middle }, nudRmbHeight, null);
			mainLayout.AddRow (nLayout5);

			var nLayout6 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
			nLayout6.BeginHorizontal ();
			nLayout6.AddAutoSized (rmb0);
			nLayout6.AddAutoSized (rmb64);
			nLayout6.AddAutoSized (rmb85);
			nLayout6.AddAutoSized (rmb128);
			nLayout6.AddAutoSized (rmb170);
			nLayout6.AddAutoSized (rmb192);
			nLayout6.AddAutoSized (rmb255);
			nLayout6.EndHorizontal ();

			mainLayout.AddRow (nLayout6);



			var nLayout7 = new DynamicLayout { Padding = new Padding (0, 20) };
            nLayout7.AddRow (rbChange);
			nLayout7.AddRow (
                new Label { Text = "Rate", VerticalAlign = VerticalAlign.Middle }, 
                nudChangeRate = new NumericUpDown { 
                    Size = new Size(-1, -1), 
                    MaxValue = 255D, 
                    MinValue = -255D
                },
                cbChangeFade = new CheckBox { Text = "Fading" }
            );

			mainLayout.AddRow (nLayout7);

            var nLayout8 = new DynamicLayout { Padding = new Padding(0, 20) };
            nLayout8.AddRow (rbSmooth);
			nLayout8.AddRow (
                new Label { Text = "Rate", VerticalAlign = VerticalAlign.Middle }, 
			    TableLayout.AutoSized(
                    nudSmoothRate = new NumericUpDown { 
                        Size = new Size(-1, -1), 
                        MaxValue = 255D, 
                        MinValue = -255D
                    }
                )
            );

			mainLayout.AddRow (nLayout8);

			var newMainyLayout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
		    newMainyLayout.AddRow(null, mainLayout, null);
            newMainyLayout.Add(null);

            Content = newMainyLayout;*/
        }

        protected override void OnLoadComplete(EventArgs lcEventArgs)
        {
            /*base.OnLoadComplete(lcEventArgs);

            rbSet.CheckedChanged += delegate
            {
                setMouseMode();
            };

            rbChange.CheckedChanged += delegate
            {
                setMouseMode();
            };

            rbSmooth.CheckedChanged += delegate
            {
                setMouseMode();
            };

            // Set Mousetool, when we are shown.
            Shown += delegate {
                setMouseMode();
            };

            var heightOptions = UiOptions.Height;
            nudBrushRadius.ValueChanged += delegate
            {
                heightOptions.Brush.Radius = nudBrushRadius.Value;
            };

            nudLmbHeight.Bind(r => r.Value, heightOptions, t => t.LmbHeight);
            nudRmbHeight.Bind(r => r.Value, heightOptions, t => t.RmbHeight);

            nudChangeRate.Bind(r => r.Value, heightOptions, t => t.ChangeRate);
            cbChangeFade.Bind(r => r.Checked, heightOptions, t => t.ChangeFade);

            nudSmoothRate.Bind(r => r.Value, heightOptions, t => t.SmoothRate);*/
        }

        private void setMouseMode() 
        {
            /*if(rbSet.Checked)
            {
                UiOptions.MouseTool = MouseTool.HeightSetBrush;
            } else if(rbChange.Checked)
            {
                UiOptions.MouseTool = MouseTool.HeightChangeBrush;
            } else if(rbSmooth.Checked)
            {
                UiOptions.MouseTool = MouseTool.HeightSmoothBrush;
            }
			 */
        }
	}
}

