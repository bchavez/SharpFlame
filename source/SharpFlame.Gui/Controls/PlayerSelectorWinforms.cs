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
using System.Collections.Generic;
using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Gui.Controls
{
	public class PlayerSelectorWinforms : PlayerSelector
	{
		readonly List<Button> buttons = new List<Button> ();
		Button selectedButton;

		public string SelectedPlayer {
			get { 
				if (selectedButton == null)
				{
					return null;
				}
				return selectedButton.Text;			
			}
		}

		public PlayerSelectorWinforms (int players = 10, bool addScavenger = true)
		{
			for (var i = 0; i < players; i++)
			{
				var button = new Button { Text = i.ToString() };
				button.Click += delegate {
					SetSelected(button);
				};
				buttons.Add(button);
			}
			if (addScavenger)
			{
				var button = new Button { Text = "S" };
				button.Click += delegate {
					SetSelected(button);
				};
				buttons.Add (button);
			}

			var columns = buttons.Count / 2;
			var mod = buttons.Count % 2;

			var layout = new DynamicLayout ();
			layout.Spacing = Size.Empty;
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

		void SetSelected (Button button, bool force = false, bool sendEvent = true) 
		{
			var changed = selectedButton != button;
			if (force || changed)
			{
				selectedButton = button;
				foreach (var r in buttons)
				{
					r.Enabled = !object.ReferenceEquals (r, button);
				}

				if (sendEvent && changed) 
					OnSelectedPlayerChanged (EventArgs.Empty);
			}
		}
	}
}

