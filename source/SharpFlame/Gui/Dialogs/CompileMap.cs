using Eto.Drawing;
using Eto.Forms;

namespace SharpFlame.Gui.Dialogs
{
    public class CompileMap : Dialog
    {
        public class Model
        {
            public string MapName { get; set; }
            public string NumPlayers { get; set; }
            public string Author { get; set; }
            public string License { get; set; }

            public string SetScrollLimits { get; set; }
            public string ScrollMinX { get; set; }
            public string ScrollMaxX { get; set; }
            public string ScrollMinY { get; set; }
            public string ScrollMaxY { get; set; }
        }

        public CompileMap()
        {
            this.Title = "Compile Map";
            this.Resizable = false;

            this.DataContext = new Model();

            BuildTabPages();
            
            var layout = new TableLayout()
                {
                    Padding = new Padding(10),
                    Spacing = new Size(5, 5),
                    Rows =
                        {
                            new TableRow(new Label{Text = "Map Name:"}, null),

                        }
                };
            
        }


        private TextBox MapNameBinding()
        {
            var txt = new TextBox();
            txt.TextBinding.BindDataContext<Model>(m => m.MapName);
            return txt;
        }

        private TextBox ScrollMinXBinding()
        {
            var txt = new TextBox();
            txt.TextBinding.BindDataContext<Model>(m => m.ScrollMinX);
            return txt;
        }
        private TextBox ScrollMinYBinding()
        {
            var txt = new TextBox();
            txt.TextBinding.BindDataContext<Model>(m => m.ScrollMinY);
            return txt;
        }


        private void BuildTabPages()
        {
            new TabControl()
                {
                    Pages =
                        {
                            new TabPage
                                {
                                    Text = "Multiplayer",
                                    Content = new TableLayout()
                                        {
                                            Rows =
                                                {
                                                    new TableRow()
                                                }
                                        }
                                },
                            new TabPage
                                {
                                    Text = "Campaign"
                                }
                        }
                };
        }

    }
}