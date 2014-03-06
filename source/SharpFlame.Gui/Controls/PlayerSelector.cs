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

using System;
using System.Linq;
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Gui.Controls
{
	public class PlayerSelector : Panel
	{
		readonly List<CustomButton> buttons = new List<CustomButton> ();
		CustomButton selectedButton;

		Color backGroundColor = Color.FromArgb(0xFFF3F2EC);
		Color hoverColor = Color.FromArgb(0xFFB6BDD2);
		Color hoverBorderColor = Color.FromArgb(0xFF316AC5);

		public virtual string SelectedPlayer {
			get { 
				if (selectedButton == null)
				{
					return null;
				}
				return selectedButton.Text;			
			}
            set {
                var button = buttons.FirstOrDefault(b => b.Text == value);
                SetSelected (button, true);
            }
		}

		public event EventHandler<EventArgs> SelectedPlayerChanged;

		public virtual void OnSelectedPlayerChanged (EventArgs e)
		{
			if (SelectedPlayerChanged != null)
				SelectedPlayerChanged (this, e);
		}

		public PlayerSelector (int players = 10, bool addScavenger = true)
		{
			for (var i = 0; i < players; i++)
			{
				var button = new CustomButton { 
					Text = i.ToString(), 
					BorderWith = new Padding(1, 1), 
					BorderColor = backGroundColor,
					BackGroundColor = backGroundColor, 
					HoverColor = hoverColor,
					HoverBorderColor = hoverBorderColor
				};

				button.Click += delegate {
					SetSelected(button);
				};
				buttons.Add(button);
			}
			if (addScavenger)
			{
				var button = new CustomButton { 
					Text = "S", 
					BorderWith = new Padding(1, 1), 
					BorderColor = backGroundColor,
					BackGroundColor = backGroundColor, 
					HoverColor = hoverColor,
					HoverBorderColor = hoverBorderColor
				};
				button.Click += delegate {
					SetSelected(button);
				};
				buttons.Add (button);
			}

			var columns = buttons.Count / 2;
			var mod = buttons.Count % 2;

			var layout = new DynamicLayout { Spacing = Size.Empty };
			for (var r = 0; r < 2; r++)
			{
				layout.BeginHorizontal ();
				for (var c = 0; c < columns; c++)
				{
					layout.Add (buttons [c + (r * columns)]);
				}
				if (r == 0 && mod == 1)
				{
					layout.Add (null);
				} else if (r == 1 && mod == 1)
				{
					layout.Add (buttons[buttons.Count - 1]);
				}
				layout.EndBeginHorizontal ();
			}

			Content = layout;
		}

		void SetSelected (CustomButton button, bool force = false, bool sendEvent = true) 
		{
			var changed = selectedButton != button;
			if (force || changed)
			{
				selectedButton = button;
				foreach (var r in buttons)
				{
					r.Enabled = !ReferenceEquals (r, button);
				}

				if (sendEvent && changed) 
					OnSelectedPlayerChanged (EventArgs.Empty);
			}
		}
	}
}

