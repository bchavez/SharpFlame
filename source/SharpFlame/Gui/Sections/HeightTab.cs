#region License
 /*
 The MIT License (MIT)

 Copyright (c) 2013-2014 The SharpFlame Authors.

 Permission is hereby granted, free of charge, to any person obtaining a copy
 of this software and associated documentation files (the "Software"), to deal
 in the Software without restriction, including without limitation the rights
 to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
 copies of the Software, and to permit persons to whom the Software is
 furnished to do so, subject to the following conditions:

 The above copyright notice and this permission notice shall be included in
 all copies or substantial portions of the Software.

 THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
 IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
 FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
 AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
 LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
 OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
 THE SOFTWARE.
 */
#endregion

using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame;
using SharpFlame.UiOptions;

namespace SharpFlame.Gui.Sections
{
    public class HeightTab : Panel
	{
        private readonly Options uiOptions;

        public HeightTab (Options argUiOptions)
		{
            uiOptions = argUiOptions;

			var setRadio = new RadioButton { Text = "Set" };
			var changeRadio = new RadioButton (setRadio){ Text = "Change" };
			var smoothRadio = new RadioButton (changeRadio) { Text = "Smooth" };

			var mainLayout = new DynamicLayout ();

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

			var nLayout1 = new DynamicLayout { Padding = Padding.Empty };
			nLayout1.AddRow (new Label { Text = "Radius:", VerticalAlign = VerticalAlign.Middle },
							 new NumericUpDown { Size = new Size(-1, -1), Value = 2, MaxValue = Constants.MapMaxSize, MinValue = 1 }, 
							 circularButton, 
							 squareButton);
			mainLayout.AddRow (nLayout1);

			var nLayout2 = new DynamicLayout { Padding = Padding.Empty };
			nLayout2.AddRow (setRadio, new Label { Text = "(0 - 255)", VerticalAlign = VerticalAlign.Middle });
			mainLayout.AddRow (nLayout2);

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
			nLayout7.AddRow (changeRadio);
			nLayout7.AddRow (new Label { Text = "Rate", VerticalAlign = VerticalAlign.Middle }, 
							 new NumericUpDown { Size = new Size(-1, -1), Value = 16, MaxValue = 512, MinValue = 0 },
							 new CheckBox { Text = "Fading" });

			mainLayout.AddRow (nLayout7);

            var nLayout8 = new DynamicLayout { Padding = new Padding(0, 20) };
			nLayout8.AddRow (smoothRadio);
			nLayout8.AddRow (new Label { Text = "Rate", VerticalAlign = VerticalAlign.Middle }, 
							 TableLayout.AutoSized(new NumericUpDown { Size = new Size(-1, -1), Value = 3, MaxValue = 512, MinValue = 0 }));

			mainLayout.AddRow (nLayout8);

			var nLayout9 = new DynamicLayout ();
			nLayout9.AddRow (new Label { Text = "Multiply Heights of Selection", VerticalAlign = VerticalAlign.Middle });
			mainLayout.AddRow (nLayout9);

			var nLayout10 = new DynamicLayout { Padding = Padding.Empty };
			nLayout10.AddRow(new NumericUpDown { Size = new Size(-1, -1), Value = 1, MaxValue = 512, MinValue = 0 },
							new Button { Text = "Do" }, null);
			mainLayout.AddRow (nLayout10);

			var nLayout11 = new DynamicLayout ();
			nLayout11.AddRow (new Label { Text = "Offset Heights of Selection", VerticalAlign = VerticalAlign.Middle });
			mainLayout.AddRow (nLayout11);

			var nLayout12 = new DynamicLayout { Padding = Padding.Empty };
			nLayout12.AddRow(new NumericUpDown { Size = new Size(-1, -1), Value = 1, MaxValue = 512, MinValue = 0 },
			new Button { Text = "Do" }, null);
			mainLayout.AddRow (nLayout12);


			var newMainyLayout = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
		    newMainyLayout.AddRow(null, mainLayout, null);
            newMainyLayout.Add(null);

            SetupEventHandlers ();

            Content = newMainyLayout;
		}

        void SetupEventHandlers() {
            // Set Mousetool, when we are shown.
            Shown += delegate {
                uiOptions.MouseTool = MouseTool.ObjectSelect;
            };
        }
	}
}

