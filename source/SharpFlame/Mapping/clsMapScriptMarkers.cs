using System.Windows.Forms;
using SharpFlame.Collections;
using SharpFlame.Mapping.Objects;
using SharpFlame.Mapping.Script;

namespace SharpFlame.Mapping
{
    public partial class clsMap
    {
        public ConnectedList<clsScriptPosition, clsMap> ScriptPositions;
        public ConnectedList<clsScriptArea, clsMap> ScriptAreas;

        public string GetDefaultScriptLabel(string Prefix)
        {
            int Number = 1;
            App.sResult Valid = new App.sResult();
            string Label = "";

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

        public App.sResult ScriptLabelIsValid(string Text)
        {
            App.sResult ReturnResult = new App.sResult();
            ReturnResult.Success = false;
            ReturnResult.Problem = "";

            if ( Text == null )
            {
                ReturnResult.Problem = "Label cannot be nothing.";
                return ReturnResult;
            }

            string LCaseText = Text.ToLower();

            if ( LCaseText.Length < 1 )
            {
                ReturnResult.Problem = "Label cannot be nothing.";
                return ReturnResult;
            }

            char CurrentChar = (char)0;
            bool Invalid = default(bool);

            Invalid = false;
            foreach ( char tempLoopVar_CurrentChar in LCaseText )
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

            clsUnit Unit = default(clsUnit);

            foreach ( clsUnit tempLoopVar_Unit in Units )
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

            clsScriptPosition ScriptPosition = default(clsScriptPosition);

            foreach ( clsScriptPosition tempLoopVar_ScriptPosition in ScriptPositions )
            {
                ScriptPosition = tempLoopVar_ScriptPosition;
                if ( LCaseText == ScriptPosition.Label.ToLower() )
                {
                    ReturnResult.Problem = "Label text is already in use.";
                    return ReturnResult;
                }
            }

            clsScriptArea ScriptArea = default(clsScriptArea);

            foreach ( clsScriptArea tempLoopVar_ScriptArea in ScriptAreas )
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