using System;

namespace Eco
{

    public class LiquidityToolData
    {
        public LiquidityToolData(float ESP) { ExpectedStockPrice = ESP; }
        public float ExpectedStockPrice;
    }

    class LiquidityTool : MarketTool<LiquidityToolData>
    {
        public float MeasuredDividend = -1, Dividend = -1, PrevDividend = -1;

        public override LiquidityToolData StrategyOutcome(Company cp)
        {

            if (cp.Dividend != MeasuredDividend)
            {
                MeasuredDividend = cp.Dividend;
                PrevDividend = Dividend;
                Dividend = (cp.Dividend * (31556926.0f / Master.inst.SecondsPerTick));
            }
            if (PrevDividend == -1)
                PrevDividend = Dividend;

            float MarketTrust = PrevDividend > 0 ? MathF.Pow(2, (Dividend - PrevDividend) / PrevDividend) : 0;
            float StockPrice = 40 * Dividend * MathF.Pow(cp.CompetitivePosition, 3) * MarketTrust
                + MathF.Pow((float)cp.CompetitivePosition, 3) * (float)cp.Value * (float)cp.StockPart;
            return new LiquidityToolData(StockPrice);
        }
    }
}
