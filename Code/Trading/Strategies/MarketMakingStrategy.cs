using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Eco
{
    class MarketMakingStrategy : Trader.Strategy
    {
        public int ActionTimeDeduction = 60; //IS CHANGED LATER IN INITIALISATION, this is to prevent dividebyzero exceptions
        public List<LiquidityMarketWatcher> MarketWatchers = new List<LiquidityMarketWatcher>();
        public MarketMakingStrategy(Trader t)
        {
            trader = t;
        }

        public override void Init()
        {
            ActionTimeDeduction = Master.HFTEnabled ? 1 : 60; //zorgt dr voor dat de handelaren als hftraders gaan werken als HFT enabled is

            foreach (Company cp in trader.InterestedCompanies)
            {
                MarketWatchers.Add(new LiquidityMarketWatcher(this, cp, trader));
            }

        }

        public float timeintval = 60;
        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO
            timeintval = ActionTimeDeduction * 2.0f * (float)Master.rn.NextDouble();
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MarketWatchers[i].UpdateInsights();
            }
            trader.ActionTime -= timeintval;
            return MR;
        }
    }
}
