using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{

    public abstract class MarketTool<ToolData>
    {
        Random rn = new Random();
        public static int MinimumPriceCount = 50;
        public abstract ToolData StrategyOutcome(Company cp);
    }
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
                MeasuredDividend = (float)cp.Dividend;
                PrevDividend = Dividend;
                Dividend = (float)(cp.Dividend * (31556926.0 / Master.inst.SecondsPerTick));
            }
            if (PrevDividend == -1)
                PrevDividend = Dividend;

            float MarketTrust = PrevDividend > 0 ? MathF.Pow(2, (Dividend - PrevDividend) / PrevDividend) : 0;
            float StockPrice = 40 * Dividend * MathF.Pow((float)cp.CompetitivePosition, 3) * MarketTrust
                + MathF.Pow((float)cp.CompetitivePosition, 3) * (float)cp.Value * (float)cp.StockPart;
            return new LiquidityToolData(StockPrice);
        }
    }
}
