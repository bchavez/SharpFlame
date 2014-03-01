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
using SharpFlame.Core;
using SharpFlame.Gui.Sections;

namespace SharpFlame.Gui.Forms
{
	public class MainForm : Form
	{
		public MainForm(SharpFlameApplication app)
		{
			ClientSize = new Size(1024, 768);
			Title = string.Format ("No Map - {0} {1}", Constants.ProgramName, Constants.ProgramVersionNumber);
			Icon = Resources.SharpFlameIcon();

			var tabControl = new TabControl ();
			tabControl.TabPages.Add(new TabPage { Text = "Textures", Content = new TextureTab(app) });
			tabControl.TabPages.Add(new TabPage { Text = "Terrain", Content = new TerrainTab(app) });
			tabControl.TabPages.Add(new TabPage { Text = "Height", Content = new HeightTab(app) });
			tabControl.TabPages.Add(new TabPage { Text = "Resize", Content = new ResizeTab(app) });
			tabControl.TabPages.Add(new TabPage { Text = "Place Objects", Content = new PlaceObjectsTab(app) });
            tabControl.TabPages.Add(new TabPage { Text = "Object", Content = new ObjectTab(app) });
            tabControl.TabPages.Add(new TabPage { Text = "Label", Content = new LabelsTab(app) });

            tabControl.SelectedIndexChanged += delegate
            {
                tabControl.SelectedPage.Content.OnShown(EventArgs.Empty);
            };

			var splitter = new Splitter
			{
				Position = 392,
				FixedPanel = SplitterFixedPanel.Panel1,
				Panel1 = tabControl,
				Panel2 = new MainMapView(app)
			};

			// 7. Set the content of the form to use the layout

			Content = splitter;

			generateMenuToolBar ();
			Maximize ();
		}     

		void generateMenuToolBar()
		{
			var about = new Actions.About();
			var quit = new Actions.Quit();

			var menu = new MenuBar();
			// create standard system menu (e.g. for OS X)
			Application.Instance.CreateStandardMenu(menu.Items);

			// add our own items to the menu

			var file = menu.Items.GetSubmenu("&File", 100);
			file.Items.GetSubmenu ("&New Map", 100);
			file.Items.AddSeparator ();
			file.Items.GetSubmenu ("&Open", 200);
			file.Items.AddSeparator ();

			var saveMenu = file.Items.GetSubmenu ("&Save", 300);
			file.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("&Map fmap");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("&Quick Save fmap");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("Export map &LND");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("Export &Tile Types");
			saveMenu.Items.AddSeparator ();
			saveMenu.Items.GetSubmenu ("Minimap Bitmap");
			saveMenu.Items.GetSubmenu ("Heightmap Bitmap");

			file.Items.GetSubmenu ("&Import", 400);
			file.Items.AddSeparator ();
			file.Items.GetSubmenu ("&Compile Map", 500);
			file.Items.AddSeparator ();
			file.Items.Add(quit);

			var toolsMenu = menu.Items.GetSubmenu("&Tools", 600);
			toolsMenu.Items.GetSubmenu ("Reinterpret Terrain", 100);
			toolsMenu.Items.GetSubmenu ("Water Triangle Correction", 200);
			toolsMenu.Items.AddSeparator ();
			toolsMenu.Items.GetSubmenu ("Flaten Under Oils", 300);
			toolsMenu.Items.GetSubmenu ("Flaten Under Structures", 400);
			toolsMenu.Items.AddSeparator ();
			toolsMenu.Items.GetSubmenu ("Generator", 500);

			menu.Items.GetSubmenu("&Options", 900);

			var help = menu.Items.GetSubmenu("&Help", 1000);
			help.Items.Add(about);

			// optional, removes empty submenus and duplicate separators
			// menu.Items.Trim();

			Menu = menu;
		}
	}
}
