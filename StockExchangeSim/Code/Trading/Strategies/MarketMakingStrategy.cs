using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class MarketMakingStrategy : Trader.Strategy
    {
        
        //DON'T FORGET TO IMPLEMENT: float latestInsightResult
        public override Trader.MarketResults StrategyOutcome(Trader trader, ExchangeBroker exchange)
        {
            throw new NotImplementedException();
        }
    }
}
