    #region

using System;
using System.Collections.Generic;
using System.Drawing;
using System.Windows.Forms;
using SharpFlame.Old.Collections;
using SharpFlame.Old.Colors;
using SharpFlame.Old.AppSettings;
using SharpFlame.Old.Controls;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Old.FileIO;
using SharpFlame.Old.Maths;

#endregion

namespace SharpFlame.Old
{
    public partial class frmOptions
    {
        private readonly KeyboardProfile ChangedKeyControls;
        private readonly Rgba MinimapCliffColour;
        private readonly Rgba MinimapSelectedObjectColour;
        private readonly PathSetControl objectDataPathSetControl = new PathSetControl("Object Data Directories");
        private readonly PathSetControl tilesetsPathSetControl = new PathSetControl("Tilesets Directories");
        private bool AllowClose;
        private Font DisplayFont;
        private ColourControl clrMinimapCliffs;
        private ColourControl clrMinimapSelectedObjects;

        private Option<KeyboardControl>[] lstKeyboardControls_Items = {};

        public frmOptions()
        {
            InitializeComponent();

            Icon = App.ProgramIcon;

            tilesetsPathSetControl.Dock = DockStyle.Fill;
            objectDataPathSetControl.Dock = DockStyle.Fill;
            TableLayoutPanel1.Controls.Add(tilesetsPathSetControl, 0, 0);
            TableLayoutPanel1.Controls.Add(objectDataPathSetControl, 0, 1);

            ChangedKeyControls = (KeyboardProfile)(KeyboardManager.KeyboardProfile.GetCopy(new KeyboardProfileCreator()));

            txtAutosaveChanges.Text = SettingsManager.Settings.AutoSaveMinChanges.ToStringInvariant();
            txtAutosaveInterval.Text = SettingsManager.Settings.AutoSaveMinIntervalSeconds.ToStringInvariant();
            cbxAutosaveCompression.Checked = SettingsManager.Settings.AutoSaveCompress;
            cbxAutosaveEnabled.Checked = SettingsManager.Settings.AutoSaveEnabled;
            cbxAskDirectories.Checked = SettingsManager.Settings.DirectoriesPrompt;
            cbxPointerDirect.Checked = SettingsManager.Settings.DirectPointer;
            DisplayFont = SettingsManager.Settings.MakeFont();
            UpdateDisplayFontLabel();
            txtFOV.Text = SettingsManager.Settings.FOVDefault.ToStringInvariant();

            MinimapCliffColour = new Rgba(SettingsManager.Settings.MinimapCliffColour);
            clrMinimapCliffs = new ColourControl(MinimapCliffColour);
            pnlMinimapCliffColour.Controls.Add(clrMinimapCliffs);

            MinimapSelectedObjectColour = new Rgba(SettingsManager.Settings.MinimapSelectedObjectsColour);
            clrMinimapSelectedObjects = new ColourControl(MinimapSelectedObjectColour);
            pnlMinimapSelectedObjectColour.Controls.Add(clrMinimapSelectedObjects);

            txtMinimapSize.Text = SettingsManager.Settings.MinimapSize.ToStringInvariant();
            cbxMinimapObjectColours.Checked = SettingsManager.Settings.MinimapTeamColours;
            cbxMinimapTeamColourFeatures.Checked = SettingsManager.Settings.MinimapTeamColoursExceptFeatures;
            cbxMipmaps.Checked = SettingsManager.Settings.Mipmaps;
            cbxMipmapsHardware.Checked = SettingsManager.Settings.MipmapsHardware;
            txtUndoSteps.Text = SettingsManager.Settings.UndoLimit.ToStringInvariant();

            tilesetsPathSetControl.SetPaths(SettingsManager.Settings.TilesetDirectories);
            objectDataPathSetControl.SetPaths(SettingsManager.Settings.ObjectDataDirectories);

            txtMapBPP.Text = SettingsManager.Settings.MapViewBPP.ToStringInvariant();
            txtMapDepth.Text = SettingsManager.Settings.MapViewDepth.ToStringInvariant();
            txtTexturesBPP.Text = SettingsManager.Settings.TextureViewBPP.ToStringInvariant();
            txtTexturesDepth.Text = SettingsManager.Settings.TextureViewDepth.ToStringInvariant();

            cbxPickerOrientation.Checked = SettingsManager.Settings.PickOrientation;

            UpdateKeyboardControls(-1);
        }

        public void btnSave_Click(Object sender, EventArgs e)
        {
            var NewSettings = (clsSettings)(SettingsManager.Settings.GetCopy(new SettingsCreator()));
            double dblTemp = 0;
            var intTemp = 0;

            if ( IOUtil.InvariantParse(txtAutosaveChanges.Text, ref dblTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingAutoSaveMinChanges,
                    new Change<UInt32>((uint)(MathUtil.ClampDbl(dblTemp, 1.0D, (Convert.ToDouble(UInt32.MaxValue)) - 1.0D))));
            }
            if ( IOUtil.InvariantParse(txtAutosaveInterval.Text, ref dblTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingAutoSaveMinIntervalS,
                    new Change<UInt32>((uint)(MathUtil.ClampDbl(dblTemp, 1.0D, (Convert.ToDouble(UInt32.MaxValue)) - 1.0D))));
            }
            NewSettings.SetChanges(SettingsManager.SettingAutoSaveCompress, new Change<bool>(cbxAutosaveCompression.Checked));
            NewSettings.SetChanges(SettingsManager.SettingAutoSaveEnabled, new Change<bool>(cbxAutosaveEnabled.Checked));
            NewSettings.SetChanges(SettingsManager.SettingDirectoriesPrompt, new Change<bool>(cbxAskDirectories.Checked));
            NewSettings.SetChanges(SettingsManager.SettingDirectPointer, new Change<bool>(cbxPointerDirect.Checked));
            NewSettings.SetChanges(SettingsManager.SettingFontFamily, new Change<FontFamily>(DisplayFont.FontFamily));
            if ( IOUtil.InvariantParse(txtFOV.Text, ref dblTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingFovDefault, new Change<double>(dblTemp));
            }
            NewSettings.SetChanges(SettingsManager.SettingMinimapCliffColour, new Change<Rgba>(MinimapCliffColour));
            NewSettings.SetChanges(SettingsManager.SettingMinimapSelectedObjectsColour, new Change<Rgba>(MinimapSelectedObjectColour));
            if ( IOUtil.InvariantParse(txtMinimapSize.Text, ref intTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingMinimapSize, new Change<int>(intTemp));
            }
            NewSettings.SetChanges(SettingsManager.SettingMinimapTeamColours, new Change<bool>(cbxMinimapObjectColours.Checked));
            NewSettings.SetChanges(SettingsManager.SettingMinimapTeamColoursExceptFeatures, new Change<bool>(cbxMinimapTeamColourFeatures.Checked));
            NewSettings.SetChanges(SettingsManager.SettingMipmaps, new Change<bool>(cbxMipmaps.Checked));
            NewSettings.SetChanges(SettingsManager.SettingMipmapsHardware, new Change<bool>(cbxMipmapsHardware.Checked));
            if ( IOUtil.InvariantParse(txtUndoSteps.Text, ref intTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingUndoLimit, new Change<int>(intTemp));
            }
            var tilesetPaths = new List<string>();
            var objectsPaths = new List<string>();
            var controlTilesetPaths = tilesetsPathSetControl.GetPaths;
            var controlobjectsPaths = objectDataPathSetControl.GetPaths;
            for ( var i = 0; i <= controlTilesetPaths.GetUpperBound(0); i++ )
            {
                tilesetPaths.Add(controlTilesetPaths[i]);
            }
            for ( var i = 0; i <= controlobjectsPaths.GetUpperBound(0); i++ )
            {
                objectsPaths.Add(controlobjectsPaths[i]);
            }
            NewSettings.SetChanges(SettingsManager.SettingTilesetDirectories, new Change<List<string>>(tilesetPaths));
            NewSettings.SetChanges(SettingsManager.SettingObjectDataDirectories, new Change<List<string>>(objectsPaths));
            if ( IOUtil.InvariantParse(txtMapBPP.Text, ref intTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingMapViewBpp, new Change<int>(intTemp));
            }
            if ( IOUtil.InvariantParse(txtMapDepth.Text, ref intTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingMapViewDepth, new Change<int>(intTemp));
            }
            if ( IOUtil.InvariantParse(txtTexturesBPP.Text, ref intTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingTextureViewBpp, new Change<int>(intTemp));
            }
            if ( IOUtil.InvariantParse(txtTexturesDepth.Text, ref intTemp) )
            {
                NewSettings.SetChanges(SettingsManager.SettingTextureViewDepth, new Change<int>(intTemp));
            }
            NewSettings.SetChanges(SettingsManager.SettingPickOrientation, new Change<bool>(cbxPickerOrientation.Checked));

            SettingsManager.UpdateSettings(NewSettings);

            var Map = Program.frmMainInstance.MainMap;
            if ( Map != null )
            {
                Map.MinimapMakeLater();
            }
            Program.frmMainInstance.View_DrawViewLater();

            KeyboardManager.KeyboardProfile = ChangedKeyControls;

            Finish(DialogResult.OK);
        }

        public void btnCancel_Click(Object sender, EventArgs e)
        {
            Finish(DialogResult.Cancel);
        }

        public void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !AllowClose;
        }

        //setting DialogResult in mono tries to close the form

        private void Finish(DialogResult result)
        {
            AllowClose = true;
            Program.frmOptionsInstance = null;
            if ( Modal )
            {
                DialogResult = result; //mono closes here
#if !Mono
                Close();
#endif
            }
            else
            {
                Close();
            }
        }

        public void btnFont_Click(Object sender, EventArgs e)
        {
            var FontDialog = new FontDialog();

            var Result = default(DialogResult);
            try //mono 267 has crashed here
            {
                FontDialog.Font = DisplayFont;
                FontDialog.FontMustExist = true;
                Result = FontDialog.ShowDialog(this);
            }
            catch
            {
                Result = DialogResult.Cancel;
            }
            if ( Result == DialogResult.OK )
            {
                DisplayFont = FontDialog.Font;
                UpdateDisplayFontLabel();
            }
        }

        public void btnAutosaveOpen_Click(Object sender, EventArgs e)
        {
            Program.frmMainInstance.Load_Autosave_Prompt();
        }

        private void UpdateDisplayFontLabel()
        {
            lblFont.Text = DisplayFont.FontFamily.Name + " " + Convert.ToString(DisplayFont.SizeInPoints) + " ";
            if ( DisplayFont.Bold )
            {
                lblFont.Text += "B";
            }
            if ( DisplayFont.Italic )
            {
                lblFont.Text += "I";
            }
        }

        private void UpdateKeyboardControl(int index)
        {
            lstKeyboardControls.Items[index] = GetKeyControlText((Option<KeyboardControl>)(KeyboardManager.OptionsKeyboardControls.Options[index]));
        }

        private void UpdateKeyboardControls(int selectedIndex)
        {
            lstKeyboardControls.Hide();
            lstKeyboardControls.Items.Clear();
            lstKeyboardControls_Items = new Option<KeyboardControl>[KeyboardManager.OptionsKeyboardControls.Options.Count];
            for ( var i = 0; i <= KeyboardManager.OptionsKeyboardControls.Options.Count - 1; i++ )
            {
                var item = (Option<KeyboardControl>)(KeyboardManager.OptionsKeyboardControls.Options[i]);
                var text = GetKeyControlText(item);
                lstKeyboardControls_Items[lstKeyboardControls.Items.Add(text)] = item;
            }
            lstKeyboardControls.SelectedIndex = selectedIndex;
            lstKeyboardControls.Show();
        }

        private string GetKeyControlText(Option<KeyboardControl> item)
        {
            var text = item.SaveKey + " = ";
            var control = (KeyboardControl)(ChangedKeyControls.GetValue(item));
            for ( var j = 0; j <= control.Keys.GetUpperBound(0); j++ )
            {
                var key = Keys.A;
                var keyText = Enum.GetName(typeof(Keys), key);
                if ( keyText == null )
                {
                    text += ((Int32)key).ToStringInvariant();
                }
                else
                {
                    text += keyText;
                }
                if ( j < control.Keys.GetUpperBound(0) )
                {
                    text += " + ";
                }
            }
            if ( control.UnlessKeys.GetUpperBound(0) >= 0 )
            {
                text += " unless ";
                for ( var j = 0; j <= control.UnlessKeys.GetUpperBound(0); j++ )
                {
                    var key = Keys.A;
                    var keyText = Enum.GetName(typeof(Keys), key);
                    if ( keyText == null )
                    {
                        text += ((Int32)key).ToStringInvariant();
                    }
                    else
                    {
                        text += keyText;
                    }
                    if ( j < control.UnlessKeys.GetUpperBound(0) )
                    {
                        text += ", ";
                    }
                }
            }
            if ( control != item.DefaultValue )
            {
                text += " (modified)";
            }

            return text;
        }

        public void btnKeyControlChange_Click(Object sender, EventArgs e)
        {
            if ( lstKeyboardControls.SelectedIndex < 0 )
            {
                return;
            }

            var capture = new frmKeyboardControl();
            if ( capture.ShowDialog() != DialogResult.OK )
            {
                return;
            }
            if ( capture.Results.Count == 0 )
            {
                return;
            }
            var keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            var previous = (KeyboardControl)(ChangedKeyControls.GetValue(keyOption));

            var keys = new Keys[capture.Results.Count];
            for ( var i = 0; i <= capture.Results.Count - 1; i++ )
            {
                keys[i] = capture.Results[i];
            }
            var copy = new KeyboardControl(keys, previous.UnlessKeys);
            ChangedKeyControls.SetChanges(keyOption, new Change<KeyboardControl>(copy));
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }

        public void btnKeyControlChangeUnless_Click(Object sender, EventArgs e)
        {
            if ( lstKeyboardControls.SelectedIndex < 0 )
            {
                return;
            }

            var capture = new frmKeyboardControl();
            if ( capture.ShowDialog() != DialogResult.OK )
            {
                return;
            }
            if ( capture.Results.Count == 0 )
            {
                return;
            }
            var keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            var previous = (KeyboardControl)(ChangedKeyControls.GetValue(keyOption));

            var unlessKeys = new Keys[capture.Results.Count];
            for ( var i = 0; i <= capture.Results.Count - 1; i++ )
            {
                unlessKeys[i] = capture.Results[i];
            }
            var copy = new KeyboardControl(previous.Keys, unlessKeys);
            ChangedKeyControls.SetChanges(keyOption, new Change<KeyboardControl>(copy));
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }

        public void btnKeyControlChangeDefault_Click(Object sender, EventArgs e)
        {
            if ( lstKeyboardControls.SelectedIndex < 0 )
            {
                return;
            }

            var keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            ChangedKeyControls.SetChanges(keyOption, null);
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }
    }
}