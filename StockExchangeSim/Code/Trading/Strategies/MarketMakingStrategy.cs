using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class MarketMakingStrategy : Trader.Strategy
    {

        public List<LiquidityMarketWatcher> MarketWatchers = new List<LiquidityMarketWatcher>();

        public MarketMakingStrategy(Trader t)
        {
            foreach (Company cp in t.InterestedCompanies)
                MarketWatchers.Add(new LiquidityMarketWatcher(cp, t));
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MarketWatchers[i].UpdateInsights();
            }
            //trader.ActionTime -= 100;
            return MR;
        }
        public override void Observe()
        {
            throw new NotImplementedException();
        }
    }
}
