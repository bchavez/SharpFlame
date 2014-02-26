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

namespace SharpFlame.Gui.Sections
{
    public class ObjectTab : Panel
    {
        public ObjectTab ()
        {
            var mainLayout = new DynamicLayout ();

            var nLayout0 = new DynamicLayout ();
            nLayout0.Padding = Padding.Empty;
            var nLayout1 = new DynamicLayout ();
            nLayout1.AddRow(
                new Label { Text = "Type:", VerticalAlign = VerticalAlign.Middle },
                new Label { Text = "A0CyborgFactory (Cyborg Factory)", VerticalAlign = VerticalAlign.Middle });
           

            nLayout0.AddRow (nLayout1);

            PlayerSelector playerSelector;
            if (Generator.IsWinForms)
            {
                playerSelector = new PlayerSelectorWinforms (Constants.PlayerCountMax);
            } else
            {
                playerSelector = new PlayerSelector (Constants.PlayerCountMax);
            }
            playerSelector.SelectedPlayer = "S";

            var nLayout2 = new DynamicLayout ();
            nLayout2.Padding = Padding.Empty;
            nLayout2.AddRow(new Label { Text = "Player", VerticalAlign = VerticalAlign.Middle }, 
                            TableLayout.AutoSized(playerSelector)
            );

            nLayout0.AddRow (nLayout2);

            var nLayout3 = new DynamicLayout ();
            var nLayout4 = new DynamicLayout ();
            nLayout4.Padding = Padding.Empty;
            nLayout4.AddRow(TableLayout.AutoSized(new NumericUpDown { MaxValue = 360, MinValue = 0, Value = 0, Size = new Size(-1, -1) }),
                            new Label { Text = "(0-359)", VerticalAlign = VerticalAlign.Middle });

            nLayout3.AddRow(new Label { Text = "Rotation:", VerticalAlign = VerticalAlign.Middle },
                            nLayout4,
                            new Button { Text = "Realign" });                            

            nLayout3.AddRow(new Label { Text = "ID:", VerticalAlign = VerticalAlign.Middle },
                            new TextBox { Text = "1200", Enabled = false },
                            new Button { Text = "Flatten Terrain" });

            nLayout3.AddRow(new Label { Text = "Label:", VerticalAlign = VerticalAlign.Middle },
                            new TextBox {});

            nLayout3.AddRow(new Label { Text = "Health%:", VerticalAlign = VerticalAlign.Middle },
                            TableLayout.AutoSized(new NumericUpDown { MinValue = 0, MaxValue = 100, Value = 100, Size = new Size(-1, -1) }));

            nLayout0.AddRow (nLayout3);

            mainLayout.AddRow (null, nLayout0, null);

            var nLayout5 = new DynamicLayout ();
            nLayout5.AddRow(TableLayout.AutoSized (new Button { Text = "Convert Templates To Design" }),
                            new CheckBox { Text = "Designables Only", Checked = true });
            mainLayout.AddRow (null, nLayout5, null);

            var nLayout6 = new DynamicLayout ();
            nLayout6.AddRow (new Label { Text = "Type", VerticalAlign = VerticalAlign.Middle }, new ComboBox { });
            nLayout6.AddRow (new Label { Text = "Body", VerticalAlign = VerticalAlign.Middle }, new ComboBox { });
            nLayout6.AddRow (new Label { Text = "Propulsion", VerticalAlign = VerticalAlign.Middle }, new ComboBox { });

            var radio0 = new RadioButton { Text = "0" };
            var radio1 = new RadioButton (radio0) { Text = "1" };
            var radio2 = new RadioButton (radio1) { Text = "2" };
            var radio3 = new RadioButton (radio2) { Text = "3" };

            var nLayout7 = new DynamicLayout();
            nLayout7.Padding = Padding.Empty;
            nLayout7.AddRow(new Label { Text = "Turrets", VerticalAlign = VerticalAlign.Middle }, radio1);
            nLayout6.AddRow(nLayout7, new ComboBox {});
    
            var nLayout8 = new DynamicLayout();
            nLayout8.Padding = Padding.Empty;
            nLayout8.AddRow(radio0, radio2);
            nLayout6.AddRow(nLayout8, new ComboBox {});

            nLayout6.AddRow(radio3, new ComboBox {});

            mainLayout.AddRow(null, nLayout6, null);
            mainLayout.Add (null);

            Content = mainLayout;
        }
    }
}

