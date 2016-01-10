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
		public static Icon ProgramIcon { get; private set; }

		public static Bitmap RotateAntiClockwise { get; private set; }

		public static Bitmap RotateClockwise { get; private set; }

		public static Bitmap FlipX { get; private set; }

		public static Bitmap Problem { get; private set; }

		public static Bitmap Warning { get; private set; }

		public static Bitmap NoMap { get; private set; }

		public static Bitmap Circle { get; private set; }

		public static Bitmap Square { get; private set; }
		public static Bitmap MouseLeft { get; private set; }
		public static Bitmap MouseRight { get; private set; }

		public static Bitmap Place { get; private set; }
		public static Bitmap Line { get; private set; }

	    public static Bitmap Selection { get; private set; }
		public static Bitmap SelectionCopy { get; private set; }
		public static Bitmap SelectionPasteOptions { get; private set; }
        public static Bitmap SelectionPaste { get; private set; }
        public static Bitmap SelectionRotateAntiClockwise { get; private set; }
        public static Bitmap SelectionRotateClockwise { get; private set; }
        public static Bitmap SelectionFlipX { get; private set; }
        public static Bitmap ObjectsSelect { get; private set; }

        public static Bitmap Gateways { get; private set; }
        public static Bitmap DisplayAutoTexture { get; private set; }
        public static Bitmap DrawTileOrientation { get; private set; }

        public static Bitmap Save { get; private set; }

	    public static void LoadResources()
	    {
            ProgramIcon = LoadIcon(nameof(ProgramIcon));

	        RotateAntiClockwise = LoadBmp(nameof(RotateAntiClockwise));
	        RotateClockwise = LoadBmp(nameof(RotateClockwise));
	        FlipX = LoadBmp(nameof(FlipX));
	        Problem = LoadBmp(nameof(Problem));
	        Warning = LoadBmp(nameof(Warning));
	        NoMap = LoadBmp(nameof(NoMap));
	        Circle = LoadBmp(nameof(Circle));
	        Square = LoadBmp(nameof(Square));
	        MouseLeft = LoadBmp(nameof(MouseLeft));
	        MouseRight = LoadBmp(nameof(MouseRight));
	        Place = LoadBmp(nameof(Place));
	        Line = LoadBmp(nameof(Line));

            Selection = LoadBmp(nameof(Selection));
            SelectionCopy = LoadBmp(nameof(SelectionCopy));
            SelectionPasteOptions = LoadBmp(nameof(SelectionPasteOptions));
            SelectionPaste = LoadBmp(nameof(SelectionPaste));
            SelectionRotateAntiClockwise = LoadBmp(nameof(SelectionRotateAntiClockwise));
            SelectionRotateClockwise = LoadBmp(nameof(SelectionRotateClockwise));
            SelectionFlipX = LoadBmp(nameof(SelectionFlipX));
            ObjectsSelect = LoadBmp(nameof(ObjectsSelect));

            Gateways = LoadBmp(nameof(Gateways));
            DisplayAutoTexture = LoadBmp(nameof(DisplayAutoTexture));
            DrawTileOrientation = LoadBmp(nameof(DrawTileOrientation));
            Save = LoadBmp(nameof(Save));
        }

        public static Bitmap LoadBmp(string resource, string ext = "png")
	    {
	        return Bitmap.FromResource($"SharpFlame.Resources.{resource}.{ext}");
	    }

	    public static Icon LoadIcon(string resource, string ext = "ico")
	    {
            return Icon.FromResource($"SharpFlame.Resources.{resource}.{ext}");
        }
	}
}

