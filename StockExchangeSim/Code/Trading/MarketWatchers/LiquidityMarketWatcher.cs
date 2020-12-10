using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class LiquidityMarketWatcher : MarketWatcher<MarketMakingStrategy>
    {
        LiquidityTool LT = new LiquidityTool();
        LiquidityToolData LTD = null;
        public override void RedoInsights()
        {
            throw new NotImplementedException();
        }

        public override float UpdateInsights()
        {
            if (LTD == null)
            {
                LTD = LT.StrategyOutcome(cp);
            }
            throw new NotImplementedException();
        }
    }
}
