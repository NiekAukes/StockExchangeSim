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

        public List<Holder> holders = new List<Holder>();

        public InvestorStrategy(Trader t)
        {
            t.money += 100000;
            foreach (Company cp in t.InterestedCompanies)
            {
                MarketWatchers.Add(new ComparisonMarketWatcher(cp));
                Holder hd = new Holder(cp, t);
                hd.bidask.Bid = 1000000000;
                hd.bidask.Ask = 0;
                hd.MaxStockLimit = 0;
                Master.inst.exchange.RegisterHolder(hd);
                holders.Add(hd);
            }
        }

        public override Trader.MarketResults StrategyOutcome(Trader trader, ECNBroker exchange)
        {
            //TODO
            Trader.MarketResults MR = new Trader.MarketResults();
            for (int i = 0; i < MarketWatchers.Count; i++)
            {
                MR.Results.Add(new Tuple<Company, float>(MarketWatchers[i].cp, MarketWatchers[i].UpdateInsights()));
            }

            foreach(var result in MR.Results)
            {
                int comp = trader.InterestedCompanies.IndexOf(result.Item1);
                if (result.Item2 > 0)
                {
                    //wants stocks
                    if (result.Item1.percentageSold < 40)
                    {
                        //make a deal with company
                        float percentage = trader.money * (result.Item2 * 0.02f) / (result.Item1.Value);
                        List<Stock> stocks = result.Item1.TradeStocks(percentage > 40 - result.Item1.percentageSold ? 40 - result.Item1.percentageSold : percentage, trader);
                        trader.stocks[comp].AddRange(stocks);
                    }
                    if (trader.stocks[comp].Count > 0)
                    {
                        //has stocks, do nothing
                        holders[comp].bidask.Bid = int.MaxValue;
                        holders[comp].bidask.Ask = result.Item1.stockprice;
                    }
                    else
                    {
                        //does not have stocks, buy some (10% max money investing)
                        int stockamount = (int)(trader.money * (result.Item2 * 0.02) / (result.Item1.stockprice));

                        holders[comp].bidask.Bid = int.MaxValue;
                        holders[comp].bidask.Ask = result.Item1.stockprice;

                        holders[comp].MaxStockLimit = stockamount;

                    }
                }
                else
                {
                    //does not want stocks
                    if (trader.stocks[comp].Count > 0)
                    {
                        //has stocks, sell them
                        int stockamount = 0;

                        holders[comp].bidask.Bid = result.Item1.stockprice / 1.01f;
                        holders[comp].bidask.Ask = int.MaxValue;

                        holders[comp].MaxStockLimit = stockamount;
                    }
                    else
                    {
                        //does not have stocks, do nothing
                        holders[comp].bidask.Bid = int.MaxValue;
                        holders[comp].bidask.Ask = int.MaxValue;
                    }
                }
            }

            trader.ActionTime -= (int)(100000 * Master.rn.NextDouble());
            return MR;
        }
    }
}
