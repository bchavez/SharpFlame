

using System;
using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Controls
{
	public class CustomButton : Panel
	{
		readonly TableLayout borderLayout;
		readonly DynamicLayout layout;
		readonly Label label;

		public Color BackGroundColor 
		{
			get { return BackgroundColor; }
			set { 
				layout.BackgroundColor = value;
				BackgroundColor = value;
			}
		}

		public string Text
		{
			get { return label.Text; }
			set { label.Text = value; }
		}

		public Padding BorderWith
		{
			get { return borderLayout.Padding; }
			set { borderLayout.Padding = value; }
		}

        Color borderColor;
		public Color BorderColor
		{
			get { return borderColor; }
			set { 
                borderColor = value;
                borderLayout.BackgroundColor = value; 
            }
		}

		public Color HoverColor
		{
			get;
			set;
		}

		public Color HoverBorderColor
		{
			get;
			set;
		}

		public Padding Spacing
		{
			get { return (Padding)layout.Padding; }
			set { layout.Padding = value; }
		}

		public CustomButton ()
		{
			borderLayout = new TableLayout (1, 1) { Spacing = Size.Empty };
			layout = new DynamicLayout ();
			label = new Label ();

			Text = "_NOT_SET_";
			BorderWith = new Padding (1, 1);
            BorderColor = Eto.Drawing.Colors.Black;
            HoverColor = Eto.Drawing.Colors.Gray;

            layout.AddCentered (label);

			borderLayout.Add(layout, 0, 0, true, true);

			// Change background color on enter/leave.
			layout.MouseEnter += delegate {
				layout.BackgroundColor = HoverColor;
				if (HoverBorderColor != new Color()) {
					borderLayout.BackgroundColor = HoverBorderColor;
				}
			};

			layout.MouseLeave += delegate {
				layout.BackgroundColor = BackgroundColor;
                borderLayout.BackgroundColor = BorderColor;
			};


			// Connect the click events.
			borderLayout.MouseDown += delegate 
			{
				OnClick (EventArgs.Empty);
			};

            if(Generator.IsWpf)
            {
                layout.MouseDown += delegate
                {
                    OnClick(EventArgs.Empty);   
                };

                label.MouseDown += delegate
                {
                    OnClick(EventArgs.Empty);   
                };
            }

			Content = borderLayout;
		}

		EventHandler<EventArgs> click;

		/// <summary>
		/// Event to handle when the user clicks the button
		/// </summary>
		public event EventHandler<EventArgs> Click
		{
			add { click += value; }
			remove { click -= value; }
		}

		/// <summary>
		/// Raises the <see cref="Click"/> event
		/// </summary>
		/// <param name="e">Event arguments</param>
		public virtual void OnClick (EventArgs e)
		{
			if (click != null)
				click (this, e);
		}
	}
}

