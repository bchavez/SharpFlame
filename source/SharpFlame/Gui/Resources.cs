using System;
using System.IO;
using System.Linq.Expressions;
using System.Reflection;
using Eto.Drawing;
using SharpFlame.Util;

namespace SharpFlame.Gui
{
	public class Resources
	{
		public static Icon ProgramIcon
		{
			get { return new Icon(GetBytes(()=>ProgramIcon, ".ico")); }
		}

		public static Bitmap RotateAntiClockwise
		{
			get { return new Bitmap(GetBytes(()=> RotateAntiClockwise)); }
		}

		public static Bitmap RotateClockwise
		{
			get { return new Bitmap(GetBytes(()=> RotateClockwise)); }
		}

		public static Bitmap FlipX
		{
			get { return new Bitmap(GetBytes(()=> FlipX)); }
		}

		public static Bitmap Problem
		{
			get { return new Bitmap(GetBytes(()=> Problem)); }
		}

		public static Bitmap Warning
		{
			get { return new Bitmap(GetBytes(()=>Warning)); }
		}

		public static Bitmap NoMap
		{
			get { return new Bitmap(GetBytes(() => NoMap)); }
		}

		public static Bitmap Circle
		{
			get { return new Bitmap(GetBytes(() => Circle)); }
		}

		public static Bitmap Square
		{
			get{return new Bitmap(GetBytes(()=> Square));}
		}
		public static Bitmap MouseLeft
		{
			get { return new Bitmap(GetBytes(() => MouseLeft)); }
		}
		public static Bitmap MouseRight
		{
			get { return new Bitmap(GetBytes(() => MouseRight)); }
		}

		public static Bitmap Place
		{
			get { return new Bitmap(GetBytes(() => Place)); }
		}
		public static Bitmap Line
		{
			get { return new Bitmap(GetBytes(() => Line)); }
		}

		public static Bitmap Selection => new Bitmap(GetBytes(() => Selection));
		public static Bitmap SelectionCopy => new Bitmap(GetBytes(() => SelectionCopy));
		public static Bitmap SelectionPasteOptions => new Bitmap(GetBytes(() => SelectionPasteOptions));
		public static Bitmap SelectionPaste => new Bitmap(GetBytes(() => SelectionPaste));
		public static Bitmap SelectionRotateAntiClockwise => new Bitmap(GetBytes(() => SelectionRotateAntiClockwise));
		public static Bitmap SelectionRotateClockwise => new Bitmap(GetBytes(() => SelectionRotateClockwise));
		public static Bitmap SelectionFlipX => new Bitmap(GetBytes(() => SelectionFlipX));
		public static Bitmap ObjectsSelect => new Bitmap(GetBytes(() => ObjectsSelect));

		public static Bitmap Gateways => new Bitmap(GetBytes(() => Gateways));
		public static Bitmap DisplayAutoTexture => new Bitmap(GetBytes(() => DisplayAutoTexture));
		public static Bitmap DrawTileOrientation => new Bitmap(GetBytes(() => DrawTileOrientation));

		public static Bitmap Save => new Bitmap(GetBytes(() => Save));
		

		public static Lazy<Assembly> Assembly = new Lazy<Assembly>(System.Reflection.Assembly.GetExecutingAssembly);

		public static Stream GetBytes(Expression<Func<object>> exp, string ext)
		{
			var name = PropertyName.For(exp) + ext;
			var fullName = Path.ChangeExtension(name, ext);
			return GetBytes(fullName);
		}

		public static Stream GetBytes(Expression<Func<object>> exp)
		{
			return GetBytes(exp, ".png");
		}

		public static Stream GetBytes(string fileName)
		{
			var frn = FullResourceName(fileName);
			return Assembly.Value.GetManifestResourceStream(frn);
		}

		public static string FullResourceName(string file)
		{
			return string.Concat(Assembly.Value.GetName().Name, ".Resources.", file);
		}
	}
}

