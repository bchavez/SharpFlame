#region

using SharpFlame.Core;
using SharpFlame.Core.Domain;
using SharpFlame.Old.Mapping.Objects;

#endregion

namespace SharpFlame.Old
{
    public class ResultWarningGoto<GotoType> : Result.Warning where GotoType : clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();

            MapGoto.Perform();
        }
    }

    public class ResultProblemGoto<GotoType> : Result.Problem where GotoType : clsResultItemGotoInterface
    {
        public GotoType MapGoto;

        public override void DoubleClicked()
        {
            base.DoubleClicked();

            MapGoto.Perform();
        }
    }

    public abstract class clsResultItemGotoInterface
    {
        public abstract void Perform();
    }

    public class clsResultItemTileGoto : clsResultItemGotoInterface
    {
        public XYInt TileNum;
        public clsViewInfo View;

        public override void Perform()
        {
            View.LookAtTile(TileNum);
        }
    }

    public class clsResultItemPosGoto : clsResultItemGotoInterface
    {
        public XYInt Horizontal;
        public clsViewInfo View;

        public override void Perform()
        {
            View.LookAtPos(Horizontal);
        }
    }

    public sealed class modResults
    {
        public static ResultProblemGoto<clsResultItemPosGoto> CreateResultProblemGotoForObject(Unit unit)
        {
            var resultGoto = new clsResultItemPosGoto();
            resultGoto.View = unit.MapLink.Source.ViewInfo;
            resultGoto.Horizontal = unit.Pos.Horizontal;
            var resultProblem = new ResultProblemGoto<clsResultItemPosGoto>();
            resultProblem.MapGoto = resultGoto;
            return resultProblem;
        }
    }
}