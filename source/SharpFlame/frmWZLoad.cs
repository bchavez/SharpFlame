namespace SharpFlame
{
    public partial class frmWZLoad
    {
        public class clsOutput
        {
            public int Result;
        }

        public string[] lstMap_MapName;

        public clsOutput Output;

        public struct sMapNameList
        {
            public string[] Names;
        }

        public frmWZLoad(string[] MapNames, clsOutput NewOutput, string FormTitle)
        {
            InitializeComponent();

            Icon = modProgram.ProgramIcon;

            Output = NewOutput;
            Output.Result = -1;

            int A = 0;

            lstMap.Items.Clear();
            lstMap_MapName = MapNames;
            for ( A = 0; A <= MapNames.GetUpperBound(0); A++ )
            {
                lstMap.Items.Add(MapNames[A]);
            }

            Text = FormTitle;
        }

        public void lstMaps_DoubleClick(object sender, System.EventArgs e)
        {
            if ( lstMap.SelectedIndex >= 0 )
            {
                Output.Result = lstMap.SelectedIndex;
                Close();
            }
        }
    }
}