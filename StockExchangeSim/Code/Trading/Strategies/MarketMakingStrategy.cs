using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class MarketMakingStrategy : Trader.Strategy
    {
        public int ActionTimeDeduction = 60;
        public bool isHighfrequency = true;
        public List<LiquidityMarketWatcher> MarketWatchers = new List<LiquidityMarketWatcher>();
        public MarketMakingStrategy(Trader t)
        {
            trader = t;
        }

        public override void Init()
        {
            foreach (Company cp in trader.InterestedCompanies)
            {
                MarketWatchers.Add(new LiquidityMarketWatcher(this, cp, trader));
            }

        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MarketWatchers[i].UpdateInsights();
            }
            trader.ActionTime -= ActionTimeDeduction;
            return MR;
        }
    }
}
