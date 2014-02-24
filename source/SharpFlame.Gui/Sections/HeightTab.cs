using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Sections
{
	public class HeightTab : Panel
	{
		public HeightTab ()
		{
			var setRadio = new RadioButton { Text = "Set" };
			var changeRadio = new RadioButton (setRadio){ Text = "Change" };
			var smoothRadio = new RadioButton (changeRadio) { Text = "Smooth" };

			var mainLayout = new DynamicLayout ();

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

			var nLayout1 = new DynamicLayout ();
			nLayout1.Padding = Padding.Empty;
			nLayout1.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle },
							 new NumericUpDown { Size = new Size(-1, -1), Value = 2, MaxValue = 512, MinValue = 1 }, 
							 circularButton, 
							 squareButton);
			mainLayout.AddRow (null, nLayout1, null);

			var nLayout2 = new DynamicLayout ();
			nLayout2.Padding = Padding.Empty;	
			nLayout2.AddRow (setRadio, new Label { Text = "(0 - 255)", VerticalAlign = VerticalAlign.Middle });
			mainLayout.AddRow (null, nLayout2, null);

			var lmbHeight = new NumericUpDown { Size = new Size(-1, -1), Value = 85, MaxValue = 255, MinValue = 0 };
			var lmb0 = new Button { Text = "0", Size = new Size(35, 26) };
			var lmb64 = new Button { Text = "64", Size = new Size(35, 26) };
			var lmb85 = new Button { Text = "85", Size = new Size(35, 26), Enabled = false};
			var lmb128 = new Button { Text = "128", Size = new Size(35, 26) };
			var lmb170 = new Button { Text = "170", Size = new Size(35, 26) };
			var lmb192 = new Button { Text = "192", Size = new Size(35, 26) };
			var lmb255 = new Button { Text = "255", Size = new Size(35, 26) };

			lmb0.Click += delegate {
				lmbHeight.Value = 0D;	 
			};

			lmb64.Click += delegate {
				lmbHeight.Value = 64D;	 
			};

			lmb85.Click += delegate {
				lmbHeight.Value = 85D;	 
			};

			lmb128.Click += delegate {
				lmbHeight.Value = 128D;	 
			};

			lmb170.Click += delegate {
				lmbHeight.Value = 170D;	 
			};

			lmb192.Click += delegate {
				lmbHeight.Value = 192D;	 
			};

			lmb255.Click += delegate {
				lmbHeight.Value = 255D;	 
			};



			lmbHeight.ValueChanged += delegate {
				lmb0.Enabled = true;
				lmb64.Enabled = true;
				lmb85.Enabled = true;
				lmb128.Enabled = true;
				lmb170.Enabled = true;
				lmb192.Enabled = true;
				lmb255.Enabled = true;

				if (lmbHeight.Value == 0) {
					lmb0.Enabled = false;
				} else if (lmbHeight.Value == 64) {
					lmb64.Enabled = false;
				} else if (lmbHeight.Value == 85) {
					lmb85.Enabled = false;
				} else if (lmbHeight.Value == 128) {
					lmb128.Enabled = false;
				} else if (lmbHeight.Value == 170) {
					lmb170.Enabled = false;
				} else if (lmbHeight.Value == 192) {
					lmb192.Enabled = false;
				} else if (lmbHeight.Value == 255) {
					lmb255.Enabled = false;
				}
			};

			var nLayout3 = new DynamicLayout ();
			nLayout3.AddRow (null, new Label { Text = "LMB Height", VerticalAlign = VerticalAlign.Middle }, lmbHeight, null);
			mainLayout.AddRow (null, nLayout3, null);

			var nLayout4 = new DynamicLayout ();
			nLayout4.Padding = Padding.Empty;
			nLayout4.Spacing = Size.Empty;

			nLayout4.BeginHorizontal ();
			nLayout4.AddAutoSized (lmb0);
			nLayout4.AddAutoSized (lmb64);
			nLayout4.AddAutoSized (lmb85);
			nLayout4.AddAutoSized (lmb128);
			nLayout4.AddAutoSized (lmb170);
			nLayout4.AddAutoSized (lmb192);
			nLayout4.AddAutoSized (lmb255);
			nLayout4.EndHorizontal ();

			mainLayout.AddRow (null, nLayout4, null);

			var rmbHeight = new NumericUpDown { Size = new Size(-1, -1), Value = 0, MaxValue = 255, MinValue = 0 };
			var rmb0 = new Button { Text = "0", Size = new Size(35, 26), Enabled = false};
			var rmb64 = new Button { Text = "64", Size = new Size(35, 26) };
			var rmb85 = new Button { Text = "85", Size = new Size(35, 26) };
			var rmb128 = new Button { Text = "128", Size = new Size(35, 26) };
			var rmb170 = new Button { Text = "170", Size = new Size(35, 26) };
			var rmb192 = new Button { Text = "192", Size = new Size(35, 26) };
			var rmb255 = new Button { Text = "255", Size = new Size(35, 26) };

			rmb0.Click += delegate {
				rmbHeight.Value = 0D;	 
			};

			rmb64.Click += delegate {
				rmbHeight.Value = 64D;	 
			};

			rmb85.Click += delegate {
				rmbHeight.Value = 85D;	 
			};

			rmb128.Click += delegate {
				rmbHeight.Value = 128D;	 
			};

			rmb170.Click += delegate {
				rmbHeight.Value = 170D;	 
			};

			rmb192.Click += delegate {
				rmbHeight.Value = 192D;	 
			};

			rmb255.Click += delegate {
				rmbHeight.Value = 255D;	 
			};



			rmbHeight.ValueChanged += delegate {
				rmb0.Enabled = true;
				rmb64.Enabled = true;
				rmb85.Enabled = true;
				rmb128.Enabled = true;
				rmb170.Enabled = true;
				rmb192.Enabled = true;
				rmb255.Enabled = true;

				if (rmbHeight.Value == 0) {
					rmb0.Enabled = false;
				} else if (rmbHeight.Value == 64) {
					rmb64.Enabled = false;
				} else if (rmbHeight.Value == 85) {
					rmb85.Enabled = false;
				} else if (rmbHeight.Value == 128) {
					rmb128.Enabled = false;
				} else if (rmbHeight.Value == 170) {
					rmb170.Enabled = false;
				} else if (rmbHeight.Value == 192) {
					rmb192.Enabled = false;
				} else if (rmbHeight.Value == 255) {
					rmb255.Enabled = false;
				}
			};

			var nLayout5 = new DynamicLayout ();
			nLayout5.AddRow (null, new Label { Text = "RMB Height", VerticalAlign = VerticalAlign.Middle }, rmbHeight, null);
			mainLayout.AddRow (null, nLayout5, null);

			var nLayout6 = new DynamicLayout ();
			nLayout6.Padding = Padding.Empty;
			nLayout6.Spacing = Size.Empty;

			nLayout6.BeginHorizontal ();
			nLayout6.AddAutoSized (rmb0);
			nLayout6.AddAutoSized (rmb64);
			nLayout6.AddAutoSized (rmb85);
			nLayout6.AddAutoSized (rmb128);
			nLayout6.AddAutoSized (rmb170);
			nLayout6.AddAutoSized (rmb192);
			nLayout6.AddAutoSized (rmb255);
			nLayout6.EndHorizontal ();

			mainLayout.AddRow (null, nLayout6, null);
		
			mainLayout.BeginHorizontal ();
			mainLayout.Add (null);
			mainLayout.Add (null);
			mainLayout.Add (null);
			mainLayout.EndHorizontal ();

			mainLayout.BeginHorizontal ();
			mainLayout.Add (null);
			mainLayout.Add (changeRadio);
			mainLayout.Add (null);
			mainLayout.EndHorizontal ();

			var nLayout7 = new DynamicLayout ();
			nLayout7.Padding = Padding.Empty;
			nLayout7.AddRow (new Label { Text = "Rate", VerticalAlign = VerticalAlign.Middle }, 
							 new NumericUpDown { Size = new Size(-1, -1), Value = 16, MaxValue = 512, MinValue = 0 },
							 new CheckBox { Text = "Fading" });

			mainLayout.AddRow (null, nLayout7, null);

			mainLayout.AddRow (null, smoothRadio, null);
			var nLayout8 = new DynamicLayout ();
			nLayout8.Padding = Padding.Empty;
			nLayout8.AddRow (new Label { Text = "Rate", VerticalAlign = VerticalAlign.Middle }, 
							 TableLayout.AutoSized(new NumericUpDown { Size = new Size(-1, -1), Value = 3, MaxValue = 512, MinValue = 0 }));

			mainLayout.AddRow (null, nLayout8, null);

			mainLayout.Add (null);

			Content = mainLayout;
		}
	}
}
