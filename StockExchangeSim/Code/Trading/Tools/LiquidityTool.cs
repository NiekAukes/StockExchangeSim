using System;

namespace Eco
{

    public class LiquidityToolData
    {
       
    }

    class LiquidityTool : MarketTool<LiquidityToolData>
    {
        public override LiquidityToolData StrategyOutcome(Company cp)
        {
            throw new NotImplementedException();
        }
    }
}
