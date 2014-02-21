#region

using System;
using SharpFlame.Maths;

#endregion

namespace SharpFlame.Controls
{
    public partial class BrushControl
    {
        private readonly clsBrush brush;
        private bool nudRadiusIsBusy;

        public BrushControl(clsBrush newBrush)
        {
            InitializeComponent();

            brush = newBrush;

            UpdateControlValues();

            nudRadius.ValueChanged += nudRadius_Changed;
            nudRadius.Leave += nudRadius_Changed;
        }

        public void UpdateControlValues()
        {
            nudRadius.Enabled = false;
            tabShape.Enabled = false;

            if ( brush == null )
            {
                return;
            }

            nudRadius.Value = (decimal)(MathUtil.ClampDbl(brush.Radius, (double)nudRadius.Minimum, (double)nudRadius.Maximum));
            switch ( brush.Shape )
            {
                case ShapeType.Circle:
                    tabShape.SelectedIndex = 0;
                    break;
                case ShapeType.Square:
                    tabShape.SelectedIndex = 1;
                    break;
            }
            nudRadius.Enabled = true;
            tabShape.Enabled = true;
        }

        private void nudRadius_Changed(object sender, EventArgs e)
        {
            if ( nudRadiusIsBusy )
            {
                return;
            }

            nudRadiusIsBusy = true;

            double newRadius = 0;
            var converted = false;
            try
            {
                newRadius = (double)nudRadius.Value;
                converted = true;
            }
            catch
            {
            }
            if ( converted )
            {
                brush.Radius = newRadius;
            }

            nudRadiusIsBusy = false;
        }

        public void tabShape_SelectedIndexChanged(object sender, EventArgs e)
        {
            if ( !tabShape.Enabled )
            {
                return;
            }

            switch ( tabShape.SelectedIndex )
            {
                case 0:
                    brush.Shape = ShapeType.Circle;
                    break;
                case 1:
                    brush.Shape = ShapeType.Square;
                    break;
            }
        }
    }
}