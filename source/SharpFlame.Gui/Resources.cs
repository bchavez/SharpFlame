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

using System.Reflection;
using Eto.Drawing;
using Eto;

namespace SharpFlame.Gui
{
	public class Resources
	{
		static string prefix;
		public static string Prefix { get { return prefix = prefix ?? "SharpFlame.Gui.Resources."; } }

		public static string SharpFlameIconName = "flaME.ico";
		public static string BtnRotateAntiClockwiseName = "btnrotateanticlockwise.png";
		public static string BtnRotateClockwiseName = "btnrotateclockwise.png";
		public static string BtnFlipXName = "btnflipx.png";

		public static Icon SharpFlameIcon(Generator generator = null)
		{
			return Icon.FromResource (string.Format ("{0}{1}", Prefix, SharpFlameIconName), generator);
		}

		public static Bitmap BtnRotateAntiClockwise(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnRotateAntiClockwiseName), Assembly.GetExecutingAssembly(), generator);
		}

		public static Bitmap BtnRotateClockwise(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnRotateClockwiseName), Assembly.GetExecutingAssembly(), generator);
		}

		public static Bitmap BtnFlipX(Generator generator = null)
		{
			return Bitmap.FromResource (string.Format ("{0}{1}", Prefix, BtnFlipXName), Assembly.GetExecutingAssembly(), generator);
		}
	}
}

