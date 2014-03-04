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

using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;

namespace SharpFlame.Gui.Dialogs
{
	public class About : Dialog
	{
		public About()
		{
			Title = string.Format ("{0} {1}", Constants.ProgramName, Constants.ProgramVersion());
			Resizable = true;

			var layout = new DynamicLayout(new Padding(20, 5), new Size(10, 10));

			layout.AddCentered(new ImageView
			{
				Image = Resources.SharpFlameIcon()
			}, true, true);
			
			layout.Add(new Label
			{
				Text = Constants.ProgramName,
				Font = new Font(SystemFont.Bold, 20),
				HorizontalAlign = HorizontalAlign.Center
			});

			layout.Add(new Label
			{
				Text = string.Format("Version {0}", Constants.ProgramVersion()),
				Font = new Font(SystemFont.Default, 10),
				HorizontalAlign = HorizontalAlign.Center
			});

			layout.Add(new Label
			           {
				Text = "Copyright 2013 by Flail13",
				Font = new Font(SystemFont.Default, 10),
				HorizontalAlign = HorizontalAlign.Center
			});

			
			layout.Add(new Label
			{
				Text = "Copyright 2014 by Cowboy, pcdummy and jorzi",
				Font = new Font(SystemFont.Default, 10),
				HorizontalAlign = HorizontalAlign.Center
			});

			layout.AddCentered(CloseButton());

			Content = layout;
		}

		Control CloseButton()
		{
			var button = new Button
			{
				Text = "Close"
			};
			DefaultButton = button;
			AbortButton = button;
			button.Click += delegate
			{
				Close();
			};
			return button;
		}
	}
}

