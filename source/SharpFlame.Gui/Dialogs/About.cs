using System;
using System.Reflection;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame;
using SharpFlame.Core;

namespace SharpFlame.Gui.Dialogs
{
	public class About : Dialog
	{
		public About()
		{
			this.Title = string.Format ("{0} {1}", Constants.ProgramName, Constants.ProgramVersionNumber);
			this.Resizable = true;

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
				Text = string.Format("Version {0}", Constants.ProgramVersionNumber),
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

