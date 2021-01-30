using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    public class InvestorStrategy : Trader.Strategy
    {
        public List<ComparisonMarketWatcher> MarketWatchers = new List<ComparisonMarketWatcher>();

        public InvestorStrategy(Trader t)
        {
            foreach (Company cp in t.InterestedCompanies)
                MarketWatchers.Add(new ComparisonMarketWatcher(cp));
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].UpdateInsights()));
            }
            trader.ActionTime -= 10000;
            return MR;
        }
        public override void Observe()
        {
            //Observe the market
        }
    }
}
