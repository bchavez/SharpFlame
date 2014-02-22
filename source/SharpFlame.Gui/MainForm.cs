using System;
using Eto.Forms;
using Eto.Drawing;
using System.Collections.Generic;

namespace SharpFlame.Gui.Forms
{
	public class MainForm : Form
	{
		public MainForm()
		{
			this.ClientSize = new Size(1024, 768);
			this.Title = "";

			// Using a DynamicLayout for a simple table is actually a lot easier to maintain than using a TableLayout 
			// and having to specify the x/y co-ordinates for each control added.

			// 1. Create a new DynamicLayout object

			var layout = new DynamicLayout();

			// 2. Begin a horizontal row of controls

			layout.BeginHorizontal();

			// 3. Add controls for each column.  We are setting xscale to true to make each column use an equal portion
			// of the available space.

			layout.Add(new Label { Text = "First Column" }, xscale: true);
			layout.Add(new Label { Text = "Second Column" }, xscale: true);
			layout.Add(new Label { Text = "Third Column" }, xscale: true);

			// 4. End the horizontal section

			layout.EndHorizontal();

			// 5. To add a new row, begin another horizontal section and add more controls:

			layout.BeginHorizontal();
			layout.Add(new TextBox { Text = "Second Row, First Column" });
			layout.Add(new ComboBox { DataStore = new ListItemCollection { new ListItem { Text = "Second Row, Second Column" } } });
			layout.Add(new CheckBox { Text = "Second Row, Third Column" });
			layout.EndHorizontal();

			// 6. By default, the last row & column of a table expands to fill the rest of the space.  We can add one 
			// last row with nothing in it to make the space empty.  Since we are not in a horizontal group, calling 
			// Add() adds a new row.

			layout.Add(null);

			// 7. Set the content of the form to use the layout

			Content = layout;

			GenerateMenuToolBar();
		}

		void GenerateMenuToolBar()
		{
			var about = new Actions.About();
			var quit = new Actions.Quit();

			var menu = new MenuBar();
			// create standard system menu (e.g. for OS X)
			Application.Instance.CreateStandardMenu(menu.Items);

			// add our own items to the menu

			var file = menu.Items.GetSubmenu("&File", 100);
			menu.Items.GetSubmenu("&Edit", 200);
			menu.Items.GetSubmenu("&Window", 900);
			var help = menu.Items.GetSubmenu("&Help", 1000);

			if (Generator.IsMac)
			{
				// have a nice OS X style menu
				var main = menu.Items.GetSubmenu(Application.Instance.Name, 0);
				main.Items.Add(about, 0);
				main.Items.Add(quit, 1000);
			}
			else
			{
				// windows/gtk style window
				file.Items.Add(quit);
				help.Items.Add(about);
			}

			// optional, removes empty submenus and duplicate separators
			menu.Items.Trim();

			Menu = menu;

			// generate and set the toolbar
			var toolBar = new ToolBar();
			toolBar.Items.Add(quit);
			toolBar.Items.Add(new ButtonToolItem(about));

			ToolBar = toolBar;
		}
	}
}

