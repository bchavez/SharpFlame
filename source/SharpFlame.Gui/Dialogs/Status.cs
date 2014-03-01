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
using Eto.Drawing;
using Eto.Forms;
using SharpFlame.Core;

namespace SharpFlame.Gui.Dialogs
{
    public class Status : Form
    {
        readonly TreeView treeView;

        public Status (Result result)
        {
            Title = result.Text;
            Resizable = true;
            Size = new Size (500, 300);
            Topmost = true;
            ShowInTaskbar = false;
            Location = new Point (200, 100);

            var layout = new DynamicLayout { BackgroundColor = Colors.White, Padding = Padding.Empty } ;
            treeView = new TreeView ();      
            layout.Add (treeView, true, true);
            var item = new TreeItem { Text = result.Text, Expanded = true };
            item.Children.Add (iterResult (result));
            treeView.DataStore = item;

            Content = layout;
        }

        TreeItem iterResult(Result result) {
            var item = new TreeItem
            {
                Text = result.Text,
                Expanded = true,
                // Image = Resources.IconProblem()
            };

            foreach (var lItem in ((Result)result).Items)
            {
                if (lItem is Result.Problem)
                {
                    item.Children.Add (new TreeItem { 
                        Text = ((Result.Problem)lItem).GetText,
                        Expanded = true,
                        Image = Resources.IconProblem()
                    });
                } else if (lItem is Result.Warning)
                {
                    item.Children.Add (new TreeItem { 
                        Text = ((Result.Warning)lItem).GetText,
                        Expanded = true,
                        Image = Resources.IconWarning()
                    });
                } else if (lItem is Result)
                {
                    item.Children.Add (iterResult ((Result)lItem));
                }
            }


            return item;
        }
    }
}

