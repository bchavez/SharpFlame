

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

