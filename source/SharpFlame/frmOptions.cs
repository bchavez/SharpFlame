using System;
using System.Drawing;
using System.Windows.Forms;
using SharpFlame.Collections;
using SharpFlame.Colors;
using SharpFlame.FileIO;
using SharpFlame.MathExtra;

namespace SharpFlame
{
    public partial class frmOptions
    {
        private Font DisplayFont;

        private clsRGBA_sng MinimapCliffColour;
        private ctrlColour clrMinimapCliffs;
        private clsRGBA_sng MinimapSelectedObjectColour;
        private ctrlColour clrMinimapSelectedObjects;
        private ctrlPathSet ObjectDataPathSet = new ctrlPathSet("Object Data Directories");
        private ctrlPathSet TilesetsPathSet = new ctrlPathSet("Tilesets Directories");

        private modControls.clsKeyboardProfile ChangedKeyControls;

        private clsOption<clsKeyboardControl>[] lstKeyboardControls_Items = new clsOption<clsKeyboardControl>[] {};

        public frmOptions()
        {
            InitializeComponent();

            Icon = modProgram.ProgramIcon;

#if Mono
			foreach (TabPage tab in TabControl1.TabPages)
			{
				tab.Text += " ";
			}
#endif

            TilesetsPathSet.Dock = DockStyle.Fill;
            ObjectDataPathSet.Dock = DockStyle.Fill;
            TableLayoutPanel1.Controls.Add(TilesetsPathSet, 0, 0);
            TableLayoutPanel1.Controls.Add(ObjectDataPathSet, 0, 1);

            ChangedKeyControls = (modControls.clsKeyboardProfile)(modControls.KeyboardProfile.GetCopy(new modControls.clsKeyboardProfileCreator()));

            txtAutosaveChanges.Text = IOUtil.InvariantToString_uint(modSettings.Settings.AutoSaveMinChanges);
            txtAutosaveInterval.Text = IOUtil.InvariantToString_uint(modSettings.Settings.AutoSaveMinInterval_s);
            cbxAutosaveCompression.Checked = modSettings.Settings.AutoSaveCompress;
            cbxAutosaveEnabled.Checked = modSettings.Settings.AutoSaveEnabled;
            cbxAskDirectories.Checked = modSettings.Settings.DirectoriesPrompt;
            cbxPointerDirect.Checked = modSettings.Settings.DirectPointer;
            DisplayFont = modSettings.Settings.MakeFont();
            UpdateDisplayFontLabel();
            txtFOV.Text = IOUtil.InvariantToString_dbl(modSettings.Settings.FOVDefault);

            MinimapCliffColour = new clsRGBA_sng(modSettings.Settings.MinimapCliffColour);
            clrMinimapCliffs = new ctrlColour(MinimapCliffColour);
            pnlMinimapCliffColour.Controls.Add(clrMinimapCliffs);

            MinimapSelectedObjectColour = new clsRGBA_sng(modSettings.Settings.MinimapSelectedObjectsColour);
            clrMinimapSelectedObjects = new ctrlColour(MinimapSelectedObjectColour);
            pnlMinimapSelectedObjectColour.Controls.Add(clrMinimapSelectedObjects);

            txtMinimapSize.Text = IOUtil.InvariantToString_int(modSettings.Settings.MinimapSize);
            cbxMinimapObjectColours.Checked = modSettings.Settings.MinimapTeamColours;
            cbxMinimapTeamColourFeatures.Checked = modSettings.Settings.MinimapTeamColoursExceptFeatures;
            cbxMipmaps.Checked = modSettings.Settings.Mipmaps;
            cbxMipmapsHardware.Checked = modSettings.Settings.MipmapsHardware;
            txtUndoSteps.Text = IOUtil.InvariantToString_uint(modSettings.Settings.UndoLimit);

            TilesetsPathSet.SetPaths(modSettings.Settings.TilesetDirectories);
            ObjectDataPathSet.SetPaths(modSettings.Settings.ObjectDataDirectories);
            TilesetsPathSet.SelectedNum = MathUtil.Clamp_int(Convert.ToInt32(modSettings.Settings.get_Value(modSettings.Setting_DefaultTilesetsPathNum)), -1,
                modSettings.Settings.TilesetDirectories.Count - 1);
            ObjectDataPathSet.SelectedNum = MathUtil.Clamp_int(Convert.ToInt32(modSettings.Settings.get_Value(modSettings.Setting_DefaultObjectDataPathNum)),
                -1, modSettings.Settings.ObjectDataDirectories.Count - 1);

            txtMapBPP.Text = IOUtil.InvariantToString_int(modSettings.Settings.MapViewBPP);
            txtMapDepth.Text = IOUtil.InvariantToString_int(modSettings.Settings.MapViewDepth);
            txtTexturesBPP.Text = IOUtil.InvariantToString_int(modSettings.Settings.TextureViewBPP);
            txtTexturesDepth.Text = IOUtil.InvariantToString_int(modSettings.Settings.TextureViewDepth);

            cbxPickerOrientation.Checked = modSettings.Settings.PickOrientation;

            UpdateKeyboardControls(-1);
        }

        public void btnSave_Click(Object sender, EventArgs e)
        {
            modSettings.clsSettings NewSettings = (modSettings.clsSettings)(modSettings.Settings.GetCopy(new modSettings.clsSettingsCreator()));
            double dblTemp = 0;
            int intTemp = 0;

            if ( IOUtil.InvariantParse_dbl(txtAutosaveChanges.Text, ref dblTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_AutoSaveMinChanges,
                    new clsOptionProfile.clsChange<UInt32>((uint)(MathUtil.Clamp_dbl(dblTemp, 1.0D, (Convert.ToDouble(UInt32.MaxValue)) - 1.0D))));
            }
            if ( IOUtil.InvariantParse_dbl(txtAutosaveInterval.Text, ref dblTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_AutoSaveMinInterval_s,
                    new clsOptionProfile.clsChange<UInt32>((uint)(MathUtil.Clamp_dbl(dblTemp, 1.0D, (Convert.ToDouble(UInt32.MaxValue)) - 1.0D))));
            }
            NewSettings.set_Changes(modSettings.Setting_AutoSaveCompress, new clsOptionProfile.clsChange<bool>(cbxAutosaveCompression.Checked));
            NewSettings.set_Changes(modSettings.Setting_AutoSaveEnabled, new clsOptionProfile.clsChange<bool>(cbxAutosaveEnabled.Checked));
            NewSettings.set_Changes(modSettings.Setting_DirectoriesPrompt, new clsOptionProfile.clsChange<bool>(cbxAskDirectories.Checked));
            NewSettings.set_Changes(modSettings.Setting_DirectPointer, new clsOptionProfile.clsChange<bool>(cbxPointerDirect.Checked));
            NewSettings.set_Changes(modSettings.Setting_FontFamily, new clsOptionProfile.clsChange<FontFamily>(DisplayFont.FontFamily));
            if ( IOUtil.InvariantParse_dbl(txtFOV.Text, ref dblTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_FOVDefault, new clsOptionProfile.clsChange<double>(dblTemp));
            }
            NewSettings.set_Changes(modSettings.Setting_MinimapCliffColour, new clsOptionProfile.clsChange<clsRGBA_sng>(MinimapCliffColour));
            NewSettings.set_Changes(modSettings.Setting_MinimapSelectedObjectsColour, new clsOptionProfile.clsChange<clsRGBA_sng>(MinimapSelectedObjectColour));
            if ( IOUtil.InvariantParse_int(txtMinimapSize.Text, ref intTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_MinimapSize, new clsOptionProfile.clsChange<int>(intTemp));
            }
            NewSettings.set_Changes(modSettings.Setting_MinimapTeamColours, new clsOptionProfile.clsChange<bool>(cbxMinimapObjectColours.Checked));
            NewSettings.set_Changes(modSettings.Setting_MinimapTeamColoursExceptFeatures, new clsOptionProfile.clsChange<bool>(cbxMinimapTeamColourFeatures.Checked));
            NewSettings.set_Changes(modSettings.Setting_Mipmaps, new clsOptionProfile.clsChange<bool>(cbxMipmaps.Checked));
            NewSettings.set_Changes(modSettings.Setting_MipmapsHardware, new clsOptionProfile.clsChange<bool>(cbxMipmapsHardware.Checked));
            if ( IOUtil.InvariantParse_int(txtUndoSteps.Text, ref intTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_UndoLimit, new clsOptionProfile.clsChange<int>(intTemp));
            }
            SimpleList<string> tilesetPaths = new SimpleList<string>();
            SimpleList<string> objectsPaths = new SimpleList<string>();
            string[] controlTilesetPaths = TilesetsPathSet.GetPaths;
            string[] controlobjectsPaths = ObjectDataPathSet.GetPaths;
            for ( int i = 0; i <= controlTilesetPaths.GetUpperBound(0); i++ )
            {
                tilesetPaths.Add(controlTilesetPaths[i]);
            }
            for ( int i = 0; i <= controlobjectsPaths.GetUpperBound(0); i++ )
            {
                objectsPaths.Add(controlobjectsPaths[i]);
            }
            NewSettings.set_Changes(modSettings.Setting_TilesetDirectories, new clsOptionProfile.clsChange<SimpleList<string>>(tilesetPaths));
            NewSettings.set_Changes(modSettings.Setting_ObjectDataDirectories, new clsOptionProfile.clsChange<SimpleList<string>>(objectsPaths));
            NewSettings.set_Changes(modSettings.Setting_DefaultTilesetsPathNum, new clsOptionProfile.clsChange<int>(TilesetsPathSet.SelectedNum));
            NewSettings.set_Changes(modSettings.Setting_DefaultObjectDataPathNum, new clsOptionProfile.clsChange<int>(ObjectDataPathSet.SelectedNum));
            if ( IOUtil.InvariantParse_int(txtMapBPP.Text, ref intTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_MapViewBPP, new clsOptionProfile.clsChange<int>(intTemp));
            }
            if ( IOUtil.InvariantParse_int(txtMapDepth.Text, ref intTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_MapViewDepth, new clsOptionProfile.clsChange<int>(intTemp));
            }
            if ( IOUtil.InvariantParse_int(txtTexturesBPP.Text, ref intTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_TextureViewBPP, new clsOptionProfile.clsChange<int>(intTemp));
            }
            if ( IOUtil.InvariantParse_int(txtTexturesDepth.Text, ref intTemp) )
            {
                NewSettings.set_Changes(modSettings.Setting_TextureViewDepth, new clsOptionProfile.clsChange<int>(intTemp));
            }
            NewSettings.set_Changes(modSettings.Setting_PickOrientation, new clsOptionProfile.clsChange<bool>(cbxPickerOrientation.Checked));

            modSettings.UpdateSettings(NewSettings);

            clsMap Map = Program.frmMainInstance.MainMap;
            if ( Map != null )
            {
                Map.MinimapMakeLater();
            }
            Program.frmMainInstance.View_DrawViewLater();

            modControls.KeyboardProfile = ChangedKeyControls;

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
            lstKeyboardControls.Items[index] = GetKeyControlText((clsOption<clsKeyboardControl>)(modControls.Options_KeyboardControls.Options[index]));
        }

        private void UpdateKeyboardControls(int selectedIndex)
        {
            lstKeyboardControls.Hide();
            lstKeyboardControls.Items.Clear();
            lstKeyboardControls_Items = new clsOption<clsKeyboardControl>[modControls.Options_KeyboardControls.Options.Count];
            for ( int i = 0; i <= modControls.Options_KeyboardControls.Options.Count - 1; i++ )
            {
                clsOption<clsKeyboardControl> item = (clsOption<clsKeyboardControl>)(modControls.Options_KeyboardControls.Options[i]);
                string text = GetKeyControlText(item);
                lstKeyboardControls_Items[lstKeyboardControls.Items.Add(text)] = item;
            }
            lstKeyboardControls.SelectedIndex = selectedIndex;
            lstKeyboardControls.Show();
        }

        private string GetKeyControlText(clsOption<clsKeyboardControl> item)
        {
            string text = item.SaveKey + " = ";
            clsKeyboardControl control = (clsKeyboardControl)(ChangedKeyControls.get_Value(item));
            for ( int j = 0; j <= control.Keys.GetUpperBound(0); j++ )
            {
                Keys key = Keys.A;
                string keyText = Enum.GetName(typeof(Keys), key);
                if ( keyText == null )
                {
                    text += IOUtil.InvariantToString_int((Int32)key);
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
                        text += IOUtil.InvariantToString_int((Int32)key);
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
            clsOption<clsKeyboardControl> keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            clsKeyboardControl previous = (clsKeyboardControl)(ChangedKeyControls.get_Value(keyOption));

            Keys[] keys = new Keys[capture.Results.Count];
            for ( int i = 0; i <= capture.Results.Count - 1; i++ )
            {
                keys[i] = capture.Results[i].Item;
            }
            clsKeyboardControl copy = new clsKeyboardControl(keys, previous.UnlessKeys);
            ChangedKeyControls.set_Changes(keyOption, new clsOptionProfile.clsChange<clsKeyboardControl>(copy));
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
            clsOption<clsKeyboardControl> keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            clsKeyboardControl previous = (clsKeyboardControl)(ChangedKeyControls.get_Value(keyOption));

            Keys[] unlessKeys = new Keys[capture.Results.Count];
            for ( int i = 0; i <= capture.Results.Count - 1; i++ )
            {
                unlessKeys[i] = capture.Results[i].Item;
            }
            clsKeyboardControl copy = new clsKeyboardControl(previous.Keys, unlessKeys);
            ChangedKeyControls.set_Changes(keyOption, new clsOptionProfile.clsChange<clsKeyboardControl>(copy));
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }

        public void btnKeyControlChangeDefault_Click(Object sender, EventArgs e)
        {
            if ( lstKeyboardControls.SelectedIndex < 0 )
            {
                return;
            }

            clsOption<clsKeyboardControl> keyOption = lstKeyboardControls_Items[lstKeyboardControls.SelectedIndex];
            ChangedKeyControls.set_Changes(keyOption, null);
            UpdateKeyboardControl(keyOption.GroupLink.ArrayPosition);
        }
    }
}