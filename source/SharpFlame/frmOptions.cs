using System;
using System.Drawing;
using System.Windows.Forms;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.Controls;
using SharpFlame.FileIO;
using SharpFlame.AppSettings;
using SharpFlame.Mapping;
using SharpFlame.Maths;

namespace SharpFlame
{
    public partial class frmOptions
    {
        private Font DisplayFont;

        private clsRGBA_sng MinimapCliffColour;
        private ColourControl clrMinimapCliffs;
        private clsRGBA_sng MinimapSelectedObjectColour;
        private ColourControl clrMinimapSelectedObjects;
        private PathSetControl objectDataPathSetControl = new PathSetControl("Object Data Directories");
        private PathSetControl tilesetsPathSetControl = new PathSetControl("Tilesets Directories");

        private KeyboardProfile ChangedKeyControls;

        private Option<KeyboardControl>[] lstKeyboardControls_Items = new Option<KeyboardControl>[] {};

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
            txtAutosaveInterval.Text = SettingsManager.Settings.AutoSaveMinInterval_s.ToStringInvariant();
            cbxAutosaveCompression.Checked = SettingsManager.Settings.AutoSaveCompress;
            cbxAutosaveEnabled.Checked = SettingsManager.Settings.AutoSaveEnabled;
            cbxAskDirectories.Checked = SettingsManager.Settings.DirectoriesPrompt;
            cbxPointerDirect.Checked = SettingsManager.Settings.DirectPointer;
            DisplayFont = SettingsManager.Settings.MakeFont();
            UpdateDisplayFontLabel();
            txtFOV.Text = SettingsManager.Settings.FOVDefault.ToStringInvariant();

            MinimapCliffColour = new clsRGBA_sng(SettingsManager.Settings.MinimapCliffColour);
            clrMinimapCliffs = new ColourControl(MinimapCliffColour);
            pnlMinimapCliffColour.Controls.Add(clrMinimapCliffs);

            MinimapSelectedObjectColour = new clsRGBA_sng(SettingsManager.Settings.MinimapSelectedObjectsColour);
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
            tilesetsPathSetControl.SelectedNum = MathUtil.Clamp_int(Convert.ToInt32(SettingsManager.Settings.get_Value(SettingsManager.Setting_DefaultTilesetsPathNum)), -1,
                SettingsManager.Settings.TilesetDirectories.Count - 1);
            objectDataPathSetControl.SelectedNum = MathUtil.Clamp_int(Convert.ToInt32(SettingsManager.Settings.get_Value(SettingsManager.Setting_DefaultObjectDataPathNum)),
                -1, SettingsManager.Settings.ObjectDataDirectories.Count - 1);

            txtMapBPP.Text = SettingsManager.Settings.MapViewBPP.ToStringInvariant();
            txtMapDepth.Text = SettingsManager.Settings.MapViewDepth.ToStringInvariant();
            txtTexturesBPP.Text = SettingsManager.Settings.TextureViewBPP.ToStringInvariant();
            txtTexturesDepth.Text = SettingsManager.Settings.TextureViewDepth.ToStringInvariant();

            cbxPickerOrientation.Checked = SettingsManager.Settings.PickOrientation;

            UpdateKeyboardControls(-1);
        }

        public void btnSave_Click(Object sender, EventArgs e)
        {
            clsSettings NewSettings = (clsSettings)(SettingsManager.Settings.GetCopy(new SettingsCreator()));
            double dblTemp = 0;
            int intTemp = 0;

            if ( IOUtil.InvariantParse(txtAutosaveChanges.Text, ref dblTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_AutoSaveMinChanges,
                    new Change<UInt32>((uint)(MathUtil.Clamp_dbl(dblTemp, 1.0D, (Convert.ToDouble(UInt32.MaxValue)) - 1.0D))));
            }
            if ( IOUtil.InvariantParse(txtAutosaveInterval.Text, ref dblTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_AutoSaveMinInterval_s,
                    new Change<UInt32>((uint)(MathUtil.Clamp_dbl(dblTemp, 1.0D, (Convert.ToDouble(UInt32.MaxValue)) - 1.0D))));
            }
            NewSettings.set_Changes(SettingsManager.Setting_AutoSaveCompress, new Change<bool>(cbxAutosaveCompression.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_AutoSaveEnabled, new Change<bool>(cbxAutosaveEnabled.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_DirectoriesPrompt, new Change<bool>(cbxAskDirectories.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_DirectPointer, new Change<bool>(cbxPointerDirect.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_FontFamily, new Change<FontFamily>(DisplayFont.FontFamily));
            if ( IOUtil.InvariantParse(txtFOV.Text, ref dblTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_FOVDefault, new Change<double>(dblTemp));
            }
            NewSettings.set_Changes(SettingsManager.Setting_MinimapCliffColour, new Change<clsRGBA_sng>(MinimapCliffColour));
            NewSettings.set_Changes(SettingsManager.Setting_MinimapSelectedObjectsColour, new Change<clsRGBA_sng>(MinimapSelectedObjectColour));
            if ( IOUtil.InvariantParse(txtMinimapSize.Text, ref intTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_MinimapSize, new Change<int>(intTemp));
            }
            NewSettings.set_Changes(SettingsManager.Setting_MinimapTeamColours, new Change<bool>(cbxMinimapObjectColours.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_MinimapTeamColoursExceptFeatures, new Change<bool>(cbxMinimapTeamColourFeatures.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_Mipmaps, new Change<bool>(cbxMipmaps.Checked));
            NewSettings.set_Changes(SettingsManager.Setting_MipmapsHardware, new Change<bool>(cbxMipmapsHardware.Checked));
            if ( IOUtil.InvariantParse(txtUndoSteps.Text, ref intTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_UndoLimit, new Change<int>(intTemp));
            }
            SimpleList<string> tilesetPaths = new SimpleList<string>();
            SimpleList<string> objectsPaths = new SimpleList<string>();
            string[] controlTilesetPaths = tilesetsPathSetControl.GetPaths;
            string[] controlobjectsPaths = objectDataPathSetControl.GetPaths;
            for ( int i = 0; i <= controlTilesetPaths.GetUpperBound(0); i++ )
            {
                tilesetPaths.Add(controlTilesetPaths[i]);
            }
            for ( int i = 0; i <= controlobjectsPaths.GetUpperBound(0); i++ )
            {
                objectsPaths.Add(controlobjectsPaths[i]);
            }
            NewSettings.set_Changes(SettingsManager.Setting_TilesetDirectories, new Change<SimpleList<string>>(tilesetPaths));
            NewSettings.set_Changes(SettingsManager.Setting_ObjectDataDirectories, new Change<SimpleList<string>>(objectsPaths));
            NewSettings.set_Changes(SettingsManager.Setting_DefaultTilesetsPathNum, new Change<int>(tilesetsPathSetControl.SelectedNum));
            NewSettings.set_Changes(SettingsManager.Setting_DefaultObjectDataPathNum, new Change<int>(objectDataPathSetControl.SelectedNum));
            if ( IOUtil.InvariantParse(txtMapBPP.Text, ref intTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_MapViewBPP, new Change<int>(intTemp));
            }
            if ( IOUtil.InvariantParse(txtMapDepth.Text, ref intTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_MapViewDepth, new Change<int>(intTemp));
            }
            if ( IOUtil.InvariantParse(txtTexturesBPP.Text, ref intTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_TextureViewBPP, new Change<int>(intTemp));
            }
            if ( IOUtil.InvariantParse(txtTexturesDepth.Text, ref intTemp) )
            {
                NewSettings.set_Changes(SettingsManager.Setting_TextureViewDepth, new Change<int>(intTemp));
            }
            NewSettings.set_Changes(SettingsManager.Setting_PickOrientation, new Change<bool>(cbxPickerOrientation.Checked));

            SettingsManager.UpdateSettings(NewSettings);

            clsMap Map = Program.frmMainInstance.MainMap;
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

        private bool AllowClose = false;

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
            FontDialog FontDialog = new FontDialog();

            DialogResult Result = default(DialogResult);
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
            for ( int i = 0; i <= KeyboardManager.OptionsKeyboardControls.Options.Count - 1; i++ )
            {
                Option<KeyboardControl> item = (Option<KeyboardControl>)(KeyboardManager.OptionsKeyboardControls.Options[i]);
                string text = GetKeyControlText(item);
                lstKeyboardControls_Items[lstKeyboardControls.Items.Add(text)] = item;
            }
            lstKeyboardControls.SelectedIndex = selectedIndex;
            lstKeyboardControls.Show();
        }

        private string GetKeyControlText(Option<KeyboardControl> item)
        {
            string text = item.SaveKey + " = ";
            KeyboardControl control = (KeyboardControl)(ChangedKeyControls.get_Value(item));
            for ( int j = 0; j <= control.Keys.GetUpperBound(0); j++ )
            {
                Keys key = Keys.A;
                string keyText = Enum.GetName(typeof(Keys), key);
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
                for ( int j = 0; j <= control.UnlessKeys.GetUpperBound(0); j++ )
                {
                    Keys key = Keys.A;
                    string keyText = Enum.GetName(typeof(Keys), key);
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

            frmKeyboardControl capture = new frmKeyboardControl();
            if ( capture.ShowDialog() != DialogResult.OK )
            {
                return;
            }
            if ( capture.Results.Count == 0 )
            {
                return;
            }
            Option<KeyboardControl> keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            KeyboardControl previous = (KeyboardControl)(ChangedKeyControls.get_Value(keyOption));

            Keys[] keys = new Keys[capture.Results.Count];
            for ( int i = 0; i <= capture.Results.Count - 1; i++ )
            {
                keys[i] = capture.Results[i].Item;
            }
            KeyboardControl copy = new KeyboardControl(keys, previous.UnlessKeys);
            ChangedKeyControls.set_Changes(keyOption, new Change<KeyboardControl>(copy));
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }

        public void btnKeyControlChangeUnless_Click(Object sender, EventArgs e)
        {
            if ( lstKeyboardControls.SelectedIndex < 0 )
            {
                return;
            }

            frmKeyboardControl capture = new frmKeyboardControl();
            if ( capture.ShowDialog() != DialogResult.OK )
            {
                return;
            }
            if ( capture.Results.Count == 0 )
            {
                return;
            }
            Option<KeyboardControl> keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            KeyboardControl previous = (KeyboardControl)(ChangedKeyControls.get_Value(keyOption));

            Keys[] unlessKeys = new Keys[capture.Results.Count];
            for ( int i = 0; i <= capture.Results.Count - 1; i++ )
            {
                unlessKeys[i] = capture.Results[i].Item;
            }
            KeyboardControl copy = new KeyboardControl(previous.Keys, unlessKeys);
            ChangedKeyControls.set_Changes(keyOption, new Change<KeyboardControl>(copy));
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }

        public void btnKeyControlChangeDefault_Click(Object sender, EventArgs e)
        {
            if ( lstKeyboardControls.SelectedIndex < 0 )
            {
                return;
            }

            Option<KeyboardControl> keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            ChangedKeyControls.set_Changes(keyOption, null);
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }
    }
}