using System.Windows.Forms;

namespace SharpFlame.AppSettings
{
    public sealed class modControls
    {
        public static OptionGroup Options_KeyboardControls = new OptionGroup();
        public static clsKeyboardProfile KeyboardProfile;

        //interface controls
        public static Option<clsKeyboardControl> Control_Deselect;
        public static Option<clsKeyboardControl> Control_PreviousTool;
        //selected unit controls
        public static Option<clsKeyboardControl> Control_Unit_Move;
        public static Option<clsKeyboardControl> Control_Unit_Delete;
        public static Option<clsKeyboardControl> Control_Unit_Multiselect;
        //generalised controls
        public static Option<clsKeyboardControl> Control_Slow;
        public static Option<clsKeyboardControl> Control_Fast;
        //picker controls
        public static Option<clsKeyboardControl> Control_Picker;
        //view controls
        public static Option<clsKeyboardControl> Control_View_Textures;
        public static Option<clsKeyboardControl> Control_View_Lighting;
        public static Option<clsKeyboardControl> Control_View_Wireframe;
        public static Option<clsKeyboardControl> Control_View_Units;
        public static Option<clsKeyboardControl> Control_View_ScriptMarkers;
        public static Option<clsKeyboardControl> Control_View_Move_Type;
        public static Option<clsKeyboardControl> Control_View_Rotate_Type;
        public static Option<clsKeyboardControl> Control_View_Move_Left;
        public static Option<clsKeyboardControl> Control_View_Move_Right;
        public static Option<clsKeyboardControl> Control_View_Move_Forward;
        public static Option<clsKeyboardControl> Control_View_Move_Backward;
        public static Option<clsKeyboardControl> Control_View_Move_Up;
        public static Option<clsKeyboardControl> Control_View_Move_Down;
        public static Option<clsKeyboardControl> Control_View_Zoom_In;
        public static Option<clsKeyboardControl> Control_View_Zoom_Out;
        public static Option<clsKeyboardControl> Control_View_Left;
        public static Option<clsKeyboardControl> Control_View_Right;
        public static Option<clsKeyboardControl> Control_View_Forward;
        public static Option<clsKeyboardControl> Control_View_Backward;
        public static Option<clsKeyboardControl> Control_View_Up;
        public static Option<clsKeyboardControl> Control_View_Down;
        public static Option<clsKeyboardControl> Control_View_Reset;
        public static Option<clsKeyboardControl> Control_View_Roll_Left;
        public static Option<clsKeyboardControl> Control_View_Roll_Right;
        //texture controls
        public static Option<clsKeyboardControl> Control_Clockwise;
        public static Option<clsKeyboardControl> Control_CounterClockwise;
        public static Option<clsKeyboardControl> Control_Texture_Flip;
        public static Option<clsKeyboardControl> Control_Tri_Flip;
        //gateway controls
        public static Option<clsKeyboardControl> Control_Gateway_Delete;
        //undo controls
        public static Option<clsKeyboardControl> Control_Undo;
        public static Option<clsKeyboardControl> Control_Redo;
        //script marker controls
        public static Option<clsKeyboardControl> Control_ScriptPosition;

        public static Option<clsKeyboardControl> KeyboardControlOptionCreate(string saveKey, Keys[] keys)
        {
            Option<clsKeyboardControl> result = new Option<clsKeyboardControl>(saveKey, new clsKeyboardControl(keys, new Keys[] {}));
            Options_KeyboardControls.Options.Add(result.GroupLink);
            return result;
        }

        public static Option<clsKeyboardControl> KeyboardControlOptionCreate(string saveKey, Keys[] keys, Keys[] unlessKeys)
        {
            Option<clsKeyboardControl> result = new Option<clsKeyboardControl>(saveKey, new clsKeyboardControl(keys, unlessKeys));
            Options_KeyboardControls.Options.Add(result.GroupLink);
            return result;
        }

        public static Option<clsKeyboardControl> KeyboardControlOptionCreate(string saveKey, clsKeyboardControl defaultValue)
        {
            Option<clsKeyboardControl> result = new Option<clsKeyboardControl>(saveKey, defaultValue);
            Options_KeyboardControls.Options.Add(result.GroupLink);
            return result;
        }

        public static void CreateControls()
        {
            //interface controls

            Control_Deselect = KeyboardControlOptionCreate("ObjectSelectTool", new Keys[] {Keys.Escape});
            Control_PreviousTool = KeyboardControlOptionCreate("PreviousTool", new Keys[] {Keys.Oemtilde});

            //selected unit controls

            Control_Unit_Move = KeyboardControlOptionCreate("MoveObjects", new Keys[] {Keys.M});
            Control_Unit_Delete = KeyboardControlOptionCreate("DeleteObjects", new Keys[] {Keys.Delete});
            Control_Unit_Multiselect = KeyboardControlOptionCreate("Multiselect", new Keys[] {Keys.ShiftKey});

            //generalised controls

            Control_Slow = KeyboardControlOptionCreate("ViewSlow", new Keys[] {Keys.R});
            Control_Fast = KeyboardControlOptionCreate("ViewFast", new Keys[] {Keys.F});

            //picker controls

            Control_Picker = KeyboardControlOptionCreate("Picker", new Keys[] {Keys.ControlKey});

            //view controls

            Control_View_Textures = KeyboardControlOptionCreate("ShowTextures", new Keys[] {Keys.F5});
            Control_View_Lighting = KeyboardControlOptionCreate("ShowLighting", new Keys[] {Keys.F8});
            Control_View_Wireframe = KeyboardControlOptionCreate("ShowWireframe", new Keys[] {Keys.F6});
            Control_View_Units = KeyboardControlOptionCreate("ShowObjects", new Keys[] {Keys.F7});
            Control_View_ScriptMarkers = KeyboardControlOptionCreate("ShowLabels", new Keys[] {Keys.F4});
            Control_View_Move_Type = KeyboardControlOptionCreate("ViewMoveMode", new Keys[] {Keys.F1});
            Control_View_Rotate_Type = KeyboardControlOptionCreate("ViewRotateMode", new Keys[] {Keys.F2});
            Control_View_Move_Left = KeyboardControlOptionCreate("ViewMoveLeft", new Keys[] {Keys.A});
            Control_View_Move_Right = KeyboardControlOptionCreate("ViewMoveRight", new Keys[] {Keys.D});
            Control_View_Move_Forward = KeyboardControlOptionCreate("ViewMoveForwards", new Keys[] {Keys.W});
            Control_View_Move_Backward = KeyboardControlOptionCreate("ViewMoveBackwards", new Keys[] {Keys.S});
            Control_View_Move_Up = KeyboardControlOptionCreate("ViewMoveUp", new Keys[] {Keys.E});
            Control_View_Move_Down = KeyboardControlOptionCreate("ViewMoveDown", new Keys[] {Keys.C});
            Control_View_Zoom_In = KeyboardControlOptionCreate("ViewZoomIn", new Keys[] {Keys.Home});
            Control_View_Zoom_Out = KeyboardControlOptionCreate("ViewZoomOut", new Keys[] {Keys.End});
            Control_View_Left = KeyboardControlOptionCreate("ViewRotateLeft", new Keys[] {Keys.Left});
            Control_View_Right = KeyboardControlOptionCreate("ViewRotateRight", new Keys[] {Keys.Right});
            Control_View_Forward = KeyboardControlOptionCreate("ViewRotateForwards", new Keys[] {Keys.Up});
            Control_View_Backward = KeyboardControlOptionCreate("ViewRotateBackwards", new Keys[] {Keys.Down});
            Control_View_Up = KeyboardControlOptionCreate("ViewRotateUp", new Keys[] {Keys.PageUp});
            Control_View_Down = KeyboardControlOptionCreate("ViewRotateDown", new Keys[] {Keys.PageDown});
            Control_View_Roll_Left = KeyboardControlOptionCreate("ViewRollLeft", new Keys[] {Keys.OemOpenBrackets});
            Control_View_Roll_Right = KeyboardControlOptionCreate("ViewRollRight", new Keys[] {Keys.OemCloseBrackets});
            Control_View_Reset = KeyboardControlOptionCreate("ViewReset", new Keys[] {Keys.Back});

            //texture controls

            Control_CounterClockwise = KeyboardControlOptionCreate("CounterClockwise", new Keys[] {Keys.Oemcomma});
            Control_Clockwise = KeyboardControlOptionCreate("Clockwise", new Keys[] {Keys.OemPeriod});
            Control_Texture_Flip = KeyboardControlOptionCreate("TextureFlip", new Keys[] {Keys.OemQuestion});
            Control_Tri_Flip = KeyboardControlOptionCreate("TriangleFlip", new Keys[] {Keys.OemPipe}); //\|
            Control_Gateway_Delete = KeyboardControlOptionCreate("GatewayDelete", new Keys[] {Keys.ShiftKey});

            //undo controls

            Control_Undo = KeyboardControlOptionCreate("Undo", new Keys[] {Keys.ControlKey, Keys.Z});
            Control_Redo = KeyboardControlOptionCreate("Redo", new Keys[] {Keys.ControlKey, Keys.Y});
            Control_ScriptPosition = KeyboardControlOptionCreate("PositionLabel", new Keys[] {Keys.P});

            KeyboardProfile = new clsKeyboardProfile(Options_KeyboardControls);
        }
    }

    public class KeyboardProfileCreator : OptionProfileCreator
    {
        public override OptionProfile Create()
        {
            return new clsKeyboardProfile(Options);
        }
    }

    public class clsKeyboardProfile : OptionProfile
    {
        public bool Active(Option<clsKeyboardControl> control)
        {
            return ((clsKeyboardControl)(get_Value(control))).Active;
        }

        public clsKeyboardProfile(OptionGroup options) : base(options)
        {
        }
    }

    public class clsKeyboardControl
    {
        public Keys[] Keys;
        public Keys[] UnlessKeys;

        private bool IsPressed(App.clsKeysActive KeysDown)
        {
            foreach ( Keys keys in Keys )
            {
                if ( !KeysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }
            foreach ( Keys keys in UnlessKeys )
            {
                if ( KeysDown.Keys[(int)keys] )
                {
                    return false;
                }
            }

            return true;
        }

        public clsKeyboardControl(Keys[] Keys)
        {
            this.Keys = Keys;
            UnlessKeys = new Keys[0];
        }

        public clsKeyboardControl(Keys[] Keys, Keys[] UnlessKeys)
        {
            this.Keys = Keys;
            this.UnlessKeys = UnlessKeys;
        }

        private bool _Active;

        public bool Active
        {
            get { return _Active; }
        }

        public void KeysChanged(App.clsKeysActive KeysDown)
        {
            _Active = IsPressed(KeysDown);
        }
    }
}