using System;
using System.Collections.Generic;
using System.Linq;
using Eto;
using Eto.Forms;
using Eto.Drawing;

namespace SharpFlame.Gui
{
	public class PlayerSelector : Panel
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

		public event EventHandler<EventArgs> SelectedPlayerChanged;

		public virtual void OnSelectedPlayerChanged (EventArgs e)
		{
			if (SelectedPlayerChanged != null)
				SelectedPlayerChanged (this, e);
		}

		public PlayerSelector (int players = 10, bool addScavenger = true)
		{
			for (var i = 1; i <= players; i++)
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
				} else {
					layout.Add(buttons[buttons.Count -1]);
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

