// #region License
// // /*
// // The MIT License (MIT)
// //
// // Copyright (c) 2013-2014 The SharpFlame Authors.
// //
// // Permission is hereby granted, free of charge, to any person obtaining a copy
// // of this software and associated documentation files (the "Software"), to deal
// // in the Software without restriction, including without limitation the rights
// // to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// // copies of the Software, and to permit persons to whom the Software is
// // furnished to do so, subject to the following conditions:
// //
// // The above copyright notice and this permission notice shall be included in
// // all copies or substantial portions of the Software.
// //
// // THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// // IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// // FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// // AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// // LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// // OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// // THE SOFTWARE.
// // */
// #endregion
//
using System;
using System.IO;
using System.Linq;
using System.Collections.Generic;
using System.Collections.Specialized;
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Old;

namespace SharpFlame.Gui.Dialogs
{
    public class Settings : Form
    {
        readonly SharpFlameApplication application;

        readonly Button btnOk;
        Button btnAddTilesetDirectory { get; set; }
        Button btnRemoveTilesetDirectory { get; set; }

        Button btnAddObjectDirectory  { get; set; }
        Button btnRemoveObjectDirectory  { get; set; }

        GridView grvTilesets { get; set; }
        GridView grvObjects { get; set; }

        public Settings ()
        {
            Title = "Settings";
            Resizable = true;
            Size = new Size (664, 480);
            Topmost = true;
            ShowInTaskbar = false;
            // Location = new Point (200, 100);

            application = (SharpFlameApplication)Application.Instance;

            var layout = new DynamicLayout ();
            var tabControl = new TabControl ();
            tabControl.TabPages.Add (new TabPage {
                Text = "Directories",
                Content = directories ()
            });
            tabControl.TabPages.Add (new TabPage {
                Text = "General",
                Content = general ()
            });
            tabControl.TabPages.Add (new TabPage {
                Text = "Keyboard",
                Content = keyboard ()
            });
            layout.Add (tabControl, true, true);
            layout.AddSeparateRow(
                null,
                btnOk = new Button { Text = "OK", Size = new Size(80, -1) }
            );

            setBindings ();

            Content = layout;
        }

        void setBindings() {
            var settings = application.Settings;

            btnOk.Click += delegate {
                var result = settings.Save(App.SettingsPath);
                if (result.HasProblems || result.HasWarnings) {
                    new Dialogs.Status (result).Show();
                }
                Close();
            };

            btnAddTilesetDirectory.Click += delegate
            {
                var dialog = new SelectFolderDialog();
                dialog.Directory = settings.OpenPath ?? Directory.GetCurrentDirectory();

                var result = dialog.ShowDialog(ParentWindow);
                if (result == DialogResult.Ok)
                {
                    settings.TilesetDirectories.Add(dialog.Directory);
                }
            };

            btnAddObjectDirectory.Click += delegate
            {
                var dialog = new SelectFolderDialog();
                dialog.Directory = settings.OpenPath ?? Directory.GetCurrentDirectory();

                var result = dialog.ShowDialog(ParentWindow);
                if (result == DialogResult.Ok)
                {
                    settings.ObjectDataDirectories.Add(dialog.Directory);
                }
            };

            btnRemoveTilesetDirectory.Click += delegate
            {
                foreach (var i in grvTilesets.SelectedRows) {
                    if (settings.TilesetDirectories.Count > i) {
                        settings.TilesetDirectories.RemoveAt(i);
                    }
                }
            };

            btnRemoveObjectDirectory.Click += delegate
            {
                foreach (var i in grvObjects.SelectedRows) {
                    if (settings.ObjectDataDirectories.Count > i) {
                        settings.ObjectDataDirectories.RemoveAt(i);
                    }
                }
            };

            // Load initial tileset data.
            {
                var itemsTileset = (GridItemCollection)grvTilesets.DataStore;
                foreach (var item in settings.TilesetDirectories)
                {
                    itemsTileset.Add (new OneColumnGridItem ((string)item));
                }
            }

            // Load initial object data.
            {
                var itemsObjects = (GridItemCollection)grvObjects.DataStore;
                foreach (var item in settings.ObjectDataDirectories)
                {
                    itemsObjects.Add (new OneColumnGridItem ((string)item));
                }
            }

            settings.TilesetDirectories.CollectionChanged += (sender, e) => 
            {
                var itemsTileset = (GridItemCollection)grvTilesets.DataStore;
                if (e.Action == NotifyCollectionChangedAction.Add) {
                    foreach (var item in e.NewItems) {
                        itemsTileset.Add(new OneColumnGridItem((string)item));
                    }

                } else if (e.Action == NotifyCollectionChangedAction.Remove) {
                    var tmpList = new List<OneColumnGridItem>();
                    foreach (var item in itemsTileset) {
                        tmpList.Add((OneColumnGridItem)item);
                    }   

                    foreach (var item in e.OldItems) {
                        var foundItem = tmpList.FirstOrDefault(w => ((OneColumnGridItem)w).Text == (string)item);                       
                        if (foundItem != null) {
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
                if (e.Action == NotifyCollectionChangedAction.Add)
                {
                    foreach (var item in e.NewItems)
                    {
                        itemsObjects.Add (new OneColumnGridItem ((string)item));
                    }

                } else if (e.Action == NotifyCollectionChangedAction.Remove)
                {
                    var tmpList = new List<OneColumnGridItem> ();
                    foreach (var item in itemsObjects)
                    {
                        tmpList.Add ((OneColumnGridItem)item);
                    }   

                    foreach (var item in e.OldItems)
                    {
                        var foundItem = tmpList.FirstOrDefault (w => ((OneColumnGridItem)w).Text == (string)item);                       
                        if (foundItem != null)
                        {
                            tmpList.Remove (foundItem);
                        }
                        var datastoreObjects = new GridItemCollection ();
                        datastoreObjects.AddRange (tmpList);
                        grvObjects.DataStore = datastoreObjects;
                    }
                }
            };
        }

        Panel directories () {
            var panel = new Panel ();
            var layout = new DynamicLayout ();

            grvTilesets = new GridView {
                ShowHeader = false,
                DataStore = new GridItemCollection ()
            };          
            grvTilesets.Columns.Add (new GridColumn {
                HeaderText = "Directories",
                DataCell = new TextBoxCell ("Text"),
                Editable = false
            });

            grvObjects = new GridView {
                ShowHeader = false,
                DataStore = new GridItemCollection ()
            };
            grvObjects.Columns.Add (new GridColumn {
                HeaderText = "Directories",
                DataCell = new TextBoxCell ("Text"),
                Editable = false
            });

            layout.Add (new CheckBox { Text = "Show options after startup." });
            var gboxTilesets = new GroupBox { Text = "Tileset Directories" };
            var layoutTilesets = new DynamicLayout ();
            layoutTilesets.BeginHorizontal ();
            layoutTilesets.Add (grvTilesets, true, true);
            layoutTilesets.BeginVertical ();
            layoutTilesets.Add (TableLayout.AutoSized(btnAddTilesetDirectory = new Button { Text = "Add", Size = new Size(80, -1) }));
            layoutTilesets.Add (TableLayout.AutoSized(btnRemoveTilesetDirectory = new Button { Text = "Remove", Size = new Size(80, -1) }));
            layoutTilesets.EndVertical ();
            layoutTilesets.EndHorizontal ();
            gboxTilesets.Content = layoutTilesets;
            layout.Add (gboxTilesets, true, true);

            var gboxObjects = new GroupBox { Text = "Object Data Directories" };
            var layoutObjects = new DynamicLayout ();
            layoutObjects.BeginHorizontal ();
            layoutObjects.Add (grvObjects, true, true);
            layoutObjects.BeginVertical ();
            layoutObjects.Add (TableLayout.AutoSized(btnAddObjectDirectory = new Button { Text = "Add", Size = new Size(80, -1) }));
            layoutObjects.Add (TableLayout.AutoSized(btnRemoveObjectDirectory = new Button { Text = "Remove", Size = new Size(80, -1) }));
            layoutObjects.EndVertical ();
            layoutObjects.EndHorizontal ();
            gboxObjects.Content = layoutObjects;
            layout.Add (gboxObjects, true, true);

            panel.Content = layout;
            return panel;
        }

        Panel general () {
            var panel = new Panel ();
            var layout = new DynamicLayout ();
            layout.Add (null);

            panel.Content = layout;
            return panel;
        }

        Panel keyboard () {
            var panel = new Panel ();
            var layout = new DynamicLayout ();
            layout.Add (null);

            panel.Content = layout;
            return panel;
        }

        private class OneColumnGridItem 
        {
            public string Text { get; private set; }

            public OneColumnGridItem(string text) {
                Text = text;
            }
        }
    }
}

