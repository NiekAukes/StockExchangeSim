using System;
using System.Collections.Generic;

namespace Eco
{
    class TrendStrategy : Trader.Strategy
    {
        public List<TrendMarketWatcher> MarketWatchers = new List<TrendMarketWatcher>();

        public TrendStrategy(Trader t)
        {
            trader = t;

        }
        

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MarketWatchers[i].latestInsightResult = MarketWatchers[i].UpdateInsights();
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].latestInsightResult));
            }
            trader.ActionTime -= 1000 * Master.rn.NextDouble();
            return MR;
        }

        public override void Init()
        {
            for (int i = 0; i < trader.InterestedCompanies.Count; i++)
            {
                //add marketwatcher
                MarketWatchers.Add(new TrendMarketWatcher(this, trader.InterestedCompanies[i]));

                //request stocks from companies
            //    List<Stock> stocks = trader.InterestedCompanies[i].TradeStocks(
            //        0.01f * (float)Master.rn.NextDouble(), trader, true);
            //    trader.stocks[i].AddRange(stocks);
            }
        }
    }
}
