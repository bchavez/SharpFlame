 #region License
  /*
  The MIT License (MIT)
 
  Copyright (c) 2013-2014 The SharpFlame Authors.
 
  Permission is hereby granted, free of charge, to any person obtaining a copy
  of this software and associated documentation files (the "Software"), to deal
  in the Software without restriction, including without limitation the rights
  to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
  copies of the Software, and to permit persons to whom the Software is
  furnished to do so, subject to the following conditions:
 
  The above copyright notice and this permission notice shall be included in
  all copies or substantial portions of the Software.
 
  THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
  IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
  FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
  AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
  LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
  OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
  THE SOFTWARE.
  */
 #endregion

using System;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using Eto;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;
using SharpFlame.Core.Domain.Colors;
using SharpFlame.Old;
using sd = System.Drawing;

namespace SharpFlame.Gui.Dialogs
{
    public class Settings : Form
    {
        private readonly Button btnOk;
        private Button btnAddTilesetDirectory;
        private Button btnRemoveTilesetDirectory;

        private Button btnAddObjectDirectory;
        private Button btnRemoveObjectDirectory;
        private Button btnSelectFont;

        private GridView grvTilesets;
        private GridView grvObjects;
        private GridView grvKeyboard;

        private Drawable drawMinimapCliffColour;
        private Drawable drawMinimapObjectColour;

        private Label lblFont;

        private CheckBox chkStartupShowOptions;

        private CheckBox chkAutoSaveEnabled;
        private CheckBox chkAutoSaveCompress;

        private CheckBox chkMipmapsGenerate;
        private CheckBox chkMipmapsHardware;

        private CheckBox chkMinimapTeamColours;
        private CheckBox chkMinimapExceptFeatures;

        private CheckBox chkPointerDirect;
        private CheckBox chkPickerTextureOrientations;

        private NumericUpDown nudUndoSteps;

        private NumericUpDown nudAutosaveChanges;
        private NumericUpDown nudAutosaveInterval;

        private NumericUpDown nudGraphicsMapViewColours;
        private NumericUpDown nudGraphicsMapViewDepth;
        private NumericUpDown nudGraphicsTextureViewColours;
        private NumericUpDown nudGraphicsTextureViewDepth;

        private NumericUpDown nudMinimapSize;
        private TextBox tbFoV;


        public Settings()
        {
            Title = "Settings";
            Resizable = true;
            Size = new Size(664, 480);
            Topmost = true;
            ShowInTaskbar = false;
            // Location = new Point (200, 100);

            var layout = new DynamicLayout();
            var tabControl = new TabControl();
            tabControl.TabPages.Add(new TabPage
                {
                    Text = "Directories",
                    Content = directories()
                });
            tabControl.TabPages.Add(new TabPage
                {
                    Text = "General",
                    Content = general()
                });
            tabControl.TabPages.Add(new TabPage
                {
                    Text = "Keyboard",
                    Content = keyboard()
                });
            layout.Add(tabControl, true, true);
            layout.AddSeparateRow(
                null,
                btnOk = new Button {Text = "OK", Size = new Size(80, -1)}
                );

            SetupEventHandlers();

            Content = layout;
        }

        private void SetupEventHandlers()
        {
            var settings = App.Settings;

            btnOk.Click += delegate
                {
                    var result = settings.Save(App.SettingsPath);
                    if( result.HasProblems || result.HasWarnings )
                    {
                        new Dialogs.Status(result).Show();
                    }
                    Close();
                };

            btnAddTilesetDirectory.Click += delegate
                {
                    var dialog = new SelectFolderDialog();
                    dialog.Directory = settings.OpenPath ?? Directory.GetCurrentDirectory();

                    var result = dialog.ShowDialog(ParentWindow);
                    if( result == DialogResult.Ok )
                    {
                        settings.TilesetDirectories.Add(dialog.Directory);
                    }
                };

            btnAddObjectDirectory.Click += delegate
                {
                    var dialog = new SelectFolderDialog();
                    dialog.Directory = settings.OpenPath ?? Directory.GetCurrentDirectory();

                    var result = dialog.ShowDialog(ParentWindow);
                    if( result == DialogResult.Ok )
                    {
                        settings.ObjectDataDirectories.Add(dialog.Directory);
                    }
                };

            btnRemoveTilesetDirectory.Click += delegate
                {
                    foreach( var i in grvTilesets.SelectedRows )
                    {
                        if( settings.TilesetDirectories.Count > i )
                        {
                            settings.TilesetDirectories.RemoveAt(i);
                        }
                    }
                };

            btnRemoveObjectDirectory.Click += delegate
                {
                    foreach( var i in grvObjects.SelectedRows )
                    {
                        if( settings.ObjectDataDirectories.Count > i )
                        {
                            settings.ObjectDataDirectories.RemoveAt(i);
                        }
                    }
                };

            // Load initial tileset data.
            {
                var itemsTileset = (GridItemCollection)grvTilesets.DataStore;
                foreach( var item in settings.TilesetDirectories )
                {
                    itemsTileset.Add(new OneColumnGridItem((string)item));
                }
            }

            // Load initial object data.
            {
                var itemsObjects = (GridItemCollection)grvObjects.DataStore;
                foreach( var item in settings.ObjectDataDirectories )
                {
                    itemsObjects.Add(new OneColumnGridItem((string)item));
                }
            }

            settings.TilesetDirectories.CollectionChanged += (sender, e) =>
                {
                    var itemsTileset = (GridItemCollection)grvTilesets.DataStore;
                    if( e.Action == NotifyCollectionChangedAction.Add )
                    {
                        foreach( var item in e.NewItems )
                        {
                            itemsTileset.Add(new OneColumnGridItem((string)item));
                        }

                    }
                    else if( e.Action == NotifyCollectionChangedAction.Remove )
                    {
                        var tmpList = new List<OneColumnGridItem>();
                        foreach( var item in itemsTileset )
                        {
                            tmpList.Add((OneColumnGridItem)item);
                        }

                        foreach( var item in e.OldItems )
                        {
                            var foundItem = tmpList.FirstOrDefault(w => ((OneColumnGridItem)w).Text == (string)item);
                            if( foundItem != null )
                            {
                                tmpList.Remove(foundItem);
                            }
                            var datastoreTilesets = new GridItemCollection();
                            datastoreTilesets.AddRange(tmpList);
                            grvTilesets.DataStore = datastoreTilesets;
                        }
                    }
                };

            settings.ObjectDataDirectories.CollectionChanged += (sender, e) =>
                {
                    var itemsObjects = (GridItemCollection)grvObjects.DataStore;
                    if( e.Action == NotifyCollectionChangedAction.Add )
                    {
                        foreach( var item in e.NewItems )
                        {
                            itemsObjects.Add(new OneColumnGridItem((string)item));
                        }

                    }
                    else if( e.Action == NotifyCollectionChangedAction.Remove )
                    {
                        var tmpList = new List<OneColumnGridItem>();
                        foreach( var item in itemsObjects )
                        {
                            tmpList.Add((OneColumnGridItem)item);
                        }

                        foreach( var item in e.OldItems )
                        {
                            var foundItem = tmpList.FirstOrDefault(w => ((OneColumnGridItem)w).Text == (string)item);
                            if( foundItem != null )
                            {
                                tmpList.Remove(foundItem);
                            }
                            var datastoreObjects = new GridItemCollection();
                            datastoreObjects.AddRange(tmpList);
                            grvObjects.DataStore = datastoreObjects;
                        }
                    }
                };

            // keyboard
            grvKeyboard.MouseUp += (sender, e) => 
            {
                Console.WriteLine("mouse up");
                var name = ((KeyboardGridItem)grvKeyboard.SelectedItem).Name;
                var key = App.Settings.Keyboard.Keys[name];
                var dialog = new Dialogs.KeyInput { Key = key.Key };
                dialog.ShowDialog(Application.Instance.MainForm);
            };

            drawMinimapCliffColour.MouseDown += (sender, e) =>
                {
                    var dialog = new ColorDialog();
                    var result = dialog.ShowDialog(ParentWindow);
                    if( result == DialogResult.Ok )
                    {
                        drawMinimapCliffColour.BackgroundColor = dialog.Color;
                        App.Settings.MinimapCliffColour = new Rgba(dialog.Color);
                    }
                };

            drawMinimapObjectColour.MouseDown += (sender, e) =>
                {
                    var dialog = new ColorDialog();
                    var result = dialog.ShowDialog(ParentWindow);
                    if( result == DialogResult.Ok )
                    {
                        drawMinimapObjectColour.BackgroundColor = dialog.Color;
                        App.Settings.MinimapSelectedObjectsColour = new Rgba(dialog.Color);
                    }
                };

            btnSelectFont.Click += delegate
                {
                    var fontStyle = App.Settings.FontBold ? FontStyle.Bold : FontStyle.None;
                    if( App.Settings.FontItalic )
                    {
                        fontStyle |= FontStyle.Italic;
                    }
                    var dialog = new FontDialog
                        {
                            Font = new Font(App.Settings.FontFamily.Name, App.Settings.FontSize, fontStyle)
                        };
                    var result = dialog.ShowDialog(ParentWindow);
                    if( result == DialogResult.Ok )
                    {
                        var resultFont = dialog.Font;
                        App.Settings.FontFamily = new sd.FontFamily(resultFont.FamilyName);
                        App.Settings.FontSize = resultFont.Size;
                        App.Settings.FontBold = resultFont.Bold;
                        App.Settings.FontItalic = resultFont.Italic;
                    }
                };

            App.Settings.PropertyChanged += (object sender, PropertyChangedEventArgs e) =>
                {
                    if( e.PropertyName.StartsWith("Font") )
                    {
                        setFontLabelText();
                    }
                };

            // Checkboxes
            chkStartupShowOptions.Bind(r => r.Checked, App.Settings, t => t.ShowOptionsAtStartup);
            chkAutoSaveEnabled.Bind(r => r.Checked, App.Settings, t => t.AutoSaveEnabled);
            chkAutoSaveCompress.Bind(r => r.Checked, App.Settings, t => t.AutoSaveCompress);
            chkMipmapsGenerate.Bind(r => r.Checked, App.Settings, t => t.Mipmaps);
            chkMipmapsHardware.Bind(r => r.Checked, App.Settings, t => t.MipmapsHardware);
            chkMinimapTeamColours.Bind(r => r.Checked, App.Settings, t => t.MinimapTeamColours);
            chkMinimapExceptFeatures.Bind(r => r.Checked, App.Settings, t => t.MinimapTeamColoursExceptFeatures);
            chkPointerDirect.Bind(r => r.Checked, App.Settings, t => t.DirectPointer);
            chkPickerTextureOrientations.Bind(r => r.Checked, App.Settings, t => t.PickOrientation);

            // NumericUpDowns
            nudUndoSteps.Bind(r => r.Value, App.Settings, s => s.UndoLimit);
            nudAutosaveChanges.Bind(r => r.Value, App.Settings, s => s.AutoSaveMinChanges);
            nudAutosaveInterval.Bind(r => r.Value, App.Settings, s => s.AutoSaveMinIntervalSeconds);
            nudGraphicsMapViewColours.Bind(r => r.Value, App.Settings, s => s.MapViewBPP);
            nudGraphicsMapViewDepth.Bind(r => r.Value, App.Settings, s => s.MapViewDepth);
            nudGraphicsTextureViewColours.Bind(r => r.Value, App.Settings, s => s.TextureViewBPP);
            nudGraphicsTextureViewDepth.Bind(r => r.Value, App.Settings, s => s.TextureViewDepth);
            nudMinimapSize.Bind(r => r.Value, App.Settings, s => s.MinimapSize);
            tbFoV.Bind(r => r.Text, App.Settings, s => s.FOVDefault);

        }

        private void setFontLabelText()
        {
            lblFont.Text = string.Format("{0}, {1}, {2}{3}",
                App.Settings.FontFamily.Name,
                (int)App.Settings.FontSize,
                App.Settings.FontBold ? "B" : "",
                App.Settings.FontItalic ? "I" : ""
                );
        }

        private Panel directories()
        {
            var panel = new Panel();
            var layout = new DynamicLayout();

            grvTilesets = new GridView
                {
                    ShowHeader = false,
                    DataStore = new GridItemCollection()
                };
            grvTilesets.Columns.Add(new GridColumn
                {
                    HeaderText = "Directories",
                    DataCell = new TextBoxCell("Text"),
                    Editable = false
                });

            grvObjects = new GridView
                {
                    ShowHeader = false,
                    DataStore = new GridItemCollection()
                };
            grvObjects.Columns.Add(new GridColumn
                {
                    HeaderText = "Directories",
                    DataCell = new TextBoxCell("Text"),
                    Editable = false
                });

            layout.Add(chkStartupShowOptions = new CheckBox {Text = "Show options after startup."});
            var gboxTilesets = new GroupBox {Text = "Tileset Directories"};
            var layoutTilesets = new DynamicLayout();
            layoutTilesets.BeginHorizontal();
            layoutTilesets.Add(grvTilesets, true, true);
            layoutTilesets.BeginVertical();
            layoutTilesets.Add(TableLayout.AutoSized(btnAddTilesetDirectory = new Button {Text = "Add", Size = new Size(80, -1)}));
            layoutTilesets.Add(TableLayout.AutoSized(btnRemoveTilesetDirectory = new Button {Text = "Remove", Size = new Size(80, -1)}));
            layoutTilesets.EndVertical();
            layoutTilesets.EndHorizontal();
            gboxTilesets.Content = layoutTilesets;
            layout.Add(gboxTilesets, true, true);

            var gboxObjects = new GroupBox {Text = "Object Data Directories"};
            var layoutObjects = new DynamicLayout();
            layoutObjects.BeginHorizontal();
            layoutObjects.Add(grvObjects, true, true);
            layoutObjects.BeginVertical();
            layoutObjects.Add(TableLayout.AutoSized(btnAddObjectDirectory = new Button {Text = "Add", Size = new Size(80, -1)}));
            layoutObjects.Add(TableLayout.AutoSized(btnRemoveObjectDirectory = new Button {Text = "Remove", Size = new Size(80, -1)}));
            layoutObjects.EndVertical();
            layoutObjects.EndHorizontal();
            gboxObjects.Content = layoutObjects;
            layout.Add(gboxObjects, true, true);

            panel.Content = layout;
            return panel;
        }

        private Panel general()
        {
            var gboxUndo = new GroupBox {Text = "Undo"};
            var nLayout0 = new DynamicLayout();
            nLayout0.AddRow(new Label
                {
                    Text = "Maximum stored steps:",
                    VerticalAlign = VerticalAlign.Middle
                },
                nudUndoSteps = new NumericUpDown {MinValue = 0, MaxValue = 512, Size = new Size(-1, -1)}
                );
            gboxUndo.Content = nLayout0;

            var gboxAutosave = new GroupBox {Text = "Autosave"};
            var nLayout1 = new DynamicLayout {Padding = Padding.Empty};
            nLayout1.BeginHorizontal();
            nLayout1.BeginVertical();
            nLayout1.Add(chkAutoSaveEnabled = new CheckBox {Text = "Enabled"});
            nLayout1.Add(new Label
                {
                    Text = "Number of changes:",
                    VerticalAlign = VerticalAlign.Middle
                });
            nLayout1.Add(new Label
                {
                    Text = "Time interval (s):",
                    VerticalAlign = VerticalAlign.Middle
                });
            nLayout1.EndVertical();
            nLayout1.BeginVertical();
            nLayout1.Add(chkAutoSaveCompress = new CheckBox {Text = "Use compression"});
            nLayout1.Add(nudAutosaveChanges = new NumericUpDown {MinValue = 0, MaxValue = 512, Size = new Size(-1, -1)});
            nLayout1.Add(nudAutosaveInterval = new NumericUpDown {MinValue = 0, MaxValue = 512, Size = new Size(-1, -1)});
            nLayout1.EndVertical();
            nLayout1.EndHorizontal();
            gboxAutosave.Content = nLayout1;

            var gboxDisplayFont = new GroupBox {Text = "Display Font"};
            var nLayout2 = new DynamicLayout();
            nLayout2.AddRow(
                lblFont = new Label {Text = "", VerticalAlign = VerticalAlign.Middle},
                null,
                TableLayout.AutoSized(btnSelectFont = new Button {Text = "Select", Size = new Size(80, -1)})
                );
            setFontLabelText();
            gboxDisplayFont.Content = nLayout2;

            var gboxGrahpics = new GroupBox {Text = "Graphics"};
            var nLayout3 = new DynamicLayout();
            var nLayout4 = new DynamicLayout {Padding = Padding.Empty};
            var nLayout5 = new DynamicLayout {Padding = Padding.Empty};
            nLayout4.AddRow(
                chkMipmapsGenerate = new CheckBox {Text = "Generate mipmaps"},
                chkMipmapsHardware = new CheckBox {Text = "Use Hardware"}
                );
            nLayout5.AddRow(
                null,
                new Label
                    {
                        Text = "Colour Bits",
                        VerticalAlign = VerticalAlign.Middle
                    },
                new Label
                    {
                        Text = "Depth Bits",
                        VerticalAlign = VerticalAlign.Middle
                    }
                );
            nLayout5.AddRow(
                new Label
                    {
                        Text = "Map View",
                        VerticalAlign = VerticalAlign.Middle
                    },
                nudGraphicsMapViewColours = new NumericUpDown {MinValue = 8, MaxValue = 32, Size = new Size(-1, -1)},
                nudGraphicsMapViewDepth = new NumericUpDown {MinValue = 8, MaxValue = 32, Size = new Size(-1, -1)}
                );
            nLayout5.AddRow(
                new Label
                    {
                        Text = "Textures View",
                        VerticalAlign = VerticalAlign.Middle
                    },
                nudGraphicsTextureViewColours = new NumericUpDown {MinValue = 8, MaxValue = 32, Size = new Size(-1, -1)},
                nudGraphicsTextureViewDepth = new NumericUpDown {MinValue = 8, MaxValue = 32, Size = new Size(-1, -1)}
                );
            nLayout3.Add(nLayout4);
            nLayout3.Add(nLayout5);
            gboxGrahpics.Content = nLayout3;

            var gboxMinimap = new GroupBox {Text = "Minimap"};
            var nLayout6 = new DynamicLayout();
            nLayout6.AddRow(
                new Label {Text = "Size", VerticalAlign = VerticalAlign.Middle},
                nudMinimapSize = new NumericUpDown {MinValue = 0, MaxValue = 512, Size = new Size(-1, -1)}
                );
            nLayout6.AddRow(
                chkMinimapTeamColours = new CheckBox
                    {
                        Text = "Use team colours",
                    },
                chkMinimapExceptFeatures = new CheckBox
                    {
                        Text = "Except for features",
                    }
                );
            var tblCliffColourBorder = new TableLayout(1, 1)
                {
                    BackgroundColor = Colors.Black,
                    Spacing = Size.Empty,
                    Padding = new Padding(1, 1)
                };
            tblCliffColourBorder.Add(
                drawMinimapCliffColour = new Drawable
                    {
                        Style = "direct",
                        BackgroundColor = App.Settings.MinimapCliffColour.ToEto(),
                        Size = new Size(50, 27)
                    }
                , 0, 0, false, false);

            var tblObjectHighlightBorder = new TableLayout(1, 1)
                {
                    BackgroundColor = Colors.Black,
                    Spacing = Size.Empty,
                    Padding = new Padding(1, 1)
                };
            tblObjectHighlightBorder.Add(
                drawMinimapObjectColour = new Drawable
                    {
                        Style = "direct",
                        BackgroundColor = App.Settings.MinimapSelectedObjectsColour.ToEto(),
                        Size = new Size(50, 27)
                    }
                , 0, 0, false, false);

            nLayout6.AddRow(
                new Label {Text = "Cliff Colour"},
                TableLayout.AutoSized(tblCliffColourBorder)
                );
            nLayout6.AddRow(
                new Label {Text = "Object Highlight"},
                TableLayout.AutoSized(tblObjectHighlightBorder)
                );
            gboxMinimap.Content = nLayout6;

            var gboxPointer = new GroupBox {Text = "Pointer"};
            var nLayout7 = new DynamicLayout();
            nLayout7.Add(chkPointerDirect = new CheckBox {Text = "Direct"});
            gboxPointer.Content = nLayout7;

            var gboxFOV = new GroupBox {Text = "Field Of View"};
            var nLayout8 = new DynamicLayout();
            nLayout8.AddRow(
                new Label {Text = "Default Multiplier", VerticalAlign = VerticalAlign.Middle},
                tbFoV = new TextBox {}
                );
            gboxFOV.Content = nLayout8;

            var gobxPicker = new GroupBox {Text = "Picker"};
            var nLayout9 = new DynamicLayout();
            nLayout9.Add(
                chkPickerTextureOrientations = new CheckBox {Text = "Capture texture orientations."}
                );
            gobxPicker.Content = nLayout9;

            var layout = new DynamicLayout();
            layout.BeginHorizontal();
            layout.BeginVertical(); // Column1
            layout.Add(gboxUndo, true, false);
            layout.Add(gboxAutosave, true, false);
            layout.Add(gboxDisplayFont, true, false);
            layout.Add(gboxGrahpics, true, false);
            layout.Add(null);
            layout.EndVertical();

            layout.BeginVertical(); // Column2
            layout.Add(gboxMinimap);
            layout.Add(gboxPointer);
            layout.Add(gboxFOV);
            layout.Add(gobxPicker);
            layout.Add(null);
            layout.EndVertical();
            layout.EndBeginHorizontal();

            var panel = new Panel();
            panel.Content = layout;
            return panel;
        }

        private Panel keyboard()
        {
            var panel = new Panel();
            var layout = new DynamicLayout();
            grvKeyboard = new GridView {
                DataStore = new GridItemCollection ()
            };
            grvKeyboard.Columns.Add (new GridColumn {
                HeaderText = "Name",
               DataCell = new TextBoxCell ("Name"),
                Editable = false
            });
            grvKeyboard.Columns.Add (new GridColumn {
                HeaderText = "Key",
                DataCell = new TextBoxCell ("Key"),
               Editable = false
            });

            var store = new GridItemCollection ();
            grvKeyboard.DataStore = store;
            var i = 0;
            foreach (KeyValuePair<string, KeyboardKey> pair in App.Settings.Keyboard.Keys)
            {
                store.Add(new KeyboardGridItem(
                    pair.Key, 
                    pair.Value.Invalid ? "!! " : "" + pair.Value.Key.ToShortcutString()
                ));
            }
            layout.Add (grvKeyboard, true, true);

            panel.Content = layout;
            return panel;
        }

        private class OneColumnGridItem
        {
            public string Text { get; private set; }

            public OneColumnGridItem(string text)
            {
                Text = text;
            }
        }

        private class KeyboardGridItem 
        {
            public string Name { get; private set;} 
            public string Key { get; private set;} 

            public KeyboardGridItem(string name, string key)
            {
                Name = name;
                Key = key;
           }
        }
    }
}