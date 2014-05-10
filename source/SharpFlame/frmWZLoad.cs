

using System;


namespace SharpFlame
{
    public partial class frmWZLoad
    {
        public clsOutput Output;
        public string[] lstMap_MapName;

        public frmWZLoad(string[] MapNames, clsOutput NewOutput, string FormTitle)
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            Output = NewOutput;
            Output.Result = -1;

            var A = 0;

            lstMap.Items.Clear();
            lstMap_MapName = MapNames;
            for ( A = 0; A <= MapNames.GetUpperBound(0); A++ )
            {
                lstMap.Items.Add(MapNames[A]);
            }

            Text = FormTitle;
        }

        public void lstMaps_DoubleClick(object sender, EventArgs e)
        {
            if ( lstMap.SelectedIndex >= 0 )
            {
                Output.Result = lstMap.SelectedIndex;
                Close();
            }
        }

        public class clsOutput
        {
            public int Result;
        }

        public struct sMapNameList
        {
            public string[] Names;
        }
    }
}