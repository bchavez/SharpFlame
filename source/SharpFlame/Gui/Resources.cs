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

