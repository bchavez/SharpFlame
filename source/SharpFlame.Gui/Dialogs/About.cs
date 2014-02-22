using Eto.Drawing;
using Eto.Forms;
using System;
using System.Reflection;
using SharpFlame;

namespace SharpFlame.Gui.Dialogs
{
	public class About : Dialog
	{
		public About()
		{
			this.Title = "About Eto Test";
			this.Resizable = true;

			var layout = new DynamicLayout(new Padding(20, 5), new Size(10, 10));

			layout.AddCentered(new ImageView
			{
				Image = Icon.FromResource ("SharpFlame.Gui.Resources.flaME.ico")
			}, true, true);
			
			layout.Add(new Label
			{
				Text = "About Eto Test",
				Font = new Font(SystemFont.Bold, 20),
				HorizontalAlign = HorizontalAlign.Center
			});

			var version = Assembly.GetEntryAssembly().GetName().Version;
			layout.Add(new Label
			{
				Text = string.Format("Version {0}", version),
				Font = new Font(SystemFont.Default, 10),
				HorizontalAlign = HorizontalAlign.Center
			});
			
			
			layout.Add(new Label
			{
				Text = "Copyright 2014 by Cowboy and pcdummy",
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

