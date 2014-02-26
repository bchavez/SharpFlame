#region License
// /*
// The MIT License (MIT)
//
// Copyright (c) 2013-2014 The SharpFlame Authors.
//
// Permission is hereby granted, free of charge, to any person obtaining a copy
// of this software and associated documentation files (the "Software"), to deal
// in the Software without restriction, including without limitation the rights
// to use, copy, modify, merge, publish, distribute, sublicense, and/or sell
// copies of the Software, and to permit persons to whom the Software is
// furnished to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in
// all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED "AS IS", WITHOUT WARRANTY OF ANY KIND, EXPRESS OR
// IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF MERCHANTABILITY,
// FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE
// AUTHORS OR COPYRIGHT HOLDERS BE LIABLE FOR ANY CLAIM, DAMAGES OR OTHER
// LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM,
// OUT OF OR IN CONNECTION WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN
// THE SOFTWARE.
// */
#endregion

using System;
using Eto.Forms;
using Eto.Drawing;
using SharpFlame.Core;
using SharpFlame.Gui.Controls;
using SharpFlame.Gui.UiOptions;

namespace SharpFlame.Gui.Sections
{
	public class PlaceObjectsTab : Panel
	{
        readonly SearchBox filterText;
        readonly Button btnRotation0;
        readonly Button btnRotation90;
        readonly Button btnRotation180;
        readonly Button btnRotation270;

        readonly NumericUpDown nudRotation;

		public PlaceObjectsTab ()
		{
            PlayerSelector playerSelector;
            if (Generator.IsWinForms)
            {
                playerSelector = new PlayerSelectorWinforms (Constants.PlayerCountMax);
            } else
            {
                playerSelector = new PlayerSelector (Constants.PlayerCountMax);
            }
            playerSelector.SelectedPlayer = "S";

            var topLayout = new DynamicLayout ();
            var nLayout1 = new DynamicLayout { Padding = Padding.Empty };
            nLayout1.AddRow(new Label { Text = "Player", VerticalAlign = VerticalAlign.Middle }, 
                            playerSelector, 
                            TableLayout.AutoSized(new Button { Text = "Select Player Objects" }, centered: true)
            );
            nLayout1.AddRow (null, 
                             null, 
                             ToolGroupBox()
            );

			topLayout.AddRow (null, nLayout1, null);

            var nLayout2 = new DynamicLayout { Padding = Padding.Empty, Spacing = Size.Empty };
            var nLayout3 = new DynamicLayout { Spacing = Size.Empty };
            var nLayout4 = new DynamicLayout ();

            nLayout2.AddSeparateRow (TableLayout.AutoSized (btnRotation0 = new Button { Text = "0" }),
                                     TableLayout.AutoSized (btnRotation90 = new Button { Text = "90" }),
                                     TableLayout.AutoSized (btnRotation180 = new Button { Text = "180" }),
                                     TableLayout.AutoSized (btnRotation270 = new Button { Text = "270" }));

            nLayout2.AddSeparateRow(new Label { Text = "Rotation:", VerticalAlign = VerticalAlign.Middle },
                            nudRotation = new NumericUpDown { MinValue = 0, MaxValue = 360, Value = 0, Size = new Size(-1, -1) },
                            new CheckBox { Text = "Random" }
            );
            var gbRotation = new GroupBox { Text = "Rotation" };
            gbRotation.Content = nLayout2;


            nLayout3.Add (new CheckBox { Text = "Rotate Footprints" });
            nLayout3.Add (new CheckBox { Text = "Automatic Walls" });
            nLayout4.AddRow (gbRotation, nLayout3);

            topLayout.AddRow (null, nLayout4, null);

            var nLayout5 = new DynamicLayout { Padding = Padding.Empty };
            filterText = new SearchBox { PlaceholderText = "Filter", Size = new Size(235, 27) };
            nLayout5.AddRow (filterText, TableLayout.AutoSized(new Button { Text = "Select this Objects" }));

            var tabControl = new TabControl ();
            tabControl.TabPages.Add (new TabPage { Text = "Features", Content = FeaturesPanel() });
            tabControl.TabPages.Add (new TabPage { Text = "Structures", Content = StructuresPanel() });
            tabControl.TabPages.Add (new TabPage { Text = "Droids", Content = DroidsPanel() });

            var mainLayout = new DynamicLayout ();
            mainLayout.Add (topLayout);
            mainLayout.AddRow (nLayout5);
			mainLayout.Add (tabControl);

            setBindings ();

			Content = mainLayout;
		}

        void setBindings() {
            // Rotation buttons
            btnRotation0.Click += delegate {
                nudRotation.Value = 0D;
            };

            btnRotation90.Click += delegate {
                nudRotation.Value = 90D;
            };


            btnRotation180.Click += delegate {
                nudRotation.Value = 180D;
            };

            btnRotation270.Click += delegate {
                nudRotation.Value = 270D;
            };

            // Set Mousetool, when we are shown.
            Shown += delegate {
                App.UiOptions.MouseTool = MouseTool.Default;
            };
        }

        class MyGridItem
        {
            public int Row { get; set; }

            public string InternalName
            {
                get;
                set;
            }

            public string InGameName
            {
                get;
                set;
            }

            public Color Color { get; set; }
            // used for owner-drawn cells
            public MyGridItem(Random rand, int row, string prefix)
            {
                // initialize to random values
                Row = row;
                InternalName = string.Format("{0} Internal Row {1}", prefix, row);
                InGameName = string.Format("{0} In-Game Row {1}", prefix, row);

                Color = Color.FromArgb(rand.Next(256), rand.Next(256), rand.Next(256));
            }
        }

        DynamicLayout FeaturesPanel() {
            var mainLayout = new DynamicLayout ();

            var control = new GridView { Size = new Size(300, 100) };
            control.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
            control.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });

            var items = new GridItemCollection();
            var rand = new Random();
            for (int i = 0; i < 10000; i++)
            {
                items.Add(new MyGridItem(rand, i, "Feature"));
            }
            control.DataStore = items;

            filterText.TextChanged += (s, e) =>
            {
                var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Set the filter delegate on the GridView
                control.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o =>
                {
                    var i = o as MyGridItem;
                    var matches = true;

                    // Every item in the split filter string should be within the Text property
                    foreach (var filterItem in filterItems) {
                        if (i.InternalName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1 && 
                            i.InGameName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
                        {
                            matches = false;
                            break;
                        }                       
                    }

                    return matches;
                };
            };

            mainLayout.Add (control);

            return mainLayout;
        }

        DynamicLayout StructuresPanel() {
            var mainLayout = new DynamicLayout ();
            var control = new GridView { Size = new Size(300, 100) };
            control.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true });
            control.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true });

            var items = new GridItemCollection();
            var rand = new Random();
            for (int i = 0; i < 10000; i++)
            {
                items.Add(new MyGridItem(rand, i, "Structure"));
            }
            control.DataStore = items;

            filterText.TextChanged += (s, e) =>
            {
                var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Set the filter delegate on the GridView
                control.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o =>
                {
                    var i = o as MyGridItem;
                    var matches = true;

                    // Every item in the split filter string should be within the Text property
                    foreach (var filterItem in filterItems) {
                        if (i.InternalName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1 && 
                            i.InGameName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
                        {
                            matches = false;
                            break;
                        }                       
                    }

                    return matches;
                };
            };

            mainLayout.Add (control);
            return mainLayout;
        }

        DynamicLayout DroidsPanel() {
            var mainLayout = new DynamicLayout ();
            var control = new GridView { Size = new Size(300, 100) };
            control.Columns.Add(new GridColumn { HeaderText = "Internal Name", DataCell = new TextBoxCell("InternalName"), Editable = false, Sortable = true, Width = 200 });
            control.Columns.Add(new GridColumn { HeaderText = "In-Game Name", DataCell = new TextBoxCell("InGameName"), Editable = false, Sortable = true, Width = 200 });

            var items = new GridItemCollection();
            var rand = new Random();
            for (int i = 0; i < 10000; i++)
            {
                items.Add(new MyGridItem(rand, i, "Droids"));
            }
            control.DataStore = items;

            filterText.TextChanged += (s, e) =>
            {
                var filterItems = (filterText.Text ?? "").Split(new char[] { ' ' }, StringSplitOptions.RemoveEmptyEntries);

                // Set the filter delegate on the GridView
                control.Filter = (filterItems.Length == 0) ? (Func<object, bool>)null : o =>
                {
                    var i = o as MyGridItem;
                    var matches = true;

                    // Every item in the split filter string should be within the Text property
                    foreach (var filterItem in filterItems) {
                        if (i.InternalName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1 && 
                            i.InGameName.IndexOf(filterItem, StringComparison.CurrentCultureIgnoreCase) == -1)
                        {
                            matches = false;
                            break;
                        }                       
                    }

                    return matches;
                };
            };

            mainLayout.Add (control);
            return mainLayout;
        }


        Control ToolGroupBox() {
            var control = new GroupBox { Text = "Tool" };
            var nLayout1 = new DynamicLayout ();
            var rbList = new RadioButtonList { Orientation = RadioButtonListOrientation.Vertical };
            rbList.Items.Add(new ListItem { Text = "Place" });
            rbList.Items.Add(new ListItem { Text = "Lines" });
            rbList.SelectedIndex = 0;
            nLayout1.Add (rbList);

            control.Content = nLayout1;
            return control;
        }
	}
}

