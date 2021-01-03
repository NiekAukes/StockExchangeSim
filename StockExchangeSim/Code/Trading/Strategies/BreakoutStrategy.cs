using System;
using System.Collections.Generic;

namespace Eco
{
    public class BreakoutStrategy : Trader.Strategy
    {
        public List<BreakoutMarketWatcher> MarketWatchers = new List<BreakoutMarketWatcher>();
        SupportResistanceTool SRTool;
        SupportResistanceData SRData;

        public BreakoutStrategy(Trader t)
        {
            foreach (Company cp in t.InterestedCompanies)
                MarketWatchers.Add(new BreakoutMarketWatcher(cp));
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].UpdateInsights()));
            }
            return MR;
        }
        public override void Observe()
        {
            //Observe the market
        }
    }
}
