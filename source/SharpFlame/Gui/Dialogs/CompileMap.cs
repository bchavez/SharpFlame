using System.ComponentModel;
using Eto.Drawing;
using Eto.Forms;
using FluentValidation;
using Omu.ValueInjecter;
using SharpFlame.Core;
using SharpFlame.Util;

namespace SharpFlame.Gui.Dialogs
{

    public class ControlToModel : KnownSourceValueInjection<Form>
    {
        protected override void Inject(Form source, object target)
        {
            
        }
    }

    public class CompileMapDialog : Dialog<CompileOptions>
    {
        public CompileMapDialog()
        {
            this.Title = "Compile Map";
            //this.Resizable = false;

            this.DataContext = new CompileOptions();

            CompileTypeTabPages();

            var layout = new TableLayout()
                {
                    Rows =
                        {
                            new TableRow(TableLayout.Horizontal(new Label {Text = "Map Name:"}, TableLayout.AutoSized( MapNameTextBox()))),
                            new TableRow(CompileTypeTabPages()),
                            new TableRow(SetScrollLimitsAutomaticallyCheckBox()),
                            new TableRow(ScrollLimitsGroupBox()),
                            new TableRow(TableLayout.AutoSized( CompileButton()))
                        }
                };
            
            this.Content = layout;
        }

        void cmdCompile_Click(object sender, System.EventArgs e)
        {
            
        }

        private Button CompileButton()
        {
            var cmdCompile = new Button()
                {
                    Text = "Compile"
                };
            cmdCompile.Click += this.cmdCompile_Click;
            return cmdCompile;
        }

        private TextBox MapNameTextBox()
        {
            return new TextBox()
                {
                    ID = this.BindId(x => x.MapName)
                };
        }

        private TextBox ScrollMinXTextBox()
        {
            return new TextBox()
                {
                    ID = this.BindId(x => x.ScrollMinX)
                };
        }
        private TextBox ScrollMinYTextBox()
        {
            return new TextBox()
                {
                    ID = this.BindId(x => x.ScrollMinY)
                };
        }
        private TextBox ScrollMaxYTextBox()
        {
            return new TextBox()
                {
                    ID = this.BindId(x => x.ScrollMaxY)
                };
        }
        private TextBox ScrollMaxXTextBox()
        {
            return new TextBox()
                {
                    ID = this.BindId(x => x.ScrollMaxX)
                };
        }

        private NumericUpDown PlayersNumericBox()
        {
            return new NumericUpDown()
                {
                    ID = this.BindId(x => x.NumPlayers),
                    Width = 50,
                    MaxValue = Constants.PlayerCountMax,
                    MinValue = 2
                };
        }
        private TextBox AuthorTextBox()
        {
            return new TextBox()
                {
                    ID = this.BindId(x => x.Author)
                };
        }
        private ComboBox LicenseComboBox()
        {
            return new ComboBox()
                {
                    ID = this.BindId(x => x.ScrollMaxX),
                    Items =
                        {
                            "GPL 2+",
                            "CC BY 3.0 + GPL v2+",
                            "CC BY-SA 3.0 + GPL v2+",
                            "CC0"
                        }
                };
        }

        
        private CheckBox SetScrollLimitsAutomaticallyCheckBox()
        {
            return new CheckBox()
                {
                    ID = this.BindId(x => x.SetScrollLimits),
                    Text = "Set Scroll Limits Automatically"
                };
        }

        private GroupBox ScrollLimitsGroupBox()
        {
            return new GroupBox()
                {
                    Text = "Scroll Limits",
                    Content = new TableLayout()
                        {
                            Rows =
                                {
                                    new TableRow(null, new Label {Text = "X:"}, new Label {Text = "Y:"}),
                                    new TableRow(new Label {Text = "Minimum:"}, TableLayout.AutoSized(ScrollMinXTextBox()), TableLayout.AutoSized(ScrollMinYTextBox())),
                                    new TableRow(new Label {Text = "Maximum:"}, TableLayout.AutoSized(ScrollMaxXTextBox()), TableLayout.AutoSized(ScrollMaxYTextBox())),
                                }
                        }
                };
        }


        private EnumDropDown<CampaignType> CampaignTypeDropDown()
        {
            return new EnumDropDown<CampaignType>()
                {
                };
        }

        private TabPage MultiplayerTabPage()
        {
            return new TabPage
                {
                    Text = "Multiplayer",
                    Content = new TableLayout()
                        {
                            Rows =
                                {
                                    new TableRow(new Label{Text = "Players:"}, TableLayout.AutoSized( PlayersNumericBox() )),
                                    new TableRow(new Label{Text = "Author:"}, TableLayout.AutoSized(AuthorTextBox())),
                                    new TableRow(new Label{Text = "License:"}, TableLayout.AutoSized( LicenseComboBox())),
                                }
                        }
                };
        }

        private TabPage CampaignTabPage()
        {
            return new TabPage()
                {
                    Text = "Campaign",
                    Content = new TableLayout()
                        {
                            Rows =
                                {
                                    new TableRow(new Label{Text = "Type:"}, CampaignTypeDropDown() ),
                                }
                        }
                };
        }

        private TabControl CompileTypeTabPages()
        {
            return new TabControl()
                {
                    Pages =
                        {
                            MultiplayerTabPage(),
                            CampaignTabPage()
                        }
                };
        }

    }

    public class CompileOptions
    {
        public string MapName { get; set; }
        public int NumPlayers { get; set; }
        public string Author { get; set; }
        public string License { get; set; }

        public bool SetScrollLimits { get; set; }
        public int ScrollMinX { get; set; }
        public int ScrollMaxX { get; set; }
        public int ScrollMinY { get; set; }
        public int ScrollMaxY { get; set; }
    }

    public enum CampaignType
    {
        [Description("Initial scenario state")]
        Initial = 0,
        [Description("Scenario scroll area expansion")]
        Scroll,
        [Description("Stand alone mission")]
        StandAlone
    }

    public class CompileMapConfigValidator : AbstractValidator<CompileOptions>
    {
        public CompileMapConfigValidator()
        {
            RuleFor(x => x.MapName)
                .NotEmpty();
            RuleFor(x => x.NumPlayers)
                .GreaterThanOrEqualTo(2);
            RuleFor(x => x.ScrollMaxX)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.ScrollMaxY)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.ScrollMinX)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.ScrollMinY)
                .GreaterThanOrEqualTo(0);
            RuleFor(x => x.License)
                .NotEmpty();
        }
    }
}