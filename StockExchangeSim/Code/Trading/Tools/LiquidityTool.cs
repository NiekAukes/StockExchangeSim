using System;

namespace Eco
{

    public class LiquidityToolData
    {
        int buyAmount, SellAmount;
    }

    class LiquidityTool : MarketTool<LiquidityToolData>
    {
        public override LiquidityToolData StrategyOutcome(Company cp)
        {
            
            throw new NotImplementedException();
        }
    }
}
