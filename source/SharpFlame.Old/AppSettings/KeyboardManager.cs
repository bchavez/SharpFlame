#region

using System.Windows.Forms;

#endregion

namespace SharpFlame.Old.AppSettings
{
    public sealed class KeyboardManager
    {
        public static OptionGroup OptionsKeyboardControls = new OptionGroup();
        public static KeyboardProfile KeyboardProfile;

        //interface controls
        public static Option<KeyboardControl> Deselect;
        public static Option<KeyboardControl> PreviousTool;
        //selected unit controls
        public static Option<KeyboardControl> UnitMove;
        public static Option<KeyboardControl> UnitDelete;
        public static Option<KeyboardControl> UnitMultiselect;
        //generalised controls
        public static Option<KeyboardControl> Slow;
        public static Option<KeyboardControl> Fast;
        //picker controls
        public static Option<KeyboardControl> Picker;
        //view controls
        public static Option<KeyboardControl> ViewTextures;
        public static Option<KeyboardControl> ViewLighting;
        public static Option<KeyboardControl> ViewWireframe;
        public static Option<KeyboardControl> ViewUnits;
        public static Option<KeyboardControl> ViewScriptMarkers;
        public static Option<KeyboardControl> ViewMoveType;
        public static Option<KeyboardControl> ViewRotateType;
        public static Option<KeyboardControl> ViewMoveLeft;
        public static Option<KeyboardControl> ViewMoveRight;
        public static Option<KeyboardControl> ViewMoveForward;
        public static Option<KeyboardControl> ViewMoveBackward;
        public static Option<KeyboardControl> ViewMoveUp;
        public static Option<KeyboardControl> ViewMoveDown;
        public static Option<KeyboardControl> ViewZoomIn;
        public static Option<KeyboardControl> ViewZoomOut;
        public static Option<KeyboardControl> ViewLeft;
        public static Option<KeyboardControl> ViewRight;
        public static Option<KeyboardControl> ViewForward;
        public static Option<KeyboardControl> ViewBackward;
        public static Option<KeyboardControl> ViewUp;
        public static Option<KeyboardControl> ViewDown;
        public static Option<KeyboardControl> ViewReset;
        public static Option<KeyboardControl> ViewRollLeft;
        public static Option<KeyboardControl> ViewRollRight;
        //texture controls
        public static Option<KeyboardControl> Clockwise;
        public static Option<KeyboardControl> CounterClockwise;
        public static Option<KeyboardControl> TextureFlip;
        public static Option<KeyboardControl> TriFlip;
        //gateway controls
        public static Option<KeyboardControl> Gateway_Delete;
        //undo controls
        public static Option<KeyboardControl> Undo;
        public static Option<KeyboardControl> Redo;
        //script marker controls
        public static Option<KeyboardControl> ScriptPosition;

        public static Option<KeyboardControl> KeyboardControlOptionCreate(string saveKey, Keys[] keys)
        {
            var result = new Option<KeyboardControl>(saveKey, new KeyboardControl(keys, new Keys[] {}));
            OptionsKeyboardControls.Options.Add(result.GroupLink);
            return result;
        }

        public static Option<KeyboardControl> KeyboardControlOptionCreate(string saveKey, Keys[] keys, Keys[] unlessKeys)
        {
            var result = new Option<KeyboardControl>(saveKey, new KeyboardControl(keys, unlessKeys));
            OptionsKeyboardControls.Options.Add(result.GroupLink);
            return result;
        }

        public static Option<KeyboardControl> KeyboardControlOptionCreate(string saveKey, KeyboardControl defaultValue)
        {
            var result = new Option<KeyboardControl>(saveKey, defaultValue);
            OptionsKeyboardControls.Options.Add(result.GroupLink);
            return result;
        }

        public static void CreateControls()
        {
            //interface controls

            Deselect = KeyboardControlOptionCreate("ObjectSelectTool", new[] {Keys.Escape});
            PreviousTool = KeyboardControlOptionCreate("PreviousTool", new[] {Keys.Oemtilde});

            //selected unit controls

            UnitMove = KeyboardControlOptionCreate("MoveObjects", new[] {Keys.M});
            UnitDelete = KeyboardControlOptionCreate("DeleteObjects", new[] {Keys.Delete});
            UnitMultiselect = KeyboardControlOptionCreate("Multiselect", new[] {Keys.ShiftKey});

            //generalised controls

            Slow = KeyboardControlOptionCreate("ViewSlow", new[] {Keys.R});
            Fast = KeyboardControlOptionCreate("ViewFast", new[] {Keys.F});

            //picker controls

            Picker = KeyboardControlOptionCreate("Picker", new[] {Keys.ControlKey});

            //view controls

            ViewTextures = KeyboardControlOptionCreate("ShowTextures", new[] {Keys.F5});
            ViewLighting = KeyboardControlOptionCreate("ShowLighting", new[] {Keys.F8});
            ViewWireframe = KeyboardControlOptionCreate("ShowWireframe", new[] {Keys.F6});
            ViewUnits = KeyboardControlOptionCreate("ShowObjects", new[] {Keys.F7});
            ViewScriptMarkers = KeyboardControlOptionCreate("ShowLabels", new[] {Keys.F4});
            ViewMoveType = KeyboardControlOptionCreate("ViewMoveMode", new[] {Keys.F1});
            ViewRotateType = KeyboardControlOptionCreate("ViewRotateMode", new[] {Keys.F2});
            ViewMoveLeft = KeyboardControlOptionCreate("ViewMoveLeft", new[] {Keys.A});
            ViewMoveRight = KeyboardControlOptionCreate("ViewMoveRight", new[] {Keys.D});
            ViewMoveForward = KeyboardControlOptionCreate("ViewMoveForwards", new[] {Keys.W});
            ViewMoveBackward = KeyboardControlOptionCreate("ViewMoveBackwards", new[] {Keys.S});
            ViewMoveUp = KeyboardControlOptionCreate("ViewMoveUp", new[] {Keys.E});
            ViewMoveDown = KeyboardControlOptionCreate("ViewMoveDown", new[] {Keys.C});
            ViewZoomIn = KeyboardControlOptionCreate("ViewZoomIn", new[] {Keys.Home});
            ViewZoomOut = KeyboardControlOptionCreate("ViewZoomOut", new[] {Keys.End});
            ViewLeft = KeyboardControlOptionCreate("ViewRotateLeft", new[] {Keys.Left});
            ViewRight = KeyboardControlOptionCreate("ViewRotateRight", new[] {Keys.Right});
            ViewForward = KeyboardControlOptionCreate("ViewRotateForwards", new[] {Keys.Up});
            ViewBackward = KeyboardControlOptionCreate("ViewRotateBackwards", new[] {Keys.Down});
            ViewUp = KeyboardControlOptionCreate("ViewRotateUp", new[] {Keys.PageUp});
            ViewDown = KeyboardControlOptionCreate("ViewRotateDown", new[] {Keys.PageDown});
            ViewRollLeft = KeyboardControlOptionCreate("ViewRollLeft", new[] {Keys.OemOpenBrackets});
            ViewRollRight = KeyboardControlOptionCreate("ViewRollRight", new[] {Keys.OemCloseBrackets});
            ViewReset = KeyboardControlOptionCreate("ViewReset", new[] {Keys.Back});

            //texture controls

            CounterClockwise = KeyboardControlOptionCreate("CounterClockwise", new[] {Keys.Oemcomma});
            Clockwise = KeyboardControlOptionCreate("Clockwise", new[] {Keys.OemPeriod});
            TextureFlip = KeyboardControlOptionCreate("TextureFlip", new[] {Keys.OemQuestion});
            TriFlip = KeyboardControlOptionCreate("TriangleFlip", new[] {Keys.OemPipe}); //\|
            Gateway_Delete = KeyboardControlOptionCreate("GatewayDelete", new[] {Keys.ShiftKey});

            //undo controls

            Undo = KeyboardControlOptionCreate("Undo", new[] {Keys.ControlKey, Keys.Z});
            Redo = KeyboardControlOptionCreate("Redo", new[] {Keys.ControlKey, Keys.Y});
            ScriptPosition = KeyboardControlOptionCreate("PositionLabel", new[] {Keys.P});

            KeyboardProfile = new KeyboardProfile(OptionsKeyboardControls);
        }
    }
}