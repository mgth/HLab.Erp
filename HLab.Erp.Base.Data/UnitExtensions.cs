using System.Collections.Generic;

namespace HLab.Erp.Base.Data;

public static class UnitExtensions
{
    public static Unit BestMatch(this IEnumerable<Unit> units, double absQty)
    {
        Unit bestUnit = null;
        double bestQty = 0;
        foreach (var unit in units)
        {
            var qty = unit.Qty(absQty);

            if (
                bestUnit != null 
                && (qty >= bestQty || qty < 1.0) 
                && (bestQty >= 1.0 || qty <= bestQty)
                ) continue;

            bestUnit = unit;
            bestQty = qty;
        }
        return bestUnit;

    }
}