namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class frmOptions : Form
    {
        [AccessedThroughProperty("btnAutosaveOpen")]
        private Button _btnAutosaveOpen;
        [AccessedThroughProperty("btnCancel")]
        private Button _btnCancel;
        [AccessedThroughProperty("btnFont")]
        private Button _btnFont;
        [AccessedThroughProperty("btnKeyControlChange")]
        private Button _btnKeyControlChange;
        [AccessedThroughProperty("btnKeyControlChangeDefault")]
        private Button _btnKeyControlChangeDefault;
        [AccessedThroughProperty("btnKeyControlChangeUnless")]
        private Button _btnKeyControlChangeUnless;
        [AccessedThroughProperty("btnSave")]
        private Button _btnSave;
        [AccessedThroughProperty("cbxAskDirectories")]
        private CheckBox _cbxAskDirectories;
        [AccessedThroughProperty("cbxAutosaveCompression")]
        private CheckBox _cbxAutosaveCompression;
        [AccessedThroughProperty("cbxAutosaveEnabled")]
        private CheckBox _cbxAutosaveEnabled;
        [AccessedThroughProperty("cbxMinimapObjectColours")]
        private CheckBox _cbxMinimapObjectColours;
        [AccessedThroughProperty("cbxMinimapTeamColourFeatures")]
        private CheckBox _cbxMinimapTeamColourFeatures;
        [AccessedThroughProperty("cbxMipmaps")]
        private CheckBox _cbxMipmaps;
        [AccessedThroughProperty("cbxMipmapsHardware")]
        private CheckBox _cbxMipmapsHardware;
        [AccessedThroughProperty("cbxPickerOrientation")]
        private CheckBox _cbxPickerOrientation;
        [AccessedThroughProperty("cbxPointerDirect")]
        private CheckBox _cbxPointerDirect;
        [AccessedThroughProperty("GroupBox1")]
        private GroupBox _GroupBox1;
        [AccessedThroughProperty("GroupBox2")]
        private GroupBox _GroupBox2;
        [AccessedThroughProperty("GroupBox3")]
        private GroupBox _GroupBox3;
        [AccessedThroughProperty("GroupBox4")]
        private GroupBox _GroupBox4;
        [AccessedThroughProperty("GroupBox5")]
        private GroupBox _GroupBox5;
        [AccessedThroughProperty("GroupBox6")]
        private GroupBox _GroupBox6;
        [AccessedThroughProperty("GroupBox7")]
        private GroupBox _GroupBox7;
        [AccessedThroughProperty("GroupBox8")]
        private GroupBox _GroupBox8;
        [AccessedThroughProperty("Label1")]
        private Label _Label1;
        [AccessedThroughProperty("Label10")]
        private Label _Label10;
        [AccessedThroughProperty("Label11")]
        private Label _Label11;
        [AccessedThroughProperty("Label12")]
        private Label _Label12;
        [AccessedThroughProperty("Label13")]
        private Label _Label13;
        [AccessedThroughProperty("Label2")]
        private Label _Label2;
        [AccessedThroughProperty("Label3")]
        private Label _Label3;
        [AccessedThroughProperty("Label4")]
        private Label _Label4;
        [AccessedThroughProperty("Label5")]
        private Label _Label5;
        [AccessedThroughProperty("Label6")]
        private Label _Label6;
        [AccessedThroughProperty("Label7")]
        private Label _Label7;
        [AccessedThroughProperty("Label8")]
        private Label _Label8;
        [AccessedThroughProperty("Label9")]
        private Label _Label9;
        [AccessedThroughProperty("lblFont")]
        private Label _lblFont;
        [AccessedThroughProperty("lstKeyboardControls")]
        private ListBox _lstKeyboardControls;
        [AccessedThroughProperty("pnlMinimapCliffColour")]
        private Panel _pnlMinimapCliffColour;
        [AccessedThroughProperty("pnlMinimapSelectedObjectColour")]
        private Panel _pnlMinimapSelectedObjectColour;
        [AccessedThroughProperty("TabControl1")]
        private TabControl _TabControl1;
        [AccessedThroughProperty("TableLayoutPanel1")]
        private TableLayoutPanel _TableLayoutPanel1;
        [AccessedThroughProperty("TabPage1")]
        private TabPage _TabPage1;
        [AccessedThroughProperty("TabPage2")]
        private TabPage _TabPage2;
        [AccessedThroughProperty("TabPage3")]
        private TabPage _TabPage3;
        [AccessedThroughProperty("txtAutosaveChanges")]
        private TextBox _txtAutosaveChanges;
        [AccessedThroughProperty("txtAutosaveInterval")]
        private TextBox _txtAutosaveInterval;
        [AccessedThroughProperty("txtFOV")]
        private TextBox _txtFOV;
        [AccessedThroughProperty("txtMapBPP")]
        private TextBox _txtMapBPP;
        [AccessedThroughProperty("txtMapDepth")]
        private TextBox _txtMapDepth;
        [AccessedThroughProperty("txtMinimapSize")]
        private TextBox _txtMinimapSize;
        [AccessedThroughProperty("txtTexturesBPP")]
        private TextBox _txtTexturesBPP;
        [AccessedThroughProperty("txtTexturesDepth")]
        private TextBox _txtTexturesDepth;
        [AccessedThroughProperty("txtUndoSteps")]
        private TextBox _txtUndoSteps;
        private bool AllowClose;
        private modControls.clsKeyboardProfile ChangedKeyControls;
        private ctrlColour clrMinimapCliffs;
        private ctrlColour clrMinimapSelectedObjects;
        private IContainer components;
        private Font DisplayFont;
        private clsOption<clsKeyboardControl>[] lstKeyboardControls_Items;
        private clsRGBA_sng MinimapCliffColour;
        private clsRGBA_sng MinimapSelectedObjectColour;
        private ctrlPathSet ObjectDataPathSet;
        private ctrlPathSet TilesetsPathSet;

        public frmOptions()
        {
            base.FormClosing += new FormClosingEventHandler(this.frmOptions_FormClosing);
            this.ObjectDataPathSet = new ctrlPathSet("Object Data Directories");
            this.TilesetsPathSet = new ctrlPathSet("Tilesets Directories");
            this.lstKeyboardControls_Items = new clsOption<clsKeyboardControl>[0];
            this.AllowClose = false;
            this.InitializeComponent();
            this.Icon = modProgram.ProgramIcon;
            this.TilesetsPathSet.Dock = DockStyle.Fill;
            this.ObjectDataPathSet.Dock = DockStyle.Fill;
            this.TableLayoutPanel1.Controls.Add(this.TilesetsPathSet, 0, 0);
            this.TableLayoutPanel1.Controls.Add(this.ObjectDataPathSet, 0, 1);
            this.ChangedKeyControls = (modControls.clsKeyboardProfile) modControls.KeyboardProfile.GetCopy(new modControls.clsKeyboardProfileCreator());
            this.txtAutosaveChanges.Text = modIO.InvariantToString_uint(modSettings.Settings.AutoSaveMinChanges);
            this.txtAutosaveInterval.Text = modIO.InvariantToString_uint(modSettings.Settings.AutoSaveMinInterval_s);
            this.cbxAutosaveCompression.Checked = modSettings.Settings.AutoSaveCompress;
            this.cbxAutosaveEnabled.Checked = modSettings.Settings.AutoSaveEnabled;
            this.cbxAskDirectories.Checked = modSettings.Settings.DirectoriesPrompt;
            this.cbxPointerDirect.Checked = modSettings.Settings.DirectPointer;
            this.DisplayFont = modSettings.Settings.MakeFont();
            this.UpdateDisplayFontLabel();
            this.txtFOV.Text = modIO.InvariantToString_dbl(modSettings.Settings.FOVDefault);
            this.MinimapCliffColour = new clsRGBA_sng(modSettings.Settings.MinimapCliffColour);
            this.clrMinimapCliffs = new ctrlColour(this.MinimapCliffColour);
            this.pnlMinimapCliffColour.Controls.Add(this.clrMinimapCliffs);
            this.MinimapSelectedObjectColour = new clsRGBA_sng(modSettings.Settings.MinimapSelectedObjectsColour);
            this.clrMinimapSelectedObjects = new ctrlColour(this.MinimapSelectedObjectColour);
            this.pnlMinimapSelectedObjectColour.Controls.Add(this.clrMinimapSelectedObjects);
            this.txtMinimapSize.Text = modIO.InvariantToString_int(modSettings.Settings.MinimapSize);
            this.cbxMinimapObjectColours.Checked = modSettings.Settings.MinimapTeamColours;
            this.cbxMinimapTeamColourFeatures.Checked = modSettings.Settings.MinimapTeamColoursExceptFeatures;
            this.cbxMipmaps.Checked = modSettings.Settings.Mipmaps;
            this.cbxMipmapsHardware.Checked = modSettings.Settings.MipmapsHardware;
            this.txtUndoSteps.Text = modIO.InvariantToString_uint(modSettings.Settings.UndoLimit);
            this.TilesetsPathSet.SetPaths(modSettings.Settings.TilesetDirectories);
            this.ObjectDataPathSet.SetPaths(modSettings.Settings.ObjectDataDirectories);
            this.TilesetsPathSet.SelectedNum = modMath.Clamp_int(Conversions.ToInteger(modSettings.Settings.get_Value(modSettings.Setting_DefaultTilesetsPathNum)), -1, modSettings.Settings.TilesetDirectories.Count - 1);
            this.ObjectDataPathSet.SelectedNum = modMath.Clamp_int(Conversions.ToInteger(modSettings.Settings.get_Value(modSettings.Setting_DefaultObjectDataPathNum)), -1, modSettings.Settings.ObjectDataDirectories.Count - 1);
            this.txtMapBPP.Text = modIO.InvariantToString_int(modSettings.Settings.MapViewBPP);
            this.txtMapDepth.Text = modIO.InvariantToString_int(modSettings.Settings.MapViewDepth);
            this.txtTexturesBPP.Text = modIO.InvariantToString_int(modSettings.Settings.TextureViewBPP);
            this.txtTexturesDepth.Text = modIO.InvariantToString_int(modSettings.Settings.TextureViewDepth);
            this.cbxPickerOrientation.Checked = modSettings.Settings.PickOrientation;
            this.UpdateKeyboardControls(-1);
        }

        private void btnAutosaveOpen_Click(object sender, EventArgs e)
        {
            modMain.frmMainInstance.Load_Autosave_Prompt();
        }

        private void btnCancel_Click(object sender, EventArgs e)
        {
            this.Finish(DialogResult.Cancel);
        }

        private void btnFont_Click(object sender, EventArgs e)
        {
            DialogResult cancel;
            FontDialog dialog = new FontDialog();
            try
            {
                dialog.Font = this.DisplayFont;
                dialog.FontMustExist = true;
                cancel = dialog.ShowDialog(this);
            }
            catch (Exception exception1)
            {
                ProjectData.SetProjectError(exception1);
                cancel = DialogResult.Cancel;
                ProjectData.ClearProjectError();
            }
            if (cancel == DialogResult.OK)
            {
                this.DisplayFont = dialog.Font;
                this.UpdateDisplayFontLabel();
            }
        }

        private void btnKeyControlChange_Click(object sender, EventArgs e)
        {
            if (this.lstKeyboardControls.SelectedIndex >= 0)
            {
                frmKeyboardControl control = new frmKeyboardControl();
                if ((control.ShowDialog() == DialogResult.OK) && (control.Results.Count != 0))
                {
                    clsOption<clsKeyboardControl> optionItem = this.lstKeyboardControls_Items[this.lstKeyboardControls.SelectedIndex];
                    clsKeyboardControl control3 = (clsKeyboardControl) this.ChangedKeyControls.get_Value(optionItem);
                    Keys[] keys = new Keys[(control.Results.Count - 1) + 1];
                    int num2 = control.Results.Count - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        keys[i] = control.Results[i].Item;
                    }
                    clsKeyboardControl control2 = new clsKeyboardControl(keys, control3.UnlessKeys);
                    this.ChangedKeyControls.set_Changes(optionItem, new clsOptionProfile.clsChange<clsKeyboardControl>(control2));
                    this.UpdateKeyboardControl(optionItem.GroupLink.ArrayPosition);
                }
            }
        }

        private void btnKeyControlChangeDefault_Click(object sender, EventArgs e)
        {
            if (this.lstKeyboardControls.SelectedIndex >= 0)
            {
                clsOption<clsKeyboardControl> optionItem = this.lstKeyboardControls_Items[this.lstKeyboardControls.SelectedIndex];
                this.ChangedKeyControls.set_Changes(optionItem, null);
                this.UpdateKeyboardControl(optionItem.GroupLink.ArrayPosition);
            }
        }

        private void btnKeyControlChangeUnless_Click(object sender, EventArgs e)
        {
            if (this.lstKeyboardControls.SelectedIndex >= 0)
            {
                frmKeyboardControl control = new frmKeyboardControl();
                if ((control.ShowDialog() == DialogResult.OK) && (control.Results.Count != 0))
                {
                    clsOption<clsKeyboardControl> optionItem = this.lstKeyboardControls_Items[this.lstKeyboardControls.SelectedIndex];
                    clsKeyboardControl control3 = (clsKeyboardControl) this.ChangedKeyControls.get_Value(optionItem);
                    Keys[] unlessKeys = new Keys[(control.Results.Count - 1) + 1];
                    int num2 = control.Results.Count - 1;
                    for (int i = 0; i <= num2; i++)
                    {
                        unlessKeys[i] = control.Results[i].Item;
                    }
                    clsKeyboardControl control2 = new clsKeyboardControl(control3.Keys, unlessKeys);
                    this.ChangedKeyControls.set_Changes(optionItem, new clsOptionProfile.clsChange<clsKeyboardControl>(control2));
                    this.UpdateKeyboardControl(optionItem.GroupLink.ArrayPosition);
                }
            }
        }

        private void btnSave_Click(object sender, EventArgs e)
        {
            double num;
            int num2;
            modSettings.clsSettings copy = (modSettings.clsSettings) modSettings.Settings.GetCopy(new modSettings.clsSettingsCreator());
            if (modIO.InvariantParse_dbl(this.txtAutosaveChanges.Text, ref num))
            {
                copy.set_Changes(modSettings.Setting_AutoSaveMinChanges, new clsOptionProfile.clsChange<uint>((uint) Math.Round(modMath.Clamp_dbl(num, 1.0, 4294967294))));
            }
            if (modIO.InvariantParse_dbl(this.txtAutosaveInterval.Text, ref num))
            {
                copy.set_Changes(modSettings.Setting_AutoSaveMinInterval_s, new clsOptionProfile.clsChange<uint>((uint) Math.Round(modMath.Clamp_dbl(num, 1.0, 4294967294))));
            }
            copy.set_Changes(modSettings.Setting_AutoSaveCompress, new clsOptionProfile.clsChange<bool>(this.cbxAutosaveCompression.Checked));
            copy.set_Changes(modSettings.Setting_AutoSaveEnabled, new clsOptionProfile.clsChange<bool>(this.cbxAutosaveEnabled.Checked));
            copy.set_Changes(modSettings.Setting_DirectoriesPrompt, new clsOptionProfile.clsChange<bool>(this.cbxAskDirectories.Checked));
            copy.set_Changes(modSettings.Setting_DirectPointer, new clsOptionProfile.clsChange<bool>(this.cbxPointerDirect.Checked));
            copy.set_Changes(modSettings.Setting_FontFamily, new clsOptionProfile.clsChange<FontFamily>(this.DisplayFont.FontFamily));
            if (modIO.InvariantParse_dbl(this.txtFOV.Text, ref num))
            {
                copy.set_Changes(modSettings.Setting_FOVDefault, new clsOptionProfile.clsChange<double>(num));
            }
            copy.set_Changes(modSettings.Setting_MinimapCliffColour, new clsOptionProfile.clsChange<clsRGBA_sng>(this.MinimapCliffColour));
            copy.set_Changes(modSettings.Setting_MinimapSelectedObjectsColour, new clsOptionProfile.clsChange<clsRGBA_sng>(this.MinimapSelectedObjectColour));
            if (modIO.InvariantParse_int(this.txtMinimapSize.Text, ref num2))
            {
                copy.set_Changes(modSettings.Setting_MinimapSize, new clsOptionProfile.clsChange<int>(num2));
            }
            copy.set_Changes(modSettings.Setting_MinimapTeamColours, new clsOptionProfile.clsChange<bool>(this.cbxMinimapObjectColours.Checked));
            copy.set_Changes(modSettings.Setting_MinimapTeamColoursExceptFeatures, new clsOptionProfile.clsChange<bool>(this.cbxMinimapTeamColourFeatures.Checked));
            copy.set_Changes(modSettings.Setting_Mipmaps, new clsOptionProfile.clsChange<bool>(this.cbxMipmaps.Checked));
            copy.set_Changes(modSettings.Setting_MipmapsHardware, new clsOptionProfile.clsChange<bool>(this.cbxMipmapsHardware.Checked));
            if (modIO.InvariantParse_int(this.txtUndoSteps.Text, ref num2))
            {
                copy.set_Changes(modSettings.Setting_UndoLimit, new clsOptionProfile.clsChange<int>(num2));
            }
            modLists.SimpleList<string> list2 = new modLists.SimpleList<string>();
            modLists.SimpleList<string> list = new modLists.SimpleList<string>();
            string[] getPaths = this.TilesetsPathSet.GetPaths;
            string[] strArray = this.ObjectDataPathSet.GetPaths;
            int upperBound = getPaths.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                list2.Add(getPaths[i]);
            }
            int num6 = strArray.GetUpperBound(0);
            for (int j = 0; j <= num6; j++)
            {
                list.Add(strArray[j]);
            }
            copy.set_Changes(modSettings.Setting_TilesetDirectories, new clsOptionProfile.clsChange<modLists.SimpleList<string>>(list2));
            copy.set_Changes(modSettings.Setting_ObjectDataDirectories, new clsOptionProfile.clsChange<modLists.SimpleList<string>>(list));
            copy.set_Changes(modSettings.Setting_DefaultTilesetsPathNum, new clsOptionProfile.clsChange<int>(this.TilesetsPathSet.SelectedNum));
            copy.set_Changes(modSettings.Setting_DefaultObjectDataPathNum, new clsOptionProfile.clsChange<int>(this.ObjectDataPathSet.SelectedNum));
            if (modIO.InvariantParse_int(this.txtMapBPP.Text, ref num2))
            {
                copy.set_Changes(modSettings.Setting_MapViewBPP, new clsOptionProfile.clsChange<int>(num2));
            }
            if (modIO.InvariantParse_int(this.txtMapDepth.Text, ref num2))
            {
                copy.set_Changes(modSettings.Setting_MapViewDepth, new clsOptionProfile.clsChange<int>(num2));
            }
            if (modIO.InvariantParse_int(this.txtTexturesBPP.Text, ref num2))
            {
                copy.set_Changes(modSettings.Setting_TextureViewBPP, new clsOptionProfile.clsChange<int>(num2));
            }
            if (modIO.InvariantParse_int(this.txtTexturesDepth.Text, ref num2))
            {
                copy.set_Changes(modSettings.Setting_TextureViewDepth, new clsOptionProfile.clsChange<int>(num2));
            }
            copy.set_Changes(modSettings.Setting_PickOrientation, new clsOptionProfile.clsChange<bool>(this.cbxPickerOrientation.Checked));
            modSettings.UpdateSettings(copy);
            clsMap mainMap = modMain.frmMainInstance.MainMap;
            if (mainMap != null)
            {
                mainMap.MinimapMakeLater();
            }
            modMain.frmMainInstance.View_DrawViewLater();
            modControls.KeyboardProfile = this.ChangedKeyControls;
            this.Finish(DialogResult.OK);
        }

        [DebuggerNonUserCode]
        protected override void Dispose(bool disposing)
        {
            try
            {
                if (disposing && (this.components != null))
                {
                    this.components.Dispose();
                }
            }
            finally
            {
                base.Dispose(disposing);
            }
        }

        private void Finish(DialogResult result)
        {
            this.AllowClose = true;
            modMain.frmOptionsInstance = null;
            if (this.Modal)
            {
                this.DialogResult = result;
                this.Close();
            }
            else
            {
                this.Close();
            }
        }

        private void frmOptions_FormClosing(object sender, FormClosingEventArgs e)
        {
            e.Cancel = !this.AllowClose;
        }

        private string GetKeyControlText(clsOption<clsKeyboardControl> item)
        {
            string str2 = item.SaveKey + " = ";
            clsKeyboardControl control = (clsKeyboardControl) this.ChangedKeyControls.get_Value(item);
            int upperBound = control.Keys.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                Keys keys = control.Keys[i];
                string name = Enum.GetName(typeof(Keys), keys);
                if (name == null)
                {
                    str2 = str2 + modIO.InvariantToString_int((int) keys);
                }
                else
                {
                    str2 = str2 + name;
                }
                if (i < control.Keys.GetUpperBound(0))
                {
                    str2 = str2 + " + ";
                }
            }
            if (control.UnlessKeys.GetUpperBound(0) >= 0)
            {
                str2 = str2 + " unless ";
                int num4 = control.UnlessKeys.GetUpperBound(0);
                for (int j = 0; j <= num4; j++)
                {
                    Keys keys2 = control.UnlessKeys[j];
                    string str4 = Enum.GetName(typeof(Keys), keys2);
                    if (str4 == null)
                    {
                        str2 = str2 + modIO.InvariantToString_int((int) keys2);
                    }
                    else
                    {
                        str2 = str2 + str4;
                    }
                    if (j < control.UnlessKeys.GetUpperBound(0))
                    {
                        str2 = str2 + ", ";
                    }
                }
            }
            if (control != item.DefaultValue)
            {
                str2 = str2 + " (modified)";
            }
            return str2;
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.TabControl1 = new TabControl();
            this.TabPage3 = new TabPage();
            this.Label12 = new Label();
            this.cbxAskDirectories = new CheckBox();
            this.TableLayoutPanel1 = new TableLayoutPanel();
            this.TabPage1 = new TabPage();
            this.GroupBox3 = new GroupBox();
            this.cbxPickerOrientation = new CheckBox();
            this.GroupBox8 = new GroupBox();
            this.Label13 = new Label();
            this.txtTexturesDepth = new TextBox();
            this.txtTexturesBPP = new TextBox();
            this.Label10 = new Label();
            this.txtMapDepth = new TextBox();
            this.txtMapBPP = new TextBox();
            this.Label8 = new Label();
            this.Label9 = new Label();
            this.cbxMipmapsHardware = new CheckBox();
            this.cbxMipmaps = new CheckBox();
            this.GroupBox7 = new GroupBox();
            this.txtFOV = new TextBox();
            this.Label4 = new Label();
            this.GroupBox6 = new GroupBox();
            this.cbxPointerDirect = new CheckBox();
            this.GroupBox5 = new GroupBox();
            this.Label6 = new Label();
            this.pnlMinimapSelectedObjectColour = new Panel();
            this.Label5 = new Label();
            this.pnlMinimapCliffColour = new Panel();
            this.txtMinimapSize = new TextBox();
            this.cbxMinimapTeamColourFeatures = new CheckBox();
            this.cbxMinimapObjectColours = new CheckBox();
            this.Label3 = new Label();
            this.GroupBox4 = new GroupBox();
            this.lblFont = new Label();
            this.btnFont = new Button();
            this.GroupBox2 = new GroupBox();
            this.txtAutosaveInterval = new TextBox();
            this.txtAutosaveChanges = new TextBox();
            this.btnAutosaveOpen = new Button();
            this.cbxAutosaveCompression = new CheckBox();
            this.Label2 = new Label();
            this.cbxAutosaveEnabled = new CheckBox();
            this.Label1 = new Label();
            this.GroupBox1 = new GroupBox();
            this.txtUndoSteps = new TextBox();
            this.Label11 = new Label();
            this.TabPage2 = new TabPage();
            this.btnKeyControlChangeDefault = new Button();
            this.Label7 = new Label();
            this.btnKeyControlChangeUnless = new Button();
            this.btnKeyControlChange = new Button();
            this.lstKeyboardControls = new ListBox();
            this.btnCancel = new Button();
            this.btnSave = new Button();
            this.TabControl1.SuspendLayout();
            this.TabPage3.SuspendLayout();
            this.TabPage1.SuspendLayout();
            this.GroupBox3.SuspendLayout();
            this.GroupBox8.SuspendLayout();
            this.GroupBox7.SuspendLayout();
            this.GroupBox6.SuspendLayout();
            this.GroupBox5.SuspendLayout();
            this.GroupBox4.SuspendLayout();
            this.GroupBox2.SuspendLayout();
            this.GroupBox1.SuspendLayout();
            this.TabPage2.SuspendLayout();
            this.SuspendLayout();
            this.TabControl1.Controls.Add(this.TabPage3);
            this.TabControl1.Controls.Add(this.TabPage1);
            this.TabControl1.Controls.Add(this.TabPage2);
            Point point2 = new Point(12, 12);
            this.TabControl1.Location = point2;
            this.TabControl1.Name = "TabControl1";
            this.TabControl1.SelectedIndex = 0;
            Size size2 = new Size(0x27d, 0x18e);
            this.TabControl1.Size = size2;
            this.TabControl1.TabIndex = 0x23;
            this.TabPage3.Controls.Add(this.Label12);
            this.TabPage3.Controls.Add(this.cbxAskDirectories);
            this.TabPage3.Controls.Add(this.TableLayoutPanel1);
            point2 = new Point(4, 0x19);
            this.TabPage3.Location = point2;
            Padding padding2 = new Padding(0);
            this.TabPage3.Margin = padding2;
            this.TabPage3.Name = "TabPage3";
            size2 = new Size(0x275, 0x171);
            this.TabPage3.Size = size2;
            this.TabPage3.TabIndex = 2;
            this.TabPage3.Text = "Directories";
            this.TabPage3.UseVisualStyleBackColor = true;
            this.Label12.AutoSize = true;
            point2 = new Point(0x12e, 15);
            this.Label12.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label12.Margin = padding2;
            this.Label12.Name = "Label12";
            size2 = new Size(0xf5, 20);
            this.Label12.Size = size2;
            this.Label12.TabIndex = 0x2a;
            this.Label12.Text = "Options on this tab take effect on restart.";
            this.Label12.UseCompatibleTextRendering = true;
            this.cbxAskDirectories.AutoSize = true;
            point2 = new Point(0x18, 14);
            this.cbxAskDirectories.Location = point2;
            padding2 = new Padding(4);
            this.cbxAskDirectories.Margin = padding2;
            this.cbxAskDirectories.Name = "cbxAskDirectories";
            size2 = new Size(0xe1, 0x15);
            this.cbxAskDirectories.Size = size2;
            this.cbxAskDirectories.TabIndex = 0x27;
            this.cbxAskDirectories.Text = "Show options before loading data";
            this.cbxAskDirectories.UseCompatibleTextRendering = true;
            this.cbxAskDirectories.UseVisualStyleBackColor = true;
            this.TableLayoutPanel1.ColumnCount = 1;
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 20f));
            point2 = new Point(3, 0x2a);
            this.TableLayoutPanel1.Location = point2;
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 2;
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 50f));
            size2 = new Size(0x26f, 0x144);
            this.TableLayoutPanel1.Size = size2;
            this.TableLayoutPanel1.TabIndex = 0x29;
            this.TabPage1.Controls.Add(this.GroupBox3);
            this.TabPage1.Controls.Add(this.GroupBox8);
            this.TabPage1.Controls.Add(this.GroupBox7);
            this.TabPage1.Controls.Add(this.GroupBox6);
            this.TabPage1.Controls.Add(this.GroupBox5);
            this.TabPage1.Controls.Add(this.GroupBox4);
            this.TabPage1.Controls.Add(this.GroupBox2);
            this.TabPage1.Controls.Add(this.GroupBox1);
            point2 = new Point(4, 0x19);
            this.TabPage1.Location = point2;
            this.TabPage1.Name = "TabPage1";
            padding2 = new Padding(3);
            this.TabPage1.Padding = padding2;
            size2 = new Size(0x275, 0x171);
            this.TabPage1.Size = size2;
            this.TabPage1.TabIndex = 0;
            this.TabPage1.Text = "General";
            this.TabPage1.UseVisualStyleBackColor = true;
            this.GroupBox3.Controls.Add(this.cbxPickerOrientation);
            point2 = new Point(0x13c, 0x120);
            this.GroupBox3.Location = point2;
            this.GroupBox3.Name = "GroupBox3";
            size2 = new Size(0x130, 0x36);
            this.GroupBox3.Size = size2;
            this.GroupBox3.TabIndex = 0x2d;
            this.GroupBox3.TabStop = false;
            this.GroupBox3.Text = "Picker";
            this.GroupBox3.UseCompatibleTextRendering = true;
            this.cbxPickerOrientation.AutoSize = true;
            point2 = new Point(8, 0x16);
            this.cbxPickerOrientation.Location = point2;
            padding2 = new Padding(4);
            this.cbxPickerOrientation.Margin = padding2;
            this.cbxPickerOrientation.Name = "cbxPickerOrientation";
            size2 = new Size(0xc0, 0x15);
            this.cbxPickerOrientation.Size = size2;
            this.cbxPickerOrientation.TabIndex = 0x33;
            this.cbxPickerOrientation.Text = "Capture texture orientations";
            this.cbxPickerOrientation.UseCompatibleTextRendering = true;
            this.cbxPickerOrientation.UseVisualStyleBackColor = true;
            this.GroupBox8.Controls.Add(this.Label13);
            this.GroupBox8.Controls.Add(this.txtTexturesDepth);
            this.GroupBox8.Controls.Add(this.txtTexturesBPP);
            this.GroupBox8.Controls.Add(this.Label10);
            this.GroupBox8.Controls.Add(this.txtMapDepth);
            this.GroupBox8.Controls.Add(this.txtMapBPP);
            this.GroupBox8.Controls.Add(this.Label8);
            this.GroupBox8.Controls.Add(this.Label9);
            this.GroupBox8.Controls.Add(this.cbxMipmapsHardware);
            this.GroupBox8.Controls.Add(this.cbxMipmaps);
            point2 = new Point(6, 0xf4);
            this.GroupBox8.Location = point2;
            this.GroupBox8.Name = "GroupBox8";
            size2 = new Size(0x130, 0x77);
            this.GroupBox8.Size = size2;
            this.GroupBox8.TabIndex = 0x2d;
            this.GroupBox8.TabStop = false;
            this.GroupBox8.Text = "Graphics";
            this.GroupBox8.UseCompatibleTextRendering = true;
            this.Label13.AutoSize = true;
            point2 = new Point(7, 0x60);
            this.Label13.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label13.Margin = padding2;
            this.Label13.Name = "Label13";
            size2 = new Size(90, 20);
            this.Label13.Size = size2;
            this.Label13.TabIndex = 50;
            this.Label13.Text = "Textures View";
            this.Label13.UseCompatibleTextRendering = true;
            point2 = new Point(180, 0x5d);
            this.txtTexturesDepth.Location = point2;
            padding2 = new Padding(4);
            this.txtTexturesDepth.Margin = padding2;
            this.txtTexturesDepth.Name = "txtTexturesDepth";
            size2 = new Size(0x3d, 0x16);
            this.txtTexturesDepth.Size = size2;
            this.txtTexturesDepth.TabIndex = 0x31;
            point2 = new Point(0x69, 0x5d);
            this.txtTexturesBPP.Location = point2;
            padding2 = new Padding(4);
            this.txtTexturesBPP.Margin = padding2;
            this.txtTexturesBPP.Name = "txtTexturesBPP";
            size2 = new Size(0x3d, 0x16);
            this.txtTexturesBPP.Size = size2;
            this.txtTexturesBPP.TabIndex = 0x30;
            this.Label10.AutoSize = true;
            point2 = new Point(0x21, 0x43);
            this.Label10.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label10.Margin = padding2;
            this.Label10.Name = "Label10";
            size2 = new Size(0x40, 20);
            this.Label10.Size = size2;
            this.Label10.TabIndex = 0x2e;
            this.Label10.Text = "Map View";
            this.Label10.UseCompatibleTextRendering = true;
            point2 = new Point(180, 0x43);
            this.txtMapDepth.Location = point2;
            padding2 = new Padding(4);
            this.txtMapDepth.Margin = padding2;
            this.txtMapDepth.Name = "txtMapDepth";
            size2 = new Size(0x3d, 0x16);
            this.txtMapDepth.Size = size2;
            this.txtMapDepth.TabIndex = 0x2c;
            point2 = new Point(0x69, 0x43);
            this.txtMapBPP.Location = point2;
            padding2 = new Padding(4);
            this.txtMapBPP.Margin = padding2;
            this.txtMapBPP.Name = "txtMapBPP";
            size2 = new Size(0x3d, 0x16);
            this.txtMapBPP.Size = size2;
            this.txtMapBPP.TabIndex = 0x2a;
            this.Label8.AutoSize = true;
            point2 = new Point(0x60, 0x2f);
            this.Label8.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label8.Margin = padding2;
            this.Label8.Name = "Label8";
            size2 = new Size(70, 20);
            this.Label8.Size = size2;
            this.Label8.TabIndex = 0x2d;
            this.Label8.Text = "Colour Bits";
            this.Label8.UseCompatibleTextRendering = true;
            this.Label9.AutoSize = true;
            point2 = new Point(0xae, 0x2f);
            this.Label9.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label9.Margin = padding2;
            this.Label9.Name = "Label9";
            size2 = new Size(0x43, 20);
            this.Label9.Size = size2;
            this.Label9.TabIndex = 0x2b;
            this.Label9.Text = "Depth Bits";
            this.Label9.UseCompatibleTextRendering = true;
            this.cbxMipmapsHardware.AutoSize = true;
            point2 = new Point(0xa9, 0x16);
            this.cbxMipmapsHardware.Location = point2;
            padding2 = new Padding(4);
            this.cbxMipmapsHardware.Margin = padding2;
            this.cbxMipmapsHardware.Name = "cbxMipmapsHardware";
            size2 = new Size(0x70, 0x15);
            this.cbxMipmapsHardware.Size = size2;
            this.cbxMipmapsHardware.TabIndex = 0x29;
            this.cbxMipmapsHardware.Text = "Use Hardware";
            this.cbxMipmapsHardware.UseCompatibleTextRendering = true;
            this.cbxMipmapsHardware.UseVisualStyleBackColor = true;
            this.cbxMipmaps.AutoSize = true;
            point2 = new Point(8, 0x16);
            this.cbxMipmaps.Location = point2;
            padding2 = new Padding(4);
            this.cbxMipmaps.Margin = padding2;
            this.cbxMipmaps.Name = "cbxMipmaps";
            size2 = new Size(0x8d, 0x15);
            this.cbxMipmaps.Size = size2;
            this.cbxMipmaps.TabIndex = 40;
            this.cbxMipmaps.Text = "Generate mipmaps";
            this.cbxMipmaps.UseCompatibleTextRendering = true;
            this.cbxMipmaps.UseVisualStyleBackColor = true;
            this.GroupBox7.Controls.Add(this.txtFOV);
            this.GroupBox7.Controls.Add(this.Label4);
            point2 = new Point(0x13c, 0xe8);
            this.GroupBox7.Location = point2;
            this.GroupBox7.Name = "GroupBox7";
            size2 = new Size(0x130, 50);
            this.GroupBox7.Size = size2;
            this.GroupBox7.TabIndex = 0x2c;
            this.GroupBox7.TabStop = false;
            this.GroupBox7.Text = "Field Of View";
            this.GroupBox7.UseCompatibleTextRendering = true;
            point2 = new Point(0x9f, 15);
            this.txtFOV.Location = point2;
            padding2 = new Padding(4);
            this.txtFOV.Margin = padding2;
            this.txtFOV.Name = "txtFOV";
            size2 = new Size(0x8a, 0x16);
            this.txtFOV.Size = size2;
            this.txtFOV.TabIndex = 0x19;
            this.Label4.AutoSize = true;
            point2 = new Point(8, 0x12);
            this.Label4.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label4.Margin = padding2;
            this.Label4.Name = "Label4";
            size2 = new Size(0x69, 20);
            this.Label4.Size = size2;
            this.Label4.TabIndex = 0x1a;
            this.Label4.Text = "Default Multiplier";
            this.Label4.UseCompatibleTextRendering = true;
            this.GroupBox6.Controls.Add(this.cbxPointerDirect);
            point2 = new Point(0x13c, 0xb0);
            this.GroupBox6.Location = point2;
            this.GroupBox6.Name = "GroupBox6";
            size2 = new Size(0x130, 50);
            this.GroupBox6.Size = size2;
            this.GroupBox6.TabIndex = 0x2b;
            this.GroupBox6.TabStop = false;
            this.GroupBox6.Text = "Pointer";
            this.GroupBox6.UseCompatibleTextRendering = true;
            this.cbxPointerDirect.AutoSize = true;
            point2 = new Point(7, 0x16);
            this.cbxPointerDirect.Location = point2;
            padding2 = new Padding(4);
            this.cbxPointerDirect.Margin = padding2;
            this.cbxPointerDirect.Name = "cbxPointerDirect";
            size2 = new Size(0x3e, 0x15);
            this.cbxPointerDirect.Size = size2;
            this.cbxPointerDirect.TabIndex = 40;
            this.cbxPointerDirect.Text = "Direct";
            this.cbxPointerDirect.UseCompatibleTextRendering = true;
            this.cbxPointerDirect.UseVisualStyleBackColor = true;
            this.GroupBox5.Controls.Add(this.Label6);
            this.GroupBox5.Controls.Add(this.pnlMinimapSelectedObjectColour);
            this.GroupBox5.Controls.Add(this.Label5);
            this.GroupBox5.Controls.Add(this.pnlMinimapCliffColour);
            this.GroupBox5.Controls.Add(this.txtMinimapSize);
            this.GroupBox5.Controls.Add(this.cbxMinimapTeamColourFeatures);
            this.GroupBox5.Controls.Add(this.cbxMinimapObjectColours);
            this.GroupBox5.Controls.Add(this.Label3);
            point2 = new Point(0x13c, 7);
            this.GroupBox5.Location = point2;
            this.GroupBox5.Name = "GroupBox5";
            size2 = new Size(0x130, 0xa3);
            this.GroupBox5.Size = size2;
            this.GroupBox5.TabIndex = 0x2a;
            this.GroupBox5.TabStop = false;
            this.GroupBox5.Text = "Minimap";
            this.GroupBox5.UseCompatibleTextRendering = true;
            this.Label6.AutoSize = true;
            point2 = new Point(8, 100);
            this.Label6.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label6.Margin = padding2;
            this.Label6.Name = "Label6";
            size2 = new Size(0x63, 20);
            this.Label6.Size = size2;
            this.Label6.TabIndex = 0x2d;
            this.Label6.Text = "Object Highlight";
            this.Label6.UseCompatibleTextRendering = true;
            point2 = new Point(0x84, 100);
            this.pnlMinimapSelectedObjectColour.Location = point2;
            this.pnlMinimapSelectedObjectColour.Name = "pnlMinimapSelectedObjectColour";
            size2 = new Size(0xa4, 0x1d);
            this.pnlMinimapSelectedObjectColour.Size = size2;
            this.pnlMinimapSelectedObjectColour.TabIndex = 0x2c;
            this.Label5.AutoSize = true;
            point2 = new Point(8, 0x43);
            this.Label5.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label5.Margin = padding2;
            this.Label5.Name = "Label5";
            size2 = new Size(0x47, 20);
            this.Label5.Size = size2;
            this.Label5.TabIndex = 0x2b;
            this.Label5.Text = "Cliff Colour";
            this.Label5.UseCompatibleTextRendering = true;
            point2 = new Point(0x84, 0x43);
            this.pnlMinimapCliffColour.Location = point2;
            this.pnlMinimapCliffColour.Name = "pnlMinimapCliffColour";
            size2 = new Size(0xa4, 0x1d);
            this.pnlMinimapCliffColour.Size = size2;
            this.pnlMinimapCliffColour.TabIndex = 0x2a;
            point2 = new Point(0x9f, 15);
            this.txtMinimapSize.Location = point2;
            padding2 = new Padding(4);
            this.txtMinimapSize.Margin = padding2;
            this.txtMinimapSize.Name = "txtMinimapSize";
            size2 = new Size(0x3d, 0x16);
            this.txtMinimapSize.Size = size2;
            this.txtMinimapSize.TabIndex = 0x19;
            this.cbxMinimapTeamColourFeatures.AutoSize = true;
            point2 = new Point(0x93, 0x2a);
            this.cbxMinimapTeamColourFeatures.Location = point2;
            padding2 = new Padding(4);
            this.cbxMinimapTeamColourFeatures.Margin = padding2;
            this.cbxMinimapTeamColourFeatures.Name = "cbxMinimapTeamColourFeatures";
            size2 = new Size(0x8b, 0x15);
            this.cbxMinimapTeamColourFeatures.Size = size2;
            this.cbxMinimapTeamColourFeatures.TabIndex = 0x29;
            this.cbxMinimapTeamColourFeatures.Text = "Except for features";
            this.cbxMinimapTeamColourFeatures.UseCompatibleTextRendering = true;
            this.cbxMinimapTeamColourFeatures.UseVisualStyleBackColor = true;
            this.cbxMinimapObjectColours.AutoSize = true;
            point2 = new Point(8, 0x2a);
            this.cbxMinimapObjectColours.Location = point2;
            padding2 = new Padding(4);
            this.cbxMinimapObjectColours.Margin = padding2;
            this.cbxMinimapObjectColours.Name = "cbxMinimapObjectColours";
            size2 = new Size(0x83, 0x15);
            this.cbxMinimapObjectColours.Size = size2;
            this.cbxMinimapObjectColours.TabIndex = 40;
            this.cbxMinimapObjectColours.Text = "Use team colours";
            this.cbxMinimapObjectColours.UseCompatibleTextRendering = true;
            this.cbxMinimapObjectColours.UseVisualStyleBackColor = true;
            this.Label3.AutoSize = true;
            point2 = new Point(8, 0x12);
            this.Label3.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label3.Margin = padding2;
            this.Label3.Name = "Label3";
            size2 = new Size(0x1f, 20);
            this.Label3.Size = size2;
            this.Label3.TabIndex = 0x1a;
            this.Label3.Text = "Size";
            this.Label3.UseCompatibleTextRendering = true;
            this.GroupBox4.Controls.Add(this.lblFont);
            this.GroupBox4.Controls.Add(this.btnFont);
            point2 = new Point(6, 0xb0);
            this.GroupBox4.Location = point2;
            this.GroupBox4.Name = "GroupBox4";
            size2 = new Size(0x130, 0x3e);
            this.GroupBox4.Size = size2;
            this.GroupBox4.TabIndex = 0x29;
            this.GroupBox4.TabStop = false;
            this.GroupBox4.Text = "Display Font";
            this.GroupBox4.UseCompatibleTextRendering = true;
            point2 = new Point(8, 0x1b);
            this.lblFont.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.lblFont.Margin = padding2;
            this.lblFont.Name = "lblFont";
            size2 = new Size(0xb6, 0x1d);
            this.lblFont.Size = size2;
            this.lblFont.TabIndex = 0x27;
            this.lblFont.Text = "Current font";
            this.lblFont.UseCompatibleTextRendering = true;
            point2 = new Point(0xd0, 0x15);
            this.btnFont.Location = point2;
            this.btnFont.Name = "btnFont";
            size2 = new Size(0x59, 0x1d);
            this.btnFont.Size = size2;
            this.btnFont.TabIndex = 0x26;
            this.btnFont.Text = "Select";
            this.btnFont.UseCompatibleTextRendering = true;
            this.btnFont.UseVisualStyleBackColor = true;
            this.GroupBox2.Controls.Add(this.txtAutosaveInterval);
            this.GroupBox2.Controls.Add(this.txtAutosaveChanges);
            this.GroupBox2.Controls.Add(this.btnAutosaveOpen);
            this.GroupBox2.Controls.Add(this.cbxAutosaveCompression);
            this.GroupBox2.Controls.Add(this.Label2);
            this.GroupBox2.Controls.Add(this.cbxAutosaveEnabled);
            this.GroupBox2.Controls.Add(this.Label1);
            point2 = new Point(6, 0x3f);
            this.GroupBox2.Location = point2;
            this.GroupBox2.Name = "GroupBox2";
            size2 = new Size(0x130, 0x6b);
            this.GroupBox2.Size = size2;
            this.GroupBox2.TabIndex = 0x25;
            this.GroupBox2.TabStop = false;
            this.GroupBox2.Text = "Autosave";
            this.GroupBox2.UseCompatibleTextRendering = true;
            point2 = new Point(140, 0x4a);
            this.txtAutosaveInterval.Location = point2;
            padding2 = new Padding(4);
            this.txtAutosaveInterval.Margin = padding2;
            this.txtAutosaveInterval.Name = "txtAutosaveInterval";
            size2 = new Size(0x3d, 0x16);
            this.txtAutosaveInterval.Size = size2;
            this.txtAutosaveInterval.TabIndex = 0x19;
            point2 = new Point(140, 0x33);
            this.txtAutosaveChanges.Location = point2;
            padding2 = new Padding(4);
            this.txtAutosaveChanges.Margin = padding2;
            this.txtAutosaveChanges.Name = "txtAutosaveChanges";
            size2 = new Size(0x3d, 0x16);
            this.txtAutosaveChanges.Size = size2;
            this.txtAutosaveChanges.TabIndex = 0x16;
            this.btnAutosaveOpen.DialogResult = DialogResult.Cancel;
            point2 = new Point(0xd1, 0x47);
            this.btnAutosaveOpen.Location = point2;
            this.btnAutosaveOpen.Name = "btnAutosaveOpen";
            size2 = new Size(0x59, 0x1d);
            this.btnAutosaveOpen.Size = size2;
            this.btnAutosaveOpen.TabIndex = 0x27;
            this.btnAutosaveOpen.Text = "Open Map";
            this.btnAutosaveOpen.UseCompatibleTextRendering = true;
            this.btnAutosaveOpen.UseVisualStyleBackColor = true;
            this.cbxAutosaveCompression.AutoSize = true;
            point2 = new Point(140, 0x13);
            this.cbxAutosaveCompression.Location = point2;
            padding2 = new Padding(4);
            this.cbxAutosaveCompression.Margin = padding2;
            this.cbxAutosaveCompression.Name = "cbxAutosaveCompression";
            size2 = new Size(130, 0x15);
            this.cbxAutosaveCompression.Size = size2;
            this.cbxAutosaveCompression.TabIndex = 0x1b;
            this.cbxAutosaveCompression.Text = "Use compression";
            this.cbxAutosaveCompression.UseCompatibleTextRendering = true;
            this.cbxAutosaveCompression.UseVisualStyleBackColor = true;
            this.Label2.AutoSize = true;
            point2 = new Point(7, 0x36);
            this.Label2.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label2.Margin = padding2;
            this.Label2.Name = "Label2";
            size2 = new Size(0x7d, 20);
            this.Label2.Size = size2;
            this.Label2.TabIndex = 0x1a;
            this.Label2.Text = "Number of changes:";
            this.Label2.UseCompatibleTextRendering = true;
            this.cbxAutosaveEnabled.AutoSize = true;
            point2 = new Point(7, 0x16);
            this.cbxAutosaveEnabled.Location = point2;
            padding2 = new Padding(4);
            this.cbxAutosaveEnabled.Margin = padding2;
            this.cbxAutosaveEnabled.Name = "cbxAutosaveEnabled";
            size2 = new Size(0x4c, 0x15);
            this.cbxAutosaveEnabled.Size = size2;
            this.cbxAutosaveEnabled.TabIndex = 3;
            this.cbxAutosaveEnabled.Text = "Enabled";
            this.cbxAutosaveEnabled.UseCompatibleTextRendering = true;
            this.cbxAutosaveEnabled.UseVisualStyleBackColor = true;
            this.Label1.AutoSize = true;
            point2 = new Point(7, 0x4a);
            this.Label1.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label1.Margin = padding2;
            this.Label1.Name = "Label1";
            size2 = new Size(0x68, 20);
            this.Label1.Size = size2;
            this.Label1.TabIndex = 0x18;
            this.Label1.Text = "Time interval (s):";
            this.Label1.UseCompatibleTextRendering = true;
            this.GroupBox1.Controls.Add(this.txtUndoSteps);
            this.GroupBox1.Controls.Add(this.Label11);
            point2 = new Point(6, 6);
            this.GroupBox1.Location = point2;
            this.GroupBox1.Name = "GroupBox1";
            size2 = new Size(0x130, 0x33);
            this.GroupBox1.Size = size2;
            this.GroupBox1.TabIndex = 0x24;
            this.GroupBox1.TabStop = false;
            this.GroupBox1.Text = "Undo";
            this.GroupBox1.UseCompatibleTextRendering = true;
            point2 = new Point(0x9e, 15);
            this.txtUndoSteps.Location = point2;
            padding2 = new Padding(4);
            this.txtUndoSteps.Margin = padding2;
            this.txtUndoSteps.Name = "txtUndoSteps";
            size2 = new Size(0x3d, 0x16);
            this.txtUndoSteps.Size = size2;
            this.txtUndoSteps.TabIndex = 0x16;
            this.Label11.AutoSize = true;
            point2 = new Point(7, 0x12);
            this.Label11.Location = point2;
            padding2 = new Padding(4, 0, 4, 0);
            this.Label11.Margin = padding2;
            this.Label11.Name = "Label11";
            size2 = new Size(0x8f, 20);
            this.Label11.Size = size2;
            this.Label11.TabIndex = 0x18;
            this.Label11.Text = "Maximum stored steps:";
            this.Label11.UseCompatibleTextRendering = true;
            this.TabPage2.Controls.Add(this.btnKeyControlChangeDefault);
            this.TabPage2.Controls.Add(this.Label7);
            this.TabPage2.Controls.Add(this.btnKeyControlChangeUnless);
            this.TabPage2.Controls.Add(this.btnKeyControlChange);
            this.TabPage2.Controls.Add(this.lstKeyboardControls);
            point2 = new Point(4, 0x19);
            this.TabPage2.Location = point2;
            this.TabPage2.Name = "TabPage2";
            padding2 = new Padding(3);
            this.TabPage2.Padding = padding2;
            size2 = new Size(0x275, 0x171);
            this.TabPage2.Size = size2;
            this.TabPage2.TabIndex = 1;
            this.TabPage2.Text = "Keyboard";
            this.TabPage2.UseVisualStyleBackColor = true;
            point2 = new Point(0x1a1, 0x9c);
            this.btnKeyControlChangeDefault.Location = point2;
            this.btnKeyControlChangeDefault.Name = "btnKeyControlChangeDefault";
            size2 = new Size(160, 0x23);
            this.btnKeyControlChangeDefault.Size = size2;
            this.btnKeyControlChangeDefault.TabIndex = 4;
            this.btnKeyControlChangeDefault.Text = "Set To Default";
            this.btnKeyControlChangeDefault.UseCompatibleTextRendering = true;
            this.btnKeyControlChangeDefault.UseVisualStyleBackColor = true;
            point2 = new Point(0x1a3, 0x62);
            this.Label7.Location = point2;
            this.Label7.Name = "Label7";
            size2 = new Size(0x9e, 0x42);
            this.Label7.Size = size2;
            this.Label7.TabIndex = 3;
            this.Label7.Text = "The key combination will be ignored while an \"unless key\" is pressed.";
            this.Label7.UseCompatibleTextRendering = true;
            point2 = new Point(0x1a1, 0x3b);
            this.btnKeyControlChangeUnless.Location = point2;
            this.btnKeyControlChangeUnless.Name = "btnKeyControlChangeUnless";
            size2 = new Size(160, 0x23);
            this.btnKeyControlChangeUnless.Size = size2;
            this.btnKeyControlChangeUnless.TabIndex = 2;
            this.btnKeyControlChangeUnless.Text = "Change Unless Keys";
            this.btnKeyControlChangeUnless.UseCompatibleTextRendering = true;
            this.btnKeyControlChangeUnless.UseVisualStyleBackColor = true;
            point2 = new Point(0x1a1, 0x12);
            this.btnKeyControlChange.Location = point2;
            this.btnKeyControlChange.Name = "btnKeyControlChange";
            size2 = new Size(160, 0x23);
            this.btnKeyControlChange.Size = size2;
            this.btnKeyControlChange.TabIndex = 1;
            this.btnKeyControlChange.Text = "Change Keys";
            this.btnKeyControlChange.UseCompatibleTextRendering = true;
            this.btnKeyControlChange.UseVisualStyleBackColor = true;
            this.lstKeyboardControls.FormattingEnabled = true;
            this.lstKeyboardControls.ItemHeight = 0x10;
            point2 = new Point(0x11, 0x12);
            this.lstKeyboardControls.Location = point2;
            this.lstKeyboardControls.Name = "lstKeyboardControls";
            this.lstKeyboardControls.ScrollAlwaysVisible = true;
            size2 = new Size(0x18a, 0x144);
            this.lstKeyboardControls.Size = size2;
            this.lstKeyboardControls.TabIndex = 0;
            point2 = new Point(0x225, 0x1a0);
            this.btnCancel.Location = point2;
            this.btnCancel.Name = "btnCancel";
            size2 = new Size(100, 0x1d);
            this.btnCancel.Size = size2;
            this.btnCancel.TabIndex = 0x27;
            this.btnCancel.Text = "Cancel";
            this.btnCancel.UseCompatibleTextRendering = true;
            this.btnCancel.UseVisualStyleBackColor = true;
            point2 = new Point(0x1bb, 0x1a0);
            this.btnSave.Location = point2;
            this.btnSave.Name = "btnSave";
            size2 = new Size(100, 0x1d);
            this.btnSave.Size = size2;
            this.btnSave.TabIndex = 40;
            this.btnSave.Text = "Accept";
            this.btnSave.UseCompatibleTextRendering = true;
            this.btnSave.UseVisualStyleBackColor = true;
            this.AutoScaleMode = AutoScaleMode.None;
            size2 = new Size(0x293, 0x1c4);
            this.ClientSize = size2;
            this.Controls.Add(this.btnSave);
            this.Controls.Add(this.btnCancel);
            this.Controls.Add(this.TabControl1);
            this.FormBorderStyle = FormBorderStyle.FixedSingle;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "frmOptions";
            this.Text = "Options";
            this.TabControl1.ResumeLayout(false);
            this.TabPage3.ResumeLayout(false);
            this.TabPage3.PerformLayout();
            this.TabPage1.ResumeLayout(false);
            this.GroupBox3.ResumeLayout(false);
            this.GroupBox3.PerformLayout();
            this.GroupBox8.ResumeLayout(false);
            this.GroupBox8.PerformLayout();
            this.GroupBox7.ResumeLayout(false);
            this.GroupBox7.PerformLayout();
            this.GroupBox6.ResumeLayout(false);
            this.GroupBox6.PerformLayout();
            this.GroupBox5.ResumeLayout(false);
            this.GroupBox5.PerformLayout();
            this.GroupBox4.ResumeLayout(false);
            this.GroupBox2.ResumeLayout(false);
            this.GroupBox2.PerformLayout();
            this.GroupBox1.ResumeLayout(false);
            this.GroupBox1.PerformLayout();
            this.TabPage2.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        private void UpdateDisplayFontLabel()
        {
            Label lblFont;
            this.lblFont.Text = this.DisplayFont.FontFamily.Name + " " + Conversions.ToString(this.DisplayFont.SizeInPoints) + " ";
            if (this.DisplayFont.Bold)
            {
                lblFont = this.lblFont;
                lblFont.Text = lblFont.Text + "B";
            }
            if (this.DisplayFont.Italic)
            {
                lblFont = this.lblFont;
                lblFont.Text = lblFont.Text + "I";
            }
        }

        private void UpdateKeyboardControl(int index)
        {
            this.lstKeyboardControls.Items[index] = this.GetKeyControlText((clsOption<clsKeyboardControl>) modControls.Options_KeyboardControls.Options[index]);
        }

        private void UpdateKeyboardControls(int selectedIndex)
        {
            this.lstKeyboardControls.Hide();
            this.lstKeyboardControls.Items.Clear();
            this.lstKeyboardControls_Items = new clsOption<clsKeyboardControl>[(modControls.Options_KeyboardControls.Options.Count - 1) + 1];
            int num2 = modControls.Options_KeyboardControls.Options.Count - 1;
            for (int i = 0; i <= num2; i++)
            {
                clsOption<clsKeyboardControl> item = (clsOption<clsKeyboardControl>) modControls.Options_KeyboardControls.Options[i];
                string keyControlText = this.GetKeyControlText(item);
                this.lstKeyboardControls_Items[this.lstKeyboardControls.Items.Add(keyControlText)] = item;
            }
            this.lstKeyboardControls.SelectedIndex = selectedIndex;
            this.lstKeyboardControls.Show();
        }

        public virtual Button btnAutosaveOpen
        {
            get
            {
                return this._btnAutosaveOpen;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnAutosaveOpen_Click);
                if (this._btnAutosaveOpen != null)
                {
                    this._btnAutosaveOpen.Click -= handler;
                }
                this._btnAutosaveOpen = value;
                if (this._btnAutosaveOpen != null)
                {
                    this._btnAutosaveOpen.Click += handler;
                }
            }
        }

        public virtual Button btnCancel
        {
            get
            {
                return this._btnCancel;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnCancel_Click);
                if (this._btnCancel != null)
                {
                    this._btnCancel.Click -= handler;
                }
                this._btnCancel = value;
                if (this._btnCancel != null)
                {
                    this._btnCancel.Click += handler;
                }
            }
        }

        public virtual Button btnFont
        {
            get
            {
                return this._btnFont;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnFont_Click);
                if (this._btnFont != null)
                {
                    this._btnFont.Click -= handler;
                }
                this._btnFont = value;
                if (this._btnFont != null)
                {
                    this._btnFont.Click += handler;
                }
            }
        }

        internal virtual Button btnKeyControlChange
        {
            get
            {
                return this._btnKeyControlChange;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnKeyControlChange_Click);
                if (this._btnKeyControlChange != null)
                {
                    this._btnKeyControlChange.Click -= handler;
                }
                this._btnKeyControlChange = value;
                if (this._btnKeyControlChange != null)
                {
                    this._btnKeyControlChange.Click += handler;
                }
            }
        }

        internal virtual Button btnKeyControlChangeDefault
        {
            get
            {
                return this._btnKeyControlChangeDefault;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnKeyControlChangeDefault_Click);
                if (this._btnKeyControlChangeDefault != null)
                {
                    this._btnKeyControlChangeDefault.Click -= handler;
                }
                this._btnKeyControlChangeDefault = value;
                if (this._btnKeyControlChangeDefault != null)
                {
                    this._btnKeyControlChangeDefault.Click += handler;
                }
            }
        }

        internal virtual Button btnKeyControlChangeUnless
        {
            get
            {
                return this._btnKeyControlChangeUnless;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnKeyControlChangeUnless_Click);
                if (this._btnKeyControlChangeUnless != null)
                {
                    this._btnKeyControlChangeUnless.Click -= handler;
                }
                this._btnKeyControlChangeUnless = value;
                if (this._btnKeyControlChangeUnless != null)
                {
                    this._btnKeyControlChangeUnless.Click += handler;
                }
            }
        }

        public virtual Button btnSave
        {
            get
            {
                return this._btnSave;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnSave_Click);
                if (this._btnSave != null)
                {
                    this._btnSave.Click -= handler;
                }
                this._btnSave = value;
                if (this._btnSave != null)
                {
                    this._btnSave.Click += handler;
                }
            }
        }

        public virtual CheckBox cbxAskDirectories
        {
            get
            {
                return this._cbxAskDirectories;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxAskDirectories = value;
            }
        }

        public virtual CheckBox cbxAutosaveCompression
        {
            get
            {
                return this._cbxAutosaveCompression;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxAutosaveCompression = value;
            }
        }

        public virtual CheckBox cbxAutosaveEnabled
        {
            get
            {
                return this._cbxAutosaveEnabled;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxAutosaveEnabled = value;
            }
        }

        public virtual CheckBox cbxMinimapObjectColours
        {
            get
            {
                return this._cbxMinimapObjectColours;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxMinimapObjectColours = value;
            }
        }

        public virtual CheckBox cbxMinimapTeamColourFeatures
        {
            get
            {
                return this._cbxMinimapTeamColourFeatures;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxMinimapTeamColourFeatures = value;
            }
        }

        public virtual CheckBox cbxMipmaps
        {
            get
            {
                return this._cbxMipmaps;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxMipmaps = value;
            }
        }

        public virtual CheckBox cbxMipmapsHardware
        {
            get
            {
                return this._cbxMipmapsHardware;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxMipmapsHardware = value;
            }
        }

        public virtual CheckBox cbxPickerOrientation
        {
            get
            {
                return this._cbxPickerOrientation;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxPickerOrientation = value;
            }
        }

        public virtual CheckBox cbxPointerDirect
        {
            get
            {
                return this._cbxPointerDirect;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._cbxPointerDirect = value;
            }
        }

        public virtual GroupBox GroupBox1
        {
            get
            {
                return this._GroupBox1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox1 = value;
            }
        }

        public virtual GroupBox GroupBox2
        {
            get
            {
                return this._GroupBox2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox2 = value;
            }
        }

        public virtual GroupBox GroupBox3
        {
            get
            {
                return this._GroupBox3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox3 = value;
            }
        }

        public virtual GroupBox GroupBox4
        {
            get
            {
                return this._GroupBox4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox4 = value;
            }
        }

        public virtual GroupBox GroupBox5
        {
            get
            {
                return this._GroupBox5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox5 = value;
            }
        }

        public virtual GroupBox GroupBox6
        {
            get
            {
                return this._GroupBox6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox6 = value;
            }
        }

        public virtual GroupBox GroupBox7
        {
            get
            {
                return this._GroupBox7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox7 = value;
            }
        }

        public virtual GroupBox GroupBox8
        {
            get
            {
                return this._GroupBox8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._GroupBox8 = value;
            }
        }

        public virtual Label Label1
        {
            get
            {
                return this._Label1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label1 = value;
            }
        }

        public virtual Label Label10
        {
            get
            {
                return this._Label10;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label10 = value;
            }
        }

        public virtual Label Label11
        {
            get
            {
                return this._Label11;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label11 = value;
            }
        }

        public virtual Label Label12
        {
            get
            {
                return this._Label12;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label12 = value;
            }
        }

        public virtual Label Label13
        {
            get
            {
                return this._Label13;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label13 = value;
            }
        }

        public virtual Label Label2
        {
            get
            {
                return this._Label2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label2 = value;
            }
        }

        public virtual Label Label3
        {
            get
            {
                return this._Label3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label3 = value;
            }
        }

        public virtual Label Label4
        {
            get
            {
                return this._Label4;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label4 = value;
            }
        }

        public virtual Label Label5
        {
            get
            {
                return this._Label5;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label5 = value;
            }
        }

        public virtual Label Label6
        {
            get
            {
                return this._Label6;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label6 = value;
            }
        }

        internal virtual Label Label7
        {
            get
            {
                return this._Label7;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label7 = value;
            }
        }

        public virtual Label Label8
        {
            get
            {
                return this._Label8;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label8 = value;
            }
        }

        public virtual Label Label9
        {
            get
            {
                return this._Label9;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._Label9 = value;
            }
        }

        public virtual Label lblFont
        {
            get
            {
                return this._lblFont;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblFont = value;
            }
        }

        internal virtual ListBox lstKeyboardControls
        {
            get
            {
                return this._lstKeyboardControls;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lstKeyboardControls = value;
            }
        }

        internal virtual Panel pnlMinimapCliffColour
        {
            get
            {
                return this._pnlMinimapCliffColour;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlMinimapCliffColour = value;
            }
        }

        internal virtual Panel pnlMinimapSelectedObjectColour
        {
            get
            {
                return this._pnlMinimapSelectedObjectColour;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._pnlMinimapSelectedObjectColour = value;
            }
        }

        public virtual TabControl TabControl1
        {
            get
            {
                return this._TabControl1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabControl1 = value;
            }
        }

        internal virtual TableLayoutPanel TableLayoutPanel1
        {
            get
            {
                return this._TableLayoutPanel1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel1 = value;
            }
        }

        public virtual TabPage TabPage1
        {
            get
            {
                return this._TabPage1;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage1 = value;
            }
        }

        internal virtual TabPage TabPage2
        {
            get
            {
                return this._TabPage2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage2 = value;
            }
        }

        internal virtual TabPage TabPage3
        {
            get
            {
                return this._TabPage3;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TabPage3 = value;
            }
        }

        public virtual TextBox txtAutosaveChanges
        {
            get
            {
                return this._txtAutosaveChanges;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtAutosaveChanges = value;
            }
        }

        public virtual TextBox txtAutosaveInterval
        {
            get
            {
                return this._txtAutosaveInterval;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtAutosaveInterval = value;
            }
        }

        public virtual TextBox txtFOV
        {
            get
            {
                return this._txtFOV;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtFOV = value;
            }
        }

        public virtual TextBox txtMapBPP
        {
            get
            {
                return this._txtMapBPP;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtMapBPP = value;
            }
        }

        public virtual TextBox txtMapDepth
        {
            get
            {
                return this._txtMapDepth;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtMapDepth = value;
            }
        }

        public virtual TextBox txtMinimapSize
        {
            get
            {
                return this._txtMinimapSize;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtMinimapSize = value;
            }
        }

        public virtual TextBox txtTexturesBPP
        {
            get
            {
                return this._txtTexturesBPP;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtTexturesBPP = value;
            }
        }

        public virtual TextBox txtTexturesDepth
        {
            get
            {
                return this._txtTexturesDepth;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtTexturesDepth = value;
            }
        }

        public virtual TextBox txtUndoSteps
        {
            get
            {
                return this._txtUndoSteps;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._txtUndoSteps = value;
            }
        }
    }
}

