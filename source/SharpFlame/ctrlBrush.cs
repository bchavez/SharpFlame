using System;

namespace SharpFlame
{
	public partial class ctrlBrush
	{
		
		private clsBrush Brush;
		
		public ctrlBrush(clsBrush NewBrush)
		{
			InitializeComponent();
			
			Brush = NewBrush;
			
			UpdateControlValues();
			
			nudRadius.ValueChanged += nudRadius_Changed;
			nudRadius.Leave += nudRadius_Changed;
		}
		
		public void UpdateControlValues()
		{
			
			nudRadius.Enabled = false;
			tabShape.Enabled = false;
			
			if (Brush == null)
			{
				return;
			}
			
			nudRadius.Value = (decimal) (modMath.Clamp_dbl(Brush.Radius, (double) nudRadius.Minimum, (double) nudRadius.Maximum));
			switch (Brush.Shape)
			{
				case clsBrush.enumShape.Circle:
					tabShape.SelectedIndex = 0;
					break;
				case clsBrush.enumShape.Square:
					tabShape.SelectedIndex = 1;
					break;
			}
			nudRadius.Enabled = true;
			tabShape.Enabled = true;
		}
		
		private bool nudRadiusIsBusy = false;
		
		private void nudRadius_Changed(object sender, EventArgs e)
		{
			if (nudRadiusIsBusy)
			{
				return;
			}
			
			nudRadiusIsBusy = true;
			
			double NewRadius = 0;
			bool Converted = false;
			try
			{
				NewRadius = (double) nudRadius.Value;
				Converted = true;
			}
			catch
			{
				
			}
			if (Converted)
			{
				Brush.Radius = NewRadius;
			}
			
			nudRadiusIsBusy = false;
		}
		
		public void tabShape_SelectedIndexChanged(object sender, System.EventArgs e)
		{
			if (!tabShape.Enabled)
			{
				return;
			}
			
			switch (tabShape.SelectedIndex)
			{
				case 0:
					Brush.Shape = clsBrush.enumShape.Circle;
					break;
				case 1:
					Brush.Shape = clsBrush.enumShape.Square;
					break;
			}
		}
	}
	
}
