namespace FlaME
{
    using Microsoft.VisualBasic.CompilerServices;
    using System;

    [StandardModule]
    public sealed class modResults
    {
        public static clsResultProblemGoto<clsResultItemPosGoto> CreateResultProblemGotoForObject(clsMap.clsUnit unit)
        {
            clsResultItemPosGoto goto2 = new clsResultItemPosGoto {
                View = unit.MapLink.Source.ViewInfo,
                Horizontal = unit.Pos.Horizontal
            };
            return new clsResultProblemGoto<clsResultItemPosGoto> { MapGoto = goto2 };
        }
    }
}

