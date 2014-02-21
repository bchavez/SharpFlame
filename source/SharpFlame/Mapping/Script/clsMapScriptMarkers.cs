#region

using System.Windows.Forms;
using SharpFlame.Collections;
using SharpFlame.Core.Collections;
using SharpFlame.FileIO;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;
using SharpFlame.Util;

#endregion

namespace SharpFlame.Mapping
{
    public partial class Map
    {
        public ConnectedList<clsScriptArea, Map> ScriptAreas;
        public ConnectedList<clsScriptPosition, Map> ScriptPositions;

        public string GetDefaultScriptLabel(string Prefix)
        {
            var Number = 1;
            var Valid = new sResult();
            var Label = "";

            do
            {
                Label = Prefix + Number.ToStringInvariant();
                Valid = ScriptLabelIsValid(Label);
                if ( Valid.Success )
                {
                    return Label;
                }
                Number++;
                if ( Number >= 16384 )
                {
                    MessageBox.Show("Error: Unable to set default script label.");
                    return "";
                }
            } while ( true );
        }

        public sResult ScriptLabelIsValid(string Text)
        {
            var ReturnResult = new sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            if ( Text == null )
            {
                ReturnResult.Problem = "Label cannot be nothing.";
                return ReturnResult;
            }

            var LCaseText = Text.ToLower();

            if ( LCaseText.Length < 1 )
            {
                ReturnResult.Problem = "Label cannot be nothing.";
                return ReturnResult;
            }

            var CurrentChar = (char)0;
            var Invalid = default(bool);

            Invalid = false;
            foreach ( var tempLoopVar_CurrentChar in LCaseText )
            {
                CurrentChar = tempLoopVar_CurrentChar;
                if ( !((CurrentChar >= 'a' && CurrentChar <= 'z') || (CurrentChar >= '0' && CurrentChar <= '9') || CurrentChar == '_') )
                {
                    Invalid = true;
                    break;
                }
            }
            if ( Invalid )
            {
                ReturnResult.Problem = "Label contains invalid characters. Use only letters, numbers or underscores.";
                return ReturnResult;
            }

            var Unit = default(Unit);

            foreach ( var tempLoopVar_Unit in Units )
            {
                Unit = tempLoopVar_Unit;
                if ( Unit.Label != null )
                {
                    if ( LCaseText == Unit.Label.ToLower() )
                    {
                        ReturnResult.Problem = "Label text is already in use.";
                        return ReturnResult;
                    }
                }
            }

            var ScriptPosition = default(clsScriptPosition);

            foreach ( var tempLoopVar_ScriptPosition in ScriptPositions )
            {
                ScriptPosition = tempLoopVar_ScriptPosition;
                if ( LCaseText == ScriptPosition.Label.ToLower() )
                {
                    ReturnResult.Problem = "Label text is already in use.";
                    return ReturnResult;
                }
            }

            var ScriptArea = default(clsScriptArea);

            foreach ( var tempLoopVar_ScriptArea in ScriptAreas )
            {
                ScriptArea = tempLoopVar_ScriptArea;
                if ( LCaseText == ScriptArea.Label.ToLower() )
                {
                    ReturnResult.Problem = "Label text is already in use.";
                    return ReturnResult;
                }
            }

            ReturnResult.Success = true;
            return ReturnResult;
        }
    }
}