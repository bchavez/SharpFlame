using System;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;

namespace SharpFlame.Gui
{
	public class PlaceObjectsTab : Panel
	{
		public PlaceObjectsTab ()
		{
			var mainLayout = new DynamicLayout ();

			mainLayout.AddRow (new PlayerSelector (Constants.PlayerCountMax), null);

			mainLayout.Add (null);

			Content = mainLayout;
		}
	}
}

