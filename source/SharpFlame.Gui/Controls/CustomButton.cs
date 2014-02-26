#region License
// /*
// The MIT License (MIT)
//
// Copyright (c) 2013-2014 The SharpFlame Authors.
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

using System;
using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Gui.Controls
{
	public class CustomButton : Panel
	{
		TableLayout borderLayout;
		DynamicLayout layout;
		Label label;

		public Color BackGroundColor 
		{
			get { return base.BackgroundColor; }
			set { 
				layout.BackgroundColor = value;
				base.BackgroundColor = value;
			}
		}

		public string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		public Padding BorderWith
		{
			get { return borderLayout.Padding; }
			set { borderLayout.Padding = value; }
		}

        Color borderColor;
		public Color BorderColor
		{
			get { return borderColor; }
			set { 
                borderColor = value;
                borderLayout.BackgroundColor = value; 
            }
		}

		public Color HoverColor
		{
			get;
			set;
		}

		public Color HoverBorderColor
		{
			get;
			set;
		}

		public Padding Spacing
		{
			get { return (Padding)layout.Padding; }
			set { layout.Padding = value; }
		}

		public CustomButton ()
		{
			borderLayout = new TableLayout (1, 1) { Spacing = Size.Empty };
			layout = new DynamicLayout ();
			label = new Label ();

			Text = "_NOT_SET_";
			BorderWith = new Padding (1, 1);
			BorderColor = Eto.Drawing.Colors.Black;
            HoverColor = Eto.Drawing.Colors.Gray;

			layout.Add (label);

			borderLayout.Add(layout, 0, 0, true, true);

			// Change background color on enter/leave.
			layout.MouseEnter += delegate {
				layout.BackgroundColor = HoverColor;
				if (HoverBorderColor != new Color{}) {
					borderLayout.BackgroundColor = HoverBorderColor;
				}
			};

			layout.MouseLeave += delegate {
				layout.BackgroundColor = BackgroundColor;
                borderLayout.BackgroundColor = BorderColor;
			};


			// Connect the click events.
			borderLayout.MouseDown += delegate 
			{
				OnClick (EventArgs.Empty);
			};

			layout.MouseDown += delegate
			{
				OnClick (EventArgs.Empty);   
			};

			label.MouseDown += delegate
			{
				OnClick (EventArgs.Empty);   
			};

			Content = borderLayout;
		}

		EventHandler<EventArgs> click;

		/// <summary>
		/// Event to handle when the user clicks the button
		/// </summary>
		public event EventHandler<EventArgs> Click
		{
			add { click += value; }
			remove { click -= value; }
		}

		/// <summary>
		/// Raises the <see cref="Click"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		public virtual void OnClick (EventArgs e)
		{
			if (click != null)
				click (this, e);
		}
	}
}

