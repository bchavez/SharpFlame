

using System;
using System.Collections.Generic;
using System.Linq;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Controls
{
	public class PlayerSelectorWinforms : PlayerSelector
	{
		readonly List<Button> buttons = new List<Button> ();
		Button selectedButton;

		public override string SelectedPlayer {
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

		void SetSelected (Button button, bool force = false, bool sendEvent = true) 
		{
			var changed = selectedButton != button;
			if (force || changed)
			{
				selectedButton = button;
				foreach (var r in buttons)
				{
					r.Enabled = !ReferenceEquals (r, button);
				}

				button.Focus ();

				if (sendEvent && changed) 
					OnSelectedPlayerChanged (EventArgs.Empty);
			}
		}
	}
}

