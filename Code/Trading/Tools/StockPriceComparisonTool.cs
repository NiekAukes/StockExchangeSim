using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    public class StockPriceComparisonToolData
    {
        public StockPriceComparisonToolData(float ESP) { ExpectedStockPrice = ESP; }
        public float ExpectedStockPrice;
    }
    class StockPriceComparisonTool : MarketTool<StockPriceComparisonToolData>
    {
        public float MeasuredDividend = -1, Dividend = -1, PrevDividend = -1;

        public override StockPriceComparisonToolData StrategyOutcome(Company cp)
        {

            if (cp.Dividend != MeasuredDividend)
            {
                MeasuredDividend = cp.Dividend;
                PrevDividend = Dividend;
                Dividend = (cp.Dividend * (31556926.0f / Master.inst.SecondsPerTick));
            }
            if (PrevDividend == -1)
                PrevDividend = Dividend;

            
            float StockPrice = //Dividend * 40//cp.CompetitivePosition
                MathF.Pow(cp.CompetitivePosition, 3) * cp.Value * cp.StockPart;
            return new StockPriceComparisonToolData(StockPrice);
        }
    }
}
