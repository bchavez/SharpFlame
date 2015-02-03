using System;
using Eto.Forms;
using FluentValidation;
using FluentValidation.Attributes;
using SharpFlame.Core;
using SharpFlame.Gui.Controls;
using SharpFlame.Mapping;
using Z.ExtensionMethods;

namespace SharpFlame.Gui.Dialogs
{
    public class CompileMapDialog : Dialog2<CompileOptions>
    {
        protected TextBox txtMapName;
        protected NumericUpDown numPlayers;
        protected TextBox txtAuthor;
        protected ComboBox cboLicense;
        protected CampaignDropDown ddlCampType;
        protected CheckBox chkAutoScrollLimits;
        protected GroupBox grpLimits;
        protected NumericUpDown numScrollMinX;
        protected NumericUpDown numScrollMinY;
        protected NumericUpDown numScrollMaxX;
        protected NumericUpDown numScrollMaxY;
        protected TabControl tabCompileType;

        public CompileMapDialog()
        {
            Eto.Serialization.Xaml.XamlReader.Load(this);

            this.DataContext = new CompileOptions();

            Init();
        }

        private void Init()
        {
            this.txtMapName.TextBinding.BindDataContext<CompileOptions>(c => c.MapName);
            
            this.numPlayers.ValueBinding.BindDataContext<CompileOptions>(c => c.NumPlayers);
            this.numPlayers.MaxValue = Constants.PlayerCountMax;
            this.numPlayers.MinValue = 2;

            this.txtAuthor.TextBinding.BindDataContext<CompileOptions>(c => c.Author);

            this.cboLicense.SelectedValueBinding.BindDataContext<CompileOptions>(c => c.License);

            this.ddlCampType.SelectedValueBinding.BindDataContext<CompileOptions>(c => c.CampType);

            this.chkAutoScrollLimits.CheckedBinding.BindDataContext<CompileOptions>(c => c.AutoScrollLimits);

            this.grpLimits.Bind(x => x.Enabled, this.chkAutoScrollLimits.CheckedBinding.Convert(i => !i.Value));

            this.numScrollMinX.ValueBinding.BindDataContext<CompileOptions>(c => c.ScrollMinX);
            this.numScrollMinY.ValueBinding.BindDataContext<CompileOptions>(c => c.ScrollMinY);
            this.numScrollMaxX.ValueBinding.BindDataContext<CompileOptions>(c => c.ScrollMaxX);
            this.numScrollMaxY.ValueBinding.BindDataContext<CompileOptions>(c => c.ScrollMaxY);


        }

        void cmdCompile_Click(object sender, System.EventArgs e)
        {
            var options = this.DataContext;

            var validator = new CompileOptionsValidator();
            var results = validator.Validate(options);

            if( !results.IsValid )
            {
                var warn = new Result("Precompile error", false);
                results.Errors.ForEach(err => warn.ProblemAdd(err.ErrorMessage));
                App.ShowWarnings(warn);
                return;
            }

            options.CompileType = this.tabCompileType.SelectedPage.Text.StartsWith("mulit", StringComparison.OrdinalIgnoreCase)
                ? CompileType.Multiplayer : CompileType.Campaign;

            this.Close(options);
        }
    }
}