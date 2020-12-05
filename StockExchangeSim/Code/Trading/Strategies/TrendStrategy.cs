using System;
using System.Collections.Generic;

namespace Eco
{
    class TrendStrategy : Trader.Strategy
    {
        Trader trader = null;
        public List<TrendMarketWatcher> MarketWatchers = new List<TrendMarketWatcher>();

        public TrendStrategy(Trader t)
        {
            trader = t;

            foreach (Company cp in t.InterestedCompanies)
                MarketWatchers.Add(new TrendMarketWatcher(cp));
        }
        public override void Observe()
        {
            throw new NotImplementedException();
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ExchangeBroker exchange)
        {
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].UpdateInsights()));
            }
            return MR;
        }
    }
}
