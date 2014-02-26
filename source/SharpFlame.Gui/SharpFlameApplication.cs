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
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Gui.Forms;

namespace SharpFlame.Gui
{
	public class SharpFlameApplication : Application
	{
		public SharpFlameApplication(Generator generator)
			: base(generator)
		{
			// Allows manual Button size on GTK2.
			Button.DefaultSize = new Size (1, 1);

			Name = string.Format ("No Map - {0} {1}", Constants.ProgramName, Constants.ProgramVersionNumber);
			Style = "application";

            App.Tileset = new List<Tileset> {
                new Tileset { Name = "Arizona" },
                new Tileset { Name = "Urban" },
                new Tileset { Name = "Rocky Mountains" }
            };
		}

		public override void OnInitialized(EventArgs e)
		{
			MainForm = new MainForm();

			base.OnInitialized(e);          

			// show the main form
			MainForm.Show();
		}

		public override void OnTerminating(System.ComponentModel.CancelEventArgs e)
		{
			base.OnTerminating(e);

			var result = MessageBox.Show(MainForm, "Are you sure you want to quit?", MessageBoxButtons.YesNo, MessageBoxType.Question);
			if (result == DialogResult.No)
				e.Cancel = true;
		}
	}
}

