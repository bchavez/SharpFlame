namespace FlaME
{
    using Matrix3D;
    using Microsoft.VisualBasic;
    using Microsoft.VisualBasic.CompilerServices;
    using OpenTK;
    using OpenTK.Graphics;
    using OpenTK.Graphics.OpenGL;
    using System;
    using System.Collections;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Drawing;
    using System.Runtime.CompilerServices;
    using System.Windows.Forms;

    [DesignerGenerated]
    public class ctrlMapView : UserControl
    {
        [AccessedThroughProperty("btnClose")]
        private Button _btnClose;
        [AccessedThroughProperty("lblPos")]
        private ToolStripStatusLabel _lblPos;
        [AccessedThroughProperty("lblTile")]
        private ToolStripStatusLabel _lblTile;
        [AccessedThroughProperty("lblUndo")]
        private ToolStripStatusLabel _lblUndo;
        [AccessedThroughProperty("lblVertex")]
        private ToolStripStatusLabel _lblVertex;
        [AccessedThroughProperty("ListSelect")]
        private ContextMenuStrip _ListSelect;
        private frmMain _Owner;
        [AccessedThroughProperty("pnlDraw")]
        private Panel _pnlDraw;
        [AccessedThroughProperty("ssStatus")]
        private StatusStrip _ssStatus;
        [AccessedThroughProperty("TableLayoutPanel1")]
        private TableLayoutPanel _TableLayoutPanel1;
        [AccessedThroughProperty("TableLayoutPanel2")]
        private TableLayoutPanel _TableLayoutPanel2;
        [AccessedThroughProperty("tabMaps")]
        private TabControl _tabMaps;
        [AccessedThroughProperty("tmrDraw")]
        private Timer _tmrDraw;
        [AccessedThroughProperty("tmrDrawDelay")]
        private Timer _tmrDrawDelay;
        [AccessedThroughProperty("UndoMessageTimer")]
        private Timer _UndoMessageTimer;
        private IContainer components;
        public bool DrawPending;
        public bool DrawView_Enabled = false;
        private Timer GLInitializeDelayTimer;
        public modMath.sXY_int GLSize;
        public float GLSize_XPerY;
        public bool IsGLInitialized = false;
        private bool ListSelectIsPicker;
        private ToolStripItem[] ListSelectItems = new ToolStripItem[0];
        public GLControl OpenGLControl;

        public ctrlMapView(frmMain Owner)
        {
            this._Owner = Owner;
            this.InitializeComponent();
            this.ListSelect = new ContextMenuStrip();
            this.UndoMessageTimer = new Timer();
            this.OpenGLControl = modMain.OpenGL1;
            this.pnlDraw.Controls.Add(this.OpenGLControl);
            this.GLInitializeDelayTimer = new Timer();
            this.GLInitializeDelayTimer.Interval = 50;
            this.GLInitializeDelayTimer.Tick += new EventHandler(this.GLInitialize);
            this.GLInitializeDelayTimer.Enabled = true;
            this.tmrDraw = new Timer();
            this.tmrDraw.Interval = 1;
            this.tmrDrawDelay = new Timer();
            this.tmrDrawDelay.Interval = 30;
            this.UndoMessageTimer.Interval = 0xfa0;
        }

        private void btnClose_Click(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                if (!mainMap.frmMainLink.IsConnected)
                {
                    Interaction.MsgBox("Error: Map should be closed already.", MsgBoxStyle.ApplicationModal, null);
                }
                else if (mainMap.ClosePrompt())
                {
                    mainMap.Deallocate();
                }
            }
        }

        public GLFont CreateGLFont(Font BaseFont)
        {
            return new GLFont(new Font(BaseFont.FontFamily, 24f, BaseFont.Style, GraphicsUnit.Pixel));
        }

        public void DisplayUndoMessage(string Text)
        {
            this.lblUndo.Text = Text;
            this.UndoMessageTimer.Enabled = false;
            this.UndoMessageTimer.Enabled = true;
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

        private void DrawView()
        {
            if (this.DrawView_Enabled & this.IsGLInitialized)
            {
                sRGB_sng bGColour;
                if (OpenTK.Graphics.GraphicsContext.CurrentContext != this.OpenGLControl.Context)
                {
                    this.OpenGLControl.MakeCurrent();
                }
                clsMap mainMap = this.MainMap;
                if (mainMap == null)
                {
                    bGColour.Red = 0.5f;
                    bGColour.Green = 0.5f;
                    bGColour.Blue = 0.5f;
                }
                else if (mainMap.Tileset == null)
                {
                    bGColour.Red = 0.5f;
                    bGColour.Green = 0.5f;
                    bGColour.Blue = 0.5f;
                }
                else
                {
                    bGColour = mainMap.Tileset.BGColour;
                }
                GL.ClearColor(bGColour.Red, bGColour.Green, bGColour.Blue, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit | ClearBufferMask.DepthBufferBit);
                if (mainMap != null)
                {
                    mainMap.GLDraw();
                }
                GL.Flush();
                this.OpenGLControl.SwapBuffers();
                this.Refresh();
            }
        }

        public void DrawView_SetEnabled(bool Value)
        {
            if (Value)
            {
                if (!this.DrawView_Enabled)
                {
                    this.DrawView_Enabled = true;
                    this.DrawViewLater();
                }
            }
            else
            {
                this.tmrDraw.Enabled = false;
                this.DrawView_Enabled = false;
            }
        }

        public void DrawViewLater()
        {
            this.DrawPending = true;
            if (!this.tmrDrawDelay.Enabled)
            {
                this.tmrDraw.Enabled = true;
            }
        }

        private void GLInitialize(object sender, EventArgs e)
        {
            if (this.OpenGLControl.Context != null)
            {
                this.GLInitializeDelayTimer.Enabled = false;
                this.GLInitializeDelayTimer.Tick -= new EventHandler(this.GLInitialize);
                this.GLInitializeDelayTimer.Dispose();
                this.GLInitializeDelayTimer = null;
                this.ResizeOpenGL();
                this.OpenGLControl.MouseDown += new MouseEventHandler(this.OpenGL_MouseDown);
                this.OpenGLControl.MouseUp += new MouseEventHandler(this.OpenGL_MouseUp);
                this.OpenGLControl.MouseWheel += new MouseEventHandler(this.OpenGL_MouseWheel);
                this.OpenGLControl.MouseMove += new MouseEventHandler(this.OpenGL_MouseMove);
                this.OpenGLControl.MouseEnter += new EventHandler(this.OpenGL_MouseEnter);
                this.OpenGLControl.MouseLeave += new EventHandler(this.OpenGL_MouseLeave);
                this.OpenGLControl.Resize += new EventHandler(this.OpenGL_Resize);
                this.OpenGLControl.Leave += new EventHandler(this.OpenGL_LostFocus);
                this.OpenGLControl.PreviewKeyDown += new PreviewKeyDownEventHandler(this.OpenGL_KeyDown);
                this.OpenGLControl.KeyUp += new KeyEventHandler(this.OpenGL_KeyUp);
                if (OpenTK.Graphics.GraphicsContext.CurrentContext != this.OpenGLControl.Context)
                {
                    this.OpenGLControl.MakeCurrent();
                }
                GL.PixelStore(PixelStoreParameter.PackAlignment, 1);
                GL.PixelStore(PixelStoreParameter.UnpackAlignment, 1);
                GL.ClearColor(0f, 0f, 0f, 1f);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.ShadeModel(ShadingModel.Smooth);
                GL.Hint(HintTarget.PerspectiveCorrectionHint, HintMode.Nicest);
                GL.Enable(EnableCap.DepthTest);
                float[] @params = new float[4];
                float[] numArray7 = new float[4];
                float[] numArray2 = new float[4];
                @params[0] = 0.3333333f;
                @params[1] = 0.3333333f;
                @params[2] = 0.3333333f;
                @params[3] = 1f;
                numArray7[0] = 0.6666667f;
                numArray7[1] = 0.6666667f;
                numArray7[2] = 0.6666667f;
                numArray7[3] = 1f;
                numArray2[0] = 0.75f;
                numArray2[1] = 0.75f;
                numArray2[2] = 0.75f;
                numArray2[3] = 1f;
                GL.Light(LightName.Light0, LightParameter.Diffuse, numArray2);
                GL.Light(LightName.Light0, LightParameter.Specular, numArray7);
                GL.Light(LightName.Light0, LightParameter.Ambient, @params);
                @params[0] = 0.25f;
                @params[1] = 0.25f;
                @params[2] = 0.25f;
                @params[3] = 1f;
                numArray7[0] = 0.5f;
                numArray7[1] = 0.5f;
                numArray7[2] = 0.5f;
                numArray7[3] = 1f;
                numArray2[0] = 0.5625f;
                numArray2[1] = 0.5625f;
                numArray2[2] = 0.5625f;
                numArray2[3] = 1f;
                GL.Light(LightName.Light1, LightParameter.Diffuse, numArray2);
                GL.Light(LightName.Light1, LightParameter.Specular, numArray7);
                GL.Light(LightName.Light1, LightParameter.Ambient, @params);
                float[] numArray4 = new float[4];
                float[] numArray6 = new float[4];
                float[] numArray3 = new float[4];
                float[] numArray5 = new float[1];
                numArray6[0] = 0f;
                numArray6[1] = 0f;
                numArray6[2] = 0f;
                numArray6[3] = 0f;
                numArray3[0] = 1f;
                numArray3[1] = 1f;
                numArray3[2] = 1f;
                numArray3[3] = 1f;
                numArray4[0] = 1f;
                numArray4[1] = 1f;
                numArray4[2] = 1f;
                numArray4[3] = 1f;
                numArray5[0] = 0f;
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Ambient, numArray3);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Specular, numArray6);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Diffuse, numArray4);
                GL.Material(MaterialFace.FrontAndBack, MaterialParameter.Shininess, numArray5);
                this.IsGLInitialized = true;
            }
        }

        [DebuggerStepThrough]
        private void InitializeComponent()
        {
            this.ssStatus = new StatusStrip();
            this.lblTile = new ToolStripStatusLabel();
            this.lblVertex = new ToolStripStatusLabel();
            this.lblPos = new ToolStripStatusLabel();
            this.lblUndo = new ToolStripStatusLabel();
            this.pnlDraw = new Panel();
            this.TableLayoutPanel1 = new TableLayoutPanel();
            this.TableLayoutPanel2 = new TableLayoutPanel();
            this.tabMaps = new TabControl();
            this.btnClose = new Button();
            this.ssStatus.SuspendLayout();
            this.TableLayoutPanel1.SuspendLayout();
            this.TableLayoutPanel2.SuspendLayout();
            this.SuspendLayout();
            this.ssStatus.Items.AddRange(new ToolStripItem[] { this.lblTile, this.lblVertex, this.lblPos, this.lblUndo });
            Point point2 = new Point(0, 0x188);
            this.ssStatus.Location = point2;
            this.ssStatus.Name = "ssStatus";
            Size size2 = new Size(0x51c, 0x20);
            this.ssStatus.Size = size2;
            this.ssStatus.TabIndex = 0;
            this.ssStatus.Text = "StatusStrip1";
            this.lblTile.AutoSize = false;
            this.lblTile.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            Padding padding2 = new Padding(2, 3, 2, 2);
            this.lblTile.Margin = padding2;
            this.lblTile.Name = "lblTile";
            size2 = new Size(0xc0, 0x1b);
            this.lblTile.Size = size2;
            this.lblTile.TextAlign = ContentAlignment.MiddleLeft;
            this.lblVertex.AutoSize = false;
            this.lblVertex.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            padding2 = new Padding(2, 3, 2, 2);
            this.lblVertex.Margin = padding2;
            this.lblVertex.Name = "lblVertex";
            size2 = new Size(0x100, 0x1b);
            this.lblVertex.Size = size2;
            this.lblVertex.TextAlign = ContentAlignment.MiddleLeft;
            this.lblPos.AutoSize = false;
            this.lblPos.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            padding2 = new Padding(2, 3, 2, 2);
            this.lblPos.Margin = padding2;
            this.lblPos.Name = "lblPos";
            size2 = new Size(320, 0x1b);
            this.lblPos.Size = size2;
            this.lblPos.TextAlign = ContentAlignment.MiddleLeft;
            this.lblUndo.AutoSize = false;
            this.lblUndo.Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Regular, GraphicsUnit.Point, 0);
            this.lblUndo.Name = "lblUndo";
            size2 = new Size(0x100, 0x1b);
            this.lblUndo.Size = size2;
            this.lblUndo.TextAlign = ContentAlignment.MiddleLeft;
            this.pnlDraw.Dock = DockStyle.Fill;
            point2 = new Point(0, 0x1c);
            this.pnlDraw.Location = point2;
            padding2 = new Padding(0);
            this.pnlDraw.Margin = padding2;
            this.pnlDraw.Name = "pnlDraw";
            size2 = new Size(0x51c, 0x16c);
            this.pnlDraw.Size = size2;
            this.pnlDraw.TabIndex = 1;
            this.TableLayoutPanel1.ColumnCount = 1;
            this.TableLayoutPanel1.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel1.Controls.Add(this.pnlDraw, 0, 1);
            this.TableLayoutPanel1.Controls.Add(this.TableLayoutPanel2, 0, 0);
            this.TableLayoutPanel1.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TableLayoutPanel1.Location = point2;
            this.TableLayoutPanel1.Name = "TableLayoutPanel1";
            this.TableLayoutPanel1.RowCount = 2;
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Absolute, 28f));
            this.TableLayoutPanel1.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            size2 = new Size(0x51c, 0x188);
            this.TableLayoutPanel1.Size = size2;
            this.TableLayoutPanel1.TabIndex = 2;
            this.TableLayoutPanel2.ColumnCount = 2;
            this.TableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel2.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 32f));
            this.TableLayoutPanel2.Controls.Add(this.tabMaps, 0, 0);
            this.TableLayoutPanel2.Controls.Add(this.btnClose, 1, 0);
            this.TableLayoutPanel2.Dock = DockStyle.Fill;
            point2 = new Point(0, 0);
            this.TableLayoutPanel2.Location = point2;
            padding2 = new Padding(0);
            this.TableLayoutPanel2.Margin = padding2;
            this.TableLayoutPanel2.Name = "TableLayoutPanel2";
            this.TableLayoutPanel2.RowCount = 1;
            this.TableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Percent, 100f));
            this.TableLayoutPanel2.RowStyles.Add(new RowStyle(SizeType.Absolute, 20f));
            size2 = new Size(0x51c, 0x1c);
            this.TableLayoutPanel2.Size = size2;
            this.TableLayoutPanel2.TabIndex = 2;
            this.tabMaps.Appearance = TabAppearance.Buttons;
            this.tabMaps.Dock = DockStyle.Fill;
            point2 = new Point(3, 3);
            this.tabMaps.Location = point2;
            this.tabMaps.Name = "tabMaps";
            this.tabMaps.SelectedIndex = 0;
            size2 = new Size(0x4f6, 0x16);
            this.tabMaps.Size = size2;
            this.tabMaps.TabIndex = 2;
            this.btnClose.Dock = DockStyle.Fill;
            point2 = new Point(0x4fc, 0);
            this.btnClose.Location = point2;
            padding2 = new Padding(0);
            this.btnClose.Margin = padding2;
            this.btnClose.Name = "btnClose";
            size2 = new Size(0x20, 0x1c);
            this.btnClose.Size = size2;
            this.btnClose.TabIndex = 3;
            this.btnClose.Text = "X";
            this.btnClose.UseVisualStyleBackColor = true;
            this.AutoScaleMode = AutoScaleMode.None;
            this.Controls.Add(this.TableLayoutPanel1);
            this.Controls.Add(this.ssStatus);
            padding2 = new Padding(0);
            this.Margin = padding2;
            this.Name = "ctrlMapView";
            size2 = new Size(0x51c, 0x1a8);
            this.Size = size2;
            this.ssStatus.ResumeLayout(false);
            this.ssStatus.PerformLayout();
            this.TableLayoutPanel1.ResumeLayout(false);
            this.TableLayoutPanel2.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();
        }

        private void ListSelect_Click(object Sender, ToolStripItemClickedEventArgs e)
        {
            clsMap.clsUnit tag = (clsMap.clsUnit) e.ClickedItem.Tag;
            if (this.ListSelectIsPicker)
            {
                modMain.frmMainInstance.ObjectPicker(tag.Type);
            }
            else
            {
                if (tag.MapSelectedUnitLink.IsConnected)
                {
                    tag.MapDeselect();
                }
                else
                {
                    tag.MapSelect();
                }
                modMain.frmMainInstance.SelectedObject_Changed();
                this.DrawViewLater();
            }
        }

        private void ListSelect_Close(object sender, ToolStripDropDownClosedEventArgs e)
        {
            int upperBound = this.ListSelectItems.GetUpperBound(0);
            for (int i = 0; i <= upperBound; i++)
            {
                this.ListSelectItems[i].Tag = null;
                this.ListSelectItems[i].Dispose();
            }
            this.ListSelect.Items.Clear();
            this.ListSelectItems = new ToolStripItem[0];
            modProgram.ViewKeyDown_Clear();
        }

        public void ListSelectBegin(bool isPicker)
        {
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (mouseOverTerrain == null)
            {
                Debugger.Break();
            }
            else
            {
                this.ListSelect.Close();
                this.ListSelect.Items.Clear();
                this.ListSelectItems = new ToolStripItem[(mouseOverTerrain.Units.Count - 1) + 1];
                int num2 = mouseOverTerrain.Units.Count - 1;
                for (int i = 0; i <= num2; i++)
                {
                    clsMap.clsUnit unit = mouseOverTerrain.Units[i];
                    this.ListSelectItems[i] = new ToolStripButton(unit.Type.GetDisplayTextCode());
                    this.ListSelectItems[i].Tag = unit;
                    this.ListSelect.Items.Add(this.ListSelectItems[i]);
                }
                this.ListSelectIsPicker = isPicker;
                Point position = new Point(mainMap.ViewInfo.MouseOver.ScreenPos.X, mainMap.ViewInfo.MouseOver.ScreenPos.Y);
                this.ListSelect.Show(this, position);
            }
        }

        private void OpenGL_KeyDown(object sender, PreviewKeyDownEventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                IEnumerator enumerator;
                Matrix3DMath.Matrix3D matrix = new Matrix3DMath.Matrix3D();
                clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
                modProgram.IsViewKeyDown.Keys[(int) e.KeyCode] = true;
                try
                {
                    enumerator = modControls.Options_KeyboardControls.Options.GetEnumerator();
                    while (enumerator.MoveNext())
                    {
                        clsOption<clsKeyboardControl> current = (clsOption<clsKeyboardControl>) enumerator.Current;
                        ((clsKeyboardControl) modControls.KeyboardProfile.get_Value(current)).KeysChanged(modProgram.IsViewKeyDown);
                    }
                }
                finally
                {
                    if (enumerator is IDisposable)
                    {
                        (enumerator as IDisposable).Dispose();
                    }
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_Undo))
                {
                    string str;
                    if (mainMap.UndoPosition > 0)
                    {
                        str = "Undid: " + mainMap.Undos[mainMap.UndoPosition - 1].Name;
                        clsMap.clsMessage newItem = new clsMap.clsMessage {
                            Text = str
                        };
                        mainMap.Messages.Add(newItem);
                        mainMap.UndoPerform();
                        this.DrawViewLater();
                    }
                    else
                    {
                        str = "Nothing to undo";
                    }
                    this.DisplayUndoMessage(str);
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_Redo))
                {
                    string str2;
                    if (mainMap.UndoPosition < mainMap.Undos.Count)
                    {
                        str2 = "Redid: " + mainMap.Undos[mainMap.UndoPosition].Name;
                        clsMap.clsMessage message2 = new clsMap.clsMessage {
                            Text = str2
                        };
                        mainMap.Messages.Add(message2);
                        mainMap.RedoPerform();
                        this.DrawViewLater();
                    }
                    else
                    {
                        str2 = "Nothing to redo";
                    }
                    this.DisplayUndoMessage(str2);
                }
                if (modProgram.IsViewKeyDown.Keys[0x11])
                {
                    if (e.KeyCode == Keys.D1)
                    {
                        modProgram.VisionRadius_2E = 6;
                    }
                    else if (e.KeyCode == Keys.D2)
                    {
                        modProgram.VisionRadius_2E = 7;
                    }
                    else if (e.KeyCode == Keys.D3)
                    {
                        modProgram.VisionRadius_2E = 8;
                    }
                    else if (e.KeyCode == Keys.D4)
                    {
                        modProgram.VisionRadius_2E = 9;
                    }
                    else if (e.KeyCode == Keys.D5)
                    {
                        modProgram.VisionRadius_2E = 10;
                    }
                    else if (e.KeyCode == Keys.D6)
                    {
                        modProgram.VisionRadius_2E = 11;
                    }
                    else if (e.KeyCode == Keys.D7)
                    {
                        modProgram.VisionRadius_2E = 12;
                    }
                    else if (e.KeyCode == Keys.D8)
                    {
                        modProgram.VisionRadius_2E = 13;
                    }
                    else if (e.KeyCode == Keys.D9)
                    {
                        modProgram.VisionRadius_2E = 14;
                    }
                    else if (e.KeyCode == Keys.D0)
                    {
                        modProgram.VisionRadius_2E = 15;
                    }
                    modProgram.VisionRadius_2E_Changed();
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Move_Type))
                {
                    if (modProgram.ViewMoveType == modProgram.enumView_Move_Type.Free)
                    {
                        modProgram.ViewMoveType = modProgram.enumView_Move_Type.RTS;
                    }
                    else if (modProgram.ViewMoveType == modProgram.enumView_Move_Type.RTS)
                    {
                        modProgram.ViewMoveType = modProgram.enumView_Move_Type.Free;
                    }
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Rotate_Type))
                {
                    modProgram.RTSOrbit = !modProgram.RTSOrbit;
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Reset))
                {
                    mainMap.ViewInfo.FOV_Multiplier_Set(modSettings.Settings.FOVDefault);
                    if (modProgram.ViewMoveType == modProgram.enumView_Move_Type.Free)
                    {
                        Matrix3DMath.MatrixSetToXAngle(matrix, Math.Atan(2.0));
                        mainMap.ViewInfo.ViewAngleSet_Rotate(matrix);
                    }
                    else if (modProgram.ViewMoveType == modProgram.enumView_Move_Type.RTS)
                    {
                        Matrix3DMath.MatrixSetToXAngle(matrix, Math.Atan(2.0));
                        mainMap.ViewInfo.ViewAngleSet_Rotate(matrix);
                    }
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Textures))
                {
                    modProgram.Draw_TileTextures = !modProgram.Draw_TileTextures;
                    this.DrawViewLater();
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Wireframe))
                {
                    modProgram.Draw_TileWireframe = !modProgram.Draw_TileWireframe;
                    this.DrawViewLater();
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Units))
                {
                    modProgram.Draw_Units = !modProgram.Draw_Units;
                    int num3 = mainMap.SectorCount.Y - 1;
                    for (int i = 0; i <= num3; i++)
                    {
                        int num4 = mainMap.SectorCount.X - 1;
                        for (int j = 0; j <= num4; j++)
                        {
                            IEnumerator enumerator2;
                            try
                            {
                                enumerator2 = mainMap.Sectors[j, i].Units.GetEnumerator();
                                while (enumerator2.MoveNext())
                                {
                                    clsMap.clsUnitSectorConnection connection = (clsMap.clsUnitSectorConnection) enumerator2.Current;
                                    clsMap.clsUnit unit = connection.Unit;
                                    if ((unit.Type.Type == clsUnitType.enumType.PlayerStructure) && (((clsStructureType) unit.Type).StructureBasePlate != null))
                                    {
                                        modMath.sXY_int _int;
                                        _int.X = j;
                                        _int.Y = i;
                                        mainMap.SectorGraphicsChanges.Changed(_int);
                                        continue;
                                    }
                                }
                            }
                            finally
                            {
                                if (enumerator2 is IDisposable)
                                {
                                    (enumerator2 as IDisposable).Dispose();
                                }
                            }
                        }
                    }
                    mainMap.Update();
                    this.DrawViewLater();
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_ScriptMarkers))
                {
                    modProgram.Draw_ScriptMarkers = !modProgram.Draw_ScriptMarkers;
                    this.DrawViewLater();
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_View_Lighting))
                {
                    if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Off)
                    {
                        modProgram.Draw_Lighting = modProgram.enumDrawLighting.Half;
                    }
                    else if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Half)
                    {
                        modProgram.Draw_Lighting = modProgram.enumDrawLighting.Normal;
                    }
                    else if (modProgram.Draw_Lighting == modProgram.enumDrawLighting.Normal)
                    {
                        modProgram.Draw_Lighting = modProgram.enumDrawLighting.Off;
                    }
                    this.DrawViewLater();
                }
                if ((modTools.Tool == modTools.Tools.TextureBrush) && (mouseOverTerrain != null))
                {
                    if (modControls.KeyboardProfile.Active(modControls.Control_Clockwise))
                    {
                        mainMap.ViewInfo.Apply_Texture_Clockwise();
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_CounterClockwise))
                    {
                        mainMap.ViewInfo.Apply_Texture_CounterClockwise();
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_Texture_Flip))
                    {
                        mainMap.ViewInfo.Apply_Texture_FlipX();
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_Tri_Flip))
                    {
                        mainMap.ViewInfo.Apply_Tri_Flip();
                    }
                }
                if (modTools.Tool == modTools.Tools.ObjectSelect)
                {
                    if (modControls.KeyboardProfile.Active(modControls.Control_Unit_Delete) && (mainMap.SelectedUnits.Count > 0))
                    {
                        IEnumerator enumerator3;
                        try
                        {
                            enumerator3 = mainMap.SelectedUnits.GetItemsAsSimpleList().GetEnumerator();
                            while (enumerator3.MoveNext())
                            {
                                clsMap.clsUnit unit2 = (clsMap.clsUnit) enumerator3.Current;
                                mainMap.UnitRemoveStoreChange(unit2.MapLink.ArrayPosition);
                            }
                        }
                        finally
                        {
                            if (enumerator3 is IDisposable)
                            {
                                (enumerator3 as IDisposable).Dispose();
                            }
                        }
                        modMain.frmMainInstance.SelectedObject_Changed();
                        mainMap.UndoStepCreate("Object Deleted");
                        mainMap.Update();
                        mainMap.MinimapMakeLater();
                        this.DrawViewLater();
                    }
                    if ((modControls.KeyboardProfile.Active(modControls.Control_Unit_Move) && (mouseOverTerrain != null)) && (mainMap.SelectedUnits.Count > 0))
                    {
                        modMath.sXY_int _int2;
                        Position.XY_dbl _dbl = modProgram.CalcUnitsCentrePos(mainMap.SelectedUnits.GetItemsAsSimpleList());
                        _int2.X = ((int) Math.Round(Math.Round((double) ((mouseOverTerrain.Pos.Horizontal.X - _dbl.X) / 128.0)))) * 0x80;
                        _int2.Y = ((int) Math.Round(Math.Round((double) ((mouseOverTerrain.Pos.Horizontal.Y - _dbl.Y) / 128.0)))) * 0x80;
                        clsMap.clsObjectPosOffset tool = new clsMap.clsObjectPosOffset {
                            Map = mainMap,
                            Offset = _int2
                        };
                        mainMap.SelectedUnitsAction(tool);
                        mainMap.UndoStepCreate("Objects Moved");
                        mainMap.Update();
                        mainMap.MinimapMakeLater();
                        modMain.frmMainInstance.SelectedObject_Changed();
                        this.DrawViewLater();
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_Clockwise))
                    {
                        clsMap.clsObjectRotationOffset offset2 = new clsMap.clsObjectRotationOffset {
                            Map = mainMap,
                            Offset = -90
                        };
                        mainMap.SelectedUnitsAction(offset2);
                        mainMap.Update();
                        modMain.frmMainInstance.SelectedObject_Changed();
                        mainMap.UndoStepCreate("Object Rotated");
                        this.DrawViewLater();
                    }
                    if (modControls.KeyboardProfile.Active(modControls.Control_CounterClockwise))
                    {
                        clsMap.clsObjectRotationOffset offset3 = new clsMap.clsObjectRotationOffset {
                            Map = mainMap,
                            Offset = 90
                        };
                        mainMap.SelectedUnitsAction(offset3);
                        mainMap.Update();
                        modMain.frmMainInstance.SelectedObject_Changed();
                        mainMap.UndoStepCreate("Object Rotated");
                        this.DrawViewLater();
                    }
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_Deselect))
                {
                    modTools.Tool = modTools.Tools.ObjectSelect;
                    this.DrawViewLater();
                }
                if (modControls.KeyboardProfile.Active(modControls.Control_PreviousTool))
                {
                    modTools.Tool = modTools.PreviousTool;
                    this.DrawViewLater();
                }
            }
        }

        private void OpenGL_KeyUp(object sender, KeyEventArgs e)
        {
            IEnumerator enumerator;
            modProgram.IsViewKeyDown.Keys[(int) e.KeyCode] = false;
            try
            {
                enumerator = modControls.Options_KeyboardControls.Options.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    clsOption<clsKeyboardControl> current = (clsOption<clsKeyboardControl>) enumerator.Current;
                    ((clsKeyboardControl) modControls.KeyboardProfile.get_Value(current)).KeysChanged(modProgram.IsViewKeyDown);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
        }

        public void OpenGL_LostFocus(object eventSender, EventArgs eventArgs)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.SuppressMinimap = false;
                mainMap.ViewInfo.MouseOver = null;
                mainMap.ViewInfo.MouseLeftDown = null;
                mainMap.ViewInfo.MouseRightDown = null;
                modProgram.ViewKeyDown_Clear();
            }
        }

        private void OpenGL_MouseDown(object sender, MouseEventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.ViewInfo.MouseDown(e);
            }
        }

        public void OpenGL_MouseEnter(object sender, EventArgs e)
        {
            if (Form.ActiveForm == modMain.frmMainInstance)
            {
                this.OpenGLControl.Focus();
            }
        }

        private void OpenGL_MouseLeave(object sender, EventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.ViewInfo.MouseOver = null;
            }
        }

        public void OpenGL_MouseMove(object sender, MouseEventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                mainMap.ViewInfo.Map.ViewInfo.MouseOver = new clsViewInfo.clsMouseOver();
                mainMap.ViewInfo.MouseOver.ScreenPos.X = e.X;
                mainMap.ViewInfo.MouseOver.ScreenPos.Y = e.Y;
                mainMap.ViewInfo.MouseOver_Pos_Calc();
            }
        }

        private void OpenGL_MouseUp(object sender, MouseEventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
                mainMap.SuppressMinimap = false;
                if (e.Button == MouseButtons.Left)
                {
                    if (mainMap.ViewInfo.GetMouseLeftDownOverMinimap() == null)
                    {
                        if (modTools.Tool == modTools.Tools.TerrainBrush)
                        {
                            mainMap.UndoStepCreate("Ground Painted");
                        }
                        else if (modTools.Tool == modTools.Tools.CliffTriangle)
                        {
                            mainMap.UndoStepCreate("Cliff Triangles");
                        }
                        else if (modTools.Tool == modTools.Tools.CliffBrush)
                        {
                            mainMap.UndoStepCreate("Cliff Brush");
                        }
                        else if (modTools.Tool == modTools.Tools.CliffRemove)
                        {
                            mainMap.UndoStepCreate("Cliff Remove Brush");
                        }
                        else if (modTools.Tool == modTools.Tools.HeightChangeBrush)
                        {
                            mainMap.UndoStepCreate("Height Change");
                        }
                        else if (modTools.Tool == modTools.Tools.HeightSetBrush)
                        {
                            mainMap.UndoStepCreate("Height Set");
                        }
                        else if (modTools.Tool == modTools.Tools.HeightSmoothBrush)
                        {
                            mainMap.UndoStepCreate("Height Smooth");
                        }
                        else if (modTools.Tool == modTools.Tools.TextureBrush)
                        {
                            mainMap.UndoStepCreate("Texture");
                        }
                        else if (modTools.Tool == modTools.Tools.RoadRemove)
                        {
                            mainMap.UndoStepCreate("Road Remove");
                        }
                        else if ((modTools.Tool == modTools.Tools.ObjectSelect) && (mainMap.Unit_Selected_Area_VertexA != null))
                        {
                            if (mouseOverTerrain != null)
                            {
                                this.SelectUnits(mainMap.Unit_Selected_Area_VertexA.XY, mouseOverTerrain.Vertex.Normal);
                            }
                            mainMap.Unit_Selected_Area_VertexA = null;
                        }
                    }
                    mainMap.ViewInfo.MouseLeftDown = null;
                }
                else if (e.Button == MouseButtons.Right)
                {
                    if (mainMap.ViewInfo.GetMouseRightDownOverMinimap() == null)
                    {
                        if (modTools.Tool == modTools.Tools.HeightChangeBrush)
                        {
                            mainMap.UndoStepCreate("Height Change");
                        }
                        else if (modTools.Tool == modTools.Tools.HeightSetBrush)
                        {
                            mainMap.UndoStepCreate("Height Set");
                        }
                    }
                    mainMap.ViewInfo.MouseRightDown = null;
                }
            }
        }

        public void OpenGL_MouseWheel(object sender, MouseEventArgs e)
        {
            clsMap mainMap = this.MainMap;
            if (mainMap != null)
            {
                int num2 = (int) Math.Round(Math.Abs((double) (((double) e.Delta) / 120.0)));
                for (int i = 0; i <= num2; i++)
                {
                    modMath.sXYZ_int _int;
                    Position.XYZ_dbl _dbl;
                    Matrix3DMath.VectorForwardsRotationByMatrix(mainMap.ViewInfo.ViewAngleMatrix, (Math.Sign(e.Delta) * Math.Max((double) mainMap.ViewInfo.ViewPos.Y, 512.0)) / 24.0, ref _dbl);
                    _int.Set_dbl(_dbl);
                    mainMap.ViewInfo.ViewPosChange(_int);
                }
            }
        }

        public void OpenGL_Resize(object eventSender, EventArgs eventArgs)
        {
            clsMap mainMap = this.MainMap;
            this.GLSize.X = this.OpenGLControl.Width;
            this.GLSize.Y = this.OpenGLControl.Height;
            if (this.GLSize.Y != 0)
            {
                this.GLSize_XPerY = (float) (((double) this.GLSize.X) / ((double) this.GLSize.Y));
            }
            this.Viewport_Resize();
            if (mainMap != null)
            {
                mainMap.ViewInfo.FOV_Calc();
            }
            this.DrawViewLater();
        }

        private void pnlDraw_Resize(object sender, EventArgs e)
        {
            if (this.OpenGLControl != null)
            {
                this.ResizeOpenGL();
            }
        }

        public void Pos_Display_Update()
        {
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (mouseOverTerrain == null)
            {
                this.lblTile.Text = "";
                this.lblVertex.Text = "";
                this.lblPos.Text = "";
            }
            else
            {
                this.lblTile.Text = "Tile x:" + Conversions.ToString(mouseOverTerrain.Tile.Normal.X) + ", y:" + Conversions.ToString(mouseOverTerrain.Tile.Normal.Y);
                this.lblVertex.Text = "Vertex  x:" + Conversions.ToString(mouseOverTerrain.Vertex.Normal.X) + ", y:" + Conversions.ToString(mouseOverTerrain.Vertex.Normal.Y) + ", alt:" + Conversions.ToString((int) (mainMap.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height * mainMap.HeightMultiplier)) + " (" + Conversions.ToString(mainMap.Terrain.Vertices[mouseOverTerrain.Vertex.Normal.X, mouseOverTerrain.Vertex.Normal.Y].Height) + "x" + Conversions.ToString(mainMap.HeightMultiplier) + ")";
                this.lblPos.Text = "Pos x:" + Conversions.ToString(mouseOverTerrain.Pos.Horizontal.X) + ", y:" + Conversions.ToString(mouseOverTerrain.Pos.Horizontal.Y) + ", alt:" + Conversions.ToString(mouseOverTerrain.Pos.Altitude) + ", slope: " + Conversions.ToString((double) (Math.Round((double) ((mainMap.GetTerrainSlopeAngle(mouseOverTerrain.Pos.Horizontal) / 0.017453292519943295) * 10.0)) / 10.0)) + "\x00b0";
            }
        }

        public void RemoveUndoMessage(object sender, EventArgs e)
        {
            this.UndoMessageTimer.Enabled = false;
            this.lblUndo.Text = "";
        }

        public void ResizeOpenGL()
        {
            if (this.OpenGLControl.Context != null)
            {
                this.OpenGLControl.Width = this.pnlDraw.Width;
                this.OpenGLControl.Height = this.pnlDraw.Height;
            }
        }

        private void SelectUnits(modMath.sXY_int VertexA, modMath.sXY_int VertexB)
        {
            clsMap.clsUnit unit;
            clsMap mainMap = this.MainMap;
            clsViewInfo.clsMouseOver.clsOverTerrain mouseOverTerrain = mainMap.ViewInfo.GetMouseOverTerrain();
            if (((Math.Abs((int) (VertexA.X - VertexB.X)) <= 1) & (Math.Abs((int) (VertexA.Y - VertexB.Y)) <= 1)) & (mouseOverTerrain != null))
            {
                if (mouseOverTerrain.Units.Count > 0)
                {
                    if (mouseOverTerrain.Units.Count == 1)
                    {
                        unit = mouseOverTerrain.Units[0];
                        if (unit.MapSelectedUnitLink.IsConnected)
                        {
                            unit.MapDeselect();
                        }
                        else
                        {
                            unit.MapSelect();
                        }
                    }
                    else
                    {
                        this.ListSelectBegin(false);
                    }
                }
            }
            else
            {
                modMath.sXY_int _int;
                modMath.sXY_int _int2;
                modMath.sXY_int _int3;
                modMath.sXY_int _int4;
                modMath.sXY_int _int5;
                modMath.sXY_int _int6;
                modMath.sXY_int _int7;
                modMath.ReorderXY(VertexA, VertexB, ref _int7, ref _int2);
                _int6.X = _int7.X * 0x80;
                _int6.Y = _int7.Y * 0x80;
                _int.X = _int2.X * 0x80;
                _int.Y = _int2.Y * 0x80;
                _int5.X = Math.Min((int) Math.Round(Conversion.Int((double) (((double) _int7.X) / 8.0))), mainMap.SectorCount.X - 1);
                _int5.Y = Math.Min((int) Math.Round(Conversion.Int((double) (((double) _int7.Y) / 8.0))), mainMap.SectorCount.Y - 1);
                _int3.X = Math.Min((int) Math.Round(Conversion.Int((double) (((double) _int2.X) / 8.0))), mainMap.SectorCount.X - 1);
                _int3.Y = Math.Min((int) Math.Round(Conversion.Int((double) (((double) _int2.Y) / 8.0))), mainMap.SectorCount.Y - 1);
                int y = _int3.Y;
                _int4.Y = _int5.Y;
                while (_int4.Y <= y)
                {
                    int x = _int3.X;
                    _int4.X = _int5.X;
                    while (_int4.X <= x)
                    {
                        IEnumerator enumerator;
                        try
                        {
                            enumerator = mainMap.Sectors[_int4.X, _int4.Y].Units.GetEnumerator();
                            while (enumerator.MoveNext())
                            {
                                clsMap.clsUnitSectorConnection current = (clsMap.clsUnitSectorConnection) enumerator.Current;
                                unit = current.Unit;
                                if (((((unit.Pos.Horizontal.X >= _int6.X) & (unit.Pos.Horizontal.Y >= _int6.Y)) & (unit.Pos.Horizontal.X <= _int.X)) & (unit.Pos.Horizontal.Y <= _int.Y)) && !unit.MapSelectedUnitLink.IsConnected)
                                {
                                    unit.MapSelect();
                                }
                            }
                        }
                        finally
                        {
                            if (enumerator is IDisposable)
                            {
                                (enumerator as IDisposable).Dispose();
                            }
                        }
                        _int4.X++;
                    }
                    _int4.Y++;
                }
            }
            modMain.frmMainInstance.SelectedObject_Changed();
            this.DrawViewLater();
        }

        private void tabMaps_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (this.tabMaps.Enabled)
            {
                if (this.tabMaps.SelectedTab == null)
                {
                    this._Owner.SetMainMap(null);
                }
                else
                {
                    clsMap tag = (clsMap) this.tabMaps.SelectedTab.Tag;
                    this._Owner.SetMainMap(tag);
                }
            }
        }

        private void tmrDraw_Tick(object sender, EventArgs e)
        {
            this.tmrDraw.Enabled = false;
            if (this.DrawPending)
            {
                this.DrawView();
                this.DrawPending = false;
                this.tmrDrawDelay.Enabled = true;
            }
        }

        private void tmrDrawDelay_Tick(object sender, EventArgs e)
        {
            if (this.DrawPending)
            {
                this.DrawPending = false;
                this.DrawView();
            }
            else
            {
                this.tmrDrawDelay.Enabled = false;
            }
        }

        public void UpdateTabs()
        {
            clsMap mainMap;
            IEnumerator enumerator;
            this.tabMaps.Enabled = false;
            this.tabMaps.TabPages.Clear();
            try
            {
                enumerator = this._Owner.LoadedMaps.GetEnumerator();
                while (enumerator.MoveNext())
                {
                    mainMap = (clsMap) enumerator.Current;
                    this.tabMaps.TabPages.Add(mainMap.MapView_TabPage);
                }
            }
            finally
            {
                if (enumerator is IDisposable)
                {
                    (enumerator as IDisposable).Dispose();
                }
            }
            mainMap = this.MainMap;
            if (mainMap != null)
            {
                this.tabMaps.SelectedIndex = mainMap.frmMainLink.ArrayPosition;
            }
            else
            {
                this.tabMaps.SelectedIndex = -1;
            }
            this.tabMaps.Enabled = true;
        }

        public void Viewport_Resize()
        {
            if (modProgram.ProgramInitialized)
            {
                if (OpenTK.Graphics.GraphicsContext.CurrentContext != this.OpenGLControl.Context)
                {
                    this.OpenGLControl.MakeCurrent();
                }
                GL.Viewport(0, 0, this.GLSize.X, this.GLSize.Y);
                GL.Clear(ClearBufferMask.ColorBufferBit);
                GL.Flush();
                this.OpenGLControl.SwapBuffers();
                this.Refresh();
                this.DrawViewLater();
            }
        }

        internal virtual Button btnClose
        {
            get
            {
                return this._btnClose;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.btnClose_Click);
                if (this._btnClose != null)
                {
                    this._btnClose.Click -= handler;
                }
                this._btnClose = value;
                if (this._btnClose != null)
                {
                    this._btnClose.Click += handler;
                }
            }
        }

        public virtual ToolStripStatusLabel lblPos
        {
            get
            {
                return this._lblPos;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblPos = value;
            }
        }

        public virtual ToolStripStatusLabel lblTile
        {
            get
            {
                return this._lblTile;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblTile = value;
            }
        }

        public virtual ToolStripStatusLabel lblUndo
        {
            get
            {
                return this._lblUndo;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblUndo = value;
            }
        }

        public virtual ToolStripStatusLabel lblVertex
        {
            get
            {
                return this._lblVertex;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._lblVertex = value;
            }
        }

        private ContextMenuStrip ListSelect
        {
            get
            {
                return this._ListSelect;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                ToolStripDropDownClosedEventHandler handler = new ToolStripDropDownClosedEventHandler(this.ListSelect_Close);
                ToolStripItemClickedEventHandler handler2 = new ToolStripItemClickedEventHandler(this.ListSelect_Click);
                if (this._ListSelect != null)
                {
                    this._ListSelect.Closed -= handler;
                    this._ListSelect.ItemClicked -= handler2;
                }
                this._ListSelect = value;
                if (this._ListSelect != null)
                {
                    this._ListSelect.Closed += handler;
                    this._ListSelect.ItemClicked += handler2;
                }
            }
        }

        private clsMap MainMap
        {
            get
            {
                return this._Owner.MainMap;
            }
        }

        public virtual Panel pnlDraw
        {
            get
            {
                return this._pnlDraw;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.pnlDraw_Resize);
                if (this._pnlDraw != null)
                {
                    this._pnlDraw.Resize -= handler;
                }
                this._pnlDraw = value;
                if (this._pnlDraw != null)
                {
                    this._pnlDraw.Resize += handler;
                }
            }
        }

        public virtual StatusStrip ssStatus
        {
            get
            {
                return this._ssStatus;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._ssStatus = value;
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

        internal virtual TableLayoutPanel TableLayoutPanel2
        {
            get
            {
                return this._TableLayoutPanel2;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                this._TableLayoutPanel2 = value;
            }
        }

        internal virtual TabControl tabMaps
        {
            get
            {
                return this._tabMaps;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tabMaps_SelectedIndexChanged);
                if (this._tabMaps != null)
                {
                    this._tabMaps.SelectedIndexChanged -= handler;
                }
                this._tabMaps = value;
                if (this._tabMaps != null)
                {
                    this._tabMaps.SelectedIndexChanged += handler;
                }
            }
        }

        private Timer tmrDraw
        {
            get
            {
                return this._tmrDraw;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tmrDraw_Tick);
                if (this._tmrDraw != null)
                {
                    this._tmrDraw.Tick -= handler;
                }
                this._tmrDraw = value;
                if (this._tmrDraw != null)
                {
                    this._tmrDraw.Tick += handler;
                }
            }
        }

        private Timer tmrDrawDelay
        {
            get
            {
                return this._tmrDrawDelay;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.tmrDrawDelay_Tick);
                if (this._tmrDrawDelay != null)
                {
                    this._tmrDrawDelay.Tick -= handler;
                }
                this._tmrDrawDelay = value;
                if (this._tmrDrawDelay != null)
                {
                    this._tmrDrawDelay.Tick += handler;
                }
            }
        }

        public virtual Timer UndoMessageTimer
        {
            get
            {
                return this._UndoMessageTimer;
            }
            [MethodImpl(MethodImplOptions.Synchronized)]
            set
            {
                EventHandler handler = new EventHandler(this.RemoveUndoMessage);
                if (this._UndoMessageTimer != null)
                {
                    this._UndoMessageTimer.Tick -= handler;
                }
                this._UndoMessageTimer = value;
                if (this._UndoMessageTimer != null)
                {
                    this._UndoMessageTimer.Tick += handler;
                }
            }
        }
    }
}

