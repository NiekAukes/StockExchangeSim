using System;
using System.Collections.Generic;

namespace Eco
{
    public class BreakoutStrategy : Trader.Strategy
    {
        public List<BreakoutMarketWatcher> MarketWatchers = new List<BreakoutMarketWatcher>();
        
        public BreakoutStrategy(Trader t)
        {
            trader = t;
        }

        public override void Init()
        {

            for (int i = 0; i < trader.InterestedCompanies.Count; i++)
            {
                //add marketwatcher
                MarketWatchers.Add(new BreakoutMarketWatcher(this, trader.InterestedCompanies[i]));

                //request stocks from companies
                //List<Stock> stocks = trader.InterestedCompanies[i].TradeStocks(
                //    0.01f * (float)Master.rn.NextDouble(), trader, true);
                //trader.stocks[i].AddRange(stocks);
            }
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO 
            //MarketWatchers[0].RedoneInsights += BreakoutStrategy_RedoneInsights; was just a test
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MarketWatchers[i].latestInsightResult = MarketWatchers[i].UpdateInsights();
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].latestInsightResult));
            }
            trader.ActionTime -= 1000 * Master.rn.NextDouble();
            return MR;
        }
    }
}
