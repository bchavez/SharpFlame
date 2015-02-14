using System;
using System.Net.Mime;
using Eto.Forms;
using FluentValidation;
using FluentValidation.Attributes;
using SharpFlame.Core;
using SharpFlame.Domain;
using SharpFlame.Gui.Controls;
using SharpFlame.Mapping;
using SharpFlame.Util;
using Z.ExtensionMethods;

namespace SharpFlame.Gui.Dialogs
{
    public class CompileMapDialog : Dialog2<InterfaceOptions>
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
        protected TabPage tabMulti;

        public CompileMapDialog()
        {
            //Eto.Serialization.Xaml.XamlReader.Load(this);
            XomlReader.Load(this);

            this.DataContext = this.DataContext ?? new InterfaceOptions();

            BindSetup();
        }

        private void BindSetup()
        {
            this.txtMapName.TextBinding.BindDataContext<InterfaceOptions>(c => c.CompileName);

            this.numPlayers.ValueBinding.BindDataContext<InterfaceOptions>(c => c.CompileMultiPlayers, (c, v) =>c.CompileMultiPlayers = Convert.ToInt32(v),
                defaultGetValue: this.DataContext.CompileMultiPlayers);

            this.numPlayers.MaxValue = Constants.PlayerCountMax;

            this.txtAuthor.TextBinding.BindDataContext<InterfaceOptions>(c => c.CompileMultiAuthor);

            this.cboLicense.BindDataContext(x => x.Text, (InterfaceOptions o) => o.CompileMultiLicense);

            this.ddlCampType.SelectedValueBinding.BindDataContext<InterfaceOptions>(c => c.CampaignGameType);

            this.chkAutoScrollLimits.CheckedBinding.BindDataContext<InterfaceOptions>(c => c.AutoScrollLimits);

            this.grpLimits.Bind(x => x.Enabled, this.chkAutoScrollLimits.CheckedBinding.Convert(i => !i.Value));

            this.numScrollMinX.ValueBinding.BindDataContext<InterfaceOptions>(c => c.ScrollMin.X, (c, v) => c.ScrollMin.X = Convert.ToInt32(v));
            this.numScrollMinY.ValueBinding.BindDataContext<InterfaceOptions>(c => c.ScrollMin.Y, (c, v) => c.ScrollMin.Y = Convert.ToInt32(v));
            this.numScrollMaxX.ValueBinding.BindDataContext<InterfaceOptions>(c => c.ScrollMax.X, (c, v) => c.ScrollMax.X = Convert.ToUInt32(v));
            this.numScrollMaxY.ValueBinding.BindDataContext<InterfaceOptions>(c => c.ScrollMax.Y, (c, v) => c.ScrollMax.Y = Convert.ToUInt32(v));

            //not working, Visible property doesn't have a changed event

            //this.tabMulti.BindDataContext(c => c.Visible,
            //    Binding.Property((InterfaceOptions o) => o.CompileType).ToBool(CompileType.Multiplayer, CompileType.Campaign));

            //this.tabMulti.BindDataContext(x => x.Visible,
            //    Binding.Delegate<InterfaceOptions, bool>(
            //        m => m.CompileType == CompileType.Multiplayer,
            //        (m, v) =>
            //            {
            //                m.CompileType = v ? CompileType.Multiplayer : CompileType.Campaign;
            //            }));
        }

        void cmdCompile_Click(object sender, System.EventArgs e)
        {
            var options = this.DataContext;

            options.CompileType = tabMulti.Visible ? CompileType.Multiplayer : CompileType.Campaign;

            var validator = new InterfaceOptionsValidator();
            var results = validator.Validate(options);

            if( !results.IsValid )
            {
                var warn = new Result("Precompile error", false);
                results.Errors.ForEach(err => warn.ProblemAdd(err.ErrorMessage));
                App.ShowWarnings(warn);
                return;
            }

            this.Close(options);
        }
    }
}